using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace MotionInterpolation.interpolators
{
    public class RealTimeInterpolator
    {

        public double GetValue(double normalizedTime, double startVal, double endVal)
        {
            double diff = endVal - startVal;

            return startVal + diff*normalizedTime;
        }

        public Vector3D GetVectorValue(double normalizedTime, Vector3D startVal, Vector3D endVal)
        {
            Vector3D diff = endVal - startVal;

            return startVal + diff * normalizedTime;
        }
    }
}
