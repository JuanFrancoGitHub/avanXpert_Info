using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace avanXpert_Info
{
    public class ClassRegistro
    {
        //TextBox TextLineaUltimoEvento;
        public string EncabezArchivos;

        public ClassRegistro(string encabezArchivos)
        {
            //TextLineaUltimoEvento = textLineaUltimoEvento;
            //MainWindow.LineaUltimoEstado= textLineaUltimoEvento;
            EncabezArchivos = encabezArchivos;

        }   //Constructor. Recibe como argumento el nombre de los archivos (al que sigue la fecha)

        public System.IO.StreamWriter AbrirRegistro(string PathArchivo)
        {
            System.IO.StreamWriter ArchivoLog = null;
            FileStream fs = null;
            string nombreArchivo = PathArchivo + "\\" + EncabezArchivos + DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString("00") + DateTime.Today.Day.ToString("00") + ".log";
            try //Apertura del fichero de registro o log
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ISO-8859-1");
                fs = new FileStream(nombreArchivo, FileMode.Append);
                ArchivoLog = new StreamWriter(fs, encode);
                fs = null;
            }
            catch (Exception e)
            {
                if (MessageBox.Show("Error al abrir el archivo de registro " + nombreArchivo + "\n ¿Desea ver el detalle del error?",
                    Constantes.ErrorMsgCaption, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    MessageBox.Show(e.ToString(), "Detalle del error", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Cancel, MessageBoxOptions.DefaultDesktopOnly);
                }
                ArchivoLog = null;
            }
            return (ArchivoLog);
        }

        public int CerrarRegistro(System.IO.StreamWriter ArchivoLog)
        {
            if (ArchivoLog != null)
            {
                ArchivoLog.Close();
                ArchivoLog = null;
                return (1);
            }
            else
                return (1);
        }

        public int RegLinea(System.IO.StreamWriter ArchivoLog, string TextoRegistrar, bool MostrarMensajes)
        {
            string msg = DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString("000") + "ms " + TextoRegistrar;
            ArchivoLog.WriteLine(msg);
            ArchivoLog.Flush();
            if (MostrarMensajes)
            {

                Console.WriteLine(msg);
                try
                {
                    //MainWindow.LineaUltimoEstado = msg;
                    //TextLineaUltimoEvento.Text = msg;
                }
                catch
                { }
            }

            return (1);
        }

        public int RegLinea(string PathArchivo, string TextoRegistrar, bool MostrarMensajes)
        {
            System.IO.StreamWriter ArchivoLog = AbrirRegistro(PathArchivo);
            string msg = DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00") + "." + DateTime.Now.Millisecond.ToString("000") + "ms " + TextoRegistrar;
            if (MostrarMensajes)
            {
                Console.WriteLine(msg);
                try
                {
                    //MainWindow.LineaUltimoEstado = msg;
                    //TextLineaUltimoEvento.Text = msg;
                }
                catch
                { }
            }
            if (ArchivoLog != null)
            {
                ArchivoLog.WriteLine(msg);

                CerrarRegistro(ArchivoLog);
                return (-1);
            }
            return (1);
        }

    }
}
