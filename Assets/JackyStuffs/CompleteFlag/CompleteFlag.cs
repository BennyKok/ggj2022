using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class CompleteFlag : MonoBehaviour
{
    public string nextLevelName;

    private Image FadeBlackImage;

    private void Start()
    {
        GameObject UI = GameObject.FindGameObjectWithTag("UI");
        FadeBlackImage = UI.transform.GetChild(1).GetComponent<Image>();

        FadeTransparentEffect();
    }

    private async void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            await FadeBlackEffect();
            SceneManager.LoadScene(nextLevelName);
        }
    }

    private async Task FadeBlackEffect()
    {
        FadeBlackImage.color = new Color32(8, 3, 20, 0);
        while (FadeBlackImage.color.a < 0.9f)
        {
            FadeBlackImage.color += new Color32(0, 0, 0, 5);
            await Task.Delay(10);
        }
        FadeBlackImage.color = new Color32(8, 3, 20, 255);
    }

    private async void FadeTransparentEffect()
    {
        FadeBlackImage.color = new Color32(8, 3, 20, 255);
        while (FadeBlackImage.color.a > 0.1f)
        {
            FadeBlackImage.color -= new Color32(0, 0, 0, 5);
            await Task.Delay(10);
        }
        FadeBlackImage.color = new Color32(8, 3, 20, 0);
    }
}
