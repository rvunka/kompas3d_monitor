using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorModel
{
    //TODO:XML
    public class Parameter
    {
        private double _value;
        private double _minValue;
        private double _maxValue;

        public double Value
        {
            get => _value;
            //TODO: validation?
            set
            {
                _value = value;
                Validate();
            }
        }

        public double MinValue
        {
            get => _minValue;
            //TODO: validation?
            set
            {
                _minValue = value;
                Validate();
            }
        }

        public double MaxValue
        {
            get => _maxValue;
            //TODO: validation?
            set
            {
                _maxValue = value;
                Validate();
            }
        }

        public Parameter(double minValue, double maxValue, double initialValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            Value = initialValue; // Проверка значения через свойство
        }

        private void Validate()
        {
            //TODO: RSDN
            if (Value < _minValue || Value > _maxValue)
                throw new FormatException($"Значение должно быть между {_minValue} и {_maxValue}");
        }
    }
}
