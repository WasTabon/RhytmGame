using UnityEngine;
using UnityEditor;
using System.IO;

public class ShurikenGeneratorWindow : EditorWindow
{
    private int bladeCount = 4;
    private int textureSize = 512;
    private float bladeLength = 0.9f;
    private float bladeWidth = 0.5f;
    private float centerRadius = 0.15f;
    private Color bladeColor = new Color(0.85f, 0.85f, 0.9f);
    private Color centerColor = new Color(0.3f, 0.3f, 0.35f);
    private Color outlineColor = new Color(0.2f, 0.2f, 0.25f);
    private bool generateAll = true;
    private string savePath = "Assets/RhythmGame/Sprites/Shurikens";

    [MenuItem("RhythmGame/Generate Test Shurikens")]
    public static void ShowWindow()
    {
        GetWindow<ShurikenGeneratorWindow>("Shuriken Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Shuriken Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        generateAll = EditorGUILayout.Toggle("Generate All (3-7 blades)", generateAll);
        
        if (!generateAll)
        {
            bladeCount = EditorGUILayout.IntSlider("Blade Count", bladeCount, 3, 7);
        }

        GUILayout.Space(10);
        GUILayout.Label("Texture Settings", EditorStyles.boldLabel);
        textureSize = EditorGUILayout.IntPopup("Texture Size", textureSize, 
            new string[] { "256", "512", "1024" }, 
            new int[] { 256, 512, 1024 });

        GUILayout.Space(10);
        GUILayout.Label("Shape Settings", EditorStyles.boldLabel);
        bladeLength = EditorGUILayout.Slider("Blade Length", bladeLength, 0.5f, 0.95f);
        bladeWidth = EditorGUILayout.Slider("Blade Width", bladeWidth, 0.2f, 1.0f);
        centerRadius = EditorGUILayout.Slider("Center Radius", centerRadius, 0.1f, 0.4f);

        GUILayout.Space(10);
        GUILayout.Label("Colors", EditorStyles.boldLabel);
        bladeColor = EditorGUILayout.ColorField("Blade Color", bladeColor);
        centerColor = EditorGUILayout.ColorField("Center Color", centerColor);
        outlineColor = EditorGUILayout.ColorField("Outline Color", outlineColor);

        GUILayout.Space(10);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        GUILayout.Space(20);

        if (GUILayout.Button("Generate Shuriken(s)", GUILayout.Height(40)))
        {
            GenerateShurikens();
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Generated sprites will be saved as PNG files.\n" +
            "After generation, create a ShapeData asset and assign the sprites.",
            MessageType.Info);
    }

    private void GenerateShurikens()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        if (generateAll)
        {
            for (int i = 3; i <= 7; i++)
            {
                GenerateShuriken(i);
            }
            Debug.Log($"Generated 5 shurikens (3-7 blades) in {savePath}");
        }
        else
        {
            GenerateShuriken(bladeCount);
            Debug.Log($"Generated {bladeCount}-blade shuriken in {savePath}");
        }

        AssetDatabase.Refresh();
    }

