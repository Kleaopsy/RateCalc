
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RateCalc.Assets.Layouts;
using System.Collections.Generic;
using System.ComponentModel;

namespace RateCalc.Assets.ViewModels
{
    public class ChartViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private PlotModel _myModel = new PlotModel { Title = "Grafik Yükleniyor..." };
        public PlotModel MyModel
        {
            get => _myModel;
            private set
            {
                _myModel = value;
                OnPropertyChanged(nameof(MyModel));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public void UpdateChart(List<MonthlyResult> results)
        {
            var model = new PlotModel { Title = "Aylık Finansal Dağılım" };

            // X ekseni - Kategori (aylar)
            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,  // Alt taraf (x ekseni)
                Title = "Aylar",
                Key = "AylarAxis"
            };
            foreach (var r in results)
            {
                categoryAxis.Labels.Add(r.Month.ToString());
            }
            model.Axes.Add(categoryAxis);

            // Y ekseni - Sayısal (para)
            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,  // Sol taraf (y ekseni)
                Title = "Tutar",
                MinimumPadding = 0,
                AbsoluteMinimum = 0,
                Key = "TutarAxis"
            };
            model.Axes.Add(valueAxis);

            // Seriler
            var seriesYatirilan = new BarSeries
            {
                Title = "Yatırılan",
                FillColor = OxyColors.SkyBlue,
                ItemsSource = results,
                ValueField = "_M",
                XAxisKey = "AylarAxis",
                YAxisKey = "TutarAxis"
            };

            var seriesFaiz = new BarSeries
            {
                Title = "Bu Ay Faizi",
                FillColor = OxyColors.LightGreen,
                ItemsSource = results,
                ValueField = "_MI",
                XAxisKey = "AylarAxis",
                YAxisKey = "TutarAxis"
            };

            var seriesToplamFaiz = new BarSeries
            {
                Title = "Toplam Faiz",
                FillColor = OxyColors.Orange,
                ItemsSource = results,
                ValueField = "_InterestSum",
                XAxisKey = "AylarAxis",
                YAxisKey = "TutarAxis"
            };

            var seriesBirikmisToplam = new BarSeries
            {
                Title = "Birikmiş Toplam",
                FillColor = OxyColors.MediumPurple,
                ItemsSource = results,
                ValueField = "_NMI",
                XAxisKey = "AylarAxis",
                YAxisKey = "TutarAxis"
            };

            model.Series.Add(seriesYatirilan);
            model.Series.Add(seriesFaiz);
            model.Series.Add(seriesToplamFaiz);
            model.Series.Add(seriesBirikmisToplam);

            MyModel = model;
        }

        private decimal ParseDecimalSafe(string s)
        {
            if (decimal.TryParse(s, out decimal val))
                return val;
            return 0m;
        }
    }
}