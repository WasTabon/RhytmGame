using UnityEngine;

[CreateAssetMenu(fileName = "ShapeData", menuName = "RhythmGame/Shape Data")]
public class ShapeData : ScriptableObject
{
    [SerializeField] private Sprite[] shapes;

    public Sprite GetRandomShape()
    {
        if (shapes == null || shapes.Length == 0)
            return null;

        return shapes[Random.Range(0, shapes.Length)];
    }

    public Sprite GetShape(int index)
    {
        if (shapes == null || shapes.Length == 0)
            return null;

        index = Mathf.Clamp(index, 0, shapes.Length - 1);
        return shapes[index];
    }

    public Sprite[] GetAllShapes()
    {
        return shapes;
    }

    public int ShapeCount => shapes != null ? shapes.Length : 0;
}
