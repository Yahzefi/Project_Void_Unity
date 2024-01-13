using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    // LOGIC COMPONENTS
    private PlayerController controller;
    private PlayerMovement playerMovement;
    private DialogueManager dialogueManager;

    // EVENTS
    public event Action<PlayerMovement> OnPlayerMovement;

    // VISUAL COMPONENTS
    private Animator animator;

    // DATA/VARIABLES
        // SERIALIZED VARIABLES
        // HIDDEN PUBLIC VARIABLES
    //
        // PRIVATE VARIABLES
    private Vector2 playerPosition;
    private float movementSpeed;
    void Start()
    {
        // subscribing to events
        controller = GetComponent<PlayerController>();
        controller.OnPlayerInput += Init_PlayerInput;
        this.OnPlayerMovement += Init_PlayerMovement;

        // defining components
        //dialogueManager = GetComponent<DialogueManager>();

        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        // defining other values
        playerPosition = transform.position;
        movementSpeed = 2.0f;
    }

    private void Init_PlayerInput(PlayerInput playerInput)
    {
        // MOVEMENT
        if (playerInput.Type == InputType.Movement)
        {
            playerMovement ??= new(playerInput.Direction); // if playerMovement isn't defined: instantiate new one
            playerMovement.Direction = playerInput.Direction;
            OnPlayerMovement?.Invoke(playerMovement); // should only be invoked if input bool set to false after process is completely finished
            return;
        }
    }

    private void Init_PlayerMovement(PlayerMovement playerMovement)
    {
        if (animator == null) return;
        if (dialogueManager != null && dialogueManager.isRunning) return;

        switch (playerMovement.Direction)
        {
            // Move Player: UP
            case Direction.Up:
                // animation (called once)
                if (!animator.GetBool("isUp")) 
                {
                    animator.SetBool("isUp", true);
                    animator.SetBool("isLeft", false);
                    animator.SetBool("isDown", false);
                    animator.SetBool("isRight", false); 
                }
                // adjusting game object position (called while player input movement key is pressed)
                playerPosition += new Vector2(0.0f, movementSpeed * Time.deltaTime);
                this.transform.position = playerPosition;
                break;
            // Move Player: LEFT
            case Direction.Left:
                // animation (called once)
                if (!animator.GetBool("isLeft"))
                {
                    animator.SetBool("isUp", false);
                    animator.SetBool("isLeft", true);
                    animator.SetBool("isDown", false);
                    animator.SetBool("isRight", false);
                }
                // adjusting game object position (called while player input movement key is pressed)
                playerPosition -= new Vector2(movementSpeed * Time.deltaTime, 0.0f);
                this.transform.position = playerPosition;
                break;
            // Move Player: DOWN
            case Direction.Down:
                // animation (called once)
                if (!animator.GetBool("isDown"))
                {
                    animator.SetBool("isUp", false);
                    animator.SetBool("isLeft", false);
                    animator.SetBool("isDown", true);
                    animator.SetBool("isRight", false);
                }
                // adjusting game object position (called while player input movement key is pressed)
                playerPosition -= new Vector2(0.0f, movementSpeed * Time.deltaTime);
                this.transform.position = playerPosition;
                break;
            // Move Player: RIGHT
            case Direction.Right:
                // animation (called once)
                if (!animator.GetBool("isRight"))
                {
                    animator.SetBool("isUp", false);
                    animator.SetBool("isLeft", false);
                    animator.SetBool("isDown", false);
                    animator.SetBool("isRight", true);
                }
                // adjusting game object position (called while player input movement key is pressed)
                playerPosition += new Vector2(movementSpeed * Time.deltaTime, 0.0f);
                this.transform.position = playerPosition;
                break;
            case Direction.None: // sets animation to closest idle state
                animator.SetBool("isUp", false);
                animator.SetBool("isLeft", false);
                animator.SetBool("isDown", false);
                animator.SetBool("isRight", false);
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        dialogueManager = collision.GetComponent<DialogueManager>();
    }
}
