using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ai : MonoBehaviour
{
    [SerializeField] Rigidbody2D rig;
    [SerializeField] Animator an;
    [SerializeField] GameObject[] route;
    [SerializeField] Transform playerTransform;
    [SerializeField] int speedX =0, speedY=0;
    [SerializeField]bool dead = false, light = true, alert= false;
    int directionX, directionY, tmpDirectX;
    float changeDirectionTime=0;
    // Start is called before the first frame update
    void Start()
    {
        directionX = 1;
        rig.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            if (light)
            {
                if (playerTransform.position.y < route[0].transform.position.y)
                    directionY = 1;
                else
                {
                    directionY = 0;
                    if (directionX == 0)
                        directionX = tmpDirectX;
                    if (alert)
                    {
                        /*an.Play("alert");*/
                    }
                    else
                    {
                        /*an.Play("normal");*/
                        if ((playerTransform.position.x < route[0].transform.position.x || playerTransform.position.x > route[1].transform.position.x) && Time.time - changeDirectionTime > 1f)
                        {
                            changeDirectionTime = Time.time;
                            directionX = -directionX;
                        }
                    }
                }
            }
            else
            {
                if(directionX!= 0)
                    tmpDirectX = directionX;
                directionX = 0;
                directionY = -1;
            }
        }
    }
    private void FixedUpdate()
    {
        rig.velocity = new Vector2(directionX * Time.fixedDeltaTime* speedX, directionY * Time.fixedDeltaTime * speedY* 9.81f);
    }
    void isdead()
    {
        dead = true;
    }
}
