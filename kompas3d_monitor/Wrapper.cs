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
