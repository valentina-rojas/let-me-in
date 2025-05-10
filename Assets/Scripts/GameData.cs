public static class GameData
{
    private static int nivelActual = 1;
    private static int faltas = 0;
    private static int dialogosOmitidos = 0;
    private static float tiempoTotal = 0f; 

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

        public static int DialogosOmitidos
    {
        get { return dialogosOmitidos; }
        set { dialogosOmitidos = value; }
    }

    public static float TiempoTotal
    {
        get { return tiempoTotal; }
        set { tiempoTotal = value; }
    }
}

