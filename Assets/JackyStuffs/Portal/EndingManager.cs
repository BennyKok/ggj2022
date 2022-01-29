using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Video;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public Image fadeBlackImage;
    public CreditRoll credit;
    private TextMeshProUGUI theEnd;
    public VideoPlayer stopMotion;
    private async void Awake()
    {
        theEnd = fadeBlackImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        theEnd.gameObject.SetActive(false);

        stopMotion.time = 0;
        stopMotion.Play();
        stopMotion.Pause();

        fadeBlackImage.color = new Color32(8, 3, 20, 255);
        await Task.Delay(4000);
        await FadeTransparentEffect();
        stopMotion.Play();
        do { await Task.Yield(); } while (stopMotion.isPlaying);
        await Task.Delay(2000);
        theEnd.gameObject.SetActive(true);
        await FadeBlackEffect();
        await Task.Delay(2000);
        theEnd.gameObject.SetActive(false);
        credit.CreateRollingText();
        credit.ingameText.text = credit.GenerateTextLogic();
        credit.Show();
        bool finished = await credit.ScrollCredit(credit.startPosition, credit.endPosition, credit.scrollSecond);

        SceneManager.LoadScene("startmenu");
    }


    private async Task FadeTransparentEffect()
    {
        fadeBlackImage.color = new Color32(8, 3, 20, 255);
        theEnd.color = new Color32(255, 255, 255, 255);
        while (fadeBlackImage.color.a > 0.1f)
        {
            theEnd.color -= new Color32(0, 0, 0, 5);
            fadeBlackImage.color -= new Color32(0, 0, 0, 5);
            await Task.Delay(10);
        }
        fadeBlackImage.color = new Color32(8, 3, 20, 0);
        theEnd.color = new Color32(255, 255, 255, 0);
    }

    private async Task FadeBlackEffect()
    {
        fadeBlackImage.color = new Color32(8, 3, 20, 0);
        theEnd.color = new Color32(255, 255, 255, 0);
        while (fadeBlackImage.color.a < 0.9f)
        {
            theEnd.color += new Color32(0, 0, 0, 5);
            fadeBlackImage.color += new Color32(0, 0, 0, 5);
            await Task.Delay(10);
        }
        fadeBlackImage.color = new Color32(8, 3, 20, 255);
        theEnd.color = new Color32(255, 255, 255, 255);
    }
}
