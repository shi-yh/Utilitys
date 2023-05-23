using System.Collections.Generic;
using UnityEngine;

public class AIController : BaseController
{
    private float _stayTime = 2f;

    private ICommand _lastCommand;

    private KeyCode[] _states;


    protected override void Start()
    {
        base.Start();

        _lastCommand = _commands[KeyCode.None];

        _states = new KeyCode[5];

        _states[0] = KeyCode.W;
        _states[1] = KeyCode.S;
        _states[2] = KeyCode.A;
        _states[3] = KeyCode.D;
        _states[4] = KeyCode.None;
    }


    protected override ICommand HandleInput()
    {
        if (_stayTime > 0)
        {
            _stayTime -= Time.deltaTime;

            return _lastCommand;
        }

        _stayTime = Random.Range(0, 2);

        KeyCode keycode = _states[Random.Range(0, 5)];

        _lastCommand = _commands[keycode];


        return _lastCommand;
    }
}