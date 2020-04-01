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
    class ClassDatosControles : INotifyPropertyChanged
    {

        DateTime priv_fecha_hora;
        int priv_id_control;
        int priv_min_marcha;
        int priv_min_marcha_conec;
        int priv_min_disponible;
        int priv_min_dia;
        double priv_factor_utilizacion;
        double priv_factor_disponibilidad;               

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime fecha_hora
        {
            get { return priv_fecha_hora; }
            set { priv_fecha_hora = value; }
        }
        public int id_control
        {
            get { return priv_id_control; }
            set { priv_id_control = value; }
        }
        public int min_marcha
        {
            get { return priv_min_marcha; }
            set { priv_min_marcha = value; }
        }
        public int min_marcha_conec
        {
            get { return priv_min_marcha_conec; }
            set { priv_min_marcha_conec = value; }
        }
        public int min_disponible
        {
            get { return priv_min_disponible; }
            set { priv_min_disponible = value; }
        }
        public int min_dia
        {
            get { return priv_min_dia; }
            set { priv_min_dia = value; }
        }
        public double factor_utilizacion
        {
            get { return priv_factor_utilizacion; }
            set { priv_factor_utilizacion = value; }
        }
        public double factor_disponibilidad
        {
            get { return priv_factor_disponibilidad; }
            set { priv_factor_disponibilidad = value; }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        static public int LeerDatosControles(System.IO.StreamWriter ArchivoRegEventos, ClassRegistro Eventos, MySqlConnection ConexDB, Parametros Param, out ObservableCollection<ClassDatosControles> DatosControlRet, string sDesde, string sHasta,int idMolino = 0, int id_control = 0 ) //Rellenar colección observable
        {
            int retVal = 0;
            ObservableCollection<ClassDatosControles> DatosControles = new ObservableCollection<ClassDatosControles>();
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
            if (ConexDB.State.ToString() == "Open")
            {
                string sTabla = Constantes.tablaDatosControles;
                string querySelect = "";
                //if (sDesde==sHasta)
                //    querySelect = "SELECT * FROM " + sTabla + " WHERE id_control = '"+ id_control + "' ORDER BY fecha_hora";
                //else
                    querySelect = "SELECT * FROM " + sTabla + " WHERE (id_control = '" + id_control + "') AND (DATE(fecha_hora) BETWEEN '" + sDesde.ToString() + "' AND '" + sHasta.ToString() + "') ORDER BY fecha_hora";
                try
                {
                    mySqlDataAdapter.SelectCommand = new MySqlCommand(querySelect, ConexDB);
                    using (MySqlDataReader mySqlDataReader = mySqlDataAdapter.SelectCommand.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            fecha_hora = mySqlDataReader.GetOrdinal("fecha_hora"),
                            id_control = mySqlDataReader.GetOrdinal("id_control"),
                            min_marcha = mySqlDataReader.GetOrdinal("min_marcha"),
                            min_marcha_conec = mySqlDataReader.GetOrdinal("min_marcha_conec"),
                            min_disponible = mySqlDataReader.GetOrdinal("min_disponible"),
                            min_dia = mySqlDataReader.GetOrdinal("min_dia"),
                            factor_utilizacion = mySqlDataReader.GetOrdinal("factor_utilizacion"),
                            factor_disponibilidad = mySqlDataReader.GetOrdinal("factor_disponibilidad"),
                        };
                        while (mySqlDataReader.Read() == true)
                        {
                            var NuevoDatoControlTemp = new ClassDatosControles();
                            NuevoDatoControlTemp.fecha_hora = mySqlDataReader.GetDateTime(ordinals.fecha_hora);
                            NuevoDatoControlTemp.id_control = mySqlDataReader.GetInt32(ordinals.id_control);
                            NuevoDatoControlTemp.min_marcha = mySqlDataReader.GetInt32(ordinals.min_marcha);
                            NuevoDatoControlTemp.min_marcha_conec = mySqlDataReader.GetInt32(ordinals.min_marcha_conec);
                            NuevoDatoControlTemp.min_disponible = mySqlDataReader.GetInt32(ordinals.min_disponible);
                            NuevoDatoControlTemp.min_dia = mySqlDataReader.GetInt32(ordinals.min_dia);
                            NuevoDatoControlTemp.factor_utilizacion = mySqlDataReader.GetDouble(ordinals.factor_utilizacion);
                            NuevoDatoControlTemp.factor_disponibilidad = mySqlDataReader.GetDouble(ordinals.factor_disponibilidad);
                            DatosControles.Add(NuevoDatoControlTemp);
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


            DatosControlRet = DatosControles;
            return retVal;
        }
    }
}
