using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonitorModel;
using System;

namespace UnitTestProject
{
    /// <summary>
    /// Тестовый класс для проверки функциональности класса Parameter.
    /// </summary>
    [TestClass]
    public class ParameterTests
    {
        /// <summary>
        /// Проверяет, что конструктор корректно инициализирует объект с заданными начальными значениями.
        /// </summary>
        [TestMethod]
        public void Constructor_ShouldSetInitialValuesCorrectly()
        {
            double minValue = 0;
            double maxValue = 100;
            double initialValue = 50;

            var parameter = new Parameter(minValue, maxValue, initialValue);

            Assert.AreEqual(initialValue, parameter.Value);
            Assert.AreEqual(minValue, parameter.MinValue);
            Assert.AreEqual(maxValue, parameter.MaxValue);
        }

        /// <summary>
        /// Проверяет, что значение корректно устанавливается, если оно находится в допустимом диапазоне.
        /// </summary>
        [TestMethod]
        public void Value_SetWithinRange_ShouldSetValueSuccessfully()
        {
            var parameter = new Parameter(0, 100, 50);
            parameter.Value = 75;
            Assert.AreEqual(75, parameter.Value);
        }

        /// <summary>
        /// Проверяет, что при попытке установить значение ниже минимального возникает исключение ArgumentOutOfRangeException.
        /// </summary>
        [TestMethod]
        public void Value_SetBelowMinValue_ShouldThrowArgumentOutOfRangeException()
        {
            var parameter = new Parameter(0, 100, 50);

            var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                parameter.Value = -1
            );

            Assert.IsTrue(exception.Message.Contains("должно быть между"));
        }

        /// <summary>
        /// Проверяет, что при попытке установить значение выше максимального возникает исключение ArgumentOutOfRangeException.
        /// </summary>
        [TestMethod]
        public void Value_SetAboveMaxValue_ShouldThrowArgumentOutOfRangeException()
        {
            var parameter = new Parameter(0, 100, 50);

            var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                parameter.Value = 101
            );

            Assert.IsTrue(exception.Message.Contains("должно быть между"));
        }

        /// <summary>
        /// Проверяет, что минимальное значение может быть успешно обновлено, если оно ниже текущего значения параметра.
        /// </summary>
        [TestMethod]
        public void MinValue_SetBelowCurrentValue_ShouldUpdateMinValueSuccessfully()
        {
            var parameter = new Parameter(0, 100, 50);
            parameter.MinValue = -10;
            Assert.AreEqual(-10, parameter.MinValue);
        }

        /// <summary>
        /// Проверяет, что минимальное значение можно установить ниже текущего значения параметра.
        /// </summary>
        [TestMethod]
        public void MinValue_SetBelowValue_ShouldSucceed()
        {
            var parameter = new Parameter(0, 100, 50);
            parameter.MinValue = 25;

            Assert.AreEqual(25, parameter.MinValue);
        }

        /// <summary>
        /// Проверяет, что максимальное значение может быть успешно обновлено, если оно выше текущего значения параметра.
        /// </summary>
        [TestMethod]
        public void MaxValue_SetAboveValue_ShouldSucceed()
        {
            var parameter = new Parameter(0, 100, 50);
            parameter.MaxValue = 150;

            Assert.AreEqual(150, parameter.MaxValue);
        }

        /// <summary>
        /// Проверяет, что максимальное значение может быть успешно обновлено, если оно больше текущего значения параметра.
        /// </summary>
        [TestMethod]
        public void MaxValue_SetAboveCurrentValue_ShouldUpdateMaxValueSuccessfully()
        {
            var parameter = new Parameter(0, 100, 50);
            parameter.MaxValue = 150;
            Assert.AreEqual(150, parameter.MaxValue);
        }
    }
}
