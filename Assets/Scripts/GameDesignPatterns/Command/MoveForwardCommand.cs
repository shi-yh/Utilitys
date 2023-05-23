using UnityEngine;

public class MoveForwardCommand : ICommand
{
    public void Execute(IGameActor actor)
    {
        actor.MoveForward();
    }
}

public class MoveBackwardCommand : ICommand
{
    public void Execute(IGameActor actor)
    {
        actor.MoveBackward();
    }
}

public class MoveLeftCommand : ICommand
{
    public void Execute(IGameActor actor)
    {
        actor.MoveLeft();
    }
}

public class MoveRightCommand : ICommand
{
    public void Execute(IGameActor actor)
    {
        actor.MoveRight();
    }
}


public class EmptyCommand : ICommand
{
    public void Execute(IGameActor actor)
    {
    }
}