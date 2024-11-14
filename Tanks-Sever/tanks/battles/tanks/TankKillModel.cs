using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.lobby.battles;
using Tanks_Sever.tanks.services;
using Tanks_Sever.tanks.battles.tanks.data;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.utils;
using Tanks_Sever.tanks.battles.effect;

namespace Tanks_Sever.tanks.battles.tanks
{
    public class TankKillModel
    {
        private const int ExperienceForKill = 10;
        private const int TimeToRestartBattle = 10000;
        private readonly BattlefieldModel _bfModel;
        private double _battleFund;
        private readonly BattleInfo _battleInfo;
        private LobbysServices lobbysServices = LobbysServices.Instance;

        public TankKillModel(BattlefieldModel bfModel)
        {
            _bfModel = bfModel;
            _battleInfo = bfModel.battleInfo;
        }

        public void DamageTank(BattlefieldPlayerController controller, BattlefieldPlayerController damager, float damage, bool considerDD)
        {
            if (controller == null || damager == null) return;

            var tank = controller.tank;
            if (tank.State.Equals("newcome") || tank.State.Equals("suicide")) return;

            if (!_battleInfo.Team || controller == damager || !controller.playerTeamType.Equals(damager.playerTeamType) || _battleInfo.FriendlyFire)
            {
                var resistance = controller.tank.GetColormap().GetResistance(damager.tank.GetWeapon().GetEntity().GetType());
                damage = WeaponUtils.CalculateDamageWithResistance(damage, resistance);
                if (tank.IsUsedEffect(EffectType.ARMOR))
                {
                    damage /= 2.0f;
                }

                if (damager.tank.IsUsedEffect(EffectType.DAMAGE) && considerDD)
                {
                    damage *= 2.0f;
                }

                var lastDamage = tank.LastDamagers.TryGetValue(damager, out var existingDamageData) ? existingDamageData : null;
                var damageData = new DamageTankData
                {
                    Damage = damage,
                    TimeDamage = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Damager = damager
                };

                if (lastDamage != null)
                {
                    damageData.Damage += lastDamage.Damage;
                }

                if (damager.tank.IsUsedEffect(EffectType.DAMAGE) && considerDD)
                {
                    damageData.Damage /= 2.0f;
                }

                if (controller != damager)
                {
                    tank.LastDamagers[damager] = damageData;
                }

                tank.Health -= WeaponUtils.CalculateHealth(tank, damage);
                ChangeHealth(tank, tank.Health);
                if (tank.Health <= 0)
                {
                    tank.Health = 0;
                    KillTank(controller, damager);
                }
            }
        }

        public bool HealPlayer(BattlefieldPlayerController healer, BattlefieldPlayerController target, float addHeal)
        {
            var targetTank = target.tank;
            if (targetTank.State.Equals("newcome") || targetTank.State.Equals("suicide")) return false;

            if (targetTank.Health >= 10000) return false;

            targetTank.Health += WeaponUtils.CalculateHealth(targetTank, addHeal);
            if (targetTank.Health >= 10000)
            {
                targetTank.Health = 10000;
            }

            ChangeHealth(targetTank, targetTank.Health);
            return true;
        }

        public void ChangeHealth(Tank tank, int value)
        {
            if (tank == null) return;

            tank.Health = value;
            _bfModel.SendToAllPlayers(CommandType.BATTLE, "change_health", tank.Id, tank.Health.ToString());
        }

        public void KillTank(BattlefieldPlayerController controller, BattlefieldPlayerController killer)
        {
            var tank = controller.tank;
            tank.State = "suicide";
            tank.GetWeapon().StopFire();
            controller.ClearEffects();
            controller.Send(CommandType.BATTLE, "local_user_killed");

            if (killer == null)
            {
                _bfModel.SendToAllPlayers(CommandType.BATTLE, "kill_tank", tank.Id, "suicide");
            }
            else
            {
                HandleTankKill(controller, killer);
            }

            _bfModel.statistics.ChangeStatistic(controller);
            _bfModel.RespawnPlayer(controller, false);
            controller.tank.LastDamagers.Clear();
        }

        private void HandleTankKill(BattlefieldPlayerController controller, BattlefieldPlayerController killer)
        {
            _bfModel.SendToAllPlayers(CommandType.BATTLE, "kill_tank", controller.tank.Id, "killed", killer.tank.Id);
            if (controller == killer)
            {
                controller.statistic.AddDeaths();
                _bfModel.statistics.ChangeStatistic(controller);
            }
            else
            {
                HandleTeamKill(controller, killer);
                HandleScoreAndFund(controller, killer);
            }

            if (_battleInfo.NumKills > 0 && killer.statistic.Kills >= _battleInfo.NumKills)
            {
                RestartBattle(false);
            }

            if (controller.flag != null)
            {
                _bfModel.ctfModel.DropFlag(controller, controller.tank.Position);
            }
        }

