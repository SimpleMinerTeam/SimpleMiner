using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }

        private void cbFailOver_Checked(object sender, RoutedEventArgs e)
        {
            cbMain.IsChecked = false;
        }
    }
}
