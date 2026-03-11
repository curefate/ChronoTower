using UnityEngine;

public class HideWall : MonoBehaviour
{
    [SerializeField] private bool ifReverseDirection;
    [SerializeField] private bool ifCanHide;
    public void SetIfCanHide(bool value)
    {
        ifCanHide = value;
        if (!ifCanHide)
        {
            meshRenderer.enabled = true;
        }
    }
    [SerializeField] private float angle = 40f;
    [SerializeField] private MeshRenderer meshRenderer;

    private Transform camPos;

    private void Start()
    {
        camPos = Camera.main.transform;
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void Update()
    {
        if (!ifCanHide) return;

        Vector3 toCam = camPos.position - transform.position;
        toCam = Vector3.ProjectOnPlane(toCam, Vector3.up).normalized;
        float angleToCam = Vector3.Angle(ifReverseDirection ? transform.right : -transform.right, toCam);
        if (angleToCam < angle)
        {
            meshRenderer.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
        }
    }
}
