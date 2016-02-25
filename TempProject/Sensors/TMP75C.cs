using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempProject.Classes;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml;

/**
Creare una generalizzazione più elevata
**/
namespace TempProject
{
    //Classe dell'evento specifico generato per visualizzare il risultato della temperatura
    class TMP75CEventArgs : EventArgs
    {
        public double Temperature { get; private set; }

        public TMP75CEventArgs(double temperature)
        {
            this.Temperature = temperature;
        }
    }
    
    /**
        Classe che controlla il sensore Texas Instrument TMP75C 
        Per i reference al datasheet vedere : http://www.ti.com/lit/ds/symlink/tmp75c.pdf
    **/
    class TMP75C : I2CBaseClass<TMP75CEventArgs>
    {
        //canale di comunicazione che rappresenta il sensore che comunica tramite I2C
        private I2cDevice temperatureSensor;

        //Timer per effettuare il polling della temperatura
        private DispatcherTimer timer;

        //Evento che aggiorna il valore sulla view 
        public event SetSensorValue Changed;

        //Indirizzo del sensore di temperatura
        private const byte TMP75C_ADDRESS = 0x48;

        //Registro per leggere i dati della temperatura (è qui che il sensore salva i dati). Registro read-only
        private const byte TEMP_I2C_REGISTER = 0x00;

        //Metodo che inizializza il sensore
        public override async void InitI2CSensor()
        {
            try
            {
                //Tramite il canale di comunicazione , acquisisco una stringa per effettuare query sul sensore
                string i2cDeviceSelector = I2cDevice.GetDeviceSelector();

                //Lista che rappresenta i dispositivi trovati con il precedente comando
                IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);

                //Imposta i settaggi per la comunicazione con il sensore , specificando l'indirizzo dello slave
                var tmp75c_settings = new I2cConnectionSettings(TMP75C_ADDRESS);

                //Imposta la velocità di lettura 
                tmp75c_settings.BusSpeed = I2cBusSpeed.FastMode;

                //Crea il canale di comunicazione (asincrono) con il device i2c utilizzando le impostazioni specificate
                temperatureSensor = await I2cDevice.FromIdAsync(devices[0].Id, tmp75c_settings);

                //polling timer
                timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        //Metodo che imposta il polling della temperatura sul registro specificato ogni mills
        public override void Timer_Tick(object sender, object e)
        {
            try
            {
                //Rappresenta il registro dove leggere la temperatura (in esadecimale)
                var command = new byte[1];

                //Rappresenta un array di 12 bit, divisi in msb , xsb e lsb nel quale andranno scritti i valori della temperatura
                //ritornati dal sensore
                var temperatureData = new byte[2];

                //imposta il registro dove leggere la temperatura
                command[0] = TEMP_I2C_REGISTER;

                //Effettua l'operazione di lettura nel registro specificato in 'command' e li immagazzina nell'array temperatureData
                temperatureSensor.WriteRead(command, temperatureData);

                var msb = temperatureData[0];
                var lsb = temperatureData[1];

                //Conversione dei dati grezzi e shift dei bit come da manuale
                var rawTempReading = (msb << 8 | lsb) >> 4;

                //Algoritmo per la conversione da raw a celsius
                double rawTemperature = rawTempReading * 0.0625;

                //Approssimazione a due cifre decimali del risultato
                double temperature = Math.Round(rawTemperature, 2);

                //Auto richiamo l'evento così da visualizzare su schermo i valori letti
                if (Changed != null)
                {
                    //richiama il delegate e gli passo l'evento di tipo TMP75C con la temperatura letta
                    Changed(this, new TMP75CEventArgs(temperature));
                }

                Debug.WriteLine(rawTempReading);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
    }
}
