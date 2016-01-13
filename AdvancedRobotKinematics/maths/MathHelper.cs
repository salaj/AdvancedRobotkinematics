using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using MotionInterpolation.bases;

namespace MotionInterpolation.maths
{
    public class MathHelper
    {
        public void EulerToRadian(ref double Angle)
        {
           Angle = Math.PI * 2.0f / 360.0f * Angle;
        }

        public void RadianToEuler(ref double Angle)
        {
            Angle = 360.0f/(Math.PI*2.0f)*Angle;
        }

        public double Angle(Vector3D v, Vector3D w)
        {
            double denominator = (v.Length*w.Length);

            return Math.Atan2(Vector3D.CrossProduct(v, w).Length, Vector3D.DotProduct(v, w));
        }
    }
}
