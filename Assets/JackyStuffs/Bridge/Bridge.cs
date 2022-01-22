using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public GameObject bridgePrefab;
    [HideInInspector]public List<Vector3> pointPosition;
    [HideInInspector]public List<BoxCollider> bridges;
    private void Awake()
    {
        pointPosition = new List<Vector3>();

        for(int i = 0; i < transform.childCount; i++)
        {
            pointPosition.Add(transform.GetChild(i).position);
        }
    }

    private void Start()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent += OnSwitch;
    }

    private void OnDestroy()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent -= OnSwitch;
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
    }

    private void OnSwitch()
    {
        if (DayNightSwitcher.Instance.currentDayNight == DayNightSwitcher.DayNightEnum.day)
        {
            CreateBridge();
        }
        else
        {
            DestroyBridge();
        }
    }

    private void CreateBridge()
    {
        DestroyBridge();

        for(int i = 0; i < pointPosition.Count - 1; i++)
        {
            Vector3 midpoint = (pointPosition[i] + pointPosition[i + 1]) / 2;
            GameObject temp = Instantiate(bridgePrefab, midpoint, Quaternion.identity);
            temp.transform.rotation = Quaternion.LookRotation(pointPosition[i + 1] - temp.transform.position);

            bridges.Add(temp.GetComponent<BoxCollider>());
            float pointDistance = Vector3.Distance(pointPosition[i], pointPosition[i + 1]);
            temp.transform.localScale += new Vector3(0, 0, pointDistance - 1);
        }
    }

    private void DestroyBridge()
    {
        foreach (BoxCollider b in bridges)
        {
            Destroy(b.gameObject);
        }
        bridges.Clear();
    }
}
