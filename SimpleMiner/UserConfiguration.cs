using System;
using System.Collections.Generic;

namespace SimpleCPUMiner
{
    [Serializable]
    public class UserConfiguration
    {
        private int selectedConfig;
        private List<MinerSettings> settingsList = new List<MinerSettings>();

        public int SelectedConfigIndex
        {
            get
            {
                return selectedConfig;
            }
            set
            {
                selectedConfig = value;
            }
        }

        public List<MinerSettings> SettingsList
        {
            get
            {
                return settingsList;
            }
            set
            {
                settingsList = value;
            }
        }
    }
}
