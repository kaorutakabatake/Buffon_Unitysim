using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


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
    private float TIME_ATTACK_LIMIT = 12;
    public float limitTime;
    public bool isCountDown = false;
    private bool isThrowNeedle = true;
    private GameObject resetNeedleObject;
    public GameObject timeAttackBottun;
    private GameObject timeBack;
    private string[] ranking_user_name = new string[10];
    private long[] ranking_number_of_throws = new long[10];
    private long[] ranking_needles_crossed = new long[10];
    private StreamReader sr;

    void Start()
    {
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


        timeText.SetText("");
        startText.SetText("");
        startText.color = Color.red;
        resetNeedleObject = GameObject.Find("ResetNeedle");
        timeBack = GameObject.Find("TimeBack");//時間制限の背景
        timeBack.SetActive(false);

        // ファイルから総投擲数、総横切り数、ランキングデータを読み込む
        sr = new StreamReader("Assets/SaveData/file_save.txt");
        string[] needle_data_tmp =  sr.ReadLine().Split(":");
        numberOfThrows = long.Parse(needle_data_tmp[0]);
        needlesCrossed = long.Parse(needle_data_tmp[1]);
        for(int i = 0; i < 10; i++){
            string[] ranking_data_tmp =  sr.ReadLine().Split(":");
            ranking_user_name[i] = ranking_data_tmp[1];//名前
            ranking_number_of_throws[i] = long.Parse(ranking_data_tmp[2]);//投擲数
            ranking_needles_crossed[i] = long.Parse(ranking_data_tmp[3]);//横切り数
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
                resetNeedleObject.SetActive(true);//針をリセットボタンを有効化
                timeAttackBottun.SetActive(true);//タイムアタックボタンを有効化
                timeBack.SetActive(false);//時間制限の背景を無効化
                isCountDown = false;//カウントダウン中か否かのフラグをfalseに
                isThrowNeedle = false;//針を投げられるか否かのフラグをfalseに
                timeText.SetText("");
                limitTime = TIME_ATTACK_LIMIT;//制限時間を初期化
            } else if(limitTime < 10){//タイムアタックゲーム中
                startText.SetText("");
                timeText.SetText("残り時間：" + limitTime.ToString("F0"));
                timeBack.SetActive(true);//時間制限の背景を有効化
                isThrowNeedle = true;
            } else if(limitTime < 11){
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

        //Eを押した時針を投入
        if(Input.GetKeyDown(KeyCode.E)){
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
            string tmp = numberOfThrows.ToString()+ ":" + needlesCrossed.ToString();
            for(int i = 0; i < 10; i++){
                tmp = tmp + "\n" + i.ToString() + ":" + ranking_user_name[i] + ":" + ranking_number_of_throws[i] + ":" + ranking_needles_crossed[i];
            }
            StreamWriter sw = new StreamWriter("Assets/SaveData/file_save.txt", false);
            sw.WriteLine(tmp);
            sw.Flush();
            sw.Close();
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
}