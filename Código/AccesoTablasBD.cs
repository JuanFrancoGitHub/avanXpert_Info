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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace avanXpert_Info
{

    public partial class MainWindow : Window
    {
        public ManualResetEvent sendDone = new ManualResetEvent(false);
        private int ListarMolinos()
        {
            int retVal = 0;
            DataTable dtMolinos = new DataTable();
            MySqlDataReader mySqlDataReader=null;
            if (ConexDB.State.ToString() == "Open")
            {
                string sTabla = Constantes.tablaMolinos;
                string querySelectMolinos = "SELECT * FROM " + sTabla + " ORDER BY Id";
                MySqlDataAdapter mySqlDataAdpapMolinos = new MySqlDataAdapter();
                try
                {
                    mySqlDataAdpapMolinos.SelectCommand = new MySqlCommand(querySelectMolinos, ConexDB);
                    mySqlDataReader = mySqlDataAdpapMolinos.SelectCommand.ExecuteReader();
                    dtMolinos.Load(mySqlDataReader);
                }
                catch (MySqlException ex)
                {
                    string[] primeraLinea = ex.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    eventos.RegLinea(ArchivoRegEventos, "Error acceso a tabla de avanXpert " + sTabla + ": " + primeraLinea[0], param.DepuracionVerMensajes);
                    retVal = -1;
                }

                retVal = dtMolinos.Rows.Count;
                //Cambia la visualización de los botones de la ventana
                for (int idx0 = 0; idx0 < dtMolinos.Rows.Count; idx0++)
                {
                    switch (idx0)
                    {
                        case 0:
                            LabelMolino1.Content = dtMolinos.Rows[idx0]["name"];
                            break;
                        case 1:
                            LabelMolino2.Content = dtMolinos.Rows[idx0]["name"];
                            break;
                        case 2:
                            LabelMolino3.Content = dtMolinos.Rows[idx0]["name"];
                            break;
                        case 3:
                            LabelMolino4.Content = dtMolinos.Rows[idx0]["name"];
                            break;
                    }
                }
                for (int idx0 = dtMolinos.Rows.Count; idx0 < 4; idx0++)
                {
                    switch (idx0)
                    {
                        case 0:
                            GridMolino1.Visibility = Visibility.Hidden;
                            break;
                        case 1:
                            GridMolino2.Visibility = Visibility.Hidden;
                            break;
                        case 2:
                            GridMolino3.Visibility = Visibility.Hidden;
                            break;
                        case 3:
                            GridMolino4.Visibility = Visibility.Hidden;
                            break;
                    }
                }
                dtMolinos.Dispose();
                mySqlDataAdpapMolinos.Dispose();
                if (mySqlDataReader != null)
                    mySqlDataReader.Dispose();
            }
            else
            {
                GridMolino1.Visibility = Visibility.Hidden;
                GridMolino2.Visibility = Visibility.Hidden;
                GridMolino3.Visibility = Visibility.Hidden;
                GridMolino4.Visibility = Visibility.Hidden;
            }
            return retVal;
        }


    }
}