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

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace avanXpert_Info
{

    public partial class MainWindow : Window
    {

        static string ConectString = "";
        MySqlConnection ConexDB;

        private int ConectarDB()
        {
            int retVal = 0;
            if (param.ConectarAutoDB == true)
            {
                try
                {
                    if (param.DBSoloVisualizar)
                        ConectString = @"server=" + param.IpMySQLprim + ";port=" + param.TcpPortMySQL + "; userid=Visualizar;password="+ param.PassMySql + ";database=avanxpert;CharSet=utf8";
                    else
                        ConectString = @"server=" + param.IpMySQLprim + ";port=" + param.TcpPortMySQL + "; userid="+param.UserMySql+";password="+param.PassMySql+";database=avanxpert;CharSet=utf8";
                    ConexDB = new MySqlConnection(ConectString);

                    // Se abre la conexion
                    ConexDB.Open();

                    string stm = "SELECT VERSION()";
                    MySqlCommand cmdConectar = new MySqlCommand(stm, ConexDB);
                    string version = Convert.ToString(cmdConectar.ExecuteScalar());
                    retVal = 1;
                }
                catch (MySqlException ex)
                {
                    string[] primeraLinea = ex.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    eventos.RegLinea(ArchivoRegEventos, "Error apertura base de datos avanXpert: " + primeraLinea[0], param.DepuracionVerMensajes);
                    //MenItChecConectarDB.IsChecked = false;
                    retVal = -1;
                }
                finally
                {
                    eventos.RegLinea(ArchivoRegEventos, "Conexión a base de datos avanXpert: " + ConexDB.State.ToString(), param.DepuracionVerMensajes);
                }
            }
            if (ConexDB.State.ToString() != "Open") //No se pudo conectar. Se intenta con se el secundario
            {
                if (MessageBox.Show("No se pudo conectar a la base de datos del servidor primario en " + param.IpMySQLprim + "\n¿Desea conectarse al secundario? \n\n Precaución: ¡Esta base de datos podría no estar actualizada!",
                    Constantes.ErrorMsgCaption, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (param.DBSoloVisualizar)
                            ConectString = @"server=" + param.IpMySQLstby + ";port=" + param.TcpPortMySQL + "; userid=Visualizar;password=" + param.PassMySql + ";database=avanxpert;CharSet=utf8";
                        else
                            ConectString = @"server=" + param.IpMySQLstby + ";port=" + param.TcpPortMySQL + "; userid=" + param.UserMySql + ";password=" + param.PassMySql + ";database=avanxpert;CharSet=utf8";
                        ConexDB = new MySqlConnection(ConectString);

                        // Se abre la conexion
                        ConexDB.Open();

                        string stm = "SELECT VERSION()";
                        MySqlCommand cmdConectar = new MySqlCommand(stm, ConexDB);
                        string version = Convert.ToString(cmdConectar.ExecuteScalar());
                        retVal = 2;
                    }
                    catch (MySqlException ex)
                    {
                        string[] primeraLinea = ex.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        eventos.RegLinea(ArchivoRegEventos, "Error apertura base de datos avanXpert: " + primeraLinea[0], param.DepuracionVerMensajes);
                        //MenItChecConectarDB.IsChecked = false;
                        retVal = -2;

                    }
                    finally
                    {
                        if (ConexDB.State.ToString() == "Open")
                        {
                            eventos.RegLinea(ArchivoRegEventos, "Conexión a base de datos avanXpert establecida en servidor secundario " + param.IpMySQLstby, param.DepuracionVerMensajes);
                            MessageBox.Show("Conexión a base de datos avanXpert establecida en servidor secundario " + param.IpMySQLstby, Constantes.ErrorMsgCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        }
                    }
                }
            }
            return retVal;

        }

        private void DesConectarDB()
        {

            eventos.RegLinea(ArchivoRegEventos, "Se pulsa desconectar de la base de datos", param.DepuracionVerMensajes);
            ConexDB.Close();
            GC.Collect();
            GC.WaitForFullGCComplete();

        }
    }
}