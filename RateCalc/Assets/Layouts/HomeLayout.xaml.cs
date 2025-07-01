using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using Functions;
using static Functions.SavedCalculationsReader;

namespace RateCalc.Assets.Layouts
{
    /// <summary>
    /// Interaction logic for HomeLayout.xaml
    /// </summary>
    public partial class HomeLayout : UserControl
    {

        public event EventHandler<SavedCalculation>? CalculationSelected;
        private List<SavedCalculation>? savedCalculations;
        public HomeLayout()
        {
            InitializeComponent();
            Loaded += HomeLayout_Loaded;
        }

        private void HomeLayout_Loaded(object sender, RoutedEventArgs e)
        {
            RateCalcOpening? _mainWindow = Application.Current.MainWindow as RateCalcOpening;
            LoadSavedCalculations();
        }

        private async void LoadSavedCalculations()
        {
            try
            {
                // Loading göster
                ShowLoadingState(true);

                // Hesaplamaları yükle (async yapabilirsin gerekirse)
                savedCalculations = await Task.Run(() => GetSavedCalculations());

                // UI'ı güncelle
                PopulateCalculationsList();

                // Loading gizle
                ShowLoadingState(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hesaplamalar yüklenirken hata oluştu: {ex.Message}",
                               "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowEmptyState();
            }
        }

        private void PopulateCalculationsList()
        {
            SavedCalculationsPanel.Children.Clear();

            if (savedCalculations == null || !savedCalculations.Any())
            {
                ShowEmptyState();
                return;
            }

            foreach (var calculation in savedCalculations)
            {
                var calculationItem = CreateCalculationItem(calculation);
                SavedCalculationsPanel.Children.Add(calculationItem);
            }
        }

        private Border CreateCalculationItem(SavedCalculation calculation)
        {
            var border = new Border();
            border.Style = (Style)FindResource("SavedCalculationItemStyle");

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Sol taraf - Bilgiler
            var leftPanel = new StackPanel();
            leftPanel.Orientation = Orientation.Vertical;
            leftPanel.Cursor = Cursors.Hand;
            Grid.SetColumn(leftPanel, 0);

            // Click event'i sadece leftPanel'e ekle
            leftPanel.MouseLeftButtonUp += (s, e) => OnCalculationSelected(calculation);

            // Başlık
            var titleText = new TextBlock();
            titleText.Text = calculation.Name;
            titleText.Style = (Style)FindResource("TitleTextStyle");
            leftPanel.Children.Add(titleText);

            // Tarih
            var dateText = new TextBlock();

            var _lang = SettingsFunctions.ControlLang();
            string _s = "";
            switch (_lang)
            {
                case "en":
                    _s = "Saved on";
                    break;
                case "tr":
                    _s = "Kaydedildiği tarih";
                    break;
                case "fr":
                    _s = "Enregistré le";
                    break;
                case "de":
                    _s = "Gespeichert am";
                    break;
                case "es":
                    _s = "Guardado el";
                    break;
                default:
                    _s = "Saved on";
                    break;
            }
            dateText.Text = $"{_s}: {calculation.SaveDate.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"))}";
            dateText.Style = (Style)FindResource("DateTextStyle");
            leftPanel.Children.Add(dateText);

