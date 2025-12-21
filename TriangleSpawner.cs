using UnityEngine;

public class TriangleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject trianglePrefab;
    public float spawnInterval = 2f;

    [Header("Area Spawn")]
    public float minX = -8f, maxX = 8f, minY = -4f, maxY = 4f;

    [Header("Rotasi Segitiga (Manual)")]
    public bool enableRotation = true;
    public float rotationSpeedMin = 50f, rotationSpeedMax = 150f;

    private float timer;
    private int randSeed = 12345; // generator acak manual

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTriangle();
            timer = 0f;
        }
    }

    float ManualRandom(float min, float max)
    {
        // Linear congruential generator sederhana
        randSeed = (1103515245 * randSeed + 12345) & 0x7fffffff;
        float normalized = (randSeed % 10000) / 10000f;
        return min + (max - min) * normalized;
    }

    // Aproksimasi sin dan cos manual (Taylor series, cukup akurat utk 0–360)
    float SinApprox(float x)
    {
        return x - (x * x * x) / 6f + (x * x * x * x * x) / 120f;
    }

    float CosApprox(float x)
    {
        return 1 - (x * x) / 2f + (x * x * x * x) / 24f;
    }

    void SpawnTriangle()
    {
        if (trianglePrefab == null) return;

        // Posisi acak manual
        float randomX = ManualRandom(minX, maxX);
        float randomY = ManualRandom(minY, maxY);
        Vector2 spawnPos = new Vector2(randomX, randomY);

        // Sudut acak (radian)
        float randomDeg = ManualRandom(0f, 360f);
        float angleRad = randomDeg * (3.1415926f / 180f);

        // Rotasi manual 2D
        float cosA = CosApprox(angleRad);
        float sinA = SinApprox(angleRad);

        // Rotasi sumbu atas
        Vector2 up = new Vector2(0f, 1f);
        Vector2 rotatedUp = new Vector2(
            up.x * cosA - up.y * sinA,
            up.x * sinA + up.y * cosA
        );

        // Buat segitiga tanpa Quaternion
        GameObject tri = Instantiate(trianglePrefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);

        // Simpan sudut manual agar bisa diputar kemudian
        TriangleManualRotator rot = tri.AddComponent<TriangleManualRotator>();
        rot.currentAngle = randomDeg;
        rot.rotationSpeed = ManualRandom(rotationSpeedMin, rotationSpeedMax);
    }
}

public class TriangleManualRotator : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float currentAngle = 0f;

    void Update()
    {
        currentAngle += rotationSpeed * Time.deltaTime;
        if (currentAngle > 360f) currentAngle -= 360f;

        // Konversi ke radian
        float rad = currentAngle * (3.1415926f / 180f);

        // Rotasi manual sumbu
        float cosA = 1 - (rad * rad) / 2f + (rad * rad * rad * rad) / 24f;
        float sinA = rad - (rad * rad * rad) / 6f + (rad * rad * rad * rad * rad) / 120f;

        // Set orientasi manual ke Unity
        Vector3 rotEuler = new Vector3(0, 0, currentAngle);
        transform.eulerAngles = rotEuler;
    }
}
