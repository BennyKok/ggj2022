using System.Collections;
using System.Collections.Generic;
using BennyKok.Bootstrap;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : Singleton<DeathScreen>
{
    // static dead instance;
    Image I;
    [SerializeField] List<Sprite> pic = new List<Sprite>();
    int Count;

    bool died = true; // change it if u want

    // Start is called before the first frame update
    void Start()
    {
        // instance = this;
        I = GetComponent<Image>();
        if (I.sprite == null)
        {
            I.sprite = pic[0];
        }
    }

    void RestKey()
    {
        if (!Input.anyKeyDown) return;

        I.sprite = pic[0];
        Count = 0;
    }

    private GameObject @from;

    public void OK(GameObject from)
    {
        this.from = from;
    }

// Update is called once per frame
    void Update()
    {
        //3 sprite 0 got 0 g, sprite 2 got 2g
        if (died)
        {
            switch (Count)
            {
                case 0:
                    if (Input.GetKeyDown("g"))
                    {
                        I.sprite = pic[1];
                        Count = 1;
                    }
                    else
                    {
                        RestKey();
                    }

                    break;
                case 1:
                    if (Input.anyKeyDown && !Input.GetKeyDown("g"))
                    {
                        I.sprite = pic[0];
                        Count = 0;
                    }
                    else if (Input.GetKeyDown("g"))
                    {
                        I.sprite = pic[2];
                        Count = 2;
                        
                        StopCoroutine("Restart");
                        StartCoroutine("Restart");
                    }
                    else
                    {
                        RestKey();
                    }

                    break;
                case 2:
                    //restart
                    if (Input.GetKeyDown("j"))
                    {
                        StopCoroutine("Restart");
                        StartCoroutine("RestartDelay");
                        Debug.Log("Happy game jam");
                    }
                    else
                    {
                        RestKey();
                    }

                    break;
            }
        }
    }


    // // delay 2 seconds and reload scene
    // public void Restart()
    // {
    //     transform.DOShakePosition(2f, 10f, 20, 90, false, true);
    //     Invoke("RestartScene", 2f);
    // }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(from.scene.name);
    }

    IEnumerator RestartDelay()
    {
        transform.DOShakePosition(2f, 10f, 20, 90, false, false);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(from.scene.name);
    }

    // void RestartScene()
    // {
    //     SceneManager.LoadScene(from.scene.name);
    // }

    public override void OnAwake()
    {
        gameObject.SetActive(false);
    }
}