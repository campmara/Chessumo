using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AppManager))]
public class AppManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		AppManager app = (AppManager)target;
		if (GUILayout.Button("Reset PlayerPrefs"))
		{
            app.ResetPlayerPrefs();
		}
	}
}
