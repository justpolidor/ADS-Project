using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempProject.BaseClasses;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml;

namespace TempProject.Classes
{
    //Classe base che verrà estesa da tutti i sensori
    abstract class I2CBaseClass<T> : I2CBaseInterface where T : EventArgs
    {
        public delegate void SetSensorValue(object sender, T e);

        public abstract void Timer_Tick(object sender, object e);

        public abstract void InitI2CSensor();
      
    }
}
