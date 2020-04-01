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
using System.Windows.Media.Animation;

namespace avanXpert_Info
{
    /// <summary>
    /// Lógica de interacción para UserCtrlVisor.xaml
    /// </summary>
    public partial class UserCtrlVisorGsUnid : UserControl
    {
        public UserCtrlVisorGsUnid()
        {
            InitializeComponent();
            DataContext = this;
            //txtFormato = FormatoValor;
        }
        string txtFormato; //Formato de visualización del display
        //Declaraciones para animación de color de texto
        ColorAnimation AmimacionColorTexto1;
        DoubleAnimation AmimacionTamañoTexto1;
        Storyboard StoryBoardAnimacionTexto;
        Duration duracion = new Duration(TimeSpan.FromMilliseconds(500));

        //Propiedades para enlazar (binding) cuando se instancia en ventanas
        public string FormatoValor
        {

            get { return txtFormato; }
            set { txtFormato = value; }
        }

        public string Valor { get; set; }
        public double ValorFontSize { get; set; }
        public string Magnitud { get; set; }
        public Brush Fondo { get; set; }

        string ValorInicio; //Valor de la variable al inicio de la animación

        double ValorFontSizeInicio;



        private void UserCtrlVisorName_Initialized(object sender, EventArgs e)
        {

            // Inicializaciones para animación de color de texto
            AmimacionColorTexto1 = new ColorAnimation();
            AmimacionTamañoTexto1 = new DoubleAnimation();


            StoryBoardAnimacionTexto = new Storyboard();

            StoryBoardAnimacionTexto.Duration = duracion;
            StoryBoardAnimacionTexto.AccelerationRatio = 0.9;

            StoryBoardAnimacionTexto.Children.Add(AmimacionColorTexto1);
            StoryBoardAnimacionTexto.Children.Add(AmimacionTamañoTexto1);



            Storyboard.SetTarget(AmimacionColorTexto1, TextBlockCantidad);
            Storyboard.SetTargetProperty(AmimacionColorTexto1, new PropertyPath("Foreground.Color"));
            Storyboard.SetTarget(AmimacionTamañoTexto1, TextBlockCantidad);
            Storyboard.SetTargetProperty(AmimacionTamañoTexto1, new PropertyPath(TextBlock.HeightProperty));



            AmimacionTamañoTexto1.From = TextBlockCantidad.MaxHeight / 4;
            AmimacionTamañoTexto1.To = TextBlockCantidad.MaxHeight;



            AmimacionColorTexto1.From = Colors.LightGray;
            AmimacionColorTexto1.To = Colors.Black;

        }
        public void IniciarAnimacion()
        {

            AmimacionColorTexto1.Duration = duracion;
            AmimacionTamañoTexto1.Duration = duracion;

            StoryBoardAnimacionTexto.CurrentTimeInvalidated += StoryBoardAnimacionTexto_CurrentTimeInvalidated;
            StoryBoardAnimacionTexto.Completed += StoryBoardAnimacionTexto_Completed;

            ValorInicio = TextBlockCantidad.Text;
            ValorFontSizeInicio = ValorFontSize;

            TextBlockCantidad.Background = Fondo;

            StoryBoardAnimacionTexto.Begin();
        }
        private void StoryBoardAnimacionTexto_CurrentTimeInvalidated(object sender, EventArgs e)
        {

            TextBlockCantidad.FontSize = ValorFontSizeInicio;
            double dValue = 0;
            bool tryParse = double.TryParse(ValorInicio, out dValue);
            if (tryParse)
            {
                TextBlockCantidad.Text = (TextBlockCantidad.Height / AmimacionTamañoTexto1.To.Value * dValue).ToString(txtFormato);
            }

        }
        private void StoryBoardAnimacionTexto_Completed(object sender, EventArgs e)
        {
            TextBlockCantidad.Text = (ValorInicio);

            StoryBoardAnimacionTexto.Completed -= StoryBoardAnimacionTexto_Completed;
            StoryBoardAnimacionTexto.CurrentTimeInvalidated -= StoryBoardAnimacionTexto_CurrentTimeInvalidated;
        }


    }
}
