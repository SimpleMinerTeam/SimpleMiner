using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;
using static SimpleCPUMiner.Consts;

namespace SimpleCPUMiner.Model
{
    [Serializable]
    public class PoolSettingsXmlUI : PoolSettingsXml
    {
        [XmlIgnore]
        public Brush BackgroundBrush { get; set; }

        private bool _isActiveCPUPool;

        [XmlIgnore]
        public bool IsActiveCPUPool
        {
            get { return _isActiveCPUPool; }
            set
            {
                _isActiveCPUPool = value;

                if (IsActiveCPUPool && IsActiveGPUPool)
                    BackgroundBrush = PoolListBackgrounds.Purple;
                else if(IsActiveCPUPool)
                    BackgroundBrush = PoolListBackgrounds.Orange;
                else
                {
                    BackgroundBrush = PoolListBackgrounds.Transparent;
                }

                NotifyPropertyChanged(nameof(BackgroundBrush));
            }
        }

        private bool _isActiveGPUPool;

        [XmlIgnore]
        public bool IsActiveGPUPool
        {
            get { return _isActiveGPUPool; }
            set
            {
                _isActiveGPUPool = value;
                if (IsActiveCPUPool && IsActiveGPUPool)
                    BackgroundBrush = PoolListBackgrounds.Purple;
                else if (IsActiveGPUPool)
                    BackgroundBrush = PoolListBackgrounds.Green;
                else
                    BackgroundBrush = PoolListBackgrounds.Transparent;

                NotifyPropertyChanged(nameof(BackgroundBrush));
            }
        }
    }
}
