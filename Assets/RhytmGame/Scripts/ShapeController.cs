using UnityEngine;
using System.Collections.Generic;

public class ShapeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ShapeData shapeData;

    [Header("Blade Colors")]
    [SerializeField] private Color[] bladeColors;

    [Header("Marker Settings")]
    [SerializeField] private float markerDistance = 2f;
    [SerializeField] private float markerSize = 0.3f;
    [SerializeField] private int markerSortingOrder = 15;

    [Header("Current State (Read Only)")]
    [SerializeField] private int currentShapeIndex;
    [SerializeField] private int bladeCount;

    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Color[] BladeColors => bladeColors;
    public int BladeCount => bladeCount;
    public int CurrentShapeIndex => currentShapeIndex;

    private List<SpriteRenderer> bladeMarkers = new List<SpriteRenderer>();
    private Sprite markerSprite;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        CreateMarkerSprite();
    }

    private void CreateMarkerSprite()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        markerSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    public void SetRandomShape()
    {
        if (shapeData == null || shapeData.ShapeCount == 0)
            return;

        int index = Random.Range(0, shapeData.ShapeCount);
        SetShape(index);
    }

    public void SetShape(int index)
    {
        if (shapeData == null)
            return;

        currentShapeIndex = index;
        var sprite = shapeData.GetShape(index);
        
        if (sprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            ParseBladeCountFromName(sprite.name);
        }
    }

    public void SetShape(Sprite sprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            ParseBladeCountFromName(sprite.name);
        }
    }

    private void ParseBladeCountFromName(string spriteName)
    {
        bladeCount = 4;
        
        for (int i = 3; i <= 7; i++)
        {
            if (spriteName.Contains(i.ToString() + "blade") || 
                spriteName.Contains(i.ToString() + "_blade") ||
                spriteName.Contains(i.ToString() + "Blade"))
            {
                bladeCount = i;
                break;
            }
        }
    }

    public void SetBladeColors(Color[] colors)
    {
        bladeColors = colors;
        UpdateMarkerColors();
    }

    public void GenerateRandomBladeColors(int count)
    {
        bladeCount = count;
        bladeColors = new Color[count];
        
        Color[] possibleColors = new Color[]
        {
            new Color(1f, 0.3f, 0.3f),
            new Color(0.3f, 1f, 0.3f),
            new Color(0.3f, 0.5f, 1f),
            new Color(1f, 1f, 0.3f),
            new Color(1f, 0.5f, 0f),
            new Color(0.8f, 0.3f, 1f),
            new Color(0f, 1f, 1f)
        };

        for (int i = 0; i < count; i++)
        {
            bladeColors[i] = possibleColors[Random.Range(0, possibleColors.Length)];
        }

        CreateBladeMarkers();
    }

    public void CreateBladeMarkers()
    {
        ClearMarkers();

        if (bladeCount <= 0 || bladeColors == null || bladeColors.Length == 0)
            return;

        float angleStep = 360f / bladeCount;
        float offsetAngle = angleStep / 2f;

        for (int i = 0; i < bladeCount; i++)
        {
            float angle = angleStep * i + offsetAngle - 90f;
            float angleRad = angle * Mathf.Deg2Rad;

            Vector3 position = new Vector3(
                Mathf.Cos(angleRad) * markerDistance,
                Mathf.Sin(angleRad) * markerDistance,
                0f
            );

            GameObject markerGO = new GameObject($"BladeMarker_{i}");
            markerGO.transform.SetParent(transform);
            markerGO.transform.localPosition = position;
            markerGO.transform.localRotation = Quaternion.Euler(0, 0, angle + 45f);
            markerGO.transform.localScale = Vector3.one * markerSize;

            SpriteRenderer markerRenderer = markerGO.AddComponent<SpriteRenderer>();
            markerRenderer.sprite = markerSprite;
            markerRenderer.sortingOrder = markerSortingOrder;
            
            if (i < bladeColors.Length)
            {
                markerRenderer.color = bladeColors[i];
            }

            bladeMarkers.Add(markerRenderer);
        }
    }

    public void ClearMarkers()
    {
        foreach (var marker in bladeMarkers)
        {
            if (marker != null)
            {
                if (Application.isPlaying)
                    Destroy(marker.gameObject);
                else
                    DestroyImmediate(marker.gameObject);
            }
        }
        bladeMarkers.Clear();
    }

    private void UpdateMarkerColors()
    {
        for (int i = 0; i < bladeMarkers.Count && i < bladeColors.Length; i++)
        {
            if (bladeMarkers[i] != null)
            {
                bladeMarkers[i].color = bladeColors[i];
            }
        }
    }

    public void SetShapeData(ShapeData data)
    {
        shapeData = data;
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }

    public void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        foreach (var marker in bladeMarkers)
        {
            if (marker != null)
            {
                var color = marker.color;
                color.a = alpha;
                marker.color = color;
            }
        }
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }

    public void SetRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public float GetRotation()
    {
        return transform.rotation.eulerAngles.z;
    }

    public void SetMarkerDistance(float distance)
    {
        markerDistance = distance;
        if (bladeMarkers.Count > 0)
        {
            CreateBladeMarkers();
        }
    }

    public void SetMarkerSize(float size)
    {
        markerSize = size;
        foreach (var marker in bladeMarkers)
        {
            if (marker != null)
            {
                marker.transform.localScale = Vector3.one * markerSize;
            }
        }
    }

    private void OnDestroy()
    {
        ClearMarkers();
    }
}
