using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kompas6API5;
using Kompas6Constants;
using Kompas6Constants3D;

namespace kompas3d_monitor
{
    internal class Wrapper
    {
        private KompasObject _kompas;

        public Wrapper()
        {
            OpenCAD();
        }

        public void CreateLine(Point point1,  Point point2) 
        {
            
        }
        public void CreateBox(Point point1, Point point2) { }
        public void CreateFile(string fileName) { }
        public void OpenFile(string fileName) { }
        public void OpenCAD() 
        {
            if (_kompas == null)
            {
                try
                {
                    // Подключение к уже запущенному Компас-3D
                    Type kompasType = Type.GetTypeFromProgID("KOMPAS.Application.5");
                    _kompas = (KompasObject)Activator.CreateInstance(kompasType);

                    if (_kompas != null)
                    {
                        _kompas.Visible = true; // Делаем Компас-3D видимым
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка подключения к Компас-3D: {ex.Message}");
                }
            }
        }

    }
}
