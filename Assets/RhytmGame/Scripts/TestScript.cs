using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        var shape = FindObjectOfType<ShapeController>();
        shape.SetRandomShape();
        shape.GenerateRandomBladeColors(shape.BladeCount);
    }
}
