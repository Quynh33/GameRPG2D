using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum PlayerState
{
    walk,
    attack,
    interact,
    stagger,
    idle
}

public enum PlayerFace
{
    Up,
    Down,
    Left,
    Right
}

public class PlayerMovement : MonoBehaviour
{
    public PlayerState currentState;
    public float speed;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;
    public FloatValue currentHealth;
    public SignalSender playerHealthSignal;
    public VectorValue startingPosition;
    public Inventory playerInventory;
    public SpriteRenderer receivedItemSprite;
    public SignalSender reduceMagic;

    public PlayerFace currentFace;
    public GameObject HitBoxDown;
    public GameObject HitBoxUp;
    public GameObject HitBoxLeft;
    public GameObject HitBoxRight;
    [Header("Projectile Stuff")]
    public GameObject projectile;
    public Item bow;

    void Start()
    {
        currentState = PlayerState.walk;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
        currentFace = PlayerFace.Down;
        transform.position = startingPosition.initialValue;
    }

    void Update()
    {
        if (currentState == PlayerState.interact)
        {
            return;
        }
        change = Vector3.zero;
        change.x = Input.GetAxis("Horizontal");
        change.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("attack") && currentState != PlayerState.attack && currentState != PlayerState.stagger)
        {
            ActivateHitbox();
            StartCoroutine(AttackCo());
        }
        else if (Input.GetButtonDown("Second Weapon") && currentState != PlayerState.attack
          && currentState != PlayerState.stagger)
        {
            if (playerInventory.CheckForItem(bow))
            {
                StartCoroutine(SecondAttackCo());
            }
        }
        else if (currentState == PlayerState.walk || currentState == PlayerState.idle)
        {
            UpdateAnimationAndMove();
        }
    }

    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;

        yield return null;

        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(0.33f);
        currentState = PlayerState.walk;

        // Sau khi animation hoàn thành, tắt hitbox
        yield return new WaitForSeconds(0.33f); // Thời gian animation của bạn
        TurnOffHitboxes();
        if (currentState != PlayerState.interact)
        {
            currentState = PlayerState.walk;
        }
    }
    private IEnumerator SecondAttackCo()
    {
        //animator.SetBool("attacking", true);
        currentState = PlayerState.attack;
        yield return null;
        MakeArrow();
        //animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.3f);
        if (currentState != PlayerState.interact)
        {
            currentState = PlayerState.walk;
        }
    }
    private void MakeArrow()
    {
        if (playerInventory.currentMagic > 0)
        {
            Vector2 temp = new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
            Arrow arrow = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Arrow>();
            arrow.Setup(temp, ChooseArrowDirection());
            playerInventory.ReduceMagic(arrow.magicCost);
            reduceMagic.Raise();
        }
    }

    Vector3 ChooseArrowDirection()
    {
        float temp = Mathf.Atan2(animator.GetFloat("moveY"), animator.GetFloat("moveX")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }

    public void RaiseItem()
    {
        if (playerInventory.currentItem != null)
        {
            if (currentState != PlayerState.interact)
            {
                animator.SetBool("receive item", true);
                currentState = PlayerState.interact;
                receivedItemSprite.sprite = playerInventory.currentItem.itemSprite;
            }
            else
            {
                animator.SetBool("receive item", false);
                currentState = PlayerState.idle;
                receivedItemSprite.sprite = null;
                playerInventory.currentItem = null;
            }
        }
    }
    private void UpdateFace()
    {
        if (change.x > 0)
        {
            currentFace = PlayerFace.Right;
        }
        else if (change.x < 0)
        {
            currentFace = PlayerFace.Left;
        }
        if (change.y > 0)
        {
            currentFace = PlayerFace.Up;
        }
        else if (change.y < 0)
        {
            currentFace = PlayerFace.Down;
        }
    }

    void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
            UpdateFace();
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    void MoveCharacter()
    {
        change.Normalize();
        myRigidbody.MovePosition(transform.position + change * speed * Time.deltaTime);
    }

    private void ActivateHitbox()
    {
        TurnOffHitboxes();
        switch (currentFace)
        {
            case PlayerFace.Down:
                HitBoxDown.SetActive(true);
                break;
            case PlayerFace.Up:
                HitBoxUp.SetActive (true);
                break;
            case PlayerFace.Left:
                HitBoxLeft.SetActive(true);
                break;
            case PlayerFace.Right:
                HitBoxRight.SetActive(true);
                break;
        }
    }

    private void TurnOffHitboxes()
    {
        HitBoxDown.SetActive(false);
        HitBoxUp.SetActive(false);
        HitBoxLeft.SetActive(false);
        HitBoxRight.SetActive(false);
    }

    public void Knock(float knockTime, float damage)
    {
        currentHealth.RuntimeValue -= damage;
        playerHealthSignal.Raise();
        if (currentHealth.RuntimeValue > 0)
        {
            StartCoroutine(KnockCo(knockTime));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator KnockCo(float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = PlayerState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }
}