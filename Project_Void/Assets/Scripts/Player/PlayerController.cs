using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // LOGIC COMPONENTS
    private PlayerInput playerInput;

    // EVENTS
    public event Action<PlayerInput> OnPlayerInput;

    // VISUAL COMPONENTS
    public Sprite[] idleSprites;

    // DATA/VARIABLES
        // SERIALIZED VARIABLES
    //
        // HIDDEN PUBLIC VARIABLES
    //
        // PRIVATE VARIABLES
    //

    private void Start()
    {
        //
    }



    private void Update ()
    {
        // MOVEMENT
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            playerInput ??= new(InputType.Movement); // if playerInput hasn't been defined
            playerInput.Type = InputType.Movement;
            // set player direction
            playerInput.Direction = Input.GetKey(KeyCode.W) ? Direction.Up
                : Input.GetKey(KeyCode.A) ? Direction.Left
                : Input.GetKey(KeyCode.S) ? Direction.Down
                : Input.GetKey(KeyCode.D) ? Direction.Right
                : playerInput.Direction;
            playerInput.MovementKeyIsPressed = true; // movement key set: TRUE
            OnPlayerInput?.Invoke(playerInput);
            
        }
        else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            if (playerInput == null || playerInput.Direction == Direction.None) return;
            playerInput.Direction = Direction.None; // when no movement keys are held: set direction to none
            if (playerInput.MovementKeyIsPressed) OnPlayerInput?.Invoke(playerInput); // runs once to transition from moving animation to idle
            playerInput.MovementKeyIsPressed = false; // movement key set: FALSE
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Smack");
    }
}
