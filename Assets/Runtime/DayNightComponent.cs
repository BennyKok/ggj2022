using UnityEngine;

public abstract class DayNightComponent : MonoBehaviour
{
    protected virtual void Start()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent += OnDayNightSwitch;
    }

    protected abstract void OnDayNightSwitch(bool isLight);

    protected virtual void OnDestroy()
    {
        DayNightSwitcher.Instance.SwitchDayNightEvent -= OnDayNightSwitch;
    }
}