using Meta.XR.MRUtilityKit;
using UnityEngine;

public class FollowQRCode : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    private Transform _target;

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        _target = trackable.transform;
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        if (_target == trackable.transform)
        {
            _target = null;
        }
    }

    void Update()
    {
        if (_target != null)
        {
            transform.position = _target.position + offset;
        }
    }
}
