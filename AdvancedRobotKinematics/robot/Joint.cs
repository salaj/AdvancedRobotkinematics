using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedRobotKinematics.robot
{
    //public enum Type
    //{
    //    Prismatic,
    //    Revolute
    //}

    public class Joint
    {
        //public Type Type { get; set; }
        public Frame Frame { get; set; }
        public int Id;

        private static int id = 0;

        public Joint()
        {
            this.Frame = new Frame();
            //this.Type = Type;
            Id = id++;
        }
    }
}
