using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using AdvancedRobotKinematics.bases;
using MotionInterpolation.bases;

namespace AdvancedRobotKinematics.robot
{
    public class Frame
    {
        public Vector3D X { get; set; }
        public Vector3D Y { get; set; }
        public Vector3D Z { get; set; }

        public Position P { get; set; }

        public Frame()
        {
            X = new Vector3D(1.0f, 0, 0);
            Y = new Vector3D(0, 1.0f, 0.0f);
            Z = new Vector3D(0, 0, 1.0f);
            P = new Position();
        }

        public Frame(Frame other)
        {
            X = new Vector3D(other.X.X, other.X.Y, other.X.Z);
            Y = new Vector3D(other.Y.X, other.Y.Y, other.Y.Z);
            Z = new Vector3D(other.Z.X, other.Z.Y, other.Z.Z);
            P = new Position(other.P);
        }
    }
}
