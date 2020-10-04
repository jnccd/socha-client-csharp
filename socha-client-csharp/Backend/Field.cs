using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socha_client_csharp
{
    public class Field : ICloneable
    {
        public int X, Y;
        public PieceColor? State;

        /// <summary>
        /// Create a new field object
        /// </summary>
        public Field(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            State = null;
        }

        /// <summary>
        /// Used to set all the variables of this class in one line of code
        /// </summary>
        public void Update(int X, int Y, PieceColor? State)
        {
            this.X = X;
            this.Y = Y;
            this.State = State;
        }

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
