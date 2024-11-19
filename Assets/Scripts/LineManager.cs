using UnityEngine;

public class LineManager : MonoBehaviour
{
    public int numberOfLines = 11;  // 線の数
    private float lineSpacing = 2.0f; // 線の間隔
    public float[] lineZpos;
    
    void Start()
    {
        lineZpos = new float[numberOfLines];
        for (int i = 0; i < numberOfLines; i++)
        {
            CreateLine(i);
        }
    }

    void CreateLine(int index)
    {
        // 新しいラインを生成し、適切な位置に配置
        GameObject newLine = new GameObject("Line" + index);  // 空のオブジェクトを作成
        LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();  // LineRendererを追加

        // タグを設定
        newLine.tag = "Line";

        lineRenderer.positionCount = 2;
        float zPos = index * lineSpacing - 10; // 各線のZ座標を間隔に基づいて設定
        lineZpos[index] = zPos;
        lineRenderer.SetPosition(0, new Vector3(-10,(float)0.1, zPos));
        lineRenderer.SetPosition(1, new Vector3(10, (float)0.1, zPos));
        
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));  // デフォルトのマテリアルを設定
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;

        // // BoxColliderを追加して線に物理的な判定を持たせる
        // BoxCollider collider = newLine.AddComponent<BoxCollider>();
        // collider.size = new Vector3(20, 0.1f, 0.1f);  // 線の長さと同じようにコライダーを設定
        // collider.center = new Vector3(0, 0, zPos);
        // collider.isTrigger = false;  // 衝突判定のためにTriggerにする
    }
}
