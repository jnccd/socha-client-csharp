using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace SochaClient
{
    /// <summary>
    /// This is a generic move class
    /// </summary>
    public class Move : ICloneable
    {
        public readonly Point From, To;
        public Piece Piece;

        public Move(Point from, Point to, Piece piece)
        {
            From = from;
            To = to;
            Piece = piece;
        }

        /// <summary>
        /// Checks if this move can be performed on game State S
        /// </summary>
        /// <param name="S"> The game State this move should be performed on </param> 
        public bool IsLegalOn(State S) // https://youtu.be/nz20lu2AM2k?t=8
        {
            if (S.CurrentPlayer.Color != Piece.PColor)
            {
                Debug.WriteLine("Illegal: Wrong Color!");
                return false;
            }

            if (!S.Board.IsInBounds(To))
            {
                Debug.WriteLine("Illegal: OOB!");
                return false;
            }

            if (S.Board.GetField(To).Piece != null && S.Board.GetField(To).Piece.PColor == Piece.PColor)
            {
                Debug.WriteLine("Illegal: Cant move there!");
                return false;
            }

            if (!S.Board.GetField(From).PossibleCoordsToMoveTo().Contains(To))
            {
                Debug.WriteLine("Illegal: Not possible!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Converts this Move to XML
        /// <para>This is used to pack the Move into a format that can be send to the Server</para> 
        /// <para>You usually wont need this Method if you are programming your Client Logic</para> 
        /// </summary>
        public string ToXML() =>
            $"<room roomId=\"{Program.RoomID}\">\n" +
                $"<data class=\"move\">\n" +
                    $"<from x=\"{From.X}\" y=\"{From.Y}\"/>" +
                    $"<to x = \"{To.X}\" y=\"{To.Y}\"/>" +
                $"</data>\n" +
            $"</room>";

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone() => MemberwiseClone();
    }
}
