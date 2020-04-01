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
    class ClassControles : INotifyPropertyChanged
    {
        int priv_id;
        int priv_id_molino;
        string priv_nombre;
        int priv_tipo;
        int priv_id_fuzzy;
        int priv_id_mpc;


        public event PropertyChangedEventHandler PropertyChanged;
        public int id
        {
            get { return priv_id; }
            set { priv_id = value; }
        }
        public int id_molino
        {
            get { return priv_id_molino; }
            set { priv_id_molino = value; }
        }
        public string nombre
        {
            get { return priv_nombre; }
            set { priv_nombre = value; }
        }
        public int tipo
        {
            get { return priv_tipo; }
            set { priv_tipo = value; }
        }
        public int id_fuzzy
        {
            get { return priv_id_fuzzy; }
            set { priv_id_fuzzy = value; }
        }
        public int id_mpc
        {
            get { return priv_id_mpc; }
            set { priv_id_mpc = value; }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        static public int LeerControles(System.IO.StreamWriter ArchivoRegEventos, ClassRegistro Eventos, MySqlConnection ConexDB, Parametros Param, out ObservableCollection<ClassControles> ContolesRet, int idMolino = 0) //Rellenar colección observable
        {
            int retVal = 0;
            ObservableCollection<ClassControles> Controles = new ObservableCollection<ClassControles>();
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
            if (ConexDB.State.ToString() == "Open")
            {
                string sTabla = Constantes.tablaControles;
                string querySelect = "SELECT * FROM " + sTabla + " WHERE id_molino = '" + idMolino + "' ORDER BY id";
                try
                {
                    mySqlDataAdapter.SelectCommand = new MySqlCommand(querySelect, ConexDB);
                    using (MySqlDataReader mySqlDataReader = mySqlDataAdapter.SelectCommand.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            id = mySqlDataReader.GetOrdinal("id"),
                            nombre = mySqlDataReader.GetOrdinal("nombre"),
                            id_molino = mySqlDataReader.GetOrdinal("id_molino"),
                            tipo = mySqlDataReader.GetOrdinal("tipo"),
                            id_fuzzy = mySqlDataReader.GetOrdinal("id_fuzzy"),
                            id_mpc = mySqlDataReader.GetOrdinal("id_mpc"),
                        };
                        while (mySqlDataReader.Read() == true)
                        {
                            var NuevoControlTemp = new ClassControles();


                            NuevoControlTemp.id = mySqlDataReader.GetInt32(ordinals.id);
                            NuevoControlTemp.id_molino = mySqlDataReader.GetInt32(ordinals.id_molino);
                            NuevoControlTemp.nombre = mySqlDataReader.GetString(ordinals.nombre);
                            NuevoControlTemp.tipo = mySqlDataReader.GetInt32(ordinals.tipo);
                            NuevoControlTemp.id_fuzzy = mySqlDataReader.GetInt32(ordinals.id_fuzzy);
                            NuevoControlTemp.id_mpc = mySqlDataReader.GetInt32(ordinals.id_mpc);

                            Controles.Add(NuevoControlTemp);
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


            ContolesRet = Controles;
            return retVal;
        }
    }
}
