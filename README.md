# 🎮 Novastra - Turn Based Battle Test

> Unity Technical Test - Turn Based Battle, Post Processing & Visual Novel

---

# 📖 Overview

Project ini merupakan implementasi **Turn-Based Battle System** yang terinspirasi dari *Honkai: Star Rail*, dipadukan dengan sistem **Post Processing** berbasis Unity URP Volume serta **Visual Novel**.

Fokus utama project ini adalah membangun sistem yang:

* Modular
* Mudah dikembangkan
* Mudah di-maintain
* Data-driven
* Memiliki pemisahan tanggung jawab (Separation of Concerns)

---

# ✨ Features

## ⚔ Battle System

* ✅ Turn Based Battle
* ✅ Action Value (AV) Turn Order seperti Honkai: Star Rail
* ✅ Character Selection
* ✅ 2 Player & 2 Enemy
* ✅ Basic Attack
* ✅ Data-driven Skill System
* ✅ Target Selection
* ✅ Battle Camera
* ✅ Battle Animation
* ✅ Audio & VFX Support

---

## 🎨 Post Processing

* ✅ Bloom
* ✅ Color Grading
* ✅ Vignette
* ✅ Low HP Screen Effect
* ✅ Attack Bloom Pulse
* ✅ Damage Flash
* ✅ Smooth Transition

---

## 🎬 Animation

* ✅ Idle
* ✅ Attack
* ✅ Hit Reaction
* ✅ Death

---

## 📚 Visual Novel

* ✅ Dialogue System
* ✅ Dialogue History
* ✅ Auto Mode
* ✅ Fast Forward

---

# 🏗 Architecture

```text
BattleManager
│
├── BattleFlow
│
├── TurnManager
│
├── BattleActionExecutor
│      │
│      ▼
│   IActionEffect
│      │
│      ├── AttackEffect
│      ├── HealEffect
│      ├── BuffEffect
│      └── ...
│
├── TargetSystem
│
├── BattleUIController
│
└── BattlePresentationContext
       │
       ├── Camera
       ├── Audio
       ├── Post Process
       └── Distortion
```

---

# 🔄 Battle Flow

```text
Battle Start
      │
      ▼
Calculate Turn Order
      │
      ▼
Current Unit Turn
      │
      ├── Player
      │      ▼
      │  Select Action
      │      ▼
      │  Select Target
      │
      └── Enemy AI
             ▼
       Choose Action
      │
      ▼
Execute Action
      │
      ▼
Presentation
(Animation / Camera / Audio / VFX)
      │
      ▼
Damage Calculation
      │
      ▼
Death Check
      │
      ▼
Next Turn
```

---

# 🧩 System Design

## Battle System

Battle dibagi menjadi beberapa sistem yang memiliki tanggung jawab masing-masing.

| System                | Responsibility                        |
| --------------------- | ------------------------------------- |
| BattleManager         | Koordinator utama battle              |
| BattleFlow            | Mengatur alur pertarungan             |
| TurnManager           | Mengatur Action Value dan urutan turn |
| BattleActionExecutor  | Mengeksekusi skill                    |
| BattleActionPresenter | Menangani visual presentation         |
| TargetSystem          | Target selection                      |
| BattleUIController    | UI Battle                             |

---

## Data Driven

Project menggunakan **ScriptableObject** sebagai sumber data utama.

### UnitSO

Berisi:

* Character Data
* Stats
* Skills
* Visual Prefab
* AI

### BattleActionSO

Berisi:

* Skill Data
* Damage
* Target Type
* Audio
* VFX
* Timeline
* Turn Modifier

Pendekatan ini memungkinkan penambahan karakter maupun skill tanpa mengubah kode utama.

---

# 💡 Technical Decisions

## ScriptableObject

Digunakan untuk memisahkan data dan logika sehingga balancing maupun penambahan konten menjadi lebih mudah.

---

## IActionEffect

Skill menggunakan pola **Strategy Pattern** melalui interface `IActionEffect`.

Keuntungan:

* Mudah menambah skill baru.
* Tidak bergantung pada switch yang besar.
* Setiap mekanisme berada pada class masing-masing.

---

## Separation of Gameplay & Presentation

Gameplay dan visual dipisahkan.

Gameplay menangani:

* Damage
* HP
* Turn
* Battle Logic

Presentation menangani:

* Animation
* Camera
* Audio
* VFX
* Post Processing

Dengan pendekatan ini, perubahan visual tidak memengaruhi gameplay.

---

## Action Value System

Turn menggunakan **Action Value** sehingga mendukung fitur seperti:

* Speed Based Turn
* Advance Forward
* Delay
* Interrupt

tanpa mengubah struktur utama Turn Manager.

---

## Fungus Integration

Fungus digunakan sebagai fondasi sistem Visual Novel untuk mempercepat pengembangan dialogue dan branching event.

Framework ini kemudian dikustomisasi dengan menambahkan beberapa fitur seperti Auto Mode, Fast Forward, Dialogue History, serta integrasi dengan Battle System agar event dialogue dapat memicu transisi gameplay, animasi, maupun efek visual secara langsung.

Dengan pendekatan ini, sistem Visual Novel tetap memanfaatkan kestabilan Fungus namun tetap fleksibel untuk dikembangkan sesuai kebutuhan project.

---

# 🛠 Tech Stack

* Unity 6
* C#
* Universal Render Pipeline (URP)
* DOTween
* ScriptableObject Architecture

---

# 📷 Screenshots

> Tambahkan screenshot Battle, Character Select, dan Visual Novel di sini.

---

# 🎥 Demo

> Tambahkan link video demo (1–3 menit).

---

# 👨‍💻 Author

**Dzaki Adani**

Unity Game Developer
