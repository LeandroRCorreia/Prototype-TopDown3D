using UnityEngine;

public static class CharacterStringsConstants
{
    public static readonly string percentReachMaxVelocity = "percentReachMaxVelocity";
}

[RequireComponent(typeof(CharacterMovement3D))]
public class CharacterAnimationController : MonoBehaviour
{

    private CharacterMovement3D characterMovement;

    private Animator animator;


    private readonly static int speedPercentHash = Animator.StringToHash(CharacterStringsConstants.percentReachMaxVelocity);


    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement3D>();
        animator = GetComponentInChildren<Animator>();
    }

    void LateUpdate()
    {
        var speedPercent = characterMovement.CurrentVelocity.sqrMagnitude / (characterMovement.MaxSpeed * characterMovement.MaxSpeed);
        animator.SetFloat(speedPercentHash, speedPercent);

    }

}
