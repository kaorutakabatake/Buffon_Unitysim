using Unity.VisualScripting;
using UnityEngine;

public class Needle : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private GameObject RightCube;//針の右端
    private GameObject LeftCube;//針の左端
    private float[] lineZpos;
    private int numberOfLines;
    private GameManager gameManagerScript;
    private bool isCalled = false;

    void Start(){
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        RightCube = this.gameObject.transform.GetChild(0).gameObject;
        LeftCube = this.gameObject.transform.GetChild(1).gameObject;
        meshRenderer = this.gameObject.transform.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.black;
    }

    void Update(){
        if(this.gameObject.GetComponent<Rigidbody>().IsSleeping() && isCalled == false){//動きが止まった時に一度動く
            GameObject object_LineManager = GameObject.Find("LineManager");
            LineManager lineScript = object_LineManager.GetComponent<LineManager>();
            lineZpos = lineScript.lineZpos;
            numberOfLines = lineScript.numberOfLines;
            if(putOutArea(RightCube) != putOutArea(LeftCube)){
                gameManagerScript.needlesCrossed++;
                gameManagerScript.yourNeedlesCrossed++;
                meshRenderer.material.color = Color.red;
            }
            gameManagerScript.numberOfThrows++;
            gameManagerScript.yourNumberOfThrows++;
            gameManagerScript.saveCount++;
            isCalled = true;
        }
    }

    int putOutArea(GameObject Cube){
        float zPos = Cube.transform.position.z;
        for(int i = 0; i < numberOfLines - 1; i++){
            if(lineZpos[i] < zPos && zPos < lineZpos[i + 1]){
                return i;
            }
        }
        return 1000;
    }
}
