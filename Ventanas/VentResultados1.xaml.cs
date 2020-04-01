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
using System.Threading;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

using avanXpert_Info;
using avanXpert_Info.Código;
using avanXpert_Info.Código.Clases;


using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;


namespace avanXpert_Info.Ventanas
{
    /// <summary>
    /// Lógica de interacción para VentResultados1.xaml
    /// </summary>
    public partial class VentResultados1 : Window
    {
        System.IO.StreamWriter ArchivoRegEventos;
        ClassRegistro Eventos;
        MainWindow mainWindow;
        MySqlConnection ConexDB;
        Parametros Param;
        int idMolino = 0;
        private int NumColumnasChart =10;
        //ObservableCollection<ClassDatosControles> datosControles;
        //ObservableCollection<ClassControles> Controles;
        
        // Datos de utilización de AvanXpert: Horas de molino, AvX conectado e indice de utilización
        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesBrutoUtil;
        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesUtil;
        
        // Datos de rendimiento de molino: Horas de aliment, aliment objetivo e indice de producción
        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesBrutoRdtoProd;
        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesRdtoProd;

        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesBrutoCalidad;
        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesCalidad;

        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesBrutoEficiencia;
        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesEficiencia;

        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesBrutoConsE;
        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesConsE;


        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesBrutoFRec;
        private ObservableCollection<ClassDatosHorIndices> DatosHorIndicesFrec;

        private ObservableCollection<ClassReceta> RecetasMolino;


        DatosAnimación Datos1 = new DatosAnimación() 
        { 
            sUtilizaciónMedia = "0.00", sHorasMolTot = "0", sHorasAvXTot = "0" , 
            sHorasAlimObj="0", sProductividad="0.00", sProducción="0",sMuestras="0",
            sCalidad="0",sEficiencia="0", sConsEspAvXp ="0.0", sConsEspNoAvXp="0.0" ,
            sFRecAvXp="0.00", sFRecNoAvXp = "0.00",sProducciónNoAvXp="0"
        };
        
        #region //Definición de series para los gráficos
        public SeriesCollection SerieHorasMolino { get; set; }
        public SeriesCollection SeriesDatosUtiliz { get; set; }
        public SeriesCollection SeriesDatosCalidad { get; set; }
        public SeriesCollection SeriesDatosEficiencia{ get; set; }
        public SeriesCollection SeriesDatosProd { get; set; }
        #endregion

        #region //Definición de formato para las series 
        public Func<double, string> FormatterTiempo { get; set; }
        public Func<double, string> FormatterEjeYh { get; set; }
        public Func<double, string> FormatterEjeYperc0dec { get; set; }
        public Func<double, string> FormatterEjeYperc1dec { get; set; }
        public Func<double, string> FormatterEjeYmuestras { get; set; }
        public Func<double, string> FormatterEjeYpu2dec { get; set; }
        #endregion

        public VentResultados1(MainWindow VentMain, System.IO.StreamWriter eventosMain, ClassRegistro RegEventosMain, MySqlConnection ConexDbMain, Parametros paramMain, int idMolinoMain = 0)
        {
            mainWindow = VentMain;
            ArchivoRegEventos = eventosMain;
            Eventos = RegEventosMain;
            ConexDB = ConexDbMain;
            Param = paramMain;
            idMolino = idMolinoMain;
            InitializeComponent();

        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            DgTipos.SelectionChanged -= DgTipos_SelectionChanged;
            RecetasMolino = new ObservableCollection<ClassReceta>();
            ClassReceta recetaVacía = new ClassReceta();
            recetaVacía.id_molino = idMolino;
            recetaVacía.Indice_Receta = -1;
            recetaVacía.name = "Todos";
            RecetasMolino.Add(recetaVacía);

            FillOcRecetas();
            DgTipos.ItemsSource = RecetasMolino;

            DgTipos.Focus();
            DgTipos.SelectedItem = RecetasMolino[0];
            DgTipos.ScrollIntoView(DgTipos.Items[0]);

            FechaHasta.SelectedDate = DateTime.Today;
            TimeSpan difT = new TimeSpan(2, 0, 0, 0);
            DateTime previo = DateTime.Today;
            previo = previo.Subtract(difT);
            FechaDesde.SelectedDate = previo;
            DateTime Desde = FechaDesde.SelectedDate.Value;
            DgTipos.SelectionChanged += DgTipos_SelectionChanged;

            FormatterTiempo = value => new System.DateTime((long)(Math.Max(0, value) * TimeSpan.FromHours(1).Ticks)).ToString("dd/MM (HH") + "h)";
            FormatterEjeYh = value => value.ToString("00") + "%";
            FormatterEjeYperc0dec = value => value.ToString("00") + "%";
            FormatterEjeYpu2dec = value => value.ToString("0.00");
            FormatterEjeYperc1dec = value => value.ToString("00.0") + "%";
            FormatterEjeYmuestras = value => value.ToString("0.0") + "/h";


            SerieHorasMolino = new SeriesCollection();
            SeriesDatosUtiliz = new SeriesCollection();
            SeriesDatosCalidad = new SeriesCollection();
            SeriesDatosEficiencia = new SeriesCollection();

        }

