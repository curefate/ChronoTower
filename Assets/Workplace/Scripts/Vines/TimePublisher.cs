using System.Collections.Generic;
using UnityEngine;

public class TimePublisher : MonoBehaviour
{
    public static TimePublisher Instance { get; private set; }
    [SerializeField] private List<ITimeListener> listeners;

    private bool _isKickedOff;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        listeners = new List<ITimeListener>();
    }

    public void NotifyTimeChanged(TimeEventType eventType)
    {
        foreach (var listener in listeners)
        {
            listener.OnTimeChanged(eventType);
        }
    }

    public void RegisterListener(ITimeListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(ITimeListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }

    private void KickOff()
    {
        NotifyTimeChanged(TimeEventType.TimeReversed);
    }

    private void Update()
    {
        if (!_isKickedOff)
        {
            KickOff();
            _isKickedOff = true;
        }
    }
}

public enum TimeEventType
{
    None,
    TimeProgressed,
    TimeReversed,
    //TimePaused,
    //TimeResumed
}

public interface ITimeListener
{
    void OnTimeChanged(TimeEventType eventType);
}