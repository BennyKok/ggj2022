using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ai : MonoBehaviour
{
    [SerializeField] Rigidbody rig;
    [SerializeField] Animator an;
    [SerializeField] GameObject[] route;

    [SerializeField]
    GameObject playerTransform
    {
        get => gameObject;
    }

    [SerializeField] int speedX = 0, speedY = 0;
    [SerializeField] BoxCollider box;

    public bool dead = false, Light = true, alert = false;

    // public static ai Instance;
    int directionX, directionY, tmpDirectX;
    float changeDirectionTime = 0, distance=10;
    GameObject controlledplayer;
    RaycastHit hit;


    // Start is called before the first frame update

    void Start()
    {
        controlledplayer = GameObject.FindWithTag("Player");
        DayNightSwitcher.Instance.SwitchDayNightEvent += DayNightSwitch;
        directionX = 1;
    }

    // Update is called once per frame
    void Update()
    { 
        /*if (distance < Vector3.Distance(playerTransform.transform.position, controlledplayer.transform.position))
        {
            if (Physics.Raycast(playerTransform.transform.position, new Vector3(-1, 0, 0), out hit))
            {
                Debug.Log("1");
                if (hit.collider.tag == "Player")
                {
                    alert = true;
                    Debug.Log("2");
                }
            }
        }
        else
            alert = false;*/
        if (!dead)
        {
            if (!Light)
            {
                box.isTrigger = true;
                if (alert)
                {
                    directionX = controlledplayer.transform.position.x < playerTransform.transform.position.x ? -1 : 1;
                    playerTransform.transform.position = Vector3.MoveTowards(playerTransform.transform.position, controlledplayer.transform.position, 2 * Time.deltaTime);
                    an.Play("alert");
                    playerTransform.transform.localScale = new Vector3(-directionX, 1, 1);
                }
                else {
                    heightconditiion();
                        if (directionX == 0)
                            directionX = tmpDirectX;
                        an.Play("patrol");
                        if (playerTransform.transform.position.x < route[0].transform.position.x)
                            directionX = 1;
                        else if(playerTransform.transform.position.x > route[1].transform.position.x)
                            directionX = -1;
                    playerTransform.transform.localScale = new Vector3(-directionX, 1, 1);
                }
            }
            else
            {
                box.isTrigger = false;
                an.Play("box");
                if (directionX != 0)
                    tmpDirectX = directionX;
                directionX = 0;
                directionY = -1;
            }
        }
    }

    private void FixedUpdate()
    {
        rig.velocity = new Vector2(directionX * Time.fixedDeltaTime * speedX,
            directionY * Time.fixedDeltaTime * speedY * 9.81f);
    }

    public void isdead()
    {
        dead = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            CoreRef.Instance.playerEntity.OnDamage(1);

        if (other.CompareTag("CollectBox"))
        {
            isdead();
        }
    }

    void DayNightSwitch(bool isLight)
    {
        Light = isLight;
    }

    private void OnDestroy()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent -= DayNightSwitch;
    }
    void heightconditiion()
    {
            if (playerTransform.transform.position.y < route[0].transform.position.y - 1)
                directionY = 1;
            else if (playerTransform.transform.position.y > route[0].transform.position.y + 1)// moved up at alert
                directionY = -1;
            else
                directionY = 0;
    }
}
