using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using AdvancedRobotKinematics.bases;
using AdvancedRobotKinematics.maths;
using HelixToolkit.Wpf;
using MotionInterpolation.bases;
using MotionInterpolation.interpolators;
using MotionInterpolation.maths;

namespace AdvancedRobotKinematics.robot
{
    public enum InterpolationType
    {
        Configuration,
        InternalPositions
    }

    public class Robot
    {
        private int numberOfComponents = 5;
        public Component[] components;
        public Joint[] joints;

        public static InterpolationType InterpolationType = InterpolationType.Configuration;

        /// <summary>
        /// Configuration space parameters
        /// </summary>
        private double a1, a2, q2, a3, a4, a5;

        private ModelVisual3D startFrameEuler;
        private ModelVisual3D endFrameEuler;

        private Configuration startConfiguration;
        private Configuration endConfiguration;

        private Vector3D[] startInternalPositions;
        private Vector3D[] endInternalPositions;

        public Robot(HelixViewport3D left, ModelVisual3D FrameStartEuler, ModelVisual3D FrameEndEuler)
        {

            this.startFrameEuler = FrameStartEuler;
            this.endFrameEuler = FrameEndEuler;

            Initialize(left);
        }

        public void Initialize(HelixViewport3D left)
        {
            int n = numberOfComponents;
            components = new Component[n];
            joints = new Joint[n + 1];
            for (int i = 0; i < n; i++)
            {
                components[i] = new Component();
            }
            for (int i = 0; i < n + 1; i++)
            {
                joints[i] = new Joint();
            }
            for (int i = 0; i < n; i++)
            {
                components[i].Begin = joints[i];
                components[i].End = joints[i + 1];
                components[i].Length = 5.0f;
                left.Children.Add(components[i].Tube);
            }

            components[2].Tube.Fill = Brushes.Red;
            //components[0].Length = 0.0f;
            //components[1].Length = 5.0f;
            //components[3].Length = 2.0f;
            //components[4].Length = 1.0f;

            //Update();
        }

        public void SetupEffector(Position position, Rotation rotation)
        {
            joints[5].Frame.P = position;
            Quaternion q = new Quaternion();
            Singleton<EulerToQuaternionConverter>.Instance.Convert(rotation.R, rotation.P, rotation.Y, ref q);
            Matrix3D m = Singleton<EulerToQuaternionConverter>.Instance.BuildMatrix3DFromQuaternion(q);
            joints[5].Frame.X = new Vector3D(m.M11, m.M12, m.M13);
            joints[5].Frame.Y = new Vector3D(m.M21, m.M22, m.M23);
            joints[5].Frame.Z = new Vector3D(m.M31, m.M32, m.M33);
            joints[5].Frame.X.Normalize();
            joints[5].Frame.Y.Normalize();
            joints[5].Frame.Z.Normalize();
        }

        public void SetupEffector(Position position, Quaternion quaternion)
        {
            joints[5].Frame.P = position;
            Matrix3D m = Singleton<EulerToQuaternionConverter>.Instance.BuildMatrix3DFromQuaternion(quaternion);
            joints[5].Frame.X = new Vector3D(m.M11, m.M12, m.M13);
            joints[5].Frame.Y = new Vector3D(m.M21, m.M22, m.M23);
            joints[5].Frame.Z = new Vector3D(m.M31, m.M32, m.M33);
            joints[5].Frame.X.Normalize();
            joints[5].Frame.Y.Normalize();
            joints[5].Frame.Z.Normalize();
        }

        private void calculatePositions()
        {
            //Frame F0 has all vars set to 0

            //p1 = p0
            joints[1].Frame.P = new Position(joints[0].Frame.P);
            //p2 = p0 + l1 * z0
            joints[2].Frame.P = new Position(joints[0].Frame.P.Value + components[1].Length * joints[0].Frame.Z);
            //p4 = p5 - l4 * x5
            joints[4].Frame.P = new Position(joints[5].Frame.P.Value - components[4].Length * joints[5].Frame.X);

            calculateP3();
        }

        public void UpdatePositionsVisual()
        {
            for (int i = 0; i < numberOfComponents; i++)
            {
                components[i].Refresh();
            }
        }

