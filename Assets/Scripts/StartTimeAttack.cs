using TMPro;
using UnityEngine;

public class StartTimeAttack : MonoBehaviour
{
    private GameManager gameManagerScript;
    private GameObject resetNeedleObject;
    private ResetNeedle resetNeedle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        resetNeedleObject = GameObject.Find("ResetNeedle");
        resetNeedle = resetNeedleObject.GetComponent<ResetNeedle>();
    }
    public void OnPressed()
    {
        gameManagerScript.isCountDown = true;
        resetNeedle.OnPressed();
        resetNeedleObject.SetActive(false);
    }
}
