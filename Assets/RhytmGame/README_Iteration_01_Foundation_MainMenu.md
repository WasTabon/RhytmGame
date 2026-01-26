# README — Iteration 01: Foundation + Main Menu

## Что добавлено

### Скрипты:
- **GameManager.cs** — Синглтон, хранит выбранный режим игры (Levels/Infinite/TimeAttack)
- **SceneLoader.cs** — Синглтон, загрузка сцен с fade эффектом
- **FadeController.cs** — Синглтон, управление fade in/out через DOTween
- **MainMenuController.cs** — Логика главного меню, переключение панелей
- **ButtonAnimationTrigger.cs** — Анимация кнопок (hover, press)
- **MainMenuInit.cs** — Инициализация синглтонов при старте MainMenu
- **GameSceneInit.cs** — Инициализация Game сцены

---

## Инструкция по настройке

### 1. Установка DOTween
1. Window → Package Manager → My Assets → DOTween (если ещё не установлен)
2. После импорта появится окно DOTween Setup → нажми **Setup DOTween**

### 2. Создание сцен

#### MainMenu сцена:
1. Создай новую сцену: `Assets/RhythmGame/Scenes/MainMenu.unity`
2. Создай **Canvas** (UI → Canvas):
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler: Scale With Screen Size, Reference Resolution: 1080x1920, Match: 0.5

3. Внутри Canvas создай структуру:

```
Canvas
├── FadePanel (UI → Image)
│   - Color: Black (0,0,0,1)
│   - Anchor: Stretch-Stretch (fill entire screen)
│   - Raycast Target: true
│
├── MainMenuPanel (UI → Panel или Empty с RectTransform)
│   - Anchor: Center, Pivot: 0.5, 0.5
│   - Pos: 0, 0
│   │
│   ├── TitleText (UI → Text или TextMeshPro)
│   │   - Text: "STARLOCK"
│   │   - Font Size: 80-120
│   │   - Pos Y: 400
│   │
│   ├── PlayButton (UI → Button)
│   │   - Text: "PLAY"
│   │   - Pos Y: 100
│   │
│   ├── SettingsButton (UI → Button)
│   │   - Text: "SETTINGS"
│   │   - Pos Y: -50
│   │
│   └── QuitButton (UI → Button)
│       - Text: "QUIT"
│       - Pos Y: -200
│
├── ModeSelectPanel (UI → Panel)
│   - Anchor: Center
│   - Pos: 0, 0 (скрипт сдвинет вправо при старте)
│   │
│   ├── TitleText — "SELECT MODE"
│   ├── LevelsButton — "LEVELS"
│   ├── InfiniteButton — "INFINITE"
│   ├── TimeAttackButton — "TIME ATTACK"
│   └── BackButton — "BACK"
│
└── SettingsPanel (UI → Panel)
    - Anchor: Center
    - Pos: 0, 0
    │
    ├── TitleText — "SETTINGS"
    └── BackButton — "BACK"
```

4. Создай пустой GameObject **MainMenuInit**:
   - Добавь компонент **MainMenuInit**
   - Перетащи FadePanel (Image) в поле Fade Image

5. Создай пустой GameObject **MainMenuController**:
   - Добавь компонент **MainMenuController**
   - Заполни все поля (панели и кнопки)

#### Game сцена:
1. Создай новую сцену: `Assets/RhythmGame/Scenes/Game.unity`
2. Создай **Canvas** с тем же Canvas Scaler
3. Внутри Canvas создай **FadePanel** (как в MainMenu)
4. Создай пустой GameObject **GameSceneInit**:
   - Добавь компонент **GameSceneInit**
   - Перетащи FadePanel в поле Fade Image

### 3. Build Settings
1. File → Build Settings
2. Добавь сцены в таком порядке:
   - `Scenes/MainMenu` (index 0)
   - `Scenes/Game` (index 1)

---

## Как тестировать

1. Открой сцену **MainMenu**
2. Нажми Play
3. Проверь:
   - При старте экран плавно появляется из чёрного (fade in)
   - Кнопка **PLAY** → панель уезжает влево, появляется выбор режима
   - Кнопка **BACK** → возврат к главному меню
   - Кнопки при наведении увеличиваются, при нажатии уменьшаются
   - Выбор любого режима → fade out → загрузка Game сцены → fade in
   - Кнопка **SETTINGS** → панель настроек
   - Кнопка **QUIT** → выход (в Editor остановит Play mode)

---

## Ожидаемый результат

- Полностью рабочее главное меню с плавными переходами
- Три панели: Main, Mode Select, Settings
- Все кнопки анимированы
- Fade переходы между сценами
- Выбранный режим сохраняется в GameManager.Instance.CurrentMode

---

## Структура папок после итерации

```
Assets/
  RhythmGame/
    Scenes/
      MainMenu.unity
      Game.unity
    Scripts/
      GameManager.cs
      SceneLoader.cs
      FadeController.cs
      MainMenuController.cs
      ButtonAnimationTrigger.cs
      MainMenuInit.cs
      GameSceneInit.cs
```
