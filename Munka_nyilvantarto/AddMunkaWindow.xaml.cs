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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Munka_nyilvantarto
{
    /// <summary>
    /// Interaction logic for AddMunkaWindow.xaml
    /// </summary>
    public partial class AddMunkaWindow : Window
    {
        private Munka _munka;
        public AddMunkaWindow()
        {
            InitializeComponent();
            KezdesDatePicker.SelectedDate = DateTime.Now;
            MunkaSzamaTextBox.Text = Munkak.MMunkaSzam;
            AjánlatTextBox.Text = "0";
        }
        public AddMunkaWindow(Munka munka) : this()
        {
            _munka = munka;

            // Mezők kitöltése az átadott munka adataival
            MunkaTipusaComboBox.SelectedItem = munka.MunkaTipusa;
            MunkaSzamaTextBox.Text = munka.MunkaSzama;
            MunkaneveTextBox.Text = munka.MunkaNeve;
            MegrendeloNeveTextBox.Text = munka.MegrendeloNeve;
            telepulesneveTextBox.Text = munka.TelepulesNeve;
            FizetveCheckBox.IsChecked = munka.Fizetve;
            KezdesDatePicker.SelectedDate = munka.Kezdes;
            AjánlatTextBox.Text = Convert.ToString(munka.Ajánlat_e);
            MegjegyzesTextBox.Text = munka.Megjegyzes;
            BefejezesDatePicker.SelectedDate = munka.Befejezes;
            SzamlazasDatePicker.SelectedDate = munka.Szamlazas;

            // Törlés gomb láthatóvá tétele
            DeleteButton.Visibility = Visibility.Visible;
            Keszbutton.Visibility = Visibility.Visible;
            Addbutton.Visibility = Visibility.Collapsed;
        }

        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            bool helyesadatok = true;
            // Get the input values
            string munkaTipusa = (MunkaTipusaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string munkaSzama = MunkaSzamaTextBox.Text;
            string munkaneve = MunkaneveTextBox.Text;
            string megrendeloneve = MegrendeloNeveTextBox.Text;
            string telepulesneve = telepulesneveTextBox.Text;
            int ajánlat = int.Parse(AjánlatTextBox.Text);
            string megjegyzes = MegjegyzesTextBox.Text;
            bool fizetve = FizetveCheckBox.IsChecked ?? false;
            DateTime? kezdes = KezdesDatePicker.SelectedDate;
            DateTime? szamlazas = SzamlazasDatePicker.SelectedDate;
            DateTime? befejezes = BefejezesDatePicker.SelectedDate;

            // Check if the required fields are filled
            if (string.IsNullOrWhiteSpace(munkaTipusa) || string.IsNullOrWhiteSpace(munkaSzama))
            {
                MessageBox.Show("Kevés adat", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (ajánlat == null)
            {
                ajánlat = 0;
            }

            for (int i = 0; i < Munkak.MunkakList.Count; i++)
            {
                if (Munkak.MunkakList[i].MunkaSzama == munkaSzama)
                {
                    MessageBox.Show(munkaSzama + " Számú munka már létezik!");
                    helyesadatok = false;
                }
            }

            Munkak.munkaszamleptetese(munkaSzama);

            if (helyesadatok)
            {

                // Add the new Munka to the list
                Munkak.MunkakList.Add(new Munka(
                munkaTipusa, 
                munkaSzama, 
                munkaneve, 
                megrendeloneve, 
                telepulesneve, 
                ajánlat, 
                megjegyzes, 
                fizetve, 
                Convert.ToDateTime(kezdes), 
                Convert.ToDateTime(befejezes), 
                Convert.ToDateTime(szamlazas)));

                // Close the window after adding
                this.DialogResult = true;
                this.Close();
            }
        }

        

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_munka != null && Munkak.MunkakList.Contains(_munka))
            {
                Munkak.MunkakList.Remove(_munka);
            }
            this.DialogResult = true;
            this.Close();
        }
        private void keszButtonClick(object sender, RoutedEventArgs e)
        {
            Munkak.MunkakList.Remove(_munka);
            for (int i = 0; i < Munkak.Sz_Megbenemfejezett.Count; i++)
            {
                if (MunkaSzamaTextBox.Text == Munkak.Sz_Megbenemfejezett[i].MunkaSzama)
                    Munkak.Sz_Megbenemfejezett.Remove(_munka);
            }
            for (int i = 0; i < Munkak.Sz_Szamlazando.Count; i++)
            {
                if (MunkaSzamaTextBox.Text == Munkak.Sz_Szamlazando[i].MunkaSzama)
                    Munkak.Sz_Szamlazando.Remove(_munka);
            }
            for (int i = 0; i < Munkak.Sz_Befejezett.Count; i++)
            {
                if (MunkaSzamaTextBox.Text == Munkak.Sz_Befejezett[i].MunkaSzama)
                    Munkak.Sz_Befejezett.Remove(_munka);
            }
            OnAddButtonClick(sender, e);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MunkaSzamElotag_Checked(object sender, RoutedEventArgs e)
        {
            MunkaSzamaTextBox.Text = Munkak.VMunkaSzam;
        }
        private void MunkaSzamElotag_Unchecked(object seder, RoutedEventArgs e)
        {
            MunkaSzamaTextBox.Text = Munkak.MMunkaSzam;
        }
    }
}
