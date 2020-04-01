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
    public partial class UserCtrlVisorPeq : UserControl
    {
        public UserCtrlVisorPeq()
        {
            InitializeComponent();
            DataContext = this;
            //txtFormato = "###";// FormatoValor;
        }
        string txtFormato; //Formato de visualización del display
        //Declaraciones para animación de color de texto
        ColorAnimation AmimacionColorTexto1, AmimacionColorTexto2;
        DoubleAnimation AmimacionTamañoTexto1, AmimacionTamañoTexto2;
        Storyboard StoryBoardAnimacionTexto;
        Duration duracion = new Duration(TimeSpan.FromMilliseconds(500));

        //Propiedades para enlazar (binding) cuando se instancia en ventanas
        public string Valor { get; set; }
        public string FormatoValor
        {
            get { return txtFormato; }
            set { txtFormato = value; }
        }
        public double ValorFontSize { get; set; }
        public string Unids { get; set; }
        public string Magnitud { get; set; }

        string ValorInicio; //Valor de la variable al inicio de la animación
        double ValorFontSizeInicio;

        private void UserCtrlVisorName_Initialized(object sender, EventArgs e)
        {


            // Inicializaciones para animación de color de texto
            AmimacionColorTexto1 = new ColorAnimation();
            AmimacionTamañoTexto1 = new DoubleAnimation();

            AmimacionColorTexto2 = new ColorAnimation();
            AmimacionTamañoTexto2 = new DoubleAnimation();
            StoryBoardAnimacionTexto = new Storyboard();


            StoryBoardAnimacionTexto.Duration = duracion;
            StoryBoardAnimacionTexto.AccelerationRatio = 0.9;

            StoryBoardAnimacionTexto.Children.Add(AmimacionColorTexto1);
            StoryBoardAnimacionTexto.Children.Add(AmimacionTamañoTexto1);

            StoryBoardAnimacionTexto.Children.Add(AmimacionColorTexto2);
            StoryBoardAnimacionTexto.Children.Add(AmimacionTamañoTexto2);
            Storyboard.SetTarget(AmimacionColorTexto1, TextBlockCantidad);
            Storyboard.SetTargetProperty(AmimacionColorTexto1, new PropertyPath("Foreground.Color"));
            Storyboard.SetTarget(AmimacionTamañoTexto1, TextBlockCantidad);
            Storyboard.SetTargetProperty(AmimacionTamañoTexto1, new PropertyPath(TextBlock.HeightProperty));


            Storyboard.SetTarget(AmimacionColorTexto2, TextBlockUnid);
            Storyboard.SetTargetProperty(AmimacionColorTexto2, new PropertyPath("Foreground.Color"));
            Storyboard.SetTarget(AmimacionTamañoTexto2, TextBlockUnid);
            Storyboard.SetTargetProperty(AmimacionTamañoTexto2, new PropertyPath(TextBlock.HeightProperty));

            AmimacionTamañoTexto1.From = TextBlockCantidad.MaxHeight / 4;
            AmimacionTamañoTexto1.To = TextBlockCantidad.MaxHeight;
            AmimacionTamañoTexto2.From = TextBlockCantidad.MaxHeight / 4;
            AmimacionTamañoTexto2.To = TextBlockCantidad.MaxHeight;


            AmimacionColorTexto1.From = Colors.LightGray;
            AmimacionColorTexto1.To = Colors.Black;

            AmimacionColorTexto2.From = Colors.LightGray;
            AmimacionColorTexto2.To = Colors.Black;
        }
        public void IniciarAnimacion()
        {
            AmimacionColorTexto2.Duration = duracion;
            AmimacionColorTexto1.Duration = duracion;
            AmimacionTamañoTexto1.Duration = duracion;
            AmimacionTamañoTexto2.Duration = duracion;
            StoryBoardAnimacionTexto.CurrentTimeInvalidated += StoryBoardAnimacionTexto_CurrentTimeInvalidated;
            StoryBoardAnimacionTexto.Completed += StoryBoardAnimacionTexto_Completed;

            ValorInicio = TextBlockCantidad.Text;
            ValorFontSizeInicio = ValorFontSize;

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



        //public string Unids
        //{
        //    get
        //    {
        //        return (string)GetValue(UnidsProperty);
        //    }

        //    set
        //    {
        //        SetValue(UnidsProperty, Unids);
        //    }
        //}
        //public string Magnitud
        //{
        //    get
        //    {
        //        return (string)GetValue(MagnitudProperty);
        //    }

        //    set
        //    {
        //        SetValue(MagnitudProperty, Unids);
        //    }
        //}


        //public string Valor
        //{
        //    get
        //    {
        //        return (string)GetValue(ValorProperty);
        //    }

        //    set
        //    {
        //        SetValue(ValorProperty, Valor);
        //    }
        //}


        //public static readonly DependencyProperty ValorProperty =
        //      DependencyProperty.Register("Valor", typeof(string), typeof(UserCtrlVisor),
        //      new FrameworkPropertyMetadata("00.0", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ValorChanged)));


        //public static readonly DependencyProperty UnidsProperty =
        //    DependencyProperty.Register("Unids", typeof(string), typeof(UserCtrlVisor), new FrameworkPropertyMetadata("-", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ValorChanged)));

        //public static readonly DependencyProperty MagnitudProperty =
        //    DependencyProperty.Register("Magnitud", typeof(string), typeof(UserCtrlVisor),new FrameworkPropertyMetadata("-", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ValorChanged)));

        //private static void ValorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ((UserCtrlVisor)d).Valor = ((UserCtrlVisor)d).Valor;
        //}

    }
}
