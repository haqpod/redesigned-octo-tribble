using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1Script : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public Healthbar healthbar;

    public float rightScreenEdge;
    public float leftScreenEdge;
    public float bottomScreenEdge;
    public float topScreenEdge;
    
    public GameObject playerone;
    public GameManager gm;
    public Rigidbody2D rb;
    public Animator animator;

    private GameObject playerObj = null;

   
    SpriteRenderer rend;


    bool facingRight = true;
    bool teleportActive = false;
    bool invulnerable = false;

    private const float moveSpeed = 3f;
    
    private Vector3 moveDir;
    private Vector3 lastMoveDir;
    private bool isDashButtonDown;
    public Vector3 attackDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastMoveDir = new Vector3(1, 0).normalized;
    }

    void Start()
    {
         
        //Cursor.visible = false;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);

        if (playerObj == null)
            playerObj = GameObject.FindGameObjectWithTag("player1");

        rend = GetComponent<SpriteRenderer>();


    }

    void Update()
    {
        if (gm.gameOver)
        { 
            return;
             
        }

        float moveX = 0f;
        float moveY = 0f;

        if (teleportActive == false)
        {

            if (Input.GetKey(KeyCode.W))
            {
                moveY = +1f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                moveY = -1f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                moveX = -1f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                moveX = +1f;
            }

        }

        moveDir = new Vector3(moveX, moveY).normalized;
        if (moveX != 0 || moveY != 0)
        {
            //not idle
            lastMoveDir = moveDir;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(PerformTeleport());

        }
 
       

        if (Input.GetMouseButton(0)) 
        {
            Attack();
        }

         void Attack()
    {
           float x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - playerObj.transform.position.x;
           float y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - playerObj.transform.position.y;

           attackDir = new Vector2(x,y);
           animator.SetFloat("MouseClickX", x);
            animator.SetFloat("MouseClickY", y);

            /*Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Debug.Log("player position x =" + playerObj.transform.position.x);
            Debug.Log("player position y =" + playerObj.transform.position.y); */
            //Debug.Log(attackDir);
            

    }


         float move = Input.GetAxisRaw("Horizontal");

       // rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized
     //* moveSpeed * Time.deltaTime;

        /* if (move < 0 && facingRight)
        {
            Flip();
        }

        else if (move > 0 && !facingRight)
        {
            Flip();
        }*/


     animator.SetFloat("moveX", rb.velocity.x);
     animator.SetFloat("moveY", rb.velocity.y);

     //set the player to keep looking in last moved direction when idling
     if (
         //Input.GetAxisRaw("Horizontal") == 1  ||
         //Input.GetAxisRaw("Horizontal") == -1 ||

         Input.GetAxisRaw("Vertical") != 0 ||
         Input.GetAxisRaw("Horizontal") !=0


         //Input.GetAxisRaw("Vertical") == 1    ||
         //Input.GetAxisRaw("Vertical") == -1
         )
        {
          animator.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
          
            animator.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));
        } 

         

     
    }

    private void FixedUpdate()
    {

        if (gm.gameOver)
        {
            return;

        }

        rb.velocity = moveDir * moveSpeed;



       /* if (isDashButtonDown)
        {
            StartCoroutine(waiting());

            IEnumerator waiting()
            {
                animator.SetBool("isTeleporting", true);

                yield return new WaitForSeconds(2);
                // GameObject player1Copy = Instantiate(gameObject, transform.position, transform.rotation);

                // yield return new WaitForSeconds(2);
                // Destroy(player1Copy);

                float dashAmount = 3f;
                rb.MovePosition(transform.position + lastMoveDir * dashAmount);
                isDashButtonDown = false;
                Debug.Log("hello world");

                yield return new WaitForSeconds(2);




                //Invoke("Dash", 2.0f);



            }

        } */

    }

    IEnumerator PerformTeleport()
    {

            teleportActive = !teleportActive;
            invulnerable = !invulnerable;
            animator.SetBool("isTeleportingFadeOut", true);
            yield return new WaitForSeconds(0.5f);
            Dash();
            yield return new WaitForSeconds(0.2f);

            animator.SetBool("isTeleportingFadeIn", true);
            yield return new WaitForSeconds(0.5f);

            teleportActive = !teleportActive;
            invulnerable = !invulnerable;


        yield break;
       
    }


    /*void Dash()
    {


        if (isDashButtonDown)
        {
            StartCoroutine(waiting());

            IEnumerator waiting()
            {
                animator.SetBool("isTeleporting", true);

                yield return new WaitForSeconds(2);
                // GameObject player1Copy = Instantiate(gameObject, transform.position, transform.rotation);

                // yield return new WaitForSeconds(2);
                // Destroy(player1Copy);

                float dashAmount = 3f;
                rb.MovePosition(transform.position + lastMoveDir * dashAmount);
                isDashButtonDown = false;
                Debug.Log("hello world");

                yield return new WaitForSeconds(2);




                //Invoke("Dash", 2.0f);



            }

        }

    }
    */

       
void Dash()
    {
        float dashAmount = 5f;
        rb.MovePosition(transform.position + lastMoveDir * dashAmount);
        isDashButtonDown = false;

    } 


    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

      void OnTriggerEnter2D(Collider2D other)
      {
        if (gm.gameOver)
        {
            return;
        }

        if (invulnerable == false)
        {



            if (other.CompareTag("boss"))
            {
                TakeDamage(10);
                gm.UpdateLives(-100);
                animator.SetBool("isDead", true);
                rb.velocity = new Vector2(0, 0);
            }

            else if (other.CompareTag("bossProjectile1"))
            {

                if (currentHealth >= 2)
                {
                    animator.SetBool("isHurt", true);
                }
                TakeDamage(1);
            }
        }
      }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            animator.SetBool("isDead", true);
            gm.UpdateLives(-1);
            rb.velocity = new Vector2(0, 0); 
        }
    }

     
    

}


