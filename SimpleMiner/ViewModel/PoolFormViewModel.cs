using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using static SimpleCPUMiner.Consts;

namespace SimpleCPUMiner.ViewModel
{
    public class PoolFormViewModel : ViewModelBase
    {
        public PoolSettingsXmlUI Pool { get; set; }
        public RelayCommand<Window> CancelCommand { get; private set; }
        public RelayCommand<Window> SaveCommand { get; private set; }
        public Action<PoolSettingsXmlUI> AddPool;
        public Action<PoolSettingsXmlUI> UpdatePoolList;
        public List<Coin> CoinList { get; set; }
        public List<Algo> Algorithms { get; set; }
        private Algo _selectedAlgo { get; set; }
        private Coin _selectedCoin;

        public Coin SelectedCoin
        {
            get { return _selectedCoin; }
            set
            {
                _selectedCoin = value;

                if (_selectedCoin == null)
                {
                    _selectedCoin = new Coin
                    {
                        CoinType = CoinTypes.OTHER,
                        Icon = "coinOther.png",
                        ShortName = "OTHER"
                    };

                    SelectedAlgo = Algorithms.Where(x => x.ID == (int)SupportedAlgos.CryptoNight).FirstOrDefault();
                }
                else
                {
                    SelectedAlgo = Algorithms.Where(x => x.ID == SelectedCoin.Algorithm).FirstOrDefault();
                }

                RaisePropertyChanged(nameof(SelectedAlgo));
            }
        }

        public Algo SelectedAlgo
        {
            get { return _selectedAlgo; }

            set
            {
                _selectedAlgo = value;

                if (!_selectedAlgo.IsCpuSupport)
                {
                    Pool.IsCPUPool = false;
                }
                if (!_selectedAlgo.IsGpuSupport)
                {
                    Pool.IsGPUPool = false;
                }

                RaisePropertyChanged(nameof(SelectedAlgo));
            }
        }

        public PoolFormViewModel()
        {
            CancelCommand = new RelayCommand<Window>(Cancel);
            SaveCommand = new RelayCommand<Window>(Save);
            CoinList = Coins;
            Algorithms = Consts.Algorithms;
        }

        public void RefreshUI()
        {
            RaisePropertyChanged(nameof(Pool));
        }

        private void Save(Window obj)
        {
            Pool.CoinType = SelectedCoin.CoinType;
            Pool.Algorithm = SelectedAlgo.ID.ToString();
            StringBuilder error = new StringBuilder();

            if (string.IsNullOrEmpty(Pool.URL))
                error.AppendLine("Warning! Pool address is empty!");

            if (Pool.IsMain || Pool.IsFailOver)
            {
                try
                {
                    Dns.GetHostEntry(Pool.URL);
                }
                catch
                {
                    error.AppendLine("Warning! Pool address is incorrect or unreachable, it may cause application hang!");
                }
            }

            if (string.IsNullOrEmpty(Pool.Username))
                error.AppendLine("Warning! Wallet address is empty!");
            else
                Pool.Username = Utils.RemoveWhitespace(Pool.Username);

            if (Pool.Port<1)
                error.AppendLine("The port field has an invalid value!");

            if (string.IsNullOrEmpty(Pool.Name))
                Pool.Name = Pool.URL;

            if(error.Length>0)
            {
                error.Insert(0, $"The following issue(s) found during save:{Environment.NewLine}");
                error.AppendLine(Environment.NewLine);
                error.AppendLine("Do you still want to save the profile?");
                var dr = MessageBox.Show(error.ToString(), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (dr == MessageBoxResult.No)
                    return;
            }


            if (AddPool != null)
            {
                AddPool(Pool);
            }
            else
            {
                UpdatePoolList(Pool);
            }
            obj.Close();
        }

        private void Cancel(Window obj)
        {
            obj.Close();
        }

        internal void UpdateCoinType()
        {
            SelectedCoin = CoinList.Where(x => x.CoinType.Equals(Pool.CoinType)).FirstOrDefault();
        }
    }
}
