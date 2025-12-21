using UnityEngine;

public class ManualCircle : MonoBehaviour
{
    private Material playerMat;

    // posisi manual yang kita simpan sendiri (bukan memakai transform langsung)
    private Vector2 position;

    public float lifetime;      // lama hidup lingkaran
    private float timer;
    public float playerSize = 1f;

    // kecepatan gerak
    public float speed = 2f;
    public float speedBoostMultiplier = 2f;

    // Prefab asap
    public GameObject smokePrefab;
    public float smokeSpawnRate = 0.1f; // 1 asap setiap 0.1 detik
    private float smokeTimer = 0f;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        playerMat = GetComponent<Renderer>().material;

        // warna awal putih
        playerMat.SetColor("_Color", Color.white);

        // inisialisasi posisi awal lingkaran (0,0)
        position = new Vector2(0f, 0f);
    }

    void Update()
    {
        // Cegah player bergerak atau mengubah skor setelah game berakhir
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        timer += Time.deltaTime;

        if (timer >= lifetime && sr != null)
        {
            playerMat.SetColor("_Color", Color.white);
        }

        Vector2 translation = Vector2.zero;

        // membaca input manual (tanpa Input.GetAxis)
        if (Input.GetKey(KeyCode.W))
            translation += new Vector2(0, 1);

        if (Input.GetKey(KeyCode.S))
            translation += new Vector2(0, -1);

        if (Input.GetKey(KeyCode.A))
            translation += new Vector2(-1, 0);

        if (Input.GetKey(KeyCode.D))
            translation += new Vector2(1, 0);

        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // ======== NORMALISASI MANUAL ========
        if (translation != Vector2.zero)
        {
            float length = Mathf.Sqrt(translation.x * translation.x + translation.y * translation.y);
            if (length > 0)
            {
                translation.x /= length;
                translation.y /= length;
            }

            // kecepatan dasar
            translation.x *= speed * Time.deltaTime;
            translation.y *= speed * Time.deltaTime;

            // ======== BOOST SPEED SAAT SHIFT DITEKAN ========
            if (shiftHeld)
            {
                translation.x *= speedBoostMultiplier;
                translation.y *= speedBoostMultiplier;
            }
        }

        // ======== TRANSFORMASI TRANSLASI MANUAL ========
        position.x += translation.x;
        position.y += translation.y;

        // update ke transform unity (boleh, karena hanya menerapkan hasil perhitungan)
        transform.position = new Vector3(position.x, position.y, 0f);

        // ======== SPAWN ASAP SAAT SHIFT ========
        if (shiftHeld)
        {
            smokeTimer += Time.deltaTime;

            if (smokeTimer >= smokeSpawnRate)
            {
                smokeTimer = 0f;
                SpawnSmoke();

                // --- Skor berkurang 1 ---
                if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
                    GameManager.Instance.AddScore(-1);

                // --- Lingkaran mengecil ---
                playerSize -= 0.01f;
                if (playerSize < 0.2f) playerSize = 0.2f; // batas minimal
                transform.localScale = new Vector3(playerSize, playerSize, 1f);
            }
        }
        else
        {
            smokeTimer = 0f; // reset agar tidak stuck setelah ditekan lagi
        }
    }

    void SpawnSmoke()
    {
        GameObject s = Instantiate(
            smokePrefab,
            new Vector3(position.x, position.y, 0f),
            Quaternion.identity
        );

        SmokeParticle sp = s.GetComponent<SmokeParticle>();
        if (sp != null)
            sp.SetStartPosition(position);
    }

    public void EatFood()
    {
        playerSize += 0.01f;
        transform.localScale = new Vector3(playerSize, playerSize, 1);
        if (sr != null)
        {
            playerMat.SetColor("_Color", new Color(0f, 0f, 0.502f, 1f));
            lifetime = timer + 0.5f;
        }
    }

    // ======== DETEKSI MAKAN SEGITIGA ========
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        // Deteksi jika menabrak objek dengan nama mengandung "Triangle"
        if (other.gameObject.name.Contains("Triangle"))
        {
            EatFood();
            Debug.Log("Nyam! Makan segitiga.");
            Destroy(other.gameObject);

            // Tambah skor 1 poin lewat GameManager
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(1);
        }
    }
}
