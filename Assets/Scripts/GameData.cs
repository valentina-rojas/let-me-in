public static class GameData
{
    private static int nivelActual = 2;
    private static int faltas = 0;
    public static int NivelActual
    {
        get { return nivelActual; }
        set { nivelActual = value; }
    }

    public static int Faltas
    {
        get { return faltas; }
        set { faltas = value; }
    }
}

