using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace AdvancedRobotKinematics.maths
{

    /// <summary>
    /// RHS - right hand side transform
    /// </summary>
    public class TransformMatrixBuilder
    {
        private Vector3D rotation;
        private Vector3D position;

        public TransformMatrixBuilder(Vector3D rotation, Vector3D position)
        {
            this.rotation = rotation;
            this.position = position;
        }

        public Transform3D GetEulerTransform()
        {
            double R, P, Y, StartPositionX, StartPositionY, StartPositionZ;
            R = rotation.X;
            P = rotation.Y;
            Y = rotation.Z;
            StartPositionX = position.X;
            StartPositionY = position.Y;
            StartPositionZ = position.Z;
            //Obrót wokół Y
            Matrix3D rotationHeading = new Matrix3D(
               Math.Cos(P), 0, Math.Sin(P), 0,
                0, 1, 0, 0,
               -Math.Sin(P), 0, Math.Cos(P), 0,
                0, 0, 0, 1
            );
            //Obrót wokół Z
            Matrix3D rotationAttitude = new Matrix3D(
                Math.Cos(Y), -Math.Sin(Y), 0, 0,
                Math.Sin(Y), Math.Cos(Y), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
            //Obrót wokół X
            Matrix3D rotationBank = new Matrix3D(
                1, 0, 0, 0,
                0, Math.Cos(R), -Math.Sin(R), 0,
                0, Math.Sin(R), Math.Cos(R), 0,
                0, 0, 0, 1
            );

            Matrix3D translation = new Matrix3D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                StartPositionX, StartPositionY, StartPositionZ, 1
            );

            return new MatrixTransform3D(
                 rotationHeading * rotationAttitude * rotationBank * translation
                );
        }

        public Transform3D GetQuaternionTransform(Matrix3D rotation)
        {
            double  StartPositionX, StartPositionY, StartPositionZ;
            StartPositionX = position.X;
            StartPositionY = position.Y;
            StartPositionZ = position.Z;

            Matrix3D translation = new Matrix3D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                StartPositionX, StartPositionY, StartPositionZ, 1
            );

            return new MatrixTransform3D(
                 rotation * translation
                );
        }
    }
}
