using UnityEngine;

public class MerchantMovement : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float Speed;
    public float JumpForce;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    private bool Grounded;
    
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator= GetComponent<Animator>();
    }


    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        // As� es como tiene que ser pero por motivos del tama�o de la imagen
        // se usa el siguiente ajuste temporal
        if(Horizontal > 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal < 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //if (Horizontal < 0.0f) transform.localScale = new Vector3(-0.0831f, 0.1132f, 1.0f);
        //else if (Horizontal > 0.0f) transform.localScale = new Vector3(0.0831f, 0.1132f, 1.0f);
        Debug.DrawRay(transform.position, Vector3.down * 0.3f, Color.red);
        Animator.SetBool("running", Horizontal != 0.0f);
        //Animator.SetBool("jumping", Horizontal != 0.0f);
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.3f))
        {
            Grounded = true;
            //Animator.SetBool("isGrounded",true);
            //Animator.SetBool("jumping", false);
        }
        else
        {
            Grounded = false;
            //Animator.SetBool("isGrounded",false);
            //Animator.SetBool("jumping", true);
        }
                
            
        if (Input.GetKeyDown(KeyCode.P) && Grounded)
        {
            
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Shoot();
        }
        
        
    }
    
    private void Jump()
    {
        
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }

    private void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 0.0831f) direction = Vector3.right;
        else direction = Vector3.left;

            GameObject bullet = Instantiate(BulletPrefab, transform.position + direction * 0.1f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);
    }

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal, Rigidbody2D.linearVelocity.y);
    }
}
