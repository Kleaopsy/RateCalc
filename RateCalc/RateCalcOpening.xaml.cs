using Functions;
using MahApps.Metro.Controls;
using RateCalc.Assets.Layouts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace RateCalc
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 
    public partial class RateCalcOpening : MetroWindow
    {
        public String _lang;
        HomeLayout _homeLayout = new Assets.Layouts.HomeLayout();
        BillingLayout _billingLayout = new Assets.Layouts.BillingLayout();
        SettingsLayout _settingsLayout = new Assets.Layouts.SettingsLayout();
        public RateCalcOpening()
        {
            InitializeComponent();
            SettingsFunctions.Control();
            _lang = SettingsFunctions.ControlLang();
            SettingsFunctions.ChangeLanguage(_lang);

            homeBtn.Tag = "active";
            MainContent.Content = _homeLayout;
        }
        private void closeBtn_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            TaskFunctions.CloseBtn(sender, e);
        }
        private void minBtn_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            TaskFunctions.MinBtn(sender, e);
        }
        private void maxBtn_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            TaskFunctions.MaxBtn(sender, e);
        }
        private void taskBarDrag_Down(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TaskFunctions.DragWindow(sender, e);
        }

        public static object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
                return d / 2;
            return value;
        }

        private void pageChange_Clicked(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag as string != "active")
            {
                homeBtn.Tag = "";
                homeBtn.Cursor = Cursors.Hand;

                billinBtn.Tag = "";
                billinBtn.Cursor = Cursors.Hand;

                settingsBtn.Tag = "";
                settingsBtn.Cursor = Cursors.Hand;

                (sender as Button)?.Tag = "active";
                (sender as Button)?.Cursor = Cursors.Arrow;
                switch ((sender as Button)?.Name)
                {
                    case "homeBtn":
                        MainContent.Content = _homeLayout;
                        break;
                    case "billinBtn":
                        MainContent.Content = _billingLayout;
                        break;
                    case "settingsBtn":
                        MainContent.Content = _settingsLayout;
                        break;
                    default:
                        MainContent.Content = _homeLayout;
                        break;
                }
            }
        }
    }
}
