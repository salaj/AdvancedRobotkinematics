using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using AdvancedRobotKinematics.bases;
using MotionInterpolation.bases;

namespace AdvancedRobotKinematics.interpolators
{
    public class LinearInterpolator
    {
        private double StartAngleR;
        private double StartAngleP;
        private double StartAngleY;
        private double EndAngleR;
        private double EndAngleP;
        private double EndAngleY;

        private double StartPositionX;
        private double StartPositionY;
        private double StartPositionZ;
        private double EndPositionX;
        private double EndPositionY;
        private double EndPositionZ;

        private Quaternion startQuaternion;
        private Quaternion endQuaternion;

        public void SetupInterpolator(double StartAngleR, double StartAngleP, double StartAngleY, double EndAngleR,
            double EndAngleP, double EndAngleY, double StartPositionX,
         double StartPositionY,
         double StartPositionZ,
         double EndPositionX,
         double EndPositionY,
         double EndPositionZ,
            Quaternion startQuaternion,
            Quaternion endQuaternion
            )
        {
            this.StartAngleP = StartAngleP;
            this.StartAngleR = StartAngleR;
            this.StartAngleY = StartAngleY;

            this.EndAngleR = EndAngleR;
            this.EndAngleP = EndAngleP;
            this.EndAngleY = EndAngleY;

            this.StartPositionX = StartPositionX;
            this.StartPositionY = StartPositionY;
            this.StartPositionZ = StartPositionZ;

            this.EndPositionX = EndPositionX;
            this.EndPositionY = EndPositionY;
            this.EndPositionZ = EndPositionZ;

            this.startQuaternion = startQuaternion;
            this.endQuaternion = endQuaternion;
        }

        public void CalculateCurrentAngle(ref Rotation currentRotation, double normalizedTime)
        {
            currentRotation.R = StartAngleR + normalizedTime * (EndAngleR - StartAngleR);
            currentRotation.P = StartAngleP + normalizedTime * (EndAngleP - StartAngleP);
            currentRotation.Y = StartAngleY + normalizedTime * (EndAngleY - StartAngleY);
        }

        public void CalculateCurrentPosition(ref Position currentPosition, double normalizedTime)
        {
            currentPosition.X = StartPositionX + normalizedTime * (EndPositionX - StartPositionX);
            currentPosition.Y = StartPositionY + normalizedTime * (EndPositionY - StartPositionY);
            currentPosition.Z = StartPositionZ + normalizedTime * (EndPositionZ - StartPositionZ);
        }

        public void CalculateCurrentQuaternion(ref Quaternion currentQuaternion, double timeFactor)
        {
                var x = startQuaternion.X * (1 - timeFactor) + endQuaternion.X * timeFactor;
                var y = startQuaternion.Y * (1 - timeFactor) + endQuaternion.Y * timeFactor;
                var z = startQuaternion.Z * (1 - timeFactor) + endQuaternion.Z * timeFactor;
                var w = startQuaternion.W * (1 - timeFactor) + endQuaternion.W * timeFactor;
                currentQuaternion = new Quaternion(x, y, z, w);
                currentQuaternion.Normalize();
        }
    }
}
