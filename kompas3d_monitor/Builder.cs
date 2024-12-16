using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kompas6API5;
using Kompas6Constants3D;
using KompasAPI7;

namespace kompas3d_monitor
{
    internal class Builder
    {
        private Wrapper _wrapper;

        public Builder()
        {
            _wrapper = new Wrapper();
        }

        /// <summary>
        /// Построение монитора целиком.
        /// </summary>
        /// <param name="parameters">Объект с параметрами монитора.</param>
        public void Build(Parameters parameters)
        {
            BuildDisplay(parameters);
            BuildBorder(parameters);
            BuildJoint(parameters);
            BuildStand(parameters);
            BuildBase(parameters);
            Console.WriteLine("Построение монитора завершено.");
        }

        /// <summary>
        /// Построение экрана монитора.
        /// </summary>
        private void BuildDisplay(Parameters parameters)
        {
            Console.WriteLine("Построение экрана...");
            _wrapper.CreateBox(0, 0, parameters.ParametersDict[ParameterType.ScreenWidth].Value, parameters.ParametersDict[ParameterType.ScreenHeight].Value, parameters.ParametersDict[ParameterType.ScreenDepth].Value);
        }

        /// <summary>
        /// Построение рамки монитора.
        /// </summary>
        private void BuildBorder(Parameters parameters)
        {
            Console.WriteLine("Построение рамки...");
            double outerWidth = parameters.ParametersDict[ParameterType.ScreenWidth].Value + 2 * parameters.ParametersDict[ParameterType.BorderThickness].Value;
            double outerHeight = parameters.ParametersDict[ParameterType.ScreenHeight].Value + 2 * parameters.ParametersDict[ParameterType.BorderHeight].Value;

            // Внешний прямоугольник рамки
            _wrapper.CreateBox(0, 0, outerWidth, outerHeight, parameters.ParametersDict[ParameterType.BorderThickness].Value);

            // Вырез для экрана (внутренняя часть рамки)
            _wrapper.CreateBox(parameters.ParametersDict[ParameterType.BorderThickness].Value, parameters.ParametersDict[ParameterType.BorderHeight].Value,
                parameters.ParametersDict[ParameterType.ScreenWidth].Value, parameters.ParametersDict[ParameterType.ScreenHeight].Value, parameters.ParametersDict[ParameterType.BorderThickness].Value);
        }

        /// <summary>
        /// Построение соединительного рычага.
        /// </summary>
        private void BuildJoint(Parameters parameters)
        {
            Console.WriteLine("Построение соединения...");
            double x = parameters.ParametersDict[ParameterType.ScreenWidth].Value / 2 - parameters.ParametersDict[ParameterType.JointLength].Value / 2;
            double y = -parameters.ParametersDict[ParameterType.BorderHeight].Value - parameters.ParametersDict[ParameterType.JointHeight].Value;
            _wrapper.CreateBox(x, y, parameters.ParametersDict[ParameterType.JointLength].Value, parameters.ParametersDict[ParameterType.JointHeight].Value, parameters.ParametersDict[ParameterType.BorderThickness].Value);
        }

        /// <summary>
        /// Построение стойки монитора.
        /// </summary>
        private void BuildStand(Parameters parameters)
        {
            Console.WriteLine("Построение стойки...");
            double x = parameters.ParametersDict[ParameterType.ScreenWidth].Value / 2 - parameters.ParametersDict[ParameterType.StandWidth].Value / 2;
            double y = -parameters.ParametersDict[ParameterType.BorderHeight].Value - parameters.ParametersDict[ParameterType.JointHeight].Value - parameters.ParametersDict[ParameterType.StandHeight].Value;
            _wrapper.CreateBox(x, y, parameters.ParametersDict[ParameterType.StandWidth].Value, parameters.ParametersDict[ParameterType.StandHeight].Value, parameters.ParametersDict[ParameterType.StandThickness].Value);
        }

        /// <summary>
        /// Построение подставки монитора.
        /// </summary>
        private void BuildBase(Parameters parameters)
        {
            Console.WriteLine("Построение подставки...");
            double x = parameters.ParametersDict[ParameterType.ScreenWidth].Value / 2 - parameters.ParametersDict[ParameterType.BaseWidth].Value / 2;
            double y = -parameters.ParametersDict[ParameterType.BorderHeight].Value - parameters.ParametersDict[ParameterType.JointHeight].Value - parameters.ParametersDict[ParameterType.StandHeight].Value - parameters.ParametersDict[ParameterType.BaseHeight].Value;
            _wrapper.CreateBox(x, y, parameters.ParametersDict[ParameterType.BaseWidth].Value, parameters.ParametersDict[ParameterType.BaseHeight].Value, parameters.ParametersDict[ParameterType.BaseThickness].Value);
        }

    }
}
