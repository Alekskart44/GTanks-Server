using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.json;
using Tanks_Sever.tanks.users.garage.items;
using Tanks_Sever.tanks.battles.effect;
using Tanks_Sever.tanks.battles.tanks.math;
using Tanks_Sever.tanks.main.procotol.commands;
using Tanks_Sever.tanks.battles.inventory.effects;
using Repository;

namespace Tanks_Sever.tanks.battles.effect.controller
{
    public class InventoryController
    {
        private const string INIT_INVENTORY_COMMAND = "init_inventory";
        private const string ACTIVATE_ITEM_COMMAND = "activate_item";
        private const string ENABLE_EFFECT_COMMAND = "enable_effect";

        private BattlefieldPlayerController player;

        public InventoryController(BattlefieldPlayerController player)
        {
            this.player = player;
        }

        public void Init()
        {
            player.Send(CommandType.BATTLE, INIT_INVENTORY_COMMAND, JSONUtils.ParseInitInventoryCommand(player.GetGarage()));
        }

        public void ActivateItem(string id, Vector3 tankPos)
        {
            Item item = player.GetGarage().GetItemById(id);
            if (item != null && item.Count >= 1)
            {
                Effect effect = GetEffectById(id);
                if (!player.tank.IsUsedEffect(effect.GetEffectType()))
                {
                    effect.Activate(player, true, tankPos);
                    OnActivatedItem(item, effect.GetDurationTime());
                    item.Count--;

                    if (item.Count <= 0)
                    {
                        player.GetGarage().Items.Remove(item);
                    }

                    new Thread(() =>
                    {
                        player.GetGarage().ParseJSONData();
                        GenericManager.repository.Update(player.GetGarage());
                    }).Start();
                }
            }
        }

        private void OnActivatedItem(Item item, int durationTime)
        {
            player.Send(CommandType.BATTLE, ACTIVATE_ITEM_COMMAND, item.Id);
            player.battleModel.SendToAllPlayers(CommandType.BATTLE, ENABLE_EFFECT_COMMAND, player.GetUser().GetNickname(), item.Index.ToString(), durationTime.ToString());
        }

        private Effect GetEffectById(string id)
        {
            Effect effect = null;

            switch (id)
            {
                case "health":
                    effect = new HealthEffect();
                    break;
                case "double_damage":
                    effect = new DamageEffect();
                    break;
                case "n2o":
                    effect = new NitroEffect();
                    break;
                case "mine":
                    effect = new Mine();
                    break;
                case "armor":
                    effect = new ArmorEffect();
                    break;
                default:
                    Console.WriteLine("Effect with id: " + id + " not found!");
                    break;
            }

            return effect;
        }
    }
}