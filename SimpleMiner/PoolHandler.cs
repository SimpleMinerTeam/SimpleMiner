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
                            poolList.Add(new PoolSettingsXml() {
                                ID =  oldPool.ID,
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
                Password = DefaultSettings.Password
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
                Password = DefaultSettings.Password
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
                Password = DefaultSettings.Password
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
                Port = 3333,
                Username = "",
                Password = DefaultSettings.Password
            });

            return Pools;
        }

    }
}
