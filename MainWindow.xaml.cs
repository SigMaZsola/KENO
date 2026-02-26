using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kenó
{
    public partial class MainWindow : Window
    {
        List<NapiKeno> huzasok = new List<NapiKeno>();
        List<int> tippek = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
            FeltoltSzamGrid();
            lbHuzasok.SelectionChanged += lbHuzasok_SelectionChanged;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReadFile();
        }

        private void ReadFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == false) return;

            lbHuzasok.Items.Clear();
            huzasok.Clear();

            string[] sorok = File.ReadAllLines(ofd.FileName, Encoding.UTF8);
            sorok = sorok.Skip(1).ToArray();

            int hibasDb = 0;

            foreach (var sor in sorok)
            {
                NapiKeno huzas = new NapiKeno(sor);
                huzasok.Add(huzas);
                lbHuzasok.Items.Add(sor);

                if (huzas.hibas)
                    hibasDb++;
            }

            if (hibasDb > 0)
            {
                MessageBox.Show(
                    $"Összesen {hibasDb} hibás sor volt.",
                    "Hibás adatok",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void lbHuzasok_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ErdemesSzamolni();
        }

        private void FeltoltSzamGrid()
        {
            grNumberField.Children.Clear();

            int szam = 1;
            for (int sor = 0; sor < 10; sor++)
            {
                for (int oszlop = 0; oszlop < 8; oszlop++)
                {
                    Button btn = new Button();
                    btn.Content = szam.ToString();
                    btn.Tag = szam;
                    btn.Margin = new Thickness(2);
                    btn.Background = Brushes.LightGreen;
                    btn.Click += SzamGomb_Click;

                    Grid.SetRow(btn, sor);
                    Grid.SetColumn(btn, oszlop);

                    grNumberField.Children.Add(btn);
                    szam++;
                }
            }
        }

        private void SzamGomb_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int szam = (int)btn.Tag;

            if (!tippek.Contains(szam))
            {
                if (tippek.Count == 10)
                {
                    MessageBox.Show("Már 10 szám van kiválasztva!");
                    return;
                }

                tippek.Add(szam);
                btn.Background = Brushes.Yellow;
            }
            else
            {
                tippek.Remove(szam);
                btn.Background = Brushes.LightGreen;
            }

            FrissitLabel();

            // Ha 10 tipp van, lezárjuk a tét mezőt
            if (tippek.Count == 10)
            {
                tbOsszeg.IsReadOnly = true;  // már nem írható :contentReference[oaicite:2]{index=2}
            }

            ErdemesSzamolni();
        }

        private void FrissitLabel()
        {
            if (tippek.Count == 0)
            {
                lTippCointainer.Content = "Tippek: (még nincs)";
                return;
            }
            lTippCointainer.Content = "Tippek: " + string.Join(", ", tippek.OrderBy(x => x));
        }

        private void tbOsszeg_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErdemesSzamolni();
        }

        private void ErdemesSzamolni()
        {
            if (!int.TryParse(tbOsszeg.Text, out int tet))
            {
                lNyeremeny.Foreground = Brushes.Red;
                lNyeremeny.Content = "Adj meg egy érvényes számot!";
                return;
            }

            int[] engedelyezettTetek = { 100, 200, 400, 600, 800, 1000 };
            if (!engedelyezettTetek.Contains(tet))
            {
                lNyeremeny.Foreground = Brushes.Red;
                lNyeremeny.Content = "A tét csak 100, 200, 400, 600, 800 vagy 1000 lehet!";
                return;
            }

            if (tippek.Count < 10)
            {
                lNyeremeny.Foreground = Brushes.Red;
                lNyeremeny.Content = "Add meg mind a 10 tippet!";
                return;
            }

            if (lbHuzasok.SelectedIndex < 0)
            {
                lNyeremeny.Foreground = Brushes.Red;
                lNyeremeny.Content = "Válassz egy sorsolást!";
                return;
            }

            SzamolNyeremeny(lbHuzasok.SelectedIndex);
        }

        private void SzamolNyeremeny(int index)
        {
            int tet = int.Parse(tbOsszeg.Text);
            var huzott = huzasok[index].huzottSzamok;
            int talalatok = tippek.Count(tipp => huzott.Contains(tipp));

            Dictionary<string, int> nyeroParok = new Dictionary<string, int>()
    {
        {"10-10",1000000}, {"10-9",8000}, {"10-8",350},
        {"9-9",100000}, {"9-8",1200}, {"9-7",100},
        {"8-8",20000}, {"8-7",350}, {"8-6",25},
        {"7-7",5000}, {"7-6",60}, {"7-5",6},
        {"6-6",500}, {"6-5",20},
        {"5-5",200}, {"5-4",10}, {"5-3",2},
        {"4-4",100}, {"4-3",2},
        {"3-3",15}, {"3-2",1},
        {"2-2",6},
        {"1-1",2}
    };

            string kulcs = $"{tippek.Count}-{talalatok}";
            int szorz = nyeroParok.ContainsKey(kulcs) ? nyeroParok[kulcs] : 0;
            int nyeremeny = tet * szorz;

            if (szorz == 0)
            {
                lNyeremeny.Foreground = Brushes.Red;
                lNyeremeny.Content = $"Találatok: {talalatok} – Sajnos nem nyertél!";
            }
            else
            {
                lNyeremeny.Foreground = Brushes.Green;
                lNyeremeny.Content = $"Találatok: {talalatok}   Szorzó: {szorz}x  |  Nyeremény: {nyeremeny:N0} Ft";
            }
        }

        private void btnKiszamol_Click(object sender, RoutedEventArgs e)
        {
            ErdemesSzamolni();
        }
    }
    }