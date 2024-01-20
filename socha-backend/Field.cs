using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SochaClient.Backend
{
    public class Field : ICloneable
    {
        public FieldType FType;
        public Board Parent { get; private set; }
        public CubeCoords Coords { get; private set; }

        public Field(FieldType fType, CubeCoords coords, Board parent)
        {
            FType = fType;
            Coords = coords;
            Parent = parent;
        }

        public Color ToColor()
        {
            switch (FType)
            {
                case FieldType.island:
                    return Color.FromArgb(0, 255, 0);
                case FieldType.water:
                    return Color.FromArgb(0, 0, 255);
                case FieldType.passenger:
                    return Color.FromArgb(255, 0, 0);
                default:
                    throw new ArgumentException("wat");
            };
        }

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
