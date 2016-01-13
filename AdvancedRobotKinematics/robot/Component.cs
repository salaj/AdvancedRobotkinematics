using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace AdvancedRobotKinematics.robot
{
    public class Component
    {
        static double tubeDiameter = 0.4;
        public TubeVisual3D Tube { get; set; }
        static Random r = new Random();

        public Component()
        {
            Tube = new TubeVisual3D();
            Tube.Path = new Point3DCollection();
            Tube.Path.Add(new Point3D(-15, 0, 0));
            Tube.Path.Add(new Point3D(15, 0, 0));
            Tube.Diameter = tubeDiameter;
            Tube.Fill = new SolidColorBrush(Color.FromRgb((byte)(r.Next(0, 255)), (byte)(r.Next(0, 255)), (byte)(r.Next(0, 255))));
            Tube.IsPathClosed = false;
        }

        public void Refresh()
        {
            Tube.Path[0] = new Point3D(Begin.Frame.P.X, Begin.Frame.P.Y, Begin.Frame.P.Z);
            Tube.Path[1] = new Point3D(End.Frame.P.X, End.Frame.P.Y, End.Frame.P.Z);
        }
        public Joint Begin { get; set; }
        public Joint End { get; set; }
        public double Length { get; set; }
    }
}
