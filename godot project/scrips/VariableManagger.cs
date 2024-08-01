using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dream.scrips
{
    internal class VariableManagger
    {
        public static float bouncyBlock = 100;//Velocity added by bouncy block. Bouncy block always resets velocity. Todo: Bouncing can be chabge a lot according to needs.

        public static float SpringPower = 70;// Velocity added by spring. Only active in Mode 0
        public static bool SpringResetYVel = true; // only active in mode 0. If true Y Velocity is reset before SpringPower is apllied.
        public static int SpringMode = 1; //0 adds Y velocity according to  SpringPower. If SpringResetYVel is true, Y velocity is reset first.
                                          //1 changes Y velocity from down to up (* -1) and multiply it by SpringFactor
        public static float SpringFactor = 0.7f;// Only active in mode 1. multiplies velocity.

        public static float BeltPower = 10; //velocity added every frame
        public static float MaxBeltSpeed = 50; //belt cannot accelerate beyond tghis velocity


    }
}
