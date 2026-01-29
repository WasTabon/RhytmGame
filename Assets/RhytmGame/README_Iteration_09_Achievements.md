# README — Iteration 09: Progression + Achievements

## Что добавлено

### Скрипты:
- **AchievementData.cs** — ScriptableObject с определениями достижений
- **AchievementManager.cs** — Отслеживание и разблокировка достижений
- **AchievementPopup.cs** — Всплывающее уведомление при разблокировке
- **AchievementCard.cs** — Карточка достижения в списке
- **AchievementsUI.cs** — Панель со списком всех достижений
- **StatsManager.cs** — Глобальная статистика игрока
- **AchievementsSetupWindow.cs** — Editor скрипт для настройки

### Prefabs:
- **AchievementCard.prefab** — Создаётся автоматически

---

## Быстрая настройка (Editor скрипт)

1. **RhythmGame → Setup Achievements**
2. Нажми **Create Achievement Data** — создаст 20 достижений
3. Назначь **Target Canvas** (Canvas в MainMenu сцене)
4. Нажми **Create Achievements UI** — панель со списком
5. Нажми **Create Achievement Popup** — всплывашка
6. Нажми **Create Managers** — StatsManager + AchievementManager

### После создания:
- В **AchievementManager** назначь **Achievement Data**
- Подключи AchievementsUI к MainMenu (кнопка в Settings или отдельная)

---

## Достижения (20 штук)

### Beginner (4):
| ID | Название | Условие | Звёзды |
|----|----------|---------|--------|
| first_perfect | First Blood | Первый Perfect | 1 |
| first_level | Baby Steps | Первый уровень пройден | 1 |
| shapes_10 | Getting Started | 10 шейпов пройдено | 1 |
| combo_10 | Combo Starter | Комбо 10 | 1 |

### Skill (6):
| ID | Название | Условие | Звёзды |
|----|----------|---------|--------|
| perfect_5_row | On Fire | 5 Perfect подряд | 2 |
| perfect_10_row | Unstoppable | 10 Perfect подряд | 3 |
| combo_25 | Combo Master | Комбо 25 | 2 |
| combo_50 | Combo Legend | Комбо 50 | 3 |
| no_miss_level | Flawless Run | Уровень без промахов | 3 |
| perfect_accuracy | Perfectionist | 100% Perfect на уровне | 5 |

### Endurance (6):
| ID | Название | Условие | Звёзды |
|----|----------|---------|--------|
| shapes_50 | Shape Shifter | 50 шейпов | 2 |
| shapes_100 | Century | 100 шейпов | 3 |
| shapes_500 | Shape Veteran | 500 шейпов | 5 |
| score_1000 | Point Collector | 1,000 очков | 1 |
| score_10000 | Score Hunter | 10,000 очков | 3 |
| playtime_30 | Dedicated | 30 минут игры | 2 |

### Mastery (4):
| ID | Название | Условие | Звёзды |
|----|----------|---------|--------|
| levels_5 | Rising Star | 5 уровней | 2 |
| levels_15 | Halfway There | 15 уровней | 3 |
| levels_all | Champion | Все уровни | 10 |
| combo_100 | Rhythm God | Комбо 100 | 10 |

---

## Глобальная статистика (StatsManager)

### Сессия:
- sessionPerfects, sessionGoods, sessionMisses
- sessionMaxCombo, sessionScore
- sessionPerfectsInRow

### За всё время (сохраняется):
- totalPerfects, totalGoods, totalMisses
- totalShapesCompleted, totalScore
- bestComboEver, levelsCompleted
- totalPlayTimeMinutes

---

## Анимации

### Achievement Popup:
- Slide сверху + fade in
- Punch scale при появлении
- Автоматический fade out через 3 секунды
- Очередь если несколько достижений сразу
- Цвет зависит от категории

### Achievements Panel:
- Slide справа при открытии/закрытии
- Карточки появляются каскадом
- При скролле — карточки влетают/вылетают с анимацией

### Achievement Card:
- Разблокированные — яркие с цветной полоской категории
- Заблокированные — затемнённые с "???" вместо названия
- При разблокировке в реальном времени — вспышка

---

## Цвета категорий

| Категория | Цвет |
|-----------|------|
| Beginner | Зелёный |
| Skill | Синий |
| Endurance | Оранжевый |
| Mastery | Фиолетовый |

---

## Структура после настройки

