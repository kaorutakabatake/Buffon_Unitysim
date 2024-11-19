using UnityEngine;

public class ResetNeedle : MonoBehaviour
{
    private GameManager gameManagerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void OnPressed()
    {
        gameManagerScript.yourNeedlesCrossed = 0;
        gameManagerScript.yourNumberOfThrows = 0;
        int step = gameManagerScript.needlesQueue.Count;//queueに入っている針の本数
        for(int i = 0; i < step; i++){
            GameObject oldNeedle = gameManagerScript.needlesQueue.Dequeue();
            Destroy(oldNeedle);
        }
    }
}
