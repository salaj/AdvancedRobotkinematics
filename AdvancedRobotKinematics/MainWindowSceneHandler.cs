using System.Windows.Shapes;
using AdvancedRobotKinematics.bases;
using AdvancedRobotKinematics.interpolators;
using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Media.Media3D;
using MotionInterpolation.bases;

namespace AdvancedRobotKinematics
{
    public partial class MainWindow
    {
        private Vector3D xDirection;
        private Vector3D yDirection;
        private Vector3D zDirection;
        CombinedManipulator frameEuler;
        CombinedManipulator frameQuaternion;

        private void InitializeScene()
        {
            linearInterpolator = new LinearInterpolator();
            sphericalLinearInterpolator = new SphericalLinearInterpolator();
            const double maxVal = 8;

            var arrowX = new ArrowVisual3D();
            arrowX.Direction = new Vector3D(1, 0, 0);
            arrowX.Point1 = new Point3D(0, 0, 0);
            arrowX.Point2 = new Point3D(maxVal, 0, 0);
            arrowX.Diameter = 0.1;
            arrowX.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportLeft.Children.Add(arrowX);

            arrowX = new ArrowVisual3D();
            arrowX.Direction = new Vector3D(1, 0, 0);
            arrowX.Point1 = new Point3D(0, 0, 0);
            arrowX.Point2 = new Point3D(maxVal, 0, 0);
            arrowX.Diameter = 0.1;
            arrowX.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportRight.Children.Add(arrowX);

            var arrowMX = new ArrowVisual3D();
            arrowMX.Direction = new Vector3D(-1, 0, 0);
            arrowMX.Point1 = new Point3D(0, 0, 0);
            arrowMX.Point2 = new Point3D(-maxVal, 0, 0);
            arrowMX.Diameter = 0.1;
            arrowMX.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportLeft.Children.Add(arrowMX);

            arrowMX = new ArrowVisual3D();
            arrowMX.Direction = new Vector3D(-1, 0, 0);
            arrowMX.Point1 = new Point3D(0, 0, 0);
            arrowMX.Point2 = new Point3D(-maxVal, 0, 0);
            arrowMX.Diameter = 0.1;
            arrowMX.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportRight.Children.Add(arrowMX);

            var arrowY = new ArrowVisual3D();
            arrowY.Direction = new Vector3D(0, 0, 1);
            arrowY.Point1 = new Point3D(0, 0, 0);
            arrowY.Point2 = new Point3D(0, 0, maxVal);
            arrowY.Diameter = 0.1;
            arrowY.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportLeft.Children.Add(arrowY);

            arrowY = new ArrowVisual3D();
            arrowY.Direction = new Vector3D(0, 0, 1);
            arrowY.Point1 = new Point3D(0, 0, 0);
            arrowY.Point2 = new Point3D(0, 0, maxVal);
            arrowY.Diameter = 0.1;
            arrowY.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportRight.Children.Add(arrowY);

            var arrowMY = new ArrowVisual3D();
            arrowMY.Direction = new Vector3D(0, 0, -1);
            arrowMY.Point1 = new Point3D(0, 0, 0);
            arrowMY.Point2 = new Point3D(0, 0, -maxVal);
            arrowMY.Diameter = 0.1;
            arrowMY.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportLeft.Children.Add(arrowMY);

            arrowMY = new ArrowVisual3D();
            arrowMY.Direction = new Vector3D(0, 0, -1);
            arrowMY.Point1 = new Point3D(0, 0, 0);
            arrowMY.Point2 = new Point3D(0, 0, -maxVal);
            arrowMY.Diameter = 0.1;
            arrowMY.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportRight.Children.Add(arrowMY);

            var arrowZ = new ArrowVisual3D();
            arrowZ.Direction = new Vector3D(0, 1, 0);
            arrowZ.Point1 = new Point3D(0, 0, 0);
            arrowZ.Point2 = new Point3D(0, maxVal, 0);
            arrowZ.Diameter = 0.1;
            arrowZ.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportLeft.Children.Add(arrowZ);

            arrowZ = new ArrowVisual3D();
            arrowZ.Direction = new Vector3D(0, 1, 0);
            arrowZ.Point1 = new Point3D(0, 0, 0);
            arrowZ.Point2 = new Point3D(0,maxVal, 0);
            arrowZ.Diameter = 0.1;
            arrowZ.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportRight.Children.Add(arrowZ);

            var arrowMZ = new ArrowVisual3D();
            arrowMZ.Direction = new Vector3D(0, -1, 0);
            arrowMZ.Point1 = new Point3D(0, 0, 0);
            arrowMZ.Point2 = new Point3D(0, -maxVal,0);
            arrowMZ.Diameter = 0.1;
            arrowMZ.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportLeft.Children.Add(arrowMZ);

            arrowMZ = new ArrowVisual3D();
            arrowMZ.Direction = new Vector3D(0, 0, -1);
            arrowMZ.Point1 = new Point3D(0, 0, 0);
            arrowMZ.Point2 = new Point3D(0, -maxVal, 0);
            arrowMZ.Diameter = 0.1;
            arrowMZ.Fill = System.Windows.Media.Brushes.Black;
            HelixViewportRight.Children.Add(arrowMZ);

            var xArrowText = new TextVisual3D();
            xArrowText.Text = "X";
            xArrowText.Position = new Point3D(maxVal - 0.5, 0, 0.5);
            xArrowText.Height = 0.5;
            xArrowText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewportLeft.Children.Add(xArrowText);

            xArrowText = new TextVisual3D();
            xArrowText.Text = "X";
            xArrowText.Position = new Point3D(maxVal - 0.5, 0, 0.5);
            xArrowText.Height = 0.5;
            xArrowText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewportRight.Children.Add(xArrowText);

            var yArrowText = new TextVisual3D();
            yArrowText.Text = "Y";
            yArrowText.Position = new Point3D(0, 0.5, maxVal - 0.5);
            yArrowText.Height = 0.5;
            yArrowText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewportLeft.Children.Add(yArrowText);

            yArrowText = new TextVisual3D();
            yArrowText.Text = "Y";
            yArrowText.Position = new Point3D(0, 0.5, maxVal - 0.5);
            yArrowText.Height = 0.5;
            yArrowText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewportRight.Children.Add(yArrowText);

            var zArrowText = new TextVisual3D();
            zArrowText.Text = "Z";
            zArrowText.Position = new Point3D(0.5, maxVal - 0.5, 0 );
            zArrowText.Height = 0.5;
            zArrowText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewportLeft.Children.Add(zArrowText);

            zArrowText = new TextVisual3D();
            zArrowText.Text = "Z";
            zArrowText.Position = new Point3D(0.5, maxVal - 0.5, 0);
            zArrowText.Height = 0.5;
            zArrowText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewportRight.Children.Add(zArrowText);

            var leftText = new TextVisual3D();
            leftText.Text = "Euler Angles Interpolation";
            leftText.Position = new Point3D(0, 0, maxVal + 0.5);
            leftText.Height = 1;
            leftText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewportLeft.Children.Add(leftText);

            var rightText = new TextVisual3D();
            rightText.Text = "Quaternion Interpolation";
            rightText.Position = new Point3D(0, 0, maxVal + 0.5);
            rightText.Height = 1;
            rightText.FontWeight = System.Windows.FontWeights.Bold;
            HelixViewportRight.Children.Add(rightText);

            
            SetupStartConfiguration();
            SetupEndConfiguration();

            frameEuler = new CombinedManipulator()
            {
                CanRotateX = false,
                CanRotateY = false,
                CanRotateZ = false
            };

            frameQuaternion = new CombinedManipulator()
            {
                CanRotateX = false,
                CanRotateY = false,
                CanRotateZ = false
            };
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (buttonsFlags[0]) return;
            for (int i = 0; i < buttonsFlags.Length; i++)
                buttonsFlags[i] = false;
            buttonsFlags[0] = true;

            linearInterpolator.SetupInterpolator(
                startAngleR,
                startAngleP,
                startAngleY,
                endAngleR,
                endAngleP,
                endAngleY,
                startPositionX,
                startPositionY,
                startPositionZ,
                endPositionX,
                endPositionY,
                endPositionZ,
                startQuaternion,
                endQuaternion);

            sphericalLinearInterpolator.SetupInterpolator(
                 startQuaternion,
                endQuaternion
                );

            if (!animationStarted)
            {
                animationStarted = true;
                //HelixViewportLeft.Children.Remove(FrameStartEulerManipulator);
                //HelixViewportLeft.Children.Remove(FrameEndEulerManipulator);
                //HelixViewportRight.Children.Remove(FrameStartQuaternionManipulator);
                //HelixViewportRight.Children.Remove(FrameEndQuaternionManipulator);
                currentPosition = new Position(StartPositionX, StartPositionY, StartPositionZ);
                currentRotation = new Rotation(startAngleR, startAngleP, startAngleY);
                currentQuaternion = startQuaternion;

                SetupCurrentConfiguration();

                HelixViewportLeft.Children.Add(frameEuler);
                HelixViewportRight.Children.Add(frameQuaternion);
                startTime = DateTime.Now;
                dispatcherTimer.Start();
                return;
            }
            timeDelay += DateTime.Now - stopTime;
            dispatcherTimer.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (buttonsFlags[1]) return;
            for (int i = 0; i < buttonsFlags.Length; i++)
                buttonsFlags[i] = false;
            buttonsFlags[1] = true;
            dispatcherTimer.Stop();
            stopTime = DateTime.Now;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (buttonsFlags[2]) return;
            for (int i = 0; i < buttonsFlags.Length; i++)
                buttonsFlags[i] = false;
            buttonsFlags[2] = true;
            dispatcherTimer.Stop();
            animationStarted = false;
            timeDelay = new TimeSpan();

            HelixViewportLeft.Children.Remove(frameEuler);
            HelixViewportRight.Children.Remove(frameQuaternion);

            //HelixViewportLeft.Children.Add(FrameStartEulerManipulator);
            //HelixViewportLeft.Children.Add(FrameEndEulerManipulator);
            //HelixViewportRight.Children.Add(FrameStartQuaternionManipulator);
            //HelixViewportRight.Children.Add(FrameEndQuaternionManipulator);
        }