        private void calculateAnglesGeometricMethod()
        {

            //n = (p4 - p0) x (p2 - p0)
            Vector3D n = Vector3D.CrossProduct((joints[4].Frame.P.Value - joints[0].Frame.P.Value),
                    (joints[2].Frame.P.Value - joints[0].Frame.P.Value));
            //n = (p4 - p0) x (p2 - p0) / |(p4 - p0) x (p2 - p0)|
            n.Normalize();
            Vector3D x5 = joints[5].Frame.X;
            Vector3D z5 = joints[5].Frame.Z;


            a1 = Math.Atan2(joints[4].Frame.P.Y, joints[4].Frame.P.X);
            a2 = Singleton<MathHelper>.Instance.Angle(joints[2].Frame.P.Value - joints[0].Frame.P.Value, joints[3].Frame.P.Value - joints[2].Frame.P.Value) - Math.PI / 2.0f;
            q2 = (joints[3].Frame.P.Value - joints[2].Frame.P.Value).Length;
            a3 = Singleton<MathHelper>.Instance.Angle(joints[3].Frame.P.Value - joints[2].Frame.P.Value, joints[4].Frame.P.Value - joints[3].Frame.P.Value) - Math.PI / 2.0f;
            a4 = Singleton<MathHelper>.Instance.Angle(n, x5) + Math.PI / 2.0f;
            a5 = Singleton<MathHelper>.Instance.Angle(joints[3].Frame.P.Value - joints[4].Frame.P.Value, z5);

            convertAnglesToEuler();
        }

        private void calcaulateAnglesAlgebraicMethod()
        {
            Vector3D forward, right, up;

            forward = joints[5].Frame.X;
            right = joints[5].Frame.Y;
            up = joints[5].Frame.Z;

            a1 = Math.Atan2(joints[5].Frame.P.Y - components[4].Length * forward.Y, joints[5].Frame.P.X - components[4].Length * forward.X);
            double c1 = Math.Cos(a1);
            double s1 = Math.Sin(a1);
            a4 = Math.Asin(c1 * forward.Y - s1 * forward.X);
            double c4 = Math.Cos(a4);
            double s4 = Math.Sin(a4);
            a5 = Math.Atan2((s1 * up.X - c1 * up.Y),
                (c1 * right.Y - s1 * right.X));
            double c5 = Math.Cos(a5);
            double s5 = Math.Sin(a5);
            a2 =
                Math.Atan2(
                    -c1 * c4 * (joints[5].Frame.P.Z - components[4].Length * forward.Z - components[1].Length) -
                    components[3].Length * (forward.X + s1 * s4),
                    c4 * (joints[5].Frame.P.X - components[4].Length * forward.X) -
                    c1 * components[3].Length * forward.Z);
            double c2 = Math.Cos(a2);
            double s2 = Math.Sin(a2);

            q2 = (c4 * (joints[5].Frame.P.X - components[4].Length * forward.X) -
                  c1 * components[3].Length * forward.Z) / (c1 * c2 * c4);

            //q2 = (joints[5].Frame.P.X - components[4].Length * forward.X) / (c1 * c2) -
            //      ( components[3].Length * forward.Z) / (c2 * c4);

            double a23 = Math.Atan2(-forward.Z / c4, (forward.X + s1 * s4) / (c1 * c4));
            a3 = a23 - a2;

            convertAnglesToEuler();
        }

        private void convertAnglesToEuler()
        {
            Singleton<MathHelper>.Instance.RadianToEuler(ref a1);
            Singleton<MathHelper>.Instance.RadianToEuler(ref a2);
            Singleton<MathHelper>.Instance.RadianToEuler(ref a3);
            Singleton<MathHelper>.Instance.RadianToEuler(ref a4);
            Singleton<MathHelper>.Instance.RadianToEuler(ref a5);
        }

        private Vector3D calculateZ4Vector(Vector3D n, Vector3D x5)
        {
            var z4 = new Vector3D();
            double a = n.Y - x5.Y / x5.X * n.X;
            double b = n.Z - x5.Z / x5.X * n.X;

            if (a != 0)
            {
                var first = ((b / a * x5.Y - x5.Z) / x5.X);
                var second = b / a;
                var third = 1;
                var val = first * first + second * second + third * third;
                z4.Z = Math.Sqrt(1 / val);
                z4.X = z4.Z * first;
                z4.Y = -z4.Z * second;
            }
            else if (b != 0)
            {
                var first = ((a / b * x5.Z - x5.Y) / x5.X);
                var second = a / b;
                var third = 1;
                var val = first * first + second * second + third * third;
                z4.Y = Math.Sqrt(1 / val);
                z4.X = z4.Y * first;
                z4.Z = -z4.Y * second;
            }
            else
            {
                int c = 10;
            }
            return z4;
        }

