using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SimpleCPUMiner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static SimpleCPUMiner.Consts;

namespace SimpleCPUMiner.ViewModel
{
    public class PoolFormViewModel : ViewModelBase
    {
        public PoolSettings Pool { get; set; }
        public RelayCommand<Window> CancelCommand { get; private set; }
        public RelayCommand<Window> SaveCommand { get; private set; }
        public Action<PoolSettings> AddPool;
        public Action<PoolSettings> UpdatePoolList;
        public List<Coinz> CoinList { get; set; }
        public Coinz SelectedCoin { get; set; }

        public class Coinz
        {
            public CoinTypes Type { get; set; }
            public String Name { get; set; }          
        }

        public PoolFormViewModel()
        {
            CancelCommand = new RelayCommand<Window>(Cancel);
            SaveCommand = new RelayCommand<Window>(Save);
            CoinList = new List<Coinz>() {
                new Coinz() { Name = "Other", Type = CoinTypes.OTHER },
                new Coinz() { Name = "BCN", Type = CoinTypes.BCN },
                new Coinz() { Name = "ETN", Type = CoinTypes.ETN },
                new Coinz() { Name = "KRB", Type = CoinTypes.KRB },
                new Coinz() { Name = "SUMO", Type = CoinTypes.SUMO },
                new Coinz() { Name = "XMR", Type = CoinTypes.XMR }
            };

        }

        private void Save(Window obj)
        {
            Pool.CoinType = SelectedCoin.Type;

            StringBuilder error = new StringBuilder();

            if (String.IsNullOrEmpty(Pool.URL))
                error.AppendLine("Warning pool address is empty!");

            if (String.IsNullOrEmpty(Pool.Username))
                error.AppendLine("Warning wallet address is empty!");

            if (Pool.Port<1)
                error.AppendLine("The port field has an invalid value!");

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
            SelectedCoin = CoinList.Where(x => x.Type.Equals(Pool.CoinType)).FirstOrDefault();
        }
    }
}
