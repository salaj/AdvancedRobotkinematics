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

    public class Joint
    {
        public Frame Frame { get; set; }
        public int Id;

        private static int id = 0;
        private TruncatedConeVisual3D cyllinder;

        private double cyllinderHeight = 0.5;

        public Joint()
        {
            this.Frame = new Frame();
            Id = id++;
        }

        public void AddCyllinder(HelixViewport3D left)
        {
            cyllinder = new TruncatedConeVisual3D()
            {
                Fill = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
                Origin = new Point3D(0, 0, 0),
                BaseRadius = Component.TubeDiameter / 2,
                Height = cyllinderHeight,
                TopRadius = Component.TubeDiameter / 2
            };

            left.Children.Add(cyllinder);
        }

        public void UpdateNormal(Vector3D normal)
        {
            if (cyllinder == null)
                return;
            cyllinder.Normal = normal;
        }

        public void Refresh()
        {
            if (cyllinder == null)
                return;
            cyllinder.Origin = new Point3D(Frame.P.X, Frame.P.Y, Frame.P.Z);
            cyllinder.Origin -= cyllinder.Normal * (cyllinderHeight / 2);
        }
    }
}
