using SQLite;
using System;
using System.ComponentModel;

namespace Ahorcado_Estados.Negocios
{
    [Table("Configuracion")]
    public class Configuracion : INotifyPropertyChanged
    {
        private int id;

        [PrimaryKey]
        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (nombre != value)
                {
                    nombre = value;
                    NotifyPropertyChanged("Nombre");
                }
            }
        }

        private int tiempoLimite;

        public int TiempoLimite
        {
            get { return tiempoLimite; }
            set
            {
                if (tiempoLimite != value)
                {
                    tiempoLimite = value;
                    NotifyPropertyChanged("TiempoLimite");
                }
            }
        }

        private int puntos;

        public int Puntos
        {
            get { return puntos; }
            set
            {
                if (puntos != value)
                {
                    puntos = value;
                    NotifyPropertyChanged("Puntos");
                }
            }
        }

        private int victorias;

        public int Victorias
        {
            get { return victorias; }
            set
            {
                if (victorias != value)
                {
                    victorias = value;
                    NotifyPropertyChanged("Victorias");
                }
            }
        }

        private int derrotas;

        public int Derrotas
        {
            get { return derrotas; }
            set
            {
                if (derrotas != value)
                {
                    derrotas = value;
                    NotifyPropertyChanged("Derrotas");
                }
            }
        }

        private string idUsuario;

        public string IDUsuario
        {
            get { return idUsuario; }
            set
            {
                if (idUsuario != value)
                {
                    idUsuario = value;
                    NotifyPropertyChanged("IDUsuario");
                }
            }
        }

        private bool msBand;

        public bool MSBand
        {
            get { return msBand; }
            set
            {
                if (msBand != value)
                {
                    msBand = value;
                    NotifyPropertyChanged("MSBand");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
