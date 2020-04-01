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
public class DatosAnimación : INotifyPropertyChanged
{
    public double UtilizaciónMedia = 0;
    public double HorasMolTot = 0;
    public double HorasAvXTot = 0;
    public double HorasAlimObj = 0;
    public double Productividad = 0;
    public double Producción = 0;
    public double ProducciónNoAvXp = 0;
    public double ConsEspAvXp = 0;
    public double ConsEspNoAvXp = 0;
    public double FRecAvXp = 0;
    public double FRecNoAvXp = 0;
    public float Calidad = 0;
    public double Eficiencia = 0;
    public int Muestras = 0;
    public int MuestrasOK = 0;

    private string priv_sUtilizaciónMedia;
    private string priv_sProductividad;
    private string priv_sHorasMolTot;
    private string priv_sHorasAvXTot;
    private string priv_sHorasAlimObj;
    private string priv_sProducción;
    private string priv_sProducciónNoAvXp;
    private string priv_sMuestras;
    private string priv_sMuestrasOK;
    private string priv_sCalidad;
    private string priv_sEficiencia;
    public string priv_sConsEspAvXp;
    public string priv_sConsEspNoAvXp;
    public string priv_sFRecAvXp;
    public string priv_sFRecNoAvXp;
    public string sUtilizaciónMedia
    {
        get { return priv_sUtilizaciónMedia; }
        set
        {
            priv_sUtilizaciónMedia = value;
            OnPropertyChanged("sUtilizaciónMedia");
        }
    }
    public string sHorasMolTot
    {
        get { return priv_sHorasMolTot; }
        set
        {
            priv_sHorasMolTot = value;
            OnPropertyChanged("sHorasMolTot");
        }
    }
    public string sHorasAvXTot
    {
        get { return priv_sHorasAvXTot; }
        set
        {
            priv_sHorasAvXTot = value;
            OnPropertyChanged("sHorasAvXTot");
        }
    }
    public string sHorasAlimObj
    {
        get { return priv_sHorasAlimObj; }
        set
        {
            priv_sHorasAlimObj = value;
            OnPropertyChanged("sHorasAlimObj");
        }
    }
    public string sProductividad
    {
        get { return priv_sProductividad; }
        set
        {
            priv_sProductividad = value;
            OnPropertyChanged("sProductividad");
        }
    }
    public string sProducción
    {
        get { return priv_sProducción; }
        set
        {
            priv_sProducción = value;
            OnPropertyChanged("sProducción");
        }
    }
    public string sProducciónNoAvXp
    {
        get { return priv_sProducciónNoAvXp; }
        set
        {
            priv_sProducciónNoAvXp = value;
            OnPropertyChanged("sProducciónNoAvXp");
        }
    }

    public string sMuestras
    {
        get { return priv_sMuestras; }
        set
        {
            priv_sMuestras = value;
            OnPropertyChanged("sMuestras");
        }
    }
    public string sMuestrasOK
    {
        get { return priv_sMuestrasOK; }
        set
        {
            priv_sMuestrasOK = value;
            OnPropertyChanged("sMuestrasOK");
        }
    }
    public string sCalidad
    {
        get { return priv_sCalidad; }
        set
        {
            priv_sCalidad = value;
            OnPropertyChanged("sCalidad");
        }
    }
    public string sEficiencia
    {
        get { return priv_sEficiencia; }
        set
        {
            priv_sEficiencia = value;
            OnPropertyChanged("sEficiencia");
        }
    }
    public string sConsEspAvXp
    {
        get { return priv_sConsEspAvXp; }
        set
        {
            priv_sConsEspAvXp = value;
            OnPropertyChanged("sConsEspAvXp");
        }
    }
    public string sConsEspNoAvXp
    {
        get { return priv_sConsEspNoAvXp; }
        set
        {
            priv_sConsEspNoAvXp = value;
            OnPropertyChanged("sConsEspNoAvXp");
        }
    }

    public string sFRecAvXp
    {
        get { return priv_sFRecAvXp; }
        set
        {
            priv_sFRecAvXp = value;
            OnPropertyChanged("sFRecAvXp");
        }
    }

    public string sFRecNoAvXp
    {
        get { return priv_sFRecNoAvXp; }
        set
        {
            priv_sFRecNoAvXp = value;
            OnPropertyChanged("sFRecNoAvXp");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public static string ConvTexto(double Cantidad, string formato = "0.00")
    {
        string retVal = Cantidad.ToString(formato); ;
        if (retVal == "")
        {
            return "0";
        }
        else if ((Cantidad == 1.00) && (formato == "0.00"))
            return "1.00";
        else
            return retVal;
    }

    public void ResetValores()
    {
        sUtilizaciónMedia = "0.00";
        sHorasMolTot = "000";
        sHorasAvXTot = "000";
        sHorasAlimObj = "000";
        sProductividad = "00.0";
        sProducción = "000";
    }
    public void VaciarValores()
    {
        sUtilizaciónMedia = "";
        sHorasMolTot = "";
        sHorasAvXTot = "";
        sHorasAlimObj = "";
        sProductividad = "";
        sProducción = "";
    }

}