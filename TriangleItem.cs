using UnityEngine;

public class TriangleItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Baris ini akan memberitahu kita SIAPA yang menabrak segitiga
        Debug.Log("Segitiga ditabrak oleh: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("KENA! Menghancurkan segitiga...");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Yang nabrak bukan Player, tapi tag-nya adalah: " + other.tag);
        }
    }
}