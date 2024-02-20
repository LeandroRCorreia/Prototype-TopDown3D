using UnityEngine;

[RequireComponent(typeof(CharacterMovement3D))]
public class EnemyIAController : MonoBehaviour
{

    private CharacterMovement3D characterMovement3D;

    [Header("FOV Parameters")]
    [SerializeField] private FieldOfView3D fieldOfView3D;
    [SerializeField] private Transform muzzle;

    [SerializeField] private float maxTimeOutFieldOfViewToForget = 4f;
    private float lastTimeInsideFieldOfVIew = Mathf.NegativeInfinity;
    private bool IsInMemoryTarget => Time.time <= lastTimeInsideFieldOfVIew + maxTimeOutFieldOfViewToForget;
    
    private PlayerController playerReference;


    void Awake()
    {
        characterMovement3D = GetComponent<CharacterMovement3D>();
    }

    void Start()
    {
        playerReference = PlayerController.PlayerInstance;
    }

    void Update()
    {
        var isInsideFieldOfView = fieldOfView3D.IsTargetInsideFOV(playerReference.transform);

        if (isInsideFieldOfView || IsInMemoryTarget)
        {
            lastTimeInsideFieldOfVIew = isInsideFieldOfView ? Time.time : lastTimeInsideFieldOfVIew;
            var vector = (playerReference.transform.position - transform.position).normalized;
            characterMovement3D.SetInput(vector.x, vector.z);
        }
        else
        {
            characterMovement3D.SetInput(0, 0);
        }

    }

}