        private void HandleTeamKill(BattlefieldPlayerController controller, BattlefieldPlayerController killer)
        {
            if (_battleInfo.Team)
            {
                if (controller.playerTeamType.Equals(killer.playerTeamType))
                {
                    if (_battleInfo.FriendlyFire)
                    {
                        controller.statistic.AddDeaths();
                        _bfModel.statistics.ChangeStatistic(controller);
                    }
                }
                else
                {
                    killer.statistic.AddKills(false);
                    controller.statistic.AddDeaths();
                    UpdateTeamScores(killer);
                }
            }
            else
            {
                killer.statistic.AddKills(true);
                controller.statistic.AddDeaths();
            }
        }

        private void UpdateTeamScores(BattlefieldPlayerController killer)
        {
            if (killer.playerTeamType.Equals("BLUE"))
            {
                if (!_battleInfo.BattleType.Equals("CTF"))
                {
                    _battleInfo.ScoreBlue++;
                }

                _bfModel.SendToAllPlayers(CommandType.BATTLE, "change_team_scores", "BLUE", _battleInfo.ScoreBlue.ToString());
            }
            else if (killer.playerTeamType.Equals("RED"))
            {
                if (!_battleInfo.BattleType.Equals("CTF"))
                {
                    _battleInfo.ScoreRed++;
                }

                _bfModel.SendToAllPlayers(CommandType.BATTLE, "change_team_scores", "RED", _battleInfo.ScoreRed.ToString());
            }
        }

        private void HandleScoreAndFund(BattlefieldPlayerController controller, BattlefieldPlayerController killer)
        {
            if (_battleInfo.Team)
            {
                var lastDamagers = controller.tank.LastDamagers;
                if (lastDamagers.Count() <= 1)
                {
                    AddScore(killer, 10);
                }
                else
                {
                    HandleMultipleDamagers(lastDamagers);
                }
            }
            else
            {
                var killScore = controller.flag != null ? 20 : 10;
                AddScore(killer, killScore);
            }

            AddFund(0.037 * (killer.GetUser().GetRang() + 1) + 0.01);
        }

        private void HandleMultipleDamagers(Dictionary<BattlefieldPlayerController, DamageTankData> lastDamagers)
        {
            var damagers = new List<DamageTankData>(lastDamagers.Values);
            var damager1 = damagers[^1];
            var damager2 = damagers[^2];
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (currentTime - damager1.TimeDamage <= 10000L && currentTime - damager2.TimeDamage <= 10000L)
            {
                CalculateSharedScore(damager1, damager2);
            }
            else
            {
                AddScore(damager1.Damager, 10);
            }
        }

        private void CalculateSharedScore(DamageTankData damager1, DamageTankData damager2)
        {
            int score1, score2;
            if (damager1.Damage > damager2.Damage)
            {
                score1 = (int)(0.15 * (100.0f / (damager1.Damager.tank.GetHull().Hp / damager1.Damage)));
                score2 = 15 - score1;
            }
            else if (damager2.Damage > damager1.Damage)
            {
                score2 = (int)(0.15 * (100.0f / (damager2.Damager.tank.GetHull().Hp / damager2.Damage)));
                score1 = 15 - score2;
            }
            else
            {
                score1 = score2 = 7;
            }

            score1 = Math.Abs(score1);
            score2 = Math.Abs(score2);
            AddScore(damager1.Damager, score1);
            AddScore(damager2.Damager, score2);
        }

        private void AddScore(BattlefieldPlayerController player, int score)
        {
            //player.playerTeamType.AddScore(score);
            _bfModel.statistics.ChangeStatistic(player);
            TanksServices.getInstance().AddScore(player.parentLobby, score);
        }

        public void RestartBattle(bool timeLimitFinish)
        {
            CalculatePrizes();
            _bfModel.BattleFinish();
            _bfModel.SendToAllPlayers(CommandType.BATTLE, "battle_finish", JSONUtils.ParseFinishBattle(_bfModel.Players, TimeToRestartBattle));
            Task.Run(async () =>
            {
                await Task.Delay(10000);
                _bfModel.BattleRestart();
            });
        }

        private void CalculatePrizes()
        {
            if (_bfModel.Players == null || _bfModel.Players.Count == 0) return;

            var users = new List<BattlefieldPlayerController>(_bfModel.Players.Values);
            if (!_battleInfo.Team)
            {
                users.Sort((p1, p2) => p2.statistic.Score.CompareTo(p1.statistic.Score));
                PrizeCalculator(users, 3);
            }
            else
            {
                PrizeCalculator(users, 2);
            }
        }

        private void PrizeCalculator(List<BattlefieldPlayerController> users, int maxUsersToPrize)
        {
            for (var i = 0; i < Math.Min(users.Count(), maxUsersToPrize); i++)
            {
                var score = users[i].statistic.Score;
                var prizeMultiplier = i switch
                {
                    0 => 0.5,
                    1 => 0.3,
                    2 => 0.2,
                    _ => 0
                };

                if (score > 0)
                {
                    AddScore(users[i], (int)(_battleFund * prizeMultiplier));
                }
            }
        }

        public void AddFund(double addBattleFund)
        {
            _battleFund += addBattleFund;
            _bfModel.SendToAllPlayers(CommandType.BATTLE, "change_fund", _battleFund.ToString("F2"));
        }

        public double GetBattleFund()
        {
            return _battleFund;
        }

        public void SetBattleFund(int value)
        {
            _battleFund = value;
        }
    }
}