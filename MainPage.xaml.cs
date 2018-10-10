using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Microcharts;
using SkiaSharp;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace App1
{
    public partial class MainPage : ContentPage
    {
        // Request send interval, it equals one second.
        private const int interval = 1000;
        private List<DeviceInfo> infos = new List<DeviceInfo>();
        private BarChart barChart;
        private Grid grid;
        private Label label;
        
        public MainPage()
        {
            InitializeComponent();
            grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(50, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(125, GridUnitType.Star)});
            grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(65, GridUnitType.Star)});
            grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star) });

            var scrolView = ScrolView;
            label = new Label { Text = "Sequence", Margin = new Thickness(20), WidthRequest = 300 };
            label.HorizontalTextAlignment = TextAlignment.Center;
            
            BarChart barChart = Parse(GetValues());
            MyBarChart.Chart = barChart;

            Button button = new Button
            {
                WidthRequest = 80,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 35,
                FontSize = 10,
                Text = "update",
                VerticalOptions = LayoutOptions.Center
            };
            button.Clicked += Button_Clicked;
            grid.Children.Add(label, 0, 0);
            grid.Padding = 10;
            grid.RowSpacing = 20;
            grid.Children.Add(MyBarChart, 0, 1);
            /**
             * The line below was commented out because of my task conditions.
             * grid.Children.Add(button, 0, 2);
             **/
            scrolView.Content = grid;
            scrolView.ScrollToAsync(label, ScrollToPosition.Start, true);
            UpdateBarChart();
            /**
             * This is a block of code which sends request to web api service every time unit.
             * In my case it sends GET request every second. Also it remove old bar chart and 
             * initiate new one and then add it to existing grid.
             **/
            Device.StartTimer(new TimeSpan(interval), () =>
            {
                UpdateBarChart();
                return true;
            });
        }

        /**
         * In case of using button for updating your bar chart just comment out
         * Device.StartTimer() block of code and uncomment grid.Children.Add(button, 0, 2) 
         **/
        private void Button_Clicked(object sender, EventArgs e)
        {
            UpdateBarChart();
        }

        private void UpdateBarChart()
        {
            BarChart bc = Parse(GetValues());
            MyBarChart.IsVisible = false;
            grid.Children.Remove(MyBarChart);
            MyBarChart.Chart = bc;
            grid.Children.Add(MyBarChart);
            MyBarChart.IsVisible = true;
            label.Text = "Rssi updated";
        }

        public string GetValues()
        {
            /**
             * IP(10.0.2.2) is a localhost of your local machine where you start your emulator.
             * If you use localhost instead then android app will send request to itself and 
             * you'll get error.
             **/
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://10.0.2.2/WebApp/api/values");
            request.ContentType = "application/json";
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();
            string result;
            using (var strRdr = new StreamReader(response.GetResponseStream()))
            {
                result = strRdr.ReadToEnd();
            }
            return result;
        }

        public BarChart Parse(string str)
        {
            var fromJson = JsonConvert.DeserializeObject<List<DeviceInfo>>(str);
            return barChart = new BarChart()
            {
                Entries = fromJson.Select(v => new Microcharts.Entry(v.Rssi * -1)
                {
                    Label = v.DeviceId.ToString(),
                    ValueLabel = v.Rssi.ToString(),
                    Color = SKColor.Parse("#80A1C1")
                })
            }; 
        }
    }
}
