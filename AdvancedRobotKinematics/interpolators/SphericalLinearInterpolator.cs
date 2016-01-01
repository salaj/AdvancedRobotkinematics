using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace AdvancedRobotKinematics.interpolators
{
    public class SphericalLinearInterpolator
    {
        private Quaternion startQuaternion;
        private Quaternion endQuaternion;

        public void SetupInterpolator(Quaternion startQuaternion,Quaternion endQuaternion)
        {
            this.startQuaternion = startQuaternion;
            this.endQuaternion = endQuaternion;
        }

        public void CalculateCurrentQuaternion(ref Quaternion currentQuaternion, double timeFactor)
        {
                double dotProduct = startQuaternion.X * endQuaternion.Y + startQuaternion.Y * endQuaternion.Y
                    + startQuaternion.Z * endQuaternion.Z + startQuaternion.W * endQuaternion.W;

                var a = Math.Acos(dotProduct);
                a = Math.Abs(a);

                var firstFactor = Math.Sin((1 - timeFactor) * a) / Math.Sin(a);
                var secondFactor = Math.Sin(timeFactor * a) / Math.Sin(a);
                var x = firstFactor * startQuaternion.X + secondFactor * endQuaternion.X;
                var y = firstFactor * startQuaternion.Y + secondFactor * endQuaternion.Y;
                var z = firstFactor * startQuaternion.Z + secondFactor * endQuaternion.Z;
                var w = firstFactor * startQuaternion.W + secondFactor * endQuaternion.W;
                currentQuaternion = new Quaternion(x, y, z, w);
                currentQuaternion.Normalize();
        }
    }
}
