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
using System.Windows.Media.Animation;

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

using avanXpert_Info.Código;
using avanXpert_Info.Código.Clases;


using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;


namespace avanXpert_Info.Ventanas
{
    /// <summary>
    /// Lógica de interacción para VentDatos1.xaml
    /// </summary>
    public partial class VentDatos1 : Window
    {
        System.IO.StreamWriter ArchivoRegEventos;
        ClassRegistro Eventos;
        MainWindow mainWindow;
        MySqlConnection ConexDB;
        Parametros Param;
        int idMolino = 0;
        ObservableCollection<ClassDatosControles> datosControles;
        ObservableCollection<ClassControles> Controles;
        public Func<double, string> Formatter { get; set; }
        public SeriesCollection SeriesUtilizacion { get; set; }

        public VentDatos1(MainWindow VentMain, System.IO.StreamWriter eventosMain, ClassRegistro RegEventosMain, MySqlConnection ConexDbMain, Parametros paramMain, int idMolinoMain = 0)
        {
            mainWindow = VentMain;
            ArchivoRegEventos = eventosMain;
            Eventos = RegEventosMain;
            ConexDB = ConexDbMain;
            Param = paramMain;
            idMolino = idMolinoMain;

            //SeriesCollection DatosSeries = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Values = new ChartValues<double> { 3, 5, 7, 4 }
            //    },
            //    new ColumnSeries
            //    {
            //        Values = new ChartValues<decimal> { 5, 6, 2, 7 }
            //    }
            //};

            InitializeComponent();
            Controles = new ObservableCollection<ClassControles>();
            ClassControles.LeerControles(ArchivoRegEventos, Eventos, ConexDB, Param, out Controles, idMolino);

            ComboBoxControl.ItemsSource = Controles;
            ComboBoxControl.DisplayMemberPath = "nombre";
            ComboBoxControl.SelectedValuePath = "id";
            ComboBoxControl.SelectedIndex = 0;
        }


        //Declaraciones para animación de color de texto
        ColorAnimation AmimacionColorTexto1, AmimacionColorTexto2;
        DoubleAnimation AmimacionTamañoTexto1, AmimacionTamañoTexto2;
        Storyboard StoryBoardAnimacionTexto;



        private void Window_Initialized(object sender, EventArgs e)
        {
            FechaHasta.SelectedDate = DateTime.Today;
            TimeSpan difT = new TimeSpan(1, 0, 0, 0);
            DateTime previo = DateTime.Today;
            previo = previo.Subtract(difT);
            FechaDesde.SelectedDate = previo;




            // Inicializaciones para animación de color de texto
            AmimacionColorTexto1 = new ColorAnimation();
            AmimacionTamañoTexto1 = new DoubleAnimation();

            AmimacionColorTexto2 = new ColorAnimation();
            AmimacionTamañoTexto2 = new DoubleAnimation();
            StoryBoardAnimacionTexto = new Storyboard();
            Duration duracion = new Duration(TimeSpan.FromMilliseconds(250));

            StoryBoardAnimacionTexto.Duration = duracion;

            StoryBoardAnimacionTexto.Children.Add(AmimacionColorTexto1);
            StoryBoardAnimacionTexto.Children.Add(AmimacionTamañoTexto1);

            StoryBoardAnimacionTexto.Children.Add(AmimacionColorTexto2);
            StoryBoardAnimacionTexto.Children.Add(AmimacionTamañoTexto2);
            Storyboard.SetTarget(AmimacionColorTexto1, TextBlockDisponibilidad);
            Storyboard.SetTargetProperty(AmimacionColorTexto1, new PropertyPath("Foreground.Color"));
            Storyboard.SetTarget(AmimacionTamañoTexto1, TextBlockDisponibilidad);
            Storyboard.SetTargetProperty(AmimacionTamañoTexto1, new PropertyPath(TextBlock.HeightProperty));


            Storyboard.SetTarget(AmimacionColorTexto2, TextBlockUtilización);
            Storyboard.SetTargetProperty(AmimacionColorTexto2, new PropertyPath("Foreground.Color"));
            Storyboard.SetTarget(AmimacionTamañoTexto2, TextBlockUtilización);
            Storyboard.SetTargetProperty(AmimacionTamañoTexto2, new PropertyPath(TextBlock.HeightProperty));

            AmimacionColorTexto2.Duration = duracion;
            AmimacionColorTexto1.Duration = duracion;

            AmimacionTamañoTexto1.Duration = duracion;
            AmimacionTamañoTexto2.Duration = duracion;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ActualizarVista();
        }



