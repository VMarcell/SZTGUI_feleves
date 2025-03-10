using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace Munka_nyilvantarto
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            // CheckBox események
            MvtCheckBox.Checked += (s, e) => { UpdateTableData("MVT"); DeselectOthers(MvtCheckBox); };
            EvtCheckBox.Checked += (s, e) => { UpdateTableData("EVT"); DeselectOthers(EvtCheckBox); };
            AllCheckBox.Checked += (s, e) => { UpdateTableData(null); DeselectOthers(AllCheckBox); };

            // Alapértelmezett adatok
            UpdateTableData(null);

            // Statisztikai adatok betöltése
            LoadStatistics();
        }

        private void DeselectOthers(CheckBox selectedCheckBox)
        {
            foreach (var checkBox in new[] { MvtCheckBox, EvtCheckBox, AllCheckBox })
            {
                if (checkBox != selectedCheckBox)
                {
                    checkBox.IsChecked = false;
                }
            }
        }

        private void LoadStatistics()
        {
            // Átlagos napok kezdés és befejezés között
            KezdesBefejezesStat.Text = "Átlagos napok kezdés és befejezés között: " + GetAverageDaysStartToFinish();

            // Átlagos napok számlázás és befejezés között
            SzamlazasBefejezesStat.Text = "Átlagos napok számlázás és befejezés között: " + GetAverageDaysBillingToFinish();

            // Munkatípusok darabszáma
            var workTypeCounts = GetWorkTypeCounts();
            MunkatipusStat.Children.Clear();
            foreach (var pair in workTypeCounts)
            {
                MunkatipusStat.Children.Add(new TextBlock { Text = $"{pair.Key}: {pair.Value}" });
            }

            // Legaktívabb megrendelők
            var topClients = GetTopClients();
            TopClientsStat.Children.Clear();
            foreach (var client in topClients)
            {
                TopClientsStat.Children.Add(new TextBlock { Text = client });
            }
        }

        private void UpdateTableData(string prefix)
        {
            StatDataGrid.ItemsSource = GetFilteredStatistics(prefix);
        }

        private double GetAverageDaysStartToFinish()
        {
            var completedWorks = Munkak.MunkakList.Where(m => m.Kezdes != DateTime.MinValue && m.Befejezes != DateTime.MinValue);
            return completedWorks.Any() ? completedWorks.Average(m => (m.Befejezes - m.Kezdes).TotalDays) : 0;
        }

        private double GetAverageDaysBillingToFinish()
        {
            var billedWorks = Munkak.MunkakList.Where(m => m.Befejezes != DateTime.MinValue && m.Szamlazas != DateTime.MinValue);
            return billedWorks.Any() ? billedWorks.Average(m => (m.Szamlazas - m.Befejezes).TotalDays) : 0;
        }

        private Dictionary<string, int> GetWorkTypeCounts()
        {
            return Munkak.MunkakList
                .GroupBy(m => m.MunkaTipusa)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private List<string> GetTopClients()
        {
            return Munkak.MunkakList
                .GroupBy(m => m.MegrendeloNeve)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => $"{g.Key}: {g.Count()} munka")
                .ToList();
        }

        private List<StatisticRow> GetFilteredStatistics(string prefix)
        {
            var stats = new List<StatisticRow>();

            for (int month = 1; month <= 12; month++)
            {
                var worksInMonth = Munkak.MunkakList
                    .Where(m => m.Szamlazas.Month == month && (prefix == null || m.MunkaSzama.StartsWith(prefix)));

                stats.Add(new StatisticRow
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    Count = worksInMonth.Count(),
                    TotalValue = worksInMonth.Sum(m => m.Ajánlat_e)
                });
            }

            stats.Add(new StatisticRow
            {
                Month = "Összesen",
                Count = stats.Sum(s => s.Count),
                TotalValue = stats.Sum(s => s.TotalValue)
            });

            return stats;
        }
    }

    public class StatisticRow
    {
        public string Month { get; set; }
        public int Count { get; set; }
        public int TotalValue { get; set; }
    }
}
