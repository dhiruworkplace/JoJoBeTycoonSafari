using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UI;

public class TyperTest : MonoBehaviour
{

    public float charsPerSeconds = 0.2f;

    private string content;

    private Text textTest;

    private float timer;

    private int currentPos;

    private bool isActive;

    // Use this for initialization

    void Start()
    {

        textTest = gameObject. GetComponent<Text>();

        content = textTest.text;

        charsPerSeconds = Mathf.Max(0.2f, charsPerSeconds);

        timer = charsPerSeconds;

        isActive = false;

        currentPos = 0;

    }

    // Update is called once per frame

    void Update()
    {

        if (isActive == true)
        {
            StartTyperEffect();
        }
    }

    public void TyperEffect()
    {
        if (textTest == null)
        {
            textTest = gameObject.GetComponent<Text>();
        }
        content = textTest.text;
        textTest.text = null;
        currentPos = 0;
        isActive = true;
    }

    private void StartTyperEffect()
    {
        timer += Time.deltaTime*100f;
        if (timer > charsPerSeconds)
        {
            timer -= charsPerSeconds;
            currentPos++;
            textTest.text = content.Substring(0, currentPos);
            if (currentPos >= content.Length)
            {
                FinishTyperEffect();
            }
        }
    }

    private void FinishTyperEffect()
    {
        isActive = false;
        timer = charsPerSeconds;
        currentPos = 0;
        textTest.text = content;

    }

}