# Starlock: Rhythm Core - Итерация 1

## Что добавлено

### Core Systems (Scripts/Core/)
- **GameManager.cs** - Синглтон для управления состоянием игры и режимами
- **AudioManager.cs** - Управление музыкой и звуковыми эффектами с сохранением громкости
- **VibrationManager.cs** - Haptic feedback для iOS (Light/Medium/Heavy/Success/Error)
- **SceneTransition.cs** - Плавные переходы между сценами с fade эффектом
- **Bootstrap.cs** - Инициализация всех менеджеров при старте

### UI System (Scripts/UI/)
- **UIAnimator.cs** - Статический класс с DOTween расширениями для UI анимаций
- **ScreenBase.cs** - Базовый класс для всех экранов (show/hide с анимациями)
- **MainMenuScreen.cs** - Главный экран меню с кнопками
- **SettingsScreen.cs** - Экран настроек (громкость, вибрация)
- **LevelSelectScreen.cs** - Экран выбора уровней (заглушка, будет реализован позже)
- **AchievementsScreen.cs** - Экран достижений (заглушка, будет реализован позже)
- **MainMenuUI.cs** - Контроллер переключения между экранами
- **AnimatedButton.cs** - Компонент для кнопок с анимацией нажатия
- **SafeArea.cs** - Адаптация UI под notch на iOS

### Plugins
- **Plugins/iOS/HapticFeedback.mm** - Нативный iOS плагин для вибрации

---

## Инструкция по настройке

### 1. Подготовка проекта

1. Создай новый Unity проект (2D Built-in)
2. Установи **DOTween** через Package Manager или Asset Store (бесплатная версия)
3. Установи **TextMeshPro** (обычно уже установлен, если нет - Window > TextMeshPro > Import TMP Essential Resources)

### 2. Структура папок

Создай следующую структуру папок в Assets:
```
Assets/
├── Scripts/
│   ├── Core/
│   └── UI/
├── Scenes/
├── Prefabs/
│   └── UI/
├── Plugins/
│   └── iOS/
├── Audio/
│   ├── Music/
│   └── SFX/
└── Resources/
    └── Audio/
        └── SFX/
```

### 3. Копирование файлов

Скопируй все файлы из архива в соответствующие папки.

### 4. Создание сцены MainMenu

1. Создай новую сцену: `Assets/Scenes/MainMenu.unity`
2. Удали Main Camera по умолчанию

### 5. Создание Canvas

1. Создай Canvas (GameObject > UI > Canvas)
2. Настрой Canvas:
   - Render Mode: **Screen Space - Overlay**
   - Canvas Scaler:
     - UI Scale Mode: **Scale With Screen Size**
     - Reference Resolution: **1080 x 1920**
     - Screen Match Mode: **Match Width Or Height**
     - Match: **0.5**
3. Добавь компонент **SafeArea** на дочерний объект Canvas (создай пустой GameObject "SafeArea" внутри Canvas и добавь компонент)

### 6. Создание Main Menu Screen

Внутри SafeArea создай структуру:

```
SafeArea
└── MainMenuScreen (добавь компонент MainMenuScreen + CanvasGroup)
    ├── TitleContainer (Empty GameObject)
    │   └── TitleText (TextMeshPro - "STARLOCK")
    │       - Font Size: 72
    │       - Alignment: Center
    │       - Anchor: Top Center
    │       - Pos Y: -200
    └── ButtonsContainer
        ├── PlayButton (Button + AnimatedButton)
        │   └── Text (TextMeshPro - "PLAY")
        ├── InfiniteButton (Button + AnimatedButton)
        │   └── Text (TextMeshPro - "INFINITE")
        ├── TimeAttackButton (Button + AnimatedButton)
        │   └── Text (TextMeshPro - "TIME ATTACK")
        ├── AchievementsButton (Button + AnimatedButton)
        │   └── Text (TextMeshPro - "ACHIEVEMENTS")
        └── SettingsButton (Button + AnimatedButton)
            └── Text (TextMeshPro - "SETTINGS")
```

**Настройки кнопок:**
- Размер: 600 x 100
- Расстояние между кнопками: 130 по вертикали
- Anchor: Middle Center
- Используй Vertical Layout Group на ButtonsContainer (Spacing: 30)

**Привязка в инспекторе MainMenuScreen:**
- Title Container → TitleContainer
- Title Text → TitleText
- Play Button → PlayButton
- Infinite Button → InfiniteButton
- и т.д. для всех кнопок
- Play Button Rect, Infinite Button Rect и т.д. → соответствующие RectTransform кнопок

### 7. Создание Settings Screen

