﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Ultz.BeagleFramework.Common.Models;

namespace SIVA.Core.JsonFiles
{
    public static class DataStorage
    {
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static bool SaveExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }

    public class UserAccount : DataModel
    {
        /*public UserAccount()
        {
            Warns = new Dictionary<ulong, string>();
        }*/

        public ulong Id { get; set; }

        public uint Xp { get; set; }

        public uint LevelNumber => (uint) Math.Sqrt(Xp / 50);

        //public Dictionary<ulong, string> Warns { get; set; }

        //public uint WarnCount { get; set; }

        public int Money { get; set; }
    }

    public static class UserAccounts
    {
        private static readonly List<UserAccount> accounts;

        private static readonly string accountsFile = "data/UAccounts.json";

        static UserAccounts()
        {
            if (DataStorage.SaveExists(accountsFile))
            {
                accounts = DataStorage.LoadUserAccounts(accountsFile).ToList();
            }
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            DataStorage.SaveUserAccounts(accounts, accountsFile);
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }

        private static UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in accounts
                where a.Id == id
                select a;

            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount
            {
                Id = id,
                Xp = 5,
                Money = 0
            };
            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}