        private void RefreshEulerToQuaternion(object sender, RoutedEventArgs e)
        {
            //SetupStartConfiguration(ConversionType.EulerToQuaternion);
            //SetupEndConfiguration(ConversionType.EulerToQuaternion);
            SetupStartConfiguration();
            SetupEndConfiguration();
        }


        private void RefreshQuaternionToEuler(object sender, RoutedEventArgs e)
        {
            //SetupStartConfiguration(ConversionType.QuaternionToEuler);
            //SetupEndConfiguration(ConversionType.QuaternionToEuler);
            SetupStartConfiguration();
            SetupEndConfiguration();
        }

        private void EndApplyChangesButton_Click(object sender, RoutedEventArgs e)
        {
            SetupEndConfiguration();
        }


        private void ApplyConfigurationFromEulerAnglesSceneButton_Click(object sender, RoutedEventArgs e)
        {
            var startMat = FrameStartEuler.Transform.Value;
            var endMat = FrameEndEuler.Transform.Value;
            //FrameStartQuaternion.Transform = new MatrixTransform3D(startMat);
            //FrameEndQuaternion.Transform = new MatrixTransform3D(endMat);
            ExtractDataFromTransformationMatrices(startMat, endMat);
            //SetupStartConf();
            //SetupEndConf();
        }

