using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socha_client_csharp
{
    /// <summary>
    /// This is a generic move class
    /// </summary>
    public abstract class Move
    {
        /// <summary>
        /// Checks if this move can be performed on Board B
        /// </summary>
        /// <param name="S"> The State this move should be performed on </param>
        public abstract bool IsLegalOn(State S);

        /// <summary>
        /// Converts this Move to XML
        /// <para>This is used to pack the Move into a format that can be send to the Server</para> 
        /// <para>You usually wont need this Method if you are programming your Client Logic</para> 
        /// </summary>
        public abstract string ToXML();
    }

    public class SkipMove : Move
    {
        /// <summary>
        /// Checks if this move can be performed on Board B
        /// </summary>
        /// <param name="B"> The Board this move should be performed on </param>
        public override bool IsLegalOn(State S) => S.Turn > 1;

        public override string ToXML() => $"<data class=\"sc.plugin2021.SkipMove\"/>";
    }

    /// <summary>
    /// This Class contains the X and Y coordinates the Move starts from, aswell as the Direction of the Move and DebugHints.
    /// <para>The coordinates the Move ends on can be calculated by calling GetEndpointOn</para> 
    /// </summary>
    public class SetMove : Move
    {
        public PieceColor Color;
        public PieceKind Kind;
        public Rotation Rot;
        public bool Flipped;
        public int X, Y;

        public List<string> DebugHints;

        /// <summary>
        /// Creates a New Move
        /// </summary>
        public SetMove(PieceColor Color, PieceKind Kind, Rotation Rot, bool Flipped, int X, int Y)
        {
            this.Color = Color;
            this.Kind = Kind;
            this.Rot = Rot;
            this.Flipped = Flipped;
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Returns all points that this move will change on the board
        /// </summary>
        internal Point[] GetAffectedPositions()
        {
            static Point Coordinates(int x, int y) => new Point(x, y);

            Point[] shapePos;
            if (Kind == PieceKind.MONO)
                shapePos = new Point[] { Coordinates(0, 0) };
            else if (Kind == PieceKind.DOMINO)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(1, 0) };
            else if (Kind == PieceKind.TRIO_L)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(0, 1), Coordinates(1, 1) };
            else if (Kind == PieceKind.TRIO_I)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(0, 1), Coordinates(0, 2) };
            else if (Kind == PieceKind.TETRO_O)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(1, 0), Coordinates(0, 1), Coordinates(1, 1) };
            else if (Kind == PieceKind.TETRO_T)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(1, 0), Coordinates(2, 0), Coordinates(1, 1) };
            else if (Kind == PieceKind.TETRO_I)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(0, 1), Coordinates(0, 2), Coordinates(1, 2) };
            else if (Kind == PieceKind.TETRO_Z)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(1, 0), Coordinates(1, 1), Coordinates(2, 1) };
            else if (Kind == PieceKind.PENTO_L)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(0, 1), Coordinates(0, 2), Coordinates(0, 3), Coordinates(1, 3) };
            else if (Kind == PieceKind.PENTO_T)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(1, 0), Coordinates(2, 0), Coordinates(1, 1), Coordinates(1, 2) };
            else if (Kind == PieceKind.PENTO_V)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(0, 1), Coordinates(0, 2), Coordinates(1, 2), Coordinates(2, 2) };
            else if (Kind == PieceKind.PENTO_S)
                shapePos = new Point[] { Coordinates(1, 0), Coordinates(2, 0), Coordinates(3, 0), Coordinates(0, 1), Coordinates(1, 1) };
            else if (Kind == PieceKind.PENTO_Z)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(1, 0), Coordinates(1, 1), Coordinates(1, 2), Coordinates(2, 2) };
            else if (Kind == PieceKind.PENTO_I)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(0, 1), Coordinates(0, 2), Coordinates(0, 3), Coordinates(0, 4) };
            else if (Kind == PieceKind.PENTO_P)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(1, 0), Coordinates(0, 1), Coordinates(1, 1), Coordinates(0, 2) };
            else if (Kind == PieceKind.PENTO_W)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(0, 1), Coordinates(1, 1), Coordinates(1, 2), Coordinates(2, 2) };
            else if (Kind == PieceKind.PENTO_U)
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(0, 1), Coordinates(1, 1), Coordinates(2, 1), Coordinates(2, 0) };
            else if (Kind == PieceKind.PENTO_R)
                shapePos = new Point[] { Coordinates(0, 1), Coordinates(1, 1), Coordinates(1, 2), Coordinates(2, 1), Coordinates(2, 0) };
            else if (Kind == PieceKind.PENTO_X)
                shapePos = new Point[] { Coordinates(1, 0), Coordinates(0, 1), Coordinates(1, 1), Coordinates(2, 1), Coordinates(1, 2) };
            else if (Kind == PieceKind.PENTO_Y)
                shapePos = new Point[] { Coordinates(0, 1), Coordinates(1, 0), Coordinates(1, 1), Coordinates(1, 2), Coordinates(1, 3) };
            else 
                shapePos = new Point[] {  };

            // Apply transformations
            if (Rot == Rotation.LEFT)
                shapePos = shapePos.Select(x => new Point(-x.Y, x.X)).ToArray();
            else if (Rot == Rotation.RIGHT)
                shapePos = shapePos.Select(x => new Point(x.Y, -x.X)).ToArray();
            else if (Rot == Rotation.MIRROR)
                shapePos = shapePos.Select(x => new Point(x.X, -x.Y)).ToArray();

            if (Flipped)
                shapePos = shapePos.Select(x => new Point(-x.X, x.Y)).ToArray();

            // Normalize coordinates
            int minX = shapePos.Min(x => x.X);
            int minY = shapePos.Min(X => X.Y);
            shapePos = shapePos.Select(x => new Point(x.X - minX, x.Y - minY)).ToArray();

            return shapePos;
        }

        /// <summary>
        /// Checks if this move can be performed on Board B
        /// </summary>
        /// <param name="B"> The Board this move should be performed on </param>
        public override bool IsLegalOn(State B) // https://youtu.be/nz20lu2AM2k?t=8
        {
            throw new NotImplementedException();
        }

        public override string ToXML() => 
            $"<room roomId=\"{Program.RoomID}\">" +
                $"<data class=\"sc.plugin2021.SetMove\">" +
                    $"<piece color=\"{Color}\" kind=\"{Kind}\" rotation=\"{Rot}\" isFlipped=\"{Flipped}\">" +
                        $"<position x=\"{X}\" y=\"{Y}\"/>" +
                    $"</piece>" +
                $"{(DebugHints.Count > 0 ? DebugHints.Select(x => $"<hint content=\"{x}\"/>").Aggregate((x, y) => x + y) : "")}" +
                $"</data>" +
            $"</room>";
    }
}
