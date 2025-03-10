using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munka_nyilvantarto
{
    static class Munkak
    {
        public static string MMunkaSzam = "MVT-001";
        public static string VMunkaSzam = "EVT-001";
        public static ObservableCollection<Munka> MunkakList { get; } = new ObservableCollection<Munka>();
        public static ObservableCollection<Munka> Sz_Szamlazando { get; } = new ObservableCollection<Munka>();
        public static ObservableCollection<Munka> Sz_Megbenemfejezett { get; } = new ObservableCollection<Munka>();
        public static ObservableCollection<Munka> Sz_Befejezett { get; } = new ObservableCollection<Munka>();

        public static void munkaszamleptetese(string m)
        {
            if (m == Munkak.MMunkaSzam || m == Munkak.VMunkaSzam)
            {
                if (string.IsNullOrEmpty(m) || !m.Contains("-"))
                    throw new ArgumentException("A bemeneti string nem érvényes formátumú.");

                // Keresd meg a kötőjelet és vágd szét a stringet
                int dashIndex = m.LastIndexOf('-');

                // Vedd ki a számot, és próbáld meg átalakítani int-é
                string numberPart = m.Substring(dashIndex + 1);
                if (!int.TryParse(numberPart, out int number))
                    throw new ArgumentException("A kötőjel utáni rész nem érvényes szám.");

                // Növeld a számot
                number++;

                // Fűzd össze az eredeti prefixet az új számmal, háromjegyű formázással
                string result = m.Substring(0, dashIndex + 1) + number.ToString("D3");

                if (m == Munkak.MMunkaSzam)
                {
                    Munkak.MMunkaSzam = result;
                }
                else if (m == Munkak.VMunkaSzam)
                {
                    Munkak.VMunkaSzam = result;
                }
            }
        }
    }

    public class Munka
    {
        public string MunkaTipusa { get; set; }
        public string MunkaSzama { get; set; }
        public string MunkaNeve { get; set; }
        public string MegrendeloNeve { get; set; }
        public string TelepulesNeve { get; set; }
        public int Ajánlat_e { get; set; }
        public string Megjegyzes {  get; set; }
        public bool Fizetve {  get; set; }
        public DateTime Kezdes { get; set; }
        public DateTime Befejezes { get; set; }
        public DateTime Szamlazas { get; set; }

        public Munka(string munkaTipusa, string munkaSzama, string munkaNeve, string megrendeloNeve, string telepulesNeve, int ajánlat_e, string megjegyzes, bool fizetve, DateTime kezdés, DateTime befejezes, DateTime szamlazas)
        {
            MunkaTipusa = munkaTipusa;
            MunkaSzama = munkaSzama;
            MunkaNeve = munkaNeve;
            MegrendeloNeve = megrendeloNeve;
            TelepulesNeve = telepulesNeve;
            Ajánlat_e = ajánlat_e;
            Megjegyzes = megjegyzes;
            Fizetve = fizetve;
            Kezdes = kezdés;
            Befejezes = befejezes;
            Szamlazas = szamlazas;
        }
    }
}
