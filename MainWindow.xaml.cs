
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Gra2D
{
    public partial class MainWindow : Window
    {
        public const int LAS = 1;
        public const int LAKA = 2;
        public const int SKALA = 3;
        public const int Zelazo = 4;
        public const int ILE_TERENOW = 5;

        private int[,] mapa;
        private int szerokoscMapy;
        private int wysokoscMapy;
        private Image[,] tablicaTerenu;
        private const int RozmiarSegmentuW = 100;
        private const int RozmiarSegmentuH = 100;

        private BitmapImage[] obrazyTerenu = new BitmapImage[ILE_TERENOW];

        private int pozycjaGraczaX = 0;
        private int pozycjaGraczaY = 0;
        private Image obrazGracza;
        private int iloscDrewna = 0;
        private int IloscKamienia = 0;
        private int IleZelaza = 0;
        private int licznikResetow = 0;

        public MainWindow()
        {
            InitializeComponent();
            WczytajObrazyTerenu();
            obrazGracza = new Image
            {
                Width = RozmiarSegmentuW,
                Height = RozmiarSegmentuH
            };
            BitmapImage bmpGracza = new BitmapImage(new Uri("gracz.png", UriKind.Relative));
            obrazGracza.Source = bmpGracza;
            try
            {
                BitmapImage testImg = new BitmapImage(new Uri("Zelazo.png", UriKind.Relative));
                testImage.Source = testImg;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się załadować obrazu: " + ex.Message);
            }

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MenuItem klik = sender as MenuItem;
            if (klik != null)
            {
                string naglowek = klik.Header.ToString();
                string sciezka = "";
                switch (naglowek)
                {
                    case "Tung": sciezka = "C:\\Users\\jakub\\source\\repos\\tralalalalalal\\Tung.jpg"; break;
                    case "tralalelo": sciezka = "C:\\Users\\jakub\\source\\repos\\tralalalalalal\\Trala.jpg"; break;
                    case "Battler": sciezka = "C:\\Users\\jakub\\source\\repos\\tralalalalalal\\Battler.jpg"; break;
                }
                if (!string.IsNullOrEmpty(sciezka))
                {
                    BitmapImage nowyGracz = new BitmapImage(new Uri(sciezka, UriKind.Absolute));
                    obrazGracza.Source = nowyGracz;
                }
            }
        }

        private void GenerujMape(int ileSegmentowX, int ileSegmentowY, int Tryb)
        {


            Random rnd = new Random();
            using (StreamWriter writer = new StreamWriter("mapa.txt"))
            {
                for (int i = 0; i < ileSegmentowX; i++)
                {
                    for (int j = 0; j < ileSegmentowY; j++)
                    {
                        int losowanie = rnd.Next(100);
                        if (Tryb == 1)
                        {
                            if (losowanie < 40) writer.Write("1 ");
                            else if (losowanie <= 80) writer.Write("2 ");
                            else writer.Write("3 ");
                        }
                        else if (Tryb == 2)
                        {

                            if (losowanie < 60)
                            {
                                writer.Write("1 ");
                            }
                            else
                            {
                                writer.Write("4 ");
                            }
                        }




                    }
                    writer.WriteLine();
                }

            }


        }

        private void WczytajObrazyTerenu()
        {
            obrazyTerenu[LAS] = new BitmapImage(new Uri("las.png", UriKind.Relative));
            obrazyTerenu[LAKA] = new BitmapImage(new Uri("laka.png", UriKind.Relative));
            obrazyTerenu[SKALA] = new BitmapImage(new Uri("skala.png", UriKind.Relative));
            obrazyTerenu[Zelazo] = new BitmapImage(new Uri("Zelazo.png", UriKind.Relative));

        }

        private void WczytajMape(string sciezkaPliku)
        {
            try
            {
                var linie = File.ReadAllLines(sciezkaPliku);
                wysokoscMapy = linie.Length;
                szerokoscMapy = linie[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                mapa = new int[wysokoscMapy, szerokoscMapy];

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    var czesci = linie[y].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        mapa[y, x] = int.Parse(czesci[x]);
                    }
                }

                SiatkaMapy.Children.Clear();
                SiatkaMapy.RowDefinitions.Clear();
                SiatkaMapy.ColumnDefinitions.Clear();

                for (int y = 0; y < wysokoscMapy; y++)
                    SiatkaMapy.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(RozmiarSegmentuH) });
                for (int x = 0; x < szerokoscMapy; x++)
                    SiatkaMapy.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(RozmiarSegmentuW) });

                tablicaTerenu = new Image[wysokoscMapy, szerokoscMapy];
                for (int y = 0; y < wysokoscMapy; y++)
                {
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        Image obraz = new Image { Width = RozmiarSegmentuW, Height = RozmiarSegmentuH };
                        int rodzaj = mapa[y, x];
                        if (rodzaj >= 1 && rodzaj < obrazyTerenu.Length)
                            obraz.Source = obrazyTerenu[rodzaj];
                        else
                            obraz.Source = null;

                        Grid.SetRow(obraz, y);
                        Grid.SetColumn(obraz, x);
                        SiatkaMapy.Children.Add(obraz);
                        tablicaTerenu[y, x] = obraz;
                    }
                }

                SiatkaMapy.Children.Add(obrazGracza);
                Panel.SetZIndex(obrazGracza, 1);
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        private void AktualizujPozycjeGracza()
        {
            Grid.SetRow(obrazGracza, pozycjaGraczaY);
            Grid.SetColumn(obrazGracza, pozycjaGraczaX);
        }

        private void WykopSkale()
        {
            int[] kierunkiX = { -1, 1, 0, 0 };
            int[] kierunkiY = { 0, 0, -1, 1 };
            for (int i = 0; i < 4; i++)
            {
                int noweX = pozycjaGraczaX + kierunkiX[i];
                int noweY = pozycjaGraczaY + kierunkiY[i];

                if (noweX >= 0 && noweX < szerokoscMapy && noweY >= 0 && noweY < wysokoscMapy)
                {
                    if (mapa[noweY, noweX] == SKALA)
                    {
                        mapa[noweY, noweX] = LAKA;
                        tablicaTerenu[noweY, noweX].Source = obrazyTerenu[LAKA];
                        IloscKamienia++;
                        EtykietaKamienia.Content = "Kamień: " + IloscKamienia;
                        break;
                    }
                }
            }
        }
        private void WykopZelazo()
        {
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = pozycjaGraczaX + dx[i];
                int ny = pozycjaGraczaY + dy[i];

                if (nx >= 0 && nx < szerokoscMapy && ny >= 0 && ny < wysokoscMapy)
                {
                    if (mapa[ny, nx] == Zelazo)
                    {
                        mapa[ny, nx] = LAKA;
                        tablicaTerenu[ny, nx].Source = obrazyTerenu[LAKA];
                        IleZelaza++;
                        EtykietaZelaza.Content = "Żelazo: " + IleZelaza;
                        break;
                    }
                }
            }
        }

        private void OnWykop(object sender, RoutedEventArgs e)
        {
            WykopSkale();
        }

        private void OknoGlowne_KeyDown(object sender, KeyEventArgs e)
        {

            int nowyX = pozycjaGraczaX;
            int nowyY = pozycjaGraczaY;

            if (e.Key == Key.Up) nowyY--;
            else if (e.Key == Key.Down) nowyY++;
            else if (e.Key == Key.Left) nowyX--;
            else if (e.Key == Key.Right) nowyX++;

            if (nowyX >= 0 && nowyX < szerokoscMapy && nowyY >= 0 && nowyY < wysokoscMapy)
            {
                if (mapa[nowyY, nowyX] != SKALA && mapa[nowyY, nowyX] != Zelazo)
                {
                    pozycjaGraczaX = nowyX;
                    pozycjaGraczaY = nowyY;
                    AktualizujPozycjeGracza();
                }
            }


            if (e.Key == Key.C)
            {
                if (mapa[pozycjaGraczaY, pozycjaGraczaX] == LAS)
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = LAKA;
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[LAKA];
                    iloscDrewna++;
                    EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
                }
            }
            if (iloscDrewna > 4)
            {
                wyswietl.Content = "Odblokowano Drewniany Kilof!!! Kliknij: R";
                if (e.Key == Key.R)
                {
                    WykopSkale();
                }
            }
            if (IloscKamienia > 2)
            {
                wyswietl.Content = "Odblokowano Kamienny Kilof!!! Kliknij: Q";
                if (e.Key == Key.Q)
                {
                    WykopZelazo();
                    WykopSkale();
                }
            }

            bool reset = true;
            for (int i = 0; i < mapa.GetLength(0); i++)
            {
                for (int j = 0; j < mapa.GetLength(1); j++)
                {
                    if (mapa[i, j] != LAKA)
                    {
                        reset = false;
                        break;
                    }
                }
            }
            if (reset == true)
            {
                licznikResetow++;

                if (licznikResetow >= 2)
                {
                    wyswietl.Content = "Koniec gry!";
                    
                    return;
                }
                else
                {
                    GenerujMape(wysokoscMapy, szerokoscMapy, 2);
                    WczytajMape("mapa.txt");
                }
            }
            if (e.Key == Key.J) 
            {
                licznikResetow = 0;
            }

            if (e.Key == Key.D)
            {
                GenerujMape(wysokoscMapy, szerokoscMapy, 1);
                WczytajMape("mapa.txt");

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem KLIK = sender as MenuItem;
            if (KLIK != null)
            {
                string naglowek = KLIK.Header.ToString();
                switch (naglowek)
                {
                    case "Sterowanie":
                        wyswietl.Content = "Poruszaj Się Strzałkami";
                        break;
                    case "Akcja":
                        wyswietl.Content = "Zbieranie Drewna: C, Reset: D";
                        break;
                    case "Exit":
                        Application.Current.Shutdown();
                        break;
                }
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            GenerujMape(5, 5, 1);
            WczytajMape("mapa.txt");
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            GenerujMape(6, 6, 1);
            WczytajMape("mapa.txt");
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            GenerujMape(7, 7, 1);
            WczytajMape("mapa.txt");
        }
    }
}
