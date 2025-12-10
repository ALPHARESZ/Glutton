using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class TriangleItem : MonoBehaviour
{
    [Header("Waktu Hidup dan Transisi")]
    public float appearDuration = 0.7f;    // durasi kemunculan (muncul pelan)
    public float lifetime = 8f;            // waktu sebelum mulai menghilang
    public float shrinkDuration = 1f;      // waktu pengecilan (menghilang)

    [Header("Efek Partikel Lenyap")]
    public GameObject vanishEffectPrefab;

    private float timer = 0f;
    private bool isAppearing = true;
    private bool isShrinking = false;

    private Vector2 originalScale2D;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        // Simpan ukuran awal dan set ke 0 dulu (invisible)
        originalScale2D = new Vector2(transform.localScale.x, transform.localScale.y);
        transform.localScale = Vector3.zero;

        // Mulai coroutine untuk muncul perlahan
        StartCoroutine(ManualAppear());
    }

    void Update()
    {
        // Hitung waktu hidup setelah selesai muncul
        if (!isAppearing)
        {
            timer += Time.deltaTime;

            if (!isShrinking && timer >= lifetime)
            {
                isShrinking = true;
                StartCoroutine(ManualShrinkAndDestroy());
            }
        }
    }

    // ====================== MANUAL APPEAR ======================
    IEnumerator ManualAppear()
    {
        float t = 0f;

        while (t < appearDuration)
        {
            t += Time.deltaTime;
            float progress = t / appearDuration;

            // Skala linear manual: s(t) = (t / T)
            float s = progress;
            if (s > 1f) s = 1f;

            float newX = originalScale2D.x * s;
            float newY = originalScale2D.y * s;
            transform.localScale = new Vector3(newX, newY, 1f);

            // Alpha manual: a(t) = (t / T)
            if (sr != null)
            {
                Color c = originalColor;
                c.a = progress;
                sr.color = c;
            }

            yield return null;
        }

        // Setelah muncul penuh
        isAppearing = false;
        timer = 0f;
    }

    // ====================== MANUAL SHRINK ======================
    IEnumerator ManualShrinkAndDestroy()
    {
        float t = 0f;

        while (t < shrinkDuration)
        {
            t += Time.deltaTime;
            float progress = t / shrinkDuration;

            // Skala manual mengecil: s(t) = 1 - (t / T)
            float s = 1.0f - progress;
            if (s < 0f) s = 0f;

            float newX = originalScale2D.x * s;
            float newY = originalScale2D.y * s;
            transform.localScale = new Vector3(newX, newY, 1f);

            // Fade-out manual
            if (sr != null)
            {
                Color c = originalColor;
                c.a = 1.0f - progress;
                sr.color = c;
            }

            yield return null;
        }

        // Tambahkan efek lenyap
        if (vanishEffectPrefab != null)
            Instantiate(vanishEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Segitiga dimakan oleh player!");
            if (vanishEffectPrefab != null)
                Instantiate(vanishEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }
}
