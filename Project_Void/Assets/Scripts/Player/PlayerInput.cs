using System;
using System.Diagnostics;

// ENUMS
public enum InputType { Movement, Interact };
public enum Direction { Up, Left, Down, Right, None };

// INTERFACES
interface IPlayerInput 
{
    InputType Type { get; set; }
    Direction Direction { get; set; }
    bool MovementKeyIsPressed { get; set; }
}
interface IPlayerMovement
{
    Direction Direction { get; set; }
}

// CLASSES
public class PlayerInput : IPlayerInput
{
    public PlayerInput (InputType type)
    {
        Type = type;
    }

    public PlayerInput(InputType type, Direction direction)
    {
        Type = type;
        Direction = direction;
    }

    public InputType Type { get; set; }
    public Direction Direction { get; set; }
    public bool MovementKeyIsPressed { get; set; }
    
}

public class PlayerMovement : IPlayerMovement
{
    public PlayerMovement (Direction direction)
    {
        Direction = direction;
    }

    public Direction Direction { get; set; }
}

