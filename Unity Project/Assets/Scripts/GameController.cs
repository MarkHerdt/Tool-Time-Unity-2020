using System.Collections.Generic;
using UnityEngine;

namespace ToolTime
{
    public class GameController : MonoBehaviour
    {
        // Value to win the game at the end of the countdown
        public float percentageToWin = 0.75f;
        float totalDamageBroadcasted;

        // Text objects for end screen
        public GameObject levelCompleteUI;

        private void Start()
        {
            // Subscribe to global events
            EventSystemController.self.onGlobalTimerEnd += OnGlobalTimerEnd;
            EventSystemController.self.onBroadcastTotalDamage += PassDamageValue;

            levelCompleteUI.SetActive(false);

            //Cursor.visible = false;
        }

        private void PassDamageValue(float value)
        {
            totalDamageBroadcasted = value;
            if (totalDamageBroadcasted > percentageToWin)
            {
                EventSystemController.self.VictoryIsInDanger(true);
                //Debug.Log("Sieg ist in Gefahr!");
            }
            else
            {
                EventSystemController.self.VictoryIsInDanger(false);
            }
        }

        private void OnGlobalTimerEnd()
        {
            //Debug.Log("Timer ist abgelaufen");
            //Debug.Log("Gesamtschaden: " + (totalDamageBroadcasted * 100) + "%, zu erreichender Wert: "+ (percentageToWin * 100));

            // show end screen UI
            levelCompleteUI.SetActive(true);

            if (totalDamageBroadcasted <= percentageToWin)
            {
                // Game won

                Debug.Log("<color=green>!!! Spiel gewonnen !!!</color>");
            }
            else
            {
                // Game lost

                Debug.Log("<color=red>!!! Spiel verloren !!!</color>");
            }
        }
    }
}
