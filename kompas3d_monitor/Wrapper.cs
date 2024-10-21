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
        public void CreateLine(Point point1,  Point point2) { }
        public void CreateBox(Point point1, Point point2) { }
        //public Sketch CreateSketch() { } 
        //public void Extrusion(Sketch sketch, Route route) { }
        public void CreateFile(string fileName) { }
        public void OpenFile(string fileName) { }
        public void OpenCAD() { }

    }
}
