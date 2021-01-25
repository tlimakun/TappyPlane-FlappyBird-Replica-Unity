using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Countdown : MonoBehaviour
{
    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;

    private Text countdown;

    private void OnEnable()
    {
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            countdown.text = (count - i).ToString();
            yield return new WaitForSeconds(1);
        }

        OnCountdownFinished();
    }
}
