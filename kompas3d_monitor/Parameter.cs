using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kompas3d_monitor
{
    internal class Parameter
    {
        private double _value;
        private double _minValue;
        private double _maxValue;

        public double Value
        {
            get => _value;
            set
            {
                _value = value;
            }
        }

        public double MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
            }
        }
        
        public double MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
            }
        }

        public Parameter(double minValue, double maxValue, double initialValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Value = initialValue; // Проверка значения через свойство
            Validate();
        }

        public void Validate()
        {
            if (Value < _minValue || Value > _maxValue)
                throw new FormatException($"Значение должно быть между {_minValue} и {_maxValue}");
        }
    }
}
