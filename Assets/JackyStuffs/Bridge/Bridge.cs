using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class Bridge : MonoBehaviour
{
    public GameObject bridgeCollider;
    public GameObject bridgeStar;
    [HideInInspector]public List<Vector3> pointPosition;
    [HideInInspector]public List<GameObject> bridges;
    [HideInInspector] public List<GameObject> stars;

    private LineRenderer lineRenderer;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

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

    GameObject bridgeHolder = null;
    private void CreateBridge()
    {
        DestroyBridge();

        bridgeHolder = new GameObject(gameObject.name + " Holder");
        for (int i = 0; i < pointPosition.Count - 1; i++)
        {
            Vector3 midpoint = (pointPosition[i] + pointPosition[i + 1]) / 2;
            GameObject temp = Instantiate(bridgeCollider, midpoint, Quaternion.identity);
            temp.transform.rotation = Quaternion.LookRotation(pointPosition[i + 1] - temp.transform.position);
            bridges.Add(temp);
            temp.transform.SetParent(bridgeHolder.transform);

            BoxCollider bridgeBoxCollider = temp.GetComponent<BoxCollider>();
            float pointDistance = Vector3.Distance(pointPosition[i], pointPosition[i + 1]);
            bridgeBoxCollider.size += new Vector3(0, 0, pointDistance - 1);

            if (i == 0)
            {
                GameObject frontStar = Instantiate(bridgeStar, pointPosition[i], Quaternion.identity);
                stars.Add(frontStar);
                frontStar.transform.SetParent(bridgeHolder.transform);
            }
            GameObject endStar = Instantiate(bridgeStar, pointPosition[i + 1], Quaternion.identity);
            stars.Add(endStar);
            endStar.transform.SetParent(bridgeHolder.transform);
        }

        lineAnimationCancelSource = new CancellationTokenSource();
        UpdateLineWidth(pointPosition.Count, lineAnimationCancelSource.Token);
    }

    private void DestroyBridge()
    {
        if (bridgeHolder != null)
            Destroy(bridgeHolder);
        bridges.Clear();
        stars.Clear();

        if (lineAnimationCancelSource != null)
        {
            lineAnimationCancelSource.Cancel();
            lineAnimationCancelSource.Dispose();
            lineAnimationCancelSource = null;
        }
        lineRenderer.positionCount = 0;
    }

    private CancellationTokenSource lineAnimationCancelSource = null;
    private async void UpdateLineWidth(int pointCount, CancellationToken token)
    {
        if (pointCount < 2) return;

        lineRenderer.positionCount = pointCount;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        for (int i = 0; i < pointCount; i++)
        {
            lineRenderer.SetPosition(i, pointPosition[0]);
        }

        try
        {
            lineRenderer.SetPosition(0, pointPosition[0]);
            for (int i = 1; i < pointCount; i++)
            {
                float percentage = 0;
                do
                {
                    lineRenderer.SetPosition(i, Vector3.Lerp(pointPosition[i - 1], pointPosition[i], percentage));
                    percentage += 0.1f;
                    await Task.Delay(10, token);
                } while (percentage < 1);
            }

            while (true)
            {
                lineRenderer.startWidth = Mathf.Sin(Time.timeSinceLevelLoad) * 0.09f;
                lineRenderer.endWidth = Mathf.Sin(Time.timeSinceLevelLoad) * 0.09f;
                for(int i = 0; i < stars.Count; i++)
                {
                    stars[i].transform.position += new Vector3(0, Mathf.Sin(Time.timeSinceLevelLoad + i * 2) * 0.01f, 0);
                }
                await Task.Delay(10, token);
            }
        }
        catch (System.OperationCanceledException) when (token.IsCancellationRequested)
        {
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            return;
        }
    }

    private void OnApplicationQuit()
    {
        lineAnimationCancelSource.Cancel();
        lineAnimationCancelSource.Dispose();
        lineAnimationCancelSource = null;
    }
}
