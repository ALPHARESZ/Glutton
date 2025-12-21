# 🎮 Glutton – 2D Game (Unity + Grafika Komputer)

## 📌 Deskripsi Proyek
**Glutton** adalah sebuah game 2D yang dikembangkan menggunakan **Unity** dengan pendekatan **grafika komputer berbasis transformasi manual**.  
Pemain mengendalikan sebuah **lingkaran** yang bergerak di bidang 2D untuk mengumpulkan objek **segitiga** guna meningkatkan skor.

Proyek ini **tidak menggunakan fungsi transformasi bawaan Unity** seperti `Translate()`, `Rotate()`, dan `Scale()`.  
Seluruh pergerakan, rotasi, dan perubahan ukuran dihitung secara **manual menggunakan konsep matematis grafika komputer**.

## 🧠 Konsep Grafika Komputer yang Digunakan
- **Translasi 2D** (pergerakan player menggunakan WASD)
- **Rotasi 2D** (segitiga berotasi searah jarum jam)
- **Scaling 2D** (player membesar dan mengecil)
- **Transformasi warna (Color Transformation)**
- **Custom Shader**
- **Interpolasi waktu (delta time)**

## 🎯 Mekanisme Permainan
### Player (Lingkaran)
- Skor awal: **20**
- Kontrol:
  - `W A S D` → bergerak
  - `Shift` → berlari (speed meningkat)
- Saat berlari:
  - Skor -1
  - Ukuran player mengecil
  - Muncul efek asap berbentuk persegi

### Objek Segitiga (Food)
- Muncul secara **acak**
- Berotasi **searah jarum jam**
- Jika disentuh player:
  - Skor +1
  - Player membesar
  - Segitiga menghilang
- Jika tidak disentuh player selama 8 detik
  - Segitiga mengecil lalu menghilang

### Kondisi Akhir
- 🏆 **Menang** → Skor mencapai **100**
- 💀 **Kalah** → Skor mencapai **0**

## 📂 Struktur Repository
Repositori ini **hanya berisi isi dari folder `Assets/`** dari proyek Unity.

## ▶ Cara Menjalankan Proyek
1. Buka **Unity Hub**
2. Buat **project Unity 2D baru**
3. Tutup Unity Editor
4. Salin **seluruh isi repositori ini** ke dalam folder `Assets/` project Unity dengan cara:
    - Buka folder `Assets/` project Unity melalui **command promt**
    - Ketik:
    ```bash
    git clone https://github.com/ALPHARESZ/Glutton.git
    ```
5. Buka kembali project di Unity Hub
6. Jalankan scene `glutton.unity`

## ⚠ Catatan Penting
- Proyek ini **sengaja tidak menyertakan folder lain** (`Library`, `Packages`, `ProjectSettings`)
- Seluruh transformasi dilakukan **tanpa fungsi transform bawaan Unity**
- Proyek ini dibuat untuk keperluan **pembelajaran dan tugas Grafika Komputer**

## 🛠 Teknologi yang Digunakan
- Unity 2D
- C#
- Konsep Matematis Grafika Komputer
