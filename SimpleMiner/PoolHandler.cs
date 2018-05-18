using GalaSoft.MvvmLight.Messaging;
using SimpleCPUMiner.Messages;
using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (!File.Exists(Consts.PoolFilePath))
                {
                    if (File.Exists(Consts.PoolFilePathOld))
                    {
                        //ilyenkor kell migrálnunk
                        var poolzToLoad = Utils.DeSerializeObject<List<PoolSettings>>(Consts.PoolFilePathOld);
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

            poolList = Utils.XmlDeserialize<List<PoolSettingsXml>>(Consts.PoolFilePath);

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
                Algorithm = Algorithm.CryptoNightV7
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
                Algorithm = Algorithm.CryptoNight
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
                Name = "Sumokoin Pool",
                Website = "http://miner-coin.eu/sumokoin/",
                Algorithm = Algorithm.CryptoNightHeavy
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 4,
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
                Algorithm = Algorithm.CryptoNight
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 5,
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
                Algorithm = Algorithm.CryptoNightV7
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 6,
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
                Algorithm = Algorithm.CryptoNightV7
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 7,
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
                Algorithm = Algorithm.CryptoNightHeavy
            });

            Pools.Add(new PoolSettingsXml()
            {
                ID = 8,
                CoinType = CoinTypes.XHV,
                IsCPUPool = true,
                IsGPUPool = true,
                IsFailOver = false,
                IsMain = false,
                IsRemoveable = true,
                URL = "haven.miner.rocks",
                Port = 5555,
                Username = "",
                Password = DefaultSettings.Password,
                Name = "Miner.Rocks (XHV)",
                Website = "https://haven.miner.rocks/",
                Algorithm = Algorithm.CryptoNightHeavy
            });

            return Pools;
        }

    }
}
