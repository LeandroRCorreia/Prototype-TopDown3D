using UnityEngine;

public class CharacterMovement3D : MonoBehaviour
{
    [SerializeField] private float maxSpeedXZ = 15f;
    [SerializeField] private float acceleration = 100f;

    [SerializeField] private float speedRotationY = 20f;

    private Vector3 velocity;
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

    public void SetInput(float horizontal, float vertical)
    {

        direction = new Vector3(horizontal, 0, vertical).normalized;
        targetVelocity = maxSpeedXZ * direction; 
    }

    void FixedUpdate()
    {
        velocity = Vector3.MoveTowards(velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        Quaternion targetRotation = ProcessRotation();

        CheckHorizontalCollision();
        float projectedPointPositionY = CheckVerticalCollision();


        var lastPosition = transform.position;
        var targetPosition = lastPosition + velocity * Time.fixedDeltaTime;
        targetPosition.y = projectedPointPositionY;

        transform.SetPositionAndRotation(targetPosition, targetRotation);
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
        var rayLenght = velocity * Time.fixedDeltaTime;

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
            velocity = new Vector3(projectedVector.x, velocity.y, projectedVector.z);

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
        Vector3 projectedVector;

        var inverseNormal = hit.normal * -1;
        var velocityMagnitude = velocity.magnitude;
        var dot = Vector3.Dot(velocity, inverseNormal);
        var cos = dot / (velocityMagnitude / inverseNormal.magnitude);

        var isApproximatellyCollinears = Mathf.Approximately(cos, 1.0f) || cos >= 0.98f && cos <= 1;
        if (!isApproximatellyCollinears)
        {
            projectedVector = Vector3.ProjectOnPlane(velocity, hit.normal);
        }
        else
        {
            projectedVector = Vector3.ProjectOnPlane(Quaternion.Euler(0, 45, 0) * velocity, hit.normal);
        }

        return projectedVector.normalized * velocityMagnitude;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(ColliderCenter, colliderSize);


        Gizmos.color = Color.blue;
        Gizmos.DrawRay(ColliderCenter, -transform.up * rayDistance);
        


    }

}
