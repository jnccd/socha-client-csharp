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

        public bool hasPiranha()
        {
            return State == FieldState.BLUE || State == FieldState.RED;
        }

        public void Update(int X, int Y, FieldState State)
        {
            this.X = X;
            this.Y = Y;
            this.State = State;
        }
    }

    public enum FieldState { EMPTY, RED, BLUE, OBSTRUCTED }
}