        bool Inicializado = false;
        private void BotActualizarVista_Click(object sender, RoutedEventArgs e)
        {
            Inicializado = true;
             //ChartOEE_Actualizado = false;
             //ChartHoras_Actualizado = false;
             //ChartProd_Actualizado = false;
             //ChartCalidad_Actualizado = false;

             //ChartOEE_SeriesCargadas = false;
             //ChartHoras_SeriesCargadas = false;
             //ChartProd_SeriesCargadas = false;
             //ChartCalidad_SeriesCargadas = false;

            DateTime Desde = FechaDesde.SelectedDate.Value;
            DateTime Hasta = FechaHasta.SelectedDate.Value;
            if (Desde > Hasta)
            {
                MessageBox.Show("La fecha \"A\" debe ser posterior a \"De\".", Constantes.ErrorMsgCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.Cancel, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            ClassReceta RecetaSelect = DgTipos.SelectedItem as ClassReceta;
            int receta = RecetaSelect.Indice_Receta;
            // ActualizarVistaCharUtiliz(receta);
            //Eventos.RegLinea(ArchivoRegEventos, "Ini Tick", Param.DepuracionVerMensajes);
            IndicesDep depart = new IndicesDep();
            switch (idMolino)
            {
                case 0:
                    depart = Param.D1;
                    break;
                case 1:
                    depart = Param.D2;
                    break;
                case 2:
                    depart = Param.D3;
                    break;
                case 3:
                    depart = Param.D4;
                    break;

            }
            LeerDatosUtilizProd(receta,depart);
            LeerDatosRdtoProd(receta, depart);
            LeerDatosCalidad(receta, depart);
            int num_casados=ComponerDatosEfic(DatosHorIndicesBrutoRdtoProd, DatosHorIndicesBrutoCalidad, out DatosHorIndicesBrutoEficiencia);

            LeerDatosConsumo(receta, depart);
            LeerDatosFRec(receta, depart);

            EnlazarValoresVisores();

            //Datos1.VaciarValores();
           
            //Los datos de utilización no es necesario agruparlos (resumir) porque no van a ninguna serie de los gráficos. Tampoco los de consumo
            ResumirDatosHorIndices(DatosHorIndicesBrutoRdtoProd, out DatosHorIndicesRdtoProd, NumColumnasChart);
            ResumirDatosHorIndices(DatosHorIndicesBrutoCalidad, out DatosHorIndicesCalidad, NumColumnasChart);
            ResumirDatosHorIndices(DatosHorIndicesBrutoEficiencia, out DatosHorIndicesEficiencia, NumColumnasChart);


           
            CargarSeriesCalidad();
            
            CargarSeriesProduc();

            CargarSeriesUtiliz();

            CargarSeriesEficiencia();
            



            //DgTest1.ItemsSource = DatosHorIndicesBrutoRdtoProd;
            //DgTest2.ItemsSource = DatosHorIndicesBrutoCalidad;
            //DgTest3.ItemsSource = DatosHorIndicesBrutoEficiencia;

        }
        private void DgTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ClassReceta RecetaSelect = DgTipos.SelectedItem as ClassReceta;
            int receta = RecetaSelect.Indice_Receta;

            for (int i = 0; i < DgTipos.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)DgTipos.ItemContainerGenerator.ContainerFromIndex(i);
                row.Style = null;
            }


            //ActualizarVista(receta);

        }
        private void FillOcRecetas() //Rellenar colección observable Recetas
        {
            if (RecetasMolino == null)
            {
                RecetasMolino = new ObservableCollection<ClassReceta>();
            }
            MySqlDataAdapter mySqlDataAdapReceta = new MySqlDataAdapter();
            if (ConexDB.State.ToString() == "Open")
            {
                string sTabla = Constantes.tablaRecetas;
                string querySelectRecetas = "SELECT *  FROM " + sTabla + " WHERE id_molino = '" + idMolino + "' ORDER BY indice_receta";
                try
                {
                    mySqlDataAdapReceta.SelectCommand = new MySqlCommand(querySelectRecetas, ConexDB);
                    using (MySqlDataReader mySqlDataReaderRecetas = mySqlDataAdapReceta.SelectCommand.ExecuteReader())
                    {
                        var ordinals = new
                        {
                            id = mySqlDataReaderRecetas.GetOrdinal("id"),
                            name = mySqlDataReaderRecetas.GetOrdinal("name"),
                            idMolino = mySqlDataReaderRecetas.GetOrdinal("id_molino"),
                            status = mySqlDataReaderRecetas.GetOrdinal("status"),
                            activa = mySqlDataReaderRecetas.GetOrdinal("activa"),
                            Indice_Receta = mySqlDataReaderRecetas.GetOrdinal("Indice_Receta"),
                        };
                        while (mySqlDataReaderRecetas.Read() == true)
                        {
                            var NuevaRecetaTemp = new ClassReceta();
                            String test = mySqlDataReaderRecetas.GetString(ordinals.name);
                            NuevaRecetaTemp.id = mySqlDataReaderRecetas.GetInt32(ordinals.id);
                            NuevaRecetaTemp.name = mySqlDataReaderRecetas.GetString(ordinals.name);
                            NuevaRecetaTemp.id_molino = mySqlDataReaderRecetas.GetInt32(ordinals.idMolino);
                            NuevaRecetaTemp.status = mySqlDataReaderRecetas.GetInt32(ordinals.status);
                            NuevaRecetaTemp.activa = mySqlDataReaderRecetas.GetInt32(ordinals.activa);
                            NuevaRecetaTemp.Indice_Receta = mySqlDataReaderRecetas.GetInt32(ordinals.Indice_Receta);

                            RecetasMolino.Add(NuevaRecetaTemp);
                        }
                        mySqlDataReaderRecetas.Dispose();
                    }
                }
                catch (MySqlException ex)
                {
                    string[] primeraLinea = ex.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    Eventos.RegLinea(ArchivoRegEventos, "Error acceso a tabla de avanXpert " + sTabla + " en " + System.Reflection.MethodBase.GetCurrentMethod().Name + ": " + primeraLinea[0], Param.DepuracionVerMensajes);
                }

            }
            mySqlDataAdapReceta.Dispose();
        }

        private void LeerDatosUtilizProd(int TipoSelec, IndicesDep Dep)
        {
            DateTime Desde = FechaDesde.SelectedDate.Value;
            string sDesde = Desde.Year + "-" + Desde.Month + "-" + Desde.Day;
            DateTime Hasta = FechaHasta.SelectedDate.Value;
            string sHasta = Hasta.Year + "-" + Hasta.Month + "-" + Hasta.Day;
            DatosHorIndicesBrutoUtil = new ObservableCollection<ClassDatosHorIndices>();
            DatosHorIndicesUtil = new ObservableCollection<ClassDatosHorIndices>();
            int id_indice = Dep.IndiceAlim;//Índice de alimentación, 0
            ClassDatosHorIndices.LeerDatosHorIndices(ArchivoRegEventos, Eventos, ConexDB, Param, out DatosHorIndicesBrutoUtil, TipoSelec, sDesde, sHasta, idMolino, id_indice);
            //ResumirDatosHorIndices(DatosHorIndicesBrutoUtil, out DatosHorIndicesUtil, NumColumnasChart);
            //CargarSeriesUtiliz();
        }
        private void LeerDatosRdtoProd(int TipoSelec, IndicesDep Dep)
        {
            DateTime Desde = FechaDesde.SelectedDate.Value;
            string sDesde = Desde.Year + "-" + Desde.Month + "-" + Desde.Day;
            DateTime Hasta = FechaHasta.SelectedDate.Value;
            string sHasta = Hasta.Year + "-" + Hasta.Month + "-" + Hasta.Day;
            DatosHorIndicesBrutoRdtoProd = new ObservableCollection<ClassDatosHorIndices>();
            DatosHorIndicesRdtoProd = new ObservableCollection<ClassDatosHorIndices>();
                        
            int id_indice = Dep.IndiceRdtoAlim;//Índice de rendimiento de la alimentación, 2
            ClassDatosHorIndices.LeerDatosHorIndices(ArchivoRegEventos, Eventos, ConexDB, Param, out DatosHorIndicesBrutoRdtoProd, TipoSelec, sDesde, sHasta, idMolino, id_indice);


        }
        private void LeerDatosCalidad(int TipoSelec, IndicesDep Dep)
        {
            DateTime Desde = FechaDesde.SelectedDate.Value;
            string sDesde = Desde.Year + "-" + Desde.Month + "-" + Desde.Day;
            DateTime Hasta = FechaHasta.SelectedDate.Value;
            string sHasta = Hasta.Year + "-" + Hasta.Month + "-" + Hasta.Day;
            DatosHorIndicesBrutoCalidad = new ObservableCollection<ClassDatosHorIndices>();
            DatosHorIndicesCalidad = new ObservableCollection<ClassDatosHorIndices>();

            int id_indice = Dep.IndiceCalidad;//Índice de calidad, 3
            ClassDatosHorIndices.LeerDatosHorIndices(ArchivoRegEventos, Eventos, ConexDB, Param, out DatosHorIndicesBrutoCalidad, TipoSelec, sDesde, sHasta, idMolino, id_indice);


        }
        private void LeerDatosConsumo(int TipoSelec, IndicesDep Dep)
        {
            DateTime Desde = FechaDesde.SelectedDate.Value;
            string sDesde = Desde.Year + "-" + Desde.Month + "-" + Desde.Day;
            DateTime Hasta = FechaHasta.SelectedDate.Value;
            string sHasta = Hasta.Year + "-" + Hasta.Month + "-" + Hasta.Day;
            DatosHorIndicesBrutoConsE = new ObservableCollection<ClassDatosHorIndices>();
            DatosHorIndicesConsE = new ObservableCollection<ClassDatosHorIndices>();

            int id_indice = Dep.IndiceConsEsp;//Índice de cons específico, 4
            ClassDatosHorIndices.LeerDatosHorIndices(ArchivoRegEventos, Eventos, ConexDB, Param, out DatosHorIndicesBrutoConsE, TipoSelec, sDesde, sHasta, idMolino, id_indice);


        }
        private void LeerDatosFRec(int TipoSelec, IndicesDep Dep)
        {
            DateTime Desde = FechaDesde.SelectedDate.Value;
            string sDesde = Desde.Year + "-" + Desde.Month + "-" + Desde.Day;
            DateTime Hasta = FechaHasta.SelectedDate.Value;
            string sHasta = Hasta.Year + "-" + Hasta.Month + "-" + Hasta.Day;
            DatosHorIndicesBrutoFRec = new ObservableCollection<ClassDatosHorIndices>();
            DatosHorIndicesFrec = new ObservableCollection<ClassDatosHorIndices>();

            int id_indice = Dep.IndiceFRec;//Índice de factor de recirculación, 10
            ClassDatosHorIndices.LeerDatosHorIndices(ArchivoRegEventos, Eventos, ConexDB, Param, out DatosHorIndicesBrutoFRec, TipoSelec, sDesde, sHasta, idMolino, id_indice);


        }

