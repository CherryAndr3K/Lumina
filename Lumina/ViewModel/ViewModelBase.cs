using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            //? es el operadopr de invocacion condicional
            PropertyChanged?.Invoke(this,
               new PropertyChangedEventArgs(propertyName));
        }


    }
}
