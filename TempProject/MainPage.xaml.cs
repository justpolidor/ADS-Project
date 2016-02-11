using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using System.Diagnostics;

// Il modello di elemento per la pagina vuota è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace TempProject
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private TMP75C tmp75c;

        // TODO : creare una lista di sensori così da non dichiararci istanze come sopra (ma non qui...)
        //List<I2CBaseInterface> sensors_list = new List<I2CBaseInterface>();

        public MainPage()
        {
            this.InitializeComponent();

            //Creo un'istanza del sensore tmp75c 
            tmp75c = new TMP75C();

            //Inizializzo il sensore 
            tmp75c.InitI2CSensor();

            //Mi registro all'evento e ogni volta che viene richiamato mi richiamo il metodo che aggiorna la view
            tmp75c.Changed += Tmp75c_Changed;  

        }

        //Imposto la view con la temperatura passata 
        private void Tmp75c_Changed(object sender, TMP75CEventArgs e)
        {
            TemperatureMessage.Text = e.Temperature.ToString();
        }

    }
}
