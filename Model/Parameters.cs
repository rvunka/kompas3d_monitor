using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kompas3d_monitor
{
    public class Parameters
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
            _aspectRatio = AspectRatio.SixteenNine;
            _parametersDict.Add(ParameterType.ScreenWidth, new Parameter(50, 1000, 200));
            _parametersDict.Add(ParameterType.ScreenHeight, new Parameter(50, 1000, 112));
            _parametersDict.Add(ParameterType.ScreenThickness, new Parameter(5, 30, 10));
            _parametersDict.Add(ParameterType.BorderHeight, new Parameter(5, 30, 10));
            _parametersDict.Add(ParameterType.BorderWidth, new Parameter(5, 30, 10));
            _parametersDict.Add(ParameterType.BorderDepth, new Parameter(0, 20, 5));
            _parametersDict.Add(ParameterType.StandHeight, new Parameter(50, 200, 125));
            _parametersDict.Add(ParameterType.StandWidth, new Parameter(30, 100, 40));
            _parametersDict.Add(ParameterType.StandThickness, new Parameter(10, 50, 10));
            _parametersDict.Add(ParameterType.BaseHeight, new Parameter(10, 150, 10));
            _parametersDict.Add(ParameterType.BaseWidth, new Parameter(50, 400, 75));
            _parametersDict.Add(ParameterType.BaseThickness, new Parameter(50, 400, 75));
            _parametersDict.Add(ParameterType.JointHeight, new Parameter(20, 200, 20));
            _parametersDict.Add(ParameterType.JointWidth, new Parameter(20, 200, 60));
            _parametersDict.Add(ParameterType.JointLenght, new Parameter(0, 150, 15));
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

                if (_aspectRatio != AspectRatio.Custom)
                {
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
            }
            else
            {
                throw new KeyNotFoundException("Parameter not found");
            }
        }

        public void SetAspectRatio(AspectRatio aspectRatio)
        {
            _aspectRatio = aspectRatio;

            if (_aspectRatio != AspectRatio.Custom)
            {
                double screenWidth = _parametersDict[ParameterType.ScreenWidth].Value;
                _parametersDict[ParameterType.ScreenHeight].Value = screenWidth * GetAspectRatioFactor(_aspectRatio);
            }
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
