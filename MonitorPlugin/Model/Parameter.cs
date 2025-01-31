using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorModel
{
    /// <summary>
    /// Представляет параметр с минимальным и максимальным значением, 
    /// а также с текущим значением, проверяющим корректность в заданном диапазоне.
    /// </summary>
    public class Parameter
    {
        //TODO:XML
        /// <summary>
        /// Текущее значение параметра.
        /// </summary>
        private double _value;

        /// <summary>
        /// Минимально допустимое значение параметра.
        /// </summary>
        private double _minValue;

        /// <summary>
        /// Максимально допустимое значение параметра.
        /// </summary>
        private double _maxValue;

        /// <summary>
        /// Текущее значение параметра.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается, если значение выходит за пределы <see cref="MinValue"/> и <see cref="MaxValue"/>.
        /// </exception>
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                Validate();
            }
        }

        /// <summary>
        /// Минимальное значение параметра.
        /// </summary>
        public double MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                Validate();
            }
        }

        /// <summary>
        /// Максимальное значение параметра.
        /// </summary>
        public double MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                Validate();
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Parameter"/>.
        /// </summary>
        /// <param name="minValue">Минимальное значение параметра.</param>
        /// <param name="maxValue">Максимальное значение параметра.</param>
        /// <param name="initialValue">Начальное значение параметра.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается, если значение выходит за пределы <paramref name="minValue"/> и <paramref name="maxValue"/>.
        /// </exception>
        public Parameter(double minValue, double maxValue, double initialValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            Value = initialValue; 
        }

        /// <summary>
        /// Проверяет, что значение находится в пределах диапазона от <see cref="MinValue"/> до <see cref="MaxValue"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается, если значение выходит за пределы <see cref="MinValue"/> и <see cref="MaxValue"/>.
        /// </exception>
        private void Validate()
        {
            if (Value < _minValue || Value > _maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(Value),
                    $"Значение должно быть между {_minValue} и {_maxValue}");
            }
        }

    }
}
