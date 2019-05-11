using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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

            getWeather(city, country);
        }

        private async void getWeather(string city, string country)
        {
            var client = new HttpClient();

            var response = await client.GetAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?q=" + city + "," + country + "&appid=" + key + "&units=metric"));

            var result = await response.Content.ReadAsStringAsync();

            JObject weather = JObject.Parse(result);

            var temperature = (double)weather["main"]["temp"];

            string plas = null;

            foreach(char item in temperature.ToString())
            {
                if (item != '-')
                {
                    plas = "+";
                }
            }

            tBResult.Text = $"Текущая температура  {plas}{temperature} °С ";
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
