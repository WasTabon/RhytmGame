# README — Iteration 06: Scoring + Combo + HUD

## Что добавлено

### Скрипты:
- **ScoreManager.cs** — Подсчёт очков, комбо, статистика
- **GameHUD.cs** — UI отображение с анимациями

---

## Система очков

### Базовые очки:

| Результат | Очки |
|-----------|------|
| Perfect | 100 |
| Good | 50 |
| Miss | 0 |

### Множитель комбо:

```
итоговые_очки = базовые_очки × (1 + combo × 0.1)
```

| Combo | Множитель | Perfect даёт |
|-------|-----------|--------------|
| 0 | ×1.0 | 100 |
| 5 | ×1.5 | 150 |
| 10 | ×2.0 | 200 |
| 20 | ×3.0 | 300 |

### Комбо:
- **Perfect / Good** → combo +1
- **Miss** → combo = 0

---

## Настройка на сцене

### 1. Создать UI тексты в Canvas:

**ScoreText:**
- ПКМ на Canvas → UI → Text - TextMeshPro
- Назови **ScoreText**
- Anchor: Top Left
- Position: (150, -50)
- Font Size: 48
- Alignment: Left
- Text: "SCORE: 0"

**ComboText:**
- Создай ещё один TMP текст
- Назови **ComboText**
- Anchor: Top Left
- Position: (150, -110)
- Font Size: 36
- Alignment: Left
- Text: "COMBO: x0"

**RoundText:**
- Создай ещё один TMP текст
- Назови **RoundText**
- Anchor: Bottom Left
- Position: (150, 250) — над DebugPanel
- Font Size: 36
- Alignment: Left
- Text: "ROUND: 1"

### 2. Создать ScoreManager:

1. Создай пустой объект **ScoreManager** в корне сцены
2. Добавь компонент **ScoreManager**
3. LockMechanic подхватится автоматически

### 3. Создать GameHUD:

1. Создай пустой объект **GameHUD** в корне сцены
2. Добавь компонент **GameHUD**
3. Назначь ссылки:
   - **Score Text** → ScoreText
   - **Combo Text** → ComboText
   - **Round Text** → RoundText

---

## Структура сцены после настройки

```
Game Scene
├── Canvas
│   ├── ScoreText (TMP) ← НОВОЕ
│   ├── ComboText (TMP) ← НОВОЕ
│   ├── RoundText (TMP) ← НОВОЕ
│   ├── FeedbackText (TMP)
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
├── ScoreManager ← НОВОЕ
├── GameHUD ← НОВОЕ
└── GameSceneInit
```

---

## Параметры ScoreManager

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Perfect Score | Очки за Perfect | 100 |
| Good Score | Очки за Good | 50 |
| Combo Multiplier Step | Шаг множителя за каждое комбо | 0.1 |

---

## Параметры GameHUD

### Score Animation:
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Score Punch Scale | Масштаб анимации | 1.2 |
| Score Punch Duration | Длительность | 0.2 |

### Combo Animation:
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Combo Punch Scale | Масштаб анимации | 1.3 |
| Combo Punch Duration | Длительность | 0.15 |
| Combo Shake Duration | Длительность shake при сбросе | 0.3 |
| Combo Shake Strength | Сила shake | 10 |

### Combo Colors (меняется автоматически):
| Комбо | Цвет |
|-------|------|
| 0-4 | Белый |
| 5-9 | Зелёный |
| 10-19 | Жёлтый |
| 20-49 | Оранжевый |
| 50+ | Красный |

---

## Как тестировать

1. Настрой сцену как описано выше
2. Нажми **Play**
3. Попадай по сюрикенам:
   - Счёт увеличивается
   - Комбо растёт
   - Текст комбо меняет цвет
   - Анимации punch при попадании
4. Промахнись:
   - Комбо сбрасывается
   - Текст комбо трясётся

---

## API

### ScoreManager:

```csharp
// Получить значения
int score = ScoreManager.Instance.CurrentScore;
int combo = ScoreManager.Instance.CurrentCombo;
int maxCombo = ScoreManager.Instance.MaxCombo;
float accuracy = ScoreManager.Instance.GetAccuracy();
float multiplier = ScoreManager.Instance.GetCurrentMultiplier();

// Статистика
int perfects = ScoreManager.Instance.PerfectCount;
int goods = ScoreManager.Instance.GoodCount;
int misses = ScoreManager.Instance.MissCount;

// Сброс
ScoreManager.Instance.ResetStats();

// События
ScoreManager.Instance.OnScoreChanged += (total, added) => { };
ScoreManager.Instance.OnComboChanged += (combo) => { };
ScoreManager.Instance.OnComboReset += () => { };
```

---

## Ожидаемый результат

- Счёт отображается в левом верхнем углу
- Комбо под счётом (скрыто при combo=0)
- Номер раунда в левом нижнем углу
- Анимации при изменении счёта и комбо
- Цвет комбо меняется с ростом
- Shake при сбросе комбо
