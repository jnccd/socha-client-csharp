using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SochaClient.Backend
{
    public class Field : ICloneable
    {
        public FieldType FType;
        public bool IsMidstream;
        public Board Parent { get; private set; }
        public CubeCoords Coords { get; private set; }
       
        public Field(FieldType fType, bool isMidstream, CubeCoords coords, Board parent)
        {
            FType = fType;
            IsMidstream = isMidstream;
            Coords = coords;
            Parent = parent;
        }

        public Color ToColor() =>
            FType switch
            {
                FieldType.island => Color.FromArgb(0, 255, 0),
                FieldType.water => Color.FromArgb(0, 0, 255),
                FieldType.passenger => Color.FromArgb(255, 0, 0),
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
