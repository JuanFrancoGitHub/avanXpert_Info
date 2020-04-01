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
    class ClassDatosHorIndices : INotifyPropertyChanged
    {

        DateTime priv_fecha_hora;
        private int priv_id_indice;
        private int priv_id_receta;
        private double priv_media;
        private double priv_media_conec;

        private double priv_desv_tip;
        private double priv_desv_tip_conec;
        private double priv_total;
        private double priv_total_conec;

        private int priv_num_valores;
        private int priv_num_valores_conec;

        public event PropertyChangedEventHandler PropertyChanged;
        public ClassDatosHorIndices()
        {
            agrupa = 1;
            //media = 0.0;
            //media_conec = 0.0;

        }

        public DateTime fecha_hora
        {
            get { return priv_fecha_hora; }
            set { priv_fecha_hora = value; }
        }
        public int id_indice
        {
            get { return priv_id_indice; }
            set { priv_id_indice = value; }
        }
        public int id_receta
        {
            get { return priv_id_receta; }
            set { priv_id_receta = value; }
        }
        public double media
        {
            get { return priv_media; }
            set { priv_media = value; }
        }
        public double media_conec
        {
            get { return priv_media_conec; }
            set { priv_media_conec = value; }
        }
        public double desv_tip
        {
            get { return priv_desv_tip; }
            set { priv_desv_tip = value; }
        }
        public double desv_tip_conec
        {
            get { return priv_desv_tip_conec; }
            set { priv_desv_tip_conec = value; }
       }
        public double total
        {
            get { return priv_total; }
            set { priv_total = value; }
        }
        public double total_conec
        {
            get { return priv_total_conec; }
            set { priv_total_conec = value; }
        }

        public int num_valores
        {
            get { return priv_num_valores; }
            set { priv_num_valores = value; }
        }
        public int num_valores_conec
        {
            get { return priv_num_valores_conec; }
            set { priv_num_valores_conec = value; }
        }
        public int agrupa //Indica cuantos elementos agrupa en las colecciones de resumen
        {
            get; set;
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        static public int LeerDatosHorIndices(System.IO.StreamWriter ArchivoRegEventos, ClassRegistro Eventos, MySqlConnection ConexDB, 
            Parametros Param, out ObservableCollection<ClassDatosHorIndices> DatosHorIndicesRet, int id_receta, string sDesde, string sHasta, int idMolino = 0, int id_control = 0) //Rellenar colección observable
        {
            int retVal = 0;
            ObservableCollection<ClassDatosHorIndices> DatosHorIndices = new ObservableCollection<ClassDatosHorIndices>();
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
            if (ConexDB.State.ToString() == "Open")
            {
                string sTabla = Constantes.tablaDatosHorIndices;
                string querySelect = "SELECT * FROM " + sTabla + " WHERE(id_receta = '" + id_receta + "') AND (id_indice = '" + id_control + "') AND (DATE(fecha_hora) BETWEEN '" + sDesde.ToString() + "' AND '" + sHasta.ToString() + "') ORDER BY fecha_hora";
                try
                {
                    mySqlDataAdapter.SelectCommand = new MySqlCommand(querySelect, ConexDB);
                    using (MySqlDataReader mySqlDataReader = mySqlDataAdapter.SelectCommand.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            fecha_hora = mySqlDataReader.GetOrdinal("fecha_hora"),
                            id_indice = mySqlDataReader.GetOrdinal("id_indice"),
                            id_receta = mySqlDataReader.GetOrdinal("id_receta"),
                          
                            media = mySqlDataReader.GetOrdinal("media"),
                            media_conec = mySqlDataReader.GetOrdinal("media_conec"),
                            
                            desv_tip = mySqlDataReader.GetOrdinal("desv_tip"),
                            desv_tip_conec = mySqlDataReader.GetOrdinal("desv_tip_conec"),

                            total = mySqlDataReader.GetOrdinal("total"),
                            total_conec = mySqlDataReader.GetOrdinal("total_conec"),

                            num_valores = mySqlDataReader.GetOrdinal("num_valores"),
                            num_valores_conec = mySqlDataReader.GetOrdinal("num_valores_conec"),
                     
                           
                        };
                        while (mySqlDataReader.Read() == true)
                        {
                            var NuevoDatosHorIndicesTemp = new ClassDatosHorIndices();
                            NuevoDatosHorIndicesTemp.fecha_hora = mySqlDataReader.GetDateTime(ordinals.fecha_hora);
                            NuevoDatosHorIndicesTemp.id_indice = mySqlDataReader.GetInt32(ordinals.id_indice);
                            NuevoDatosHorIndicesTemp.id_receta = mySqlDataReader.GetInt32(ordinals.id_receta);
                            
                            NuevoDatosHorIndicesTemp.media = mySqlDataReader.GetDouble(ordinals.media);
                            NuevoDatosHorIndicesTemp.media_conec = mySqlDataReader.GetDouble(ordinals.media_conec);

                            NuevoDatosHorIndicesTemp.desv_tip = mySqlDataReader.GetDouble(ordinals.desv_tip);
                            NuevoDatosHorIndicesTemp.desv_tip_conec = mySqlDataReader.GetDouble(ordinals.desv_tip_conec);


                            NuevoDatosHorIndicesTemp.total = mySqlDataReader.GetDouble(ordinals.total);
                            NuevoDatosHorIndicesTemp.total_conec = mySqlDataReader.GetDouble(ordinals.total_conec);

                            NuevoDatosHorIndicesTemp.num_valores = mySqlDataReader.GetInt32(ordinals.num_valores);
                            NuevoDatosHorIndicesTemp.num_valores_conec = mySqlDataReader.GetInt32(ordinals.num_valores_conec);



                            DatosHorIndices.Add(NuevoDatosHorIndicesTemp);

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


            DatosHorIndicesRet = DatosHorIndices;
            return retVal;
        }
    }
}
