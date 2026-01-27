# README — Iteration 04: Rotation + Music Reaction

## Что добавлено

### Скрипты:
- **RotationController.cs** — Вращение сюрикена с реакцией на музыку

---

## Параметры RotationController

### Base Rotation
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Base Speed | Базовая скорость вращения (градусов/сек) | 90 |
| Clockwise | Направление по часовой стрелке | true |

### Music Reaction - Speed (Mid)
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Enable Speed Reaction | Включить реакцию скорости на Mid | true |
| Speed Multiplier | Множитель ускорения от Mid | 2 |

### Music Reaction - Scale (Low)
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Enable Scale Reaction | Включить пульсацию от Bass | true |
| Base Scale | Базовый масштаб | 1 |
| Scale Amount | Амплитуда пульсации | 0.2 |

### Music Reaction - Direction (High)
| Параметр | Описание | Дефолт |
|----------|----------|--------|
| Enable Direction Change | Включить смену направления | true |
| High Threshold | Порог High для смены (0-1) | 0.7 |
| Direction Cooldown | Кулдаун между сменами (сек) | 1 |

### Debug (Read Only)
- Current Speed — текущая скорость
- Current Scale — текущий масштаб
- Current Direction — направление (1 или -1)

---

## Настройка на сцене

### Добавить RotationController:
1. Выбери **PlayerShape** в Hierarchy
2. Add Component → **RotationController**
3. Готово!

RotationController автоматически найдёт AudioAnalyzer.

---

## Как тестировать

1. Добавь **RotationController** на **PlayerShape**
2. Убедись что музыка играет (MusicManager настроен)
3. Нажми **Play**
4. Сюрикен должен:
   - Вращаться с базовой скоростью
   - Ускоряться на средних частотах
   - Пульсировать на басах
   - Менять направление на высоких (если превышен порог)

### Настройка в реалтайме:
- Крути параметры в инспекторе во время Play
- Смотри на Debug значения

### Если реакция слабая:
- Увеличь **Speed Multiplier** (например 3-5)
- Увеличь **Scale Amount** (например 0.3-0.5)
- Уменьши **High Threshold** (например 0.5)

### Если реакция слишком сильная:
- Уменьши множители
- Увеличь пороги

---

## API

```csharp
var rotation = GetComponent<RotationController>();

// Управление
rotation.StopRotation();           // Остановить
rotation.ResumeRotation();         // Продолжить
rotation.ResetRotation();          // Сбросить угол на 0

// Настройки
rotation.SetBaseSpeed(120f);       // Изменить базовую скорость
rotation.SetClockwise(false);      // Против часовой
rotation.SetBaseScale(1.5f);       // Изменить базовый масштаб

// Получить значения
float angle = rotation.GetCurrentRotation();
float speed = rotation.GetCurrentSpeed();
int dir = rotation.GetCurrentDirection();  // 1 или -1
```

---

## Ожидаемый результат

- Сюрикен постоянно вращается
- На басах (Low) пульсирует размер
- На средних (Mid) ускоряется вращение
- На высоких (High) может сменить направление
- Все параметры настраиваются в реалтайме

---

## Структура на сцене

```
GameArea
└── PlayerShape
    ├── SpriteRenderer
    ├── ShapeController
    ├── RotationController  ← НОВОЕ
    └── BladeMarker_0, BladeMarker_1, ... (создаются в рантайме)
```
