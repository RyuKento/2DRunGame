using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] RectTransform Transform;
    public float jp = 550f,scroll = 10f;
    public const int MAX_JUMP_COUNT = 2;
    public int jumpCount = 0;
    public bool isJump =false;
    Rigidbody2D rb2d;
    public float hp;
    private Animator animCrouch = null;
    private Animator animJump = null;
    [SerializeField] CapsuleCollider2D　m_collider;
    float m_initialHeight;
    // Start is called before the first frame update
    void Start()
    {
        //RigidBody2Dコンポーネントを取得
        rb2d = GetComponent<Rigidbody2D>();
        animCrouch = GetComponent<Animator>();
        animJump = GetComponent<Animator>();
        m_initialHeight = m_collider.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        float crouchKey = Input.GetAxis("crouch");
        float jumpKey = Input.GetAxis("Jump");
        //体力を常に低下
        hp += 0.01f;
        //RigidBody2Dの速度にVector2を設定
        rb2d.velocity = new Vector2(scroll, rb2d.velocity.y);
        //スペースキーが押されたときに上限回数ジャンプしていないとき
        if (Input.GetKeyDown(KeyCode.Space)&&jumpCount<MAX_JUMP_COUNT)
        {
            //ジャンプを許可する
            isJump = true;
        }
        //ジャンプが許可されているとき
        if (isJump)
        {
            //RigidBody2Dの速度をリセットし2度目のジャンプの挙動を1度目と同じにする
            rb2d.velocity = Vector2.zero;
            //ジャンプさせる
            rb2d.AddForce(Vector2.up * jp,ForceMode2D.Impulse);
            //ジャンプの回数をカウント
            jumpCount++;
            //ジャンプを許可する
            isJump = false;
        }
        if (crouchKey < 0)
        {
            animCrouch.SetBool("crouch", true);
            Vector2 size = m_collider.size;
            size.y = m_initialHeight / 2;
            m_collider.size = size;
        }
        else
        {
            animCrouch.SetBool("crouch", false);
            Vector2 size = m_collider.size;
            size.y = m_initialHeight;
            m_collider.size = size;
        }
        if (jumpKey< 0)
        {
            animJump.SetBool("Jump", true);
        }
        else
        {
            animJump.SetBool("Jump", false);
        }
    }

    //Playerが地面と衝突したときにジャンプの回数をリセットする
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            Debug.Log("衝突");
            jumpCount = 0;  //回数をリセット
        }
    }
}
