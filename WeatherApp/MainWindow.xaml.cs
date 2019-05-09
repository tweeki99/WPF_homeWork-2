using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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

namespace WeatherApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            LoadWether();
        }

        public City LoadWether()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string json = client.DownloadString("http://api.apixu.com/v1/forecast.json?key=9f444d313d7546a184451817193004&q=" + NameTextBox.Text);
                    City city = JsonConvert.DeserializeObject<City>(json);
                    
                    StringBuilder request = new StringBuilder("https://api.darksky.net/forecast/f1e8310d8c32d31f537a8fb8596c270b/");
                    request.Append(city.Location.Lat);
                    request.Append(",");
                    request.Append(city.Location.Lon);

                    json = client.DownloadString(request.ToString());
                    Weather weather = JsonConvert.DeserializeObject<Weather>(json);

                    (stackPanel.Children[2] as Label).Content = city.Location.Country + " " + city.Location.Region + " " + city.Location.Name;

                    for (int i = 0; i < 8; i++)
                    {
                        weather.Daily.Data[i].TemperatureMin = (weather.Daily.Data[i].TemperatureMin - 32) * 5 / 9;
                        weather.Daily.Data[i].TemperatureMax = (weather.Daily.Data[i].TemperatureMax - 32) * 5 / 9;
                        weather.Daily.Data[i].WindSpeed = Math.Round(weather.Daily.Data[i].WindSpeed / 2.237, 2) ;
                        weather.Daily.Data[i].Humidity = weather.Daily.Data[i].Humidity * 100;

                        DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        date = date.AddSeconds(weather.Daily.Data[i].Time).ToLocalTime();

                        (wrapPanel.Children[i] as Card).Visibility = Visibility.Visible;
                        (((wrapPanel.Children[i] as Card).Content as Grid).Children[0] as Label).Content = Convert.ToInt32(weather.Daily.Data[i].TemperatureMin).ToString() + " - " + Convert.ToInt32(weather.Daily.Data[i].TemperatureMax).ToString() + " ℃";
                        ((((wrapPanel.Children[i] as Card).Content as Grid).Children[1] as StackPanel).Children[0] as Label).Content = date.ToLongDateString();
                        ((((wrapPanel.Children[i] as Card).Content as Grid).Children[1] as StackPanel).Children[1] as Label).Content = "Humidity: " + weather.Daily.Data[i].Humidity.ToString() +" %";
                        ((((wrapPanel.Children[i] as Card).Content as Grid).Children[1] as StackPanel).Children[2] as Label).Content = "Wind: " + weather.Daily.Data[i].WindSpeed.ToString() + " m/s";
                        (((wrapPanel.Children[i] as Card).Content as Grid).Children[2] as Image).Source = new BitmapImage(new Uri(weather.Daily.Data[i].Icon+".png", UriKind.RelativeOrAbsolute));
                    }
                    return JsonConvert.DeserializeObject<City>(json);
                }
                catch (WebException)
                {
                    MessageBox.Show("Введите корректное название города");
                    return null;
                }
            }
        }

        private void NameTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            string key = e.Key.ToString();

            if (key == "Return")
            {
                LoadWether();
            }
        }
    }
}
