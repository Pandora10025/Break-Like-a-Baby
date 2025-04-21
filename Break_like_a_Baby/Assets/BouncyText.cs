using TMPro;
using UnityEngine;

public class BouncyText : MonoBehaviour
{
    public float bounceSpeed = 5f;
    public float bounceAmount = 5f;

    private TMP_Text textComponent;
    private Mesh mesh;
    private Vector3[] vertices;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textComponent.ForceMeshUpdate();

        mesh = textComponent.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < textComponent.textInfo.characterCount; i++)
        {
            var charInfo = textComponent.textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int index = charInfo.vertexIndex;

            float bounce = Mathf.Sin(Time.time * bounceSpeed + i * 0.2f) * bounceAmount;

            for (int j = 0; j < 4; j++)
            {
                vertices[index + j] += new Vector3(0, bounce, 0);
            }
        }

        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }
}