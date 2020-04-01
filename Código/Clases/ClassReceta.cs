using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace avanXpert_Info.Código.Clases
{
    public class ClassReceta : INotifyPropertyChanged
    {
        int priv_id;
        string priv_name;
        int priv_id_molino;
        int priv_status;
        int priv_activa;
        int priv_Indice_Receta;
        public event PropertyChangedEventHandler PropertyChanged;
        public int id
        {
            get { return priv_id; }
            set { priv_id = value; }
        }
        public string name
        {
            get { return priv_name; }
            set { priv_name = value; }
        }
        public int id_molino
        {
            get { return priv_id_molino; }
            set { priv_id_molino = value; }
        }
        public int status
        {
            get { return priv_status; }
            set { priv_status = value; }
        }
        public int activa
        {
            get { return priv_activa; }
            set { priv_activa = value; OnPropertyChanged(); }
        }
        public int Indice_Receta
        {
            get { return priv_Indice_Receta; }
            set { priv_Indice_Receta = value; }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}