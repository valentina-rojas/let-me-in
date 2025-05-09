using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticVariables : MonoBehaviour
{
    public static class SessionData
    {
        public static int level = 1;

        public static int time;

        public static int strikes;

        public static int dlgSkipped;

        public static int charIndex;

        public static int reactionTime;

        public static string reason;

        public static bool cinSkipped = false;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
