using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Move Values")]
    public bool canMove;
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
        FlipSprite();
        #endregion
    }

    private void FlipSprite()
    {
        if (moveX != 0 && moveX != wherePlayerSeeing)
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

        if (rb.velocity.y < 0 && !isFalling)
        {
            AnimationTrigger("Fall");
            isFalling = true;
            
        }

        if (rb.velocity.y == 0)
        {
            isJumping = false;
            isFalling = false;
        }
    }

    private void CheckIsTouchingGround()
    {
        isTouchingGround = Physics2D.OverlapCircle(checkGroundLocal.position, checkGroundRadius, checkGroundLayer);
    }

    private void Attack()
    {
        if (canAttack && inputAttackWasPressed && stateAttack < 2)
        {
            stateAttack++;
            AnimationTrigger("Attack");
        }
    }

    public void ResetAttack()
    {
        stateAttack = 0;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(checkGroundLocal.position, checkGroundRadius);
    }
}
