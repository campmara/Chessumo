using UnityEngine;
using UnityEditor;

/// <summary>
/// Small class that adds a button in the inspector to clear the PlayerPrefs save data.
/// </summary>
[CustomEditor(typeof(AppManager))]
public class AppManagerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        AppManager app = (AppManager)target;
        if (GUILayout.Button("Reset PlayerPrefs")) {
            app.ResetPlayerPrefs();
        }
    }
}
