using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Ahorcado_Estados.Negocios;

namespace Ahorcado_Estados.Presentacion
{
    public sealed partial class Score : Page
    {
        Share share;

        public Score()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Configuracion configuracion = await App.metodos.CargarConfiguracion();

            share = new Share(AppInfo.Name, 
                              String.Format("My score in #AhorcadoDeEstados for #WindowsPhone is {0} points and {1} victories. Can you beat me? Download it now!", configuracion.Puntos, configuracion.Victorias), 
                              AppInfo.Link);

            txtCorrect.Text = configuracion.Victorias.ToString();
            txtGames.Text = (configuracion.Victorias + configuracion.Derrotas).ToString();
            txtPlayer.Text = configuracion.Nombre;
            txtPoints.Text = configuracion.Puntos.ToString();
            txtWrong.Text = configuracion.Derrotas.ToString();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (share != null)
                share.Kill();
        }

        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            if (share != null)
                share.Show();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VerEstados));
        }
    }
}
