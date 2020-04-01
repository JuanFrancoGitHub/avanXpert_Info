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
    class ClassIndicesControl : INotifyPropertyChanged
    {
        int priv_id;
        int priv_tipo;
        string priv_nombre;
        int priv_id_control;
        int priv_id_molino;
        int priv_id_instrumento;
        int priv_id_consigna;

        public event PropertyChangedEventHandler PropertyChanged;
        public int id
        {
            get { return priv_id; }
            set { priv_id = value; }
        }
        public int tipo
        {
            get { return priv_tipo; }
            set { priv_tipo = value; }
        }
        public string nombre
        {
            get { return priv_nombre; }
            set { priv_nombre = value; }
        }
        public int id_control
        {
            get { return priv_id_control; }
            set { priv_id_control = value; }
        }
        public int id_molino
        {
            get { return priv_id_molino; }
            set { priv_id_molino = value; }
        }
        public int id_instrumento
        {
            get { return priv_id_instrumento; }
            set { priv_id_instrumento = value; }
        }
        public int id_consigna
        {
            get { return priv_id_consigna; }
            set { priv_id_consigna = value; }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        static public int LeerIndicesControl(System.IO.StreamWriter ArchivoRegEventos, ClassRegistro Eventos, MySqlConnection ConexDB, Parametros Param, out ObservableCollection<ClassIndicesControl> TiposIndiceRet, int idMolino = 0) //Rellenar colección observable
        {
            int retVal = 0;
            ObservableCollection<ClassIndicesControl> IndicesC = new ObservableCollection<ClassIndicesControl>();
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
            if (ConexDB.State.ToString() == "Open")
            {
                string sTabla = Constantes.tablaIndicesControl;
                string querySelect = "SELECT * FROM " + sTabla + "  ORDER BY tipo";
                try
                {
                    mySqlDataAdapter.SelectCommand = new MySqlCommand(querySelect, ConexDB);
                    using (MySqlDataReader mySqlDataReader = mySqlDataAdapter.SelectCommand.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            id = mySqlDataReader.GetOrdinal("id"),
                            tipo = mySqlDataReader.GetOrdinal("tipo"),
                            nombre = mySqlDataReader.GetOrdinal("nombre"),
                            id_control = mySqlDataReader.GetOrdinal("id_control"),
                            id_molino = mySqlDataReader.GetOrdinal("id_molino"),
                            id_instrumento = mySqlDataReader.GetOrdinal("id_instrumento"),
                            id_consigna = mySqlDataReader.GetOrdinal("id_consigna"),

                        };
                        while (mySqlDataReader.Read() == true)
                        {
                            var NuevoIndicesControlTemp = new ClassIndicesControl();
                            NuevoIndicesControlTemp.id = mySqlDataReader.GetInt32(ordinals.id);
                            NuevoIndicesControlTemp.tipo = mySqlDataReader.GetInt32(ordinals.tipo);
                            NuevoIndicesControlTemp.nombre = mySqlDataReader.GetString(ordinals.nombre);
                            NuevoIndicesControlTemp.id_control = mySqlDataReader.GetInt32(ordinals.id_control);
                            NuevoIndicesControlTemp.id_molino = mySqlDataReader.GetInt32(ordinals.id_molino);
                            NuevoIndicesControlTemp.id_instrumento = mySqlDataReader.GetInt32(ordinals.id_instrumento);
                            NuevoIndicesControlTemp.id_consigna = mySqlDataReader.GetInt32(ordinals.id_consigna);

                            IndicesC.Add(NuevoIndicesControlTemp);
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


            TiposIndiceRet = IndicesC;
            return retVal;
        }
    }
}
