using GalaSoft.MvvmLight.Messaging;
using SimpleCPUMiner.Messages;
using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.IO;
using static SimpleCPUMiner.Consts;

namespace SimpleCPUMiner
{
    public static class PoolHandler
    {
        internal static List<PoolSettingsXml> GetPools()
        {
            List<PoolSettingsXml> poolList = null;

            try
            {
                if (!File.Exists(PoolFilePath))
                {
                    if (File.Exists(PoolFilePathOld))
                    {
                        //ilyenkor kell migrálnunk
                        var poolzToLoad = Utils.DeSerializeObject<List<PoolSettings>>(PoolFilePathOld);
                        if (poolzToLoad != null && poolzToLoad.Count > 0)
                        {
                            poolList = new List<PoolSettingsXml>();
                            foreach (var oldPool in poolzToLoad)
                            {
                                poolList.Add(new PoolSettingsXml()
                                {
                                    ID = oldPool.ID,
                                    CoinType = oldPool.CoinType,
                                    FailOverPriority = oldPool.FailOverPriority,
                                    IsCPUPool = oldPool.IsCPUPool,
                                    IsGPUPool = oldPool.IsGPUPool,
                                    IsFailOver = oldPool.IsFailOver,
                                    IsMain = oldPool.IsMain,
                                    Password = oldPool.Password,
                                    Port = oldPool.Port,
                                    Name = oldPool.URL,
                                    URL = oldPool.URL,
                                    Username = oldPool.Username
                                });
                            }
                        }
                    }
                    else
                    {
                        poolList = setDefaultPool();
                        IsGenerateDefaultPoolList = true;
                    }
                }
                else
                {
                    poolList = LoadPools();
                }
            }
            catch(Exception ex)
            {
                Messenger.Default.Send<MinerOutputMessage>(new MinerOutputMessage() { OutputText = $"Failed to load pools, {ex.Message}{Environment.NewLine}{ex.InnerException}{Environment.NewLine}{ex.StackTrace}", IsError = true });
                poolList = null;
            }

            if(poolList==null)
                poolList = setDefaultPool();

            return poolList;
        }

        internal static void SavePools(List<PoolSettingsXml> pPools)
        {
            Utils.XmlSerialize(PoolFilePath, pPools);
        }

        internal static List<PoolSettingsXml> LoadPools()
        {
            List<PoolSettingsXml> poolList = new List<PoolSettingsXml>();

            poolList = Utils.XmlDeserialize<List<PoolSettingsXml>>(PoolFilePath);

            return poolList;
        }

        /// <summary>
        /// Sets the default pool.
        /// </summary>
        private static List<PoolSettingsXml> setDefaultPool()
        {
            var Pools = new List<PoolSettingsXml>();

            Pools.Add(new PoolSettingsXml()
            {
                ID = 0,
                CoinType = CoinTypes.XMR,
                IsCPUPool = true,
                IsGPUPool = true,
                IsFailOver = false,
                IsMain = true,
                IsRemoveable = true,
                URL = "pool.supportxmr.com",
                Port = 5555,
                Username = "",
                Password = DefaultSettings.Password,
                Name = "supportXMR",
                Website = "https://supportxmr.com/#/home",
                Algorithm = ((int)SupportedAlgos.CryptoNight_V8).ToString()
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 1,
                CoinType = CoinTypes.ETN,
                IsCPUPool = true,
                IsGPUPool = true,
                IsFailOver = false,
                IsMain = false,
                IsRemoveable = true,
                URL = "etn-pool.proxpool.com",
                Port = 5555,
                Username = "",
                Password = DefaultSettings.Password,
                Name = "Proxpool (ETN)",
                Website = "http://etn.proxpool.com/",
                Algorithm = ((int)SupportedAlgos.CryptoNight).ToString()
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 2,
                CoinType = CoinTypes.SUMO,
                IsCPUPool = true,
                IsGPUPool = true,
                IsFailOver = false,
                IsMain = false,
                IsRemoveable = true,
                URL = "pool.miner-coin.eu",
                Port = 4444,
                Username = "",
                Password = DefaultSettings.Password,
                Name = "Ryo Pool",
                Website = "http://miner-coin.eu/sumokoin/",
                Algorithm = ((int)SupportedAlgos.CryptoNight_Heavy).ToString()
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 3,
                CoinType = CoinTypes.KRB,
                IsCPUPool = true,
                IsGPUPool = true,
                IsFailOver = false,
                IsMain = false,
                IsRemoveable = true,
                URL = "krb.miner.rocks",
                Port = 5555,
                Username = "",
                Password = DefaultSettings.Password,
                Name = "Miner.Rocks (KRB)",
                Website = "https://krb.miner.rocks/",
                Algorithm = ((int)SupportedAlgos.CryptoNight).ToString()
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 4,
                CoinType = CoinTypes.GRFT,
                IsCPUPool = true,
                IsGPUPool = true,
                IsFailOver = false,
                IsMain = false,
                IsRemoveable = true,
                URL = "graft.miner.rocks",
                Port = 5555,
                Username = "",
                Password = DefaultSettings.Password,
                Name = "Miner.Rocks (GRFT)",
                Website = "https://graft.miner.rocks/",
                Algorithm = ((int)SupportedAlgos.CryptoNight_V7).ToString()
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 5,
                CoinType = CoinTypes.XTL,
                IsCPUPool = true,
                IsGPUPool = true,
                IsFailOver = false,
                IsMain = false,
                IsRemoveable = true,
                URL = "stellite.miner.rocks",
                Port = 5555,
                Username = "",
                Password = DefaultSettings.Password,
                Name = "Miner.Rocks (XTL)",
                Website = "https://stellite.miner.rocks/",
                Algorithm = ((int)SupportedAlgos.CryptoNight_Stellite_V4).ToString()
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 6,
                CoinType = CoinTypes.LOKI,
                IsCPUPool = true,
                IsGPUPool = true,
                IsFailOver = false,
                IsMain = false,
                IsRemoveable = true,
                URL = "loki.miner.rocks",
                Port = 5555,
                Username = "",
                Password = DefaultSettings.Password,
                Name = "Miner.Rocks (LOKI)",
                Website = "https://loki.miner.rocks/",
                Algorithm = ((int)SupportedAlgos.CryptoNight_Heavy).ToString()
            });

            return Pools;
        }
    }
}
