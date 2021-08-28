using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveExplorerWindow : EditorWindow
{
	string path;

	[MenuItem("Window/Save Explorer")]
    public static void OpenWindow()
	{
		GetWindow<SaveExplorerWindow>("Save Explorer");
	}

	void OnGUI()
	{
		if(EditorPrefs.HasKey("Path"))
		{
			EditorPrefs.SetString("Path", GUILayout.TextField(EditorPrefs.GetString("Path")));
		}
		else
		{
			EditorPrefs.SetString("Path", GUILayout.TextField("File Path"));
		}

		if(GUILayout.Button("Explore"))
		{
			FileInfo fileInfo = new FileInfo(EditorPrefs.GetString("Path"));
			
			EditorUtility.DisplayDialog("File Size: " + fileInfo.Length + " bytes", "Path: " + EditorPrefs.GetString("Path"), "Ok (" + fileInfo.Length + " bytes)");
		}

		if(GUILayout.Button("Show in Explorer"))
		{
			EditorUtility.RevealInFinder(EditorPrefs.GetString("Path"));
		}
	}

}
