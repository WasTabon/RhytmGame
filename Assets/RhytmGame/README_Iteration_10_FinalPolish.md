# README — Iteration 10: Final Polish

## Что добавлено

### Скрипты:
- **PauseMenu.cs** — Меню паузы в игре (ESC или кнопка)
- **PauseButton.cs** — Кнопка паузы для мобильных
- **SoundManager.cs** — Управление звуками и музыкой
- **MainMenuController.cs** — Обновлён (интеграция с Achievements, звуки, слайдеры)
- **FinalPolishSetupWindow.cs** — Editor скрипт для настройки

---

## Быстрая настройка

### Game сцена:

1. **RhythmGame → Setup Final Polish**
2. Поставь галку **Is Game Scene**
3. Назначь **Canvas**
4. **Create Pause Menu** — создаст панель паузы
5. **Create Pause Button** — создаст кнопку для мобильных (верхний правый угол)
6. **Create Sound Manager** — создаст менеджер звуков

### MainMenu сцена:

1. **RhythmGame → Setup Final Polish**
2. Убери галку **Is Game Scene**
3. Назначь **Canvas**
4. **Add Settings Sliders** — добавит слайдеры в Settings панель
5. **Add Achievements Button** — добавит кнопку Achievements
6. **Create Sound Manager** — создаст менеджер звуков

---

## Pause Menu

### Функции:
- Пауза по нажатию **ESC** или кнопки
- Слайдеры громкости (Music / SFX)
- Кнопки: Resume, Restart, Menu
- Анимация появления (scale + fade)
- Останавливает `Time.timeScale`

### Кнопки:
| Кнопка | Действие |
|--------|----------|
| **Resume** | Закрывает паузу, продолжает игру |
| **Restart** | Перезапускает текущий режим |
| **Menu** | Возвращает в MainMenu |

---

## Sound Manager

### Звуки:
| ID | Когда воспроизводится |
|----|----------------------|
| perfect | Perfect hit |
| good | Good hit |
| miss | Miss |
| combo_break | Combo сброшен |
| button | Нажатие кнопки |
| achievement | Разблокировка достижения |
| level_complete | Уровень пройден |
| game_over | Game Over |

### Настройка звуков:
1. Создай AudioClip'ы (можно найти бесплатные на freesound.org)
2. В SoundManager назначь их в соответствующие поля
3. Громкость сохраняется в PlayerPrefs

### API:
```csharp
SoundManager.Instance.PlaySFX("perfect");
SoundManager.Instance.PlayButtonClick();
SoundManager.Instance.SetMusicVolume(0.5f);
SoundManager.Instance.SetSFXVolume(0.8f);
```

---

## Настройки громкости

### Сохранение:
- `PlayerPrefs["MusicVolume"]` — громкость музыки (0-1)
- `PlayerPrefs["SFXVolume"]` — громкость звуков (0-1)

### Слайдеры:
- В Pause Menu (Game сцена)
- В Settings Panel (MainMenu сцена)

---

## MainMenu интеграция

### Новые поля в MainMenuController:
- `achievementsUI` — ссылка на AchievementsUI
- `achievementsButton` — кнопка в Settings
- `musicSlider` / `sfxSlider` — слайдеры громкости

### Навигация:
```
Settings → Achievements Button → AchievementsUI.Show()
         ← Back Button         ← AchievementsUI.Hide()
```

---

## Структура после настройки

### Game сцена:
```
Game Scene
├── Canvas
│   ├── ... (HUD elements)
│   ├── PausePanel ← НОВОЕ
│   │   ├── TitleText
│   │   ├── MusicSlider
│   │   ├── SFXSlider
│   │   ├── ResumeButton
│   │   ├── RestartButton
│   │   └── MenuButton
│   ├── PauseButton ← НОВОЕ (верхний правый угол)
│   └── GameOverPanel
├── PauseMenu ← НОВОЕ
├── SoundManager ← НОВОЕ (DontDestroyOnLoad)
└── ... (other managers)
```

### MainMenu сцена:
```
MainMenu Scene
├── Canvas
│   ├── MainMenuPanel
│   ├── ModeSelectPanel
│   ├── SettingsPanel
│   │   ├── ... (existing)
│   │   ├── MusicSlider ← НОВОЕ
│   │   ├── SFXSlider ← НОВОЕ
│   │   └── AchievementsButton ← НОВОЕ
│   ├── LevelSelectPanel
│   └── AchievementsPanel
├── MainMenuController
├── SoundManager ← НОВОЕ
└── ... (other managers)
```

---

## Звуки — рекомендации

### Бесплатные источники:
- freesound.org
- opengameart.org
- mixkit.co

### Рекомендуемые форматы:
- **.wav** — для коротких эффектов
- **.ogg** — для сжатия

### Примерные звуки:
| Звук | Описание |
|------|----------|
| Perfect | Высокий, чистый тон, "пинг" |
| Good | Средний тон, менее яркий |
| Miss | Низкий, приглушённый, "тамп" |
| Combo Break | Резкий, обрывающийся |
| Button | Мягкий клик |
| Achievement | Фанфары, успех |
| Level Complete | Победный джингл |
| Game Over | Грустный/нейтральный |

---

## API

### PauseMenu:
```csharp
PauseMenu.Instance.Pause();
PauseMenu.Instance.Resume();
PauseMenu.Instance.TogglePause();
PauseMenu.Instance.IsPaused;
```

### SoundManager:
```csharp
SoundManager.Instance.PlaySFX("sound_name");
SoundManager.Instance.PlaySFX(audioClip);
SoundManager.Instance.PlayButtonClick();
SoundManager.Instance.SetMusicVolume(float);
SoundManager.Instance.SetSFXVolume(float);
SoundManager.Instance.GetMusicVolume();
SoundManager.Instance.GetSFXVolume();
SoundManager.Instance.RegisterSound("name", clip);
SoundManager.Instance.LateSubscribe();
```

---

## Важно

### DontDestroyOnLoad:
- **SoundManager** — создаётся один раз, живёт между сценами
- Если уже существует — дубликат уничтожается

### Инициализация в Game сцене:
Если менеджеры создаются в Game сцене, вызови `LateSubscribe()`:

```csharp
void Start()
{
    if (SoundManager.Instance != null)
        SoundManager.Instance.LateSubscribe();
}
```

### Time.timeScale:
- Pause Menu устанавливает `Time.timeScale = 0`
- Анимации используют `.SetUpdate(true)` чтобы работать на паузе
- При выходе из паузы `Time.timeScale = 1`

---

## Тестирование

### Pause Menu:
1. Запусти Game сцену
2. Нажми **ESC** — должно открыться меню паузы
3. Игра должна остановиться (шейп не вращается)
4. Подвигай слайдеры — громкость должна меняться
5. Нажми **Resume** — игра продолжается
6. Нажми **Restart** — раунд начинается заново
7. Нажми **Menu** — переход в MainMenu

### Звуки:
1. Назначь AudioClip'ы в SoundManager
2. Сделай Perfect/Good/Miss — должны играть звуки
3. Сбрось комбо — звук combo_break
4. Нажми кнопки в меню — звук клика

### Слайдеры в MainMenu:
1. Открой Settings
2. Подвигай слайдеры
3. Перезапусти игру — настройки должны сохраниться

---

## Ожидаемый результат

- Полноценное меню паузы с настройками
- Звуковое сопровождение всех действий
- Настройки громкости сохраняются
- Кнопка Achievements работает из Settings
- Плавные анимации и переходы
- Готовая к релизу игра!
