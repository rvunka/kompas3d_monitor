using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kompas6API5;
using Kompas6Constants;
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
            //BuildJoint(parameters);
            BuildStand(parameters);
            //BuildBase(parameters);
            Console.WriteLine("Построение монитора завершено.");
        }

        /// <summary>
        /// Построение экрана монитора.
        /// </summary>
        private void BuildDisplay(Parameters parameters)
        {
            Console.WriteLine("Построение экрана...");
            double L = parameters.ParametersDict[ParameterType.ScreenWidth].Value;
            double H = parameters.ParametersDict[ParameterType.ScreenHeight].Value;
            double g = parameters.ParametersDict[ParameterType.ScreenThickness].Value;

            // Построение внешнего прямоугольника (экран) в -Y
            _wrapper.CreateBox(0, 0, L, H, g); 
        }

        /// <summary>
        /// Построение рамки монитора.
        /// </summary>
        private void BuildBorder(Parameters parameters)
        {
            Console.WriteLine("Построение рамки...");

            double L = parameters.ParametersDict[ParameterType.ScreenWidth].Value;
            double H = parameters.ParametersDict[ParameterType.ScreenHeight].Value;
            double bW = parameters.ParametersDict[ParameterType.BorderWidth].Value;
            double bH = parameters.ParametersDict[ParameterType.BorderHeight].Value;
            double bD = parameters.ParametersDict[ParameterType.BorderDepth].Value;
            double g = parameters.ParametersDict[ParameterType.ScreenThickness].Value;

            double innerWidth = L - 2 * bW;
            double innerHeight = H - 2 * bH;

            var part = _wrapper.GetCurrentPart();
            var sketch = _wrapper.CreateSketch(part, (short)Obj3dType.o3d_planeXOZ);
            var definition = (ksSketchDefinition)sketch.GetDefinition();

            // Получаем плоскость XOZ и смещаем ее вперед на толщину экрана
            var planeXOZ = part.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            var offsetPlane = (ksEntity)part.NewEntity((short)Obj3dType.o3d_planeOffset);
            var planeDef = (ksPlaneOffsetDefinition)offsetPlane.GetDefinition();
            planeDef.offset = g;  // Смещаем вперед на толщину экрана
            planeDef.direction = true;  // Смещение в положительном направлении
            planeDef.SetPlane(planeXOZ);  // Устанавливаем плоскость от которой смещаем
            offsetPlane.Create();  // Создаем смещенную плоскость

            // Привязываем эскиз к смещенной плоскости
            definition.SetPlane(offsetPlane);
            sketch.Create();

            var sketchEdit = (ksDocument2D)definition.BeginEdit();

            // Внутренний прямоугольник для выреза (рамка)
            var rectParam = _wrapper.CreateRectangleParam(-innerWidth / 2, -innerHeight / 2, innerWidth, innerHeight);
            sketchEdit.ksRectangle(rectParam, 0);  // Рамка строится от центра

            definition.EndEdit();
            sketch.Update();

            if (sketch != null)
            {
                // Вырезаем из экрана
                _wrapper.CutExtrusion(sketch, bD, false);  // Вырезаем вперед
                Console.WriteLine("Рамка экрана вырезана.");
            }
            else
            {
                Console.WriteLine("Ошибка: Эскиз для выреза не создан.");
            }
        }

        /// <summary>
        /// Построение соединительного рычага.
        /// </summary>
        private void BuildJoint(Parameters parameters)
        {
            Console.WriteLine("Построение соединения...");
            double j = parameters.ParametersDict[ParameterType.JointWidth].Value;
            double f = parameters.ParametersDict[ParameterType.StandThickness].Value;
            double b = parameters.ParametersDict[ParameterType.BorderHeight].Value;
            double g = parameters.ParametersDict[ParameterType.ScreenThickness].Value;

            double x = -j / 2;
            double y = f / 2;
            double offsetZ = g + b;  // Рычаг располагается за экраном

            _wrapper.CreateBox(x, y, j, f, b);
        }

        /// <summary>
        /// Построение стойки монитора.
        /// </summary>
        private void BuildStand(Parameters parameters)
        {
            double standWidth = parameters.ParametersDict[ParameterType.StandWidth].Value;
            double standHeight = parameters.ParametersDict[ParameterType.StandHeight].Value;
            double standThickness = parameters.ParametersDict[ParameterType.StandThickness].Value;
            double baseHeight = parameters.ParametersDict[ParameterType.BaseThickness].Value;  // Высота подставки

            // Позиционирование стойки по центру подставки (X и Y)
            double x = -standWidth / 2;
            double y = -standThickness / 2;

            // Смещение вверх по Z на высоту подставки
            double z = baseHeight;

            // Создаем эскиз на плоскости XOY и выдавливаем вверх по Z
            var part = _wrapper.GetCurrentPart();
            var sketch = _wrapper.CreateSketch(part, (short)Obj3dType.o3d_planeXOY);
            var definition = (ksSketchDefinition)sketch.GetDefinition();
            sketch.Create();

            var sketchEdit = (ksDocument2D)definition.BeginEdit();

            // Рисуем прямоугольник стойки
            var rectParam = _wrapper.CreateRectangleParam(x, y, standWidth, standThickness);
            sketchEdit.ksRectangle(rectParam, 0);  // Прямоугольник строится от центра

            definition.EndEdit();
            sketch.Update();

            // Выдавливаем стойку вверх (по Z)
            _wrapper.Extrusion(sketch, standHeight);  // Стойка тянется вверх

            Console.WriteLine("Стойка (ножка) построена.");
        }

        /// <summary>
        /// Построение подставки монитора.
        /// </summary>
        private void BuildBase(Parameters parameters)
        {
            double D = parameters.ParametersDict[ParameterType.BaseWidth].Value;
            double s = parameters.ParametersDict[ParameterType.BaseHeight].Value;
            double z = parameters.ParametersDict[ParameterType.BaseThickness].Value;
            double p = parameters.ParametersDict[ParameterType.StandHeight].Value;
            double g = parameters.ParametersDict[ParameterType.ScreenThickness].Value;

            // Позиционирование подставки ниже стойки
            double x = -D / 2;
            double y = -s / 2;
            double offsetZ = -p - z - g;  // Подставка располагается ниже стойки и экрана

            _wrapper.CreateBox(x, y, D, s, z);
            Console.WriteLine("Подставка построена.");
        }

    }
}
