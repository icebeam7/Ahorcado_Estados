using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Text;
using Windows.UI.Popups;
using Windows.Networking.Connectivity;

namespace Ahorcado_Estados.Negocios
{
    public class Metodos
    {
        private BaseDatos db;

        public Metodos()
        {
            db = new BaseDatos();
        }

        public async Task InsertarEstados()
        {
            XDocument documento = XDocument.Load("Datos/Estados.xml");

            var query = from estado in documento.Descendants("Estado")
                        select new Estados()
                        {
                            Codigo = estado.Descendants("Codigo").First().Value,
                            Nombre = estado.Descendants("Nombre").First().Value,
                            Descripcion = estado.Descendants("Descripcion").First().Value,
                            Link = estado.Descendants("Link").First().Value, 
                            Status = false, 
                            Id = int.Parse(estado.Descendants("Codigo").First().Value),
                            Motto = estado.Descendants("Motto").First().Value, 
                        };

            await db.conn.InsertAllAsync(query.ToList());
        }

        public async Task<List<Estados>> ObtenerEstados()
        {
            return await db.conn.Table<Estados>().OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<List<Estados>> ObtenerEstadosDescubiertos()
        {
            return await db.conn.Table<Estados>().Where(x => x.Status).OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<Estados> ObtenerEstado(int id)
        {
            return await db.conn.Table<Estados>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> ContarEstados()
        {
            return await db.conn.Table<Estados>().CountAsync();
        }

        public async Task<int> ContarEstadosDescubiertos()
        {
            return await db.conn.Table<Estados>().Where(x => x.Status).CountAsync();
        }

        public async Task<Estados> ObtenerEstadoAleatorio()
        {
            Estados estado = null;
            Random r = new Random();

            int firstID = (await db.conn.Table<Estados>().OrderBy(c => c.Id).FirstOrDefaultAsync()).Id;
            int lastID = (await db.conn.Table<Estados>().OrderByDescending(c => c.Id).FirstOrDefaultAsync()).Id + 1;

            //do
            //{
                int id = r.Next(firstID, lastID);
                estado = await ObtenerEstado(id);
            //}while (estado.Status);

            return estado;
        }

        public async Task<Estados> ObtenerEstado(string nombre)
        {
            return await db.conn.Table<Estados>().Where(x => x.Nombre == nombre).FirstOrDefaultAsync();
        }

        public async Task ActualizarEstado(Estados estado)
        {
            if (!estado.Status)
            {
                estado.Status = true;
                await db.conn.UpdateAsync(estado);
            }
        }

        public async Task<Configuracion> CargarConfiguracion()
        {
            return await db.conn.Table<Configuracion>().FirstOrDefaultAsync();
        }

        public async Task AgregarConfiguracion(string nick, int time)
        {
            if ((await ContarEstados()) == 0)
            {
                await InsertarConfiguracion(nick, time);
                await InsertarEstados();
            }
        }

        public async Task InsertarConfiguracion(string nick, int time)
        {
            Configuracion configuracion = new Configuracion()
            {
                Id = 1,
                Nombre = nick,
                TiempoLimite = time,
                IDUsuario = Guid.NewGuid().ToString(),
                Puntos = 0,
                Derrotas = 0,
                Victorias = 0, 
                MSBand = false
            };

            await db.conn.InsertAsync(configuracion);
        }

        public async Task ActualizarNuevoJuego()
        {
            Configuracion configuracion = await CargarConfiguracion();
            configuracion.Derrotas++;
            await db.conn.UpdateAsync(configuracion);
        }

        public async Task ActualizarConfiguracionUsuarioAnterior(string usernombre, int timeLimit, bool msband)
        {
            Configuracion configuracion = await CargarConfiguracion();
            configuracion.Nombre = usernombre;
            configuracion.TiempoLimite = timeLimit;
            configuracion.MSBand = msband;
            await db.conn.UpdateAsync(configuracion);
        }

        public async Task ActualizarConfiguracion(bool result, int puntos)
        {
            Configuracion configuracion = await CargarConfiguracion();
            configuracion.Puntos += puntos;

            if (result)
            {
                configuracion.Victorias++;
                configuracion.Derrotas--;
            }

            await db.conn.UpdateAsync(configuracion);
        }

        public async Task ReiniciarEstados()
        { 
            List<Estados> lista = await db.conn.Table<Estados>().ToListAsync();

            foreach (var item in lista)
	        {
                item.Status = false;
	        }

            await db.conn.UpdateAllAsync(lista);
        }

        public async Task ReiniciarConfiguracion()
        {
            Configuracion configuracion = await CargarConfiguracion();

            if (configuracion != null)
            {
                configuracion.Puntos = 0;
                configuracion.Derrotas = 0;
                configuracion.Victorias = 0;

                await db.conn.UpdateAsync(configuracion);
            }
        }

        public async Task<bool> CheckIfTableExists()
        {
            return (await db.conn.Table<Estados>().CountAsync() > 0);
        }

        public async Task WaitOneSecond()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(1000);
            });
        }

        public async Task ShowMessage(string message, string title)
        {
            MessageDialog box = new MessageDialog(message, title);
            box.Commands.Add(new UICommand { Label = "Got it", Id = 0 });
            await box.ShowAsync();
        }

        public async Task RestartSettings()
        {
            MessageDialog box = new MessageDialog("Do you really want to restart your score?", "Warning");
            box.Commands.Add(new UICommand { Label = "Yes", Id = 0, Invoked = new UICommandInvokedHandler(Restart) });
            box.Commands.Add(new UICommand { Label = "No", Id = 0 });
            box.DefaultCommandIndex = 0;
            box.CancelCommandIndex = 1;

            await box.ShowAsync();
        }

        private async void Restart(IUICommand command)
        {
            await ReiniciarConfiguracion();
            await ReiniciarEstados();
            await ShowMessage("Your score was restarted", "Success");

        }

        public List<TiempoLimite> ObtenerTiempos()
        {
            return new List<TiempoLimite>() 
            { 
                new TiempoLimite() { Indice = 0, Minutos = "1 minutes", Tiempo = 60 },
                new TiempoLimite() { Indice = 1, Minutos = "2 minutes", Tiempo = 120 },
                new TiempoLimite() { Indice = 2, Minutos = "3 minutes", Tiempo = 180 }
            };
        }

        public async Task GuardarConfiguracion(string user, int tiempo, bool msband)
        {
            if (await db.conn.Table<Configuracion>().CountAsync() == 1)
                await ActualizarConfiguracionUsuarioAnterior(user, tiempo, msband);
            else
                await AgregarConfiguracion(user, tiempo);
        }
    }
}