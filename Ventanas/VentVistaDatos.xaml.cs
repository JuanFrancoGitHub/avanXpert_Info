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

namespace avanXpert_Info.Ventanas
{
    /// <summary>
    /// Lógica de interacción para VentVistaDatos.xaml
    /// </summary>
    public partial class VentVistaDatos : Window
    {
        public VentVistaDatos()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    double AlturaDg = Height / 4 - 50;
        //    Dg1.Height = AlturaDg;
        //    Dg2.Height = AlturaDg;
        //    Dg3.Height = AlturaDg;
        //    Dg4.Height = AlturaDg;
        //}
    }
}
