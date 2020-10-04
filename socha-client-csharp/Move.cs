using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace socha_client_csharp
{
    public abstract class Move
    {
        /// <summary>
        /// Converts this Move to XML
        /// <para>This is used to pack the Move into a format that can be send to the Server</para> 
        /// <para>You usually wont need this Method if you are programming your Client Logic</para> 
        /// </summary>
        public abstract string ToXML();
    }

    public class SkipMove : Move
    {
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
        /// Checks if this move can be performed on Board B
        /// </summary>
        /// <param name="B"> The Board this move should be performed on </param>
        public bool IsLegalOn(Board B) // https://youtu.be/nz20lu2AM2k?t=8
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
