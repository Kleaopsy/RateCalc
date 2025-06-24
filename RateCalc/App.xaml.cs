using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using MahApps.Metro;
using ThemeHelper;
using Functions;

namespace RateCalc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Window rateCalcOpening;
            string configPath = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";
            bool configExists = System.IO.File.Exists(configPath);
            if (configExists)
            {
                rateCalcOpening = new RateCalcOpening();
            }
            else
            {
                rateCalcOpening = new StartLangSelect();
            }
            rateCalcOpening.Show();
        }
    }

}
