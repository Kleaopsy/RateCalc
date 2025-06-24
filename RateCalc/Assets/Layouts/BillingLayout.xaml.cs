using ControlzEx.Standard;
using Functions;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.ObjectModel;
using RateCalc.Assets.ViewModels;

namespace RateCalc.Assets.Layouts
{
    /// <summary>
    /// Interaction logic for BillingLayout.xaml
    /// </summary>
    public partial class BillingLayout : UserControl
    {
        char decimalSep = '.', thousandSep = ',';
        private ChartViewModel chartViewModel;
        public BillingLayout()
        {
            InitializeComponent();
            calcResultGrid.Loaded += (s, e) => RemoveRightBorderOfLastColumnHeader(calcResultGrid);
            chartViewModel = new ChartViewModel();
            DataContext = chartViewModel;
            Loaded += BillingLayout_Loaded;
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


        bool __is_monthly_interest = false, __is_interest_in = true;
        // Button

        Dictionary<int, decimal> newMonthlyIncome = new Dictionary<int, decimal>();
        Dictionary<int, decimal> monthlyInterest = new Dictionary<int, decimal>();
        private void calcBtm_Clicked(object sender, RoutedEventArgs e)
        {
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

        private void ShowSaveButton()
        {
            CalcBtn.Visibility = Visibility.Visible;

            var anim = new DoubleAnimation
            {
                From = -100,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var transform = CalcBtn.RenderTransform as TranslateTransform;
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

            var transform = CalcBtn.RenderTransform as TranslateTransform;
            anim.Completed += (s, e) =>
            {
                CalcBtn.Visibility = Visibility.Collapsed;
            };
            transform?.BeginAnimation(TranslateTransform.XProperty, anim);
        }
        private void AnimateLabelIn(Label label)
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
        private void AnimateLabelOut(Label label)
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
                ShowSaveButton();
            }
            else
            {
                HideSaveButton();
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

        public void CalcGrid()
        {
            List<MonthlyResult> results = new List<MonthlyResult>();
            char decSep = SettingsFunctions.ControlDecSep(), thoSep = SettingsFunctions.ControlThoSep();
            for (int month = 1; month <= monthlyIncome.Keys.Max(); month++)
            {
                decimal interestSum = 0.0m;
                for (var preMonth = 1; preMonth < month; preMonth++)
                {
                    interestSum += monthlyInterest[preMonth];
                }

                results.Add(new MonthlyResult
                {
                    Month = month,
                    NMI = TaskFunctions.FormatDecimalWithCustomSeparators(newMonthlyIncome[month], 2, decSep, thoSep),
                    _NMI = newMonthlyIncome[month],
                    M = TaskFunctions.FormatDecimalWithCustomSeparators(monthlyIncome[month], 2, decSep, thoSep),
                    _M = monthlyIncome[month],
                    InterestSum = TaskFunctions.FormatDecimalWithCustomSeparators((interestSum + monthlyInterest[month]), 2, decSep, thoSep),
                    _InterestSum = (interestSum + monthlyInterest[month]),
                    MI = TaskFunctions.FormatDecimalWithCustomSeparators(monthlyInterest[month], 2, decSep, thoSep),
                    _MI = monthlyInterest[month]
                });
            }
            calcResultGrid.ItemsSource = results;
            chartViewModel.UpdateChart(results);
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
}
