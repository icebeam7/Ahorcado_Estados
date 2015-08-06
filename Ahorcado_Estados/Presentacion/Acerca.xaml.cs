using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using Ahorcado_Estados.Negocios;

namespace Ahorcado_Estados.Presentacion
{
    public sealed partial class Acerca : Page
    {
        Share share;

        public Acerca()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            share = new Share(AppInfo.Name, AppInfo.Motto, AppInfo.Link);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            share.Kill();
        }

        private void txtEmail_Tapped(object sender, TappedRoutedEventArgs e)
        {
            share.SendEmail();
        }

        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            share.Show();
        }

        private void btnAllApps_Click(object sender, RoutedEventArgs e)
        {
            share.SeeAllApps();
        }
    }
}
