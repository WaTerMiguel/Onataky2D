using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Move Values")]
    public bool canMove;
    private bool canFlip = true;
    private float moveX;
    public float speed;
    private bool isTouchingGround;
    private int wherePlayerSeeing = 1;

    [Header("Jump Values")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpAjust = 0.5f;
    private bool canJump = false;
    private bool inputJumpWasPressed;
    private bool inputJumpWasRelease;
    private bool isFalling = false;
    private bool isJumping = false;

    [Header("Checking Ground")]
    [SerializeField] float checkGroundRadius;
    [SerializeField] Transform checkGroundLocal;
    [SerializeField] LayerMask checkGroundLayer;

    [Header("Attacks")]
    public bool canAttack = true;
    public int stateAttack = 0;
    private bool inputAttackWasPressed;
    private bool isAttaking = false;
    [SerializeField] float impulseAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckInputs();
        UpdateStates();
        UpdateAnimation();
        Jump();
        Attack();
    }

    private void FixedUpdate() 
    {
        Move();

        #region All Checks
        CheckIsTouchingGround();
        #endregion
    }

    private void CheckInputs()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        inputJumpWasPressed = Input.GetKeyDown(KeyCode.Space);
        inputJumpWasRelease = Input.GetKeyUp(KeyCode.Space);
        inputAttackWasPressed = Input.GetMouseButtonDown(0);
    }

    private void Move()
    {
        #region Moving in Ground
        if (canMove && isTouchingGround)
        {
            rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
        }

        if (canMove) canFlip = true;
        FlipSprite();
        #endregion
    }

    private void FlipSprite()
    {
        if (canFlip && moveX != 0 && moveX != wherePlayerSeeing)
        {
            wherePlayerSeeing = (int)moveX;
            transform.Rotate(0f, 180f, 0f);
        }
        
    }

    private void Jump()
    {
        if (isTouchingGround) canJump = true;
        if (inputJumpWasPressed && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x * moveX, jumpForce);
            AnimationTrigger("Jump");
            isJumping = true;
            canJump = false;
        }
        
        if(inputJumpWasRelease && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpAjust);
        }
    }

    private void UpdateAnimation()
    {
        anim.SetBool("IsFalling", isFalling);
        anim.SetBool("IsJumping", isJumping);
        anim.SetBool("IsWalking", rb.velocity.x != 0);
        anim.SetInteger("qualAtaque", stateAttack);
    }

    public void AnimationTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    private void UpdateStates()
    {

        if (rb.velocity.y < 0 && !isTouchingGround && !isFalling)
        {
            AnimationTrigger("Fall");
            isFalling = true;
            
        }

        if (rb.velocity.y == 0)
        {
            isJumping = false;
            isFalling = false;
        }

        if (isAttaking)
        {
            canMove = false; 
            canFlip = false;
        } 
    }

    private void CheckIsTouchingGround()
    {
        isTouchingGround = Physics2D.OverlapCircle(checkGroundLocal.position, checkGroundRadius, checkGroundLayer);
    }

    private void Attack()
    {
        if (canAttack && !isAttaking && inputAttackWasPressed && stateAttack < 2)
        {
            stateAttack++;
            AnimationTrigger("Attack");
            isAttaking = true;
            canMove = false;
        }
    }

    public void ResetAttack()
    {
        stateAttack = 0;
        canMove = true;
        isAttaking = false;
        canFlip = true;
    }

    public void CanNextAttack()
    {
        isAttaking = false;
    }

    public void ImpulseAttack()
    {
        rb.AddForce(Vector2.right * wherePlayerSeeing * impulseAttack, ForceMode2D.Impulse);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(checkGroundLocal.position, checkGroundRadius);
    }
}
