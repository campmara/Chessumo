using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class UtilitiesShortcuts {
    [MenuItem("Utilities/Shortcuts/Clear Console %#c")] // CTRL/CMD + SHIFT + C
    public static void ClearConsole() {
        try {
            var logEntries = Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            if (logEntries != null) {
                var method = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
                if (method != null) {
                    method.Invoke(null, null);
                }
            }
        } catch (Exception exception) {
            Debug.LogError("Failed to clear the console: " + exception.ToString());
        }
    }

    [MenuItem("Utilities/Shortcuts/Save project &%s")] // ALT + CTRL + S
    static void SaveProject() {
        Debug.Log("Saved assets to disk.");
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Shortcuts/Toggle Inspector Debug %#d")] // CTRL/CMD + SHIFT + C
    public static void ToggleInspectorDebug() {
        try {
            var type = Type.GetType("UnityEditor.InspectorWindow,UnityEditor");
            if (type != null) {
                var window = EditorWindow.GetWindow(type);
                var field = type.GetField("m_InspectorMode", BindingFlags.Instance | BindingFlags.Public);
                if (field != null) {
                    var mode = (InspectorMode)field.GetValue(window);
                    var newMode = mode == InspectorMode.Debug ? InspectorMode.Normal : InspectorMode.Debug;

                    var method = type.GetMethod("SetMode", BindingFlags.Instance | ~BindingFlags.Public);
                    if (method != null) {
                        method.Invoke(window, new object[] { newMode });
                    }
                }
            }
        } catch (Exception exception) {
            Debug.LogError("Failed to toggle inspector debug: " + exception.ToString());
        }
    }

    [MenuItem("Utilities/Shortcuts/Toggle GameView maximized %#m")] // CTRL/CMD + SHIFT + M
    public static void ToggleGameViewMaximized() {
        try {
            var type = Type.GetType("UnityEditor.GameView,UnityEditor");
            if (type != null) {
                var window = EditorWindow.GetWindow(type);
                var property = type.GetProperty("maximized", BindingFlags.Instance | BindingFlags.Public);
                if (property != null) {
                    var isMaximized = (bool)property.GetValue(window, null);
                    property.SetValue(window, !isMaximized, null);
                }
            }
        } catch (Exception exception) {
            Debug.LogError("Failed to toggle GameView maximized: " + exception.ToString());
        }
    }

    [MenuItem("Utilities/Shortcuts/Toggle Inspector Lock %#l")] // CTRL/CMD + SHIFT + L
    public static void ToggleInspectorLock() {
        try {
            var type = Type.GetType("UnityEditor.InspectorWindow,UnityEditor");
            if (type != null) {
                var window = EditorWindow.GetWindow(type);

                var method = type.GetMethod("FlipLocked", BindingFlags.Instance | ~BindingFlags.Public);
                if (method != null) {
                    method.Invoke(window, null);
                }
            }
        } catch (Exception exception) {
            Debug.LogError("Failed to toggle inspector debug: " + exception.ToString());
        }
    }
}