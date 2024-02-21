using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement3D))]
public class PlayerController : MonoBehaviour
{

    private CharacterMovement3D characterMovement3D;

    private InputActions playerInputs;

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
        playerInputs = new();

    }

    void Start()
    {
        playerInputs.Enable();
        playerInputs.Player.Attack.performed += OnPlayerAttack;
    }



    void Update()
    {
        Vector2 input = playerInputs.Player.Movement.ReadValue<Vector2>();




        characterMovement3D.SetInput(input.x, input.y);
    }

    private void OnPlayerAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attacking!");
    }

}
