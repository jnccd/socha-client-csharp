using System.Xml.Serialization;

namespace SochaClient.Backend
{
    public enum PlayerTeam 
    {
        ONE,
        TWO,
    }
    public enum Direction
    {
        RIGHT,
        DOWN_RIGHT,
        DOWN_LEFT,
        LEFT,
        UP_LEFT,
        UP_RIGHT,
    }
    [XmlType(IncludeInSchema = false)]
    public enum FieldType
    {
        water,
        island,
        passenger,
    }
}
