using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnalyticsEvent = Unity.Services.Analytics.Event;

public class EventManager : MonoBehaviour
{

    public class LevelStartEvent : AnalyticsEvent
    {
        public LevelStartEvent() : base("LevelStart")
        {
        }

        public int level { set { SetParameter("level", value); } }
    }

    public class LevelCompleteEvent : AnalyticsEvent
    {
        public LevelCompleteEvent() : base("LevelComplete")
        {
        }

        public int level { set { SetParameter("level", value); } }

        public int time { set { SetParameter("time", value); } }

        public int strikes { set { SetParameter("strikes", value); } }

        public int dlgSkipped { set { SetParameter("dlgSkipped", value); } }
    }

    public class GameOverEvent : AnalyticsEvent
    {
        public GameOverEvent() : base("GameOver")
        {
        }

        public int level { set { SetParameter("level", value); } }

        public string reason { set { SetParameter("reason", value); } }

    }


    public class GameFinishedEvent : AnalyticsEvent
    {
        public GameFinishedEvent() : base("GameFinished")
        {
        }

        public int time { set { SetParameter("time", value); } }

        public int strikes { set { SetParameter("strikes", value); } }
    }


    public class QuitEvent : AnalyticsEvent
    {
        public QuitEvent() : base("Quit")
        {
        }

        public int level { set { SetParameter("level", value); } }

        public int charIndex { set { SetParameter("charIndex", value); } }
    }


    public class ScanUsedEvent : AnalyticsEvent
    {
        public ScanUsedEvent() : base("ScanUsed")
        {
        }

        public int level { set { SetParameter("level", value); } }

        public int charIndex { set { SetParameter("charIndex", value); } }
    }



    public class EndRiotEvent : AnalyticsEvent
    {
        public EndRiotEvent() : base("EndRiot")
        {
        }

        public int reactionTime { set { SetParameter("reactionTime", value); } }

    }

    public class EndCinEvent : AnalyticsEvent
    {
        public EndCinEvent() : base("EndCin")
        {
        }

        public bool cinSkipped { set { SetParameter("cinSkipped", value); } }

    }

}
