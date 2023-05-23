using UnityEngine;

public class PlayerController : BaseController
{
    protected override ICommand HandleInput()
    {
        ICommand result = _commands[KeyCode.None];

        if (Input.GetKey(KeyCode.W))
        {
            result = _commands[KeyCode.W];
        }
        else if (Input.GetKey(KeyCode.S))
        {
            result = _commands[KeyCode.S];
        }
        else if (Input.GetKey(KeyCode.A))
        {
            result = _commands[KeyCode.A];
        }
        else if (Input.GetKey(KeyCode.D))
        {
            result = _commands[KeyCode.D];
        }

        return result;
    }
}