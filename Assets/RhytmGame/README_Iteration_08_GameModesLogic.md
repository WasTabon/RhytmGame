# README — Iteration 08: Game Modes Logic

## Что добавлено

### Скрипты:
- **LevelData.cs** — ScriptableObject со списком уровней
- **GameModeController.cs** — Управление логикой режимов
- **TimeAttackTimer.cs** — Таймер для Time Attack + UI
- **GameOverUI.cs** — Панель результатов (Game Over / Level Complete)
- **LevelProgressUI.cs** — Прогресс в режиме Levels (3/5)
- **GameManager.cs** — Обновлён (добавлен SelectedLevelIndex, TimeAttackDuration)

---

## Логика режимов

| Режим | Логика | Конец |
|-------|--------|-------|
| **Infinite** | Бесконечные шейпы, счёт растёт | Ручной выход в меню |
| **Time Attack** | Таймер 60/90/120 сек | По окончании таймера → Game Over |
| **Levels** | N шейпов пройти | Прошёл все → Level Complete |

---

## Создание LevelData

1. **ПКМ в Project → Create → RhythmGame → Level Data**
2. Назови например "GameLevels"
3. В инспекторе добавь уровни:

```
Levels:
  [0] Level Name: "Level 1", Shapes To Complete: 3
  [1] Level Name: "Level 2", Shapes To Complete: 5
  [2] Level Name: "Level 3", Shapes To Complete: 7
  [3] Level Name: "Level 4", Shapes To Complete: 10
  ...
```

---

## Настройка на сцене

### 1. Создать GameModeController:

1. Создай пустой объект **GameModeController** в корне
2. Добавь компонент **GameModeController**
3. Назначь **Level Data** → твой LevelData asset

### 2. Создать TimeAttackTimer + UI:

1. В **Canvas** создай TMP текст **TimerText**:
   - Anchor: Top Right
   - Position: (-150, -50)
   - Font Size: 48
   - Alignment: Right
   - Text: "1:00"
2. Создай пустой объект **TimeAttackTimer** в корне
3. Добавь компонент **TimeAttackTimer**
4. Назначь **Timer Text** → TimerText

### 3. Создать LevelProgressUI + UI:

1. В **Canvas** создай TMP текст **ProgressText**:
   - Anchor: Top Right
   - Position: (-150, -50) — там же где таймер, они не показываются одновременно
   - Font Size: 48
   - Alignment: Right
   - Text: "0 / 5"
2. Создай пустой объект **LevelProgressUI** в корне
3. Добавь компонент **LevelProgressUI**
4. Назначь **Progress Text** → ProgressText

### 4. Создать GameOverPanel в Canvas:

1. Создай **Panel** в Canvas, назови **GameOverPanel**:
   - Anchor: Stretch
   - Color: (0, 0, 0, 0.8) — полупрозрачный чёрный
2. Добавь **CanvasGroup** компонент на панель
3. Внутри панели создай:

**TitleText (TMP):**
- Position Y: 300
- Font Size: 72
- Alignment: Center
- Text: "GAME OVER"

**ScoreText (TMP):**
- Position Y: 100
- Font Size: 48
- Text: "Score: 0"

**MaxComboText (TMP):**
- Position Y: 30
- Font Size: 36
- Text: "Max Combo: 0"

**AccuracyText (TMP):**
- Position Y: -30
- Font Size: 36
- Text: "Accuracy: 0%"

**ShapesText (TMP):**
- Position Y: -90
- Font Size: 36
- Text: "Shapes: 0"

**RetryButton (Button):**
- Position Y: -200
- Text: "RETRY"

**MenuButton (Button):**
- Position Y: -280
- Text: "MENU"

4. Создай пустой объект **GameOverUI** в корне
5. Добавь компонент **GameOverUI**
6. Назначь все ссылки (Panel, CanvasGroup, тексты, кнопки)

---

## Структура сцены после настройки