        private Vector3D handleNotPararel(Vector3D n, Vector3D x5)
        {
            Vector3D z4 = new Vector3D();
            if (x5.X != 0)
            {
                z4 = calculateZ4Vector(n, x5);
            }
            else if (x5.Y != 0)
            {
                z4 = calculateZ4Vector(n, new Vector3D(x5.Y, x5.X, x5.Z));
            }
            else if (x5.Z != 0)
            {
                z4 = calculateZ4Vector(n, new Vector3D(x5.Z, x5.X, x5.Y));
                z4 = new Vector3D(z4.Y, z4.Z, z4.X);
            }
            //p3 = p4 +- l3*z4
            //SECOND SOLUTION UNHANDLED YET
            var prevP3 = joints[3].Frame.P.Value;
            var p3First = joints[4].Frame.P.Value + components[3].Length * z4;
            var p3Second = joints[4].Frame.P.Value - components[3].Length * z4;
            Vector3D p3 = new Vector3D();
            if ((p3First - prevP3).LengthSquared < (p3Second - prevP3).LengthSquared)
            {
                p3 = p3First;
            }
            else
            {
                p3 = p3Second;
            }
            return p3;

            //return joints[4].Frame.P.Value + components[3].Length * z4;
        }

        private void calculateP3()
        {
            //n = (p4 - p0) x (p2 - p0)
            Vector3D n = Vector3D.CrossProduct((joints[4].Frame.P.Value - joints[0].Frame.P.Value),
                (joints[2].Frame.P.Value - joints[0].Frame.P.Value));
            //Vector3D n = Vector3D.CrossProduct((joints[2].Frame.P.Value - joints[0].Frame.P.Value),(joints[4].Frame.P.Value - joints[0].Frame.P.Value));
            //n = (p4 - p0) x (p2 - p0) / |(p4 - p0) x (p2 - p0)|
            n.Normalize();
            Vector3D x5 = joints[5].Frame.X;
            Vector3D p3;
            if (checkIfTwoVectorsPararel(n, x5))
            {
                //p3 = handlePararel(x5);
                throw new ConfigurationFailureException();
            }
            else
            {
                p3 = handleNotPararel(n, x5);
            }
            joints[3].Frame.P = new Position(p3);
        }

        private bool checkIfTwoVectorsPararel(Vector3D u, Vector3D v)
        {
            Vector3D w = Vector3D.CrossProduct(u, v);

            if (w == new Vector3D(0, 0, 0) || Double.IsNaN(w.X) || Double.IsNaN(w.Y) || Double.IsNaN(w.Z))
                return true;
            return false;
        }

        private void TranslateInDirection(Vector3D dir, double val, ref Transform3DGroup eulerTransformGroup)
        {
            eulerTransformGroup.Children.Add(new TranslateTransform3D(val * dir.X, 0, 0));
            eulerTransformGroup.Children.Add(new TranslateTransform3D(0, val * dir.Y, 0));
            eulerTransformGroup.Children.Add(new TranslateTransform3D(0, 0, val * dir.Z));
        }

