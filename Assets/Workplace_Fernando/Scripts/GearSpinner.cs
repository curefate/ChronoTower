using UnityEngine;

public class GearSpinner : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public Axis rotationAxis = Axis.Z;
    public float speed = 100f;

    void Update()
    {
        Vector3 axis = Vector3.zero;

        switch (rotationAxis)
        {
            case Axis.X:
                axis = Vector3.right;
                break;

            case Axis.Y:
                axis = Vector3.up;
                break;

            case Axis.Z:
                axis = Vector3.forward;
                break;
        }

        transform.Rotate(axis * speed * Time.deltaTime);
    }
}