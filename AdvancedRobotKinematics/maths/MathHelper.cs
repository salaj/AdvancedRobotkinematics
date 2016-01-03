using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace MotionInterpolation.maths
{
    public class MathHelper
    {
        public void EulerToRadian(ref double Angle)
        {
           Angle = Math.PI * 2.0f / 360.0f * Angle;
        }

        public Vector3D Normalize(Vector3D v)
        {
            return v / Vector3DLen(v);
        }

        public double Vector3DLen(Vector3D v)
        {
            return Math.Sqrt(Vector3D.DotProduct(v, v));
        }

        public double Angle(Vector3D v, Vector3D w)
        {
            return Math.Acos(Vector3D.DotProduct(v, w)/(Vector3DLen(v)*Vector3DLen(w)));
        }
    }
}
