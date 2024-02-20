using UnityEngine;

public class FieldOfView3D : MonoBehaviour
{
    [SerializeField] private float viewDistance = 5;
    [SerializeField] private float viewAngle = 60;

    [SerializeField] private Transform muzzlePoint;

    public bool IsTargetInsideFOV(Transform target)
    {
        if(muzzlePoint == null) return false;
        if(target == null) return false;

        var toTarget = target.position - muzzlePoint.position;

        const float desconsiderYAxisOfCalcule = 0;
        toTarget.y = desconsiderYAxisOfCalcule;

        var toTargetSqrMagnitude = toTarget.sqrMagnitude;
        if(toTargetSqrMagnitude >= viewDistance * viewDistance) return false;

        var dot = Vector3.Dot(muzzlePoint.forward, toTarget);
        if(dot < 0) return false;

        var halfCos = dot / (Mathf.Sqrt(toTargetSqrMagnitude) * muzzlePoint.forward.magnitude);
        var halfAngleDegress = Mathf.Acos(halfCos) * Mathf.Rad2Deg;
        return halfAngleDegress <= (viewAngle * 0.5f);   
    }

    void OnDrawGizmos()
    {
        var playerInstance = PlayerController.PlayerInstance;
        var color = Color.red;
        if(playerInstance != null)
        {
            color = IsTargetInsideFOV(playerInstance.transform) ? Color.green : Color.red;
        }

        Gizmos.color = color;
        Gizmos.DrawWireSphere(muzzlePoint.position, viewDistance);

        var rightDir = Quaternion.Euler(0,viewAngle * 0.5f, 0) * muzzlePoint.forward;
        var leftDir = Quaternion.Euler(0,-viewAngle * 0.5f, 0) * muzzlePoint.forward;


        Gizmos.DrawRay(muzzlePoint.position, rightDir * viewDistance);
        Gizmos.DrawRay(muzzlePoint.position, leftDir * viewDistance);
    }

}
