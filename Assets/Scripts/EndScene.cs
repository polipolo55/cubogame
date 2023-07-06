using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class EndScene : MonoBehaviour
{
    public float eventTimer = 0;
    public string levelName;


    public Canvas canvas;
    public GameObject textPrefab;
    public GameObject leaderBoard;
    public GameObject endOfRun;

    public TextMeshProUGUI maxTimeText;
    public TextMeshProUGUI maxTimeText2;
    public TMP_InputField inputField;


    public Vector2 startingPosition;
    public float lineHeight = 30f;
    public GameObject secret;


    private List<float> floatList;
    private bool textDisplayed = false;
    private bool endDisplayed = false;
    private float sum;


    private void Awake()
    {
        leaderBoard.SetActive(false);
        endOfRun.SetActive(false);
    }

    void Start()
    {
        sum = GameManager.Instance.totalTime();
        floatList = GameManager.Instance.getTimesList();

        Debug.Log(sum);

    }

    void Update()
    {
        eventTimer += Time.deltaTime;

        GameManager.Instance.player.canInput = false;

        if (eventTimer > 0.5f && eventTimer < 0.6f) 
        {
            GameManager.Instance.player.OnJumpInput();
        }
        if (eventTimer > 0.5f) GameManager.Instance.player.moveInput.x = 1;

        if (eventTimer > 2f && !textDisplayed) displayTimes();

        if (eventTimer > 5f && !endDisplayed) displayEnd();

        if (eventTimer > 10f)
        {
            Time.timeScale = 0;
            GameManager.Instance.runEnding = true;
            //SceneManager.LoadScene(levelName);
        }

    }

    void displayTimes()
    {
        textDisplayed = true;
        leaderBoard.SetActive(true);
        TimeSpan sumTime = TimeSpan.FromSeconds(sum);
        string sumText = sumTime.Minutes.ToString("00") + ":" + sumTime.Seconds.ToString("00") + "." + sumTime.Milliseconds.ToString("00");
        maxTimeText.text = string.Format("{0}", sumText);
        if (sumTime.Seconds == 33)
        {
            secret.SetActive(true);
        }

        for (int i = 1; i <= floatList.Count; i++)
        {
            Vector2 position = startingPosition + new Vector2(0f, -lineHeight * i);

            GameObject textObject = Instantiate(textPrefab, Vector3.zero, Quaternion.identity, canvas.transform);

            textObject.GetComponent<RectTransform>().anchoredPosition = position;

            TextMeshProUGUI textMeshPro = textObject.GetComponent<TextMeshProUGUI>();

            TimeSpan time = TimeSpan.FromSeconds(floatList[i]);

            string text = time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00") + "." + time.Milliseconds.ToString("00");

            textMeshPro.text = string.Format("{0}.- {1}", i, text);
        }

        GameManager.Instance.resetPartialList();
    }

    void displayEnd()
    {
        leaderBoard.SetActive(false);
        endOfRun.SetActive(true);

        TimeSpan sumTime = TimeSpan.FromSeconds(sum);
        string sumText = sumTime.Minutes.ToString("00") + ":" + sumTime.Seconds.ToString("00") + "." + sumTime.Milliseconds.ToString("00");
        maxTimeText2.text = string.Format("{0}", sumText);

    }

    public string GetInputData()
    {
        Debug.Log("Input Data: " + inputField.text);
        return inputField.text;
    }

    public void addToMap()
    {
        if (inputField != null) 
        {
            Debug.Log("data: " + GetInputData() + " " + sum);
            GameManager.Instance.addToLeaderboard(inputField.text, sum);
            SceneManager.LoadScene(levelName);
        }
    }

    public void cancel()
    {
        SceneManager.LoadScene(levelName);
    }
}
