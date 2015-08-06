using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Ahorcado_Estados.Negocios;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;

namespace Ahorcado_Estados.Presentacion
{
    public sealed partial class Juego : Page
    {
        Configuracion configuracion;
        int totalStates, TiempoLimite;
        int tiempo, mistakes, puntos, factor;
        int segundos, minutos, horas;
        string answer;
        int chars, spaces;
        bool victory;

        Share share;
        Estados estado;
        DispatcherTimer dispatcherTimer;

        public Juego()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            totalStates = await App.metodos.ContarEstados();

            await Cargar();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (share != null)
                share.Kill();
        }

        async Task Cargar()
        {
            await App.metodos.ActualizarNuevoJuego();
            configuracion = await App.metodos.CargarConfiguracion();
            TiempoLimite = configuracion.TiempoLimite;
            factor = configuracion.TiempoLimite / 60;
            puntos = tiempo = mistakes = 0;
            segundos = minutos = horas = 0;

            int discoveredStates = await App.metodos.ContarEstadosDescubiertos();

            txbUserName.Text = configuracion.Nombre;
            txtResults.Text = String.Format(@"{0} / {1}", discoveredStates, totalStates);
            txbTimer.Text = ConvertToTime(tiempo);

            share = new Share(AppInfo.Name,
                String.Format("My score in #AhorcadoDeEstados for #WindowsPhone is {0} pts and {1} victories. Can you beat me? Download it now!", configuracion.Puntos, configuracion.Victorias),
                AppInfo.Link);

            string url = "ms-appx:///Imagenes/ahorcado0.png";
            BitmapImage img = new BitmapImage(new Uri(url));
            imgAhorcado.Source = img;

            ActivarDetalle(false);

            txbResult.Text = "";

            if (discoveredStates < totalStates)
            {
                estado = await App.metodos.ObtenerEstadoAleatorio();

                if (!estado.Status)
                {
                    txbResult.Text = "** NEW **";
                    txbResult.Foreground = new SolidColorBrush(Colors.Yellow);
                }

                txbEstado.Text = ConvertTo_(estado.Nombre);
                answer = estado.Nombre.ToUpper();

                chars = 0;
                spaces = CountSpaceChars(answer);

                DibujarBotones();
                MostrarControles(true);
                MostrarTaps(true);
                ControlarTiempo();
            }
            else
            {
                txbResult.Text = "VICTORY!! :)";
                txbEstado.FontSize = 19;
                txbEstado.Text = "Congratulations! You completed the game! Thanks for playing! Now you know the 32 Mexican federal entities. Share your progress with your friends.";
                MostrarTaps(false);
                MostrarControles(false);
                MostrarMapa();
            }
        }

        private void DibujarBotones()
        {
            if (grdBotones.Children.Count == 1)
            {
                string alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                int i = -1;

                foreach (char letra in alfabeto)
                {
                    i++;
                    Button btn = new Button();
                    btn.Name = String.Format("btn{0}", letra);
                    btn.Tag = letra;
                    btn.Content = letra;
                    Grid.SetRow(btn, i / 4);
                    Grid.SetColumn(btn, i % 4);
                    grdBotones.Children.Add(btn);
                }
            }
        }

        private string ConvertToTime(int time)
        {
            int hours = time / 3600;
            string shours = hours.ToString();
            if (hours < 10)
                shours = "0" + shours;

            int minutes = time / 60;
            string sminutes = minutes.ToString();
            if (minutes < 10)
                sminutes = "0" + sminutes;

            int seconds = time % 60;
            string sseconds = seconds.ToString();
            if (seconds < 10)
                sseconds = "0" + sseconds;

            segundos = seconds;
            minutos = minutes;
            horas = hours;

            return String.Format(@"{0}:{1}:{2}", shours, sminutes, sseconds);
        }

        private string ConvertTo_(string name)
        {
            string converted = "";

            foreach (char c in name)
            {
                converted += (c != ' ') ? "_" : " ";
                converted += " ";
            }

            return converted;
        }

        private int CountSpaceChars(string answer)
        {
            int count = 0;

            foreach (char c in answer)
            {
                if (c == ' ')
                    count++;
            }

            return count;
        }