    private void GenerateShuriken(int blades)
    {
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        texture.SetPixels(pixels);

        Vector2 center = new Vector2(textureSize / 2f, textureSize / 2f);
        float angleStep = 360f / blades;

        for (int i = 0; i < blades; i++)
        {
            float angle = i * angleStep - 90f;
            DrawBlade(texture, center, angle, blades);
        }

        DrawCircle(texture, center, centerRadius * textureSize * 0.5f, centerColor);
        DrawCircleOutline(texture, center, centerRadius * textureSize * 0.5f, outlineColor, 3);

        texture.Apply();

        byte[] pngData = texture.EncodeToPNG();
        string fileName = $"Shuriken_{blades}blade.png";
        string fullPath = Path.Combine(savePath, fileName);
        File.WriteAllBytes(fullPath, pngData);

        DestroyImmediate(texture);

        AssetDatabase.Refresh();
        
        string assetPath = fullPath;
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }
    }

    private void DrawBlade(Texture2D texture, Vector2 center, float angleDegrees, int totalBlades)
    {
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        float widthAngle = (bladeWidth * 180f / totalBlades) * Mathf.Deg2Rad;
        
        float len = bladeLength * textureSize * 0.5f;
        float innerRadius = centerRadius * textureSize * 0.5f * 0.9f;

        Vector2 tip = center + new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * len;
        
        Vector2 baseLeft = center + new Vector2(
            Mathf.Cos(angleRad - widthAngle), 
            Mathf.Sin(angleRad - widthAngle)) * innerRadius;
        Vector2 baseRight = center + new Vector2(
            Mathf.Cos(angleRad + widthAngle), 
            Mathf.Sin(angleRad + widthAngle)) * innerRadius;

        float midLen = len * 0.5f;
        Vector2 midLeft = center + new Vector2(
            Mathf.Cos(angleRad - widthAngle * 0.7f), 
            Mathf.Sin(angleRad - widthAngle * 0.7f)) * midLen;
        Vector2 midRight = center + new Vector2(
            Mathf.Cos(angleRad + widthAngle * 0.7f), 
            Mathf.Sin(angleRad + widthAngle * 0.7f)) * midLen;

        DrawQuad(texture, baseLeft, midLeft, midRight, baseRight, bladeColor);
        DrawTriangle(texture, tip, midLeft, midRight, bladeColor);
        
        DrawLine(texture, baseLeft, midLeft, outlineColor, 2);
        DrawLine(texture, midLeft, tip, outlineColor, 2);
        DrawLine(texture, tip, midRight, outlineColor, 2);
        DrawLine(texture, midRight, baseRight, outlineColor, 2);
    }

    private void DrawQuad(Texture2D texture, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
    {
        DrawTriangle(texture, p1, p2, p4, color);
        DrawTriangle(texture, p2, p3, p4, color);
    }

    private void DrawTriangle(Texture2D texture, Vector2 p1, Vector2 p2, Vector2 p3, Color color)
    {
        int minX = Mathf.Max(0, (int)Mathf.Min(p1.x, Mathf.Min(p2.x, p3.x)) - 1);
        int maxX = Mathf.Min(textureSize - 1, (int)Mathf.Max(p1.x, Mathf.Max(p2.x, p3.x)) + 1);
        int minY = Mathf.Max(0, (int)Mathf.Min(p1.y, Mathf.Min(p2.y, p3.y)) - 1);
        int maxY = Mathf.Min(textureSize - 1, (int)Mathf.Max(p1.y, Mathf.Max(p2.y, p3.y)) + 1);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2 p = new Vector2(x, y);
                if (PointInTriangle(p, p1, p2, p3))
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private void DrawLine(Texture2D texture, Vector2 from, Vector2 to, Color color, int thickness)
    {
        float distance = Vector2.Distance(from, to);
        int steps = (int)(distance * 2);
        
        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 0 : (float)i / steps;
            Vector2 point = Vector2.Lerp(from, to, t);
            
            for (int dx = -thickness; dx <= thickness; dx++)
            {
                for (int dy = -thickness; dy <= thickness; dy++)
                {
                    int px = (int)point.x + dx;
                    int py = (int)point.y + dy;
                    if (px >= 0 && px < textureSize && py >= 0 && py < textureSize)
                    {
                        if (dx * dx + dy * dy <= thickness * thickness)
                        {
                            texture.SetPixel(px, py, color);
                        }
                    }
                }
            }
        }
    }

    private void DrawCircle(Texture2D texture, Vector2 center, float radius, Color color)
    {
        int minX = Mathf.Max(0, (int)(center.x - radius) - 1);
        int maxX = Mathf.Min(textureSize - 1, (int)(center.x + radius) + 1);
        int minY = Mathf.Max(0, (int)(center.y - radius) - 1);
        int maxY = Mathf.Min(textureSize - 1, (int)(center.y + radius) + 1);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist <= radius)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private void DrawCircleOutline(Texture2D texture, Vector2 center, float radius, Color color, int thickness)
    {
        int minX = Mathf.Max(0, (int)(center.x - radius - thickness) - 1);
        int maxX = Mathf.Min(textureSize - 1, (int)(center.x + radius + thickness) + 1);
        int minY = Mathf.Max(0, (int)(center.y - radius - thickness) - 1);
        int maxY = Mathf.Min(textureSize - 1, (int)(center.y + radius + thickness) + 1);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist >= radius - thickness && dist <= radius + thickness)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private bool PointInTriangle(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float d1 = Sign(p, p1, p2);
        float d2 = Sign(p, p2, p3);
        float d3 = Sign(p, p3, p1);

        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(hasNeg && hasPos);
    }

    private float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
}
