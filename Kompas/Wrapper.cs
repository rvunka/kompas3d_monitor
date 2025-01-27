using System;
using System.Runtime.InteropServices;
using Kompas6API5;
using Kompas6Constants;
using Kompas6Constants3D;

namespace KompasAPIWrapper
{
    //TODO: RSDN
    /// <summary>
    /// Обертка для работы с API Kompas 3D.
    /// Предоставляет методы для создания и модификации 3D-объектов в Kompas.
    /// </summary>
    public class Wrapper
    {
        private KompasObject _kompas;
        private ksDocument3D _doc3D;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Wrapper"/> и открывает Kompas 3D.
        /// </summary>
        public Wrapper()
        {
            OpenCAD();
            CreateFile();
        }

        /// <summary>
        /// Создает линию на текущем эскизе.
        /// </summary>
        /// <param name="x1">Координата X начальной точки.</param>
        /// <param name="y1">Координата Y начальной точки.</param>
        /// <param name="x2">Координата X конечной точки.</param>
        /// <param name="y2">Координата Y конечной точки.</param>
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
        /// <summary>
        /// Создает коробку (прямоугольный параллелепипед) на эскизе.
        /// </summary>
        /// <param name="offset">Смещение от базовой плоскости.</param>
        /// <param name="x">Координата X нижнего левого угла.</param>
        /// <param name="y">Координата Y нижнего левого угла.</param>
        /// <param name="width">Ширина коробки.</param>
        /// <param name="height">Высота коробки.</param>
        /// <param name="depth">Глубина коробки.</param>
        /// <param name="planeType">Тип плоскости для размещения коробки (по умолчанию <see cref="Obj3dType.o3d_planeXOZ"/>).</param>
        public void CreateBox(
            double offset,
            double x,
            double y,
            double width,
            double height,
            double depth,
            short planeType = (short)Obj3dType.o3d_planeXOZ)
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

        /// <summary>
        /// Создает круг на эскизе.
        /// </summary>
        /// <param name="offset">Смещение от базовой плоскости.</param>
        /// <param name="x">Координата X центра круга.</param>
        /// <param name="y">Координата Y центра круга.</param>
        /// <param name="radius">Радиус круга.</param>
        /// <param name="depth">Глубина выдавливания.</param>
        /// <param name="planeType">Тип плоскости для размещения круга (по умолчанию <see cref="Obj3dType.o3d_planeXOZ"/>).</param>
        public void CreateCircle(
            double offset, 
            double x, 
            double y, 
            double radius, 
            double depth, 
            short planeType = (short)Obj3dType.o3d_planeXOZ)
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

        /// <summary>
        /// Создает эскиз на указанной плоскости.
        /// </summary>
        /// <param name="part">Часть, к которой относится эскиз.</param>
        /// <param name="planeType">Тип плоскости для создания эскиза.</param>
        /// <returns>Возвращает созданный эскиз.</returns>
        public ksEntity CreateSketch(ksPart part, short planeType)
        {
            var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var definition = (ksSketchDefinition)sketch.GetDefinition();
            definition.SetPlane(part.GetDefaultEntity(planeType));
            sketch.Create();
            return sketch;
        }

        /// <summary>
        /// Выполняет операцию выдавливания эскиза на заданную глубину.
        /// </summary>
        /// <param name="sketch">Эскиз, который будет выдавлен.</param>
        /// <param name="depth">Глубина выдавливания.</param>
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

        /// <summary>
        /// Выполняет операцию вырезания по эскизу на заданную глубину.
        /// </summary>
        /// <param name="sketch">Эскиз, по которому будет выполнено вырезание.</param>
        /// <param name="depth">Глубина вырезания.</param>
        /// <param name="reverse">Направление вырезания: если <c>true</c>, вырезание будет выполнено в обратном направлении.</param>
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

        /// <summary>
        /// Создает смещенную плоскость относительно базовой плоскости.
        /// </summary
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

        /// <summary>
        /// Создает новый документ 3D-модели.
        /// </summary>
        public void CreateFile()
        {
            _doc3D = (ksDocument3D)_kompas.Document3D();
            _doc3D.Create();
        }

        /// <summary>
        /// Открывает приложение Kompas 3D, если оно не запущено.
        /// Попытка активировать уже запущенное приложение или создать новый экземпляр.
        /// </summary>
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

        /// <summary>
        /// Получает текущую часть (компонент) из документа.
        /// </summary>
        /// <returns>Текущая часть документа как объект типа <see cref="ksPart"/>.</returns>
        public ksPart GetCurrentPart()
        {
            return (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);
        }

        /// <summary>
        /// Создает параметры прямоугольника для использования в эскизе.
        /// </summary>
        /// <param name="x">Координата X для нижнего левого угла прямоугольника.</param>
        /// <param name="y">Координата Y для нижнего левого угла прямоугольника.</param>
        /// <param name="width">Ширина прямоугольника.</param>
        /// <param name="height">Высота прямоугольника.</param>
        /// <returns>Объект <see cref="RectangleParam"/> с заданными параметрами.</returns>
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
