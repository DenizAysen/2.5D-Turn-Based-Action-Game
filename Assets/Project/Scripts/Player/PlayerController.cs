using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private int speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;
    [SerializeField] private int minStepsToEncounter;
    [SerializeField] private int maxStepsToEncounter;
    #endregion

    #region Privates
    private PlayerControls _playerControls;
    private PartyManager _partyManager;
    private Rigidbody _rigidbody;
    private Vector3 _movement;
    private float _x, _z;
    private bool _movingInGrass;
    private float _stepTimer;
    private int _stepsToEnCounter;

    #region Consts
    private const string IS_WALK = "IsWalk";
    private const string BATTLE_SCENE = "BattleScene";
    private const float TIME_PER_STEP = .5f;  
    #endregion
    #endregion
    private void Awake()
    {
        _playerControls = new PlayerControls();
        CalculateStepsToNextEncounter();
    }
    private void OnEnable()
    {
        _playerControls.Enable();
    }
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _partyManager = FindObjectOfType<PartyManager>();
        if(_partyManager.GetPosition() != Vector3.zero)
        {
            transform.position = _partyManager.GetPosition();
        }
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
            if(_stepTimer > TIME_PER_STEP)
            {
                _stepTimer = 0;
                stepsInGrass++;

                if(stepsInGrass >= _stepsToEnCounter)
                {
                    _partyManager.SetPosition(transform.position);
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
            }
        }
    }
    private void CalculateStepsToNextEncounter()
    {
        _stepsToEnCounter = Random.Range(minStepsToEncounter, maxStepsToEncounter);
    }
}
