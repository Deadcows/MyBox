using System;
using MyBox.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class MyGizmo
{
    public static void DrawString(string text, Vector3 worldPos, Color? colour = null)
    {
#if UNITY_EDITOR
        Handles.BeginGUI();
        var defaultColor = GUI.color;

        if (colour.HasValue) GUI.color = colour.Value;
        var view = SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);

        GUI.color = defaultColor;
        Handles.EndGUI();
#endif
    }

    public static void DrawArrowRay(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f,
        float arrowHeadAngle = 20.0f)
    {
#if UNITY_EDITOR
        Gizmos.DrawRay(position, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) *
                        new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) *
                       new Vector3(0, 0, 1);
        Gizmos.DrawRay(position + direction, right * arrowHeadLength);
        Gizmos.DrawRay(position + direction, left * arrowHeadLength);
#endif
    }


    public static void DebugCross(Vector3 position, float size)
    {
#if UNITY_EDITOR
        var halfSize = size / 2;
        Debug.DrawLine(position.OffsetY(-halfSize), position.OffsetY(halfSize), Color.green, .2f);
        Debug.DrawLine(position.OffsetX(-halfSize), position.OffsetX(halfSize), Color.red, .2f);
        Debug.DrawLine(position.OffsetZ(-halfSize), position.OffsetZ(halfSize), Color.blue, .2f);
#endif
    }


    public static void OnDrawGizmos(Action action)
    {
        Handler.DrawGizmos += action;
    }

    private static MyGizmoHandler Handler
    {
        get { return _handler != null ? _handler : (_handler = CreateHandler()); }
    }


    private static MyGizmoHandler _handler;

    private static MyGizmoHandler CreateHandler()
    {
        var go = new GameObject("Gizmo Handler");
        go.hideFlags = HideFlags.DontSave;

        return go.AddComponent<MyGizmoHandler>();
    }
}