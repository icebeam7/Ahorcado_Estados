using SQLite;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Ahorcado_Estados.Negocios
{
    public class BaseDatos
    {
        private const string DBNAME = "Estados.db";

        public SQLiteAsyncConnection conn { get; set; }

        public BaseDatos()
        {
            conn = new SQLiteAsyncConnection(DBNAME);
            this.InitDb();
        }

        public async void InitDb()
        {
            bool dbExist = await CheckDbAsync();
            if (!dbExist)
            {
                await CreateDatabaseAsync();
            }
        }

        public async Task<bool> CheckDbAsync()
        {
            bool dbExist = true;

            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(DBNAME);
            }
            catch (Exception)
            {
                dbExist = false;
            }

            return dbExist;
        }

        private async Task CreateDatabaseAsync()
        {
            try
            {
                conn.CreateTableAsync<Estados>().Wait();
                conn.CreateTableAsync<Configuracion>().Wait();
            }
            catch(Exception) { }

        }
    }
}
