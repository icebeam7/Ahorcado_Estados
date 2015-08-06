using Ahorcado_Estados.Negocios;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Ahorcado_Estados.Presentacion
{
    public sealed partial class Configurar : Page
    {
        public Configurar()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var tiempos = App.metodos.ObtenerTiempos();
            domTimeLimit.ItemsSource = tiempos;

            Configuracion configuracion = await App.metodos.CargarConfiguracion();

            if (configuracion != null)
            {
                txtName.Text = configuracion.Nombre;
                domTimeLimit.Value = tiempos[configuracion.TiempoLimite / 60 - 1];
                tgsMSBand.IsOn = configuracion.MSBand;
            }
            else
            {
                txtName.Text = "player 1";
                domTimeLimit.Value = tiempos[1];
                tgsMSBand.IsOn = false;
            }
        }

        private async void btnDelete_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await App.metodos.RestartSettings();
            GoBack();
        }

        private async void btnSave_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string nick = txtName.Text;
            int time = (domTimeLimit.Value as TiempoLimite).Tiempo;
            bool msband = tgsMSBand.IsOn;

            await App.metodos.GuardarConfiguracion(nick, time, msband);
            GoBack();
        }

        private void GoBack()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
