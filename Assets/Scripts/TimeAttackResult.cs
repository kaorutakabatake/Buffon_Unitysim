using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeAttackResult : MonoBehaviour
{
    /* 
    スコアがランキング圏内で登録ボタンが押された際の処理
        GameManagerの変数にユーザを登録しランキングパネルを閉じ、タイムアタック、針をリセットボタンを再表示する
        また、針の状況など盤面をリセットする
    そのままバツボタンが押された際の処理
        ランキングパネルを閉じ、タイムアタック、針をリセットボタンを再表示する
        また、針の状況など盤面をリセットする
     */

    private GameManager gameManagerScript;
    private GameObject resetNeedleObject;
    private ResetNeedle resetNeedle;
    public TMP_InputField inputField;

    void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        resetNeedleObject = GameObject.Find("MainCanvas/BottunCanvas/ResetNeedle");
        resetNeedle = resetNeedleObject.GetComponent<ResetNeedle>();
    }

    public void OnPressedRegister(){
        string tmp = GameObject.Find("MainCanvas/TimeAttackCanvas/RankingPanel/Username").GetComponent<TMP_InputField>().text;
        if (tmp != "" && tmp != "名前を入力してください"){
            // ランキングのユーザー名とスコアを更新
            gameManagerScript.ranking_user_name[gameManagerScript.ranking - 1] = tmp; // 配列は0から始まるため-1
            GameObject.Find("MainCanvas/TimeAttackCanvas/RankingPanel/Username").GetComponent<TMP_InputField>().text = "";
            gameManagerScript.rankingPanel.SetActive(false);
            gameManagerScript.timeAttackBottun.SetActive(true);
            resetNeedle.OnPressed();
            resetNeedleObject.SetActive(true);
            gameManagerScript.isCountDown = false;
            gameManagerScript.isThrowNeedle = true;
            gameManagerScript.isResult = false;//リザルト画面表示中かどうか（主にカメラ切り替え用）
            gameManagerScript.SaveData();
        } else{
            // enterNameTextが空の場合
            // Debug.Log("名前を入力してください");
            GameObject.Find("MainCanvas/TimeAttackCanvas/RankingPanel/Username").GetComponent<TMP_InputField>().text = "名前を入力してください";
        }
    }

     public void OnPressedClose(){
        gameManagerScript.rankingPanel.SetActive(false);
        gameManagerScript.timeAttackBottun.SetActive(true);
        resetNeedleObject.SetActive(true);
        resetNeedle.OnPressed();
        gameManagerScript.isCountDown = false;
        gameManagerScript.isThrowNeedle = true;
        gameManagerScript.isResult = false;
        
     }
}
