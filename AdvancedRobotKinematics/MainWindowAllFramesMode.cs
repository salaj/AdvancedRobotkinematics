using HelixToolkit.Wpf;
using System.Windows;
using System.Windows.Media.Media3D;

namespace AdvancedRobotKinematics
{
    public partial class MainWindow
    {
        CombinedManipulator[] allFramesEuler;
        CombinedManipulator[] allFramesQuaternion;

        private void OnAllFramesDisplayed(object sender, RoutedEventArgs e)
        {
            //HelixViewportLeft.Children.Remove(FrameStartEulerManipulator);
            //HelixViewportLeft.Children.Remove(FrameEndEulerManipulator);
            HelixViewportRight.Children.Remove(FrameStartQuaternionManipulator);
            HelixViewportRight.Children.Remove(FrameEndQuaternionManipulator);

            PlayButton.IsEnabled = false;
            PauseButton.IsEnabled = false;
            ResetButton.IsEnabled = false;
            FramesCountBox.IsEnabled = false;

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


            if (FramesCount < 2)
                FramesCount = 2;

            allFramesEuler = new CombinedManipulator[FramesCount];
            allFramesQuaternion = new CombinedManipulator[FramesCount];

            for (int i = 0; i < FramesCount; i++)
            {
                var timeFactor = i / (FramesCount - 1.0);
                linearInterpolator.CalculateCurrentPosition(ref currentPosition, timeFactor);
                linearInterpolator.CalculateCurrentAngle(ref currentRotation, timeFactor);

                if (lerpActivated)
                    linearInterpolator.CalculateCurrentQuaternion(ref currentQuaternion, timeFactor);
                else
                {
                    sphericalLinearInterpolator.CalculateCurrentQuaternion(ref currentQuaternion, timeFactor);
                }
                SetupCurrentConfiguration();

                allFramesEuler[i] = new CombinedManipulator()
                {
                    Transform = new MatrixTransform3D(frameEuler.Transform.Value),
                    CanRotateX = false,
                    CanRotateY = false,
                    CanRotateZ = false
                };
                allFramesQuaternion[i] = new CombinedManipulator()
                {
                    Transform = new MatrixTransform3D(frameQuaternion.Transform.Value),
                    CanRotateX = false,
                    CanRotateY = false,
                    CanRotateZ = false
                };

                HelixViewportLeft.Children.Add(allFramesEuler[i]);
                HelixViewportRight.Children.Add(allFramesQuaternion[i]);
            }
        }


        private void OnAllFramesHidden(object sender, RoutedEventArgs e)
        {
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = true;
            ResetButton.IsEnabled = true;
            FramesCountBox.IsEnabled = true;

            //HelixViewportLeft.Children.Add(FrameStartEulerManipulator);
            //HelixViewportLeft.Children.Add(FrameEndEulerManipulator);
            HelixViewportRight.Children.Add(FrameStartQuaternionManipulator);
            HelixViewportRight.Children.Add(FrameEndQuaternionManipulator);

            for (int i = 0; i < FramesCount; i++)
            {
                HelixViewportLeft.Children.Remove(allFramesEuler[i]);
                HelixViewportRight.Children.Remove(allFramesQuaternion[i]);
            }
        }
    }
}
