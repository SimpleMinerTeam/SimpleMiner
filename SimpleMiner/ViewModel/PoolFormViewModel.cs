using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static SimpleCPUMiner.Consts;

namespace SimpleCPUMiner.ViewModel
{
    public class PoolFormViewModel : ViewModelBase
    {
        public PoolSettingsXml Pool { get; set; }
        public RelayCommand<Window> CancelCommand { get; private set; }
        public RelayCommand<Window> SaveCommand { get; private set; }
        public Action<PoolSettingsXml> AddPool;
        public Action<PoolSettingsXml> UpdatePoolList;
        public List<Coin> CoinList { get; set; }
        public Consts.Algorithm? SelectedAlgo { get; set; }
        private Coin _selectedCoin;

        public Coin SelectedCoin
        {
            get { return _selectedCoin; }
            set { _selectedCoin = value; SelectedAlgo = SelectedCoin.Algorithm; RaisePropertyChanged(nameof(SelectedAlgo)); }
        }

        public PoolFormViewModel()
        {
            CancelCommand = new RelayCommand<Window>(Cancel);
            SaveCommand = new RelayCommand<Window>(Save);
            CoinList = Consts.Coins;
        }

        public void RefreshUI()
        {
            RaisePropertyChanged(nameof(Pool));
        }

        private void Save(Window obj)
        {
            Pool.CoinType = SelectedCoin.CoinType;
            Pool.Algorithm = SelectedAlgo;
            StringBuilder error = new StringBuilder();

            if (String.IsNullOrEmpty(Pool.URL))
                error.AppendLine("Warning pool address is empty!");

            if (Pool.IsMain || Pool.IsFailOver)
            {
                try
                {
                    Dns.GetHostEntry(Pool.URL);
                }
                catch
                {
                    error.AppendLine("Warning pool address is incorrect or unreachable, it may cause application hang!");
                }
            }

            if (String.IsNullOrEmpty(Pool.Username))
                error.AppendLine("Warning wallet address is empty!");
            else
                Pool.Username = Utils.RemoveWhitespace(Pool.Username);

            if (Pool.Port<1)
                error.AppendLine("The port field has an invalid value!");

            if (String.IsNullOrEmpty(Pool.Name))
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
