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

namespace kompas3d_monitor
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
            sketchEdit.ksLineSeg(x1, y1, x2, y2, 1); // Создаем линию
            definition.EndEdit();

            Console.WriteLine($"Линия построена от ({x1}, {y1}) до ({x2}, {y2}).");
        }

        //TODO: RSDN
        // Метод создания прямоугольника (в виде бокса)
        public void CreateBox(double offset, double x, double y, double width, double height, double depth, short planeType = (short)Obj3dType.o3d_planeXOZ)
        {
            if (_doc3D == null)
            {
                Console.WriteLine("Документ не создан.");
                return;
            }

            // Получаем текущую деталь
            var part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);

            // Создаем эскиз
            var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var definition = (ksSketchDefinition)sketch.GetDefinition();

            // Устанавливаем плоскость для эскиза (XOZ, YOZ или XOY)
            definition.SetPlane(part.GetDefaultEntity(planeType));
            sketch.Create();

            // Задаем параметры прямоугольника
            var rectParam = (RectangleParam)_kompas.GetParamStruct((short)StructType2DEnum.ko_RectangleParam);
            rectParam.x = x;
            rectParam.y = y;
            rectParam.width = width;
            rectParam.height = height;
            rectParam.style = 1;  // Сплошная линия

            // Редактируем эскиз и строим прямоугольник
            var sketchEdit = (ksDocument2D)definition.BeginEdit();
            sketchEdit.ksRectangle(rectParam, 0);  // 0 – строить от угла
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

            Console.WriteLine($"Коробка построена: ширина {width}, высота {height}, глубина {depth} на плоскости {(Obj3dType)planeType}.");
        }

        public ksEntity CreateSketch(ksPart part, short planeType)
        {
            var sketch = (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            var definition = (ksSketchDefinition)sketch.GetDefinition();
            definition.SetPlane(part.GetDefaultEntity(planeType));
            sketch.Create();
            return sketch;
        }

        // Метод выдавливания эскиза
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

            // Привязываем эскиз к выдавливанию
            extrusionDef.SetSketch(sketch);

            // Устанавливаем направление всегда вверх (dtNormal)
            extrusionDef.directionType = (short)Direction_Type.dtNormal;

            // Выдавливаем на заданную глубину
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

            Console.WriteLine($"Вырез выполнен на глубину {depth}. Направление: {(reverse ? "Вниз" : "Вверх")}");
        }

        private ksEntity CreateOffsetPlane(ksPart part, short basePlane, double offset)
        {
            var offsetPlane = (ksEntity)part.NewEntity((short)Obj3dType.o3d_planeOffset);
            var planeDef = (ksPlaneOffsetDefinition)offsetPlane.GetDefinition();

            planeDef.SetPlane(part.GetDefaultEntity(basePlane));  // Базовая плоскость
            planeDef.offset = offset;
            planeDef.direction = false;  // Смещаем в обратную сторону
            offsetPlane.Create();

            Console.WriteLine($"Смещенная плоскость создана на расстоянии {offset}.");
            return offsetPlane;
        }

        // Метод для сохранения файла
        public void SaveFile(string filePath)
        {
            _doc3D.SaveAs(filePath);
            Console.WriteLine($"Файл сохранен по пути: {filePath}");
        }

        // Создание нового файла
        public void CreateFile()
        {
            _doc3D = (ksDocument3D)_kompas.Document3D();
            _doc3D.Create();
        }

        // Открытие существующего файла
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
                // Попытка подключения к существующему процессу Kompas3D
                this._kompas = (KompasObject)Marshal.GetActiveObject("KOMPAS.Application.5");
                Console.WriteLine("Kompas3D уже запущен.");
            }
            catch
            {
                // Если процесс не найден, создаем новый экземпляр
                Type kompasType = Type.GetTypeFromProgID("KOMPAS.Application.5");
                this._kompas = (KompasObject)Activator.CreateInstance(kompasType);
                Console.WriteLine("Запущен новый экземпляр Kompas3D.");
            }

            if (this._kompas != null)
            {
                // Делаем окно приложения видимым
                this._kompas.Visible = true;
                this._kompas.ActivateControllerAPI();
                Console.WriteLine("Kompas3D успешно запущен и доступен.");
            }
            else
            {
                Console.WriteLine("Не удалось запустить Kompas3D.");
            }

            Console.ReadLine();
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
