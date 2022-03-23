using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoScada.DesktopApplication.Models
{
    public class ValueWrapper<T> : ObservableObject
    {
        private T _value;
        public T Value
        {
            get => _value;
            set => OnPropertyChanged(ref _value, value);
        }

        public ValueWrapper(T value)
        {
            this.Value = value;
        }
    }
}