        double DisponibMedia = 0;
        double UtilizaciónMedia = 0;
        private void ActualizarVista()
        {
            DateTime Desde = FechaDesde.SelectedDate.Value;
            string sDesde = Desde.Year + "-" + Desde.Month + "-" + Desde.Day;

            DateTime Hasta = FechaHasta.SelectedDate.Value;
            string sHasta = Hasta.Year + "-" + Hasta.Month + "-" + Hasta.Day;
            ClassDatosControles.LeerDatosControles(ArchivoRegEventos, Eventos, ConexDB, Param, out datosControles, sDesde, sHasta, idMolino, int.Parse(ComboBoxControl.SelectedValue.ToString()));
            DgDatos1.ItemsSource = datosControles;


            var DatosDisponibilidad = Mappers.Xy<ClassDatosControles>()
                 .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                 .Y(ModeloDia => ModeloDia.factor_disponibilidad);


            var DatosUtilizacion = Mappers.Xy<ClassDatosControles>()
                 .X(ModeloDia => (double)ModeloDia.fecha_hora.Ticks / TimeSpan.FromHours(1).Ticks)
                 .Y(ModeloDia => ModeloDia.factor_utilizacion);

            //Notice you can also configure this type globally, so you don't need to configure every
            //SeriesCollection instance using the type.
            //more info at http://lvcharts.net/App/Index#/examples/v1/wpf/Types%20and%20Configuration
            if (SeriesUtilizacion != null)
            {
                SeriesUtilizacion = null;
                DataContext = null;

            }
            SeriesUtilizacion = new SeriesCollection();

            LineSeries serieUtilizacion = new LineSeries(DatosUtilizacion)
            {
                Values = new ChartValues<ClassDatosControles>(datosControles),
                Fill = Brushes.Transparent,
                Title = "Utilización",

                LineSmoothness = 0.6,
                StrokeThickness = 5,

                DataLabels = true,
                FontSize = 10,
                Foreground = Brushes.DarkCyan,

                Stroke = Brushes.DarkCyan,
                PointGeometrySize = 5
            };
            LineSeries serieDisponibilidad = new LineSeries(DatosDisponibilidad)
            {
                Values = new ChartValues<ClassDatosControles>(datosControles),
                Fill = Brushes.Transparent,
                Title = "Disponibilidad",

                LineSmoothness = 0.6,
                StrokeThickness = 5,

                DataLabels = true,
                FontSize = 10,
                Foreground = Brushes.DarkGreen,

                Stroke = Brushes.DarkOliveGreen,
                PointGeometrySize = 5
            };


            SeriesUtilizacion.Add(serieUtilizacion);
            SeriesUtilizacion.Add(serieDisponibilidad);            
            Formatter = value => new System.DateTime((long)(Math.Max(0, value) * TimeSpan.FromHours(1).Ticks)).ToString("dd/MM (HH") + "h)";
            DataContext = this;

            if (datosControles.Count > 0)
            {
                DisponibMedia = datosControles.Average(DatosControles => DatosControles.factor_disponibilidad);
                TextBlockDisponibilidad.Text = DisponibMedia.ToString("##.#") + "%";

                UtilizaciónMedia = datosControles.Average(DatosControles => DatosControles.factor_utilizacion);
                TextBlockUtilización.Text = UtilizaciónMedia.ToString("##.#") + "%";
                AmimacionColorTexto1.From = Colors.LightGray;
                AmimacionColorTexto1.To = Colors.Black;

                AmimacionColorTexto2.From = Colors.LightGray;
                AmimacionColorTexto2.To = Colors.Black;

                StoryBoardAnimacionTexto.CurrentTimeInvalidated += StoryBoardAnimacionTexto_CurrentTimeInvalidated;
                StoryBoardAnimacionTexto.Completed += StoryBoardAnimacionTexto_Completed;
                //StoryBoardAnimacionTexto.CurrentTimeInvalidated += (s, e) =>
                //{
                //    TextBlockDisponibilidad.Text = (TextBlockDisponibilidad.Height / AmimacionTamañoTexto1.To.Value * DisponibMedia).ToString("##.#") + "%";
                //    TextBlockUtilización.Text = (TextBlockUtilización.Height / AmimacionTamañoTexto2.To.Value * UtilizaciónMedia).ToString("##.#") + "%";
                //};

                // StoryBoardAnimacionTexto.CurrentTimeInvalidated += StoryBoardAnimacionTexto_CurrentTimeInvalidated;

                StoryBoardAnimacionTexto.Begin();

            }
            else
            {
                DisponibMedia = 0;
                UtilizaciónMedia = 0;
                TextBlockDisponibilidad.Text = "--.-" + "%";
                TextBlockUtilización.Text = "--.-" + "%";
                TextBlockDisponibilidad.Foreground = Brushes.Gray;
                TextBlockUtilización.Foreground = Brushes.Gray;
            }


            AmimacionTamañoTexto1.From = TextBlockDisponibilidad.MaxHeight / 4;
            AmimacionTamañoTexto1.To = TextBlockDisponibilidad.MaxHeight;
            AmimacionTamañoTexto2.From = TextBlockUtilización.MaxHeight / 4;
            AmimacionTamañoTexto2.To = TextBlockUtilización.MaxHeight;
            


            //StoryBoardAnimacionTexto.Duration = new Duration(TimeSpan.Parse("0:0:3"));

        }