### MainMenu сцена:
```
MainMenu Scene
├── Canvas
│   ├── ... (existing panels)
│   ├── AchievementsPanel ← НОВОЕ
│   │   ├── Header
│   │   │   ├── TitleText
│   │   │   ├── ProgressText (0/20)
│   │   │   └── StarsText (0 ★)
│   │   ├── ScrollView
│   │   │   └── Viewport
│   │   │       └── Content
│   │   └── BackButton
│   └── AchievementPopup ← НОВОЕ
├── AchievementsUI ← НОВОЕ
├── StatsManager ← НОВОЕ (DontDestroyOnLoad)
└── AchievementManager ← НОВОЕ (DontDestroyOnLoad)
```

### Game сцена:
StatsManager и AchievementManager переносятся автоматически (DontDestroyOnLoad)

---

## Подключение к MainMenu

### Вариант 1: Кнопка в Settings
Добавь в SettingsPanel кнопку "Achievements" → вызывает `AchievementsUI.Instance.Show()`

### Вариант 2: Отдельная кнопка в MainMenu
Добавь кнопку рядом с Play/Settings

### Пример кода:
```csharp
[SerializeField] private Button achievementsButton;
[SerializeField] private AchievementsUI achievementsUI;

achievementsButton.onClick.AddListener(() => {
    achievementsUI.Show();
});

achievementsUI.OnBackClicked += () => {
    // вернуться к предыдущей панели
};
```

---

## API

### StatsManager:
```csharp
// Получить статистику
StatsManager.Instance.TotalPerfects;
StatsManager.Instance.TotalScore;
StatsManager.Instance.BestComboEver;
StatsManager.Instance.TotalPlayTimeMinutes;

// Событие при изменении
StatsManager.Instance.OnStatChanged += (statName, value) => { };

// Сброс всей статистики
StatsManager.ResetAllStats();
```

### AchievementManager:
```csharp
// Проверить разблокировано ли
AchievementManager.Instance.IsUnlocked("first_perfect");

// Получить списки
AchievementManager.Instance.GetAllAchievements();
AchievementManager.Instance.GetUnlockedAchievements();
AchievementManager.Instance.GetLockedAchievements();
AchievementManager.Instance.GetAchievementsByCategory(AchievementCategory.Skill);

// Счётчики
AchievementManager.Instance.UnlockedCount;
AchievementManager.Instance.TotalStars;

// Событие при разблокировке
AchievementManager.Instance.OnAchievementUnlocked += (achievement) => { };

// Сброс всех достижений
AchievementManager.ResetAllAchievements();
```

### AchievementsUI:
```csharp
AchievementsUI.Instance.Show();
AchievementsUI.Instance.Hide();
AchievementsUI.Instance.OnBackClicked += () => { };
```

### AchievementPopup:
```csharp
// Popup показывается автоматически при разблокировке
// Можно форсировать скрытие:
AchievementPopup.Instance.ForceHide();
```

---

## Важно: Порядок инициализации

Менеджеры должны быть в сцене **до** начала игры:

1. **StatsManager** — создаётся в MainMenu, DontDestroyOnLoad
2. **AchievementManager** — создаётся в MainMenu, DontDestroyOnLoad
3. При загрузке Game сцены они уже существуют
4. `LateSubscribe()` вызывается для подписки на события в Game сцене

### Если менеджеры создаются в Game сцене:
Добавь в GameSceneInit или отдельный скрипт:
```csharp
void Start()
{
    if (StatsManager.Instance != null)
        StatsManager.Instance.LateSubscribe();
    
    if (AchievementManager.Instance != null)
        AchievementManager.Instance.LateSubscribe();
    
    if (AchievementPopup.Instance != null)
        AchievementPopup.Instance.LateSubscribe();
}
```

---

## Тестирование

### Быстрый тест достижений:
1. Запусти игру в Infinite режиме
2. Сделай первый Perfect → "First Blood" должен появиться
3. Набери 10 шейпов → "Getting Started"
4. Достигни комбо 10 → "Combo Starter"

### Проверить сохранение:
1. Выйди из игры
2. Запусти снова
3. Открой Achievements → разблокированные должны сохраниться

---

## Ожидаемый результат

- Popup появляется сверху при разблокировке достижения
- Панель достижений с плавными анимациями
- Статистика сохраняется между сессиями
- Разблокированные достижения сохраняются
- Цветовая кодировка по категориям
