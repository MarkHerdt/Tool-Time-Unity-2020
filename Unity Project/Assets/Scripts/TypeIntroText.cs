using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TypeIntroText : MonoBehaviour
{
    Text textBodyUI;
    public float letterPause = 0.04f;

    public GameObject buttonContinue;

    string message;

    bool skip;
    bool textFinished;

    // Use this for initialization
    void Start()
    {
        textBodyUI = GetComponent<Text>();
        message = textBodyUI.text;
        textBodyUI.text = "";
        StartCoroutine(TypeText());

        buttonContinue.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (textFinished)
            {
                LoadMenu();
            }
            skip = true;
        }
    }

    void LoadMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator TypeText()
    {
        foreach (char letter in message.ToCharArray())
        {
            textBodyUI.text += letter;
            if (letter.Equals('\n') || letter.Equals('.') || letter.Equals('!'))
            {
                if (!skip)
                {
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (!skip)
                {
                    yield return new WaitForSeconds(letterPause);
                }
                else
                {
                    break;
                }
            }
        }

        textBodyUI.text = message;
        skip = false;
        textFinished = true;
        buttonContinue.SetActive(true);
    }
}
