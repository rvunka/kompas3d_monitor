using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorModel
{
    //TODO:XML
    /// <summary>
    /// Класс, представляющий параметры для модели монитора.
    /// </summary>
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

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Parameters"/>.
        /// </summary>
        /// <param name="parameters">Словарь с параметрами модели монитора.</param>
        /// <param name="aspectRatio">Соотношение сторон для экрана.</param>
        public Parameters(Dictionary<ParameterType, Parameter> parameters, AspectRatio aspectRatio)
        {
            _parametersDict = parameters;
            _aspectRatio = aspectRatio;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Parameters"/> с параметрами по умолчанию.
        /// </summary>
        public Parameters()
        {
            _aspectRatio = AspectRatio.Custom;
            _baseShape = BaseShape.Trapeze;
        }

        /// <summary>
        /// Получает словарь параметров для модели монитора.
        /// </summary>
        public Dictionary<ParameterType, Parameter> ParametersDict
        {
            get { return _parametersDict; }
        }

        /// <summary>
        /// Устанавливает новое значение для указанного параметра.
        /// </summary>
        /// <param name="parameterType">Тип параметра, для которого устанавливается новое значение.</param>
        /// <param name="value">Новое значение параметра.</param>
        /// <exception cref="KeyNotFoundException">Выбрасывается, если указанный параметр не найден.</exception>
        public void AddValueToParameter(ParameterType parameterType, double value)
        {
            if (_parametersDict.ContainsKey(parameterType))
            {
                _parametersDict[parameterType].Value = value;

                if (_aspectRatio != AspectRatio.Custom)
                {
                    // Проверка связи параметров ScreenWidth и ScreenHeight
                    if (parameterType == ParameterType.ScreenWidth)
                    {
                        try
                        {
                            double heightValue = value * GetAspectRatioFactor(_aspectRatio);
                            _parametersDict[ParameterType.ScreenHeight].Value = heightValue;
                        }
                        catch { }
                    }
                    else if (parameterType == ParameterType.ScreenHeight)
                    {
                        try
                        {
                            double widthValue = value / GetAspectRatioFactor(_aspectRatio);
                            _parametersDict[ParameterType.ScreenWidth].Value = widthValue;
                        }
                        catch { }
                    }
                }
            }
            else
            {
                throw new KeyNotFoundException("Parameter not found");
            }
        }

        /// <summary>
        /// Устанавливает новое соотношение сторон для экрана.
        /// </summary>
        /// <param name="aspectRatio">Новое соотношение сторон.</param>
        public void SetAspectRatio(AspectRatio aspectRatio)
        {
            _aspectRatio = aspectRatio;

            if (_aspectRatio != AspectRatio.Custom)
            {
                double screenWidth = _parametersDict[ParameterType.ScreenWidth].Value;
                _parametersDict[ParameterType.ScreenHeight].Value = screenWidth * GetAspectRatioFactor(_aspectRatio);
            }
        }

        /// <summary>
        /// Получает коэффициент соотношения сторон для указанного типа соотношения.
        /// </summary>
        /// <param name="aspectRatio">Тип соотношения сторон.</param>
        /// <returns>Коэффициент соотношения сторон.</returns>
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

        /// <summary>
        /// Получает или устанавливает форму основания монитора.
        /// </summary>
        public BaseShape BaseShape
        {
            get { return _baseShape; }
            set { _baseShape = value; }
        }

    }
}
