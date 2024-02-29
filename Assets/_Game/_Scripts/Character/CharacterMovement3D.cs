using UnityEngine;

public class CharacterMovement3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeedXZ = 15f;
    [SerializeField] private float acceleration = 50;
    [SerializeField] private float deceleration = 45;
    [SerializeField] private float speedRotationY = 20f;

    private Vector3 targetVelocity;
    private Vector3 direction;

    [Header("Collision")]

    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;

    [SerializeField] private Vector3 colliderSize;
    private Vector3 ColliderExtents => colliderSize * 0.5f;
    private Vector3 ColliderCenter => transform.position + Vector3.up * ColliderExtents.y;

    private RaycastHit[] hitHorizontal = new RaycastHit[15];
    private RaycastHit[] hitVertical = new RaycastHit[15];
    
    public Vector3 CurrentVelocity {get; private set;}
    public float MaxSpeed => maxSpeedXZ;

    public void SetInput(float horizontal, float vertical)
    {

        direction = new Vector3(horizontal, 0, vertical).normalized;
        targetVelocity = maxSpeedXZ * direction; 
    }

    void FixedUpdate()
    {
        CurrentVelocity = ProcessVelocity();
        Quaternion targetRotation = ProcessRotation();

        CheckHorizontalCollision();
        float projectedPointPositionY = CheckVerticalCollision();

        var lastPosition = transform.position;
        var targetPosition = lastPosition + CurrentVelocity * Time.fixedDeltaTime;
        targetPosition.y = projectedPointPositionY;

        transform.SetPositionAndRotation(targetPosition, targetRotation);
    }

    private Vector3 ProcessVelocity()
    {
        Vector3 copyCurrentVelocity;

        if(CurrentVelocity.sqrMagnitude <= targetVelocity.sqrMagnitude)
        {
            copyCurrentVelocity = Vector3.MoveTowards(CurrentVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
        }
        else
        {
            copyCurrentVelocity = Vector3.MoveTowards(CurrentVelocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
        }

        return copyCurrentVelocity;
    }

    private Quaternion ProcessRotation()
    {
        Quaternion copyCurrentRotation = transform.rotation;
        if (!(direction == Vector3.zero))
        {
            var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            copyCurrentRotation = Quaternion.Slerp(copyCurrentRotation, targetRotation, speedRotationY * Time.fixedDeltaTime);

        }

        return copyCurrentRotation;
    }

    private void CheckHorizontalCollision()
    {
        var rayLenght = CurrentVelocity * Time.fixedDeltaTime;

        int hitCount = Physics.BoxCastNonAlloc(
        ColliderCenter,
        ColliderExtents,
        direction, 
        hitHorizontal,
        transform.rotation,
        rayLenght.magnitude,
        whatIsWall);

        for (int i = 0; i < hitCount; i++)
        {
            var hit = hitHorizontal[i];
            Vector3 projectedVector = CalculateWallSliding(hit);
            CurrentVelocity = new Vector3(projectedVector.x, CurrentVelocity.y, projectedVector.z);

        }

    }
    
    private float CheckVerticalCollision()
    {
        int hitCount = Physics.RaycastNonAlloc(
        ColliderCenter,
        -transform.up,
        hitVertical,
        rayDistance,
        whatIsGround);

        float projectedPointPositionY = 0;
        for (int i = 0; i < hitCount; i++)
        {
            projectedPointPositionY = GetPointPositionFromPlane(i);
        }

        return projectedPointPositionY;

        float GetPointPositionFromPlane(int i)
        {
            float projectedPointPositionY;
            var hit = hitVertical[i];
            var pointToProject = transform.position - hit.point;
            var projectedPoint = Vector3.ProjectOnPlane(pointToProject, hit.normal);
            projectedPointPositionY = hit.point.y + projectedPoint.y;
            return projectedPointPositionY;
        }
        
    }

    private Vector3 CalculateWallSliding(RaycastHit hit)
    {
        const float maxCos = 1;
        const float minCosToConsiderCollinear = 0.65f;
        Vector3 projectedCurrentVelocityOnPlane;

        var inverseNormal = hit.normal * -1;
        var velocityMagnitude = CurrentVelocity.magnitude;
        var dot = Vector3.Dot(CurrentVelocity, inverseNormal);
        var cos = dot / (velocityMagnitude * inverseNormal.magnitude);


        var isApproximatellyCollinears = Mathf.Approximately(cos, maxCos) || (cos >= minCosToConsiderCollinear && cos <= maxCos);
        if (!isApproximatellyCollinears)
        {
            projectedCurrentVelocityOnPlane = Vector3.ProjectOnPlane(CurrentVelocity, hit.normal);
        }
        else
        {
            projectedCurrentVelocityOnPlane = Vector3.ProjectOnPlane(Quaternion.Euler(0, 65, 0) * CurrentVelocity, hit.normal);

        }



        return projectedCurrentVelocityOnPlane.normalized * velocityMagnitude;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(ColliderCenter, colliderSize);


        Gizmos.color = Color.blue;
        Gizmos.DrawRay(ColliderCenter, -transform.up * rayDistance);
        


    }

}
