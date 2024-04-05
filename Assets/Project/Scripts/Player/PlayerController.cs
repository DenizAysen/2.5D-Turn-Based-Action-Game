using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;

    private PlayerControls _playerControls;
    private Rigidbody _rigidbody;
    private Vector3 _movement;
    private float _x,_z;
    private bool _movingInGrass;
    private float _stepTimer;

    private const string IS_WALK = "IsWalk";
    private const float _timePerStep = .5f;
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

        anim.SetBool(IS_WALK, _movement != Vector3.zero);

        if(_x != 0 && _x < 0)
        {
            playerSprite.flipX = true;
        }
        if(_x != 0 && _x > 0)
        {
            playerSprite.flipX = false;
        }
    }
    private void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + _movement * speed * Time.fixedDeltaTime);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f, grassLayer);
        _movingInGrass = colliders.Length != 0 && _movement != Vector3.zero;

        if(_movingInGrass) 
        {
            _stepTimer += Time.fixedDeltaTime;
            if(_stepTimer > _timePerStep)
            {
                _stepTimer = 0;
                stepsInGrass++;
            }
        }
    }
}
