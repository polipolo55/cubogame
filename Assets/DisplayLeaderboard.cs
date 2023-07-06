using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class DisplayLeaderboard : MonoBehaviour
{
    public GameObject textPrefab;
    public Transform textContainer;
    public float verticalSpacing = 20f;

    public Dictionary<float, string> Leaderboard; 
    private void Start()
    {
        Leaderboard = GameManager.Instance.getLeaderboard();
        float yOffset = 0f;
        if (Leaderboard != null)
        {
            foreach (var entry in Leaderboard)
            {
                GameObject textObject = Instantiate(textPrefab, textContainer);
                TextMeshProUGUI textMeshPro = textObject.GetComponent<TextMeshProUGUI>();
                TimeSpan time = TimeSpan.FromSeconds(entry.Key);
                string properTime = time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00") + "." + time.Milliseconds.ToString("00");
                textMeshPro.text = string.Format("{0}: {1}", properTime, entry.Value);

                textObject.transform.localPosition = new Vector3(0f, yOffset, 0f);
                yOffset -= verticalSpacing;
            }
        }
    }
}
