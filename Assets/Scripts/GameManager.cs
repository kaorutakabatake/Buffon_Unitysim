using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private GameObject topCamera;      //トップカメラ格納用
    private GameObject sideCamera;       //サイドカメラ格納用
    private bool isTopCamera = true;
    public GameObject needlePrefab; // プレハブ化した針オブジェクト
    public long numberOfThrows = 0; // 投げた針の数
    public int yourNumberOfThrows = 0; // 個人が投げた針の数
    public long needlesCrossed = 0; // 線を横切った針の数
    public int yourNeedlesCrossed = 0; // 個人の線を横切った針の数
    public int saveCount = 0;
    public int maxNeedles = 3000; // 最大本数
    public Queue<GameObject> needlesQueue = new Queue<GameObject>(); // 針オブジェクトを管理するキュー
    public TMP_Text gameInfoText;
    public TMP_Text timeText;
    public TMP_Text startText;
    private float TIME_ATTACK_LIMIT = 13;//11~1がゲーム中
    public float limitTime;
    public bool isCountDown = false;
    public bool isThrowNeedle = true;
    public GameObject timeAttackBottun;
    private GameObject timeBack;
    private int RANKING_USER_NUM = 20;//ランキングのユーザー数
    public string[] ranking_user_name;//ランキングのユーザー名
    private float[] ranking_user_score;//ランキングのユーザースコア(誤差)
    private StreamReader sr;
    public GameObject rankingPanel;//ランキングパネル全体のオブジェクト
    public TMP_Text rankingText;//結果画面のランキング表示用
    public TMP_Text userScoreText;//結果画面のユーザースコア表示用
    // public TMP_Text enterNameText;//結果画面の名前入力用
    public int ranking;//ユーザーのランキング
    public bool isResult = false;
    public GameObject resisterButton;
    public GameObject usernameObject;

    void Start()
    {
        ranking_user_name = new string[RANKING_USER_NUM];
        ranking_user_score = new float[RANKING_USER_NUM];
        limitTime = TIME_ATTACK_LIMIT;
        topCamera = GameObject.Find("TopCamera");
        sideCamera = GameObject.Find("SideCamera");
        gameInfoText.SetText(
            "　　　　円周率は...　" + "\n" +
            "今までの円周率は...　" + "\n" +
            "　　　　　誤差は...　" + "\n" +
            "あなたの円周率は...　" + "\n" +
            "　　　　　誤差は...　"
        );

        rankingPanel = GameObject.Find("RankingPanel");//ランキングパネルを取得
        rankingPanel.SetActive(false);//ランキングパネルを非表示に

        timeText.SetText("");
        startText.SetText("");
        startText.color = Color.red;
        timeBack = GameObject.Find("TimeBack");//時間制限の背景
        timeBack.SetActive(false);

        // ファイルから総投擲数、総横切り数、ランキングデータを読み込む
        // ファイルには1行目に総投擲数と総横切り数、2行目以降にランキングデータ(14人分)が保存されている
        //ランキングデータは「順位:名前:誤差」の形式で保存されている
        sr = new StreamReader("Assets/SaveData/file_save.txt");
        //1行目のデータを読み込む
        string[] needle_data_tmp =  sr.ReadLine().Split(":");
        numberOfThrows = long.Parse(needle_data_tmp[0]);
        needlesCrossed = long.Parse(needle_data_tmp[1]);
        //2行目以降のデータを読み込む
        for(int i = 0; i < RANKING_USER_NUM; i++){
            string[] ranking_data_tmp =  sr.ReadLine().Split(":");
            ranking_user_name[i] = ranking_data_tmp[1];//名前
            ranking_user_score[i] = float.Parse(ranking_data_tmp[2]);//スコア
        }
        sr.Close();

    }

    void throwNeedle()
    {
        // ランダムな位置と角度を生成
        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(-8.0f, 8.0f), 4.0f, UnityEngine.Random.Range(-8.0f, 8.0f)); // 高さ4から落とす
        Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));

        // 針を生成
        GameObject newNeedle = Instantiate(needlePrefab, randomPosition, randomRotation);
        needlesQueue.Enqueue(newNeedle);

        // 針が最大数を超えた場合、古いものを削除
        if (needlesQueue.Count > maxNeedles)
        {
            GameObject oldNeedle = needlesQueue.Dequeue(); // 古い針を取り出す
            Destroy(oldNeedle); // 古い針を削除
        }
    }

    void Update(){
        //制限時間
        if(isCountDown){
            limitTime -= Time.deltaTime;
            if(limitTime < 0){//タイムアウト
                // resetNeedleObject.SetActive(true);//針をリセットボタンを有効化
                // timeAttackBottun.SetActive(true);//タイムアタックボタンを有効化
                isResult = true;
                rankingPanel.SetActive(true);//ランキングパネルを有効化
                timeBack.SetActive(false);//時間制限の背景を無効化
                isCountDown = false;//カウントダウン中か否かのフラグをfalseに
                isThrowNeedle = false;//針を投げられるか否かのフラグをfalseに
                timeText.SetText("");
                limitTime = TIME_ATTACK_LIMIT;//制限時間を初期化
                userScoreText.SetText(//ランキングパネルのやつ
                    "投げた総数は：" + yourNumberOfThrows + "\n" +
                    "交わった針は：" + yourNeedlesCrossed + "\n" +
                    "あなたの円周率は：" + 2.0f * 2.0f * yourNumberOfThrows / (yourNeedlesCrossed * 2.0f) + "\n" +
                    "誤差は：" + MathF.Abs(MathF.PI - 2.0f * 2.0f * yourNumberOfThrows / (yourNeedlesCrossed * 2.0f))
                );
                ranking = setRankingData("YOU!", MathF.Abs(MathF.PI - 2.0f * 2.0f * yourNumberOfThrows / (yourNeedlesCrossed * 2.0f)));//ランキングデータを更新
                if(ranking <= RANKING_USER_NUM){
                    resisterButton.SetActive(true);
                    usernameObject.SetActive(true);
                } else {
                    resisterButton.SetActive(false);
                    usernameObject.SetActive(false);
                }
                setRankingText();

            } else if(limitTime < 1){
                isThrowNeedle = false;//針を投げられるか否かのフラグをfalseに
            } else if(limitTime < 11){//タイムアタックゲーム中
                startText.SetText("");
                timeText.SetText("残り時間：" + (limitTime - 1).ToString("F0"));//1秒で終わるので調整
                timeBack.SetActive(true);//時間制限の背景を有効化
                isThrowNeedle = true;
            } else if(limitTime < 12){
                startText.SetText("スタート！");
            } else {
                timeAttackBottun.SetActive(false);
                startText.SetText("よーい");
                isThrowNeedle = false;
            }
        }

        //Spaceを押した時針を投入
        if(Input.GetKeyDown(KeyCode.Space) && isThrowNeedle){
            for(int i = 0; i < 100; i++){
                throwNeedle();
            }
        }

        //Eを押した時カメラマンを切り替え
        if(Input.GetKeyDown(KeyCode.E) && isResult == false){
            if(isTopCamera){
                topCamera.SetActive(false);
                sideCamera.SetActive(true);
                isTopCamera = false;
            } else {
                topCamera.SetActive(true);
                sideCamera.SetActive(false);
                isTopCamera = true;
            }
        }

        if(saveCount > 1000){//10000本ごとにセーブ
            Debug.Log("save!");
            SaveData();
            saveCount = 0;
        }

        // // 針が線を横切った数を基にπを計算
        if (needlesCrossed > 0){
            float piEstimate = 2.0f * 2.0f * numberOfThrows / (needlesCrossed * 2.0f);
            float yourPiEstimate = 2.0f * 2.0f * yourNumberOfThrows / (yourNeedlesCrossed * 2.0f);
            gameInfoText.SetText(
                "　　　　円周率は　" + MathF.PI + "\n" +
                "今までの円周率は　" + piEstimate + "\n" +
                "　　　　　誤差は　" + MathF.Abs(MathF.PI - piEstimate) + "\n" +
                "あなたの円周率は　" + yourPiEstimate + "\n" +
                "　　　　　誤差は　" + MathF.Abs(MathF.PI - yourPiEstimate)
            );
        }
    }

    void setRankingText(){
        string tmp = "";
        for(int i = 0; i < RANKING_USER_NUM; i++){
            tmp = tmp + (i + 1).ToString() + "位：" + ranking_user_name[i] + "　　" + ranking_user_score[i].ToString() + "\n";
        }
        rankingText.SetText(tmp);
    }

    int setRankingData(string user_name, float user_score){//ランキングデータを更新 スコアは小さいものが上位 返値は順位
        int rank = RANKING_USER_NUM + 1; // 初期値は最下位+1
        for(int i = 0; i < RANKING_USER_NUM; i++){
            if(user_score < ranking_user_score[i]){
                rank = i;
                for(int j = RANKING_USER_NUM - 1; j > i; j--){
                    ranking_user_name[j] = ranking_user_name[j - 1];
                    ranking_user_score[j] = ranking_user_score[j - 1];
                }
                ranking_user_name[i] = user_name;
                ranking_user_score[i] = user_score;
                break;
            }
        }
        return rank + 1; // 順位は1から始まるようにする
    }

    public void SaveData(){
        string tmp = numberOfThrows.ToString()+ ":" + needlesCrossed.ToString();//1行目に総投擲数と総横切り数を保存
        //2行目以降にランキングデータを保存
        for(int i = 0; i < RANKING_USER_NUM; i++){
            tmp = tmp + "\n" + i.ToString() + ":" + ranking_user_name[i] + ":" + ranking_user_score[i].ToString();
        }
        StreamWriter sw = new StreamWriter("Assets/SaveData/file_save.txt", false);
        sw.WriteLine(tmp);
        sw.Flush();
        sw.Close();
    }
}