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
        // Stałe reprezentujące rodzaje terenu
        public const int LAS = 1;     // las
        public const int LAKA = 2;     // łąka
        public const int SKALA = 3;   // skały
        public const int ILE_TERENOW = 4;   // ile terenów
        // Mapa przechowywana jako tablica dwuwymiarowa int
        private int[,] mapa;
        private int szerokoscMapy;
        private int wysokoscMapy;
        // Dwuwymiarowa tablica kontrolek Image reprezentujących segmenty mapy
        private Image[,] tablicaTerenu;
        // Rozmiar jednego segmentu mapy w pikselach
        private const int RozmiarSegmentuW = 64;
        private const int RozmiarSegmentuH = 64;

        // Tablica obrazków terenu – indeks odpowiada rodzajowi terenu
        // Indeks 1: las, 2: łąka, 3: skały
        private BitmapImage[] obrazyTerenu = new BitmapImage[ILE_TERENOW];

        // Pozycja gracza na mapie
        private int pozycjaGraczaX = 0;
        private int pozycjaGraczaY = 0;
        // Obrazek reprezentujący gracza
        private Image obrazGracza;
        // Licznik zgromadzonego drewna
        private int iloscDrewna = 0;
        private int IloscKamienia = 0;
        public MainWindow()
        {
            InitializeComponent();
            WczytajObrazyTerenu();

            // Inicjalizacja obrazka gracza
            obrazGracza = new Image
            {
                Width = RozmiarSegmentuW,
                Height = RozmiarSegmentuH
            };

                BitmapImage bmpGracza = new BitmapImage(new Uri("gracz.png", UriKind.Relative));
            obrazGracza.Source = bmpGracza;
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
                    case "Tung":
                        sciezka = "C:\\Users\\jakub\\source\\repos\\tralalalalalal\\Tung.jpg";
                        break;
                    case "tralalelo":
                        sciezka = "C:\\Users\\jakub\\source\\repos\\tralalalalalal\\Trala.jpg";
                        break;
                    case "Battler":
                        sciezka = "C:\\Users\\jakub\\source\\repos\\tralalalalalal\\Battler.jpg";
                        break;
                }

                // Ładujemy obrazek TYLKO jeśli ścieżka nie jest pusta!
                if (!string.IsNullOrEmpty(sciezka))
                {
                    MainImage.Source = new BitmapImage(new Uri(sciezka, UriKind.Absolute));
                    BitmapImage nowyGracz = new BitmapImage(new Uri(sciezka, UriKind.Absolute));
                    obrazGracza.Source = nowyGracz;
                }
            }
        }

        private void GenerujMape(int ileSegmentowX, int ileSegmentowY) 
        {
            Random rnd = new Random();
            using (StreamWriter writer = new StreamWriter("mapa.txt"))
            {
                for (int i = 0; i < ileSegmentowX; i++)
                {
                    for (int j = 0; j < ileSegmentowY; j++)
                    {
                        int losowanie = rnd.Next(100);
                        if (losowanie < 40)
                            writer.Write("1 ");
                        else if (losowanie <= 80)
                            writer.Write("2 ");
                        else
                            writer.Write("3 ");
                    }
                    writer.WriteLine();
                }
            }

        }
        private void WczytajObrazyTerenu()
        {
            // Zakładamy, że tablica jest indeksowana od 0, ale używamy indeksów 1-3
            obrazyTerenu[LAS] = new BitmapImage(new Uri("las.png", UriKind.Relative));
            obrazyTerenu[LAKA] = new BitmapImage(new Uri("laka.png", UriKind.Relative));
            obrazyTerenu[SKALA] = new BitmapImage(new Uri("skala.png", UriKind.Relative));
        }

        // Wczytuje mapę z pliku tekstowego i dynamicznie tworzy tablicę kontrolek Image
        private void WczytajMape(string sciezkaPliku)
        {
            try
            {
                var linie = File.ReadAllLines(sciezkaPliku);//zwraca tablicę stringów, np. linie[0] to pierwsza linia pliku
                wysokoscMapy = linie.Length;
                szerokoscMapy = linie[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;//zwraca liczbę elementów w tablicy
                mapa = new int[wysokoscMapy, szerokoscMapy];

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    var czesci = linie[y].Split(' ', StringSplitOptions.RemoveEmptyEntries);//zwraca tablicę stringów np. czesci[0] to pierwszy element linii
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        mapa[y, x] = int.Parse(czesci[x]);//wczytanie mapy z pliku
                    }
                }

                // Przygotowanie kontenera SiatkaMapy – czyszczenie elementów i definicji wierszy/kolumn
                SiatkaMapy.Children.Clear();
                SiatkaMapy.RowDefinitions.Clear();
                SiatkaMapy.ColumnDefinitions.Clear();

                for (int y = 0; y < wysokoscMapy; y++)
                {
                    SiatkaMapy.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(RozmiarSegmentuH) });
                }
                for (int x = 0; x < szerokoscMapy; x++)
                {
                    SiatkaMapy.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(RozmiarSegmentuW) });
                }

                // Tworzenie tablicy kontrolk Image i dodawanie ich do siatki
                tablicaTerenu = new Image[wysokoscMapy, szerokoscMapy];
                for (int y = 0; y < wysokoscMapy; y++)
                {
                    for (int x = 0; x < szerokoscMapy; x++)
                    {
                        Image obraz = new Image
                        {
                            Width = RozmiarSegmentuW,
                            Height = RozmiarSegmentuH
                        };

                        int rodzaj = mapa[y, x];
                        if (rodzaj >= 1 && rodzaj < ILE_TERENOW)
                        {
                            obraz.Source = obrazyTerenu[rodzaj];//wczytanie obrazka terenu
                        }
                        else
                        {
                            obraz.Source = null;
                        }

                        Grid.SetRow(obraz, y);
                        Grid.SetColumn(obraz, x);
                        SiatkaMapy.Children.Add(obraz);//dodanie obrazka do siatki na ekranie
                        tablicaTerenu[y, x] = obraz;
                    }
                }

                // Dodanie obrazka gracza – ustawiamy go na wierzchu
                SiatkaMapy.Children.Add(obrazGracza);
                Panel.SetZIndex(obrazGracza, 1);//ustawienie obrazka gracza na wierzchu
                pozycjaGraczaX = 0;
                pozycjaGraczaY = 0;
                AktualizujPozycjeGracza();

                iloscDrewna = 0;
                EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
                IloscKamienia = 0;
                EtykietaKamienia.Content = "Kamień: " + IloscKamienia;
            }//koniec try
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania mapy: " + ex.Message);
            }
        }

        // Aktualizuje pozycję obrazka gracza w siatce
        private void AktualizujPozycjeGracza()
        {
            Grid.SetRow(obrazGracza, pozycjaGraczaY);
            Grid.SetColumn(obrazGracza, pozycjaGraczaX);
        }
        private void WykopSkale()
        {
            // Sprawdzamy pola wokół gracza (góra, dół, lewo, prawo)
            int[] kierunkiX = { -1, 1, 0, 0 }; // lewo, prawo, góra, dół
            int[] kierunkiY = { 0, 0, -1, 1 };
            for (int i = 0; i < 4; i++)
            {
                int noweX = pozycjaGraczaX + kierunkiX[i];
                int noweY = pozycjaGraczaY + kierunkiY[i];

                // Sprawdzamy, czy noweX i noweY są w granicach mapy
                if (noweX >= 0 && noweX < szerokoscMapy && noweY >= 0 && noweY < wysokoscMapy)
                {
                    // Jeśli w sąsiednim polu jest SKALA, zmieniamy ją na LAKA
                    if (mapa[noweY, noweX] == SKALA)
                    {
                        mapa[noweY, noweX] = LAKA; // Zmiana pola
                        tablicaTerenu[noweY, noweX].Source = obrazyTerenu[LAKA]; // Zmiana obrazu w siatce
                        IloscKamienia++; // Gracz zdobywa kamień
                        EtykietaKamienia.Content = "Kamień: " + IloscKamienia;
                        break; // Tylko pierwsze napotkane pole z kamieniem zmieniamy na LAKA
                    }
                }
            }
        }
        private void OnWykop(object sender, RoutedEventArgs e)
        {
            WykopSkale();
        }

        // Obsługa naciśnięć klawiszy – ruch gracza oraz wycinanie lasu
        private void OknoGlowne_KeyDown(object sender, KeyEventArgs e)
        {
            int nowyX = pozycjaGraczaX;
            int nowyY = pozycjaGraczaY;
            //zmiana pozycji gracza
            if (e.Key == Key.Up) nowyY--;
            else if (e.Key == Key.Down) nowyY++;
            else if (e.Key == Key.Left) nowyX--;
            else if (e.Key == Key.Right) nowyX++;
            //Gracz nie może wyjść poza mapę
            if (nowyX >= 0 && nowyX < szerokoscMapy && nowyY >= 0 && nowyY < wysokoscMapy)
            {
                // Gracz nie może wejść na pole ze skałami
                if (mapa[nowyY, nowyX] != SKALA)
                {
                    pozycjaGraczaX = nowyX;
                    pozycjaGraczaY = nowyY;
                    AktualizujPozycjeGracza();
                }
            }

            // Obsługa wycinania lasu – naciskamy klawisz C
            if (e.Key == Key.C)
            {
                if (mapa[pozycjaGraczaY, pozycjaGraczaX] == LAS)//jeśli gracz stoi na polu lasu
                {
                    mapa[pozycjaGraczaY, pozycjaGraczaX] = LAKA;
                    tablicaTerenu[pozycjaGraczaY, pozycjaGraczaX].Source = obrazyTerenu[LAKA];
                    iloscDrewna++;
                    EtykietaDrewna.Content = "Drewno: " + iloscDrewna;
                }
            }
            if (iloscDrewna > 4) 
                
            {
                wyswietl.Content = "Odblokowano Kilof!!!";
                if (e.Key == Key.R) 
                {
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
                GenerujMape(wysokoscMapy, szerokoscMapy);
                WczytajMape("mapa.txt");
            }

            if (e.Key == Key.D) 
            {
                GenerujMape(wysokoscMapy, szerokoscMapy);
                WczytajMape("mapa.txt");
            } 
        }

     

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem KLIK = sender as MenuItem;

            if(KLIK != null)
            {
                string naglowek = KLIK.Header.ToString();

                switch(naglowek) 
                {
                    case "Sterowanie":
                        wyswietl.Content = "Poruszaj Się Strzałkami";
                        break;
                    case "Akcja":
                        wyswietl.Content = "Zbieranie Drewna: C, Reset: D";
                        break;
                         
                }
            }
        }



        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            
            GenerujMape(5, 5);
            WczytajMape("mapa.txt");
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            GenerujMape(6, 6);
            WczytajMape("mapa.txt");
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            GenerujMape(7, 7);
            WczytajMape("mapa.txt");
        }
        
    }
    
    
   
}


