using TMPro;
using UnityEngine;

public class StartTimeAttack : MonoBehaviour
{
    private GameManager gameManagerScript;
    private GameObject resetNeedleObject;
    private ResetNeedle resetNeedle;
    void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        resetNeedleObject = GameObject.Find("MainCanvas/BottunCanvas/ResetNeedle");
        resetNeedle = resetNeedleObject.GetComponent<ResetNeedle>();
    }
    public void OnPressed()
    {
        gameManagerScript.isCountDown = true;
        resetNeedle.OnPressed();
        resetNeedleObject.SetActive(false);
    }
}
