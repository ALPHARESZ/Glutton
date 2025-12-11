using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class TriangleItem : MonoBehaviour
{
    [Header("Waktu Hidup")]
    public float appearDuration = 0.7f;
    public float lifetime = 8f;
    public float shrinkDuration = 1f;

    [Header("Efek Partikel")]
    public GameObject vanishEffectPrefab;

    [Header("Efek Mati (Dimakan)")]
    public float deathDelay = 0.15f; // Durasi merah sebelum hilang

    private float timer = 0f;
    private bool isAppearing = true;
    private bool isShrinking = false;
    private bool isDead = false;

    private Vector2 originalScale2D;
    private SpriteRenderer sr;
    private Collider2D col;
    private Material matInstance; // Instance material unik untuk objek ini

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Membuat instance material agar perubahan warna tidak menular ke segitiga lain
        if (sr != null)
        {
            matInstance = sr.material; 
        }

        // Simpan ukuran awal
        originalScale2D = new Vector2(transform.localScale.x, transform.localScale.y);
        transform.localScale = Vector3.zero;

        // Mulai muncul
        StartCoroutine(ManualAppear());
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        if (isDead) return;

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

    // --- FUNGSI UTAMA: DIPANGGIL SAAT DIMAKAN ---
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (col != null) col.enabled = false; // Matikan collider
        StopAllCoroutines(); // Hentikan animasi lain
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // 1. Ubah properti "_Color" pada Shader menjadi Merah
        if (matInstance != null)
        {
            // Warna Merah Terang (Neon Red)
            matInstance.SetColor("_Color", new Color(1f, 0f, 0f, 1f)); 
            
            // Opsional: Bikin lebih terang/menyilaukan saat dimakan
            matInstance.SetFloat("_GlowIntensity", 4.0f); 
        }

        // 2. Tunggu sekejap
        yield return new WaitForSeconds(deathDelay);

        // 3. Spawn partikel & Hancurkan
        if (vanishEffectPrefab != null)
            Instantiate(vanishEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // --- ANIMASI MUNCUL & MENGHILANG (TETAP SAMA) ---
    IEnumerator ManualAppear()
    {
        float t = 0f;
        while (t < appearDuration)
        {
            t += Time.deltaTime;
            float progress = t / appearDuration;
            
            // Update Scale
            float s = Mathf.Clamp01(progress);
            transform.localScale = new Vector3(originalScale2D.x * s, originalScale2D.y * s, 1f);

            // Update Alpha (Shader kita membaca Vertex Color alpha)
            if (sr != null)
            {
                Color c = sr.color;
                c.a = progress;
                sr.color = c;
            }
            yield return null;
        }
        isAppearing = false;
    }

    IEnumerator ManualShrinkAndDestroy()
    {
        float t = 0f;
        while (t < shrinkDuration)
        {
            t += Time.deltaTime;
            float progress = t / shrinkDuration;

            float s = Mathf.Clamp01(1.0f - progress);
            transform.localScale = new Vector3(originalScale2D.x * s, originalScale2D.y * s, 1f);

            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1.0f - progress;
                sr.color = c;
            }
            yield return null;
        }

        if (vanishEffectPrefab != null)
            Instantiate(vanishEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}