```
Game Scene
├── Main Camera + CameraShake
├── Canvas
│   ├── ScoreText
│   ├── ComboText
│   ├── RoundText
│   ├── TimerText ← НОВОЕ
│   ├── ProgressText ← НОВОЕ
│   ├── FeedbackText
│   ├── DebugPanel
│   ├── GameOverPanel ← НОВОЕ
│   │   ├── TitleText
│   │   ├── ScoreText
│   │   ├── MaxComboText
│   │   ├── AccuracyText
│   │   ├── ShapesText
│   │   ├── RetryButton
│   │   └── MenuButton
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
├── GameFeel
├── HapticFeedback
├── GameModeController ← НОВОЕ
├── TimeAttackTimer ← НОВОЕ
├── LevelProgressUI ← НОВОЕ
└── GameOverUI ← НОВОЕ
```

---

## Параметры GameModeController

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Level Data | ScriptableObject с уровнями | — |
| Time Attack Duration | Длительность Time Attack (сек) | 60 |

---

## Параметры TimeAttackTimer

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Warning Time | Когда начать мигать (сек) | 10 |
| Normal Color | Цвет таймера | Белый |
| Warning Color | Цвет при предупреждении | Красный |
| Pulse On Warning | Пульсировать при предупреждении | true |

---

## Как тестировать

### Infinite Mode:
1. В MainMenu выбери Infinite
2. Игра идёт бесконечно
3. Таймер скрыт
4. Выход только через меню (пока нет паузы)

### Time Attack Mode:
1. В MainMenu выбери Time Attack
2. Таймер запускается (60 сек по умолчанию)
3. При <10 сек — таймер краснеет и пульсирует
4. По окончании — Game Over панель

### Levels Mode:
1. В MainMenu выбери Levels (пока просто выбор режима)
2. Показывается прогресс "0 / N"
3. При успешных попаданиях счётчик растёт
4. Когда достиг N — Level Complete панель
5. Прогресс сохраняется (следующий уровень разблокируется)

---

## Сохранение прогресса

Разблокированные уровни сохраняются в PlayerPrefs:
- Ключ: `"UnlockedLevel"`
- Значение: индекс последнего разблокированного уровня

```csharp
// Получить количество доступных уровней
int unlocked = GameModeController.GetUnlockedLevelCount();
```

---

## API

### GameModeController:
```csharp
GameModeController.Instance.CurrentMode;      // текущий режим
GameModeController.Instance.ShapesCompleted;  // сколько прошёл
GameModeController.Instance.ShapesRequired;   // сколько нужно
GameModeController.Instance.IsGameActive;     // игра активна?
GameModeController.Instance.RestartGame();    // рестарт

// События
GameModeController.Instance.OnGameStart += () => { };
GameModeController.Instance.OnGameOver += () => { };
GameModeController.Instance.OnLevelComplete += () => { };
GameModeController.Instance.OnShapeProgress += (completed, required) => { };
```

### TimeAttackTimer:
```csharp
TimeAttackTimer.Instance.StartTimer(90f);  // запустить на 90 сек
TimeAttackTimer.Instance.StopTimer();
TimeAttackTimer.Instance.PauseTimer();
TimeAttackTimer.Instance.ResumeTimer();
TimeAttackTimer.Instance.AddTime(10f);     // добавить время
TimeAttackTimer.Instance.TimeRemaining;    // осталось секунд
```

### GameManager (обновлён):
```csharp
GameManager.Instance.SetSelectedLevel(2);       // выбрать уровень
GameManager.Instance.SetTimeAttackDuration(90f); // установить время
```

---

## Ожидаемый результат

- Infinite — бесконечная игра без ограничений
- Time Attack — таймер с предупреждением, Game Over по окончании
- Levels — прогресс-бар, Level Complete при достижении цели
- Game Over / Level Complete панель с результатами
- Кнопки Retry и Menu работают
- Прогресс уровней сохраняется
