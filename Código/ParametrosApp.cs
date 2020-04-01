using System;
using System.Windows;
using System.Collections;


namespace avanXpert_Info
{



    public class Parametros
    {
        public string ArchivoOrigen;

        public string DirArchivosRegistro; //Log de eventos
        public string DirArchivosReporte; //Reporte de muestras
        public string IpMySQLprim, IpMySQLstby, TcpPortMySQL, UserMySql, PassMySql;
        public bool DepuracionVerMensajes, ConectarAutoDB;
        public bool VerConsola, DBSoloVisualizar = true;
        public string AdminPassword = "NINGUNA";//En caso de no estar listado este parámetro en el .ini, esta sería la contraseña
        public IndicesDep D1 = new IndicesDep();
        public IndicesDep D2 = new IndicesDep();
        public IndicesDep D3 = new IndicesDep();
        public IndicesDep D4 = new IndicesDep();
        //        public int D1_IndiceAlim, D1_IndiceCalidad, D1_IndiceRdtoAlim, D1_IndiceConsEsp, D1_IndiceFRec;
        public int LeerParamIni(int modo)
        {
            System.IO.StreamReader ArchivoIni;
            int numlinea = 0;
            string linea;
            string[] separador = new string[] { ": " };
            string[] linea2;
            string RutaEjec;
            try //Apertura del fichero de parámetros
            {
                RutaEjec = AppDomain.CurrentDomain.BaseDirectory;
                ArchivoIni = new System.IO.StreamReader(RutaEjec + "\\avanXpert_Info.ini", System.Text.Encoding.Default, true);
            }
            catch (Exception e)
            {
                if (MessageBox.Show("Error al abrir el archivo de parámetros avanXpert_Conf.ini \n ¿Desea ver el detalle del error?",
                    Constantes.ErrorMsgCaption, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    MessageBox.Show(e.ToString(), "Detalle del error", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Cancel, MessageBoxOptions.DefaultDesktopOnly);
                }
                return (-1);
            }
            //
            try
            {
                do
                {
                    linea = ArchivoIni.ReadLine();
                    numlinea++;
                    linea2 = linea.Split(separador, StringSplitOptions.None);
                    if (linea2[0].Substring(0, 2) != "/*") //Identificador de comentario
                    {
                        switch (linea2[0])
                        {
                            case "DirArchivosRegistro":
                                DirArchivosRegistro = linea2[1];
                                break;
                            case "DirArchivosReporte":
                                DirArchivosReporte = linea2[1];
                                break;
                            case "DepuracionVerMensajes":
                                if ((linea2[1].ToUpper() == "SI") || (linea2[1].ToUpper(System.Globalization.CultureInfo.CurrentUICulture) == "SÍ"))
                                    DepuracionVerMensajes = true;
                                else
                                    DepuracionVerMensajes = false;
                                break;

                            case "VerConsola":
                                if ((linea2[1].ToUpper() == "SI") || (linea2[1].ToUpper(System.Globalization.CultureInfo.CurrentUICulture) == "SÍ"))
                                    VerConsola = true;
                                else
                                    VerConsola = false;
                                break;
                            case "ConectarAutoDB":
                                if ((linea2[1].ToUpper() == "SI") || (linea2[1].ToUpper(System.Globalization.CultureInfo.CurrentUICulture) == "SÍ"))
                                    ConectarAutoDB = true;
                                else
                                    ConectarAutoDB = false;
                                break;
                            case "IpMySQLprim":
                                IpMySQLprim = linea2[1];
                                break;
                            case "IpMySQLstby":
                                IpMySQLstby = linea2[1];
                                break;
                            case "TcpPortMySQL":
                                TcpPortMySQL = linea2[1];
                                break;
                            case "SoloVisualizar":
                                if ((linea2[1].ToUpper() == "SI") || (linea2[1].ToUpper(System.Globalization.CultureInfo.CurrentUICulture) == "SÍ"))
                                    DBSoloVisualizar = true;
                                else
                                    DBSoloVisualizar = false;
                                break;
                            case "AdminPassword":
                                AdminPassword = linea2[1];
                                break;
                            case "UserMySql":
                                UserMySql = linea2[1];
                                break;
                            case "PassMySql":
                                PassMySql = linea2[1];
                                break;

                            case "D1_IndiceAlim":
                                D1.IndiceAlim = int.Parse(linea2[1]);
                                break;
                            case "D1_IndiceCalidad":
                                D1.IndiceCalidad = int.Parse(linea2[1]);
                                break;
                            case "D1_IndiceRdtoAlim":
                                D1.IndiceRdtoAlim = int.Parse(linea2[1]);
                                break;
                            case "D1_IndiceConsEsp":
                                D1.IndiceConsEsp = int.Parse(linea2[1]);
                                break;
                            case "D1_IndiceFRec":
                                D1.IndiceFRec = int.Parse(linea2[1]);
                                break;
                            default:

                                //MessageBox.Show("No se reconoce el parámetro " + linea2[0], Constantes.ErrorMsgCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                break;
                        }//Switch
                    }
                } while (!(ArchivoIni.EndOfStream));
            }
            catch
            {
                MessageBox.Show("Error de formato en fichero config.ini línea " + numlinea, Constantes.ErrorMsgCaption, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                return (-2);
            }
            finally { }
            ArchivoIni.Close();

            return (1);
        }


    }
    public class IndicesDep
    {
        public int IndiceAlim, IndiceCalidad, IndiceRdtoAlim, IndiceConsEsp, IndiceFRec;
    }

    public class Constantes
    {
        public const string ErrorMsgCaption = "Error";
        public const string HechoMsgCaption = "Completado";
        public const string ConfirmarMsgCaption = "Confirmación";
        public const string tablaMolinos = "molinos";
        public const string tablaRecetas = "recetas";
        public const string tablaConsignas = "consignas";
        public const string tablaPerturbaciones = "perturbaciones";
        public const string tablaInestabilidades = "inestabilidad";
        public const string tablaNivelPerturbaciones = "nivelpert";
        public const string tablaNivelInestabilidades = "nivelinest";
        public const string tablaInstrumentos = "instrumentos";
        public const string tablaVarOpc = "varopc";
        public const string tablaControles = "controles";
        public const string tablaTipoIndice = "tipo_indice";
        public const string tablaIndicesControl = "indices_control";
        public const string tablaDatosControles = "datos_controles";
        public const string tablaDatosHorIndices = "datos_horarios_indices";

    }



}