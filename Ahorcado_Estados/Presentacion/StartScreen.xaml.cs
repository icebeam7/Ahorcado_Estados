using Ahorcado_Estados.Negocios;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Ahorcado_Estados.Presentacion
{
    public sealed partial class StartScreen : Page
    {
        public StartScreen()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await LoadData();
        }

        public async Task LoadData()
        {
            if (App.firstTime)
            {
                RatePopup.CheckRateReminder();
                App.firstTime = false;
            }

            if (App.metodos == null)
            {
                mainContainer.Visibility = Visibility.Collapsed;
                bsyMessage.IsBusy = true;
                HabilitarBotones(false);

                App.metodos = new Metodos();
                await App.metodos.WaitOneSecond();
                await App.metodos.AgregarConfiguracion("Player 1", 120);
                await App.metodos.WaitOneSecond();
                HabilitarBotones(true);
                bsyMessage.IsBusy = false;
                mainContainer.Visibility = Visibility.Visible;
            }
        }

        private void HabilitarBotones(bool habilitar)
        {
            bar.IsEnabled = habilitar;
        }

        private void btnAcerca_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Acerca));
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Configurar));
        }

        private void btnCalificar_Click(object sender, RoutedEventArgs e)
        {
            RatePopup.ShowRate();
        }

        private void btnLeer_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Instrucciones));
        }

        private void btnScore_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Score));
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Juego));
        }
    }
}
