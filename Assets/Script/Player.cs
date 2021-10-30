using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] RectTransform Transform;
    public float m_jumpHeight = 550f, m_scrollSpeed = 10f;
    public const int m_maxJumpCount = 2;
    [SerializeField]const float m_barrierTime = 10;
    public int m_jumpCount = 0;
    public bool isJump = false;
    public bool isInvincible = false;
    public bool isDamage = false;
    Rigidbody2D rb2d;
    public float m_alphaSin;
    [SerializeField] CapsuleCollider2D m_collider;
    float m_initialHeight;
    // Start is called before the first frame update
    void Start()
    {
        //RigidBody2Dコンポーネントを取得
        rb2d = GetComponent<Rigidbody2D>();
        m_initialHeight = m_collider.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        //操作入力の取得
        float jumpKey = Input.GetAxis("Jump");
        //alpha値を変化させるための数値を時間参照でとる
        m_alphaSin = Mathf.Sin(Time.time) /2 + 0.5f;
        //RigidBody2Dの速度にVector2を設定
        rb2d.velocity = new Vector2(m_scrollSpeed, rb2d.velocity.y);

        //スペースキーが押されたときに上限回数ジャンプしていないとき
        if (Input.GetKeyDown(KeyCode.Space) && m_jumpCount < m_maxJumpCount)
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
            rb2d.AddForce(Vector2.up * m_jumpHeight, ForceMode2D.Impulse);
            //ジャンプの回数をカウント
            m_jumpCount++;
            //ジャンプを許可する
            isJump = false;
        }
        //しゃがみボタンが押されたとき
        if (Input.GetButton("crouch"))
        {
            Vector2 size = m_collider.size;
            size.y = m_initialHeight / 2;
            m_collider.size = size;
        }
        //離された時
        else
        {
            Vector2 size = m_collider.size;
            size.y = m_initialHeight;
            m_collider.size = size;
        }
    }
    //衝突した時、相手オブジェクトのタグによって処理を行う
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            //Playerが地面と衝突したときにジャンプの回数をリセットする
            case "Ground":
                Debug.Log("衝突");
                m_jumpCount = 0;  //回数をリセット
                break;
            //無敵アイテムを拾ったときに数秒の間、障害物に当たらないようにする
            case "invincible":
                Debug.Log("無敵");
                Destroy(collision.gameObject);
                StartCoroutine(InvincibleCoroutine());
                //Debug.Log(isInvincible);
                break;
            //障害物に当たったときに無敵時間を作りダメージを与える
            case "Obstacle":
                Destroy(collision.gameObject);
                //Debug.Log(isInvincible);
                if (isInvincible == false)
                {
                    Debug.Log("障害物");
                    StartCoroutine(ObstacleCoroutine());
                    StartCoroutine(ColorCoroutine());
                    isDamage = false;
                }
                break;
        }
    }
    //無敵アイテムのコルーチン
    public IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        //Debug.Log("無敵切れ2");
        yield return new WaitForSeconds(m_barrierTime);
        isInvincible = false;
        Debug.Log("無敵アイテム切れ");

    }
    //障害物に衝突したときのコルーチン
    public IEnumerator ObstacleCoroutine()
    {
        isDamage = true;
        yield return new WaitForSeconds(3);
        Debug.Log("無敵時間終了");
    }

    public IEnumerator ColorCoroutine()
    {
        Color color = this.gameObject.GetComponent<MeshRenderer>().material.color;
        while (true)
        {

            Color _color = this.gameObject.GetComponent<MeshRenderer>().material.color;
            if (isDamage==false)
            {
                this.gameObject.GetComponent<MeshRenderer>().material.color = color;
                break;
            }
            yield return new WaitForEndOfFrame();
            _color.g = m_alphaSin;
            _color.b = m_alphaSin;
            this.gameObject.GetComponent<MeshRenderer>().material.color = _color;
        }

    }
}
