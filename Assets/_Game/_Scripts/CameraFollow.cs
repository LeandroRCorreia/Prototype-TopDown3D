using UnityEngine;

[ExecuteAlways]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance;
    [SerializeField] private float xAngle;


    void LateUpdate()
    {
        if(target != null)
        {
            Quaternion rotation = Quaternion.Euler(xAngle, 0, 0);
            transform.SetPositionAndRotation(target.position - transform.forward * distance, rotation);
        }

    }

}
