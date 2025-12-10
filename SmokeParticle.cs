using UnityEngine;

public class SmokeParticle : MonoBehaviour
{
    // Penyimpanan manual
    private Vector2 position;
    private float scale;
    private float alpha;

    // Parameter efek asap
    public float riseSpeed = 0.5f;       // ke atas
    public float fadeSpeed = 0.5f;       // memudar
    public float growSpeed = 0.1f;       // membesar
    public float lifetime = 2f;          // lama muncul

    private float timer;

    void Start()
    {
        // Posisi awal di-set oleh spawner
        timer = 0f;
        scale = 0.2f;
        alpha = 1f;
    }

    // Dipanggil dari luar untuk set posisi awal
    public void SetStartPosition(Vector2 p)
    {
        position = p;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // ========= TRANSFORMASI MANUAL ==========
        // 1. translasi ke atas
        position += new Vector2(0, riseSpeed * Time.deltaTime);

        // 2. scaling manual
        scale += growSpeed * Time.deltaTime;

        // 3. fade manual
        alpha -= fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha - fadeSpeed * Time.deltaTime);

        // ============ APPLY TO UNITY ============
        transform.position = new Vector3(position.x, position.y, 0f);
        transform.localScale = new Vector3(scale, scale, 1f);

        // ubah alpha pada sprite renderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(0.5f, 0.5f, 0.5f, 1f); 
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}
