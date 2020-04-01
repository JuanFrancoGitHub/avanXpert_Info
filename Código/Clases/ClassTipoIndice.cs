using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using avanXpert_Info.Código;
using avanXpert_Info.Código.Clases;

namespace avanXpert_Info.Código.Clases
{
    class ClassTipoIndice : INotifyPropertyChanged
    {

        int priv_id_tipo_indice;
        string priv_nombre;



        public event PropertyChangedEventHandler PropertyChanged;

        public int id_tipo_indice
        {
            get { return priv_id_tipo_indice; }
            set { priv_id_tipo_indice = value; }
        }
        public string nombre
        {
            get { return priv_nombre; }
            set { priv_nombre = value; }
        }
       
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        static public int LeerTipoIndice(System.IO.StreamWriter ArchivoRegEventos, ClassRegistro Eventos, MySqlConnection ConexDB, Parametros Param, out ObservableCollection<ClassTipoIndice> TiposIndiceRet, int idMolino = 0) //Rellenar colección observable
        {
            int retVal = 0;
            ObservableCollection<ClassTipoIndice> Controles = new ObservableCollection<ClassTipoIndice>();
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
            if (ConexDB.State.ToString() == "Open")
            {
                string sTabla = Constantes.tablaTipoIndice;
                string querySelect = "SELECT * FROM " + sTabla + "  ORDER BY id_tipo_indice";
                try
                {
                    mySqlDataAdapter.SelectCommand = new MySqlCommand(querySelect, ConexDB);
                    using (MySqlDataReader mySqlDataReader = mySqlDataAdapter.SelectCommand.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            id_tipo_indice = mySqlDataReader.GetOrdinal("id_tipo_indice"),
                            nombre = mySqlDataReader.GetOrdinal("nombre"),

                        };
                        while (mySqlDataReader.Read() == true)
                        {
                            var NuevoTipoIndiceTemp = new ClassTipoIndice();
                            NuevoTipoIndiceTemp.id_tipo_indice = mySqlDataReader.GetInt32(ordinals.id_tipo_indice);
                            NuevoTipoIndiceTemp.nombre = mySqlDataReader.GetString(ordinals.nombre);
                            Controles.Add(NuevoTipoIndiceTemp);
                        }
                        mySqlDataReader.Dispose();
                    }
                }
                catch (MySqlException ex)
                {
                    string[] primeraLinea = ex.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    Eventos.RegLinea(ArchivoRegEventos, "Error acceso a tabla de avanXpert " + sTabla + " en " + System.Reflection.MethodBase.GetCurrentMethod().Name + ": " + primeraLinea[0], Param.DepuracionVerMensajes);
                    retVal = -1;
                }

            }
            mySqlDataAdapter.Dispose();


            TiposIndiceRet = Controles;
            return retVal;
        }
    }
}
