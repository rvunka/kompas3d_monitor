using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kompas3d_monitor
{
    internal class Parameters
    {
        private Dictionary<ParameterType, Parameter> _parametersDict = new Dictionary<ParameterType, Parameter>();
        private AspectRatio _aspectRatio;

        public Parameters(Dictionary<ParameterType, Parameter> parameters, AspectRatio aspectRatio)
        {
            _parametersDict = parameters;
            _aspectRatio = aspectRatio;
        }

        public Parameters()
        {
            _aspectRatio = AspectRatio.Custom;
            _parametersDict.Add(ParameterType.ScreenWidth, new Parameter(50, 1000, 400));
            _parametersDict.Add(ParameterType.ScreenHeight, new Parameter(50, 1000, 400));
            _parametersDict.Add(ParameterType.BorderHeight, new Parameter(5, 30, 5));
            _parametersDict.Add(ParameterType.BorderThickness, new Parameter(5, 20, 5));
            _parametersDict.Add(ParameterType.ScreenDepth, new Parameter(0, 10, 0));
            _parametersDict.Add(ParameterType.StandHeight, new Parameter(50, 200, 50));
            _parametersDict.Add(ParameterType.StandWidth, new Parameter(30, 100, 30));
            _parametersDict.Add(ParameterType.StandThickness, new Parameter(10, 50, 10));
            _parametersDict.Add(ParameterType.BaseHeight, new Parameter(50, 150, 50));
            _parametersDict.Add(ParameterType.BaseWidth, new Parameter(150, 400, 150));
            _parametersDict.Add(ParameterType.BaseThickness, new Parameter(10, 50, 10));
            _parametersDict.Add(ParameterType.JointHeight, new Parameter(15, 40, 15));
            _parametersDict.Add(ParameterType.JointLength, new Parameter(50, 200, 50));
        }

        public Dictionary<ParameterType, Parameter> ParametersDict
        {
            get { return _parametersDict; }
        }

        public void AddValueToParameter(ParameterType parameterType, double value)
        {
            if (_parametersDict.ContainsKey(parameterType))
            {
                _parametersDict[parameterType].Value = value;
                _parametersDict[parameterType].Validate();

                // Проверка связи параметров ScreenWidth и ScreenHeight
                if (parameterType == ParameterType.ScreenWidth)
                {
                    double heightValue = value * GetAspectRatioFactor(_aspectRatio);
                    _parametersDict[ParameterType.ScreenHeight].Value = heightValue;
                }
                else if (parameterType == ParameterType.ScreenHeight)
                {
                    double widthValue = value / GetAspectRatioFactor(_aspectRatio);
                    _parametersDict[ParameterType.ScreenWidth].Value = widthValue;
                }
            }
            else
            {
                throw new KeyNotFoundException("Parameter not found");
            }
        }

        public void SetAspectRatio(AspectRatio aspectRatio)
        {
            _aspectRatio = aspectRatio;
            // Пересчитываем значение ScreenHeight на основе нового соотношения сторон
            double screenWidth = _parametersDict[ParameterType.ScreenWidth].Value;
            _parametersDict[ParameterType.ScreenHeight].Value = screenWidth * GetAspectRatioFactor(_aspectRatio);
        }

        private double GetAspectRatioFactor(AspectRatio aspectRatio)
        {
            switch (aspectRatio)
            {
                case AspectRatio.FourThree:
                    return 0.75;
                case AspectRatio.SixteenTen:
                    return 0.625;
                case AspectRatio.SixteenNine:
                    return 0.56;
                case AspectRatio.TwentyOneNine:
                    return 0.43;
                default:
                    return 1;
            }
        }

    }
}
