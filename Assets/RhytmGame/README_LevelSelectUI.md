# README — Level Select UI

## Что добавлено

### Скрипты:
- **LevelCard.cs** — Карточка одного уровня с анимациями
- **LevelSelectUI.cs** — Контроллер списка уровней со скроллом
- **LevelSelectSetupWindow.cs** — Editor скрипт для автоматической настройки
- **MainMenuController.cs** — Обновлён (интеграция с Level Select)
- **GameModeController.cs** — Обновлён (сохранение результатов)

### Prefabs:
- **LevelCard.prefab** — Создаётся автоматически

---

## Быстрая настройка (Editor скрипт)

1. **RhythmGame → Setup Level Select UI**
2. Назначь **Target Canvas** (Canvas в MainMenu сцене)
3. Назначь **Level Data** (твой GameLevels asset)
4. Нажми **Create Level Select UI**

Скрипт создаст:
- LevelSelectPanel с заголовком и ScrollView
- Кнопку Back
- Prefab карточки в Assets/RhythmGame/Prefabs/
- Объект LevelSelectUI с привязанными ссылками
- Автоматически обновит MainMenuController

---

## Что показывает карточка уровня

| Элемент | Описание |
|---------|----------|
| Номер | Синий квадрат с номером уровня |
| Название | "First Steps", "Warm Up", etc. |
| Shapes | "5 shapes" — сколько надо пройти |
| Best Score | Лучший счёт (или "---" если не играл) |
| Best Combo | Лучшее комбо (или "---") |
| Lock Icon | Серый — уровень заблокирован |
| Completed Icon | Зелёный — уровень пройден |

---

## Анимации

### Панель:
- Slide справа при открытии
- Slide вправо при закрытии

### Карточки:
- При появлении на экране — slide справа + fade in
- При уходе с экрана — slide влево + fade out
- При первом открытии — каскадное появление
- При клике — punch scale

---

## Навигация

```
Main Menu → Play → Mode Select
                      ↓
               Levels Button
                      ↓
             Level Select Panel
             ┌─────────────────┐
             │  SELECT LEVEL   │
             ├─────────────────┤
             │ ┌─────────────┐ │
             │ │ 1 First St. │ │
             │ └─────────────┘ │
             │ ┌─────────────┐ │
             │ │ 2 Warm Up   │ │
             │ └─────────────┘ │
             │      ...        │
             ├─────────────────┤
             │     [BACK]      │
             └─────────────────┘
                      ↓
               Back Button
                      ↓
              Mode Select Panel
```

---

## Сохранение прогресса

### Разблокировка:
- `PlayerPrefs["UnlockedLevel"]` — индекс последнего разблокированного

### Результаты каждого уровня:
- `PlayerPrefs["Level_{index}_Completed"]` — 1 если пройден
- `PlayerPrefs["Level_{index}_BestScore"]` — лучший счёт
- `PlayerPrefs["Level_{index}_BestCombo"]` — лучшее комбо

### API:
```csharp
// Сохранить результат (вызывается автоматически)
LevelSelectUI.SaveLevelResult(levelIndex, score, maxCombo);
```

---

## Ручная настройка (без Editor скрипта)

### 1. Создать LevelSelectPanel в Canvas:

1. Создай Panel, назови **LevelSelectPanel**
2. Anchor: Stretch
3. Color: тёмный полупрозрачный
4. Добавь **CanvasGroup**

### 2. Добавить Header:

1. Внутри панели создай пустой объект **Header**
2. Anchor: Top, Height: 120
3. Добавь TextMeshPro с текстом "SELECT LEVEL"

### 3. Создать ScrollView:

1. UI → Scroll View
2. Anchor: Stretch, отступы сверху 120, снизу 100
3. Удали Scrollbar Horizontal
4. Scrollbar Vertical можно оставить или удалить

### 4. Создать Card Prefab:

Используй **RhythmGame → Setup Level Select UI → Create Card Prefab Only**

Или создай вручную с компонентами:
- RectTransform
- CanvasGroup
- Image (фон)
- Button
- LevelCard

И дочерними текстами: NumberText, NameText, ShapesText, BestScoreText, BestComboText, LockIcon, CompletedIcon

### 5. Создать LevelSelectUI:

1. Create Empty → **LevelSelectUI**
2. Add Component → **LevelSelectUI**
3. Назначь все ссылки:
   - Panel Rect → LevelSelectPanel
   - Panel Canvas Group → CanvasGroup на панели
   - Scroll Rect → ScrollView
   - Content → Content внутри ScrollView
   - Viewport → Viewport внутри ScrollView
   - Card Prefab → LevelCard prefab
   - Level Data → GameLevels asset
   - Back Button → кнопка Back

### 6. Обновить MainMenuController:

В MainMenuController назначь **Level Select UI** → LevelSelectUI объект

---

## Структура после настройки

```
MainMenu Scene
├── Canvas
│   ├── MainMenuPanel
│   ├── ModeSelectPanel
│   ├── SettingsPanel
│   ├── LevelSelectPanel ← НОВОЕ
│   │   ├── Header
│   │   │   └── TitleText
│   │   ├── ScrollView
│   │   │   └── Viewport
│   │   │       └── Content
│   │   └── BackButton
│   └── FadePanel
├── MainMenuController (+ ссылка на LevelSelectUI)
├── LevelSelectUI ← НОВОЕ
└── SceneLoader
```

---

## API

### LevelSelectUI:
```csharp
LevelSelectUI.Instance.Show();   // Показать панель
LevelSelectUI.Instance.Hide();   // Скрыть панель

// Событие при нажатии Back
LevelSelectUI.Instance.OnBackClicked += () => { };

// Сохранить результат уровня
LevelSelectUI.SaveLevelResult(levelIndex, score, maxCombo);
```

### LevelCard:
```csharp
// Событие при клике на карточку
card.OnCardClicked += (int levelIndex) => { };
```

---

## Параметры LevelSelectUI

| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Card Height | Высота карточки | 180 |
| Card Spacing | Отступ между карточками | 20 |
| Top/Bottom Padding | Отступы контента | 20 |
| Side Padding | Боковые отступы | 40 |
| Panel Slide Duration | Анимация панели | 0.4 |
| Card Appear Delay | Задержка между карточками | 0.05 |

---

## Ожидаемый результат

- Вертикальный список уровней с плавным скроллом
- Карточки появляются/исчезают с анимацией при скролле
- Заблокированные уровни затемнены + иконка замка
- Пройденные уровни с зелёной галочкой
- Показывается лучший счёт и комбо
- Клик на уровень → запуск игры
