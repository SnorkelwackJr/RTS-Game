using UnityEngine;

public static class Utils
{
    static Camera _mainCamera;
    public static Camera MainCamera
    {
        get
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;
            return _mainCamera;
        }
    }

    static Texture2D _whiteTexture;
    public static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    static Ray _ray;
    static RaycastHit _hit;

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        Utils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    public static Rect GetBoundingBoxOnScreen(Bounds bounds, Camera camera)
    {
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;
        Vector3[] vertices = new Vector3[] {
            center + Vector3.right * size.x / 2f + Vector3.up * size.y / 2f + Vector3.forward * size.z / 2f,
            center + Vector3.right * size.x / 2f + Vector3.up * size.y / 2f - Vector3.forward * size.z / 2f,
            center + Vector3.right * size.x / 2f - Vector3.up * size.y / 2f + Vector3.forward * size.z / 2f,
            center + Vector3.right * size.x / 2f - Vector3.up * size.y / 2f - Vector3.forward * size.z / 2f,
            center - Vector3.right * size.x / 2f + Vector3.up * size.y / 2f + Vector3.forward * size.z / 2f,
            center - Vector3.right * size.x / 2f + Vector3.up * size.y / 2f - Vector3.forward * size.z / 2f,
            center - Vector3.right * size.x / 2f - Vector3.up * size.y / 2f + Vector3.forward * size.z / 2f,
            center - Vector3.right * size.x / 2f - Vector3.up * size.y / 2f - Vector3.forward * size.z / 2f,
        };
        Rect retVal = Rect.MinMaxRect(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = camera.WorldToScreenPoint(vertices[i]);
            if (v.x < retVal.xMin)
                retVal.xMin = v.x;
            if (v.y < retVal.yMin)
                retVal.yMin = v.y;
            if (v.x > retVal.xMax)
                retVal.xMax = v.x;
            if (v.y > retVal.yMax)
                retVal.yMax = v.y;
        }

        return retVal;
    }

    public static int GetAlphaKeyValue(string inputString)
    {
        if (inputString == "1" || inputString == "&") return 1;
        if (inputString == "2" || inputString == "é") return 2;
        if (inputString == "3" || inputString == "\"") return 3;
        if (inputString == "4" || inputString == "'") return 4;
        if (inputString == "5" || inputString == "(") return 5;
        if (inputString == "6" || inputString == "§") return 6;
        if (inputString == "7" || inputString == "è") return 7;
        if (inputString == "8" || inputString == "!") return 8;
        if (inputString == "9" || inputString == "ç") return 9;
        return -1;
    }

    public static Vector3 MiddleOfScreenPointToWorld()
        { return MiddleOfScreenPointToWorld(MainCamera); }
    public static Vector3 MiddleOfScreenPointToWorld(Camera cam)
    {
        _ray = cam.ScreenPointToRay(0.5f * new Vector2(Screen.width, Screen.height));
        if (Physics.Raycast(
                _ray,
                out _hit,
                1000f,
                Globals.TERRAIN_LAYER_MASK
            )) return _hit.point;
        return Vector3.zero;
    }
}