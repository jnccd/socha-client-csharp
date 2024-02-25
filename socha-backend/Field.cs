using System;
using System.Collections.Generic;
using System.Linq;

namespace SochaClient.Backend
{
    public class Field : ICloneable
    {
        public FieldType FType { get; private set; }
        public bool IsMidstream { get; private set; }
        public CubeCoords Coords { get; private set; }
        public Board Parent { get; private set; }
        public Direction? Dir { get; private set; }
        public int? Passengers { get; private set; }

        public Field(FieldType fType, bool isMidstream, CubeCoords coords, Board parent, Direction? dir = null, int? passengers = null)
        {
            FType = fType;
            IsMidstream = isMidstream;
            Coords = coords;
            Parent = parent;
            Dir = dir;
            Passengers = passengers;
        }

        public override string ToString() =>
            FType switch
            {
                FieldType.island => "i",
                FieldType.water => IsMidstream ? "m" : "w",
                FieldType.passenger => "p",
                _ => throw new ArgumentException("wat"),
            };

        public object Clone()
        {
            Field f = (Field)MemberwiseClone();
            f.Coords = (CubeCoords)Coords.Clone();
            return f;
        }
        public Field CloneWParent(Board cloneParent)
        {
            Field f = (Field)Clone();
            f.Parent = cloneParent;
            return f;
        }
    }
}
