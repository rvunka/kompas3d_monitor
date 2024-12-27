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
using KompasAPIWrapper;
using MonitorModel;

namespace MonitorBuilder
{
    //TODO:XML
    public class Builder
    {
        //TODO:XML
        private Wrapper _wrapper;

        //TODO:XML
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

            //TODO: RSDN
            var rectParam = _wrapper.CreateRectangleParam(-innerWidth / 2, (-innerHeight / 2) - standHeight - baseHeight, innerWidth, innerHeight);
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

            double x = -standWidth / 2;
            double y = -standThickness / 2;
            double offsetZ = -baseHeight / 2;

            _wrapper.CreateBox(standThickness + b, x, y + offsetZ, standWidth, -standHeight, standThickness);
        }

        /// <summary>
        /// Построение подставки монитора.
        /// </summary>
        private void BuildBase(Parameters parameters)
        {
            double D = parameters.ParametersDict[ParameterType.BaseWidth].Value;
            double s = parameters.ParametersDict[ParameterType.BaseHeight].Value;
            double z = parameters.ParametersDict[ParameterType.BaseThickness].Value;

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
