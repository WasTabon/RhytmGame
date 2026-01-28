# README — Iteration 07: Visual Effects + Game Feel

## Что добавлено

### Скрипты:
- **GameFeel.cs** — Центральный менеджер эффектов: punch, flash, particles
- **CameraShake.cs** — Тряска камеры на басах и при Miss
- **HapticFeedback.cs** — Вибрация на iOS/Android

### Плагины:
- **Plugins/iOS/HapticFeedback.mm** — Нативный iOS код для haptic feedback

---

## Эффекты

| Событие | Эффекты |
|---------|---------|
| **Perfect** | Punch scale шейпа (1.3x), белая вспышка, зелёные particles, лёгкая вибрация |
| **Good** | Punch scale (1.15x), жёлтая вспышка, жёлтые particles, лёгкая вибрация |
| **Miss** | Camera shake, красная вспышка, красные particles, средняя вибрация |
| **Bass** | Лёгкий camera shake в такт музыке |
| **Новый раунд** | Плавное появление шейпов (scale from 0) |

---

## Настройка на сцене

### 1. Создать GameFeel:

1. Создай пустой объект **GameFeel** в корне сцены
2. Добавь компонент **GameFeel**
3. Назначь ссылки:
   - **Player Shape** → PlayerShape (ShapeController)
   - **Target Shape** → TargetShape (ShapeController)
4. Flash Overlay и Particles создадутся автоматически

### 2. Добавить CameraShake на камеру:

1. Выбери **Main Camera**
2. Добавь компонент **CameraShake**
3. Настройки по умолчанию ок, можно подкрутить

### 3. Создать HapticFeedback:

1. Создай пустой объект **HapticFeedback** в корне сцены
2. Добавь компонент **HapticFeedback**
3. Вибрация работает только на реальном устройстве

### 4. iOS Plugin (автоматически):

Файл `Plugins/iOS/HapticFeedback.mm` автоматически включится в iOS билд.

---

## Структура сцены после настройки

```
Game Scene
├── Main Camera
│   └── CameraShake ← НОВОЕ
├── Canvas
│   ├── ScoreText
│   ├── ComboText
│   ├── RoundText
│   ├── FeedbackText
│   ├── DebugPanel
│   └── FadePanel
├── MusicManager
├── GameArea
│   ├── TargetShape
│   └── PlayerShape
├── ShapeScaler
├── LockMechanic
├── RoundManager
├── FeedbackManager
├── ScoreManager
├── GameHUD
├── GameFeel ← НОВОЕ
│   ├── FlashOverlay (создаётся автоматически)
│   ├── PerfectParticles (создаётся автоматически)
│   ├── GoodParticles (создаётся автоматически)
│   └── MissParticles (создаётся автоматически)
└── HapticFeedback ← НОВОЕ
```

---

## Параметры GameFeel

### Punch Effect:
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Perfect Punch Scale | Масштаб при Perfect | 1.3 |
| Good Punch Scale | Масштаб при Good | 1.15 |
| Punch Duration | Длительность | 0.2 |

### Flash Effect:
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Enable Flash | Вкл/выкл | true |
| Flash Duration | Длительность | 0.15 |
| Flash Alpha | Прозрачность | 0.3 |
| Perfect Flash Color | Цвет | Белый |
| Good Flash Color | Цвет | Жёлтый |
| Miss Flash Color | Цвет | Красный |

### Round Transition:
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Appear Duration | Время появления шейпов | 0.3 |
| Appear Ease | Тип анимации | OutBack |

---

## Параметры CameraShake

### Miss Shake:
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Miss Shake Duration | Длительность | 0.3 |
| Miss Shake Strength | Сила | 0.3 |
| Miss Shake Vibrato | Частота | 20 |

### Bass Shake:
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Enable Bass Shake | Вкл/выкл | true |
| Bass Shake Strength | Сила | 0.05 |
| Bass Threshold | Порог срабатывания | 0.7 |

---

## Параметры HapticFeedback

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Enable Haptics | Вкл/выкл вибрацию | true |

### Типы вибрации (iOS):
- **Light** — лёгкая (Perfect, Good)
- **Medium** — средняя (Miss)
- **Heavy** — сильная (доступна через API)

### Android:
Используется стандартный `Handheld.Vibrate()` — один тип вибрации.

---

## Как тестировать

1. Настрой сцену как описано
2. Нажми **Play**
3. Проверь эффекты:
   - **Perfect** — шейп увеличивается + белая вспышка + зелёные частицы
   - **Good** — меньше увеличение + жёлтая вспышка + жёлтые частицы
   - **Miss** — камера трясётся + красная вспышка + красные частицы
4. Включи музыку с басами — камера должна слегка трястись в такт
5. Новый раунд — шейпы плавно появляются

### Тест вибрации:
Только на реальном устройстве (iOS/Android). В Editor не работает.

---

## Настройка game feel

### Если эффекты слишком сильные:
- Уменьши **Punch Scale** (1.1-1.2)
- Уменьши **Flash Alpha** (0.1-0.2)
- Уменьши **Miss Shake Strength** (0.1-0.2)

### Если эффекты слабые:
- Увеличь **Punch Scale** (1.4-1.5)
- Увеличь **Flash Alpha** (0.4-0.5)
- Увеличь **Miss Shake Strength** (0.4-0.5)

### Если bass shake раздражает:
- Выключи **Enable Bass Shake**
- Или увеличь **Bass Threshold** (0.8-0.9)

---

## API

### GameFeel:
```csharp
GameFeel.Instance.SetReferences(playerShape, targetShape);
GameFeel.Instance.RecaptureScales();
```

### CameraShake:
```csharp
CameraShake.Instance.ShakeMiss();
CameraShake.Instance.ShakeCustom(duration, strength);
```

### HapticFeedback:
```csharp
HapticFeedback.Instance.VibrateLight();
HapticFeedback.Instance.VibrateMedium();
HapticFeedback.Instance.VibrateHeavy();
HapticFeedback.Instance.SetEnabled(false);
```

---

## Ожидаемый результат

- Perfect/Good/Miss имеют чёткий визуальный feedback
- Камера реагирует на музыку (bass shake)
- Переходы между раундами плавные
- На мобилках работает вибрация
- Игра ощущается "живой" и отзывчивой
