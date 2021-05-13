using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    enum TimeModes { easy = 120, medium = 90, hard = 60 }

    public float timeLeft;
    public Text countdown;
    bool timerRunning = false;
    bool countdownEnd = false;
    public string difficulty;

    float causeDamageInterval = 7.5f;
    float randomTime = 2.5f;

    public float internalTickCountdown;

    public bool TimerRunning
    {
        get { return timerRunning; }
        set { timerRunning = value; }
    }

    public bool CountdownEnd
    {
        get { return countdownEnd; }
    }


    void Start()
    {
        ResetTime();
        Time.timeScale = 1f;
        internalTickCountdown = causeDamageInterval;
        // Start Countdown
        TimerRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    TimerRunning = !TimerRunning;
        //}
        //else if (Input.GetKeyDown(KeyCode.R))
        //{
        //    ResetTime();
        //}

        LoseTime();
        countdown.text = Mathf.RoundToInt(timeLeft).ToString();
    }

    void ResetTime(TimeModes mode = TimeModes.easy)
    {
        timerRunning = false;
        countdownEnd = false;
        timeLeft = (float)mode;
    }
    void LoseTime()
    {
        if (TimerRunning)
        {
            timeLeft -= Time.deltaTime;
            internalTickCountdown -= Time.deltaTime;

            // Tick for new object damage
            if (internalTickCountdown <= 0)
            {
                internalTickCountdown = causeDamageInterval + Random.Range((randomTime * -1) / 2, randomTime / 2);
                //Debug.Log("<color=yellow>Timer Event:</color> "+ (int)causeDamageInterval + " Sekunden-Tick ausgelöst bei " + (int)timeLeft + " Sekunden Restzeit");
                //Debug.Log("<color=yellow>Timer Event:</color> Neues Interval " + (int)internalTickCountdown + " Sekunden");
                IntervalTick();
            }

            // Global timer ends
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                internalTickCountdown = 0;
                countdownEnd = true;
                TimerRunning = false;
                countdown.text = "";
                OnTimerEnd();
            }
        }
    }

    private void IntervalTick()
    {
        EventSystemController.self.RandomObjectBreaks();
    }

    private void OnTimerEnd()
    {
        EventSystemController.self.GlobalTimerEnd();
    }
}
