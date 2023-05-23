using UnityEngine;

public static class CommandFactory
{
    public static ICommand Create(KeyCode code)
    {
        switch (code)
        {
            case KeyCode.W: return new MoveForwardCommand();
            case KeyCode.S: return new MoveBackwardCommand();
            case KeyCode.A: return new MoveLeftCommand();
            case KeyCode.D: return new MoveRightCommand();

            default: return new EmptyCommand();
        }
    }
}