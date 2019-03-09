using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareChallengeClient
{
    public class Field
    {
        public int X, Y;
        public FieldState State;

        /// <summary>
        /// Checks if theres a fish on this Field
        /// </summary>
        public bool HasPiranha()
        {
            return State == FieldState.BLUE || State == FieldState.RED;
        }

        /// <summary>
        /// Used to set all the variables of this class in one line
        /// </summary>
        public void Update(int X, int Y, FieldState State)
        {
            this.X = X;
            this.Y = Y;
            this.State = State;
        }
    }

    public enum FieldState { EMPTY, RED, BLUE, OBSTRUCTED }
}
