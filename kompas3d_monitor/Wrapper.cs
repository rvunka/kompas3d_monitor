﻿using System;
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
using static System.Windows.Forms.AxHost;

namespace kompas3d_monitor
{
    internal class Wrapper
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
            definition.SetPlane(part.GetDefaultEntity((short)Obj3dType.o3d_planeXOY));
            sketch.Create();

            var sketchEdit = (ksDocument2D)definition.BeginEdit();
            sketchEdit.ksLineSeg(x1, y1, x2, y2, 1); // Создаем линию
            definition.EndEdit();

            Console.WriteLine($"Линия построена от ({x1}, {y1}) до ({x2}, {y2}).");
        }

        // Метод создания прямоугольника (в виде бокса)
        public void CreateBox(double x, double y, double width, double height, double depth)
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

            // Устанавливаем плоскость для эскиза (XOY)
            definition.SetPlane(part.GetDefaultEntity((short)Obj3dType.o3d_planeXOY));
            sketch.Create();

            // Создаем структуру параметров для прямоугольника
            var rectParam = (RectangleParam)_kompas.GetParamStruct((short)StructType2DEnum.ko_RectangleParam);
            rectParam.x = x;
            rectParam.y = y;
            rectParam.width = width;
            rectParam.height = height;
            rectParam.style = 1; // Сплошная линия

            // Редактируем эскиз и строим прямоугольник
            var sketchEdit = (ksDocument2D)definition.BeginEdit();
            sketchEdit.ksRectangle(rectParam, 0); // 0 – строить от угла
            definition.EndEdit();

            // Выдавливаем прямоугольник для создания 3D-бокса
            var extrusion = (ksEntity)part.NewEntity((short)Obj3dType.o3d_bossExtrusion);
            var extrusionDef = (ksBossExtrusionDefinition)extrusion.GetDefinition();
            extrusionDef.directionType = (short)Direction_Type.dtNormal;
            extrusionDef.SetSketch(sketch);
            extrusionDef.SetSideParam(true, (short)End_Type.etBlind, depth, 0, false);
            extrusion.Create();

            Console.WriteLine($"Бокс построен: ширина {width}, высота {height}, глубина {depth}.");
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
            if (_doc3D == null) return;

            var part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);
            var extrusion = (ksEntity)part.NewEntity((short)Obj3dType.o3d_bossExtrusion);
            var extrusionDef = (ksBossExtrusionDefinition)extrusion.GetDefinition();

            extrusionDef.SetSketch(sketch);
            extrusionDef.directionType = (short)Direction_Type.dtNormal;
            extrusionDef.SetSideParam(true, (short)End_Type.etBlind, depth, 0, false);
            extrusion.Create();
            Console.WriteLine($"Эскиз выдавлен на глубину {depth}.");
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

    }
}
