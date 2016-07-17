using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace E_Mig
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MainViewModel wm;

        public MainPage()
        {
            this.InitializeComponent();
            this.InitializeLayout();
            this.InitializeEvents();
        }
        void InitializeLayout()
        {
            map.ZoomLevel = 8;
            map.Center = new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition() { Latitude = 47.157466, Longitude = 19.292040 });
        }
        void InitializeEvents()
        {
            this.Loaded += MainPage_Loaded;
        }

        async Task VonatBetoltes()
        {
            wm = new MainViewModel();
            this.DataContext = wm;
        }
        async Task VonatFrissites()
        {
            wm = new MainViewModel();
            this.DataContext = wm;
        }
        void VonatDialogShow(Vonat v)
        {
            ContentDialog dlg = new ContentDialog();

            dlg.Title = v.Palyaszam;
            dlg.Content = "\n" + String.Format("UIC: \t{0} \n Vonatszám: \t{1}", new object[] { v.UIC, v.Palyaszam });
        }

        #region Event Handlers
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await VonatBetoltes();
        }
        private void btnMenuToggle_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnMenuToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mainPage_SplitView.IsPaneOpen = !mainPage_SplitView.IsPaneOpen;
        }
        private void btnMapZoomIn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            map.ZoomLevel++;
        }
        private void btnMapZoomOut_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnMapZoomOut_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (map.ZoomLevel > 1)
            {
                map.ZoomLevel--;
            }
        }

       

        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            

            Image img = (Image)sender;
            int index = Convert.ToInt32(img.Tag);
            ContentDialog dlg = new ContentDialog();
            var det = await DataConnection.getDetails(wm.Vonatok.Result[index].UIC, wm.Vonatok.Result[index].Vonatszam);

            dlg.Title = wm.Vonatok.Result[index].Palyaszam;
            dlg.Content = String.Format("\nVonatszám:\t {1}\nInduló állomás:\t {2}\nÉrkező állomás:\t {3}\nSebesség:\t {4}\nUIC:\t\t {0}", new object[] { wm.Vonatok.Result[index].UIC.ToString(), wm.Vonatok.Result[index].Vonatszam.ToString(), wm.Vonatok.Result[index].KiinduloAllomas.ToString(), wm.Vonatok.Result[index].Celallomas.ToString(), det.Sebesseg });
            dlg.IsPrimaryButtonEnabled = true;
            dlg.PrimaryButtonText = "OK";
            dlg.PrimaryButtonClick += Dlg_PrimaryButtonClick;
            await dlg.ShowAsync();
        }
        private void Dlg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            sender.Hide();
        }
        private void menu_map_Click(object sender, RoutedEventArgs e)
        {

        }
        private void menu_station_Click(object sender, RoutedEventArgs e)
        {

        }
        private void command_settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }
        private void menu_settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.wm = null;
            base.OnNavigatedFrom(e);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        #endregion

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchedViewModel sm;
            switch (((ComboBoxItem)cbQueryType.SelectedValue).Content.ToString())
            {
                case "UIC":
                    sm = new SearchedViewModel(QueryType.UIC, textBox.Text);
                    this.DataContext = sm;
                    break;
                case "Pályaszám":
                    sm = new SearchedViewModel(QueryType.Loc_No, textBox.Text);
                    this.DataContext = sm;
                    break;
                case "Vonatszám":
                    sm = new SearchedViewModel(QueryType.Train_ID, textBox.Text);
                    this.DataContext = sm;
                    break;
                case "Hely":
                    sm = new SearchedViewModel(QueryType.Location, textBox.Text);
                    this.DataContext = sm;
                    break;
            }
        }

        private void btnRefresh_Tapped(object sender, TappedRoutedEventArgs e)
        {
            wm = new MainViewModel();
            this.DataContext = wm;
        }
    }

    public class MainViewModel
    {
        public MainViewModel()
        {
            Vonatok = new TaskCompleteNotification<List<Vonat>>(DataConnection.Vonatok());
        }

        public TaskCompleteNotification<List<Vonat>> Vonatok
        {
            get;
            private set;
        }
    }
    public class SearchedViewModel
    {
        public SearchedViewModel(QueryType type, string queryStr)
        {
            switch(type)
            {
                case QueryType.Location:
                    Vonatok = new TaskCompleteNotification<List<Vonat>>(VonatQuery.queryByLocations());
                    break;
                case QueryType.Loc_No:
                    Vonatok = new TaskCompleteNotification<List<Vonat>>(VonatQuery.queryByPSz(queryStr));
                    break;
                case QueryType.Train_ID:
                    Vonatok = new TaskCompleteNotification<List<Vonat>>(VonatQuery.queryByTrainId(queryStr));
                    break;
                case QueryType.UIC:
                    Vonatok = new TaskCompleteNotification<List<Vonat>>(VonatQuery.queryByUIC(queryStr));
                    break;
            }
        }

        public TaskCompleteNotification<List<Vonat>> Vonatok
        {
            get;
            private set;
        }
    }

    public sealed class TaskCompleteNotification<TResult> : INotifyPropertyChanged
    {
        public TaskCompleteNotification(Task<TResult> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }
        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {

            }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this,
                  new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this,
                  new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
        }
        public Task<TResult> Task { get; private set; }
        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult);
            }
        }
        public TaskStatus Status { get { return Task.Status; } }
        public bool IsCompleted { get { return Task.IsCompleted; } }
        public bool IsNotCompleted { get { return !Task.IsCompleted; } }
        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status == TaskStatus.RanToCompletion;
            }
        }
        public bool IsCanceled { get { return Task.IsCanceled; } }
        public bool IsFaulted { get { return Task.IsFaulted; } }
        public AggregateException Exception { get { return Task.Exception; } }
        public Exception InnerException
        {
            get
            {
                return (Exception == null) ? null : Exception.InnerException;
            }
        }
        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ? null : InnerException.Message;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
