﻿using YetAnotherWeatherApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using YetAnotherWeatherApp.Models;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.CommunityToolkit.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using static YetAnotherWeatherApp.Services.DeviceOrientationHandler;

namespace YetAnotherWeatherApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public Orientation OrientationMode { get { return this.orientation; } }
        private WeatherDataAccess.Model.InstantDetails currentWeatherDetails;
        public WeatherDataAccess.Model.InstantDetails CurrentWeatherDetails
        {
            get => currentWeatherDetails;
            set
            {
                currentWeatherDetails = value;
                OnPropertyChanged();
            }
        }

       
        private bool dayPickerIsVisible = false;
        public bool DayPickerIsVisible
        {
            get => dayPickerIsVisible;
            set
            {
                dayPickerIsVisible = value;
                OnPropertyChanged();
            }
        }
        public ICommand OpenWebCommand { get; set; }
        public ICommand ShowDayPicker { get; set; }
        public ICommand HideDayPicker { get; set; }
        public AsyncCommand RefreshWeatherData { get; set; }

        public TimeModel TimeModel { get; set; }
        public ObservableCollection<TimeModel> Dates { get; } = new ObservableCollection<TimeModel>();

        
        public HomeViewModel()
        {
            Orientation orientation = OrientationMode;
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.yr.no/en"));
            ShowDayPicker = new Command(OnShowDayPicker);
            HideDayPicker = new Command(OnHideDayPicker);

            RefreshWeatherData = new AsyncCommand(OnRefreshWeatherData);
            TimeModel = new TimeModel()
            {
                Date = DateTime.Now,
            };

            for (int i = 0; i < 6; i++)
            {
                Dates.Add(new TimeModel()
                {
                    Date = DateTime.Now.AddDays(i),
                });
            }
        }

        async Task OnRefreshWeatherData()
        {
            var location = await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();

            var weatherData = await new WeatherDataAccess.Data.HTTPACCESS().GetWeatherFromLocationAsync(location.Latitude, location.Longitude);

            if (weatherData != null)
            {
                CurrentWeatherDetails = weatherData.Properties?.Timeseries.FirstOrDefault()?.Data?.Instant?.Details;
            }
        }

        
        void OnShowDayPicker()
        {
            if (DayPickerIsVisible is false)
            {
                DayPickerIsVisible = true;
            }
        }

        void OnHideDayPicker()
        {
            if (DayPickerIsVisible is true)
            {
                DayPickerIsVisible = false;
            }
        }
    }
}
