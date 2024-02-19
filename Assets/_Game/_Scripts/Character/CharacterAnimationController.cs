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


    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement3D>();
        animator = GetComponentInChildren<Animator>();
    }

    void LateUpdate()
    {
        animator.SetFloat(CharacterStringsConstants.percentReachMaxVelocity, Mathf.Abs(characterMovement.PercentToReachMaxVelocity));

    }

}
