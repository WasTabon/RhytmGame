# README — Iteration 02: Audio Analyzer

## Что добавлено

### Скрипты:
- **AudioAnalyzer.cs** — Анализ аудио спектра в реальном времени, выдаёт Low/Mid/High значения (0-1)
- **AudioAnalyzerDebugUI.cs** — Debug визуализация трёх полосок для настройки
- **Editor/GameSceneSetupWindow.cs** — Обновлён, теперь создаёт AudioManager и DebugUI

---

## Параметры AudioAnalyzer (все настраиваются в реалтайме)

### Spectrum Settings
| Параметр | Описание | Значение по умолчанию |
|----------|----------|-----------------------|
| Spectrum Size | Размер массива спектра (64, 128, 256...) | 64 |
| FFT Window | Тип окна для FFT анализа | BlackmanHarris |

### Frequency Ranges
| Параметр | Описание | Значение по умолчанию |
|----------|----------|-----------------------|
| Low Range | Диапазон для басов | 0-8 |
| Mid Range | Диапазон для средних | 8-32 |
| High Range | Диапазон для высоких | 32-64 |

### Sensitivity
| Параметр | Описание | Значение по умолчанию |
|----------|----------|-----------------------|
| Low Multiplier | Усиление басов | 8 |
| Mid Multiplier | Усиление средних | 4 |
| High Multiplier | Усиление высоких | 2 |

### Smoothing
| Параметр | Описание | Значение по умолчанию |
|----------|----------|-----------------------|
| Use Smoothing | Включить сглаживание | true |
| Smoothing Speed | Скорость сглаживания | 10 |

### Output (только чтение)
| Параметр | Описание |
|----------|----------|
| Low Value | Текущее значение басов (0-1) |
| Mid Value | Текущее значение средних (0-1) |
| High Value | Текущее значение высоких (0-1) |

---

## Инструкция по настройке

### 1. Добавь файлы
Скопируй новые файлы в проект:
- `Scripts/AudioAnalyzer.cs`
- `Scripts/AudioAnalyzerDebugUI.cs`
- `Scripts/Editor/GameSceneSetupWindow.cs` (замени старый)

### 2. Пересоздай Game сцену
1. Открой Game сцену
2. Удали старый контент Canvas (если есть)
3. **RhythmGame → Setup Game Scene**
4. Перетащи Canvas → **Create Game Scene UI**

### 3. Добавь музыку
1. Импортируй любой аудиофайл в `Assets/RhythmGame/Audio/`
2. Выбери **AudioManager** на сцене
3. В компоненте **AudioSource** перетащи аудиофайл в поле **AudioClip**

---

## Как тестировать

1. Открой Game сцену
2. Нажми **Play**
3. Музыка начнёт играть
4. Внизу экрана три полоски: **LOW** (красная), **MID** (зелёная), **HIGH** (синяя)
5. Они должны реагировать на музыку

### Настройка в реалтайме:
1. Выбери **AudioManager** в Hierarchy
2. В компоненте **AudioAnalyzer** крути параметры:
   - **Multipliers** — если полоски слабо двигаются, увеличь
   - **Smoothing Speed** — выше = резче реакция, ниже = плавнее
   - **Frequency Ranges** — настрой какие частоты влияют на что

---

## Как использовать в коде

```csharp
// Получить значения из любого скрипта:
float bass = AudioAnalyzer.Instance.LowValue;      // 0-1, сглаженное
float mid = AudioAnalyzer.Instance.MidValue;       // 0-1, сглаженное
float high = AudioAnalyzer.Instance.HighValue;     // 0-1, сглаженное

// Или сырые значения (без сглаживания):
float bassRaw = AudioAnalyzer.Instance.LowValueRaw;
```

---

## Ожидаемый результат

- Музыка играет при запуске
- Три цветные полоски внизу реагируют на музыку
- Все параметры можно менять в инспекторе во время игры
- Значения доступны через AudioAnalyzer.Instance

---

## Структура на сцене после настройки

```
Game Scene
├── Canvas
│   ├── Background
│   ├── DebugPanel
│   │   ├── LowBarContainer/LowBar
│   │   ├── MidBarContainer/MidBar
│   │   └── HighBarContainer/HighBar
│   └── FadePanel
├── AudioManager
│   ├── AudioSource (с твоей музыкой)
│   └── AudioAnalyzer (компонент)
├── GameSceneInit
└── AudioAnalyzerDebugUI
```
