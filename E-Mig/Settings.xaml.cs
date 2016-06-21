using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace E_Mig
{
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        void InitializeEvents()
        {
            //SystemNavigationManager.GetForCurrentView().BackRequested += Settings_BackRequested;
        }

        

        #region Event Handlers
        private void btnMenuToggle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            settings_SplitView.IsPaneOpen = !settings_SplitView.IsPaneOpen;
        }
        private void menu_train_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        private void menu_station_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        private void menu_map_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
        #endregion
    }
}
