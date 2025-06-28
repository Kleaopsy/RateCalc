using Functions;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace RateCalc.Assets.Layouts
{
    /// <summary>
    /// Interaction logic for BillingLayout.xaml
    /// </summary>
    /// 

    public partial class BillingLayout : UserControl
    {
        char decimalSep = '.', thousandSep = ',';
        private ChartViewModel chartViewModel = new ChartViewModel();
        public BillingLayout()
        {
            InitializeComponent();
            calcResultGrid.Loaded += (s, e) => RemoveRightBorderOfLastColumnHeader(calcResultGrid);
            Loaded += BillingLayout_Loaded;
            DataContext = chartViewModel;
            graphics1.LegendTextPaint = new SolidColorPaint(SKColors.White);
        }

        bool interestPercentTBoxBool = false, interestCalcTBoxBool = false;
        private void BillingLayout_Loaded(object sender, RoutedEventArgs e)
        {
            RateCalcOpening? _mainWindow = System.Windows.Application.Current.MainWindow as RateCalcOpening;
            decimalSep = SettingsFunctions.ControlDecSep();
            thousandSep = SettingsFunctions.ControlThoSep();

            if (!interestPercentTBoxBool)
                interestPercent.Text = interestPercent.Tag.ToString();
            else
            {
                if (decimalSep != __old_dec)
                {
                    MessageBox.Show("1." + __old_dec.ToString() + " 2." + decimalSep.ToString());
                    interestPercent.Text = interestPercent.Text.Replace(__old_dec.ToString(), decimalSep.ToString());
                    __old_dec = decimalSep;
                }
                if (thousandSep != __old_tho)
                {
                    interestPercent.Text = interestPercent.Text.Replace(__old_tho.ToString(), thousandSep.ToString());
                    __old_tho = thousandSep;
                }
            }
            if (!interestCalcTBoxBool)
                interestCalc.Text = interestCalc.Tag.ToString();

            ControlCalcButton();
            interestPercent.TextChanged += interestPercentText_Changed;
            interestCalc.TextChanged += interestCalcText_Changed;

            if (calcResultGrid.ItemsSource != null && calcResultGrid.Items.Cast<object>().Any())
                CalcGrid();
        }

        private async void calcBtmSave_Clicked(object sender, RoutedEventArgs e)
        {
            var _lang = SettingsFunctions.ControlLang();
            var title = "";
            var btnText = "";
            var title2 = "";
            switch (_lang)
            {
                case "en":
                    title = "Enter a name for this calculation:";
                    btnText = "Save Calculation";
                    title2 = "New Calculation";
                    break;
                case "tr":
                    title = "Bu hesaplama için bir isim girin:";
                    btnText = "Hesaplama Kaydet";
                    title2 = "Yeni Hesaplama";
                    break;
                case "fr":
                    title = "Entrez un nom pour ce calcul:";
                    btnText = "Enregistrer le calcul";
                    title2 = "Nouveau calcul";
                    break;
                case "de":
                    title = "Geben Sie einen Namen für diese Berechnung ein:";
                    btnText = "Berechnung speichern";
                    title2 = "Neue Berechnung";
                    break;
                case "es":
                    title = "Ingrese un nombre para este cálculo:";
                    btnText = "Guardar Cálculo";
                    title2 = "Nuevo Cálculo";
                    break;
                default:
                    title = "Enter a name for this calculation:";
                    btnText = "Save Calculation";
                    title2 = "New Calculation";
                    break;
            }

            var dialog = new calcSaveDialog(title, btnText, title2);
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                dialog.Owner = parentWindow;
            }

            if (dialog.ShowDialog() == true)
            {
                var response = dialog.ResponseText;
                string hesapIsmi = response + "";
                if (!string.IsNullOrEmpty(hesapIsmi.Trim()))
                {
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string rateCalcPath = Path.Combine(documentsPath, "RateCalc");

                    try
                    {
                        if (!Directory.Exists(rateCalcPath))
                        {
                            Directory.CreateDirectory(rateCalcPath);
                        }

                        // Mevcut kayıtları kontrol et
                        var savedCalculations = SavedCalculationsReader.GetSavedCalculations();
                        var existingCalc = savedCalculations.FirstOrDefault(c => c.Name.Equals(hesapIsmi.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (existingCalc != null)
                        {
                            // Dosya zaten var, üstüne yazma onayı iste
                            var overwriteTitle = "";
                            var overwriteMessage = "";

                            switch (_lang)
                            {
                                case "en":
                                    overwriteMessage = $"A calculation named '{hesapIsmi.Trim()}' already exists.\nDo you want to overwrite it?";
                                    overwriteTitle = "File Already Exists";
                                    break;
                                case "tr":
                                    overwriteMessage = $"'{hesapIsmi.Trim()}' adında bir hesaplama zaten mevcut.\nÜstüne yazmak istiyor musunuz?";
                                    overwriteTitle = "Dosya Zaten Mevcut";
                                    break;
                                case "fr":
                                    overwriteMessage = $"Un calcul nommé '{hesapIsmi.Trim()}' existe déjà.\nVoulez-vous l'écraser?";
                                    overwriteTitle = "Fichier Déjà Existant";
                                    break;
                                case "de":
                                    overwriteMessage = $"Eine Berechnung namens '{hesapIsmi.Trim()}' existiert bereits.\nMöchten Sie sie überschreiben?";
                                    overwriteTitle = "Datei Bereits Vorhanden";
                                    break;
                                case "es":
                                    overwriteMessage = $"Ya existe un cálculo llamado '{hesapIsmi.Trim()}'.\n¿Desea sobrescribirlo?";
                                    overwriteTitle = "Archivo Ya Existe";
                                    break;
                                default:
                                    overwriteMessage = $"A calculation named '{hesapIsmi.Trim()}' already exists.\nDo you want to overwrite it?";
                                    overwriteTitle = "File Already Exists";
                                    break;
                            }

                            var overwriteDialog = new calcSaveOverrideDialog(overwriteMessage, overwriteTitle);
                            parentWindow = Window.GetWindow(this);
                            if (parentWindow != null)
                            {
                                overwriteDialog.Owner = parentWindow;
                            }

                            if (overwriteDialog.ShowDialog() != true)
                            {
                                AnimateLabelOut(interestCalcSaveLabel1);
                                return;
                            }
                        }

                        List<MonthlyResult> results = GetCurrentResults();
                        string fileName = $"{hesapIsmi.Trim()}.json";
                        string filePath = Path.Combine(rateCalcPath, fileName);

                        var saveData = new
                        {
                            Name = hesapIsmi.Trim(),
                            SaveDate = DateTime.Now,
                            Results = results
                        };

                        string jsonContent = System.Text.Json.JsonSerializer.Serialize(saveData, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        await File.WriteAllTextAsync(filePath, jsonContent);
                        AnimateLabelIn(interestCalcSaveLabel1);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error Code: {ex.Message}", "Error");
                        AnimateLabelOut(interestCalcSaveLabel1);
                    }
                }
                else
                {
                    AnimateLabelOut(interestCalcSaveLabel1);
                }
            }
            else
                AnimateLabelOut(interestCalcSaveLabel1);
        }

        bool __is_monthly_interest = false, __is_interest_in = true, __calc_before = false;
        // Button

        Dictionary<int, decimal> newMonthlyIncome = new Dictionary<int, decimal>();
        Dictionary<int, decimal> monthlyInterest = new Dictionary<int, decimal>();
        private void calcBtm_Clicked(object sender, RoutedEventArgs e)
        {
            if (__calc_before)
            {
                HideSaveButton(CalcBtnSave);
            }
            newMonthlyIncome.Clear();
            monthlyInterest.Clear();

            if (interestIn.IsChecked == true)
                __is_interest_in = true;
            else
                __is_interest_in = false;

            if (interestMonthly.IsChecked == true)
                __is_monthly_interest = true;
            else
                __is_monthly_interest = false;

            var percentInterest = (__is_monthly_interest) ? decimal.Parse(interestPercent.Text) : decimal.Parse(interestPercent.Text) / 12.0m;

            if (__is_interest_in)
            {
                decimal cumulativeIncome = 0.0m;
                for (int month = 1; month <= monthlyIncome.Keys.Max(); month++)
                {
                    decimal interest = cumulativeIncome * percentInterest / 100;

                    newMonthlyIncome[month] = cumulativeIncome + monthlyIncome[month] + interest;
                    monthlyInterest[month] = interest;

                    cumulativeIncome = newMonthlyIncome[month];
                }
            }
            else
            {
                decimal cumulativeIncome = 0.0m;
                for (int month = 1; month <= monthlyIncome.Keys.Max(); month++)
                {
                    decimal interest = cumulativeIncome * percentInterest / 100;

                    newMonthlyIncome[month] = cumulativeIncome + monthlyIncome[month];
                    monthlyInterest[month] = interest;

                    cumulativeIncome = newMonthlyIncome[month];
                }
            }

            CalcGrid();

            if (!__calc_before)
            {
                __calc_before = true;
            }
            ShowSaveButton(CalcBtnSave);
        }


        Dictionary<int, Decimal> monthlyIncome = new Dictionary<int, decimal>();

        private void interestCalcText_Changed(object sender, TextChangedEventArgs e)
        {
            monthlyIncome.Clear();

            if (!string.IsNullOrWhiteSpace(interestCalc.Text) && interestCalc.Text != interestCalc.Tag.ToString())
            {
                Dictionary<int, decimal> tempIncome = new Dictionary<int, decimal>();
                string[] lines = interestCalc.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(new[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 2) continue;
                    if (int.TryParse(parts[0].Trim(), out int month))
                    {
                        string rawValue = parts[1].Trim();

                        // Binlik ayırıcıyı kaldır, ondalık ayracı "." yap
                        string cleaned = rawValue.Replace(thousandSep.ToString(), "");
                        cleaned = cleaned.Replace(decimalSep.ToString(), ".");

                        if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                        {
                            if (tempIncome.ContainsKey(month))
                            {
                                // Hata: Aynı ay tekrar edilmiş
                                interestCalcLabel1.Tag = "active";
                                AnimateLabelIn(interestCalcLabel1);
                                interestCalcLabel2.Tag = "";
                                AnimateLabelOut(interestCalcLabel2);
                                interestCalcTBoxBool = false;
                                return;
                            }

                            tempIncome[month] = value;
                        }
                    }
                }

                if (tempIncome.Count == 0)
                {
                    interestCalcTBoxBool = false;
                    return;
                }
                if (tempIncome.Count > 0)
                {
                    // Sıralı değilse hata ver
                    var monthKeys = tempIncome.Keys.ToList();
                    var sortedKeys = monthKeys.OrderBy(m => m).ToList();

                    if (!monthKeys.SequenceEqual(sortedKeys))
                    {
                        interestCalcLabel1.Tag = "active";
                        AnimateLabelIn(interestCalcLabel1);
                        interestCalcLabel2.Tag = "";
                        AnimateLabelOut(interestCalcLabel2);
                        interestCalcTBoxBool = false;
                        return;
                    }

                    // İlk ay 1 mi? veya 0 içeriyor mu?
                    if (sortedKeys[0] != 1 || sortedKeys.Any(m => m == 0))
                    {
                        interestCalcLabel1.Tag = "";
                        AnimateLabelOut(interestCalcLabel1);
                        interestCalcLabel2.Tag = "active";
                        AnimateLabelIn(interestCalcLabel2);
                        interestCalcTBoxBool = false;
                        return;
                    }

                    // Eksik ayları 0 olarak tamamla
                    int maxMonth = sortedKeys.Last();
                    for (int i = 1; i <= maxMonth; i++)
                    {
                        if (tempIncome.ContainsKey(i))
                            monthlyIncome[i] = tempIncome[i];
                        else
                            monthlyIncome[i] = 0m;
                    }
                }
                interestCalcLabel1.Tag = "";
                AnimateLabelOut(interestCalcLabel1);
                interestCalcLabel2.Tag = "";
                AnimateLabelOut(interestCalcLabel2);
                interestCalcTBoxBool = true;
            }
            ControlCalcButton();
        }


        private decimal? interestPercentValue = null;
        private char __old_dec = ' ', __old_tho = ' ';
        private void interestPercentText_Changed(object sender, TextChangedEventArgs e)
        {
            __old_dec = decimalSep;
            __old_tho = thousandSep;
            if (!string.IsNullOrEmpty(interestPercent.Text) && interestPercent.Text != interestPercent.Tag.ToString())
            {
                var input = interestPercent.Text;
                var cleanedInput = input.Replace(thousandSep.ToString(), "");
                cleanedInput = cleanedInput.Replace(decimalSep.ToString(), ".");

                if (decimal.TryParse(cleanedInput, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedValue))
                {
                    interestPercentValue = parsedValue;
                    interestPercentTBoxBool = true;
                }
                else
                {
                    interestPercentValue = null;
                    __last_textBox_text_B = true;
                    interestPercentTBoxBool = false;
                }
            }
            else
                interestPercentTBoxBool = false;

            ControlCalcButton();
        }

        private void interestPercentText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char inputChar = e.Text[0];
            bool isDigit = char.IsDigit(inputChar);
            bool isDecimalSep = inputChar == decimalSep;
            bool isThousandSep = inputChar == thousandSep;

            if (!TaskFunctions.IsValidNumericInput(interestPercent.Text, decimalSep.ToString(), thousandSep.ToString(), e.Text[0]))
            {
                e.Handled = true;
                return;
            }
            e.Handled = !(isDigit || isDecimalSep || isThousandSep);
        }
        private void interestPercentText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && thousandSep != ' ')
            {
                e.Handled = true;
            }
        }

        string __last_textBox_text = "", _second__last_textBox_text = "";
        private void textBox_Focused(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Text == textBox.Tag.ToString())
                {
                    if (textBox.Name == "interestPercent")
                    {
                        __last_textBox_text = textBox.Text;
                    }
                    else
                    {
                        _second__last_textBox_text = textBox.Text;
                    }
                    textBox.Text = "";
                }
            }
        }

        bool __last_textBox_text_B = false, _second__last_textBox_text_B = false;
        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text) || __last_textBox_text_B || _second__last_textBox_text_B)
                {
                    if (textBox.Name == "interestPercent" || __last_textBox_text_B)
                    {
                        textBox.Text = __last_textBox_text;
                        __last_textBox_text_B = false;
                    }
                    else if (textBox.Name == "interestCalc" || _second__last_textBox_text_B)
                    {
                        textBox.Text = _second__last_textBox_text;
                        _second__last_textBox_text_B = false;
                    }
                }
            }
        }

        private void ShowSaveButton(Button btn)
        {
            btn.Visibility = Visibility.Visible;

            var anim = new DoubleAnimation
            {
                From = -100,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var transform = btn.RenderTransform as TranslateTransform;
            transform?.BeginAnimation(TranslateTransform.XProperty, anim);
        }

        private void HideSaveButton(Button btn)
        {
            var anim = new DoubleAnimation
            {
                From = 0,
                To = -100,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            var transform = btn.RenderTransform as TranslateTransform;
            anim.Completed += (s, e) =>
            {
                btn.Visibility = Visibility.Collapsed;
            };
            transform?.BeginAnimation(TranslateTransform.XProperty, anim);
        }

        private void AnimateLabelIn(System.Windows.Controls.Label label)
        {
            label.Visibility = Visibility.Visible;

            var translate = label.RenderTransform as TranslateTransform;
            if (translate != null)
            {
                translate.X = -100; // Başa sar

                var animation = new DoubleAnimation
                {
                    From = -100,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
                };

                translate.BeginAnimation(TranslateTransform.XProperty, animation);
            }
        }
        private void AnimateLabelOut(System.Windows.Controls.Label label)
        {
            var translate = label.RenderTransform as TranslateTransform;
            if (translate != null)
            {
                var animation = new DoubleAnimation
                {
                    From = 0,
                    To = -100,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseIn }
                };

                animation.Completed += (s, e) =>
                {
                    label.Visibility = Visibility.Collapsed;
                };

                translate.BeginAnimation(TranslateTransform.XProperty, animation);
            }
            else
            {
                // Güvenlik için yine de kapatalım
                label.Visibility = Visibility.Collapsed;
            }
        }

        public void ControlCalcButton()
        {
            if (interestPercentTBoxBool && interestCalcTBoxBool)
            {
                ShowSaveButton(CalcBtn);
            }
            else
            {
                HideSaveButton(CalcBtn);
            }
        }
        void RemoveRightBorderOfLastColumnHeader(DataGrid dataGrid)
        {
            var headers = GetVisualChildren<DataGridColumnHeader>(dataGrid);
            foreach (var header in headers)
            {
                if ((header.Tag as string) == "LastColumn")
                {
                    var bt = header.BorderThickness;
                    header.BorderThickness = new Thickness(bt.Left, bt.Top, 0, bt.Bottom);
                    break;
                }
            }
        }

        // VisualTree helper method
        public static IEnumerable<T> GetVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                    yield return t;
                foreach (var childOfChild in GetVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
        private List<MonthlyResult> currentResults = new List<MonthlyResult>();
        public void CalcGrid()
        {
            currentResults = new List<MonthlyResult>();
            char decSep = SettingsFunctions.ControlDecSep(), thoSep = SettingsFunctions.ControlThoSep();
            for (int month = 1; month <= monthlyIncome.Keys.Max(); month++)
            {
                decimal interestSum = 0.0m;
                for (var preMonth = 1; preMonth < month; preMonth++)
                {
                    interestSum += monthlyInterest[preMonth];
                }

                currentResults.Add(new MonthlyResult
                {
                    Month = month,
                    NMI = TaskFunctions.FormatDecimalWithCustomSeparators(newMonthlyIncome[month], 2, decSep, thoSep),
                    _NMI = Math.Round(newMonthlyIncome[month], 2),
                    M = TaskFunctions.FormatDecimalWithCustomSeparators(monthlyIncome[month], 2, decSep, thoSep),
                    _M = Math.Round(monthlyIncome[month], 2),
                    InterestSum = TaskFunctions.FormatDecimalWithCustomSeparators((interestSum + monthlyInterest[month]), 2, decSep, thoSep),
                    _InterestSum = Math.Round((interestSum + monthlyInterest[month]), 2),
                    MI = TaskFunctions.FormatDecimalWithCustomSeparators(monthlyInterest[month], 2, decSep, thoSep),
                    _MI = Math.Round(monthlyInterest[month], 2)
                });
            }
            calcResultGrid.ItemsSource = currentResults;
            chartViewModel.UpdateChart(currentResults);

            if (graphics1.Visibility == Visibility.Hidden)
                graphics1.Visibility = Visibility.Visible;
            if (calcResultGrid.Visibility == Visibility.Hidden)
                calcResultGrid.Visibility = Visibility.Visible;
        }
        private List<MonthlyResult> GetCurrentResults()
        {
            return currentResults;
        }
    }
    public class MonthlyResult
    {
        public int Month { get; set; }
        public required string M { get; set; }
        public required decimal _M { get; set; }
        public required string MI { get; set; }
        public required decimal _MI { get; set; }
        public required string InterestSum { get; set; }
        public required decimal _InterestSum { get; set; }
        public required string NMI { get; set; }
        public required decimal _NMI { get; set; }
    }
    public class ChartViewModel : INotifyPropertyChanged
    {
        private ISeries[] _series = [];
        public ISeries[] Series
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged(nameof(Series));
            }
        }

        private Axis[] _xAxes = [];

        public Axis[] XAxes
        {
            get => _xAxes;
            set
            {
                _xAxes = value;
                OnPropertyChanged(nameof(XAxes));
            }
        }

        private Axis[] _yAxes = [];
        public Axis[] YAxes
        {
            get => _yAxes;
            set
            {
                _yAxes = value;
                OnPropertyChanged(nameof(YAxes));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void UpdateChart(List<MonthlyResult> results)
        {
            var labels = results.Select(r => r.Month.ToString()).ToArray();
            string _lang = SettingsFunctions.ControlLang();
            string col_1 = "", col_2 = "", col_3 = "", col_4 = "";

            switch (_lang)
            {
                case "en":
                    col_1 = "Deposited";
                    col_2 = "Monthly Interest";
                    col_3 = "Total Interest";
                    col_4 = "Total Accumulated";
                    break;
                case "tr":
                    col_1 = "Yatırılan";
                    col_2 = "Aylık Faiz";
                    col_3 = "Toplam Faiz";
                    col_4 = "Birikmiş Toplam";
                    break;
                case "fr":
                    col_1 = "Déposé";
                    col_2 = "Intérêt Mensuel";
                    col_3 = "Intérêt Total";
                    col_4 = "Total Accumulé";
                    break;
                case "de":
                    col_1 = "Eingezahlt";
                    col_2 = "Monatliche Zinsen";
                    col_3 = "Gesamtzinsen";
                    col_4 = "Gesamtsumme";
                    break;
                case "es":
                    col_1 = "Depositado";
                    col_2 = "Interés Mensual";
                    col_3 = "Interés Total";
                    col_4 = "Total Acumulado";
                    break;
                default:
                    col_1 = "Deposited";
                    col_2 = "Monthly Interest";
                    col_3 = "Total Interest";
                    col_4 = "Total Accumulated";
                    break;
            }
            Series = new ISeries[]
            {
                new ColumnSeries<decimal>
                {
                    Name = col_1,
                    Values = results.Select(r => r._M).ToArray()
                },
                new ColumnSeries<decimal>
                {
                    Name = col_2,
                    Values = results.Select(r => r._MI).ToArray()
                },
                new ColumnSeries<decimal>
                {
                    Name = col_3,
                    Values = results.Select(r => r._InterestSum).ToArray()
                },
                new ColumnSeries<decimal>
                {
                    Name = col_4,
                    Values = results.Select(r => r._NMI).ToArray()
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    LabelsRotation = 15,
                    Name = "",
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    NamePaint = new SolidColorPaint(SKColors.White)
                }
            };
            YAxes = new Axis[]{
                new Axis
                {
                    Name = "",
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    NamePaint = new SolidColorPaint(SKColors.White)
                }
            };
        }

        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
