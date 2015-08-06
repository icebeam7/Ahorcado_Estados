using Ahorcado_Estados.Negocios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Ahorcado_Estados.Presentacion
{
    public sealed partial class VerEstados : Page
    {
        List<Estados> listaEstados;

        public VerEstados()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            listaEstados = await App.metodos.ObtenerEstadosDescubiertos();
            lstDiscoveredStates.ItemsSource = listaEstados;
        }

        private void lstDiscoveredStates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Frame.Navigate(typeof(DetalleEstado), (e.AddedItems[0] as Estados).Id);
            }
            catch { }
        }
    }
}
