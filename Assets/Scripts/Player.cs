using System.Data;
using UnityEngine;



public class Player : MonoBehaviour
{
    public float speed = 7f;
    public float jumpSpeed = 15;
    public Collider2D bottomCollider;
    public CompositeCollider2D terrainCollider;

    

    float vx = 0;

    // prevVx / ���� �ӵ��� �����Ϸ��� ���� ����
    // ���� �ӵ����� �������� ���¸� run�̶��ϰ� run�̵Ǹ� �ٸ��� �����Ű������
    float prevVx = 0;
    
    bool isGrounded;//�÷��̾ ���� �پ��ִ��� Ȯ���Ϸ��� ���� ����
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
    {// GetAxisRaw Raw�� �� ������ �� �̰��ӿ��� ���ӵ��� ���⶧���� raw
        // GetAxis�� �����ʿ��� ������������ �ε巴��
        //Horizontal / 
        vx = Input.GetAxisRaw("Horizontal");

        if(vx<0)//vx�� �������
        {
            GetComponent<SpriteRenderer>().flipX = true; // flipX�� ������ Ʈ����ϸ� �¿����
            
        }
        if(vx>0)//vx�� ������
        {
            GetComponent<SpriteRenderer>().flipX=false;
        }
        
        
        if (bottomCollider.IsTouching(terrainCollider))// ���� ������ �ݶ��̴� �ΰ��� �پ��ִٸ�
        {
            if (!isGrounded)//�ƴ϶��
            {   
                if (vx == 0)//�̵��ӵ���0�̸� idle
                {
                    GetComponent<Animator>().SetTrigger("Idle");
                }
                else//�̵��ӵ���0�̾ƴϸ� run
                {
                    GetComponent<Animator>().SetTrigger("Run");
                }
            }
            else//��Ӻپ��ִٸ�
            {   
                if (prevVx != vx)//�����ӵ��� ���� �ӵ��� �ٸ��ٸ�
                {
                    if (vx == 0)//�̼���0�̸� idle
                    {
                        GetComponent<Animator>().SetTrigger("Idle");
                    }
                    else//�̼�0�̾ƴϸ� run
                    {
                        GetComponent<Animator>().SetTrigger("Run");
                    }
                }
            }

            isGrounded = true;
        }
        else//���� �پ����� �ʴٸ�
        {   
            if (isGrounded)//���� �پ��ִٸ�
            {   //���� ����
                GetComponent<Animator>().SetTrigger("Jump");
            }
            isGrounded = false;
        }
        //���� ��ǲ �ٹ�ư�� ������ ������ isGrounded�����϶� ������ ������
        if (Input.GetButton("Jump") && isGrounded)
        {// ���۳�Ʈ ������ٵ�2d().�ӵ� = y+1 * �������ǵ�
            GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpSpeed;
        }
        prevVx = vx;

        //�Ѿ˹߻�
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
