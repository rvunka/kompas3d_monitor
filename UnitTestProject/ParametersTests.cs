using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MonitorModel;
using System.Collections.Generic;

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
        public void Validate_ScreenWidth_OutOfRange_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                _parameters.AddValueToParameter(ParameterType.ScreenWidth, 1500) 
            );

            Assert.IsTrue(exception.Message.Contains("должно быть между")); 
        }

        [TestMethod]
        public void AspectRatio_ChangeAspectRatio_ShouldRecalculateHeight()
        {
            _parameters.AddValueToParameter(ParameterType.ScreenWidth, 800);
            _parameters.SetAspectRatio(AspectRatio.SixteenNine);

            double expectedHeight = 800 * 0.56; 
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
        public void JointHeight_InvalidValue_ShouldThrowException()
        {
            var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                _parameters.AddValueToParameter(ParameterType.JointHeight, 300) 
            );

            Assert.IsTrue(exception.Message.Contains("должно быть между"));
        }

        [TestMethod]
        public void GetAspectRatioFactor_FourThree_ShouldReturnCorrectValue()
        {
            var aspectRatio = AspectRatio.FourThree;
            var result = _parameters.GetAspectRatioFactor(aspectRatio);

            Assert.AreEqual(0.75, result);
        }

        [TestMethod]
        public void GetAspectRatioFactor_SixteenTen_ShouldReturnCorrectValue()
        {
            var aspectRatio = AspectRatio.SixteenTen;
            var result = _parameters.GetAspectRatioFactor(aspectRatio);

            Assert.AreEqual(0.625, result);
        }

        [TestMethod]
        public void GetAspectRatioFactor_SixteenNine_ShouldReturnCorrectValue()
        {
            var aspectRatio = AspectRatio.SixteenNine;
            var result = _parameters.GetAspectRatioFactor(aspectRatio);

            Assert.AreEqual(0.56, result);
        }

        [TestMethod]
        public void GetAspectRatioFactor_TwentyOneNine_ShouldReturnCorrectValue()
        {
            var aspectRatio = AspectRatio.TwentyOneNine;
            var result = _parameters.GetAspectRatioFactor(aspectRatio);

            Assert.AreEqual(0.43, result);
        }

        [TestMethod]
        public void GetAspectRatioFactor_Custom_ShouldReturnDefaultValue()
        {
            var aspectRatio = AspectRatio.Custom;
            var result = _parameters.GetAspectRatioFactor(aspectRatio);

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void GetAspectRatioFactor_InvalidAspectRatio_ShouldReturnDefaultValue()
        {
            var aspectRatio = (AspectRatio)999; 
            var result = _parameters.GetAspectRatioFactor(aspectRatio);

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void AddValueToParameter_SetScreenWidth_ShouldRecalculateScreenHeight()
        {
            _parameters.SetAspectRatio(AspectRatio.SixteenNine);
            _parameters.AddValueToParameter(ParameterType.ScreenWidth, 300);

            Assert.AreEqual(300, _parameters.ParametersDict[ParameterType.ScreenWidth].Value);
            Assert.AreEqual(300 * 0.56, _parameters.ParametersDict[ParameterType.ScreenHeight].Value, 0.01);
        }

        [TestMethod]
        public void AddValueToParameter_SetScreenHeight_ShouldRecalculateScreenWidth()
        {
            _parameters.SetAspectRatio(AspectRatio.SixteenNine);
            _parameters.AddValueToParameter(ParameterType.ScreenHeight, 168);

            Assert.AreEqual(168, _parameters.ParametersDict[ParameterType.ScreenHeight].Value);
            Assert.AreEqual(168 / 0.56, _parameters.ParametersDict[ParameterType.ScreenWidth].Value, 0.01);
        }

        [TestMethod]
        public void AddValueToParameter_InvalidKey_ShouldThrowKeyNotFoundException()
        {
            var invalidKey = (ParameterType)999; 

            Assert.ThrowsException<KeyNotFoundException>(() =>
                _parameters.AddValueToParameter(invalidKey, 100)
            );
        }

        [TestMethod]
        public void AddValueToParameter_ScreenWidth_CausesException_ShouldCatchIt()
        {
            var parameters = new Parameters();
            parameters.SetAspectRatio(AspectRatio.SixteenNine);
            parameters.ParametersDict.Remove(ParameterType.ScreenHeight);
            parameters.AddValueToParameter(ParameterType.ScreenWidth, 300);

            Assert.AreEqual(300, parameters.ParametersDict[ParameterType.ScreenWidth].Value);
        }

        [TestMethod]
        public void AddValueToParameter_ScreenHeight_CausesException_ShouldCatchIt()
        {
            // Arrange
            var parameters = new Parameters();
            parameters.SetAspectRatio(AspectRatio.SixteenNine);
            parameters.ParametersDict.Remove(ParameterType.ScreenWidth);
            parameters.AddValueToParameter(ParameterType.ScreenHeight, 200);

            Assert.AreEqual(200, parameters.ParametersDict[ParameterType.ScreenHeight].Value);
        }

        [TestMethod]
        public void BaseShape_SetAndGet_ShouldWorkCorrectly()
        {
            _parameters.BaseShape = BaseShape.Circle;
            var result = _parameters.BaseShape;
            Assert.AreEqual(BaseShape.Circle, result);
        }

        [TestMethod]
        public void Constructor_WithParameters_ShouldInitializeCorrectly()
        {
            var parameters = new Dictionary<ParameterType, Parameter>
            {
                { ParameterType.ScreenWidth, new Parameter(100, 1000, 500) }
            };
            var aspectRatio = AspectRatio.SixteenNine;
            var parametersObj = new Parameters(parameters, aspectRatio);

            Assert.AreEqual(0.56, parametersObj.GetAspectRatioFactor(aspectRatio)); 
            Assert.AreEqual(parameters, parametersObj.ParametersDict);
        }

    }
}
