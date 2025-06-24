using ControlzEx.Theming;
using Functions;
using MahApps.Metro.Controls;
using System.Configuration;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RateCalc
{
    public partial class StartLangSelect : MetroWindow
    {
        public StartLangSelect()
        {
            InitializeComponent();
            SettingsFunctions.Control();

            bool isDark = ThemeHelper.ThemeHelper.IsWindowsInDarkMode();
            string themeName = isDark ? "Dark.Blue" : "Light.Blue";
            ThemeManager.Current.ChangeTheme(this, themeName);

            if (isDark)
            {
                this.Resources["MahApps.Brushes.GlowAccent"] = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
                this.Resources["MahApps.Brushes.GlowInactive"] = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60)); // Daha koyu gri inaktif glow
            }
            else
            {
                this.Resources["MahApps.Brushes.GlowAccent"] = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180));
                this.Resources["MahApps.Brushes.GlowInactive"] = new SolidColorBrush(Color.FromArgb(255, 220, 220, 220)); // Açık gri inaktif glow
            }
        }
        private void closeBtn_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            TaskFunctions.CloseBtn(sender, e);
        }
        private void minBtn_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            TaskFunctions.MinBtn(sender, e);
        }
        private void taskBarDrag_Down(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TaskFunctions.DragWindow(sender, e);
        }

        private Button? _selectedLangBtn;
        string _selectedLangBtnText = "", _selectedLangBtnTextOr = "";
        bool _isLangSelected = false;
        private void langButton_Clicked(object sender, RoutedEventArgs e)
        {
            var clickedBtn = sender as Button;
            if (clickedBtn == null) return;

            if ((clickedBtn.Tag as string) != "Selected")
            {
                foreach (var child in langBtns.Children)
                {
                    if (child is Button btn)
                    {
                        btn.ClearValue(Button.TagProperty);
                    }


                    clickedBtn.Tag = "Selected";
                    _selectedLangBtn = clickedBtn;
                    _selectedLangBtnText = clickedBtn.ToolTip + "  ›";
                    _selectedLangBtnTextOr = clickedBtn.Name;

                    langSelectBtn.Content = _selectedLangBtnText;
                    if (!_isLangSelected)
                    {
                        langSelectBtn.Tag = "Show";
                        _isLangSelected = true;
                    }
                }
            }
            else
            {
                foreach (var child in langBtns.Children)
                {
                    if (child is Button btn)
                    {
                        btn.ClearValue(Button.TagProperty);
                        btn.Tag = "";
                    }
                }
                langSelectBtn.Tag = "";
                _isLangSelected = false;
            }
        }

        private void langSelectBtn_Clicked(object sender, RoutedEventArgs e)
        {
            SettingsFunctions.ControlSaveLang(_selectedLangBtnTextOr);

            RateCalcOpening rateCalcOpening = new RateCalcOpening();
            Application.Current.MainWindow = rateCalcOpening;
            rateCalcOpening.Show();
            this.Close();
        }
    }
} 