        private void ResumirDatosHorIndices(ObservableCollection<ClassDatosHorIndices> DatosBruto, out ObservableCollection<ClassDatosHorIndices> DatosResumenRet, int nMaxElem = 10) //Simplifica la colección resumiendo el número de puntos
        {
            ObservableCollection<ClassDatosHorIndices> DatosResumen = new ObservableCollection<ClassDatosHorIndices>();
            if (DatosBruto==null)
            {
                DatosResumenRet = null;
                return;
            }
            int NumElemIni = DatosBruto.Count;
            if (NumElemIni <= nMaxElem)
            {
                DatosResumenRet = DatosBruto;
                return;
            }
            if (nMaxElem < 1)
            {
                DatosResumenRet = DatosResumen;
                return;
            }
            //
            int numAgrupar = NumElemIni / nMaxElem; //Número de muestras a resumir para cada barra del gráfico
            int Resto = NumElemIni % nMaxElem; // Número de muestras que sobran, las incluyo en la primera barra
            int num_valores_t = 0, num_valores_conec_t = 0;
            double media_t = 0, media_conec_t = 0, desv_tip_t = 0, desv_tip_conec_t = 0, total = 0, total_conec = 0;
            int iResto = 0;
            bool AgrupadaConResto = false;
            for (int iGrupo = 0; iGrupo < nMaxElem; iGrupo++)
            {
                ClassDatosHorIndices MuestraAgrupada = new ClassDatosHorIndices();
                AgrupadaConResto = false;
                bool agrupando = false;
                for (int iAgrupar = 0; iAgrupar < numAgrupar; iAgrupar++)
                {
                    int iOrig = iAgrupar + iGrupo * numAgrupar + iResto;
                    if (iResto < Resto)//En los primeros grupos meto los elementos de resto. P.e. si hay 11 muestras en bruto y se agrupan de 3 en 3, salen 3 grupos con 4+4+3 elementos
                    {
                        if (!AgrupadaConResto)
                        {
                            iResto++;
                            iAgrupar--; //Para no avanzar en el bucle interno
                            AgrupadaConResto = true;
                        }
                    }
                    MuestraAgrupada.media += DatosBruto[iOrig].media * DatosBruto[iOrig].num_valores;
                    MuestraAgrupada.media_conec += DatosBruto[iOrig].media_conec * DatosBruto[iOrig].num_valores_conec;
                    MuestraAgrupada.desv_tip += DatosBruto[iOrig].desv_tip * DatosBruto[iOrig].num_valores;
                    MuestraAgrupada.desv_tip_conec += DatosBruto[iOrig].desv_tip_conec * DatosBruto[iOrig].num_valores_conec;
                    MuestraAgrupada.total += DatosBruto[iOrig].total;
                    MuestraAgrupada.total_conec += DatosBruto[iOrig].total_conec;
                    MuestraAgrupada.num_valores += DatosBruto[iOrig].num_valores;
                    MuestraAgrupada.num_valores_conec += DatosBruto[iOrig].num_valores_conec;
                    if (agrupando)
                    {
                        MuestraAgrupada.agrupa++;
                    }
                    agrupando = true;

                    //Cómputo para el dato total en pantalla
                    //num_valores_t += DatosBruto[iOrig].num_valores;
                    //num_valores_conec_t += DatosBruto[iOrig].num_valores_conec;
                    //media_t += DatosBruto[iOrig].media * DatosBruto[iOrig].num_valores;
                    //media_conec_t += DatosBruto[iOrig].media_conec * DatosBruto[iOrig].num_valores_conec;
                    //desv_tip_t += DatosBruto[iOrig].desv_tip * DatosBruto[iOrig].num_valores;
                    //desv_tip_conec_t += DatosBruto[iOrig].desv_tip_conec * DatosBruto[iOrig].num_valores_conec;
                    //total += DatosBruto[iOrig].total * DatosBruto[iOrig].num_valores;
                    //total_conec += DatosBruto[iOrig].total_conec * DatosBruto[iOrig].num_valores_conec;



                    if (iAgrupar == (numAgrupar - 1)) //Ultima muestra para el grupo. Se pone esta fecha al grupo
                    {
                        MuestraAgrupada.fecha_hora = DatosBruto[iOrig].fecha_hora;
                        MuestraAgrupada.id_indice = DatosBruto[iOrig].id_indice;
                        MuestraAgrupada.id_receta = DatosBruto[iOrig].id_receta;

                        if (MuestraAgrupada.num_valores > 0)
                        {
                            MuestraAgrupada.media = MuestraAgrupada.media / MuestraAgrupada.num_valores;
                            MuestraAgrupada.desv_tip = MuestraAgrupada.desv_tip / MuestraAgrupada.num_valores;
                        }
                        if (MuestraAgrupada.num_valores_conec > 0)
                        {
                            MuestraAgrupada.media_conec = MuestraAgrupada.media_conec / MuestraAgrupada.num_valores_conec;
                            MuestraAgrupada.desv_tip_conec = MuestraAgrupada.desv_tip_conec / MuestraAgrupada.num_valores_conec;
                        }

                    }
                }
                DatosResumen.Add(MuestraAgrupada);
                MuestraAgrupada = null;
            }

            DatosResumenRet = DatosResumen;
            // Datos1.HorasAlimObj = media_t* total /10000;
        }
        private int ComponerDatosEfic(ObservableCollection<ClassDatosHorIndices> DatosHorasRdtoProd, ObservableCollection<ClassDatosHorIndices> DatosHorasCalidad, out ObservableCollection<ClassDatosHorIndices> DatosHorasEfic)
        {
            int retVal = 0;
            if (DatosHorasRdtoProd == null)
            {
                retVal = -1;
                DatosHorasEfic = null;
                return retVal;
            }
            if (DatosHorasRdtoProd.Count == 0)
            {
                retVal = -2;
                DatosHorasEfic = null;
                return retVal;
            }
            ObservableCollection<ClassDatosHorIndices> DatosEficTemp = new ObservableCollection<ClassDatosHorIndices>();
            //Por cada elemento de la colecc de Datos de rendimiento de producción, busca en la colección de calidad el elemento con hora más reciente. Si es >1h d diferencia, se descarta y se asume calidad =100

            int num_casados=0; //Cuántos de los datos de prod se pueden casar con datos de calidad.
            foreach (ClassDatosHorIndices DatoProducc in DatosHorasRdtoProd)
            {
                DateTime HoraProd = DatoProducc.fecha_hora;
                double productividad = 0.01 * DatoProducc.media_conec;
                double utiliz = 0;
                if ((DatoProducc.num_valores_conec + DatoProducc.num_valores) > 0)
                {
                    utiliz = (double)DatoProducc.num_valores_conec / (double)(DatoProducc.num_valores_conec + DatoProducc.num_valores);
                }
                double oee = productividad * utiliz;
                bool encontrado = false;
                if ((oee!=0)&&(DatosHorasCalidad!=null)) //Si es cero ya no busco la calidad
                {
                    foreach (ClassDatosHorIndices DatoCalidad in DatosHorasCalidad)
                    {
                        if ((DatoProducc.fecha_hora > DatoCalidad.fecha_hora) &&(DatoProducc.fecha_hora - DatoCalidad.fecha_hora).TotalMinutes < 60)  //Comprueba que el ultimo dato de calidad tiene menos de una hora respecto al de producción 
                        {
                            num_casados++;
                            oee *= 0.01*DatoCalidad.media; //Tomo el valor media, no media_conec por si en ese instante justo no estuvo conectado. Si está conectado o no lo dice el dato de proudcción
                            encontrado = true;
                            break;
                        }
                    }
                }
                ClassDatosHorIndices DatoEfic = new ClassDatosHorIndices();
                DatoEfic.fecha_hora = HoraProd;
                DatoEfic.id_indice = DatoProducc.id_indice;
                DatoEfic.id_receta = DatoProducc.id_receta;
                DatoEfic.media = DatoProducc.media;
                DatoEfic.media_conec = oee;                
        
                DatoEfic.total = DatoProducc.total;
                DatoEfic.total_conec = DatoProducc.total_conec;
                DatoEfic.num_valores = DatoProducc.num_valores;
                DatoEfic.num_valores_conec = DatoProducc.num_valores_conec;
                DatosEficTemp.Add(DatoEfic);
            }
            DatosHorasEfic = DatosEficTemp;
            return num_casados;
        }
        private void CargarSeriesProduc()
        {
            if (DatosHorIndicesRdtoProd==null)
            {
                return;
            }
            var DatosHorasAvXpMarchaObj = Mappers.Xy<ClassDatosHorIndices>()
                .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                .Y(ModeloDia => (0.01 * ModeloDia.total_conec * ModeloDia.media_conec) / ModeloDia.agrupa); //0-100, porcentaje de tiempo de marcha en el que se ha llegado al obj con AvXp
            var DatosHorasAvXp = Mappers.Xy<ClassDatosHorIndices>()
                .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                .Y(ModeloDia => ModeloDia.total_conec / ModeloDia.agrupa);//0-100, porcentaje de tiempo AvXp conectado de esa hora en marcha

            var DatosHorFactorProd = Mappers.Xy<ClassDatosHorIndices>()
                 .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                 .Y(ModeloDia => ModeloDia.media_conec);//0-1 Porcentaje en el que se consigue el objetivo en el tiempo de AvXp conectado

            ChartProd.AxisY.Clear();
            ChartProd.AxisY.Add(new Axis
            {
                Name = "MarchaMolino",
                Title = "Marcha molino (%h)",
                MaxValue = 110,
                MaxRange = 100,
                MinRange = 0,
                MinValue=0,
                LabelFormatter = FormatterEjeYh,
                Foreground = System.Windows.Media.Brushes.Black,
            });
            ChartProd.AxisY.Add(new Axis
            {
                Name = "FactorProd",
                Title = "Factor de producción (%)",
                MaxValue = 105,
                MaxRange = 100,
                MinValue = 0,
                //MaxRange = 1,
                //MinRange = 0,
                LabelFormatter = FormatterEjeYperc0dec,
                Foreground = System.Windows.Media.Brushes.DarkCyan,

                Position = AxisPosition.RightTop
            });

            if (SeriesDatosProd != null)
            {
                SeriesDatosProd = null;
                DataContext = null;

            }
            SeriesDatosProd = new SeriesCollection();

            ColumnSeries serieHorasMarchaAvXp = new ColumnSeries(DatosHorasAvXp)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesRdtoProd),
                Title = "Horas AvXpert en marcha",
                ScalesYAt = 0,
                Fill = Brushes.DarkGray, //Relleno de la barra


