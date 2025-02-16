﻿using System;
using Kompas6API5;
using Kompas6Constants3D;
using KompasAPIWrapper;
using MonitorModel;

namespace MonitorBuilder
{
    /// <summary>
    /// Класс для построения различных частей монитора с использованием паттерна Builder.
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Обертка для взаимодействия с системой моделирования.
        /// </summary>
        private Wrapper _wrapper;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Builder"/>.
        /// </summary>
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
            double L = parameters.ParametersDict[ParameterType.ScreenWidth].Value;
            double H = parameters.ParametersDict[ParameterType.ScreenHeight].Value;
            double g = parameters.ParametersDict[ParameterType.ScreenThickness].Value;
            double standHeight = parameters.ParametersDict[ParameterType.StandHeight].Value;
            double baseHeight = parameters.ParametersDict[ParameterType.BaseHeight].Value;

            //Центрирование объекта = 1/2
            double x = -L / 2;
            double y = -H / 2;

            _wrapper.CreateBox(0, x, y - standHeight - baseHeight, L, H, g);
        }

        /// <summary>
        /// Построение рамки монитора.
        /// </summary>
        private void BuildBorder(Parameters parameters)
        {
            double L = parameters.ParametersDict[ParameterType.ScreenWidth].Value;
            double H = parameters.ParametersDict[ParameterType.ScreenHeight].Value;
            double bW = parameters.ParametersDict[ParameterType.BorderWidth].Value;
            double bH = parameters.ParametersDict[ParameterType.BorderHeight].Value;
            double bD = parameters.ParametersDict[ParameterType.BorderDepth].Value;
            double g = parameters.ParametersDict[ParameterType.ScreenThickness].Value;
            double standHeight = parameters.ParametersDict[ParameterType.StandHeight].Value;
            double baseHeight = parameters.ParametersDict[ParameterType.BaseHeight].Value;

            double innerWidth = L - 2 * bW;
            double innerHeight = H - 2 * bH;

            var part = _wrapper.GetCurrentPart();
            var sketch = _wrapper.CreateSketch(part, (short)Obj3dType.o3d_planeXOZ);
            var definition = (ksSketchDefinition)sketch.GetDefinition();

            var planeXOZ = part.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ);
            var offsetPlane = (ksEntity)part.NewEntity((short)Obj3dType.o3d_planeOffset);
            var planeDef = (ksPlaneOffsetDefinition)offsetPlane.GetDefinition();
            planeDef.offset = g;
            planeDef.direction = true;
            planeDef.SetPlane(planeXOZ);
            offsetPlane.Create();

            definition.SetPlane(offsetPlane);
            sketch.Create();

            var sketchEdit = (ksDocument2D)definition.BeginEdit();
            
            //Центрирование объекта = 1/2
            double x = -innerWidth / 2;
            double y = (-innerHeight / 2) - standHeight - baseHeight;
            double width = innerWidth;
            double height = innerHeight;

            var rectParam = _wrapper.CreateRectangleParam(x, y, width, height);
            sketchEdit.ksRectangle(rectParam, 0);

            definition.EndEdit();
            sketch.Update();

            if (sketch != null)
            {
                _wrapper.CutExtrusion(sketch, bD, false);
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
            double j = parameters.ParametersDict[ParameterType.JointWidth].Value;
            double f = parameters.ParametersDict[ParameterType.JointHeight].Value;
            double b = parameters.ParametersDict[ParameterType.JointLenght].Value;
            double standHeight = parameters.ParametersDict[ParameterType.StandHeight].Value;
            double baseHeight = parameters.ParametersDict[ParameterType.BaseHeight].Value;

            //Центрирование объекта = 1/2
            double x = -j / 2;
            double y = -f / 2;

            _wrapper.CreateBox(b, x, y - standHeight - baseHeight, j, f, b);
        }

        /// <summary>
        /// Построение стойки монитора.
        /// </summary>
        private void BuildStand(Parameters parameters)
        {
            double standWidth = parameters.ParametersDict[ParameterType.StandWidth].Value;
            double standHeight = parameters.ParametersDict[ParameterType.StandHeight].Value;
            double standThickness = parameters.ParametersDict[ParameterType.StandThickness].Value;
            double baseHeight = parameters.ParametersDict[ParameterType.BaseHeight].Value;
            double b = parameters.ParametersDict[ParameterType.JointLenght].Value;

            //Центрирование объекта = 1/2
            double x = -standWidth / 2;
            double y = -standThickness / 2;
            double offsetZ = -baseHeight / 2;
            double offsetX = standThickness + b;

            //TODO: RSDN
            _wrapper.CreateBox(offsetX, x, y + offsetZ, standWidth, -standHeight, standThickness);
        }

        /// <summary>
        /// Построение подставки монитора.
        /// </summary>
        private void BuildBase(Parameters parameters)
        {
            double D = parameters.ParametersDict[ParameterType.BaseWidth].Value;
            double s = parameters.ParametersDict[ParameterType.BaseHeight].Value;
            double z = parameters.ParametersDict[ParameterType.BaseThickness].Value;

            //Центрирование объекта = 1/2
            double x = -D / 2;
            double y = -z / 2;

            if (parameters.BaseShape == BaseShape.Rectangle)
            {
                _wrapper.CreateBox(0, x, y, D, z, s, (short)Obj3dType.o3d_planeXOY);
                return;
            }

            if (parameters.BaseShape == BaseShape.Circle)
            {
                _wrapper.CreateCircle(0, 0, 0, D / 2, s, (short)Obj3dType.o3d_planeXOY);
                return;
            }

            if (parameters.BaseShape == BaseShape.Trapeze)
            {
                _wrapper.CreateBox(0, x, y, D, z, s, (short)Obj3dType.o3d_planeXOY);

                var part = _wrapper.GetCurrentPart();
                var sketch = _wrapper.CreateSketch(part, (short)Obj3dType.o3d_planeXOY);
                var definition = (ksSketchDefinition)sketch.GetDefinition();
                var planeXOY = part.GetDefaultEntity((short)Obj3dType.o3d_planeXOY);

                definition.SetPlane(planeXOY);
                sketch.Create();

                var sketchEdit = (ksDocument2D)definition.BeginEdit();

                // Наименьшая сторона трапеции = наибольшая сторона - 15 * 2
                sketchEdit.ksLineSeg(x, y, x, y + z, 1);
                sketchEdit.ksLineSeg(x, y + z, x + 15, y + z, 1);
                sketchEdit.ksLineSeg(x + 15, y + z, x, y, 1);

                sketchEdit.ksLineSeg(x + D, y, x + D, y + z, 1);
                sketchEdit.ksLineSeg(x + D, y + z, x + D - 15, y + z, 1);
                sketchEdit.ksLineSeg(x + D - 15, y + z, x + D, y, 1);

                definition.EndEdit();
                sketch.Update();

                _wrapper.CutExtrusion(sketch, s, true);
                return;
            }
        }

    }
}
