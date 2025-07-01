using RateCalc;
using RateCalc.Assets.Layouts;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Functions
{
    public static class TaskFunctions
    {
        public static void CloseBtn(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(sender as DependencyObject);
            if (window != null)
                window.Close();
        }

        public static void MinBtn(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(sender as DependencyObject);
            if (window != null)
                window.WindowState = WindowState.Minimized;
        }

        public static void MaxBtn(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(sender as DependencyObject);
            if (window != null)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    window.WindowState = WindowState.Normal;
                }
                else
                {
                    window.WindowState = WindowState.Maximized;
                }
            }
        }

        public static void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window window = Window.GetWindow(sender as DependencyObject);
            if (window != null && window.ResizeMode != ResizeMode.NoResize)
            {
                if (e.ClickCount == 2)
                {
                    if (window.WindowState == WindowState.Maximized)
                    {
                        window.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        window.WindowState = WindowState.Maximized;
                    }
                }
                else
                    window.DragMove();
            }
            else if (window != null && window.ResizeMode == ResizeMode.NoResize)
                window.DragMove();
        }

        public static T? FindElementByTag<T>(DependencyObject parent, object tag) where T : FrameworkElement
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T fe && Equals(fe.Tag, tag))
                    return fe;

                var result = FindElementByTag<T>(child, tag);
                if (result != null)
                    return result;
            }
            return null;
        }
        public static bool IsValidNumericInput(string input, string decimalSep, string thousandSep, char comingChar)
        {
            int decimalSepCount = 0;

            foreach (char c in input)
            {
                if (char.IsDigit(c)) continue;

                if (c.ToString() == decimalSep)
                {
                    decimalSepCount++;
                    if (decimalSepCount > 0) {
                        if (char.IsDigit(comingChar))
                            return true;
                        else
                            return false;
                    }
                }
                else if (c.ToString() == thousandSep)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        public static string FormatDecimalWithCustomSeparators(decimal value, int decimalPlaces = 2, char decSep = '.', char thoSep = ',')
        {
            // Normalize to invariant string with dot separator
            string raw = value.ToString("N" + decimalPlaces, CultureInfo.InvariantCulture);

            // Split by dot to separate integer and fractional parts
            var parts = raw.Split('.');

            string intPart = parts[0];
            string fracPart = parts.Length > 1 ? parts[1] : "";

            // Replace default thousand separator ',' with custom one
            intPart = intPart.Replace(",", thoSep.ToString());

            return fracPart == ""
                ? intPart
                : $"{intPart}{decSep}{fracPart}";
        }
    }
    public static class SettingsFunctions
    {
        private static Configuration _Settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private static ConfigurationSection? _uisettings;
        private static UISettings? settings;
        public static void Control()
        {
            if (_Settings.Sections["UISettings"] is null)
            {
                _Settings.Sections.Add("UISettings", new UISettings());
                _Settings.Save(ConfigurationSaveMode.Modified);
            }
            _uisettings = _Settings.GetSection("UISettings");
            settings = _uisettings as UISettings;

            if (settings != null)
            {
                if (settings.SectionInformation.ForceSave != true)
                    settings.SectionInformation.ForceSave = true;
            }
            else
            {
                return;
            }
        }
        public static string ControlLang()
        {
            if (settings != null && !string.IsNullOrWhiteSpace(settings.language))
            {
                return settings.language;
            }
            return "en"; // default olarak İngilizce
        }
        public static char ControlDecSep()
        {
            if (settings != null)
            {
                return settings.DecimalSeparator;
            }
            return '.'; // default olarak İngilizce
        }
        public static char ControlThoSep()
        {
            if (settings != null)
            {
                return settings.ThousandSeparator;
            }
            return ','; // default olarak İngilizce
        }
        public static void ControlSaveLang(string lang)
        {
            Control();
            if (settings != null)
            {
                settings.language = lang;
                switch (lang)
                {
                    case "tr":
                        settings.ThousandSeparator = '.';
                        settings.DecimalSeparator = ',';
                        break;
                    case "en":
                        settings.ThousandSeparator = ',';
                        settings.DecimalSeparator = '.';
                        break;
                    case "fr":
                        settings.ThousandSeparator = ' ';
                        settings.DecimalSeparator = ',';
                        break;
                    case "de":
                        settings.ThousandSeparator = '.';
                        settings.DecimalSeparator = ',';
                        break;
                    case "es":
                        settings.ThousandSeparator = '.';
                        settings.DecimalSeparator = ',';
                        break;
                    default:
                        break;
                }   
            }
            else
            {
                return; 
            }
            _Settings.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("UISettings");
        }


        public static ResourceDictionary iconsDict = new ResourceDictionary(), colorsDict = new ResourceDictionary(), stylesDict = new ResourceDictionary();
        public static void ChangeLanguage(string lang)
        {
            var dict = new ResourceDictionary();
            dict.Source = new Uri($"Assets/Languages/{lang}.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Clear();

            iconsDict.Source = new Uri("/Assets/Icons/Icons.xaml", UriKind.Relative);
            colorsDict.Source = new Uri("/Assets/Colors/Colors.xaml", UriKind.Relative);
            stylesDict.Source = new Uri("/Assets/Styles/Styles.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(iconsDict);
            Application.Current.Resources.MergedDictionaries.Add(colorsDict);
            Application.Current.Resources.MergedDictionaries.Add(stylesDict);

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
    public class HalfValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
                return (2 * d) / 3;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public static class SavedCalculationsReader
    {
        public class SavedCalculation
        {
            public string Name { get; set; } = "";
            public DateTime SaveDate { get; set; }
            public List<MonthlyResult> Results { get; set; } = new List<MonthlyResult>();
            public string FilePath { get; set; } = "";
        }

        public static List<SavedCalculation> GetSavedCalculations()
        {
            List<SavedCalculation> savedCalculations = new List<SavedCalculation>();

            try
            {
                // Documents/RateCalc klasör yolunu al
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string rateCalcPath = Path.Combine(documentsPath, "RateCalc");

                // Klasör yoksa boş liste döndür
                if (!Directory.Exists(rateCalcPath))
                    return savedCalculations;

                // .json dosyalarını bul
                string[] jsonFiles = Directory.GetFiles(rateCalcPath, "*.json");

                foreach (string filePath in jsonFiles)
                {
                    try
                    {
                        // JSON dosyasını oku
                        string jsonContent = File.ReadAllText(filePath);

                        // JSON'u deserialize et
                        var jsonDocument = JsonDocument.Parse(jsonContent);
                        var root = jsonDocument.RootElement;

                        // Gerekli alanları kontrol et
                        if (root.TryGetProperty("Name", out var nameElement) &&
                            root.TryGetProperty("SaveDate", out var dateElement) &&
                            root.TryGetProperty("Results", out var resultsElement))
                        {
                            var calculation = new SavedCalculation
                            {
                                Name = nameElement.GetString() ?? Path.GetFileNameWithoutExtension(filePath),
                                SaveDate = dateElement.GetDateTime(),
                                FilePath = filePath
                            };

                            // Results array'ini deserialize et
                            if (resultsElement.ValueKind == JsonValueKind.Array)
                            {
                                var resultsJson = resultsElement.GetRawText();
                                var results = JsonSerializer.Deserialize<List<MonthlyResult>>(resultsJson);
                                calculation.Results = results ?? new List<MonthlyResult>();
                            }

                            savedCalculations.Add(calculation);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Hatalı dosya atlandı: {filePath} - {ex.Message}");
                        continue;
                    }
                }

                // Tarihe göre sırala (en yeni önce)
                savedCalculations = savedCalculations.OrderByDescending(c => c.SaveDate).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return savedCalculations;
        }
    }
}