```
SafeArea
└── SettingsScreen (добавь компонент SettingsScreen + CanvasGroup)
    ├── Header
    │   ├── BackButton (Button + AnimatedButton)
    │   │   └── Text (TextMeshPro - "←" или "BACK")
    │   └── TitleText (TextMeshPro - "SETTINGS")
    └── Content
        ├── MusicRow
        │   ├── Label (TextMeshPro - "Music")
        │   ├── MusicSlider (Slider)
        │   └── MusicValue (TextMeshPro - "100%")
        ├── SFXRow
        │   ├── Label (TextMeshPro - "Sound")
        │   ├── SFXSlider (Slider)
        │   └── SFXValue (TextMeshPro - "100%")
        └── VibrationRow
            ├── Label (TextMeshPro - "Vibration")
            └── VibrationToggle (Toggle)
                └── Checkmark (Image)
```

**Привязка в инспекторе SettingsScreen:**
- Header Rect → Header
- Back Button → BackButton
- Content Rect → Content
- Music Slider → MusicSlider
- Music Value Text → MusicValue
- SFX Slider → SFXSlider
- SFX Value Text → SFXValue
- Vibration Toggle → VibrationToggle
- Vibration Checkmark → Checkmark внутри Toggle

### 8. Создание Level Select Screen и Achievements Screen

Создай аналогично SettingsScreen (Header + Content), но в Content просто добавь текст "Coming Soon" по центру.

### 9. Создание Prefabs менеджеров

1. Создай пустой GameObject "GameManager" и добавь компонент GameManager
2. Сохрани как Prefab в `Assets/Prefabs/`
3. Повтори для:
   - **AudioManager** (добавь 2 AudioSource: один для музыки, один для SFX)
   - **VibrationManager**
   - **SceneTransition** (создай Canvas внутри с CanvasGroup и Image для fade)

**Настройка SceneTransition Prefab:**
```
SceneTransitionPrefab
└── FadeCanvas (Canvas, CanvasGroup)
    - Render Mode: Screen Space - Overlay
    - Sort Order: 999
    └── FadeImage (Image)
        - Color: Black
        - Stretch to fill
```
Привяжи FadeCanvas (CanvasGroup) и FadeImage в компоненте SceneTransition.

### 10. Создание Bootstrap объекта

1. Создай пустой GameObject "Bootstrap" в сцене MainMenu
2. Добавь компонент Bootstrap
3. Привяжи все prefab'ы менеджеров

### 11. Создание MainMenuUI объекта

1. Создай пустой GameObject "MainMenuUI" в сцене
2. Добавь компонент MainMenuUI
3. Привяжи все экраны (MainMenuScreen, SettingsScreen, LevelSelectScreen, AchievementsScreen)

### 12. Создание сцены Game (заглушка)

1. Создай сцену `Assets/Scenes/Game.unity`
2. Добавь текст "Game Scene - Coming in Iteration 4"
3. Добавь в Build Settings обе сцены (MainMenu первая)

---

## Тестирование

### Что проверить:

1. **Запуск сцены MainMenu**
   - Должен появиться заголовок с анимацией (увеличивается)
   - Кнопки должны появляться по очереди с анимацией

2. **Нажатие на кнопки**
   - Кнопки должны сжиматься при нажатии
   - После отпускания - возвращаться с bounce эффектом

3. **Переход в Settings**
   - Нажми "SETTINGS"
   - MainMenu должен скрыться с анимацией
   - Settings должен появиться с анимацией (слайд снизу)
   - Слайдеры должны работать (0-100%)
   - Toggle вибрации должен работать

4. **Кнопка Back**
   - Должна вернуть на главное меню с анимацией

5. **Level Select и Achievements**
   - Должны открываться и показывать заглушку
   - Кнопка Back должна работать

6. **Infinite и Time Attack**
   - Должны запускать переход на сцену Game (fade to black, загрузка сцены, fade in)

---

## Ожидаемый результат

- Плавные анимации появления/скрытия экранов
- Кнопки реагируют на нажатие визуально
- Настройки сохраняются между сессиями (PlayerPrefs)
- Переходы между сценами с fade эффектом
- На iOS устройстве будет работать вибрация при нажатии на кнопки

---

## Возможные проблемы

### DOTween не работает
Убедись что DOTween установлен и выполнен Setup (Tools > Demigiant > DOTween Utility Panel > Setup DOTween)

### Ошибки компиляции с iOS плагином
Это нормально в редакторе. Плагин работает только на реальном iOS устройстве.

### UI не масштабируется
Проверь настройки Canvas Scaler (Reference Resolution 1080x1920, Scale With Screen Size)

---

## Следующая итерация

В Итерации 2 будет добавлен:
- AudioAnalyzer с realtime настройками в инспекторе
- Визуализация спектра для отладки
- Пресеты под электронику/DnB
