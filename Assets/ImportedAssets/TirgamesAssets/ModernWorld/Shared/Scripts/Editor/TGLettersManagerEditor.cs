using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TGModernWorld
{
    [CustomEditor(typeof(TGLettersManager))]
    public class TGLettersManagerEditor : Editor
    {
        TGLettersManager _target;
        bool editorEnabled=true;


        private void OnEnable()
        {
            editorEnabled = true;
            _target = (TGLettersManager)target;
        }


        string[] GetPresets()
        {

            List<string> presetNames = new List<string>();
            foreach (TGLettersManager.LetterManagerPreset preset in _target.LetterPresets)
            {
                string str = "(Not installed)";
                if (preset.LettersPrefab)
                    str = "";
                presetNames.Add(preset.PresetName+str);
            }
            return presetNames.ToArray();
        }

        string[] GetMaterialPresets()
        {
            List<string> presetNames = new List<string>();
            foreach (TGLettersManager.LetterMaterialPreset preset in _target.LetterPresets[_target.LetterPresetID].MaterialPresets)
            {
                string str = "(Not installed)";
                if (preset.Materials.Count>0)
                {
                    int cnt = 0;
                    foreach (Material mat in preset.Materials)                    
                        if (mat != null) cnt++;
                    if (cnt == preset.Materials.Count)
                        str = "";
                }
                presetNames.Add(preset.PresetName+str);
            }
            return presetNames.ToArray();
        }


        public override void OnInspectorGUI()
        {
            _target = (TGLettersManager)target;
            // HEADER
            GUIStyle style = new GUIStyle("HelpBox");           
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 14;
            style.normal.textColor = Color.cyan;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("Letters Manager 1.0", style);
            editorEnabled = EditorGUILayout.Toggle(editorEnabled, GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
            if (!editorEnabled)
            {
                DrawDefaultInspector();
                return;
            }
            // Letter Preset
            EditorGUI.BeginChangeCheck();
            _target.LetterPresetID=EditorGUILayout.Popup("LetterPreset", _target.LetterPresetID, GetPresets());
            if (EditorGUI.EndChangeCheck())
            {
                if (_target.LetterPresetID > _target.LetterPresets.Count - 1)
                    _target.LetterPresetID = _target.LetterPresets.Count - 1;
                _target.MaterialPresetID = 0;
                _target.MaterialPanelPresetID = 0;
                _target.ApplyChanges(true);
            }
            // Material presets
            EditorGUI.BeginChangeCheck();
            _target.MaterialPresetID = EditorGUILayout.Popup("MaterialPreset", _target.MaterialPresetID, GetMaterialPresets());
            if (_target.LetterPresets[_target.LetterPresetID].LettersPanel)
            {
                _target.MaterialPanelPresetID = EditorGUILayout.Popup("PanelMaterialPreset", _target.MaterialPanelPresetID, GetMaterialPresets());
            }
            if (EditorGUI.EndChangeCheck())
            {
                _target.UpdateMaterials();
            }
            // Text
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text", GUILayout.Width(EditorGUIUtility.labelWidth));
            _target.Text = EditorGUILayout.TextArea(_target.Text.ToUpper());            
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                _target.ApplyChanges(true);
            }
            // Alignment
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TextDirection"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TextAlignment"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CharSpacing"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LineSpacing"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                _target.ApplyChanges(false);
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SpaceSpacing"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                _target.ApplyChanges(true);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}