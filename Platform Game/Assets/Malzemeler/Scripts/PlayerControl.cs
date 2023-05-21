using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float speed=1;
    private float playerSpeedX;
    private Rigidbody2D playerBody;
    private Vector3 defaultLocalScale;
    public bool onGround;
    [SerializeField] GameObject arrow;
    [SerializeField] bool attacked;
    [SerializeField] float currentAttackTimer;
    [SerializeField] float defaultAttackTimer;
    private Animator myAnimator;
    [SerializeField] int arrowNumber;
    [SerializeField] Text arrowNumberText;
    [SerializeField] AudioClip dieSound;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        defaultLocalScale = transform.localScale;
        attacked = false;
        myAnimator = GetComponent<Animator>();
        arrowNumberText.text = arrowNumber.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        playerSpeedX = Input.GetAxis("Horizontal");
        myAnimator.SetFloat("Speed", Mathf.Abs(playerSpeedX));
        playerBody.velocity = new Vector2(playerSpeedX * speed,playerBody.velocity.y);


        #region Player face direction set
        if (playerSpeedX > 0)
        {
            transform.localScale = new Vector3(defaultLocalScale.x,defaultLocalScale.y, defaultLocalScale.z);
        }

       else if (playerSpeedX < 0)
        {
            transform.localScale = new Vector3( -(defaultLocalScale.x), defaultLocalScale.y, defaultLocalScale.z);
        }
        #endregion


        #region Player jump set
        if (Input.GetKeyDown(KeyCode.Space))
        {
           if ( onGround==true )
            {
                myAnimator.SetTrigger("Jump");
                playerBody.velocity = new Vector2(playerBody.velocity.x, 7f);
            }

            
        }
        #endregion


        #region Player arrow control

        

        if (Input.GetMouseButtonDown(0) && arrowNumber>0)
        {
            if( attacked==false)
            {
                attacked = true;
                myAnimator.SetTrigger("Attack");
                Invoke("ArrowFire",0.8f);
            }
          
        }

        if (attacked == true)
        {
            currentAttackTimer -= Time.deltaTime;

        }

        else
        {
            currentAttackTimer = defaultAttackTimer;
        }

        if (currentAttackTimer <= 0 )
        {
            attacked = false;
        }



        #endregion




    }

    void ArrowFire()
    {
        GameObject playerArrow = Instantiate(arrow, transform.position, Quaternion.identity);
        playerArrow.transform.parent = GameObject.Find("Arrows").transform;


        if (transform.localScale.x > 0)
        {
            playerArrow.GetComponent<Rigidbody2D>().velocity = new Vector2(10f, 0f);
        }

        else
        {
            Vector3 arrowScale = playerArrow.transform.localScale;
            playerArrow.transform.localScale = new Vector3(-(arrowScale.x), arrowScale.y, arrowScale.z);
            playerArrow.GetComponent<Rigidbody2D>().velocity = new Vector2(-5f, 0f);
        }
        arrowNumber--;
        arrowNumberText.text = arrowNumber.ToString();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GetComponent<TimeControl>().enabled = false;
            Die();
        }

         else if (collision.gameObject.CompareTag("Finish"))
        {
            
             /*winPanel.active = true;
             Time.timeScale = 0; */
            Destroy(collision.gameObject);
            StartCoroutine(Wait(true));
            
        }
    }

    public void Die ()
    {
        GameObject.Find("Sound Controller").GetComponent<AudioSource>().clip = null;
        GameObject.Find("Sound Controller").GetComponent<AudioSource>().PlayOneShot(dieSound);
        myAnimator.SetFloat("Speed", 0);
        myAnimator.SetTrigger("Die");
       // playerBody.constraints = RigidbodyConstraints2D.FreezePosition;
        playerBody.constraints = RigidbodyConstraints2D.FreezeAll;
        enabled = false;
        //losePanel.SetActive(true);
        //Time.timeScale = 0;
        StartCoroutine(Wait(false));
    }

    IEnumerator Wait(bool win)
    {
       
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 0;

        if (win==true)
        {
            
            winPanel.SetActive(true);
        }

        else
        {
            losePanel.SetActive(true);
        }
    }


}
