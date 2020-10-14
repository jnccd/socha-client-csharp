using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace socha_client_csharp
{
    /// <summary>
    /// This is a generic move class
    /// </summary>
    public abstract class Move : ICloneable
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

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public virtual object Clone() => MemberwiseClone();
    }

    public class SkipMove : Move, ICloneable
    {
        /// <summary>
        /// Checks if this move can be performed on Board B
        /// </summary>
        /// <param name="B"> The Board this move should be performed on </param>
        public override bool IsLegalOn(State S) => !S.IsStartTurn();

        public override string ToXML() => $"<data class=\"sc.plugin2021.SkipMove\"/>";
    }

    /// <summary>
    /// This Class contains the X and Y coordinates the Move starts from, aswell as the Direction of the Move and DebugHints.
    /// <para>The coordinates the Move ends on can be calculated by calling GetEndpointOn</para> 
    /// </summary>
    public class SetMove : Move, ICloneable
    {
        public readonly PieceColor Color;
        public readonly PieceKind Kind;
        public readonly Rotation Rot;
        public readonly bool Flipped;
        public readonly int X, Y;

        public List<string> DebugHints;

        private Point[] affectedPositions;
        public Point[] AffectedPositions
        {
            get 
            {
                if (affectedPositions == null)
                    affectedPositions = GetAffectedPositions();
                return affectedPositions;
            }
        }

        /// <summary>
        /// Creates a New Move
        /// </summary>
        public SetMove(PieceColor Color, PieceKind Kind, Rotation Rot, bool Flipped, int X, int Y, List<string> DebugHints = null)
        {
            this.Color = Color;
            this.Kind = Kind;
            this.Rot = Rot;
            this.Flipped = Flipped;
            this.X = X;
            this.Y = Y;

            this.DebugHints = DebugHints;
            if (DebugHints == null)
                this.DebugHints = new List<string>();
        }

        /// <summary>
        /// Returns the kind of the piece that is set in this move
        /// </summary>
        public PieceKind GetKind() => Kind;

        /// <summary>
        /// Returns all points that this move will change on the board
        /// </summary>
        private Point[] GetAffectedPositions()
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
                shapePos = new Point[] { Coordinates(0, 0), Coordinates(0, 1), Coordinates(0, 2), Coordinates(0, 3) };
            else if (Kind == PieceKind.TETRO_L)
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
                return new Point[] {  };

            // Apply transformations
            if (Rot == Rotation.RIGHT)
                shapePos = shapePos.Select(x => new Point(-x.Y, x.X)).ToArray();
            else if (Rot == Rotation.LEFT)
                shapePos = shapePos.Select(x => new Point(x.Y, -x.X)).ToArray();
            else if (Rot == Rotation.MIRROR)
                shapePos = shapePos.Select(x => new Point(-x.X, -x.Y)).ToArray();

            if (Flipped)
                shapePos = shapePos.Select(x => new Point(-x.X, x.Y)).ToArray();

            // Normalize coordinates & Translate to board coords
            int minX = shapePos.Min(x => x.X);
            int minY = shapePos.Min(X => X.Y);
            shapePos = shapePos.Select(x => new Point(x.X - minX + X, x.Y - minY + Y)).ToArray();

            return shapePos;
        }

        /// <summary>
        /// Checks if this move can be performed on game State S
        /// </summary>
        /// <param name="S"> The game State this move should be performed on </param> 
        public override bool IsLegalOn(State S) // https://youtu.be/nz20lu2AM2k?t=8
        {
            // Is piece of right color?
            if (S.CurrentColor != Color)
                return false;

            // Is right start piece?
            if (S.IsStartTurn() && S.StartPiece != Kind)
                return false;

            // Is out of bounds?
            if (AffectedPositions.Any(x => !S.CurrentBoard.IsInBounds(x)))
                return false;

            // Is part of placable pieces?
            if (!S.CurrentPlayersShapes().Contains(Kind))
                return false;

            // --- Placement check

            // Is colliding with other pieces?
            var affectedFields = AffectedPositions.Select(x => S.CurrentBoard.GetField(x));
            if (affectedFields.
                Any(X => X.color != null))
                return false;

            if (S.IsStartTurn())
            {
                // Is in board corner?
                if (AffectedPositions.All(x =>
                    !(x.X == 0 && x.Y == 0) &&
                    !(x.X == Board.Width - 1 && x.Y == 0) &&
                    !(x.X == 0 && x.Y == Board.Height - 1) &&
                    !(x.X == Board.Width - 1 && x.Y == Board.Height - 1)))
                    return false;
            }
            else
            {
                // Is touching other pieces?
                var piece4Neighborhood = AffectedPositions.
                    SelectMany(x => S.CurrentBoard.GetField(x).Get4Neighborhood()).
                    Except(affectedFields);
                if (piece4Neighborhood.
                    Any(X => X.color != null))
                    return false;

                // Is cornering other pieces of same color?
                if (AffectedPositions.
                    SelectMany(x => S.CurrentBoard.GetField(x).Get8Neighborhood()).
                    Except(piece4Neighborhood).
                    Except(affectedFields).
                    All(X => X.color != Color))
                    return false;
            }

            return true;
        }

        public override string ToXML() => 
            $"<room roomId=\"{Program.RoomID}\">\n" +
                $"<data class=\"sc.plugin2021.SetMove\">\n" +
                    $"<piece color=\"{Color}\" kind=\"{Kind}\" rotation=\"{Rot}\" isFlipped=\"{Flipped}\">\n" +
                        $"<position x=\"{X}\" y=\"{Y}\"/>\n" +
                    $"</piece>\n" +
                    $"{(DebugHints.Count > 0 ? DebugHints.Select(x => $"<hint content=\"{x}\"/>\n").Aggregate((x, y) => x + y) : "")}" +
                $"</data>\n" +
            $"</room>";

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public override object Clone()
        {
            SetMove m = (SetMove)MemberwiseClone();

            m.DebugHints = DebugHints.Select(x => (string)x.Clone()).ToList();
            affectedPositions = null;

            return m;
        }
    }
}
