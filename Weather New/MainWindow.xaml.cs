using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Weather_New
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const string key = "a2646272358964f1ef9e45a76579ddda";

        List<Location> location = JsonConvert.DeserializeObject<List<Location>>(File.ReadAllText(@"..\..\json\city.list.json"));

        public MainWindow()
        {
            InitializeComponent();

            getLocation();
        }

        private void BtnGetWeather_Click(object sender, RoutedEventArgs e)
        {
            string city = cBCity.Text;

            string country = cBCountry.Text;

            getWeatherNow(city, country);

            getWeather4Days(city, country);
        }

        private async void getWeatherNow(string city, string country)
        {
            var client = new HttpClient();

            var response = await client.GetAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?q=" + city + "," + country + "&appid=" + key + "&units=metric"));

            var result = await response.Content.ReadAsStringAsync();

            JObject weather = JObject.Parse(result);

            JToken token = weather;

            Now.RootObject weatherNow = new Now.RootObject();

            weatherNow = token.ToObject<Now.RootObject>();

            string plas = null;

            foreach (char item in weatherNow.main.temp.ToString())
            {
                if (item != '-')
                {
                    plas = "+";
                }
            }

            tBResult1.Text = $"Текущая температура  {plas}{weatherNow.main.temp} °С \n\rАтмосферное давление {weatherNow.main.pressure} мм.рт.ст. \n\rСкорость ветра {weatherNow.wind.speed} м/с";

            string icon = null;

            foreach (Now.Weather item in weatherNow.weather)
            {
                icon = item.icon;
            }

            imgWeather.Source = new BitmapImage(new Uri("http://openweathermap.org/img/w/" + icon + ".png"));
        }

        private async void getWeather4Days(string city, string country)
        {
            var client = new HttpClient();

            var response = await client.GetAsync(new Uri("http://api.openweathermap.org/data/2.5/forecast?q=" + city + "," + country + "&appid=" + key + "&units=metric"));

            var result = await response.Content.ReadAsStringAsync();

            JObject weather = JObject.Parse(result);

            IList<JToken> tokens1 = weather["list"].Children().ToList();

            IList<_4days.List> lists = new List<_4days.List>();

            foreach (JToken item in tokens1)
            {
                _4days.List list = item.ToObject<_4days.List>();

                lists.Add(list);
            }


            string str = string.Empty;

            int count = 0;

            foreach (_4days.List item in lists)
            {
                count++;

                Regex regex = new Regex(@"00:00:00(\w*)");

                MatchCollection match = regex.Matches(item.dt_txt);

                if (match.Count > 0)
                {
                    str += "\n\r";
                }

                str += $"Дата, время {item.dt_txt};\nТемпература воздуха - {item.main.temp} °С\nСкорость ветра {item.wind.speed} м/с\n\n";
            }

            tBResult2.Text = str;
        }

        private void getLocation()
        {
            var country = from item in location
                          select item.Country;

            var countryNew = country.Distinct();

            cBCountry.ItemsSource = countryNew.ToList();
        }

        private void getCities(string selectCountry)
        {
            var city = from item in location
                       where item.Country == selectCountry
                       orderby item.City
                       select item.City;

            cBCity.ItemsSource = city.ToList();
        }

        private void CBCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectCountry = cBCountry.SelectedItem.ToString();

            getCities(selectCountry);
        }
    }

}
