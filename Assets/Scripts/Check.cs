using UnityEngine;

public class Check : MonoBehaviour
{
    [SerializeField]
    private Color TextColor = Color.white;
    [SerializeField]
    private float DestroyTime = 1f;
    private float alpha = 0f;

    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }

    private void Update()
    {
        alpha += 0.34f * Time.deltaTime;
        GetComponent<Renderer>().material.color = new Color(TextColor.r, TextColor.g, TextColor.b, 1f - alpha);
    }
}