        private void MostrarControles(bool mostrar)
        {
            foreach (var item in grdBotones.Children)
            {
                if (item is Button)
                    item.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;
            }

            txbTimer.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MostrarTaps(bool mostrar)
        {
            foreach (var item in grdBotones.Children)
            {
                if (item is Button)
                {
                    if (mostrar)
                        item.Tapped += btnLetter_Tap;
                    else
                        item.Tapped -= btnLetter_Tap;
                }
            }

            //txbTimer.Visibility = Visibility.Collapsed;
        }

        private void ControlarTiempo()
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Tick -= dispatcherTimer_Tick;
            }

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            tiempo++;
            segundos++;
            string segs = "", mins = "", hrs = "";

            if (segundos >= 60)
            {
                segundos -= 60;
                minutos++;
            }

            segs = segundos.ToString();
            if (segundos < 10)
                segs = "0" + segs;

            if (minutos >= 60)
            {
                minutos -= 60;
                horas++;
                mins = "00";
            }

            mins = minutos.ToString();
            if (minutos < 10)
                mins = "0" + mins;

            hrs = horas.ToString();
            if (horas < 10)
                hrs = "0" + hrs;

            string str = String.Format("{0}:{1}:{2}", hrs, mins, segs);
            txbTimer.Text = str;

            if (tiempo >= TiempoLimite)
            {
                Detener(false);
            }
        }

        private async void btnLetter_Tap(object sender, TappedRoutedEventArgs e)
        {
            Button btn = (Button)sender;
            char letra = btn.Tag.ToString()[0];
             
            if (answer.Contains(letra))
            {
                int j = 0;
                string word = txbEstado.Text;

                char[] buffer = new char[word.Length];

                foreach (char c in answer)
                {
                    if (c == letra)
                    {
                        chars++;
                        buffer[j * 2] = letra;
                    }
                    else
                    {
                        buffer[j * 2] = word[j * 2];
                    }

                    buffer[j * 2 + 1] = ' ';
                    j++;
                }

                string result = new string(buffer);
                txbEstado.Text = result;

                if (chars + spaces >= answer.Length)
                {
                    puntos = (configuracion.TiempoLimite - tiempo * factor) + (7 - mistakes) * (60 / factor);
                    await App.metodos.ActualizarEstado(estado);
                    await App.metodos.ActualizarConfiguracion(true, puntos);

                    // Great!
                    Detener(true);
                }
            }
            else
            {
                mistakes++;

                string url = String.Format("ms-appx:///Imagenes/ahorcado{0}.png", mistakes);
                BitmapImage img = new BitmapImage(new Uri(url));
                imgAhorcado.Source = img;

                if (mistakes == 7)
                {
                    Detener(false);
                    txbEstado.Text = ShowAnswer(answer);
                }
            }

            btn.Visibility = Visibility.Collapsed;
        }

        private async void Detener(bool win)
        {
            dispatcherTimer.Stop();

            SolidColorBrush brocha;
            string mensaje;
            victory = win;

            if (win)
            {
                mensaje = "CORRECT!! :) " + puntos + " pts!"; ;
                brocha = new SolidColorBrush(Colors.Green);
                ActivarDetalle(true);

                if (configuracion.MSBand)
                {
                    EstadosMSBand band = new EstadosMSBand();
                    await band.Publish(estado);
                }
            }
            else
            {
                mensaje = "INCORRECT!! :( 0 pts";
                brocha = new SolidColorBrush(Colors.Red);
            }

            txbResult.Text = mensaje;
            txbResult.Foreground = brocha;
            btnNext.IsEnabled = true;
            MostrarTaps(false);
        }

        private string ShowAnswer(string name)
        {
            string converted = "";

            foreach (char c in name)
            {
                converted += c.ToString();
                converted += " ";
            }

            return converted;
        }

        void ActivarDetalle(bool activar)
        {
            btnDetail.IsEnabled = activar;
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DetalleEstado), estado.Id);
        }

        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            if (share != null)
            {
                if (victory)
                    share.description = String.Format("By playing #AhorcadoDeEstados for #WindowsPhone I learned that {0} is a Mexican federal entity. Download the app now!", estado.Nombre);

                share.Show();
            }
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            await Cargar();
        }

        private void MostrarMapa()
        {
            string url = "ms-appx:///Imagenes/Estados/mapa.png";
            BitmapImage img = new BitmapImage(new Uri(url));
            Image imagen = new Image();
            
            imagen.Source = img;
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(imagen);
        }
    }
}
