using UnityEngine;

[RequireComponent(typeof(CharacterMovement3D))]
public class PlayerController : MonoBehaviour
{

    private CharacterMovement3D characterMovement3D;

    public static PlayerController PlayerInstance {get; private set;} 

    void Awake()
    {
        if(PlayerInstance == null)
        {
            PlayerInstance = this;
        }
        else
        {
            Destroy(this);
        }
        characterMovement3D = GetComponent<CharacterMovement3D>();

    }

    void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");



        characterMovement3D.SetInput(horizontal, vertical);
    }

}
