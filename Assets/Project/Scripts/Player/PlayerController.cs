using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;

    private PlayerControls _playerControls;
    private Rigidbody _rigidbody;
    private Vector3 _movement;
    private float _x,_z;
    private void Awake()
    {
        _playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        _playerControls.Enable();
    }
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        _x = _playerControls.Player.Move.ReadValue<Vector2>().x;
        _z = _playerControls.Player.Move.ReadValue<Vector2>().y;

        _movement = new Vector3(_x,0,_z).normalized;
    }
    private void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + _movement * speed * Time.fixedDeltaTime);
    }
}
