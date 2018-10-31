﻿using UnityEditor;
using UnityEngine;

// Unity's GUILayout makes code look so ugly.
// This is a simple custom inspector I wrote for the PrefabPlacerSettings to make it a bit more clear to read.
namespace DB.PrefabPlacer
{
    [CustomEditor(typeof(PrefabPlacerSettings))]
    public class CustomPrefabPlacerSettingsInspector : Editor
    {

        public PrefabPlacerSettings settings;

        private void OnEnable()
        {
            settings = (PrefabPlacerSettings)target;
        }

        public override void OnInspectorGUI()
        {
            EditorStyles.label.wordWrap = true;

            GUILayout.BeginHorizontal();
            GUIStyle labelStyle = new GUIStyle(GUI.skin.box);
            labelStyle.normal.textColor = Color.yellow;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.wordWrap = true;
            EditorGUILayout.LabelField("NOTE: Don't forget to hit the 'Save Settings' button at the bottom if you're using version control software like Git!", labelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            settings.zeroOutPosition = EditorGUILayout.Toggle("Spawn at (0,0,0)", settings.zeroOutPosition);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            settings.zeroOutRotation = EditorGUILayout.Toggle("Rotate to (0,0,0)", settings.zeroOutRotation);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            settings.spawnAtScreenCenter = EditorGUILayout.Toggle("Spawn at screen center", settings.spawnAtScreenCenter);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Placable Prefabs", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            for (int i = 0; i < settings.prefabs.Count; i++)
            {
                GUILayout.BeginVertical("box");

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Prefab Name:", EditorStyles.boldLabel);
                settings.prefabs[i].editorWindowName = EditorGUILayout.TextField(settings.prefabs[i].editorWindowName);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Prefab:");
                settings.prefabs[i].prefab = (GameObject)EditorGUILayout.ObjectField(settings.prefabs[i].prefab, typeof(GameObject), false);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Button Text Color:");
                settings.prefabs[i].editorButtonFontColor = EditorGUILayout.ColorField(settings.prefabs[i].editorButtonFontColor);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Button Font Style:");
                settings.prefabs[i].editorButtonFontStyle = (FontStyle)EditorGUILayout.EnumPopup(settings.prefabs[i].editorButtonFontStyle);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Button Color:");
                settings.prefabs[i].editorButtonColor = EditorGUILayout.ColorField(settings.prefabs[i].editorButtonColor);
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                if (GUILayout.Button("Remove"))
                {
                    RemovePrefab(i);
                    return;
                }

                GUILayout.EndVertical();

                GUILayout.Space(10);
            }

            if (GUILayout.Button("Add Prefab"))
            {
                AddPrefab();
                return;
            }

            GUILayout.Space(40);

            #region Save Asset Button
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Settings"))
            {
                SaveAssetsDirty();
            }

            GUILayout.EndHorizontal();
            #endregion
        }

        private void AddPrefab()
        {
            settings.prefabs.Add(new PrefabPlacerSettings.Prefab { editorButtonColor = Color.white });
        }

        private void RemovePrefab(int index)
        {
            settings.prefabs.RemoveAt(index);
        }

        private void SaveAssetsDirty()
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}