using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class dead : MonoBehaviour
{
    static dead instance;
    Image I;
    [SerializeField] List<Sprite> pic = new List<Sprite>();
    static int Count;
    bool died = true; // change it if u want
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        I = instance.GetComponentInParent<Image>();
        if (I.sprite == null)
        {
            I.sprite = pic[0];
        }
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
                        }
                        break;
                    case 2:
                        //restart
                        break;
            }
            
        }

        
            
    }
}
