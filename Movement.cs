using UnityEngine;

public class ManualCircleMovement : MonoBehaviour
{
    // posisi manual yang kita simpan sendiri (bukan memakai transform langsung)
    private Vector2 position;

    // kecepatan gerak
    public float speed = 5f;

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
        // P' = P + T
        position = position + translation;

        // update ke transform unity (boleh, karena ini hanya assign posisi)
        transform.position = new Vector3(position.x, position.y, 0f);
    }
}