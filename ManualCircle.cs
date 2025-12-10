using UnityEngine;

public class ManualCircle : MonoBehaviour
{
    // posisi manual yang kita simpan sendiri (bukan memakai transform langsung)
    private Vector2 position;

    // kecepatan gerak
    public float speed = 2f;

    // Prefab asap
    public GameObject smokePrefab;
    public float smokeSpawnRate = 0.1f; // 1 asap setiap 0.1 detik
    private float smokeTimer = 0f;

    void Start()
    {
        // inisialisasi posisi awal lingkaran (0,0)
        position = new Vector2(0f, 0f);
    }

    void Update()
    {
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

        // normalisasi supaya diagonal tidak lebih cepat
        if (translation != Vector2.zero)
            translation = translation.normalized * speed * Time.deltaTime;

        // transformasi translasi (manual)
        position = position + translation;

        // update ke transform unity (boleh, karena ini hanya assign posisi)
        transform.position = new Vector3(position.x, position.y, 0f);

        // ======== SPAWN ASAP SAAT SHIFT ========
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            smokeTimer += Time.deltaTime;

            if (smokeTimer >= smokeSpawnRate)
            {
                SpawnSmoke();
                smokeTimer = 0f;
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

    // ======== DETEKSI MAKAN SEGITIGA ========
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Deteksi jika menabrak objek dengan nama mengandung "Triangle"
        if (other.gameObject.name.Contains("Triangle"))
        {
            Debug.Log("Nyam! Makan segitiga.");
            Destroy(other.gameObject);

            // Tambah skor 10 poin lewat GameManager
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(1);
        }
    }
}
