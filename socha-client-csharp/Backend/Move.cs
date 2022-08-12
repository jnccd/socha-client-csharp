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
        public readonly Point From, To, HexFrom, HexTo;

        public Move(Point from, Point to)
        {
            From = from;
            To = to;

            HexFrom = Board.ArrayToHexCoords(from);
            HexTo = Board.ArrayToHexCoords(to);
        }

        /// <summary>
        /// Checks if this move can be performed on game State S
        /// </summary>
        /// <param name="S"> The game State this move should be performed on </param> 
        public bool IsLegalOn(State S) // https://youtu.be/nz20lu2AM2k?t=8
        {
            if (S.Turn < 8)
            {
                // Place Move

                if (From != null)
                {
                    Debug.WriteLine("Illegal: No Put Move!");
                    return false;
                }

                if (S.Board.GetField(To).fishes != 1)
                {
                    Debug.WriteLine("Illegal: Put only on one fish field!");
                    return false;
                }

                if (S.Board.GetField(To).Piece != null)
                {
                    Debug.WriteLine("Illegal: Put only on one fish field!");
                    return false;
                }
            }
            else
            {
                // Normal Move

                if (From != null && S.CurrentPlayer.Team != S.Board.GetField(From).Piece?.Team)
                {
                    Debug.WriteLine("Illegal: Wrong Team!");
                    return false;
                }

                if (!Board.IsInBounds(To))
                {
                    Debug.WriteLine("Illegal: OOB!");
                    return false;
                }

                if (!S.Board.GetField(To).Free())
                {
                    Debug.WriteLine("Illegal: Cant move there!");
                    return false;
                }

                if (!S.Board.GetField(From).PossibleCoordsToMoveTo(S).Contains(To))
                {
                    Debug.WriteLine("Illegal: Not possible to move there!");
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Converts this Move to XML
        /// <para>This is used to pack the Move into a format that can be send to the Server</para> 
        /// <para>You usually wont need this Method if you are programming your Client Logic</para> 
        /// </summary>
        public string ToXML()
        {
            if (HexTo.X == 0)
                GetHashCode();

            if (From != null)
                return  $"<room roomId=\"{Program.RoomID}\">\n" +
                            $"<data class=\"move\">\n" +
                                $"<from x=\"{HexFrom.X}\" y=\"{HexFrom.Y}\"/>" +
                                $"<to x = \"{HexTo.X}\" y=\"{HexTo.Y}\"/>" +
                            $"</data>\n" +
                        $"</room>";
            else
                return $"<room roomId=\"{Program.RoomID}\">\n" +
                            $"<data class=\"move\">\n" +
                            $"<to x = \"{HexTo.X}\" y=\"{HexTo.Y}\"/>" +
                            $"</data>\n" +
                        $"</room>";
        }

        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        public object Clone() => MemberwiseClone();
    }
}
