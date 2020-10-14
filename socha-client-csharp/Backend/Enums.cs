namespace SochaClient
{
    public enum PlayerTeam 
    {
        ONE,
        TWO 
    }
    public enum PieceColor 
    { 
        BLUE=1, 
        YELLOW=2, 
        RED=3, 
        GREEN=4 
    }
    public enum Rotation 
    { 
        NONE, 
        RIGHT, 
        MIRROR, 
        LEFT
    }
    public enum PieceKind
    {
        MONO, DOMINO,
        TRIO_L, TRIO_I,
        TETRO_O, TETRO_T, TETRO_I, TETRO_L, TETRO_Z,
        PENTO_L, PENTO_T, PENTO_V, PENTO_S, PENTO_Z, PENTO_I, PENTO_P, PENTO_W, PENTO_U, PENTO_R, PENTO_X, PENTO_Y
    }
}
