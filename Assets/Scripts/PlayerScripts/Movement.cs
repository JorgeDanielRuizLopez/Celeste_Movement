using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class Movement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody rb;
    private AnimationScript anim;
    public TMP_Text count;
    public GameObject fallDetector;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;
    public float forcceSwim;
    public float air = 5;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;
    public bool inWater = false;

    [Space]

    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;
    public Vector3 spawnPoint;


    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;


    void Start()
    {
        spawnPoint = transform.position;
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<AnimationScript>();
    }


    void Update()
    {
        fallDetector.transform.position = new Vector3(transform.position.x, fallDetector.transform.position.y, 0);

        print(FlipPlayer.rotationValue);

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector3 dir = Vector3.zero;


        //Flip Movement
        if (FlipPlayer.rotationValue == 0)
        {
            dir = new Vector3(x, y, 0);
        }
        else if (FlipPlayer.rotationValue == 1)
        {
            dir = new Vector3(x, y, 0);
        }
        else if (FlipPlayer.rotationValue == 2)
        {
            dir = new Vector3(x, y, 0);
        }
        else if (FlipPlayer.rotationValue == 3)
        {
            dir = new Vector3(x, y, 0);
        }


        Walk(dir);
        anim.SetHorizontalMovement(x, y, rb.velocity.y);


        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            if(side != coll.wallSide)
                anim.Flip(side*-1);
            wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }


        //Collicion in the ground and false dashin
        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }
        

        //Wall Slide
        if (wallGrab && !isDashing)
        {
            rb.useGravity = false;
            if(x > .2f || x < -.2f)
            rb.velocity = new Vector3(rb.velocity.x, 0,0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector3(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.useGravity = true;
        }

        if(coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;


        //JUMP Key
        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");

            if (coll.onGround)
                Jump(Vector3.up, false);
            if (coll.onWall && !coll.onGround)
                WallJump();

            if (inWater == true)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, 0);
                rb.AddForce(new Vector3(0, forcceSwim, 0), ForceMode.Force);
            }
            
        }


        //DASH Key
        if (Input.GetButtonDown("Fire1") && !hasDashed && !inWater)
        {
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw, 0);
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        WallParticle(y);

        if (wallGrab || wallSlide || !canMove)
            return;


        //Flip Count Zone
        if(x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }


        //Checks in Water
        if (inWater == true)
        {
            air -= Time.deltaTime;

            count.text = "" + air.ToString("f0");  
        }

        if (air < 0)
        {
            transform.position = spawnPoint;
            air = 5;
        }
    }

    //Check Ground Touch
    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = anim.sr.flipX ? -1 : 1;

        jumpParticle.Play();
    }


    //All About the DASH
    private void Dash(float x, float y, float z)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        hasDashed = true;

        anim.SetTrigger("dash");

        rb.velocity = Vector3.zero;
        Vector3 dir = Vector3.zero;

        if (FlipPlayer.rotationValue == 0)
        {
            dir = new Vector3(x, y, z);
        }
        else if (FlipPlayer.rotationValue == 1)
        {
            dir = new Vector3(z, y, x);
        }
        else if (FlipPlayer.rotationValue == 2)
        {
            dir = new Vector3(-x, y, z);
        }
        else if (FlipPlayer.rotationValue == 3)
        {
            dir = new Vector3(z, y, -x);
        }

        rb.velocity = dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        dashParticle.Play();
        rb.useGravity = false;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        dashParticle.Stop();
        rb.useGravity = true;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }



    //Wall Jump
    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector3 wallDir = coll.onRightWall ? Vector3.left : Vector3.right;

        Jump((Vector3.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
    }


    //Wall Slide
    private void WallSlide()
    {
        if(coll.wallSide != side)
         anim.Flip(side * -1);

        if (!canMove)
            return;

        bool pushingWall = false;
        if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector3(push, -slideSpeed, 0);
    }


    //WALK
    private void Walk(Vector3 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;

        if (!wallJumped)
        {
            if (FlipPlayer.rotationValue == 0)
            {
                rb.velocity = new Vector3(dir.x * speed, rb.velocity.y, 0);
            }
            else if (FlipPlayer.rotationValue == 1)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, dir.x * speed);
            }
            else if (FlipPlayer.rotationValue == 2)
            {
                rb.velocity = new Vector3(-dir.x * speed, rb.velocity.y, 0);
            }
            else if (FlipPlayer.rotationValue == 3)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, -dir.x * speed);  
            }
        }

        else
        {
            if (FlipPlayer.rotationValue == 0)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, (new Vector3(dir.x * speed, rb.velocity.y, 0)), wallJumpLerp * Time.deltaTime);
            }
            else if (FlipPlayer.rotationValue == 1)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, (new Vector3(0, rb.velocity.y, dir.x * speed)), wallJumpLerp * Time.deltaTime);
            }
            else if (FlipPlayer.rotationValue == 2)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, (new Vector3(-dir.x * speed, rb.velocity.y, 0)), wallJumpLerp * Time.deltaTime);
            }
            else if (FlipPlayer.rotationValue == 3)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, (new Vector3(0, rb.velocity.y, -dir.x * speed)), wallJumpLerp * Time.deltaTime);
            }
        }
    }


    //JUMP
    private void Jump(Vector3 dir, bool wall)
    {
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        rb.velocity = dir * jumpForce;

        particle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }

    public void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            inWater = true;
            rb.drag = 10;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        inWater = false;
        rb.drag = 0;
        air = 5;
    }
    public void OnTriggerEnter(Collider other)
    {

        if (other.tag == "FallDetector")
        {
            transform.position = spawnPoint;
        }
    }
}
