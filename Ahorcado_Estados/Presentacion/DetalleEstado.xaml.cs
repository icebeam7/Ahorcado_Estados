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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Ahorcado_Estados.Presentacion
{
    public sealed partial class DetalleEstado : Page
    {
        int id;

        public DetalleEstado()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            id = (e.Parameter != null) ? int.Parse(e.Parameter.ToString()) : -1;

            if (id > 0)
            {
                Estados estado = await App.metodos.ObtenerEstado(id);

                string url = String.Format("ms-appx:///Imagenes/Estados/{0}.png", estado.Codigo);
                string nom = estado.Nombre;
                string desc = estado.Descripcion;

                Title.Text = nom;
                imgEscudo.Source = SetImage(url);
                EscribirEnRichTextBox(desc, rtbDescripcion);
                lnkWiki.NavigateUri = new Uri(estado.Link);
            }
        }

        BitmapImage SetImage(string url)
        {
            BitmapImage img = new BitmapImage(new Uri(url));
            return img;
        }

        void EscribirEnRichTextBox(string texto, RichTextBlock rtb)
        {
            rtb.Blocks.Clear();

            Run run = new Run();
            run.Text = texto;

            Paragraph parrafo = new Paragraph();
            parrafo.Inlines.Add(run);

            rtb.Blocks.Add(parrafo);
        }
    }
}