            // Detay bilgiler (Results'tan özet)
            if (calculation.Results != null && calculation.Results.Any())
            {
                var totalMonths = calculation.Results.Count;
                var totalAccumulated = calculation.Results.LastOrDefault()?._NMI ?? 0;
                var totalInterest = calculation.Results.LastOrDefault()?._InterestSum ?? 0;
                var totalDeposited = calculation.Results.LastOrDefault()?._NMI ?? 0 - calculation.Results.LastOrDefault()?._InterestSum ?? 0;
                var detailText = new TextBlock();

                string completeString = "";
                switch (_lang)
                {
                    case "en":
                        string monthLabelEN = (totalMonths == 1) ? "Month" : "Months";
                        if (totalAccumulated == 0 && totalDeposited == 0)
                            completeString = $"{totalMonths} {monthLabelEN} • Transfer: None • Deposited: {totalDeposited:C2} • Accumulated: {totalAccumulated:C2} • Total Interest: {totalInterest:C2}";
                        else if (totalAccumulated != totalDeposited)
                            completeString = $"{totalMonths} {monthLabelEN} • Transfer: Inward • Deposited: {totalDeposited:C2} • Accumulated: {totalAccumulated:C2} • Total Interest: {totalInterest:C2}";
                        else
                            completeString = $"{totalMonths} {monthLabelEN} • Transfer: Outward • Deposited: {totalDeposited:C2} • Accumulated: {totalAccumulated:C2} • Total Interest: {totalInterest:C2}";
                        break;
                    case "tr":
                        if (totalAccumulated == 0 && totalDeposited == 0)
                            completeString = $"{totalMonths} Ay • Aktarım: Yok • Yatırılan: {totalDeposited:C2} • Birikmiş: {totalAccumulated:C2} • Toplam Faiz: {totalInterest:C2}";
                        else if (totalAccumulated != totalDeposited)
                            completeString = $"{totalMonths} Ay • Aktarım: İçeri • Yatırılan: {totalDeposited:C2} • Birikmiş: {totalAccumulated:C2} • Toplam Faiz: {totalInterest:C2}";
                        else
                            completeString = $"{totalMonths} Ay • Aktarım: Dışarı • Yatırılan: {totalDeposited:C2} • Birikmiş: {totalAccumulated:C2} • Toplam Faiz: {totalInterest:C2}";
                        break;
                    case "fr":
                        // Fransızcada "mois" tekil/çoğulda aynıdır
                        if (totalAccumulated == 0 && totalDeposited == 0)
                            completeString = $"{totalMonths} mois • Transfert : Aucun • Déposé : {totalDeposited:C2} • Accumulé : {totalAccumulated:C2} • Intérêts totaux : {totalInterest:C2}";
                        else if (totalAccumulated != totalDeposited)
                            completeString = $"{totalMonths} mois • Transfert : Entrant • Déposé : {totalDeposited:C2} • Accumulé : {totalAccumulated:C2} • Intérêts totaux : {totalInterest:C2}";
                        else
                            completeString = $"{totalMonths} mois • Transfert : Sortant • Déposé : {totalDeposited:C2} • Accumulé : {totalAccumulated:C2} • Intérêts totaux : {totalInterest:C2}";
                        break;
                    case "de":
                        string monthLabelDE = totalMonths == 1 ? "Monat" : "Monate";
                        if (totalAccumulated == 0 && totalDeposited == 0)
                            completeString = $"{totalMonths} {monthLabelDE} • Übertragung: Keine • Eingezahlt: {totalDeposited:C2} • Gesamt: {totalAccumulated:C2} • Gesamtzinsen: {totalInterest:C2}";
                        else if (totalAccumulated != totalDeposited)
                            completeString = $"{totalMonths} {monthLabelDE} • Übertragung: Intern • Eingezahlt: {totalDeposited:C2} • Gesamt: {totalAccumulated:C2} • Gesamtzinsen: {totalInterest:C2}";
                        else
                            completeString = $"{totalMonths} {monthLabelDE} • Übertragung: Extern • Eingezahlt: {totalDeposited:C2} • Gesamt: {totalAccumulated:C2} • Gesamtzinsen: {totalInterest:C2}";
                        break;
                    case "es":
                        string monthLabelES = totalMonths == 1 ? "mes" : "meses";
                        if (totalAccumulated == 0 && totalDeposited == 0)
                            completeString = $"{totalMonths} {monthLabelES} • Transferencia: Ninguna • Depositado: {totalDeposited:C2} • Acumulado: {totalAccumulated:C2} • Interés total: {totalInterest:C2}";
                        else if (totalAccumulated != totalDeposited)
                            completeString = $"{totalMonths} {monthLabelES} • Transferencia: Entrante • Depositado: {totalDeposited:C2} • Acumulado: {totalAccumulated:C2} • Interés total: {totalInterest:C2}";
                        else
                            completeString = $"{totalMonths} {monthLabelES} • Transferencia: Saliente • Depositado: {totalDeposited:C2} • Acumulado: {totalAccumulated:C2} • Interés total: {totalInterest:C2}";
                        break;
                    default:
                        monthLabelEN = (totalMonths == 1) ? "Month" : "Months";
                        if (totalAccumulated == 0 && totalDeposited == 0)
                            completeString = $"{totalMonths} {monthLabelEN} • Transfer: None • Deposited: {totalDeposited:C2} • Accumulated: {totalAccumulated:C2} • Total Interest: {totalInterest:C2}";
                        else if (totalAccumulated != totalDeposited)
                            completeString = $"{totalMonths} {monthLabelEN} • Transfer: Inward • Deposited: {totalDeposited:C2} • Accumulated: {totalAccumulated:C2} • Total Interest: {totalInterest:C2}";
                        else
                            completeString = $"{totalMonths} {monthLabelEN} • Transfer: Outward • Deposited: {totalDeposited:C2} • Accumulated: {totalAccumulated:C2} • Total Interest: {totalInterest:C2}";
                        break;
                }
                detailText.Text = completeString;
                detailText.Style = (Style)FindResource("DetailTextStyle");
                leftPanel.Children.Add(detailText);
            }

            // Silme butonu
            var deleteButton = new Button();
            deleteButton.Content = "🗑️";
            deleteButton.Style = (Style)FindResource("DeleteButtonStyle");
            deleteButton.ToolTip = GetLocalizedDeleteTooltip(_lang);
            deleteButton.Click += (s, e) => DeleteCalculation(calculation);
            Grid.SetColumn(deleteButton, 1);

            // Sağ taraf - Ok işareti
            var arrowText = new TextBlock();
            arrowText.Text = "›";
            arrowText.Foreground = System.Windows.Media.Brushes.White;
            arrowText.FontSize = 20;
            arrowText.VerticalAlignment = VerticalAlignment.Center;
            arrowText.Margin = new Thickness(10, 0, 0, 0);
            Grid.SetColumn(arrowText, 2);

