using SQLite;
using System.ComponentModel;

namespace Ahorcado_Estados.Negocios
{
    [Table("Estados")]
    public class Estados : INotifyPropertyChanged
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

        private string codigo;

        public string Codigo
        {
            get { return codigo; }
            set
            {
                if (codigo != value)
                {
                    codigo = value;
                    NotifyPropertyChanged("Codigo");
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

        private string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set
            {
                if (descripcion != value)
                {
                    descripcion = value;
                    NotifyPropertyChanged("Descripcion");
                }
            }
        }

        private string link;

        public string Link
        {
            get { return link; }
            set
            {
                if (link != value)
                {
                    link = value;
                    NotifyPropertyChanged("Link");
                }
            }
        }

        private string motto;

        public string Motto
        {
            get { return motto; }
            set
            {
                if (motto != value)
                {
                    motto = value;
                    NotifyPropertyChanged("Motto");
                }
            }
        }

        private bool status;

        public bool Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        public string Imagen
        {
            get { return string.Format("ms-appx:///Imagenes/Estados/{0}.png", Codigo); }
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