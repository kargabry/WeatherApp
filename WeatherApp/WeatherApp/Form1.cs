using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using WeatherApp.Properties;

namespace WeatherApp
{
    public partial class Form1 : Form
    {
        const string apiKey = "0d93cc53edc23dfe9c9d8b70c6fab079";
        string cityName = "Warsaw";
        public Form1()
        {
            InitializeComponent();
            GetWeather(cityName, apiKey);
            GetForecast(cityName, apiKey);
        }

        void GetWeather(string city, string apiKey) 
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", city, apiKey);

                var jsonString = web.DownloadString(url);
                var resultJson = JsonConvert.DeserializeObject<WeatherInfo.Root>(jsonString);
                WeatherInfo.Root output = resultJson;

                lbl_cityName1.Text = string.Format("{0}", output.name);
                lbl_country.Text = string.Format("{0}", output.sys.country);
                lbl_temp.Text = string.Format("{0} \u00B0" + "C", Math.Round(Convert.ToInt16(output.main.temp) - 273.15));
                lbl_mainConditions.Text = string.Format("{0}", output.weather[0].main);
                lbl_mainDescription.Text = string.Format("{0}", output.weather[0].description);
                lbl_mainSpeed.Text = string.Format("{0} km/h", output.wind.speed);
                object mainImage = Resources.ResourceManager.GetObject("_"+output.weather[0].icon);
                mainPicture.Image = (Image)mainImage;
            }
        }

        void GetForecast(string city, string apiKey) 
        {
            int cnt = 7;
            string url = string.Format("https://api.openweathermap.org/data/2.5/forecast?q={0}&units=metric&cnt={1}&appid={2}", city, cnt, apiKey);
            string[] lblNamesTab = { "lbl_day", "lbl_condition", "lbl_description", "lbl_windSpeed", "lbl_temp"};
            string[] addToStringTab = { "", "", "", " km/h", " \u00B0 C"};

            using (WebClient web = new WebClient())
            {
                var json = web.DownloadString(url);
                var jsonObject = JsonConvert.DeserializeObject<WeatherForcast>(json);
                WeatherForcast forcast = jsonObject;


                for (int i = 1; i < cnt; i++)
                {
                    string[] weatherInfoTab = { (getDate(forcast.list[i].dt)).ToString(), forcast.list[i].weather[0].main, forcast.list[i].weather[0].description, (forcast.list[i].wind.speed).ToString(), (forcast.list[i].main.temp).ToString() };

                    for (int j = 0; j < lblNamesTab.Length; j++)
                    {
                        findAndChangeTextInLabel(lblNamesTab[j], i - 1, addToStringTab[j], weatherInfoTab[j]);
                    }

                    object image = Resources.ResourceManager.GetObject("_"+ forcast.list[i].weather[0].icon);
                    ((PictureBox)this.Controls["pictureBox" + (i-1)]).Image = (Image)image;
                }
            }
        }

        DateTime getDate(double milliseconds)
        {
            DateTime day = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
            day = day.AddSeconds(milliseconds).ToLocalTime();
            return day;
        }

        void findAndChangeTextInLabel(string labelName, int labelNumber, string additionalText, string LabelText)
        {
            Label lbl_name = this.Controls.Find(labelName + labelNumber, true).FirstOrDefault() as Label;
            lbl_name.Text = string.Format("{0}" + additionalText, LabelText);
        }

        private void btn_check_Click(object sender, EventArgs e)
        {
            if(tbx_cityName.Text != null || tbx_cityName.Text != "")
            {
                GetWeather(tbx_cityName.Text, apiKey);
                GetForecast(tbx_cityName.Text, apiKey);
            }
        }

        private void tbx_cityName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_check_Click(this, new EventArgs());
            }
        }
    }
}
