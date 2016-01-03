using System.IO;
using System.Windows.Media;
using AdvancedRobotKinematics.bases;
using AdvancedRobotKinematics.interpolators;
using AdvancedRobotKinematics.maths;
using AdvancedRobotKinematics.robot;
using HelixToolkit.Wpf;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using MotionInterpolation.bases;

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
        private Position currentPosition;
        //private double currentAngleR;
        //private double currentAngleP;
        //private double currentAngleY;
        private Rotation currentRotation;
        private Quaternion currentQuaternion;
        private bool animationStarted = false;
        private DateTime startTime;
        private DateTime stopTime;
        private TimeSpan timeDelay;
        private bool[] buttonsFlags;
        private LinearInterpolator linearInterpolator;
        private SphericalLinearInterpolator sphericalLinearInterpolator;

        private Robot robot;

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
            InitializeScene();
            InitializeRobot();
        }

        void InitializeRobot()
        {
            robot = new Robot(HelixViewportLeft);
            robot.Update();
        }

        private void InitializeTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);
        }

        private void SetupStartConfiguration()
        {
            var startPosition = new Position(StartPositionX, StartPositionY, StartPositionZ);
            var startRotation = new Rotation(startAngleR, startAngleP, startAngleY);
            SetupConfiguration(FrameStartEuler, FrameStartQuaternion, startPosition, startRotation, ref startQuaternion);
        }

        private void SetupEndConfiguration()
        {
            var endPosition = new Position(EndPositionX, EndPositionY, EndPositionZ);
            var endRotation = new Rotation(endAngleR, endAngleP, endAngleY);
            SetupConfiguration(FrameEndEuler, FrameEndQuaternion, endPosition, endRotation, ref endQuaternion);
        }

        private void SetupCurrentConfiguration()
        {
            SetupConfiguration(frameEuler, frameQuaternion, currentPosition, currentRotation, ref currentQuaternion);
        }

        private void SetupConfiguration(ModelVisual3D euler, ModelVisual3D quaternion, Position position, Rotation rotation, ref Quaternion Q)
        {
            var eulerTransformGroup = new Transform3DGroup();
            var quaternionTransformGroup = new Transform3DGroup();

            eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(xDirection, rotation.R)));
            eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(yDirection, rotation.P)));
            eulerTransformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(zDirection, rotation.Y)));
            eulerTransformGroup.Children.Add(new TranslateTransform3D(position.X, position.Y, position.Z));
            euler.Transform = eulerTransformGroup;
            Singleton<EulerToQuaternionConverter>.Instance.Convert(rotation.R, rotation.P, rotation.Y, ref Q);

            quaternionTransformGroup.Children.Add(new RotateTransform3D(new QuaternionRotation3D(Q)));
            quaternionTransformGroup.Children.Add(new TranslateTransform3D(position.X, position.Y, position.Z));
            quaternion.Transform = quaternionTransformGroup;
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
            linearInterpolator.CalculateCurrentAngle(ref currentRotation, normalizedTime);
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
