using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerControls _playerControls;
    private float _x,_z;
    private void Awake()
    {
        _playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        _playerControls.Enable();
    }
    // Update is called once per frame
    void Update()
    {
        _x = _playerControls.Player.Move.ReadValue<Vector2>().x;
        _z = _playerControls.Player.Move.ReadValue<Vector2>().y;
    }
}
