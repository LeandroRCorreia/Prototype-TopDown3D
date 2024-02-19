using UnityEngine;

[ExecuteAlways]
public class CameraFollow : MonoBehaviour
{

    [SerializeField] private Transform target;

    [SerializeField] private Vector3 hand;

    [SerializeField] private Vector3 rotation;


    void LateUpdate()
    {

        transform.SetPositionAndRotation(target.position + hand, Quaternion.Euler(rotation));
    }

}
