
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // BallのPrefabが格納される配列
    public GameObject[] ballPrefabs;

    public int minLength = 3;

    private bool isDragged = false;

    private ArrayList removableBallList = new ArrayList();

    private CommonBall last_Ball = new CommonBall();

    public bool isPlaying = true;

    public Text timer;

    private Text timerText;

    public int TIME_LIMIT = 60;

    public int TIME_COUNT = 5;

    private int currentScore;

    public Text scoreText;

    public GameObject gameOver;
    // Start is called before the first frame update
    void Start()
    {
        timerText = timer.GetComponent<Text>();
        //カウントダウンの開始
        StartCoroutine("CountDown");
    }

    private IEnumerator CountDown()
    {
        //変数へ初期値を代入
        int count = TIME_COUNT;
        while (count > 0)
        {
            //カウントダウンのテキストを変更
            timerText.text = count.ToString();
            //1秒待つ
            yield return new WaitForSeconds(1.0f);
            //カウントを1つ減らす
            count -= 1;
        }
        //テキストの表示
        timerText.text = "Start!";
        //テキスト表示のための遅延
        yield return new WaitForSeconds(1.0f);
        //ボールの落下開始
        StartCoroutine("DropBall");
        //タイマーの開始
        StartCoroutine("GameTimer");
    }

    private IEnumerator GameTimer()
    {
        int count = TIME_LIMIT;
        while (count > 0)
        {
            if(isPlaying == false)
            {
                break;
            }
            //カウントダウンのテキストを変更
            timerText.text = count.ToString();
            //1秒待つ
            yield return new WaitForSeconds(1.0f);
            //カウントを1つ減らす
             count -= 1;
        }
        //テキストの表示
        if(count == 0)
        {
            timerText.text = "Finish";
        }
        isPlaying = false;
        foreach (GameObject ball in removableBallList)
        {
            ball.transform.GetComponent<CommonBall>().ResetColor();
        }
    }

    //ドラッグし始めた時
    private void OnDragStart()
    {
        //マウスオーバーしているオブジェクトの取得
        GameObject targetObject = GetCurrentTarget();
        //リスト内の要素を全てリストから消去
        removableBallList.Clear();
        //nullでないなら
        if (targetObject)
        {
            //オブジェクトがBallなら
            if (targetObject.name.IndexOf("Ball") != -1)
            {
                //取得したオブジェクトを格納
                last_Ball = targetObject.GetComponent<CommonBall>();
                //最初にクリックしたボールを消去用リストに追加
                removableBallList.Add(targetObject);
                //追加したボールの値の更新
                last_Ball.isAdd = true;
                //ボールの色の変更
                ChangeColor(targetObject);
            }
        }
    }

    //ドラッグしている時
    private void OnDragging()
    {
        //マウスオーバーしているオブジェクトの取得
        GameObject targetObject = GetCurrentTarget();
        //nullでないなら
        if (targetObject)
        {
            //オブジェクトがBallなら
            if (targetObject.name.IndexOf("Ball") != -1)
            {
                //Componentの取得
                CommonBall targetBall = targetObject.transform.GetComponent<CommonBall>();
                //最初に入れたボールと同じprefabのボールなら
                if (targetBall.KindOfId == last_Ball.KindOfId)
                {
                    //既に追加されたボールでないなら
                    if (targetBall.isAdd == false)
                    {
                        var dist = Vector2.Distance(last_Ball.transform.position, targetBall.transform.position);
                        if (dist <= 1.0)
                        {
                            //消去用リストに追加
                            removableBallList.Add(targetObject);
                            //追加したボールの値の更新
                            targetBall.isAdd = true;
                            //ボールの色の変更
                            ChangeColor(targetObject);
                            //最後に選択したボールの変更
                            last_Ball = targetBall;
                        }
                    }
                }
            }
        }
    }

    //ドラッグし終えた時
    private void OnDraggEnd()
    {
        //リストの長さを取得
        int length = removableBallList.Count;
        //消去リストの長さがminLength以上であれば
        if (length >= minLength)
        {
            foreach (GameObject ball in removableBallList)
            {
                //スコア加算
                currentScore += length;
                //スコアの描画の更新
                scoreText.text = "Score: " + currentScore.ToString();
                if(ball.transform.GetComponent<CommonBall>().KindOfId == 5)
                {
                    Explode(ball);
                }
                //ボールの消去
                Destroy(ball);
            }
            removableBallList.Clear();
        }
        //長さがminLength未満であれば
        else
        {
            foreach (GameObject ball in removableBallList)
            {
                //追加されたボールの値のリセット
                ball.transform.GetComponent<CommonBall>().isAdd = false;
                //Colorのリセット
                ball.transform.GetComponent<CommonBall>().ResetColor();
            }
        }
    }

    //現在のマウスの位置にあるオブジェクトを取得する関数
    private GameObject GetCurrentTarget()
    {
        GameObject atDeleteTarget = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

        if (hit2d)
        {
            atDeleteTarget = hit2d.transform.gameObject;
        }

        return atDeleteTarget;
    }

    //ボール落下用関数
    IEnumerator DropBall()
    {
        //int Before_Index = 0;
        int RANDOM_INDEX;
        while (isPlaying)
        {
            //生成されるボールの種類の乱数
            /*do
            {
                RANDOM_INDEX = Random.Range(0, ballPrefabs.Length);
            }
            while (Before_Index == RANDOM_INDEX);
            Before_Index = RANDOM_INDEX;*/
            RANDOM_INDEX = Random.Range(0, ballPrefabs.Length);
            //ボールが生成されるx座標の乱数
            float RANDOM_X = Random.Range(-2.0f, 2.0f);
            //ボールが生成される座標のVector
            Vector3 BALL_INITIAL_POSITION = new Vector3(RANDOM_X, 7.0f, 0.0f);
            //ボールの生成
            GameObject clonedBall = Instantiate(ballPrefabs[RANDOM_INDEX]);
            //ボールの座標設定
            clonedBall.transform.position = BALL_INITIAL_POSITION;
            //ボール生成待機(0.5秒)
            yield return new WaitForSeconds(0.5f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            if (Input.GetMouseButtonDown(0) && isDragged == false)
            {
                isDragged = true;
                OnDragStart();
            }
            else if (Input.GetMouseButton(0) && isDragged == true)
            {
                OnDragging();
            }
            else
            {
                isDragged = false;
                OnDraggEnd();
            }
        }
        if(gameOver.GetComponent<GameOver>().isGameOver == true)
        {
            isPlaying = false;
        }
    }

    private void ChangeColor(GameObject obj)
    {
        //オブジェクトにアタッチされているマテリアルコンポーネントを取得
        Material ballMaterial = obj.GetComponent<Renderer>().material;
        //マテリアルのカラーを変更
        ballMaterial.SetFloat("_Metallic", 1.0f);
    }

    private void Explode(GameObject ball)
    {
        float r = 0.5f;
        RaycastHit2D[] hits;
        hits = Physics2D.CircleCastAll(ball.transform.position, r, ball.transform.forward);
        Debug.Log(hits.Length);
        //スコア加算
        currentScore += hits.Length;
        //スコアの描画の更新
        scoreText.text = "Score; " + currentScore.ToString();
        foreach (var hit in hits)
        {
            if(hit.collider.tag == ("Ball"))
            {
                Debug.Log("ばくはつ");
                Destroy(hit.collider.gameObject);
            }
        }
        
    }
}

