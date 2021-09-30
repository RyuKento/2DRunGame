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
    public bool isJump = false, isInvincible = false;
    Rigidbody2D rb2d;
    public float alpha_Sin;
    [SerializeField] CapsuleCollider2D　m_collider;
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
        alpha_Sin = Mathf.Sin(Time.time) / 2 + 0.5f;
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
                jumpCount = 0;  //回数をリセット
                break;
            //無敵アイテムを拾ったときに数秒の間、障害物に当たらないようにする
            case "invincible":
                Debug.Log("無敵");
                Destroy(collision.gameObject);
                StartCoroutine(InvincibleCoroutine());
                isInvincible = false;
                Debug.Log("無敵切れ1");
                break;
            //障害物に当たったときに無敵時間を作りダメージを与える
            case "Obstacle":
                Destroy(collision.gameObject);
                if (isInvincible == true) 
                {
                    Debug.Log("障害物");
                    StartCoroutine(ObstacleCoroutine());
                    isInvincible = false;
                }
                break;
        }
    }
    //無敵アイテムのコルーチン
    public IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        Debug.Log("無敵切れ2");
        yield return new WaitForSeconds(10f);
    }
    //障害物に衝突したときのコルーチン
    public IEnumerator ObstacleCoroutine() { 
                isInvincible = true;
                yield return new WaitForSeconds(3);
    }
}
