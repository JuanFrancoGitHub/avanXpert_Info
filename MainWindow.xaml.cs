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
using System.Windows.Interop;
using System.Collections.ObjectModel;
using System.ComponentModel;
using avanXpert_Info.Código;
using avanXpert_Info.Código.Clases;

namespace avanXpert_Info
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Declaración de variables para funcionalidad genérica de la aplicación
        static public Parametros param;
        public ClassRegistro eventos;
        public System.IO.StreamWriter ArchivoRegEventos;
        ObservableCollection<ClassControles> Controles = new ObservableCollection<ClassControles>();
        int idMolino = 0;
        public MainWindow()
        {

            //Inicialización de variables------------------------------------------------------------------------//
            param = new Parametros();
            //Fin inicialización de variables------------------------------------------------------------------------//



            //Inicialización de la aplicación si exixte el fichero .ini---------------------------------------------------------------------------------------------------//
            int exsiteFichIni = param.LeerParamIni(1);
            if (exsiteFichIni > 0)
            {
                this.Dispatcher.UnhandledException += App_DispatcherUnhandledException; //Captura genérica de excepciones
                var hwnd = new WindowInteropHelper((Window)this).Handle;
                var value = GetWindowLong(hwnd, GWL_STYLE);
                SetWindowLong(hwnd, GWL_STYLE, (int)(value & ~WS_MAXIMIZEBOX));


                DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND); //Deshabilitar el botón de cerrar en la consola
                InitializeComponent();

                //Mostrar consola:
                if (!param.VerConsola)
                {
                    //CheckBoxConsole.IsChecked = false;
                    var handle = GetConsoleWindow();
                    ShowWindow(handle, SW_HIDE);
                }
                else
                {
                    //CheckBoxConsole.IsChecked = true;
                }

                eventos = new ClassRegistro("Log_");
                ArchivoRegEventos = eventos.AbrirRegistro(param.DirArchivosRegistro);
                eventos.RegLinea(ArchivoRegEventos, "----- Inicio aplicación -----", param.DepuracionVerMensajes);
                eventos.RegLinea(ArchivoRegEventos, "----- Aplicación inicializada en " + System.Environment.MachineName + ", " + System.Environment.UserName + " -----", false);

                if (ConectarDB() > 0)
                {
                    int numMolinos = ListarMolinos();
                    if (numMolinos > 0)
                    {
                        this.Height = numMolinos * 50 + 100;
                    }
                    ClassControles.LeerControles(ArchivoRegEventos, eventos, ConexDB, param, out Controles, idMolino);
                }

                else
                {
                    GridMolino1.Visibility = Visibility.Hidden;
                    GridMolino2.Visibility = Visibility.Hidden;
                    GridMolino3.Visibility = Visibility.Hidden;
                    GridMolino4.Visibility = Visibility.Hidden;
                    MessageBox.Show("No se ha realizado la conexión a la base de datos.", Constantes.ErrorMsgCaption, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                }
               
            }
        }
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            eventos.RegLinea(ArchivoRegEventos, "Excepción generada:\n" + e.Exception.ToString(), param.DepuracionVerMensajes);

            MessageBox.Show("Excepción generada:\n" + e.Exception.ToString());

            // Prevent default unhandled exception processing
            e.Handled = true;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Ventanas.VentDatos1 VentDatosMolino1 = new Ventanas.VentDatos1(this, ArchivoRegEventos, eventos, ConexDB, param);
            //VentDatosMolino1.ShowDialog();

            Ventanas.VentResultados1 VentResult1 = new Ventanas.VentResultados1(this, ArchivoRegEventos, eventos, ConexDB, param,0);
            VentResult1.ShowDialog();
        }

        private void BotonKPIs_Click(object sender, RoutedEventArgs e)
        {
            Button BotonMolino = sender as Button;
            Grid GridMolino =(Grid)BotonMolino.Parent;
            string NombreGrid = GridMolino.Name;
            string sNumero = "1";
            int nNumero = 1;
            if (NombreGrid.Length>1)
                sNumero= NombreGrid.Substring(NombreGrid.Length - 1, 1);
            bool tryParse = int.TryParse(sNumero, out nNumero);
            int idMolino = nNumero - 1;
            if (tryParse)
            {
                Ventanas.VentResultados1 VentResult1 = new Ventanas.VentResultados1(this, ArchivoRegEventos, eventos, ConexDB, param, idMolino);
                VentResult1.ShowDialog();
            }

        }


        private void MenuIndControl_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            MenuItem menuPadre = (MenuItem)menu.Parent;
            string NombreGrid = menuPadre.Name;
            string sNumero = "1";
            int nNumero = 1;
            if (NombreGrid.Length > 1)
                sNumero = NombreGrid.Substring(NombreGrid.Length - 1, 1);
            bool tryParse = int.TryParse(sNumero, out nNumero);
            int idMolino = nNumero - 1;

            Ventanas.VentDatos1 ventDatos1 = new Ventanas.VentDatos1(this, ArchivoRegEventos, eventos, ConexDB, param, idMolino);
            ventDatos1.ShowDialog();
        }
    }
}
