using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tanks_Sever.tanks.main.procotol;
using Tanks_Sever.tanks.system.localization;
using Tanks_Sever.tanks.users.anticheat;
using Tanks_Sever.tanks.users.garage;
using Tanks_Sever.tanks.users.locations;

namespace Tanks_Sever.tanks.users
{
    [Table("users")]
    public class User
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("uid")]
        public long Id { get; set; }

        [Required]
        [Column("nickname", TypeName = "varchar(255)")]
        public string Nickname { get; set; }

        [Required]
        [Column("password", TypeName = "varchar(255)")]
        public string Password { get; set; }

        [Required]
        [Column("rank")]
        public int Rang { get; set; } = 0;

        [Required]
        [Column("score")]
        public int Score { get; set; } = 0;

        [Required]
        [Column("crystalls")]
        public int Crystall { get; set; } = 0;

        [Required]
        [Column("next_score")]
        public int NextScore { get; set; } = 100;

        [Required]
        [Column("place")]
        public int Place { get; set; } = 0;

        [Required]
        [Column("rating")]
        public int Rating { get; set; } = 1;

        [Column("email", TypeName = "varchar(255)")]
        public string Email { get; set; } = "default@gtanks.com";

        [Required]
        [Column("last_ip", TypeName = "varchar(45)")]
        public string LastIP { get; set; }

        [Required]
        [Column("user_type")]
        public TypeUser Type { get; set; }

        [Column("last_issue_bonus")]
        public DateTime? LastIssueBonus { get; set; }

        [NotMapped]
        private UserLocation UserLocation;

        [NotMapped]
        private Garage Garage;

        [NotMapped]
        public int Warnings { get; set; } = 0;

        [NotMapped]
        public AntiCheatData AntiCheatData { get; set; }

        [NotMapped]
        public Session Session { get; set; }

        [NotMapped]
        public LocalizationHandler.Localization Localization { get; set; }

        public User(string nickname, string password)
        {
            Type = TypeUser.DEFAULT;
            AntiCheatData = new AntiCheatData();
            Nickname = nickname;
            Password = password;
            Garage = new Garage();
        }

        public User()
        {
            Type = TypeUser.DEFAULT;
            AntiCheatData = new AntiCheatData();
        }

        public string GetNickname()
        {
            return Nickname;
        }

        public void SetNickname(string nickname)
        {
            Nickname = nickname;
        }

        public string GetPassword()
        {
            return Password;
        }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public int GetRang()
        {
            return Rang;
        }

        public void SetRang(int rang)
        {
            Rang = rang;
        }

        public int GetScore()
        {
            return Score;
        }

        public void SetScore(int score)
        {
            Score = score;
        }

        public void AddScore(int score)
        {
            Score += score;
        }

        public int GetCrystall()
        {
            return Crystall;
        }

        public void SetCrystall(int crystall)
        {
            Crystall = crystall;
        }

        public void AddCrystall(int crystall)
        {
            Crystall += crystall;
        }

        public int GetNextScore()
        {
            return NextScore;
        }

        public void SetNextScore(int nextScore)
        {
            NextScore = nextScore;
        }

        public int GetPlace()
        {
            return Place;
        }

        public void SetPlace(int place)
        {
            Place = place;
        }

        public int GetRating()
        {
            return Rating;
        }

        public void SetRating(int rating)
        {
            Rating = rating;
        }

        public Garage GetGarage() 
        {
            return Garage;
        }

        public void SetGarage(Garage garage) 
        { 
            Garage = garage;
        }

        public string GetEmail()
        {
            return Email;
        }

        public void SetEmail(string email)
        {
            Email = email;
        }

        public TypeUser GetType()
        {
            return Type;
        }

        public void SetType(TypeUser type)
        {
            Type = type;
        }

        public UserLocation getUserLocation() 
        {
            return UserLocation;
        }

        public void setUserLocation(UserLocation userLocation) 
        { 
            UserLocation = userLocation;
        }

        public int GetWarnings()
        {
            return Warnings;
        }

        public void SetWarnings(int warnings)
        {
            Warnings = warnings;
        }

        public void AddWarning()
        {
            Warnings++;
        }

        public AntiCheatData getAntiCheatData()
        {
            return AntiCheatData;
        }

        public void setAntiCheatData(AntiCheatData antiCheatData)
        {
            AntiCheatData = antiCheatData;
        }

        public String getLastIP()
        {
            return LastIP;
        }

        public void setLastIP(String lastIP)
        {
            LastIP = lastIP;
        }

        public string ToString()
        {
            return $"{Rang} {Nickname} {Password}";
        }

        public LocalizationHandler.Localization GetLocalization()
        {
            return Localization;
        }

        public void SetLocalization(LocalizationHandler.Localization localization)
        {
            Localization = localization;
        }
    }
}
