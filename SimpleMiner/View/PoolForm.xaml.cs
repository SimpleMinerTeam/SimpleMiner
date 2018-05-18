using SimpleCPUMiner.ViewModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SimpleCPUMiner.View
{
    /// <summary>
    /// Interaction logic for PoolForm.xaml
    /// </summary>
    public partial class PoolForm : Window
    {
        public PoolForm()
        {
            InitializeComponent();
        }

        private void PortPrevTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            cbFailOver.IsChecked = false;
            tbOrder.Text = "0";
            var vm = DataContext as PoolFormViewModel;
            vm.RefreshUI();
        }

        private void cbFailOver_Checked(object sender, RoutedEventArgs e)
        {
            cbMain.IsChecked = false;
            var vm = DataContext as PoolFormViewModel;
            vm.RefreshUI();
        }
    }
}
