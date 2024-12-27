using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorModel
{
    //TODO:XML
    public class Parameters
    {
        //TODO: refactor
        private Dictionary<ParameterType, Parameter> _parametersDict = new Dictionary<ParameterType, Parameter>
        {
            { ParameterType.ScreenWidth, new Parameter(50, 1000, 200) },
            { ParameterType.ScreenHeight, new Parameter(50, 1000, 200) },
            { ParameterType.ScreenThickness, new Parameter(5, 30, 10) },
            { ParameterType.BorderHeight, new Parameter(5, 30, 10) },
            { ParameterType.BorderWidth, new Parameter(5, 30, 10) },
            { ParameterType.BorderDepth, new Parameter(0, 20, 5) },
            { ParameterType.StandHeight, new Parameter(50, 200, 125) },
            { ParameterType.StandWidth, new Parameter(30, 100, 40) },
            { ParameterType.StandThickness, new Parameter(10, 50, 10) },
            { ParameterType.BaseHeight, new Parameter(10, 150, 10) },
            { ParameterType.BaseWidth, new Parameter(50, 400, 75) },
            { ParameterType.BaseThickness, new Parameter(50, 400, 75) },
            { ParameterType.JointHeight, new Parameter(20, 200, 20) },
            { ParameterType.JointWidth, new Parameter(20, 200, 60) },
            { ParameterType.JointLenght, new Parameter(0, 150, 15) }
        };
        private AspectRatio _aspectRatio;
        private BaseShape _baseShape;

        public Parameters(Dictionary<ParameterType, Parameter> parameters, AspectRatio aspectRatio)
        {
            _parametersDict = parameters;
            _aspectRatio = aspectRatio;
        }

        public Parameters()
        {
            _aspectRatio = AspectRatio.Custom;
            _baseShape = BaseShape.Trapeze;
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

        public BaseShape BaseShape
        {
            get { return _baseShape; }
            set { _baseShape = value; }
        }

    }
}
