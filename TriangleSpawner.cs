using UnityEngine;

public class TriangleSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject trianglePrefab; // Masukkan Prefab Segitiga di sini
    public float spawnInterval = 2f;  // Waktu jeda antar spawn (detik)

    [Header("Spawn Area")]
    // Tentukan batas area spawn (bisa diatur di Inspector)
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnTriangle();
            timer = 0;
        }
    }

    void SpawnTriangle()
    {
        // Tentukan posisi random berdasarkan batas yang sudah diatur
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector2 spawnPos = new Vector2(randomX, randomY);

        // Spawn objek
        Instantiate(trianglePrefab, spawnPos, Quaternion.identity);
    }

    // Fitur tambahan: Menggambar kotak area spawn di Scene view agar mudah dilihat
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 1);
        Gizmos.DrawWireCube(center, size);
    }
}