                DataLabels = true, //Números con valores sobre la barra
                FontSize = 8,
                Foreground = Brushes.Black, //Texto con el valor

                Stroke = Brushes.Black, //Borde de la barra
                StrokeThickness = 1,  //Borde de la barra
                MaxColumnWidth = 20,
                ColumnPadding = -9,
                //StackMode = StackMode.Values
            };

            ColumnSeries serieHorasAxXpMarchaObj = new ColumnSeries(DatosHorasAvXpMarchaObj)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesRdtoProd),
                Title = "Horas aliment. objetivo",
                ScalesYAt = 0,
                Fill = Brushes.SlateGray, //Relleno de la barra

                DataLabels = true, //Números con valores sobre la barra
                FontSize = 8,
                Foreground = Brushes.Black, //Texto con el valor

                Stroke = Brushes.Black, //Borde de la barra
                StrokeThickness = 1,  //Borde de la barra
                MaxColumnWidth = 12,
                ColumnPadding = -15,
                //StackMode = StackMode.Values
            };

            LineSeries serieHorasFactorProd = new LineSeries(DatosHorFactorProd)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesRdtoProd),
                Fill = Brushes.Transparent,
                Title = "Factor de producción",
                ScalesYAt = 1,
                LineSmoothness = 0.6,
                StrokeThickness = 2,
                DataLabels = true,
                FontSize = 8,
                Foreground = Brushes.DarkCyan,
                Width = 10,

                Stroke = Brushes.DarkCyan,
                PointGeometrySize = 5
            };

            //ChartProd_SeriesCargadas = true;

            SeriesDatosProd.Add(serieHorasMarchaAvXp);
            SeriesDatosProd.Add(serieHorasAxXpMarchaObj);
            SeriesDatosProd.Add(serieHorasFactorProd);
            DataContext = this;
        }
        private double CalcProporcion(int Valor1, int Valor2)
        {
            double denom = (double)Valor1 + (double)Valor2;
            if (denom==0)
                return 0;
            else
                return (double)Valor1 / denom;
        }
        private void CargarSeriesUtiliz()
        {
            if (DatosHorIndicesRdtoProd==null)
            {
                return;
            }

            var DatosHorasAvXp = Mappers.Xy<ClassDatosHorIndices>()
                 .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                 .Y(ModeloDia => 100*ModeloDia.num_valores_conec / 60 / ModeloDia.agrupa);
            var DatosHorasMolino = Mappers.Xy<ClassDatosHorIndices>()
                 .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                 .Y(ModeloDia => 100*(ModeloDia.num_valores + ModeloDia.num_valores_conec)/60/ ModeloDia.agrupa);
            var DatosFactorUtiliz = Mappers.Xy<ClassDatosHorIndices>()
                  .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                  //.Y(ModeloDia => 100*ModeloDia.num_valores_conec /(ModeloDia.num_valores + ModeloDia.num_valores_conec));
                  .Y(ModeloDia => 100 * CalcProporcion(ModeloDia.num_valores_conec ,ModeloDia.num_valores));

            //Notice you can also configure this type globally, so you don't need to configure every
            //SeriesCollection instance using the type.
            //more info at http://lvcharts.net/App/Index#/examples/v1/wpf/Types%20and%20Configuration




            ChartHorasAvXprt.AxisY.Clear();
            ChartHorasAvXprt.AxisY.Add(new Axis
            {
                Name = "MarchaMolino",
                Title = "Marcha molino (%h)",
                MaxValue = 110,
                MaxRange = 100,
                MinValue = 0,
                MinRange = 0,
                LabelFormatter = FormatterEjeYh,
                Foreground = System.Windows.Media.Brushes.Black,

            }); 
            ChartHorasAvXprt.AxisY.Add(new Axis
            {
                Name = "Utiliz",
                Title = "Util. AvXpert (%)",
                MaxValue = 120,
                MaxRange = 100,
                MinValue = 0,
                MinRange = 0,
                LabelFormatter = FormatterEjeYperc0dec,
                Foreground = System.Windows.Media.Brushes.DarkCyan,

                Position = AxisPosition.RightTop
            });


            if (SeriesDatosUtiliz != null)
            {
                SeriesDatosUtiliz = null;
                DataContext = null;

            }
            SeriesDatosUtiliz = new SeriesCollection();

            ColumnSeries serieMolinoMarcha = new ColumnSeries(DatosHorasMolino)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesRdtoProd), //Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesUtil),

                Title = "Horas molino en marcha",
                ScalesYAt = 0,
                Fill = Brushes.DarkGray, //Relleno de la barra


                DataLabels = true, //Números con valores sobre la barra
                FontSize = 8,
                Foreground = Brushes.Black, //Texto con el valor

                Stroke = Brushes.Black, //Borde de la barra
                StrokeThickness = 1,  //Borde de la barra
                MaxColumnWidth = 20,
                ColumnPadding = -10,
                //StackMode = StackMode.Values
            };
            ColumnSeries serieAvXptConectado = new ColumnSeries(DatosHorasAvXp)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesRdtoProd), //Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesUtil),
                Title = "Horas avanXpert conectado",
                ScalesYAt = 0,
                Fill = Brushes.SlateGray, //Relleno de la barra


                DataLabels = false, //Números con valores sobre la barra
                FontSize = 8,
                Foreground = Brushes.MidnightBlue, //Texto con el valor

                Stroke = Brushes.MidnightBlue, //Borde de la barra
                StrokeThickness = 1,  //Borde de la barra
                MaxColumnWidth = 12,//double.PositiveInfinity,
                ColumnPadding = -15,
                //StackMode = StackMode.Values
            };
            LineSeries serieUtilizacion = new LineSeries(DatosFactorUtiliz)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesRdtoProd),//Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesUtil),
                Fill = Brushes.Transparent,
                Title = "Factor de utilización",
                ScalesYAt = 1,
                LineSmoothness = 0.6,
                StrokeThickness = 2,
                DataLabels = true,
                FontSize = 8,
                Foreground = Brushes.DarkCyan,

                Stroke = Brushes.DarkCyan,
                PointGeometrySize = 5
            };

