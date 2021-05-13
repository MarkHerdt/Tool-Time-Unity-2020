using System.Collections;
using UnityEngine;

public class DMGBar : MonoBehaviour
{
    public Transform dmgBar;
    public SpriteRenderer barSprite;
    float dmgValue;

    bool levelFinished;
    bool victoryIsInDanger;
    public float DMGValue { get => dmgValue; }

    private void Start()
    {
        if (dmgBar == null)
        {
            Debug.LogError("DMGBar: Reference for \"dmgBar\" not set in the inspector");
        }

        // Subscribe to event list
        EventSystemController.self.onGlobalTimerEnd += GlobalTimerEnd;
        EventSystemController.self.onChangeDamageBar += ChangeDamageBar;
        EventSystemController.self.onVictoryIsInDanger += VictoryIsInDanger;

        levelFinished = false;
        victoryIsInDanger = false;

        SetDMG(0);

        ChangeBarColor(Color.yellow);
    }

    void GlobalTimerEnd()
    {
        levelFinished = true;
        if (!victoryIsInDanger)
        {
            ChangeBarColor(Color.green);
        }
    }

    void ChangeDamageBar(float damageValue)
    {
        SetDMG(damageValue);
        if (victoryIsInDanger)
        {
            ChangeBarColor(Color.red);
        }
        else
        {
            ChangeBarColor(Color.yellow);
        }
    }

    void ChangeBarColor(Color color)
    {
        barSprite.color = color;
    }

    void VictoryIsInDanger(bool inDanger)
    {
        if (inDanger)
        {
            victoryIsInDanger = true;
        }
        else
        {
            victoryIsInDanger = false;
        }

    }

    /// <summary>
    /// Sets the bar width of "dmgBar"
    /// </summary>
    /// <param name="dmg"></param>
    void SetDMG(float dmg)
    {
        dmgBar.localScale = new Vector2(dmg, 1);
        //StartCoroutine(HealthBar(dmg));
        dmgValue = dmgBar.localScale.x;
    }

    IEnumerator HealthBar(float dmg)
    {
        for (int i = 0; i < dmg; i++)
        {
            dmgBar.localScale = new Vector2(+.1f, 1);
            yield return new WaitForSeconds(.01f);
        }
    }
}
