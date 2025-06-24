using Functions;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RateCalc.Assets.Layouts
{
    /// <summary>
    /// Interaction logic for SettingsLayout.xaml
    /// </summary>
    public partial class SettingsLayout : UserControl
    {
        RateCalcOpening? _mainWindow;
        public SettingsLayout()
        {
            InitializeComponent();
            Loaded += SettingsLayout_Loaded;
        }

        int _lang_index = 1;
        private void SettingsLayout_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow = Application.Current.MainWindow as RateCalcOpening;
            if (_mainWindow != null)
            {
                string _lang = _mainWindow._lang;
                switch (_lang)
                {
                    case "tr":
                        _lang_index = 0;
                        break;
                    case "en":
                        _lang_index = 1;
                        break;
                    case "fr":
                        _lang_index = 2;
                        break;
                    case "de":
                        _lang_index = 3;
                        break;
                    case "es":
                        _lang_index = 4;
                        break;
                    default:
                        _lang_index = 1;
                        break;
                }
                LanguageComboBox.SelectedIndex = _lang_index;
                LanguageComboBox.SelectionChanged += langComboBoxIndex_Changed;
                LanguageComboBox.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);
            }
        }
        bool showSaveBtn = false;
        private void langComboBoxIndex_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is string langCode)
            {
                if (langCode == _mainWindow?._lang)
                {
                    SaveBtn.Tag = "";
                    HideSaveButton();
                    showSaveBtn = false;
                }
                else
                {
                    if (!showSaveBtn)
                    {
                        SaveBtn.Tag = "active";
                        ShowSaveButton();
                        showSaveBtn = true;
                    }
                }
            }
        }

        private void SaveBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is string langCode)
            {
                string _lang = langCode;
                _mainWindow?._lang = langCode;
                _lang_index = LanguageComboBox.SelectedIndex;
                SettingsFunctions.ControlSaveLang(langCode);
                SettingsFunctions.ChangeLanguage(langCode);

                HideSaveButton();
                showSaveBtn = false;
            }
        }

        private void ShowSaveButton()
        {
            SaveBtn.Visibility = Visibility.Visible;

            var anim = new DoubleAnimation
            {
                From = -100,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var transform = SaveBtn.RenderTransform as TranslateTransform;
            transform?.BeginAnimation(TranslateTransform.XProperty, anim);
        }

        private void HideSaveButton()
        {
            var anim = new DoubleAnimation
            {
                From = 0,
                To = -100,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            var transform = SaveBtn.RenderTransform as TranslateTransform;
            anim.Completed += (s, e) =>
            {
                SaveBtn.Visibility = Visibility.Collapsed;
            };
            transform?.BeginAnimation(TranslateTransform.XProperty, anim);
        }
    }
}
