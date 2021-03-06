﻿using System.IO;
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
using MotionInterpolation.interpolators;

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
        private Quaternion currentQuaternion;
        private Quaternion previousQuaternion;

        DispatcherTimer dispatcherTimer;

        private Position currentPosition;
        private Rotation currentRotation;
        private Position previousPosition;

        private bool animationStarted = false;
        private DateTime startTime;
        private DateTime stopTime;
        private TimeSpan timeDelay;
        private bool[] buttonsFlags;
        private LinearInterpolator linearInterpolator;
        private SphericalLinearInterpolator sphericalLinearInterpolator;
        private RealTimeInterpolator realTimeInterpolator;

        private Robot robotLeft;
        private Robot robotRight;

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
            InitializeRobot();
            InitializeTimer();
            InitializeScene();
        }

        void InitializeRobot()
        {
            robotLeft = new Robot(HelixViewportLeft, FrameStartEuler, FrameEndEuler);
            robotRight = new Robot(HelixViewportRight, FrameStartQuaternion, FrameEndQuaternion);
            UpdateRods();
            //SetupStartConfiguration();
            //SetupEndConfiguration();
            //SetupStartConfiguration();
            //SetupEndConfiguration();
        }

        private void InitializeTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
        }

        private void SetupStartConfiguration()
        {
            var startPosition = new Position(StartPositionX, StartPositionY, StartPositionZ);
            var startRotation = new Rotation(startAngleR, startAngleP, startAngleY);
            try
            {
                robotLeft.SetupEffector(startPosition, startRotation);
                robotLeft.calculateConfiguration(true);

                robotRight.SetupEffector(startPosition, startRotation);
                robotRight.calculateConfiguration(true);
            }
            catch (ConfigurationFailureException e)
            {
                emergencyProcedure(true);
            }
        }

        private void SetupEndConfiguration()
        {
            var endPosition = new Position(EndPositionX, EndPositionY, EndPositionZ);
            var endRotation = new Rotation(endAngleR, endAngleP, endAngleY);
            try
            {
                robotLeft.SetupEffector(endPosition, endRotation);
                robotLeft.calculateConfiguration(false);

                robotRight.SetupEffector(endPosition, endRotation);
                robotRight.calculateConfiguration(false);
            }
            catch (ConfigurationFailureException e)
            {
                emergencyProcedure(false);
            }
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

            if (Robot.InterpolationType == InterpolationType.InternalPositions)
            {
                euler.Transform = eulerTransformGroup;
            }

            Singleton<EulerToQuaternionConverter>.Instance.Convert(rotation.R, rotation.P, rotation.Y, ref Q);

            quaternionTransformGroup.Children.Add(new RotateTransform3D(new QuaternionRotation3D(Q)));
            quaternionTransformGroup.Children.Add(new TranslateTransform3D(position.X, position.Y, position.Z));
            quaternion.Transform = quaternionTransformGroup;

        }

        private void setupInerpolators()
        {
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
        }

        private void emergencyProcedure(bool forStartConfiguration)
        {
            setupInerpolators();

            double normalizedTime = forStartConfiguration ? 0.01 : 0.99;

            currentPosition = new Position(StartPositionX, StartPositionY, StartPositionZ);
            currentRotation = new Rotation(startAngleR, startAngleP, startAngleY);

            linearInterpolator.CalculateCurrentPosition(ref currentPosition, normalizedTime);
            linearInterpolator.CalculateCurrentAngle(ref currentRotation, normalizedTime);
            if (lerpActivated)
                linearInterpolator.CalculateCurrentQuaternion(ref currentQuaternion, normalizedTime);
            else
            {
                sphericalLinearInterpolator.CalculateCurrentQuaternion(ref currentQuaternion, normalizedTime);
            }

            robotLeft.SetupEffector(currentPosition, currentRotation);
            robotLeft.calculateConfiguration(forStartConfiguration);

            robotRight.SetupEffector(currentPosition, currentRotation);
            robotRight.calculateConfiguration(forStartConfiguration);
        }

        private void interpolateEffector(double normalizedTime)
        {
            linearInterpolator.CalculateCurrentPosition(ref currentPosition, normalizedTime);
            linearInterpolator.CalculateCurrentAngle(ref currentRotation, normalizedTime);

            if (lerpActivated)
                linearInterpolator.CalculateCurrentQuaternion(ref currentQuaternion, normalizedTime);
            else
            {
                sphericalLinearInterpolator.CalculateCurrentQuaternion(ref currentQuaternion, normalizedTime);
            }
            SetupCurrentConfiguration();
        }

        private void updateRobot(double normalizedTime)
        {
            robotLeft.InterpolatePositionsLeft(realTimeInterpolator, normalizedTime);

            try
            {
                robotRight.SetupEffector(currentPosition, currentQuaternion);
            }
            catch (ConfigurationFailureException)
            {
                robotRight.SetupEffector(previousPosition, previousQuaternion);
            }
            robotRight.calculateConfiguration(true);
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
                interpolateEffector(1.0f);
                updateRobot(1.0f);
                ResetButton_Click(null, null);
                return;
            }

            interpolateEffector(normalizedTime);
            updateRobot(normalizedTime);

            previousPosition = currentPosition;
            previousQuaternion = currentQuaternion;
        }

    }
}
