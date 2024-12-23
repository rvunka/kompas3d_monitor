using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using kompas3d_monitor;

namespace UnitTestProject
{
    [TestClass]
    public class ParametersTests
    {
        private Parameters _parameters;

        [TestInitialize]
        public void Setup()
        {
            _parameters = new Parameters();
        }

        [TestMethod]
        public void Validate_ScreenWidth_WithinRange_ShouldPass()
        {
            _parameters.AddValueToParameter(ParameterType.ScreenWidth, 200);
            Assert.AreEqual(200, _parameters.ParametersDict[ParameterType.ScreenWidth].Value);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Validate_ScreenWidth_OutOfRange_ShouldThrowException()
        {
            _parameters.AddValueToParameter(ParameterType.ScreenWidth, 1500);  // Выше допустимого диапазона (50-1000)
        }

        [TestMethod]
        public void AspectRatio_ChangeAspectRatio_ShouldRecalculateHeight()
        {
            _parameters.AddValueToParameter(ParameterType.ScreenWidth, 800);
            _parameters.SetAspectRatio(AspectRatio.SixteenNine);

            double expectedHeight = 800 * 0.56; // Соотношение 16:9
            Assert.AreEqual(expectedHeight, _parameters.ParametersDict[ParameterType.ScreenHeight].Value, 0.1);
        }

        [TestMethod]
        public void CustomAspectRatio_ShouldNotRecalculateHeight()
        {
            _parameters.SetAspectRatio(AspectRatio.Custom);
            _parameters.AddValueToParameter(ParameterType.ScreenWidth, 700);

            Assert.AreEqual(700, _parameters.ParametersDict[ParameterType.ScreenWidth].Value);
            Assert.AreNotEqual(700 * 0.56, _parameters.ParametersDict[ParameterType.ScreenHeight].Value);
        }

        [TestMethod]
        public void JointHeight_ValidValue_ShouldPass()
        {
            _parameters.AddValueToParameter(ParameterType.JointHeight, 50);
            Assert.AreEqual(50, _parameters.ParametersDict[ParameterType.JointHeight].Value);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void JointHeight_InvalidValue_ShouldThrowException()
        {
            _parameters.AddValueToParameter(ParameterType.JointHeight, 300);  // Превышает диапазон (20-200)
        }
    }
}