        private void ApplyConfigurationFromQuaternionSceneButton_Click(object sender, RoutedEventArgs e)
        {
            var startMat = FrameStartQuaternion.Transform.Value;
            var endMat = FrameEndQuaternion.Transform.Value;
            //FrameStartEuler.Transform = new MatrixTransform3D(startMat);
            //FrameEndEuler.Transform = new MatrixTransform3D(endMat);
            ExtractDataFromTransformationMatrices(startMat, endMat);
            //SetupStartConf();
            //SetupEndConf();
        }


        private void ExtractEulerAngles(Quaternion q, ref double r, ref double p, ref double y)
        {
            double test = q.X * q.Y + q.Z * q.W;
            if (test > 0.499)
            { // singularity at north pole
                p = 2 * Math.Atan2(q.X, q.W);
                y = Math.PI / 2;
                r = 0;
                return;
            }
            if (test < -0.499)
            { // singularity at south pole
                p = -2 * Math.Atan2(q.X, q.W);
                y = -Math.PI / 2;
                r = 0;
                return;
            }
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;
            p = Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, 1 - 2 * sqy - 2 * sqz);
            y = Math.Asin(2 * test);
            r = Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, 1 - 2 * sqx - 2 * sqz);
        }

        private Quaternion ExtractQuaternion(Matrix3D mat)
        {
            var tr = mat.M11 + mat.M22 + mat.M33;
            double qx, qy, qz, qw;

            if (tr > 0)
            {
                var S = Math.Sqrt(tr + 1.0) * 2;
                qw = 0.25 * S;
                qx = (mat.M23 - mat.M32) / S;
                qy = (mat.M31 - mat.M13) / S;
                qz = (mat.M12 - mat.M21) / S;
            }
            else if ((mat.M11 > mat.M22) & (mat.M11 > mat.M33))
            {
                var S = Math.Sqrt(1.0 + mat.M11 - mat.M22 - mat.M33) * 2;
                qw = (mat.M23 - mat.M32) / S;
                qx = 0.25 * S;
                qy = (mat.M21 + mat.M12) / S;
                qz = (mat.M31 + mat.M13) / S;
            }
            else if (mat.M22 > mat.M33)
            {
                var S = Math.Sqrt(1.0 + mat.M22 - mat.M11 - mat.M33) * 2;
                qw = (mat.M31 - mat.M13) / S;
                qx = (mat.M21 + mat.M12) / S;
                qy = 0.25 * S;
                qz = (mat.M32 + mat.M23) / S;
            }
            else
            {
                var S = Math.Sqrt(1.0 + mat.M33 - mat.M11 - mat.M22) * 2;
                qw = (mat.M12 - mat.M21) / S;
                qx = (mat.M31 + mat.M13) / S;
                qy = (mat.M32 + mat.M23) / S;
                qz = 0.25 * S;
            }

            return new Quaternion(qx, qy, qz, qw);
        }

        private void ExtractDataFromTransformationMatrices(Matrix3D startMat, Matrix3D endMat)
        {
            StartPositionX = startMat.OffsetX;
            StartPositionY = startMat.OffsetY;
            StartPositionZ = startMat.OffsetZ;
            EndPositionX = endMat.OffsetX;
            EndPositionY = endMat.OffsetY;
            EndPositionZ = endMat.OffsetZ;

            startQuaternion = ExtractQuaternion(startMat);
            endQuaternion = ExtractQuaternion(endMat);

            StartQuaternionX = Math.Round(startQuaternion.X, 2);
            StartQuaternionY = Math.Round(startQuaternion.Y, 2);
            StartQuaternionZ = Math.Round(startQuaternion.Z, 2);
            StartQuaternionW = Math.Round(startQuaternion.W, 2);
            EndQuaternionX = Math.Round(endQuaternion.X, 2);
            EndQuaternionY = Math.Round(endQuaternion.Y, 2);
            EndQuaternionZ = Math.Round(endQuaternion.Z, 2);
            EndQuaternionW = Math.Round(endQuaternion.W, 2);

            double r = 0, p = 0, y = 0;
            ExtractEulerAngles(startQuaternion, ref r, ref p, ref y);
            StartAngleR = Math.Round(r * 180 / Math.PI, 2);
            StartAngleP = Math.Round(p * 180 / Math.PI, 2);
            StartAngleY = Math.Round(y * 180 / Math.PI, 2);
            ExtractEulerAngles(endQuaternion, ref r, ref p, ref y);
            EndAngleR = Math.Round(r * 180 / Math.PI, 2);
            EndAngleP = Math.Round(p * 180 / Math.PI, 2);
            EndAngleY = Math.Round(y * 180 / Math.PI, 2);
            //StartAngleR = -Math.Round(Math.Asin(startMat.M32) * 180 / Math.PI, 1);
            //StartAngleY = Math.Round(Math.Atan2(startMat.M12, startMat.M22) * 180 / Math.PI, 1);
            //StartAngleP = Math.Round(Math.Atan2(startMat.M31, startMat.M33) * 180 / Math.PI, 1);
            //EndAngleR = -Math.Round(Math.Asin(endMat.M32) * 180 / Math.PI, 1);
            //EndAngleY = Math.Round(Math.Atan2(endMat.M12, endMat.M22) * 180 / Math.PI, 1);
            //EndAngleP = Math.Round(Math.Atan2(endMat.M31, endMat.M33) * 180 / Math.PI, 1);
        }

    }
}
