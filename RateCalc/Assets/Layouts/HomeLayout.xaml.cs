using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RateCalc.Assets.Layouts
{
    /// <summary>
    /// Interaction logic for HomeLayout.xaml
    /// </summary>
    public partial class HomeLayout : UserControl
    {
        public HomeLayout()
        {
            InitializeComponent();
            Loaded += HomeLayout_Loaded;
        }

        private void HomeLayout_Loaded(object sender, RoutedEventArgs e)
        {
            RateCalcOpening? _mainWindow = Application.Current.MainWindow as RateCalcOpening;
        }
    }
}
