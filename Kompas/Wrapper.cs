using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Kompas6API5;
using Kompas6Constants;
using Kompas6Constants3D;
using KompasAPI7;

namespace KompasAPIWrapper
{
    //TODO: RSDN
    public class Wrapper
    {
        private KompasObject _kompas;
        private ksDocument3D _doc3D;

        public Wrapper()
        {
            OpenCAD();
            CreateFile();
        }

        public void CreateLine(double x1, double y1, double x2, double y2)
        {
            if (_doc3D == null) return;

            var part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);
            var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);

            var definition = (ksSketchDefinition)sketch.GetDefinition();
            definition.SetPlane(part.GetDefaultEntity((short)Obj3dType.o3d_planeXOZ));
            sketch.Create();

            var sketchEdit = (ksDocument2D)definition.BeginEdit();
            sketchEdit.ksLineSeg(x1, y1, x2, y2, 1);
            definition.EndEdit();
        }

        //TODO: RSDN
        public void CreateBox(double offset, double x, double y, double width, double height, double depth, short planeType = (short)Obj3dType.o3d_planeXOZ)
        {
            if (_doc3D == null)
            {
                Console.WriteLine("Документ не создан.");
                return;
            }

            var part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);
            var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var definition = (ksSketchDefinition)sketch.GetDefinition();

            definition.SetPlane(part.GetDefaultEntity(planeType));
            sketch.Create();

            var rectParam = (RectangleParam)_kompas.GetParamStruct((short)StructType2DEnum.ko_RectangleParam);
            rectParam.x = x;
            rectParam.y = y;
            rectParam.width = width;
            rectParam.height = height;
            rectParam.style = 1;
            var sketchEdit = (ksDocument2D)definition.BeginEdit();
            sketchEdit.ksRectangle(rectParam, 0);
            definition.EndEdit();

            if (offset != 0)
            {
                definition.SetPlane(CreateOffsetPlane(part, planeType, offset));
                sketch.Update();
                Extrusion(sketch, depth);
            }
            else
            {
                Extrusion(sketch, depth);
            }
        }

        public void CreateCircle(double offset, double x, double y, double radius, double depth, short planeType = (short)Obj3dType.o3d_planeXOZ)
        {
            if (_doc3D == null)
            {
                Console.WriteLine("Документ не создан.");
                return;
            }

            var part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);
            var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var definition = (ksSketchDefinition)sketch.GetDefinition();

            definition.SetPlane(part.GetDefaultEntity(planeType));
            sketch.Create();

            var circleParam = (CircleParam)_kompas.GetParamStruct((short)StructType2DEnum.ko_CircleParam);
            circleParam.xc = x;
            circleParam.yc = y;
            circleParam.rad = radius;
            var sketchEdit = (ksDocument2D)definition.BeginEdit();
            sketchEdit.ksCircle(x, y, radius, 1);
            definition.EndEdit();

            if (offset != 0)
            {
                definition.SetPlane(CreateOffsetPlane(part, planeType, offset));
                sketch.Update();
                Extrusion(sketch, depth);
            }
            else
            {
                Extrusion(sketch, depth);
            }
        }

        public ksEntity CreateSketch(ksPart part, short planeType)
        {
            var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var definition = (ksSketchDefinition)sketch.GetDefinition();
            definition.SetPlane(part.GetDefaultEntity(planeType));
            sketch.Create();
            return sketch;
        }

        public void Extrusion(ksEntity sketch, double depth)
        {
            if (_doc3D == null)
            {
                Console.WriteLine("Документ не создан.");
                return;
            }

            var part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);
            var extrusion = (ksEntity)part.NewEntity((short)Obj3dType.o3d_bossExtrusion);
            var extrusionDef = (ksBossExtrusionDefinition)extrusion.GetDefinition();

            extrusionDef.SetSketch(sketch);
            extrusionDef.directionType = (short)Direction_Type.dtNormal;
            extrusionDef.SetSideParam(true, (short)End_Type.etBlind, depth, 0, false);
            extrusion.Create();
        }

        public void CutExtrusion(ksEntity sketch, double depth, bool reverse = false)
        {
            if (_doc3D == null) return;

            var part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);
            var cutExtrusion = (ksEntity)part.NewEntity((short)Obj3dType.o3d_cutExtrusion);
            var cutDefinition = (ksCutExtrusionDefinition)cutExtrusion.GetDefinition();

            cutDefinition.SetSketch(sketch);
            cutDefinition.directionType = (short)(reverse ? Direction_Type.dtReverse : Direction_Type.dtNormal);
            cutDefinition.SetSideParam(true, (short)End_Type.etBlind, depth, 0, false);
            cutExtrusion.Create();
        }

        private ksEntity CreateOffsetPlane(ksPart part, short basePlane, double offset)
        {
            var offsetPlane = (ksEntity)part.NewEntity((short)Obj3dType.o3d_planeOffset);
            var planeDef = (ksPlaneOffsetDefinition)offsetPlane.GetDefinition();

            planeDef.SetPlane(part.GetDefaultEntity(basePlane));
            planeDef.offset = offset;
            planeDef.direction = false;
            offsetPlane.Create();

            return offsetPlane;
        }
        public void SaveFile(string filePath)
        {
            _doc3D.SaveAs(filePath);
        }

        public void CreateFile()
        {
            _doc3D = (ksDocument3D)_kompas.Document3D();
            _doc3D.Create();
        }

        public void OpenFile(string filePath)
        {
            if (_doc3D == null)
            {
                _doc3D = (ksDocument3D)_kompas.Document3D();
            }
            _doc3D.Open(filePath, false);
        }
        public void OpenCAD()
        {
            try
            {
                this._kompas = (KompasObject)Marshal.GetActiveObject("KOMPAS.Application.5");
            }
            catch
            {
                Type kompasType = Type.GetTypeFromProgID("KOMPAS.Application.5");
                this._kompas = (KompasObject)Activator.CreateInstance(kompasType);
            }

            if (this._kompas != null)
            {
                this._kompas.Visible = true;
                this._kompas.ActivateControllerAPI();
            }
        }

        public ksPart GetCurrentPart()
        {
            return (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);
        }

        public RectangleParam CreateRectangleParam(double x, double y, double width, double height)
        {
            var rectParam = (RectangleParam)_kompas.GetParamStruct((short)StructType2DEnum.ko_RectangleParam);
            rectParam.x = x;
            rectParam.y = y;
            rectParam.width = width;
            rectParam.height = height;
            rectParam.style = 1;
            return rectParam;
        }
    }

}
