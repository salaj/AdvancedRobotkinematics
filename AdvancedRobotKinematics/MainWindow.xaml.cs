using System.IO;
using System.Windows.Media;
using AdvancedRobotKinematics.interpolators;
using AdvancedRobotKinematics.maths;
using HelixToolkit.Wpf;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace AdvancedRobotKinematics
{
    public enum ConversionType
    {
        EulerToQuaternion,
        QuaternionToEuler
    };

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Quaternion startQuaternion;
        private Quaternion endQuaternion;
        DispatcherTimer dispatcherTimer;
        private Vector3D currentPosition;
        private double currentAngleR;
        private double currentAngleP;
        private double currentAngleY;
        private Quaternion currentQuaternion;
        private bool animationStarted = false;
        private DateTime startTime;
        private DateTime stopTime;
        private TimeSpan timeDelay;
        private bool[] buttonsFlags;
        private LinearInterpolator linearInterpolator;
        private SphericalLinearInterpolator sphericalLinearInterpolator;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeVariables();
            InitializeScene();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);
        }


        public Vector3D QuaternionToEulerVector3D(Quaternion q1)
        {
            double sqw = q1.W * q1.W;
            double sqx = q1.X * q1.X;
            double sqy = q1.Y * q1.Y;
            double sqz = q1.Z * q1.Z;
            double unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            double test = q1.X * q1.Y + q1.Z * q1.W;
            double heading, bank, attitude;
            if (test > 0.499 * unit)
            { // singularity at north pole
                heading = 2 * Math.Atan2(q1.X, q1.W);
                attitude = Math.PI / 2;
                bank = 0;
            }else
            if (test < -0.499 * unit)
            { // singularity at south pole
                heading = -2 * Math.Atan2(q1.X, q1.W);
                attitude = -Math.PI / 2;
                bank = 0;
            }
            else
            {
                heading = Math.Atan2(2 * q1.Y * q1.W - 2 * q1.X * q1.Z, sqx - sqy - sqz + sqw);
                attitude = Math.Asin(2 * test / unit);
                bank = Math.Atan2(2 * q1.X * q1.W - 2 * q1.Y * q1.Z, -sqx + sqy - sqz + sqw);
            }
            return new Vector3D(heading, attitude, bank);
        }

        //private void SetupStartConf()
        //{
        //    var eulerStartTransformGroup = new Transform3DGroup();
        //    var quaternionStartTransformGroup = new Transform3DGroup();

        //    eulerStartTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(xDirection, startAngleR)));
        //    eulerStartTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(yDirection, startAngleP)));
        //    eulerStartTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(zDirection, startAngleY)));
        //    eulerStartTransformGroup.Children.Add(new TranslateTransform3D(StartPositionX, StartPositionY, StartPositionZ));
        //    FrameStartEuler.Transform = eulerStartTransformGroup;

        //    startQuaternion = new Quaternion(StartQuaternionX, StartQuaternionY, StartQuaternionZ, StartQuaternionW);
        //    quaternionStartTransformGroup.Children.Add(new RotateTransform3D(new QuaternionRotation3D(startQuaternion)));
        //    quaternionStartTransformGroup.Children.Add(new TranslateTransform3D(StartPositionX, StartPositionY, StartPositionZ));
        //    FrameStartQuaternion.Transform = quaternionStartTransformGroup;
        //}


        //private void SetupEndConf()
        //{
        //    var eulerEndTransformGroup = new Transform3DGroup();
        //    var quaternionEndTransformGroup = new Transform3DGroup();

        //    eulerEndTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(xDirection, EndAngleR)));
        //    eulerEndTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(yDirection, EndAngleP)));
        //    eulerEndTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(zDirection, EndAngleY)));
        //    eulerEndTransformGroup.Children.Add(new TranslateTransform3D(EndPositionX, EndPositionY, EndPositionZ));
        //    FrameEndEuler.Transform = eulerEndTransformGroup;

        //    endQuaternion = new Quaternion(EndQuaternionX, EndQuaternionY, EndQuaternionZ, EndQuaternionW);
        //    quaternionEndTransformGroup.Children.Add(new RotateTransform3D(new QuaternionRotation3D(endQuaternion)));
        //    quaternionEndTransformGroup.Children.Add(new TranslateTransform3D(EndPositionX, EndPositionY, EndPositionZ));
        //    FrameEndQuaternion.Transform = quaternionEndTransformGroup;
        //}

        //private void SetupCurrentConf()
        //{
        //    var eulerTransformGroup = new Transform3DGroup();
        //    var quaternionTransformGroup = new Transform3DGroup();

        //    eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(xDirection, currentAngleR)));
        //    eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(yDirection, currentAngleP)));
        //    eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(zDirection, currentAngleY)));
        //    eulerTransformGroup.Children.Add(new TranslateTransform3D(currentPosition.X, currentPosition.Y, currentPosition.Z));
        //    frameEuler.Transform = eulerTransformGroup;

        //    quaternionTransformGroup.Children.Add(new RotateTransform3D(new QuaternionRotation3D(currentQuaternion)));
        //    quaternionTransformGroup.Children.Add(new TranslateTransform3D(currentPosition.X, currentPosition.Y, currentPosition.Z));
        //    frameQuaternion.Transform = quaternionTransformGroup;
        //}


        private void SetupStartConfiguration(ConversionType conversionType = ConversionType.EulerToQuaternion)
        {
            double R = 0, P = 0, Y = 0;
            if (conversionType == ConversionType.QuaternionToEuler)
            {
                var loadedQuaternion = new Quaternion(StartQuaternionX, StartQuaternionY, StartQuaternionZ,
                    StartQuaternionW);
                Vector3D eulerAngles = QuaternionToEulerVector3D(loadedQuaternion);
                P = -eulerAngles.X;
                Y = -eulerAngles.Y;
                R = -eulerAngles.Z;

                StartAngleR = R*360/(2.0f*Math.PI);
                StartAngleP = P*360/(2.0f*Math.PI);
                StartAngleY = Y*360/(2.0f*Math.PI);
            }
            else if (conversionType == ConversionType.EulerToQuaternion)
            {
                //convert from euler angles to radians
                R = Math.PI*2.0f/360.0f*-startAngleR;
                P = Math.PI*2.0f/360.0f*-startAngleP;
                Y = Math.PI*2.0f/360.0f*-startAngleY;
            }
            else
            {
                throw new InvalidDataException("undecided type of conversion");
            }

            var transformBuilder = new TransformMatrixBuilder(
                new Vector3D(R, P, Y),
                new Vector3D(startPositionX, StartPositionY, StartPositionZ)
                );
            FrameStartEuler.Transform = transformBuilder.GetEulerTransform();
            var eulerToQuaternionConverter = new EulerToQuaternionConverter();
            startQuaternion = eulerToQuaternionConverter.Convert(P, Y, R);
            //if (conversionType == ConversionType.EulerToQuaternion)
            //{
                StartQuaternionX = startQuaternion.X;
                StartQuaternionY = startQuaternion.Y;
                StartQuaternionZ = startQuaternion.Z;
                StartQuaternionW = startQuaternion.W;
            //}

            Matrix3D rotationQuaternion = eulerToQuaternionConverter.BuildMatrix3DFromQuaternion(startQuaternion);
            FrameStartQuaternion.Transform = transformBuilder.GetQuaternionTransform(rotationQuaternion);
        }

        private void SetupEndConfiguration(ConversionType conversionType = ConversionType.EulerToQuaternion)
        {
            double R = 0, P = 0, Y = 0;
            if (conversionType == ConversionType.QuaternionToEuler)
            {
                var loadedQuaternion = new Quaternion(EndQuaternionX, EndQuaternionY, EndQuaternionZ,
                    EndQuaternionW);
                Vector3D eulerAngles = QuaternionToEulerVector3D(loadedQuaternion);
                P = -eulerAngles.X;
                Y = -eulerAngles.Y;
                R = -eulerAngles.Z;

                EndAngleR = R * 360 / (2.0f * Math.PI);
                EndAngleP = P * 360 / (2.0f * Math.PI);
                EndAngleY = Y * 360 / (2.0f * Math.PI);
            }
            else if (conversionType == ConversionType.EulerToQuaternion)
            {
                //convert from euler angles to radians
                R = Math.PI * 2.0f / 360.0f * -endAngleR;
                P = Math.PI * 2.0f / 360.0f * -endAngleP;
                Y = Math.PI * 2.0f / 360.0f * -endAngleY;
            }
            else
            {
                throw new InvalidDataException("undecided type of conversion");
            }

            var transformBuilder = new TransformMatrixBuilder(
                new Vector3D(R, P, Y),
                new Vector3D(EndPositionX, EndPositionY, EndPositionZ)
                );
            FrameEndEuler.Transform = transformBuilder.GetEulerTransform();
            var eulerToQuaternionConverter = new EulerToQuaternionConverter();
            endQuaternion = eulerToQuaternionConverter.Convert(P, Y, R);
            //if (conversionType == ConversionType.EulerToQuaternion)
            //{
                EndQuaternionX = endQuaternion.X;
                EndQuaternionY = endQuaternion.Y;
                EndQuaternionZ = endQuaternion.Z;
                EndQuaternionW = endQuaternion.W;
            //}

            Matrix3D rotationQuaternion = eulerToQuaternionConverter.BuildMatrix3DFromQuaternion(endQuaternion);
            FrameEndQuaternion.Transform = transformBuilder.GetQuaternionTransform(rotationQuaternion);
        }

        private void SetupCurrentConfiguration()
        {

            double R = 0, P = 0, Y = 0;
            R = Math.PI * 2.0f / 360.0f * -currentAngleR;
            P = Math.PI * 2.0f / 360.0f * -currentAngleP;
            Y = Math.PI * 2.0f / 360.0f * -currentAngleY;


            var transformBuilder = new TransformMatrixBuilder(
                new Vector3D(R, P, Y),
                new Vector3D(currentPosition.X, currentPosition.Y, currentPosition.Z)
                );

            frameEuler.Transform = transformBuilder.GetEulerTransform();
            var eulerToQuaternionConverter = new EulerToQuaternionConverter();
            currentQuaternion = eulerToQuaternionConverter.Convert(P, Y, R);

            Matrix3D rotationQuaternion = eulerToQuaternionConverter.BuildMatrix3DFromQuaternion(currentQuaternion);
            frameQuaternion.Transform = transformBuilder.GetQuaternionTransform(rotationQuaternion);

            //var eulerTransformGroup = new Transform3DGroup();
            //var quaternionTransformGroup = new Transform3DGroup();

            //eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(xDirection, currentAngleR)));
            //eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(yDirection, currentAngleP)));
            //eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(zDirection, currentAngleY)));
            //eulerTransformGroup.Children.Add(new TranslateTransform3D(currentPosition.X, currentPosition.Y, currentPosition.Z));
            //frameEuler.Transform = eulerTransformGroup;

            //quaternionTransformGroup.Children.Add(new RotateTransform3D(new QuaternionRotation3D(currentQuaternion)));
            //quaternionTransformGroup.Children.Add(new TranslateTransform3D(currentPosition.X, currentPosition.Y, currentPosition.Z));
            //frameQuaternion.Transform = quaternionTransformGroup;
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var currentTime = DateTime.Now;
            var timeDifference = currentTime - startTime - timeDelay;
            var elapsedMilliseconds = timeDifference.TotalMilliseconds;
            var animationTimeInMilliseconds = AnimationTime;
            var normalizedTime = elapsedMilliseconds / animationTimeInMilliseconds;
            if (elapsedMilliseconds > AnimationTime)
            {
                ResetButton_Click(null, null);
                return;
            }

            linearInterpolator.CalculateCurrentPosition(ref currentPosition,  normalizedTime);
            linearInterpolator.CalculateCurrentAngle(ref currentAngleR, ref currentAngleP, ref currentAngleY, normalizedTime);
            if (lerpActivated)
                linearInterpolator.CalculateCurrentQuaternion(ref currentQuaternion, normalizedTime);
            else
            {
                sphericalLinearInterpolator.CalculateCurrentQuaternion(ref currentQuaternion, normalizedTime);
            }
            SetupCurrentConfiguration();
        }

    }
}