        private void RotateInDirection(Vector3D dir, Vector3D pos, double val, ref Transform3DGroup eulerTransformGroup)
        {
            eulerTransformGroup.Children.Add(new TranslateTransform3D(-pos.X, -pos.Y, -pos.Z));
            eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(dir, val)));
            eulerTransformGroup.Children.Add(new TranslateTransform3D(pos.X, pos.Y, pos.Z));
        }

        private Vector3D getX(Transform3DGroup eulerTransformGroup)
        {
            return new Vector3D(eulerTransformGroup.Value.M11, eulerTransformGroup.Value.M12, eulerTransformGroup.Value.M13);
        }
        private Vector3D getY(Transform3DGroup eulerTransformGroup)
        {
            return new Vector3D(eulerTransformGroup.Value.M21, eulerTransformGroup.Value.M22, eulerTransformGroup.Value.M23);
        }
        private Vector3D getZ(Transform3DGroup eulerTransformGroup)
        {
            return new Vector3D(eulerTransformGroup.Value.M31, eulerTransformGroup.Value.M32, eulerTransformGroup.Value.M33);
        }

        private Vector3D getPos(Transform3DGroup eulerTransformGroup)
        {
            return new Vector3D(eulerTransformGroup.Value.OffsetX, eulerTransformGroup.Value.OffsetY, eulerTransformGroup.Value.OffsetZ);
        }

        private void SetupConfiguration(bool forInitialFrame)
        {
            var eulerTransformGroup = new Transform3DGroup();
            //var quaternionTransformGroup = new Transform3DGroup();

            Vector3D dir, pos;

            dir = getZ(eulerTransformGroup);
            pos = getPos(eulerTransformGroup);
            RotateInDirection(dir, pos, a1, ref eulerTransformGroup);

            dir = getZ(eulerTransformGroup);
            TranslateInDirection(dir, components[1].Length, ref eulerTransformGroup);

            dir = getY(eulerTransformGroup);
            pos = getPos(eulerTransformGroup);
            RotateInDirection(dir, pos, a2, ref eulerTransformGroup);

            dir = getX(eulerTransformGroup);
            TranslateInDirection(dir, q2, ref eulerTransformGroup);

            dir = getY(eulerTransformGroup);
            pos = getPos(eulerTransformGroup);
            RotateInDirection(dir, pos, a3, ref eulerTransformGroup);

            dir = getZ(eulerTransformGroup);
            TranslateInDirection(dir, -components[3].Length, ref eulerTransformGroup);

            dir = getZ(eulerTransformGroup);
            pos = getPos(eulerTransformGroup);
            RotateInDirection(dir, pos, a4, ref eulerTransformGroup);

            dir = getX(eulerTransformGroup);
            TranslateInDirection(dir, components[4].Length, ref eulerTransformGroup);

            dir = getX(eulerTransformGroup);
            pos = getPos(eulerTransformGroup);
            RotateInDirection(dir, pos, a5, ref eulerTransformGroup);

            //ORIGINAL

            //eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(zDirection, a1)));
            //eulerTransformGroup.Children.Add(new TranslateTransform3D(0, 0, components[1].Length));
            //eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(yDirection, a2)));
            //eulerTransformGroup.Children.Add(new TranslateTransform3D(q2, 0, 0));
            //eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(yDirection, a3)));
            //eulerTransformGroup.Children.Add(new TranslateTransform3D(0, 0, -components[3].Length));
            //eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(zDirection, a4)));
            //eulerTransformGroup.Children.Add(new TranslateTransform3D(components[4].Length, 0, 0));
            //eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(xDirection, a5)));


            if (forInitialFrame)
            {
                startConfiguration = new Configuration() { A1 = a1, A2 = a2, Q2 = q2, A3 = a3, A4 = a4, A5 = a5 };
                startInternalPositions = new Vector3D[6]
                {
                    joints[0].Frame.P.Value, joints[1].Frame.P.Value, joints[2].Frame.P.Value, joints[3].Frame.P.Value,
                    joints[4].Frame.P.Value, joints[5].Frame.P.Value
                };
                startFrameEuler.Transform = eulerTransformGroup;
            }
            else
            {
                endConfiguration = new Configuration() { A1 = a1, A2 = a2, Q2 = q2, A3 = a3, A4 = a4, A5 = a5 };
                endInternalPositions = new Vector3D[6]
                {
                    joints[0].Frame.P.Value, joints[1].Frame.P.Value, joints[2].Frame.P.Value, joints[3].Frame.P.Value,
                    joints[4].Frame.P.Value, joints[5].Frame.P.Value
                };
                endFrameEuler.Transform = eulerTransformGroup;
            }
        }

        public void calculateConfiguration(bool forInitialFrame)
        {
            calculatePositions();
            if (forInitialFrame)
                UpdatePositionsVisual();
            //calculateAnglesGeometricMethod();
            calcaulateAnglesAlgebraicMethod();
            SetupConfiguration(forInitialFrame);
        }

        private void getInterpolatedPositions(Configuration interpolatedConfiguration)
        {
            var eulerTransformGroup = new Transform3DGroup();

            Position p0, p1, p2, p3, p4, p5;
            p0 = new Position(0, 0, 0);
            p1 = new Position(p0);

            Vector3D dir, pos;

            dir = getZ(eulerTransformGroup);
            pos = getPos(eulerTransformGroup);
            RotateInDirection(dir, pos, interpolatedConfiguration.A1, ref eulerTransformGroup);

            dir = getZ(eulerTransformGroup);
            TranslateInDirection(dir, components[1].Length, ref eulerTransformGroup);

            p2 = new Position(eulerTransformGroup.Value.OffsetX, eulerTransformGroup.Value.OffsetY, eulerTransformGroup.Value.OffsetZ);

            dir = getY(eulerTransformGroup);
            pos = getPos(eulerTransformGroup);
            RotateInDirection(dir, pos, interpolatedConfiguration.A2, ref eulerTransformGroup);

            dir = getX(eulerTransformGroup);
            TranslateInDirection(dir, interpolatedConfiguration.Q2, ref eulerTransformGroup);

            p3 = new Position(eulerTransformGroup.Value.OffsetX, eulerTransformGroup.Value.OffsetY, eulerTransformGroup.Value.OffsetZ);

            dir = getY(eulerTransformGroup);
            pos = getPos(eulerTransformGroup);
            RotateInDirection(dir, pos, interpolatedConfiguration.A3, ref eulerTransformGroup);

            dir = getZ(eulerTransformGroup);
            TranslateInDirection(dir, -components[3].Length, ref eulerTransformGroup);

            p4 = new Position(eulerTransformGroup.Value.OffsetX, eulerTransformGroup.Value.OffsetY, eulerTransformGroup.Value.OffsetZ);

            dir = getZ(eulerTransformGroup);
            pos = getPos(eulerTransformGroup);
            RotateInDirection(dir, pos, interpolatedConfiguration.A4, ref eulerTransformGroup);

            dir = getX(eulerTransformGroup);
            TranslateInDirection(dir, components[4].Length, ref eulerTransformGroup);

            p5 = new Position(eulerTransformGroup.Value.OffsetX, eulerTransformGroup.Value.OffsetY, eulerTransformGroup.Value.OffsetZ);

            dir = getX(eulerTransformGroup);
            pos = getPos(eulerTransformGroup);
            RotateInDirection(dir, pos, interpolatedConfiguration.A5, ref eulerTransformGroup);


            joints[0].Frame.P = p0;
            joints[1].Frame.P = p1;
            joints[2].Frame.P = p2;
            joints[3].Frame.P = p3;
            joints[4].Frame.P = p4;
            joints[5].Frame.P = p5;

            startFrameEuler.Transform = eulerTransformGroup;
        }

        public void InterpolatePositionsLeft(RealTimeInterpolator realTimeInterpolator, double normalizedTime)
        {
            if (InterpolationType == InterpolationType.Configuration)
            {
                var interpolatedConfiguration = new Configuration
                {
                    A1 = realTimeInterpolator.GetValue(normalizedTime, startConfiguration.A1, endConfiguration.A1),
                    A2 = realTimeInterpolator.GetValue(normalizedTime, startConfiguration.A2, endConfiguration.A2),
                    Q2 = realTimeInterpolator.GetValue(normalizedTime, startConfiguration.Q2, endConfiguration.Q2),
                    A3 = realTimeInterpolator.GetValue(normalizedTime, startConfiguration.A3, endConfiguration.A3),
                    A4 = realTimeInterpolator.GetValue(normalizedTime, startConfiguration.A4, endConfiguration.A4),
                    A5 = realTimeInterpolator.GetValue(normalizedTime, startConfiguration.A5, endConfiguration.A5)
                };
                getInterpolatedPositions(interpolatedConfiguration);
            }
            else if (InterpolationType == InterpolationType.InternalPositions)
            {
                joints[0].Frame.P.Value = realTimeInterpolator.GetVectorValue(normalizedTime, startInternalPositions[0],
                    endInternalPositions[0]);
                joints[1].Frame.P.Value = realTimeInterpolator.GetVectorValue(normalizedTime, startInternalPositions[1],
                    endInternalPositions[1]);
                joints[2].Frame.P.Value = realTimeInterpolator.GetVectorValue(normalizedTime, startInternalPositions[2],
                    endInternalPositions[2]);
                joints[3].Frame.P.Value = realTimeInterpolator.GetVectorValue(normalizedTime, startInternalPositions[3],
                    endInternalPositions[3]);
                joints[4].Frame.P.Value = realTimeInterpolator.GetVectorValue(normalizedTime, startInternalPositions[4],
                    endInternalPositions[4]);
                joints[5].Frame.P.Value = realTimeInterpolator.GetVectorValue(normalizedTime, startInternalPositions[5],
                    endInternalPositions[5]);
            }
            UpdatePositionsVisual();
        }
    }
}