            grid.Children.Add(leftPanel);
            grid.Children.Add(deleteButton);
            grid.Children.Add(arrowText);
            border.Child = grid;

            // MainWindow navigation - sadece leftPanel'e tıklandığında
            var mainWindow = Application.Current.MainWindow as RateCalcOpening;
            leftPanel.MouseLeftButtonDown += (s, e) =>
            {
                if (mainWindow != null && calculation.Results != null)
                    mainWindow.goToBilling(calculation.Name, calculation.Results);
            };

            return border;
        }

        private string GetLocalizedDeleteTooltip(string lang)
        {
            return lang switch
            {
                "tr" => "Hesaplama kaydını sil",
                "en" => "Delete calculation record",
                "fr" => "Supprimer l'enregistrement de calcul",
                "de" => "Berechnungsaufzeichnung löschen",
                "es" => "Eliminar registro de cálculo",
                _ => "Delete calculation record"
            };
        }

        private void DeleteCalculation(SavedCalculation calculation)
        {
            var _lang = SettingsFunctions.ControlLang();
            string title = _lang switch
            {
                "tr" => "Silme Onayı",
                "en" => "Delete Confirmation",
                "fr" => "Confirmation de suppression",
                "de" => "Löschbestätigung",
                "es" => "Confirmación de eliminación",
                _ => "Delete Confirmation"
            };

            string message = _lang switch
            {
                "tr" => $"'{calculation.Name}' hesaplama kaydını silmek istediğinizden emin misiniz?",
                "en" => $"Are you sure you want to delete the calculation record '{calculation.Name}'?",
                "fr" => $"Êtes-vous sûr de vouloir supprimer l'enregistrement de calcul '{calculation.Name}' ?",
                "de" => $"Sind Sie sicher, dass Sie die Berechnungsaufzeichnung '{calculation.Name}' löschen möchten?",
                "es" => $"¿Está seguro de que desea eliminar el registro de cálculo '{calculation.Name}'?",
                _ => $"Are you sure you want to delete the calculation record '{calculation.Name}'?"
            };

            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Dosyayı sil
                    if (File.Exists(calculation.FilePath))
                    {
                        File.Delete(calculation.FilePath);
                    }

                    // Liste'den kaldır
                    savedCalculations?.Remove(calculation);

                    // UI'ı güncelle
                    PopulateCalculationsList();

                    string successMessage = _lang switch
                    {
                        "tr" => "Hesaplama kaydı başarıyla silindi.",
                        "en" => "Calculation record deleted successfully.",
                        "fr" => "Enregistrement de calcul supprimé avec succès.",
                        "de" => "Berechnungsaufzeichnung erfolgreich gelöscht.",
                        "es" => "Registro de cálculo eliminado con éxito.",
                        _ => "Calculation record deleted successfully."
                    };

                    MessageBox.Show(successMessage, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    string errorMessage = _lang switch
                    {
                        "tr" => $"Dosya silinirken hata oluştu: {ex.Message}",
                        "en" => $"Error occurred while deleting file: {ex.Message}",
                        "fr" => $"Erreur lors de la suppression du fichier : {ex.Message}",
                        "de" => $"Fehler beim Löschen der Datei: {ex.Message}",
                        "es" => $"Error al eliminar archivo: {ex.Message}",
                        _ => $"Error occurred while deleting file: {ex.Message}"
                    };

                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ShowLoadingState(bool show)
        {
            if (show)
            {
                LoadingGrid.Visibility = Visibility.Visible;
                SavedCalculationsPanel.Visibility = Visibility.Collapsed;
                EmptyStateGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoadingGrid.Visibility = Visibility.Collapsed;
                SavedCalculationsPanel.Visibility = Visibility.Visible;
            }
        }

        private void ShowEmptyState()
        {
            LoadingGrid.Visibility = Visibility.Collapsed;
            SavedCalculationsPanel.Visibility = Visibility.Collapsed;
            EmptyStateGrid.Visibility = Visibility.Visible;
        }

        private void OnCalculationSelected(SavedCalculation calculation)
        {
            // Event'i fırlat - ana pencere bunu dinleyecek
            CalculationSelected?.Invoke(this, calculation);
        }

        // Refresh metodu - dışarıdan çağrılabilir
        public void RefreshCalculations()
        {
            LoadSavedCalculations();
        }

        // Mevcut GetSavedCalculations metodunuz buraya
        public static List<SavedCalculation> GetSavedCalculations()
        {
            // Sizin mevcut kodunuz burada olacak
            List<SavedCalculation> savedCalculations = new List<SavedCalculation>();
            try
            {
                // Documents/RateCalc klasör yolunu al
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string rateCalcPath = System.IO.Path.Combine(documentsPath, "RateCalc");
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
                                Name = nameElement.GetString() ?? System.IO.Path.GetFileNameWithoutExtension(filePath),
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
                        // Bu dosya bozuk, atla ve devam et
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