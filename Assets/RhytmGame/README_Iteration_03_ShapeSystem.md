# README — Iteration 03: Shape System

## Что добавлено

### Скрипты:
- **ShapeData.cs** — ScriptableObject для хранения списка спрайтов сюрикенов
- **ShapeController.cs** — Компонент для управления отображением сюрикена
- **Editor/ShurikenGeneratorWindow.cs** — Генератор тестовых спрайтов сюрикенов
- **Editor/GameSceneSetupWindow.cs** — Обновлён, теперь создаёт GameArea и PlayerShape

---

## Генерация тестовых спрайтов

### Как сгенерировать:

1. Открой **RhythmGame → Generate Test Shurikens**
2. Настрой параметры:
   - **Generate All** — создаст сразу 5 сюрикенов (3-7 лезвий)
   - **Blade Count** — кол-во лезвий (если Generate All выключен)
   - **Texture Size** — 256/512/1024
   - **Blade Length/Width** — форма лезвий
   - **Colors** — цвета лезвий, центра, обводки
3. Нажми **Generate Shuriken(s)**
4. Спрайты появятся в `Assets/RhythmGame/Sprites/Shurikens/`

---

## Создание ShapeData

1. **ПКМ в Project → Create → RhythmGame → Shape Data**
2. Назови например "ShurikenShapes"
3. В инспекторе разверни **Shapes** и добавь сгенерированные спрайты
4. На сцене в **PlayerShape → ShapeController** назначь этот ShapeData

---

## Структура на сцене после Setup

```
Game Scene
├── Canvas
│   ├── Background
│   ├── DebugPanel (Low/Mid/High bars)
│   └── FadePanel
├── MusicManager
│   ├── AudioSource
│   ├── MusicManager (script)
│   └── AudioAnalyzer (script)
├── GameArea
│   └── PlayerShape
│       ├── SpriteRenderer
│       └── ShapeController (script)
├── GameSceneInit
└── AudioAnalyzerDebugUI
```

---

## ShapeController API

```csharp
// Установить случайную форму из ShapeData
shapeController.SetRandomShape();

// Установить конкретную форму по индексу
shapeController.SetShape(2);

// Установить спрайт напрямую
shapeController.SetShape(mySprite);

// Работа с цветами лезвий (для механики совпадения)
shapeController.GenerateRandomBladeColors(5);  // 5 случайных цветов
shapeController.SetBladeColors(colorArray);    // свой массив цветов
Color[] colors = shapeController.BladeColors;  // получить цвета

// Трансформации
shapeController.SetRotation(45f);     // угол в градусах
shapeController.SetScale(1.5f);       // масштаб
shapeController.SetAlpha(0.5f);       // прозрачность
shapeController.SetColor(Color.red);  // цвет спрайта
```

---

## Как тестировать

1. Закинь новые файлы в проект
2. **RhythmGame → Generate Test Shurikens** → Generate
3. Создай **ShapeData** и добавь туда сгенерированные спрайты
4. Пересоздай Game сцену через **Setup Game Scene**
5. На **PlayerShape** назначь ShapeData в ShapeController
6. Нажми Play — должен отображаться сюрикен в центре экрана

### Тест через код (опционально):
Добавь тестовый скрипт:
```csharp
void Start()
{
    var shape = FindObjectOfType<ShapeController>();
    shape.SetRandomShape();
    shape.GenerateRandomBladeColors(shape.BladeCount);
}
```

---

## Ожидаемый результат

- Тестовые спрайты сюрикенов генерируются в папке
- ShapeData хранит список спрайтов
- PlayerShape отображает сюрикен в центре экрана
- ShapeController управляет спрайтом, цветами, поворотом

---

## Файлы в архиве

```
Scripts/
  ShapeData.cs
  ShapeController.cs
  Editor/
    ShurikenGeneratorWindow.cs
    GameSceneSetupWindow.cs (обновлён)
```
