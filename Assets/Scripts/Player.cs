using System.Data;
using UnityEngine;



public class Player : MonoBehaviour
{
    public float speed = 7f;
    public float jumpSpeed = 15;
    public Collider2D bottomCollider;
    public CompositeCollider2D terrainCollider;

    

    float vx = 0;

    // prevVx / 이전 속도를 저장하려고 만든 변수
    // 이전 속도에서 빨라지는 상태를 run이라하고 run이되면 다른걸 적용시키기위해
    float prevVx = 0;
    
    bool isGrounded;//플레이어가 땅과 붙어있는지 확인하려고 만든 변수
    Vector2 originPosition;
    float lastShoot;

    // Start is called before the first frame update
    void Start()
    {
       originPosition = transform.position;
      
    }

    public void Restart()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        GetComponent<BoxCollider2D>().enabled = true;

        transform.eulerAngles= Vector3.zero;
        transform.position = originPosition;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {// GetAxisRaw Raw가 더 날것의 값 이게임에선 가속도가 없기때문에 raw
        // GetAxis는 오른쪽에서 왼쪽을누르면 부드럽게
        //Horizontal / 
        vx = Input.GetAxisRaw("Horizontal");

        if(vx<0)//vx가 음수라면
        {
            GetComponent<SpriteRenderer>().flipX = true; // flipX란 변수를 트루로하면 좌우반전
            
        }
        if(vx>0)//vx가 양수라면
        {
            GetComponent<SpriteRenderer>().flipX=false;
        }
        
        
        if (bottomCollider.IsTouching(terrainCollider))// 만약 각각의 콜라이더 두개가 붙어있다면
        {
            if (!isGrounded)//아니라면
            {   
                if (vx == 0)//이동속도가0이면 idle
                {
                    GetComponent<Animator>().SetTrigger("Idle");
                }
                else//이동속도가0이아니면 run
                {
                    GetComponent<Animator>().SetTrigger("Run");
                }
            }
            else//계속붙어있다면
            {   
                if (prevVx != vx)//이전속도와 지금 속도가 다르다면
                {
                    if (vx == 0)//이속이0이면 idle
                    {
                        GetComponent<Animator>().SetTrigger("Idle");
                    }
                    else//이속0이아니면 run
                    {
                        GetComponent<Animator>().SetTrigger("Run");
                    }
                }
            }

            isGrounded = true;
        }
        else//땅에 붙어있지 않다면
        {   
            if (isGrounded)//땅에 붙어있다면
            {   //점프 시작
                GetComponent<Animator>().SetTrigger("Jump");
            }
            isGrounded = false;
        }
        //만약 인풋 겟버튼의 점프를 누르면 isGrounded상태일때 점프를 누르면
        if (Input.GetButton("Jump") && isGrounded)
        {// 컴퍼넌트 리지드바디2d().속도 = y+1 * 점프스피드
            GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpSpeed;
        }
        prevVx = vx;

        //총알발사
        if(Input.GetButtonDown("Fire1"))//&& lastShoot + 0.5f<Time.time)
        {
            Vector2 bulletV = Vector2.zero;
            if (GetComponent<SpriteRenderer>().flipX)
            {
                bulletV = new Vector2(-10, 0);
            }
            else
            {
                bulletV = new Vector2(10, 0);
            }
            GameObject bullet = ObjectPool.Instance.GetBullet();
            bullet.transform.position = transform.position;
            bullet.GetComponent<Bullet>().velocity = bulletV;
            lastShoot = Time.time;
        }
    }
    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * vx * speed * Time.fixedDeltaTime);

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
            {
            Die();
        }
    }

    void Die()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        GetComponent<Rigidbody2D>().angularVelocity = 720;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0,10),ForceMode2D.Impulse);
        GetComponent<BoxCollider2D>().enabled = false;

      GameManager.Instance.Die();
    }

}
