using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour,IGameActor
{
    protected Dictionary<KeyCode, ICommand> _commands;

    private Transform _transform;
    
    [SerializeField] private float _moveSpeed;


    private void Awake()
    {
        _commands = new Dictionary<KeyCode, ICommand>();
        _transform = transform;
    }


    protected virtual void Start()
    {
        _commands.Add(KeyCode.W, CommandFactory.Create(KeyCode.W));
        _commands.Add(KeyCode.S, CommandFactory.Create(KeyCode.S));
        _commands.Add(KeyCode.A, CommandFactory.Create(KeyCode.A));
        _commands.Add(KeyCode.D, CommandFactory.Create(KeyCode.D));
        _commands.Add(KeyCode.None, CommandFactory.Create(KeyCode.None));
    }

    private void Update()
    {
        ICommand command = HandleInput();
        
        command.Execute(this);
    }

    protected abstract ICommand HandleInput();


    public void MoveForward()
    {
        _transform.position += _transform.forward * (_moveSpeed * Time.deltaTime);
    }

    public void MoveBackward()
    {
        _transform.position -= _transform.forward * (_moveSpeed * Time.deltaTime);
    }

    public void MoveLeft()
    {
        _transform.position -= _transform.right * (_moveSpeed * Time.deltaTime);
    }

    public void MoveRight()
    {
        _transform.position += _transform.right * (_moveSpeed * Time.deltaTime);
    }
}