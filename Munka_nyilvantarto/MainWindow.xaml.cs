using ClosedXML.Excel;
using System.IO;
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

namespace Munka_nyilvantarto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string FilePath = "";
        public MainWindow()
        {
            InitializeComponent();

            LoadDataFromExcel();
            MunkaDataGrid.ItemsSource = Munkak.MunkakList;
        }

        private void LoadDataFromExcel()
        {
            if (FilePath == "")
                FilePath = "MunkakList.xlsx";
            // Ellenőrizzük, hogy létezik-e az Excel fájl
            if (!File.Exists(FilePath))
            {
                MessageBox.Show("Nem találhatóak az adatok", "Figyelem", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int Mmax = 0;
            int Vmax = 0;

            try
            {
                // Excel fájl megnyitása és adatok betöltése
                using (var workbook = new XLWorkbook(FilePath))
                {
                    var worksheet = workbook.Worksheet("Munkák");
                    var rows = worksheet.RangeUsed().RowsUsed();

                    // Töröljük a listát az új adatok előtt
                    Munkak.MunkakList.Clear();
                    Munkak.Sz_Szamlazando.Clear();
                    Munkak.Sz_Megbenemfejezett.Clear();

                    // Első sor fejléc, ezért a második sortól olvassuk
                    foreach (var row in rows.Skip(1))
                    {
                        string munkaTipusa = row.Cell(1).GetString();
                        string munkaSzama = row.Cell(2).GetString();
                        string munkaNeve = row.Cell(3).GetString();
                        string megrendeloNeve = row.Cell(4).GetString();
                        string telepulesNeve = row.Cell(5).GetString();
                        int ajanlat_e = row.Cell(6).TryGetValue(out int intValue) ? intValue : 0;
                        string megjegyzes = row.Cell(7).GetString();
                        bool fizetve = row.Cell(8).TryGetValue(out bool boolValue) ? boolValue : false;
                        DateTime kezdes = DateTime.MinValue;
                        if (DateTime.TryParse(row.Cell(9).GetString(), out DateTime parsedKezdes)) 
                        {
                            kezdes = parsedKezdes;
                        }
                        DateTime befejezes = DateTime.MinValue;
                        if (DateTime.TryParse(row.Cell(10).GetString(), out DateTime parsedBefejezes))
                        {
                            befejezes = parsedBefejezes;
                        }
                        DateTime szamlazas = DateTime.MinValue;
                        if (DateTime.TryParse(row.Cell(11).GetString(), out DateTime parsedSzamlazas))
                        {
                            szamlazas = parsedSzamlazas;
                        }

                        Munkak.MunkakList.Add(new Munka(
                            munkaTipusa,
                            munkaSzama,
                            munkaNeve,
                            megrendeloNeve,
                            telepulesNeve,
                            ajanlat_e,
                            megjegyzes,
                            fizetve,
                            kezdes,
                            befejezes,
                            szamlazas));
                        try
                        {
                            int szam = int.Parse(munkaSzama.Split('-')[1]);
                            if (munkaSzama.Split('-')[0] == "MVT")
                            {
                                if (szam > Mmax)
                                {
                                    Mmax = szam;
                                }
                            }
                            else if (munkaSzama.Split('-')[0] == "EVT")
                            {
                                if (szam > Vmax)
                                {
                                    Vmax = szam;
                                }
                            }
                        }
                        catch (Exception ex) 
                        { 
                        }
                    }
                    Munkak.MMunkaSzam = "MVT-" + (Mmax + 1).ToString("D3");
                    Munkak.VMunkaSzam = "EVT-" + (Vmax + 1).ToString("D3");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az adatok betöltése során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            for (int i = 0; i < Munkak.MunkakList.Count; i++)
            {
                szuro(Munkak.MunkakList[i]);
            }
            if (Munkak.Sz_Szamlazando.Count > 0)
            {
                MessageBox.Show(Munkak.Sz_Szamlazando.Count + " Munka nincs számlázva!!!");
            }
        }

        private void HozzaadButton_Click(object sender, RoutedEventArgs e)
        {
            AddMunkaWindow addMunkaWindow = new AddMunkaWindow();
            bool? result = addMunkaWindow.ShowDialog();

            if (result == true)
            {
                // Refresh the DataGrid to show the new entry
                MunkaDataGrid.ItemsSource = null;
                MunkaDataGrid.ItemsSource = Munkak.MunkakList;
                szuro(Munkak.MunkakList.Last());
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                // Fájlválasztó mentéshez, ha még nincs megadva az útvonal
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Mentés Excel fájlként",
                    FileName = "MunkakList.xlsx"
                };

                bool? result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    FilePath = saveFileDialog.FileName;
                }
                else
                {
                    return; // Ha nincs kiválasztva, ne mentse
                }
            }

            try
            {
                // Excel fájl létrehozása
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Munkák");

                    // Fejléc hozzáadása
                    worksheet.Cell(1, 1).Value = "Munka Típusa";
                    worksheet.Cell(1, 2).Value = "Munka Száma";
                    worksheet.Cell(1, 3).Value = "Munka Neve";
                    worksheet.Cell(1, 4).Value = "Megrendelő Neve";
                    worksheet.Cell(1, 5).Value = "Település Neve";
                    worksheet.Cell(1, 6).Value = "Ajánlat_e";
                    worksheet.Cell(1, 7).Value = "Megjegyzés";
                    worksheet.Cell(1, 8).Value = "Fizetve";
                    worksheet.Cell(1, 9).Value = "Kezdés";
                    worksheet.Cell(1, 10).Value = "Befejezés";
                    worksheet.Cell(1, 11).Value = "Számlázás";

                    // Adatok hozzáadása a listából
                    for (int i = 0; i < Munkak.MunkakList.Count; i++)
                    {
                        var munka = Munkak.MunkakList[i];
                        worksheet.Cell(i + 2, 1).Value = munka.MunkaTipusa;
                        worksheet.Cell(i + 2, 2).Value = munka.MunkaSzama;
                        worksheet.Cell(i + 2, 3).Value = munka.MunkaNeve;
                        worksheet.Cell(i + 2, 4).Value = munka.MegrendeloNeve;
                        worksheet.Cell(i + 2, 5).Value = munka.TelepulesNeve;
                        worksheet.Cell(i + 2, 6).Value = munka.Ajánlat_e;
                        worksheet.Cell(i + 2, 7).Value = munka.Megjegyzes;
                        worksheet.Cell(i + 2, 8).Value = munka.Fizetve;
                        worksheet.Cell(i + 2, 9).Value = munka.Kezdes.ToShortDateString();
                        worksheet.Cell(i + 2, 10).Value = munka.Befejezes.ToShortDateString();
                        worksheet.Cell(i + 2, 11).Value = munka.Szamlazas.ToShortDateString();
                    }

                    // Fájl mentése
                    workbook.SaveAs(FilePath);
                    MessageBox.Show($"Az adatok sikeresen mentve lettek: {FilePath}", "Sikeres mentés", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Alkalmazás bezárása
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a mentés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportButtonClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Válassz egy Excel fájlt"
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                FilePath = openFileDialog.FileName; // Fájl elérési útjának beállítása
                LoadDataFromExcel(); // Adatok betöltése az adott fájlból
            }
        }

        private void SzerkesztesButton_Click(object sender, RoutedEventArgs e)
        {
            Window selectMunkaWindow = new Window
            {
                Title = "Munka Kiválasztása",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            // Létrehozunk egy ComboBox-ot a munkaszámok listázásához
            ComboBox munkaSzamComboBox = new ComboBox
            {
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                DisplayMemberPath = "MunkaSzama",
                ItemsSource = Munkak.MunkakList
            };

            // Létrehozunk egy "Kiválasztás" gombot
            Button selectButton = new Button
            {
                Content = "Kiválasztás",
                Width = 100,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Gomb eseménykezelő
            selectButton.Click += (s, args) =>
            {
                if (munkaSzamComboBox.SelectedItem is Munka selectedMunka)
                {
                    selectMunkaWindow.DialogResult = true;
                    selectMunkaWindow.Close();

                    // Megnyitjuk az AddMunkaWindow ablakot a kiválasztott munka adataival
                    var editWindow = new AddMunkaWindow(selectedMunka);
                    editWindow.ShowDialog();

                    // Frissítsük a DataGrid-et, hogy megjelenjenek az új értékek
                    MunkaDataGrid.ItemsSource = null;
                    MunkaDataGrid.ItemsSource = Munkak.MunkakList;
                }
                else
                {
                    MessageBox.Show("Kérlek, válassz ki egy munkaszámot!", "Figyelem", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            // Ablak tartalmának beállítása egy StackPanel segítségével
            StackPanel panel = new StackPanel();
            panel.Children.Add(munkaSzamComboBox);
            panel.Children.Add(selectButton);

            selectMunkaWindow.Content = panel;

            // Megnyitjuk a SelectMunkaWindow ablakot
            selectMunkaWindow.ShowDialog();
            szuro(Munkak.MunkakList.Last());
        }

        public void szuro(Munka m)
        {
            if (m.Szamlazas == DateTime.MinValue && m.Befejezes == DateTime.MinValue)
            {
                Munkak.Sz_Megbenemfejezett.Add(m);
            }
            else if (m.Szamlazas == DateTime.MinValue && m.Kezdes <= DateTime.Now - TimeSpan.FromDays(30))
            {
                Munkak.Sz_Szamlazando.Add(m);
            }
            else
            {
                Munkak.Sz_Befejezett.Add(m);
            }
        }

        private void SzuresButton_Click(object sender, RoutedEventArgs e)
        {
            // Létrehozunk egy új ablakot
            Window szuroWindow = new Window
            {
                Title = "Szűrők",
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            // Létrehozunk egy ComboBox-ot
            ComboBox szuroComboBox = new ComboBox
            {
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Szűrők hozzáadása a ComboBox-hoz
            szuroComboBox.Items.Add("Nincs szűrő");
            szuroComboBox.Items.Add("Számlázandó");
            szuroComboBox.Items.Add("Meg nem fejezett");
            szuroComboBox.Items.Add("Befejezett");
            szuroComboBox.SelectedIndex = 0; // Alapértelmezett kiválasztás

            // "Alkalmaz" gomb létrehozása
            Button alkalmazButton = new Button
            {
                Content = "Alkalmaz",
                Width = 100,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Gomb eseménykezelő
            alkalmazButton.Click += (s, args) =>
            {
                string selectedFilter = szuroComboBox.SelectedItem.ToString();

                // Szűrés a kiválasztott lehetőség alapján
                if (selectedFilter == "Nincs szűrő")
                {
                    MunkaDataGrid.ItemsSource = Munkak.MunkakList;
                }
                else if (selectedFilter == "Számlázandó")
                {
                    MunkaDataGrid.ItemsSource = Munkak.Sz_Szamlazando;
                }
                else if (selectedFilter == "Meg nem fejezett")
                {
                    MunkaDataGrid.ItemsSource = Munkak.Sz_Megbenemfejezett;
                }
                else if (selectedFilter == "Befejezett")
                {
                    MunkaDataGrid.ItemsSource = Munkak.Sz_Befejezett;
                }

                // Ablak bezárása
                szuroWindow.Close();
            };

            // StackPanel létrehozása a ComboBox és Gomb hozzáadásához
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(szuroComboBox);
            stackPanel.Children.Add(alkalmazButton);

            // Az ablak tartalmának beállítása
            szuroWindow.Content = stackPanel;

            // Ablak megnyitása
            szuroWindow.ShowDialog();
        }

        private void StatisztikaButton_Click(object sender, RoutedEventArgs e)
        {
            var statisztikaWindow = new Window1();
            statisztikaWindow.ShowDialog();
        }

    }
}