        private void DgDatos1_AutoGeneratedColumns(object sender, EventArgs e)
        {

            DataGridTextColumn column = DgDatos1.Columns[0] as DataGridTextColumn;
            Binding binding = column.Binding as Binding;
            binding.StringFormat = "dd/MM/yyyy HH:mm:ss";
            DgDatos1.Columns[0].Header = "Fecha";
            DgDatos1.Columns[1].Visibility = Visibility.Hidden; //Id_control
            DgDatos1.Columns[2].Header = "Min. marcha";
            DgDatos1.Columns[3].Header = "Min. conec";
            DgDatos1.Columns[4].Header = "Min. disp";
            DgDatos1.Columns[5].Header = "Min. día";
            DgDatos1.Columns[6].Header = "Utiliz(%)";
            DgDatos1.Columns[7].Header = "Disponib(%)";

        }

        private void StoryBoardAnimacionTexto_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            TextBlockDisponibilidad.Text = (TextBlockDisponibilidad.Height / AmimacionTamañoTexto1.To.Value * DisponibMedia).ToString("##.#") + "%";
            TextBlockUtilización.Text = (TextBlockUtilización.Height / AmimacionTamañoTexto2.To.Value * UtilizaciónMedia).ToString("##.#") + "%";
        }

        private void StoryBoardAnimacionTexto_Completed(object sender, EventArgs e)
        {
            TextBlockDisponibilidad.Text = (DisponibMedia).ToString("##.#") + "%";
            TextBlockUtilización.Text = (UtilizaciónMedia).ToString("##.#") + "%";
            StoryBoardAnimacionTexto.Completed -= StoryBoardAnimacionTexto_Completed;
            StoryBoardAnimacionTexto.CurrentTimeInvalidated -= StoryBoardAnimacionTexto_CurrentTimeInvalidated;
        }
    


    }
}


