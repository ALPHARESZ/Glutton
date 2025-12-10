using UnityEngine;

public class TriangleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject trianglePrefab;
    [Tooltip("Waktu antar kemunculan segitiga (detik)")]
    public float spawnInterval = 2f;

    [Header("Area Spawn (Koordinat Dunia)")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Rotasi Segitiga")]
    public bool enableRotation = true;      // aktif/nonaktifkan rotasi
    public float rotationSpeedMin = 50f;    // kecepatan putar minimum (derajat/detik)
    public float rotationSpeedMax = 150f;   // kecepatan putar maksimum

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTriangle();
            timer = 0f;
        }
    }

    void SpawnTriangle()
    {
        if (trianglePrefab == null)
        {
            Debug.LogWarning("TriangleSpawner: Prefab segitiga belum diassign!");
            return;
        }

        // Posisi acak
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector2 spawnPos = new Vector2(randomX, randomY);

        // Rotasi awal acak
        float randomRotation = Random.Range(0f, 360f);
        Quaternion spawnRotation = Quaternion.Euler(0f, 0f, randomRotation);

        // Buat segitiga baru
        GameObject triangle = Instantiate(trianglePrefab, spawnPos, spawnRotation);

        // Jika rotasi diaktifkan → tambahkan efek rotasi otomatis
        if (enableRotation)
        {
            float randomSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);

            // tambahkan komponen rotasi langsung di runtime
            triangle.AddComponent<TriangleRuntimeRotator>().rotationSpeed = randomSpeed;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 1);
        Gizmos.DrawWireCube(center, size);
    }
}

// ✅ Kelas internal untuk rotasi otomatis
public class TriangleRuntimeRotator : MonoBehaviour
{
    public float rotationSpeed = 90f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
