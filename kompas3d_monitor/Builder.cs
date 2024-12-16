using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace kompas3d_monitor
{
    internal class Builder
    {
        private Wrapper _wrapper;

        public Builder()
        {
            _wrapper = new Wrapper();
        }

        public void Build(Parameters parameters) 
        {
            BuildDisplay(parameters);
            BuildFrame(parameters);
            BuildStand(parameters);
            BuildBase(parameters);
        }
        public void BuildDisplay(Parameters parameters) 
        {
            double width = parameters.ParametersDict[ParameterType.ScreenWidth].Value;
            double height = parameters.ParametersDict[ParameterType.ScreenHeight].Value;

            // Создаем прямоугольник для экрана
            _wrapper.CreateBox(new Point(0, 0), new Point((int)width, (int)height));
        }
        public void BuildFrame(Parameters parameters)
        {
            double width = parameters.ParametersDict[ParameterType.ScreenWidth].Value;
            double height = parameters.ParametersDict[ParameterType.ScreenHeight].Value;
            double borderHeight = parameters.ParametersDict[ParameterType.BorderHeight].Value;
            double borderThickness = parameters.ParametersDict[ParameterType.BorderThickness].Value;

            // Рамка будет иметь толщину и высоту снаружи экрана
            double frameWidth = width + 2 * borderThickness;
            double frameHeight = height + 2 * borderThickness;

            // Создаем коробку для рамки
            _wrapper.CreateBox(new Point(0, 0), new Point((int)frameWidth, (int)frameHeight));
            // Создаем объекты для экструзии рамки по высоте
            _wrapper.CreateBox(new Point(0, 0), new Point((int)frameWidth, (int)(borderHeight)));
        }
        public void BuildJoint(Parameters parameters) { }
        public void BuildStand(Parameters parameters) 
        {
            double standHeight = parameters.ParametersDict[ParameterType.StandHeight].Value;
            double standWidth = parameters.ParametersDict[ParameterType.StandWidth].Value;
            double standThickness = parameters.ParametersDict[ParameterType.StandThickness].Value;

            // Создаем стойку
            _wrapper.CreateBox(new Point(0, 0), new Point((int)standWidth, (int)standHeight));
        }
        public void BuildBase(Parameters parameters) 
        {
            double baseHeight = parameters.ParametersDict[ParameterType.BaseHeight].Value;
            double baseWidth = parameters.ParametersDict[ParameterType.BaseWidth].Value;
            double baseThickness = parameters.ParametersDict[ParameterType.BaseThickness].Value;

            // Создаем подставку
            _wrapper.CreateBox(new Point(0, 0), new Point((int)baseWidth, (int)baseHeight));
        }

    }
}
