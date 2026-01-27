# README — Iteration 05: Target Shape + Lock Mechanic

## Что добавлено

### Скрипты:
- **LockMechanic.cs** — Логика тапа/клика, сравнение углов и цветов, оценка Perfect/Good/Miss
- **RoundManager.cs** — Генерация раундов, управление шейпами
- **FeedbackManager.cs** — Визуальный feedback (текст Perfect/Good/Miss)

---

## Настройка на сцене

### 1. Создать TargetShape:

1. В **GameArea** создай новый пустой объект: ПКМ → Create Empty → назови **TargetShape**
2. Добавь компоненты:
   - **SpriteRenderer** (Sorting Order = 5)
   - **ShapeController**
3. В **ShapeController**:
   - Назначь **Shape Data** (тот же что и у PlayerShape)
   - Marker Sorting Order = 6 (чтобы маркеры были позади маркеров игрока)

### 2. Создать LockMechanic:

1. Создай пустой объект **LockMechanic** в корне сцены
2. Добавь компонент **LockMechanic**
3. Назначь ссылки:
   - **Player Shape** → PlayerShape (ShapeController)
   - **Target Shape** → TargetShape (ShapeController)
   - **Rotation Controller** → PlayerShape (RotationController)

### 3. Создать RoundManager:

1. Создай пустой объект **RoundManager** в корне сцены
2. Добавь компонент **RoundManager**
3. Назначь ссылки:
   - **Player Shape** → PlayerShape (ShapeController)
   - **Target Shape** → TargetShape (ShapeController)
   - **Rotation Controller** → PlayerShape (RotationController)
   - **Lock Mechanic** → LockMechanic
   - **Shape Data** → твой ShapeData asset

### 4. Создать FeedbackManager + UI:

1. В **Canvas** создай **TextMeshPro - Text**: назови **FeedbackText**
   - Anchor: Center
   - Position Y: 400 (над сюрикеном)
   - Font Size: 72
   - Alignment: Center
   - Color: White
2. Создай пустой объект **FeedbackManager** в корне сцены
3. Добавь компонент **FeedbackManager**
4. Назначь **Feedback Text** → FeedbackText (TMP)

### 5. Убрать тестовый скрипт:

Если у тебя есть TestScript — удали его, теперь RoundManager управляет шейпами.

---

## Структура сцены после настройки

```
Game Scene
├── Canvas
│   ├── DebugPanel (Low/Mid/High bars)
│   ├── FeedbackText (TMP) ← НОВОЕ
│   └── FadePanel
├── MusicManager
├── GameArea
│   ├── TargetShape ← НОВОЕ
│   │   ├── SpriteRenderer (Sorting Order = 5)
│   │   └── ShapeController
│   └── PlayerShape
│       ├── SpriteRenderer (Sorting Order = 10)
│       ├── ShapeController
│       └── RotationController
├── LockMechanic ← НОВОЕ
├── RoundManager ← НОВОЕ
├── FeedbackManager ← НОВОЕ
└── GameSceneInit
```

---

## Параметры

### LockMechanic:

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Perfect Angle Tolerance | Допуск для Perfect (градусов) | 5 |
| Good Angle Tolerance | Допуск для Good (градусов) | 15 |
| Require Color Match | Нужно ли совпадение цветов | true |

### RoundManager:

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Target Alpha | Прозрачность цели | 0.3 |
| Target Sorting Order | Слой отрисовки цели | 5 |
| Delay After Lock | Задержка перед новым раундом | 0.5 |

### FeedbackManager:

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Perfect Color | Цвет текста Perfect | Зелёный |
| Good Color | Цвет текста Good | Жёлтый |
| Miss Color | Цвет текста Miss | Красный |
| Show Duration | Длительность показа | 0.8 сек |

---

## Как тестировать

1. Настрой сцену как описано выше
2. Убедись что музыка играет
3. Нажми **Play**
4. Ты увидишь:
   - Полупрозрачный сюрикен-цель на фоне (не вращается)
   - Вращающийся сюрикен игрока
   - Цветные маркеры на обоих
5. Кликни мышкой или тапни когда сюрикены совпадут
6. Появится текст: PERFECT / GOOD / MISS
7. После небольшой паузы — новый раунд

### Управление:
- **Мышь** — клик левой кнопкой (для теста в Editor)
- **Touch** — тап (для мобилок)

---

## Логика оценки

| Условие | Результат |
|---------|-----------|
| Угол ≤ 5° И цвета совпали | **PERFECT** |
| Угол ≤ 15° И цвета совпали | **PERFECT** |
| Угол ≤ 15° И цвета НЕ совпали | **GOOD** |
| Угол > 15° | **MISS** |

При **Miss** — текущий раунд перезапускается (тот же шейп, те же цвета).
При **Perfect/Good** — новый раунд (новый шейп, новые цвета).

---

## События (для будущих итераций)

```csharp
// LockMechanic
LockMechanic.Instance.OnLock += (LockResult result) => { };
LockMechanic.Instance.OnMiss += () => { };

// RoundManager
RoundManager.Instance.OnRoundStart += (int roundNumber) => { };
RoundManager.Instance.OnRoundEnd += (LockResult result) => { };
```

---

## Ожидаемый результат

- Полупрозрачная цель на фоне с цветными маркерами
- Вращающийся сюрикен игрока с такими же цветами
- Клик/тап оценивается: Perfect/Good/Miss
- Визуальный feedback текстом
- После успеха — новый раунд
- После промаха — повтор текущего раунда
