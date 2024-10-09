using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D _rb;
    Animator _anim;

    public bool _canMove = true;
    bool _canFlip = true;
    float _moveX;
    public float speed = 1.2f ;
    bool _isTouchingGround;
    int _wherePlayerSeeing = 1;

    [Header("Jump Values")]
    [SerializeField] float jumpForce = 3f;
    [SerializeField] float jumpAjust = 0.5f;
    bool _canJump = false;
    bool _inputJumpWasPressed;
    bool _inputJumpWasRelease;
    bool _isFalling = false;
    bool _isJumping = false;

    [Header("Checking Ground")]
    [SerializeField] float checkGroundRadius = 0.05f;
    [SerializeField] Transform checkGroundLocal;
    [SerializeField] LayerMask checkGroundLayer;

    [Header("Attacks")]
    public bool canAttack = true;
    public int stateAttack = 0;
    bool _inputAttackWasPressed;
    bool _isAttaking = false;
    [SerializeField] float impulseAttack = 1.5f;

    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // getters and setters

    public PlayerBaseState CurrentState { get { return _currentState; } set{ _currentState = value; }}
    public Rigidbody2D RB { get { return _rb; }}
    public Animator Anim { get { return _anim; }}

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    protected virtual void Update()
    {
        CheckInputs();
        _currentState.UpdateState();
        UpdateStates();
        Update_animation();
        Jump();
        Attack();
    }

    protected virtual void FixedUpdate() 
    {
        Move();
        #region All Checks
        Check_isTouchingGround();
        #endregion
    }

    void CheckInputs()
    {
        _moveX = Input.GetAxisRaw("Horizontal");
        _inputJumpWasPressed = Input.GetKeyDown(KeyCode.Space);
        _inputJumpWasRelease = Input.GetKeyUp(KeyCode.Space);
        _inputAttackWasPressed = Input.GetMouseButtonDown(0);
    }

    void Move()
    {
        #region Moving in Ground
        if (_canMove && _isTouchingGround)
        {
            _rb.velocity = new Vector2(_moveX * speed, _rb.velocity.y);
        }

        if (_canMove) _canFlip = true;
        FlipSprite();
        #endregion
    }

    void FlipSprite()
    {
        if (_canFlip && _moveX != 0 && _moveX != _wherePlayerSeeing)
        {
            _wherePlayerSeeing = (int)_moveX;
            transform.Rotate(0f, 180f, 0f);
        }
        
    }

    void Jump()
    {
        if (_isTouchingGround) _canJump = true;
        if (_inputJumpWasPressed && _canJump)
        {
            _rb.velocity = new Vector2(_rb.velocity.x * _moveX, jumpForce);
            _animationTrigger("Jump");
            _isJumping = true;
            _canJump = false;
        }
        
        if(_inputJumpWasRelease && _rb.velocity.y > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * jumpAjust);
        }
    }

    void Update_animation()
    {
        _anim.SetBool("_IsFalling", _isFalling);
        _anim.SetBool("_IsJumping", _isJumping);
        _anim.SetBool("IsWalking", _rb.velocity.x != 0);
        _anim.SetInteger("qualAtaque", stateAttack);
    }

    /// <summary>
    /// Metodo usado para ativar um trigger no _animator
    /// </summary>
    /// <param name="qua"></param>
    public void _animationTrigger(string trigger)
    {
        _anim.SetTrigger(trigger);
    }

    void UpdateStates()
    {

        if (_rb.velocity.y < 0 && !_isTouchingGround && !_isFalling)
        {
            _animationTrigger("Fall");
            _isFalling = true;
            
        }

        if (_rb.velocity.y == 0)
        {
            _isJumping = false;
            _isFalling = false;
        }

        if (_isAttaking)
        {
            _canMove = false; 
            _canFlip = false;
        } 
    }

    void Check_isTouchingGround()
    {
        _isTouchingGround = Physics2D.OverlapCircle(checkGroundLocal.position, checkGroundRadius, checkGroundLayer);
    }

    void Attack()
    {
        if (canAttack && !_isAttaking && _inputAttackWasPressed && stateAttack < 2)
        {
            stateAttack++;
            _animationTrigger("Attack");
            _isAttaking = true;
            _canMove = false;
        }
    }

    public void ResetAttack()
    {
        stateAttack = 0;
        _canMove = true;
        _isAttaking = false;
        _canFlip = true;
    }

    public void CanNextAttack()
    {
        _isAttaking = false;
    }

    public void ImpulseAttack()
    {
        _rb.AddForce(Vector2.right * _wherePlayerSeeing * impulseAttack, ForceMode2D.Impulse);
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(checkGroundLocal.position, checkGroundRadius);
    }
}