/*            ChartHoras_SeriesCargadas = true*/;

            SeriesDatosUtiliz.Add(serieMolinoMarcha);
            SeriesDatosUtiliz.Add(serieAvXptConectado);
            SeriesDatosUtiliz.Add(serieUtilizacion);


            DataContext = this;

        }
        private void CargarSeriesCalidad()
        {
            if (DatosHorIndicesCalidad==null)
            {
                return;
            }

            var DatosMuestrasAvXp = Mappers.Xy<ClassDatosHorIndices>()
                 .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                 .Y(ModeloDia => (ModeloDia.media_conec * ModeloDia.num_valores_conec/60 ) / ModeloDia.agrupa); //Número de muestras por el porcentaje de las que son buenas, con avXp conectado. TODO: QUITAR 60
            var DatosMuestrasTot = Mappers.Xy<ClassDatosHorIndices>()
                 .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                 .Y(ModeloDia => (ModeloDia.num_valores_conec)/ ModeloDia.agrupa); //Número de muestras con avXp conectado.
            var DatosFactorCalidad = Mappers.Xy<ClassDatosHorIndices>()
                  .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                  .Y(ModeloDia =>  ModeloDia.media_conec); //Porcentaje de muestras buenas

            //Notice you can also configure this type globally, so you don't need to configure every
            //SeriesCollection instance using the type.
            //more info at http://lvcharts.net/App/Index#/examples/v1/wpf/Types%20and%20Configuration




            ChartCalidad.AxisY.Clear();
            ChartCalidad.AxisY.Add(new Axis
            {
                Name = "Muestras",
                Title = "Núm muestras/h",
                MaxValue = 70,//3.5,
                MaxRange = 60, //3,
                MinValue = 0,
                MinRange = 0,
                LabelFormatter = FormatterEjeYmuestras,
                Foreground = System.Windows.Media.Brushes.Black,

            });
            ChartCalidad.AxisY.Add(new Axis
            {
                Name = "Calidad",
                Title = "Calidad (%)",
                MaxValue = 120,
                MaxRange = 100,
                MinValue = 0,
                MinRange = 0,
                LabelFormatter = FormatterEjeYperc1dec,
                Foreground = System.Windows.Media.Brushes.DarkCyan,

                Position = AxisPosition.RightTop
            });


            if (SeriesDatosCalidad != null)
            {
                SeriesDatosCalidad = null;
                DataContext = null;

            }
            SeriesDatosCalidad = new SeriesCollection();

            ColumnSeries serieMuestrasAvXpTot = new ColumnSeries(DatosMuestrasTot)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesCalidad),

                Title = "Muestras analizadas",
                ScalesYAt = 0,
                Fill = Brushes.DarkGray, //Relleno de la barra


                DataLabels = true, //Números con valores sobre la barra
                FontSize = 8,
                Foreground = Brushes.Black, //Texto con el valor

                Stroke = Brushes.Black, //Borde de la barra
                StrokeThickness = 1,  //Borde de la barra
                MaxColumnWidth = 20,
                ColumnPadding = -10,
                //StackMode = StackMode.Values
            };
            ColumnSeries serieMuestrasAvXpCal = new ColumnSeries(DatosMuestrasAvXp)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesCalidad), 
                Title = "Muestras OK",
                ScalesYAt = 0,
                Fill = Brushes.SlateGray, //Relleno de la barra


                DataLabels = false, //Números con valores sobre la barra
                FontSize = 8,
                Foreground = Brushes.MidnightBlue, //Texto con el valor

                Stroke = Brushes.MidnightBlue, //Borde de la barra
                StrokeThickness = 1,  //Borde de la barra
                MaxColumnWidth = 12,//double.PositiveInfinity,
                ColumnPadding = -15,
                //StackMode = StackMode.Values
            };
            LineSeries serieCalidad = new LineSeries(DatosFactorCalidad)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesCalidad),//Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesUtil),
                Fill = Brushes.Transparent,
                Title = "Factor de calidad",
                ScalesYAt = 1,
                LineSmoothness = 0.6,
                StrokeThickness = 2,
                DataLabels = true,
                FontSize = 8,
                Foreground = Brushes.DarkCyan,

                Stroke = Brushes.DarkCyan,
                PointGeometrySize = 5
            };

            //ChartCalidad_SeriesCargadas = true;

            SeriesDatosCalidad.Add(serieMuestrasAvXpTot);
            SeriesDatosCalidad.Add(serieMuestrasAvXpCal);
            SeriesDatosCalidad.Add(serieCalidad);


            DataContext = this;

        }
        private void CargarSeriesEficiencia()
        {
            if ((DatosHorIndicesEficiencia == null) || (DatosHorIndicesRdtoProd == null))
            {
                //DatosHorIndicesEficiencia = null;
                if (SeriesDatosEficiencia != null)
                {
                    SeriesDatosEficiencia.Clear();
                    //SeriesDatosEficiencia = null;
                }
                return;
            }
            var DatosHorasMolino = Mappers.Xy<ClassDatosHorIndices>()
                 .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                 .Y(ModeloDia => 100*(ModeloDia.num_valores_conec + ModeloDia.num_valores)/60 / ModeloDia.agrupa);
            var DatosFactorEfic = Mappers.Xy<ClassDatosHorIndices>()
                  .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                  .Y(ModeloDia => ModeloDia.media_conec);

            //Notice you can also configure this type globally, so you don't need to configure every
            //SeriesCollection instance using the type.
            //more info at http://lvcharts.net/App/Index#/examples/v1/wpf/Types%20and%20Configuration


            ChartOEE.AxisY.Clear();
            ChartOEE.AxisY.Add(new Axis
            {
                Name = "MarchaMolino",
                Title = "Marcha molino (%h)",
                MaxValue = 110,
                MaxRange = 100,
                MinValue = 0,
                MinRange = 0,
                LabelFormatter = FormatterEjeYh,
                Foreground = System.Windows.Media.Brushes.Black,

            });
            ChartOEE.AxisY.Add(new Axis
            {
                Name = "Efic",
                Title = "Eficiencia",
                MaxValue = 1.20,
                MaxRange = 1.00,
                MinValue = 0,
                MinRange = 0,
                LabelFormatter = FormatterEjeYpu2dec,
                Foreground = System.Windows.Media.Brushes.DarkCyan,

                Position = AxisPosition.RightTop
            });


            if (SeriesDatosEficiencia != null)
            {
                SeriesDatosEficiencia = null;
                DataContext = null;

            }
            SeriesDatosEficiencia = new SeriesCollection();

            ColumnSeries serieMolinoMarcha = new ColumnSeries(DatosHorasMolino)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesRdtoProd), //Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesUtil),

                Title = "Horas molino en marcha",
                ScalesYAt = 0,
                Fill = Brushes.DarkGray, //Relleno de la barra


                DataLabels = true, //Números con valores sobre la barra
                FontSize = 8,
                Foreground = Brushes.Black, //Texto con el valor

                Stroke = Brushes.Black, //Borde de la barra
                StrokeThickness = 1,  //Borde de la barra
                MaxColumnWidth = 20,
                ColumnPadding = -10,
                //StackMode = StackMode.Values
            };

            LineSeries serieEficiencia = new LineSeries(DatosFactorEfic)
            {
                Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesEficiencia),//Values = new ChartValues<ClassDatosHorIndices>(DatosHorIndicesUtil),
                Fill = Brushes.Transparent,
                Title = "Factor de Eficiencia (OEE)",
                ScalesYAt = 1,
                LineSmoothness = 0.6,
                StrokeThickness = 2,
                DataLabels = true,
                FontSize = 8,
                Foreground = Brushes.DarkCyan,

                Stroke = Brushes.DarkCyan,
                PointGeometrySize = 5
            };

            //ChartOEE_SeriesCargadas = true;

            SeriesDatosEficiencia.Add(serieMolinoMarcha);
            SeriesDatosEficiencia.Add(serieEficiencia);


            DataContext = this;

        }
        private void EnlazarValoresVisores()
        {

            Visor1_Utilización.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValor1 = new Binding("sUtilizaciónMedia");
            BindingValor1.Mode = BindingMode.TwoWay;
            BindingValor1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Visor1_Utilización.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValor1);

            Visor2_Efic.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValor2E = new Binding("sEficiencia");
            BindingValor2E.Mode = BindingMode.TwoWay;
            BindingValor2E.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Visor2_Efic.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValor2E);

            VisorA_HorasAvXp.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorVA = new Binding("sHorasAvXTot");
            BindingValorVA.Mode = BindingMode.TwoWay;
            BindingValorVA.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            VisorA_HorasAvXp.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorVA);

            VisorB_HorasMol.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValor3 = new Binding("sHorasMolTot");
            BindingValor3.Mode = BindingMode.TwoWay;
            BindingValor3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            VisorB_HorasMol.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValor3);

            VisorC_HorasAlim.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorC = new Binding("sHorasAlimObj");
            //BindingValorC.Mode = BindingMode.TwoWay;
            BindingValorC.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            VisorC_HorasAlim.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorC);


            VisorD_MuetrasTot.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorD = new Binding("sMuestras");
            //BindingValorD.Mode = BindingMode.TwoWay;
            BindingValorD.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            VisorD_MuetrasTot.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorD);

            VisorE_MuetrasVal.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorE = new Binding("sMuestrasOK");
            //BindingValorE.Mode = BindingMode.TwoWay;
            BindingValorE.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            VisorE_MuetrasVal.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorE);


            Visor3_Product.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorP = new Binding("sProductividad");
            //BindingValorP.Mode = BindingMode.TwoWay;
            BindingValorP.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Visor3_Product.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorP);

            Visor4_Calidad.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValor4 = new Binding("sCalidad");
            //BindingValor4.Mode = BindingMode.TwoWay;
            BindingValor4.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Visor4_Calidad.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValor4);

            Visor5_Alim.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorAlim = new Binding("sProducción");
            //BindingValorP.Mode = BindingMode.TwoWay;
            BindingValorAlim.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Visor5_Alim.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorAlim);


            Visor7_ConsE.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorConsEspAvXp = new Binding("sConsEspAvXp");
            //BindingValorP.Mode = BindingMode.TwoWay;
            BindingValorConsEspAvXp.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Visor7_ConsE.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorConsEspAvXp);

            Visor8_Recirc.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorFRecAvXp = new Binding("sFRecAvXp");
            //BindingValorFRecAvXp.Mode = BindingMode.TwoWay;
            BindingValorFRecAvXp.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Visor8_Recirc.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorFRecAvXp);


            Visor9_AlimSin.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorAlimSin = new Binding("sProducciónNoAvXp");
            //BindingValorP.Mode = BindingMode.TwoWay;
            BindingValorAlimSin.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Visor9_AlimSin.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorAlimSin);




            Visor11_ConsESin.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorConsEspNoAvXp = new Binding("sConsEspNoAvXp");
            //BindingValorP.Mode = BindingMode.TwoWay;
            BindingValorConsEspNoAvXp.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Visor11_ConsESin.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorConsEspNoAvXp);

            Visor12_RecircSin.TextBlockCantidad.DataContext = Datos1;
            Binding BindingValorFRecNoAvXp = new Binding("sFRecNoAvXp");
            //BindingValorFRecNoAvXp.Mode = BindingMode.TwoWay;
            BindingValorFRecNoAvXp.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            Visor12_RecircSin.TextBlockCantidad.SetBinding(TextBlock.TextProperty, BindingValorFRecNoAvXp);

        }
        private void PonerValoresVisores()
        {
            //if (DatosHorIndicesUtil != null)
            //{
            //    if (DatosHorIndicesUtil.Count > 0)
            //    {
            //        Datos1.HorasMolTot = DatosHorIndicesUtil.Sum(DatosControles => DatosControles.total);
            //        Datos1.sHorasMolTot = DatosAnimación.ConvTexto(Datos1.HorasMolTot, "###");

            //        Datos1.HorasAvXTot = DatosHorIndicesUtil.Sum(DatosControles => DatosControles.total_conec);
            //        Datos1.sHorasAvXTot = DatosAnimación.ConvTexto(Datos1.HorasAvXTot, "###");

            //        Datos1.UtilizaciónMedia = DatosHorIndicesUtil.Average(DatosControles => DatosControles.total_conec / DatosControles.total);
            //        Datos1.sUtilizaciónMedia = DatosAnimación.ConvTexto(Datos1.UtilizaciónMedia, "0.00");
            //    }

            //}
            if (DatosHorIndicesBrutoRdtoProd != null)
            {
                if (DatosHorIndicesBrutoRdtoProd.Count > 0)
                {

                    Datos1.HorasMolTot = DatosHorIndicesBrutoRdtoProd.Sum(DatosControles => DatosControles.num_valores+DatosControles.num_valores_conec)/60;
                    Datos1.sHorasMolTot = DatosAnimación.ConvTexto(Datos1.HorasMolTot, "0");

                    Datos1.HorasAvXTot =DatosHorIndicesBrutoRdtoProd.Sum(DatosControles => DatosControles.num_valores_conec)/60;
                    Datos1.sHorasAvXTot = DatosAnimación.ConvTexto(Datos1.HorasAvXTot, "0");

                    Datos1.HorasAlimObj = DatosHorIndicesBrutoRdtoProd.Sum(DatosControles => DatosControles.media_conec * DatosControles.num_valores_conec) / 6000;
                    Datos1.sHorasAlimObj = DatosAnimación.ConvTexto(Datos1.HorasAlimObj, "0");

                    double total = DatosHorIndicesBrutoRdtoProd.Sum(DatosControles => DatosControles.num_valores + DatosControles.num_valores_conec );
                    if (total>0)
                    {
                        //Datos1.UtilizaciónMedia = DatosHorIndicesBrutoRdtoProd.Average(DatosControles => DatosControles.total_conec / DatosControles.total);
                        //Datos1.sUtilizaciónMedia = DatosAnimación.ConvTexto(Datos1.UtilizaciónMedia, "0.00");
                        Datos1.UtilizaciónMedia = DatosHorIndicesBrutoRdtoProd.Sum(DatosControles => DatosControles.num_valores_conec) / total;
                        Datos1.sUtilizaciónMedia = DatosAnimación.ConvTexto(Datos1.UtilizaciónMedia, "0.00");
                    }
                    else
                    {
                        Datos1.UtilizaciónMedia = 0;
                        Datos1.sUtilizaciónMedia = DatosAnimación.ConvTexto(Datos1.UtilizaciónMedia, "0.00");
                    }





                    double total_conec = DatosHorIndicesBrutoRdtoProd.Sum(DatosControles => DatosControles.num_valores_conec);
                    if (total_conec > 0)
                    {
                        Datos1.Productividad = 0.01 * DatosHorIndicesBrutoRdtoProd.Sum(DatosControles => DatosControles.media_conec * DatosControles.num_valores_conec) / total_conec;
                        Datos1.sProductividad = DatosAnimación.ConvTexto(Datos1.Productividad, "0.00");
                    }
                    else
                    {
                        Datos1.Productividad = 0;
                        Datos1.sProductividad = DatosAnimación.ConvTexto(Datos1.Productividad, "0.00");
                    }


                    double num_valoresConec = 0;
                    num_valoresConec = DatosHorIndicesBrutoUtil.Sum(DatosControles => DatosControles.num_valores_conec);
                    if (num_valoresConec > 0)
                    {
                        Datos1.Producción = DatosHorIndicesBrutoUtil.Sum(DatosControles => (DatosControles.media_conec * DatosControles.num_valores_conec))/ num_valoresConec;
                        Datos1.sProducción = DatosAnimación.ConvTexto(Datos1.Producción, "0");
                    }
                    else
                    {
                        Datos1.Producción = 0;
                        Datos1.sProducción = DatosAnimación.ConvTexto(Datos1.Producción, "0");
                    }
                    double num_valoresNoConec = 0;
                    num_valoresNoConec = DatosHorIndicesBrutoUtil.Sum(DatosControles => DatosControles.num_valores);
                    if (num_valoresNoConec > 0)
                    {
                        Datos1.ProducciónNoAvXp = DatosHorIndicesBrutoUtil.Sum(DatosControles => DatosControles.media * DatosControles.num_valores) / num_valoresNoConec;
                        Datos1.sProducciónNoAvXp = DatosAnimación.ConvTexto(Datos1.ProducciónNoAvXp, "0");
                    }
                    else
                    {
                        Datos1.ProducciónNoAvXp = 0;
                        Datos1.sProducciónNoAvXp = DatosAnimación.ConvTexto(Datos1.ProducciónNoAvXp, "0");
                    }

                }
                else
                {
                    Datos1.HorasMolTot = 0;
                    Datos1.sHorasMolTot = DatosAnimación.ConvTexto(Datos1.HorasMolTot, "0");

                    Datos1.HorasAvXTot = 0;
                    Datos1.sHorasAvXTot = DatosAnimación.ConvTexto(Datos1.HorasAvXTot, "0");

                    Datos1.UtilizaciónMedia = 0;
                    Datos1.sUtilizaciónMedia = DatosAnimación.ConvTexto(Datos1.UtilizaciónMedia, "0.00");

                    Datos1.HorasAlimObj = 0;
                    Datos1.sHorasAlimObj = DatosAnimación.ConvTexto(Datos1.HorasAlimObj, "0");

                    Datos1.Productividad = 0;
                    Datos1.sProductividad = DatosAnimación.ConvTexto(Datos1.Productividad, "0.00");

                    Datos1.Producción = 0;
                    Datos1.sProducción = DatosAnimación.ConvTexto(Datos1.Producción, "0");

                    Datos1.ProducciónNoAvXp = 0;
                    Datos1.sProducciónNoAvXp = DatosAnimación.ConvTexto(Datos1.ProducciónNoAvXp, "0");
                }
            }
            else
            {
                Datos1.HorasMolTot = 0;
                Datos1.sHorasMolTot = DatosAnimación.ConvTexto(Datos1.HorasMolTot, "0");

                Datos1.HorasAvXTot = 0;
                Datos1.sHorasAvXTot = DatosAnimación.ConvTexto(Datos1.HorasAvXTot, "0");

                Datos1.UtilizaciónMedia = 0;
                Datos1.sUtilizaciónMedia = DatosAnimación.ConvTexto(Datos1.UtilizaciónMedia, "0.00");

                Datos1.HorasAlimObj = 0;
                Datos1.sHorasAlimObj = DatosAnimación.ConvTexto(Datos1.HorasAlimObj, "0");

                Datos1.Productividad = 0;
                Datos1.sProductividad = DatosAnimación.ConvTexto(Datos1.Productividad, "0.00");

                Datos1.Producción = 0;
                Datos1.sProducción = DatosAnimación.ConvTexto(Datos1.Producción, "0");

                Datos1.ProducciónNoAvXp = 0;
                Datos1.sProducciónNoAvXp = DatosAnimación.ConvTexto(Datos1.ProducciónNoAvXp, "0");

            }
           
            if (DatosHorIndicesBrutoCalidad != null)
            {
                int num_datos = 0;
                num_datos = DatosHorIndicesBrutoCalidad.Count;
                if (num_datos > 0)
                {
                    Datos1.Muestras = DatosHorIndicesBrutoCalidad.Sum(DatosControles => (DatosControles.num_valores_conec));
                    Datos1.sMuestras = DatosAnimación.ConvTexto(Datos1.Muestras, "0");

                    Datos1.MuestrasOK = (int)DatosHorIndicesBrutoCalidad.Sum(DatosControles => (0.01 * DatosControles.media_conec * DatosControles.num_valores_conec));
                    Datos1.sMuestrasOK = DatosAnimación.ConvTexto(Datos1.MuestrasOK, "0");
                    if (Datos1.Muestras > 0)
                    {
                        Datos1.Calidad = (float)Datos1.MuestrasOK / Datos1.Muestras;
                        Datos1.sCalidad = DatosAnimación.ConvTexto(Datos1.Calidad, "0.00");
                    }
                    else
                        Datos1.Calidad = 0;
                }
                else
                {
                    Datos1.Muestras = 0;
                    Datos1.sMuestras = DatosAnimación.ConvTexto(Datos1.Producción, "0");
                    Datos1.MuestrasOK = 0;
                    Datos1.sMuestrasOK = DatosAnimación.ConvTexto(Datos1.MuestrasOK, "0");
                    Datos1.Calidad = 0;
                    Datos1.sCalidad = DatosAnimación.ConvTexto(Datos1.Calidad, "0.00");
                }
            }
            else
            {
                Datos1.Muestras = 0;
                Datos1.sMuestras = DatosAnimación.ConvTexto(Datos1.Producción, "0");
                Datos1.MuestrasOK = 0;
                Datos1.sMuestrasOK = DatosAnimación.ConvTexto(Datos1.MuestrasOK, "0");
                Datos1.Calidad = 0;
                Datos1.sCalidad = DatosAnimación.ConvTexto(Datos1.Calidad, "0.00");
            }
      
            Datos1.Eficiencia = Datos1.UtilizaciónMedia * Datos1.Productividad * Datos1.Calidad;
            Datos1.sEficiencia = DatosAnimación.ConvTexto(Datos1.Eficiencia, "0.00");


            if (DatosHorIndicesBrutoConsE != null)
            {
                int num_datos = 0;
                num_datos = DatosHorIndicesBrutoConsE.Count;
                if (num_datos > 0)
                {
                    int num_valores_conec = DatosHorIndicesBrutoConsE.Sum(DatosControles => (DatosControles.num_valores_conec));
                    if (num_valores_conec > 0)
                        Datos1.ConsEspAvXp = DatosHorIndicesBrutoConsE.Sum(DatosControles => (DatosControles.media_conec*DatosControles.num_valores_conec)) / num_valores_conec;
                    else
                        Datos1.ConsEspAvXp = 0;
                    int num_valores_Noconec = DatosHorIndicesBrutoConsE.Sum(DatosControles => (DatosControles.num_valores));
                    if (num_valores_Noconec > 0)
                        Datos1.ConsEspNoAvXp = DatosHorIndicesBrutoConsE.Sum(DatosControles => (DatosControles.media)) / num_valores_Noconec;
                    else
                        Datos1.ConsEspNoAvXp = 0;
                }
                else
                {
                    Datos1.ConsEspAvXp = 0;
                    Datos1.ConsEspNoAvXp = 0;
                }
            }
            else
            {
                Datos1.ConsEspAvXp = 0;
                Datos1.ConsEspNoAvXp = 0;
            }
            Datos1.sConsEspAvXp = DatosAnimación.ConvTexto(Datos1.ConsEspAvXp, Visor7_ConsE.FormatoValor);
            Datos1.sConsEspNoAvXp = DatosAnimación.ConvTexto(Datos1.ConsEspNoAvXp, Visor11_ConsESin.FormatoValor);


            if (DatosHorIndicesBrutoFRec != null)
            {
                int num_datos = 0;
                num_datos = DatosHorIndicesBrutoFRec.Count;
                if (num_datos > 0)
                {
                    int num_valores_conec = DatosHorIndicesBrutoFRec.Sum(DatosControles => (DatosControles.num_valores_conec));
                    if (num_valores_conec > 0)
                        Datos1.FRecAvXp = DatosHorIndicesBrutoFRec.Sum(DatosControles => (DatosControles.media_conec * DatosControles.num_valores_conec)) / num_valores_conec;
                    else
                        Datos1.FRecAvXp = 0;
                    int num_valores_Noconec = DatosHorIndicesBrutoFRec.Sum(DatosControles => (DatosControles.num_valores));
                    if (num_valores_Noconec > 0)
                        Datos1.FRecNoAvXp = DatosHorIndicesBrutoFRec.Sum(DatosControles => (DatosControles.media)) / num_valores_Noconec;
                    else
                        Datos1.FRecNoAvXp = 0;
                }
                else
                {
                    Datos1.FRecAvXp = 0;
                    Datos1.FRecNoAvXp = 0;
                }
            }
            else
            {
                Datos1.FRecAvXp = 0;
                Datos1.FRecNoAvXp = 0;
            }
            Datos1.sFRecAvXp = DatosAnimación.ConvTexto(Datos1.FRecAvXp, Visor8_Recirc.FormatoValor);
            Datos1.sFRecNoAvXp = DatosAnimación.ConvTexto(Datos1.FRecNoAvXp, Visor12_RecircSin.FormatoValor);
        }

        private void DgTipos_LostFocus(object sender, RoutedEventArgs e)
        {

            Setter TipoNegrita = new Setter(TextBlock.FontWeightProperty, FontWeights.Bold, null);

            DataGridRow row = (DataGridRow)DgTipos.ItemContainerGenerator.ContainerFromIndex(DgTipos.SelectedIndex);
            Style newStyle = new Style(row.GetType());

            newStyle.Setters.Add(TipoNegrita);

            row.Style = newStyle;


        }
        private void IniciarAnimaciónVisores()
        {
     
            Visor1_Utilización.IniciarAnimacion();
            Visor2_Efic.IniciarAnimacion();
            VisorA_HorasAvXp.IniciarAnimacion();
            VisorB_HorasMol.IniciarAnimacion();
            VisorC_HorasAlim.IniciarAnimacion();
            VisorD_MuetrasTot.IniciarAnimacion();
            VisorE_MuetrasVal.IniciarAnimacion();   
            Visor3_Product.IniciarAnimacion();
            Visor4_Calidad.IniciarAnimacion();
            
            Visor5_Alim.IniciarAnimacion();
            Visor9_AlimSin.IniciarAnimacion();

            Visor7_ConsE.IniciarAnimacion();
            Visor11_ConsESin.IniciarAnimacion();

            Visor8_Recirc.IniciarAnimacion();
            Visor12_RecircSin.IniciarAnimacion();
        }


        //bool ChartOEE_Actualizado = false;
        //bool ChartHoras_Actualizado = false;
        //bool ChartProd_Actualizado = false;
        //bool ChartCalidad_Actualizado = false;

        //bool ChartOEE_SeriesCargadas = false;
        //bool ChartHoras_SeriesCargadas = false;
        //bool ChartProd_SeriesCargadas = false;
        //bool ChartCalidad_SeriesCargadas = false;

        //private void ChartOEE_UpdaterTick(object sender)
        //{

        //    //if (Inicializado&&!ChartOEE_Actualizado&& ChartOEE_SeriesCargadas)
        //    //{
        //    //    ChartOEE_Actualizado = true;
        //    //    //Thread.Sleep(10);

        //    //}
        //}
        private void ChartHorasAvXprt_UpdaterTick(object sender)
        {
            if (Inicializado)
            {
                PonerValoresVisores();
                IniciarAnimaciónVisores();
            }
            //if (Inicializado && !ChartHoras_Actualizado && ChartHoras_SeriesCargadas)
            //{
            //    ChartHoras_Actualizado = true;
            //    //Thread.Sleep(200);
            //    PonerValoresVisores();
            //    IniciarAnimaciónVisores();
            //}
            //if (Inicializado&&!ChartHoras_Actualizado&& ChartHoras_SeriesCargadas)
            //{
            //    //Thread.Sleep(100);
            //    ChartHoras_Actualizado = true;
            //    //Eventos.RegLinea(ArchivoRegEventos, "Tick", Param.DepuracionVerMensajes);
            //    CargarSeriesEficiencia();
            //}
        }

        private void BotSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BotDatos_Click(object sender, RoutedEventArgs e)
        {
            VentVistaDatos ventVistaDatos = new VentVistaDatos();


            ventVistaDatos.Dg1.ItemsSource = DatosHorIndicesEficiencia;
            ventVistaDatos.Label1.Content = "Índice de eficiencia";

            ResumirDatosHorIndices(DatosHorIndicesBrutoUtil, out DatosHorIndicesUtil, NumColumnasChart);
            ventVistaDatos.Dg2.ItemsSource = DatosHorIndicesUtil;
            ventVistaDatos.Label2.Content = "Índice de producción";

            ventVistaDatos.Dg3.ItemsSource = DatosHorIndicesRdtoProd;
            ventVistaDatos.Label3.Content = "Índice de rendimiento de producción";

            ventVistaDatos.Dg4.ItemsSource = DatosHorIndicesCalidad;
            ventVistaDatos.Label4.Content = "Índices de calidad";

            ventVistaDatos.ShowDialog();

        }
        
        // private void ChartProd_UpdaterTick(object sender)
        // {
        //     if (Inicializado && !ChartProd_Actualizado&& ChartProd_SeriesCargadas)
        //     {
        //         //Thread.Sleep(100);
        //         ChartProd_Actualizado = true;
        //         CargarSeriesUtiliz(); 
        //     }
        // }
        //private void ChartCalidad_UpdaterTick(object sender)
        // {
        //     if (Inicializado && !ChartCalidad_Actualizado&& ChartCalidad_SeriesCargadas)
        //     {

        //         ChartCalidad_Actualizado = true;
        //         CargarSeriesProduc(); 
        //         //Thread.Sleep(200);
        //     }
        // }


    }
}
