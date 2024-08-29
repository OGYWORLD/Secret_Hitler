using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TGModernWorld
{
    [ExecuteInEditMode]
    public class TGLettersManager : MonoBehaviour
    {

        class LetterLine
        {
            public Transform LineTransform;
            public Transform BackTransform;
            public float LineSize;
        }

        [System.Serializable]
        public class LetterMaterialPreset
        {
            public string PresetName;
            public List<Material> Materials = new List<Material>();
        }

        [System.Serializable]
        public enum LettersTextDirection { Horizontal, Vertical };

        [System.Serializable]
        public class LetterManagerPreset
        {
            public string PresetName;
            public GameObject LettersPrefab;
            public GameObject LettersPanel;
            public GameObject LettersPanelEnd;
            public List<LetterMaterialPreset> MaterialPresets = new List<LetterMaterialPreset>();
        }



        public List<LetterManagerPreset> LetterPresets = new List<LetterManagerPreset>();
        public int LetterPresetID;
        public int MaterialPresetID;
        public int MaterialPanelPresetID;
        [Space]
        [TextArea]
        public string Text="TEXT";
        public LettersTextDirection TextDirection = LettersTextDirection.Horizontal;
        public TextAnchor TextAlignment = TextAnchor.MiddleCenter;
        [Range(0f,1f)]
        public float CharSpacing=0.2f;
        [Range(0f,1f)]
        public float LineSpacing = 0.1f;
        [Range(0f,2f)]
        public float SpaceSpacing = 1f;

        LetterManagerPreset letterPreset;
        List<LetterLine> letterLines= new List<LetterLine>();
        Bounds baseLetterBounds;
        Bounds basePanelBounds;
#if UNITY_EDITOR
        private void OnEnable()
        {
            if (PrefabUtility.IsPartOfPrefabInstance(transform))
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }

        void Start()
        {

        }

        void Update()
        {
        }

        public void ApplyChanges(bool all)
        {
            letterPreset = LetterPresets[LetterPresetID];
            if (letterLines.Count == 0 && Text != "")
                all = true;
            if (all)
            {
                // Removing old text
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
                UpdateLetters();
                UpdateMaterials();
            }
            UpdateLettersAlignment();
            UpdateLettersPanels();
        }


        void UpdateLetters()
        {
            // Cheking if given letter prefab is correct
            if (!letterPreset.LettersPrefab) return;
            MeshFilter filter = letterPreset.LettersPrefab.GetComponent<MeshFilter>();
            if (filter) {
                baseLetterBounds = filter.sharedMesh.bounds;
            }
            int index = letterPreset.LettersPrefab.name.IndexOf("_");
            if (index == -1) return;
            // Get letters base path
            string prefix = letterPreset.LettersPrefab.name.Substring(0, index)+"_";
            string path=AssetDatabase.GetAssetPath(letterPreset.LettersPrefab).Replace(letterPreset.LettersPrefab.name +".prefab","")+prefix;
            // Separate text to lines
            string[] txtLines=Text.ToUpper().Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            letterLines.Clear();
            // Create parent and add letters to each text line
            foreach (string txtLine in txtLines)
            {
                GameObject line = new GameObject("Line("+txtLine+")");
                line.transform.SetParent(transform);
                line.transform.localPosition = Vector3.zero;
                line.transform.localScale = Vector3.one;
                line.transform.localRotation = Quaternion.identity;
                CreateLettersLine(txtLine, path, line.transform);

                LetterLine newLine = new LetterLine();
                newLine.LineTransform = line.transform;
                if (letterPreset.LettersPanel && letterPreset.LettersPanelEnd)
                {
                    GameObject back = new GameObject("Back(" + txtLine + ")");
                    back.transform.SetParent(transform);
                    back.transform.localPosition = Vector3.zero;
                    back.transform.localScale = Vector3.one;
                    back.transform.localRotation = Quaternion.identity;
                    newLine.BackTransform = back.transform;
                    CreateLettersLinePanel(line.transform, back.transform);
                }
                letterLines.Add(newLine);
            }
        }

        void CreateLettersLinePanel(Transform lineParent, Transform backParent)
        {
            // Create start back panel
            GameObject startPanel = PrefabUtility.InstantiatePrefab(letterPreset.LettersPanelEnd) as GameObject;
            startPanel.transform.SetParent(backParent);
            startPanel.transform.localScale = Vector3.one;
            startPanel.transform.localRotation = Quaternion.identity;
            int cnt = Mathf.FloorToInt(lineParent.childCount / 2);
            for (int i = 0; i <cnt; i++)
            {
                GameObject midPanel = PrefabUtility.InstantiatePrefab(letterPreset.LettersPanel) as GameObject;
                midPanel.transform.SetParent(backParent);
                midPanel.transform.localScale = Vector3.one;
                midPanel.transform.localRotation = Quaternion.identity;
            }
            // Create end back panel
            GameObject endPanel = PrefabUtility.InstantiatePrefab(letterPreset.LettersPanelEnd) as GameObject;
            endPanel.transform.SetParent(backParent);
            endPanel.transform.localScale = new Vector3(-1,1,1);
            endPanel.transform.localRotation = Quaternion.identity;
        }

        void CreateLettersLine(string txt, string path, Transform lineParent)
        {
            for (int i = 0; i < txt.Length; i++)
            {
                // If char is a special symbols, replace name to key
                string letterChar =""+ txt[i];
                switch (letterChar)
                {
                    case "!":
                        letterChar = "S1";
                        break;
                    case "?":
                        letterChar = "S2";
                        break;
                    case "&":
                        letterChar = "S3";
                        break;
                    case "$":
                        letterChar = "S4";
                        break;
                    case "#":
                        letterChar = "S5";
                        break;
                    case "*":
                        letterChar = "S6";
                        break;

                }
                string letterPath = path + letterChar + ".prefab";
                var guid = AssetDatabase.AssetPathToGUID(letterPath);
                if (!string.IsNullOrEmpty(guid))
                {
                    Object letterPrefab = AssetDatabase.LoadAssetAtPath(letterPath, typeof(GameObject));
                    GameObject letter = PrefabUtility.InstantiatePrefab(letterPrefab) as GameObject;
                    letter.transform.SetParent(lineParent);
                    letter.transform.localPosition = Vector3.zero;
                    letter.transform.localScale = Vector3.one;
                    letter.transform.localRotation = Quaternion.identity;
                }
                else
                {         
                    // Create box if prefab not found
                    GameObject space = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    DestroyImmediate(space.GetComponent<Collider>());
                    space.transform.SetParent(lineParent);
                    space.transform.localScale = new Vector3(baseLetterBounds.size.x * SpaceSpacing, baseLetterBounds.size.y * SpaceSpacing, 0.1f);
                    space.transform.localRotation = Quaternion.identity;
                    space.transform.localPosition = Vector3.zero;
                    space.name = "CharNotFound";
                    // If char is space
                    if (txt[i] == ' ')
                    {
                        space.name = "Space";
                        space.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }
        }


        void UpdateLettersAlignment()
        {
            // Cheking if given letter prefab is correct
            if (!letterPreset.LettersPrefab) return;
            // Update letters in lines
            foreach (LetterLine line in letterLines)
            {
                line.LineSize = UpdateLineCharsAlignment(line.LineTransform);
            }
            switch (TextDirection)
            {
                case LettersTextDirection.Horizontal:
                    UpdateLettersHAlignment();
                    break;
                case LettersTextDirection.Vertical:
                    UpdateLettersVAlignment();
                    break;
            }
        }

        void UpdateLettersHAlignment()
        {
            // Apply alignment
            float h = letterLines.Count * baseLetterBounds.size.y + (letterLines.Count - 1) * LineSpacing * baseLetterBounds.size.y;
            for (int i=0;i<letterLines.Count;i++)
            {
                Vector3 pos = Vector3.zero;
                switch (TextAlignment)
                {
                    case TextAnchor.UpperLeft:
                        pos.y = -baseLetterBounds.size.y;
                        pos.x = 0;
                        break;
                    case TextAnchor.UpperCenter:
                        pos.y = -baseLetterBounds.size.y;
                        pos.x = letterLines[i].LineSize/2f;
                        break;
                    case TextAnchor.UpperRight:
                        pos.y = -baseLetterBounds.size.y;
                        pos.x = letterLines[i].LineSize;
                        break;
                    case TextAnchor.MiddleLeft:
                        pos.y = h / 2f- baseLetterBounds.size.y;
                        pos.x = 0;
                        break;
                    case TextAnchor.MiddleCenter:
                        pos.y = h / 2f - baseLetterBounds.size.y;
                        pos.x = letterLines[i].LineSize / 2f;
                        break;
                    case TextAnchor.MiddleRight:
                        pos.y = h / 2f - baseLetterBounds.size.y;
                        pos.x = letterLines[i].LineSize;
                        break;
                    case TextAnchor.LowerLeft:
                        pos.y = h- baseLetterBounds.size.y;
                        pos.x = 0;
                        break;
                    case TextAnchor.LowerCenter:
                        pos.y = h - baseLetterBounds.size.y;
                        pos.x = letterLines[i].LineSize / 2f;
                        break;
                    case TextAnchor.LowerRight:
                        pos.y = h - baseLetterBounds.size.y;
                        pos.x = letterLines[i].LineSize;
                        break;
                }
                pos.y = pos.y - baseLetterBounds.size.y * i - LineSpacing * baseLetterBounds.size.y * i;
                pos.z = -baseLetterBounds.center.z + baseLetterBounds.size.z / 2f;
                if (letterLines[i].BackTransform)
                {
                    pos.z = -basePanelBounds.center.z + basePanelBounds.size.z / 2f;
                    letterLines[i].BackTransform.localPosition = pos;
                    letterLines[i].BackTransform.localEulerAngles = new Vector3(0, 0, 0);
                    pos.z = basePanelBounds.size.z - baseLetterBounds.center.z + baseLetterBounds.size.z / 2f;
                    letterLines[i].LineTransform.localPosition = pos; 
                }
                else
                    letterLines[i].LineTransform.localPosition = pos;
            }
        }



        void UpdateLettersVAlignment()
        {
            // Apply alignment
            float h = (letterLines.Count-1) * baseLetterBounds.size.x + (letterLines.Count - 1) * LineSpacing * baseLetterBounds.size.x*2f;
            for (int i = 0; i < letterLines.Count; i++)
            {
                Vector3 pos = Vector3.zero;
                switch (TextAlignment)
                {
                    case TextAnchor.UpperLeft:
                        pos.y = 0;
                        pos.x = -baseLetterBounds.size.x/2f;
                        break;
                    case TextAnchor.UpperCenter:
                        pos.y = 0;
                        pos.x = h/2f;
                        break;
                    case TextAnchor.UpperRight:
                        pos.y = 0;
                        pos.x = h+ baseLetterBounds.size.x / 2f;
                        break;
                    case TextAnchor.MiddleLeft:
                        pos.y = letterLines[i].LineSize / 2;
                        pos.x = -baseLetterBounds.size.x / 2f;
                        break;
                    case TextAnchor.MiddleCenter:
                        pos.y = letterLines[i].LineSize / 2;
                        pos.x = h / 2f;
                        break;
                    case TextAnchor.MiddleRight:
                        pos.y = letterLines[i].LineSize / 2;
                        pos.x = h + baseLetterBounds.size.x / 2f;
                        break;
                    case TextAnchor.LowerLeft:
                        pos.y = letterLines[i].LineSize;
                        pos.x = -baseLetterBounds.size.x / 2f;
                        break;
                    case TextAnchor.LowerCenter:
                        pos.y = letterLines[i].LineSize;
                        pos.x = h / 2f;
                        break;
                    case TextAnchor.LowerRight:
                        pos.y = letterLines[i].LineSize;
                        pos.x = h + baseLetterBounds.size.x / 2f;
                        break;
                }
                pos.x = pos.x - baseLetterBounds.size.x * i - LineSpacing * baseLetterBounds.size.x *2f * i;
                pos.z = -baseLetterBounds.center.z + baseLetterBounds.size.z / 2f;
                if (letterLines[i].BackTransform)
                {
                    pos.z = basePanelBounds.size.z - baseLetterBounds.center.z + baseLetterBounds.size.z / 2f;
                    letterLines[i].LineTransform.localPosition = pos;

                    pos.z = -basePanelBounds.center.z + basePanelBounds.size.z / 2f;
                    pos.x = pos.x -baseLetterBounds.size.x/2f + basePanelBounds.size.y / 2f;
                    letterLines[i].BackTransform.localPosition = pos;
                    letterLines[i].BackTransform.localEulerAngles = new Vector3(0,0,90);
                }
                else
                    letterLines[i].LineTransform.localPosition = pos;
            }
        }

        float UpdateLineCharsAlignment(Transform lineChars)
        {
            if (!lineChars) return 0;
            float pos = 0f;
            float size = 0;
            float letterSize = 0;
            foreach (Transform letter in lineChars)
            {
                // Get Mesh and it's local bounds
                MeshFilter filter = letter.GetComponent<MeshFilter>();
                if (filter != null) {
                    Bounds letterBounds = filter.sharedMesh.bounds;
                    float letterWidth= letterBounds.size.x*letter.transform.localScale.x;
                    float letterHeight = letterBounds.size.y * letter.transform.localScale.y;
                    switch (TextDirection)
                    {
                        case LettersTextDirection.Horizontal:
                            letterSize=baseLetterBounds.size.x;
                            letter.localPosition = new Vector3(pos - letterWidth / 2f, 0, 0);
                            pos -= letterWidth + baseLetterBounds.size.x * CharSpacing;
                            size += letterWidth + baseLetterBounds.size.x * CharSpacing;
                            break;
                        case LettersTextDirection.Vertical:
                            letterSize = baseLetterBounds.size.y;
                            letter.localPosition = new Vector3(0, pos- letterBounds.size.y, 0);
                            pos -= letterHeight + letterSize * CharSpacing;
                            size += letterHeight + letterSize * CharSpacing;
                            break;
                    }
                }
            }
            if (size>0)
                size -= letterSize * CharSpacing;
            return size;
        }

        void UpdateLettersPanels()
        {
            if (!letterPreset.LettersPrefab || !letterPreset.LettersPanel || !letterPreset.LettersPanelEnd) return;
            foreach (LetterLine line in letterLines)
            {
                UpdateLettersPanel(line);
            }
        }

        void UpdateLettersPanel(LetterLine line)
        {
            if (!line.BackTransform) return;
            if (line.BackTransform.childCount < 2) return;
            Transform start = line.BackTransform.GetChild(0);
            Transform end = line.BackTransform.GetChild(line.BackTransform.childCount-1);
            float letterSize=0;
            // Get panel size
            MeshFilter filter = letterPreset.LettersPanel.GetComponent<MeshFilter>();
            basePanelBounds = filter.sharedMesh.bounds;
            switch (TextDirection)
            {
                case LettersTextDirection.Horizontal:
                    letterSize=baseLetterBounds.size.x;
                    break;
                case LettersTextDirection.Vertical:
                    letterSize = baseLetterBounds.size.y;
                    break;
            }
            float posY= baseLetterBounds.size.y / 2f - basePanelBounds.size.y / 2f;
            // Start end panels
            start.localPosition = new Vector3(-letterSize / 2f, posY, 0);
            end.localPosition = new Vector3(-line.LineSize + letterSize / 2f,posY, 0);
            // Mid panels
            if (line.BackTransform.childCount < 3) return;
            float scale = ((line.LineSize - letterSize) / basePanelBounds.size.x) / (line.BackTransform.childCount-2);
            float step = (line.LineSize - letterSize) / (line.BackTransform.childCount - 2);
            for (int i=0;i< line.BackTransform.childCount-2;i++)
            {
                Transform panel = line.BackTransform.GetChild(i+1);
                panel.localPosition = new Vector3((-letterSize / 2f)-step*i, posY, 0);
                panel.localScale = new Vector3(scale,1,1);
            }
        }


        public void UpdateMaterials()
        {
            if (!letterPreset.LettersPrefab) return;
            foreach (LetterLine line in letterLines)
            {
                UpdateLineMaterials(line.LineTransform,letterPreset.MaterialPresets[MaterialPresetID].Materials);
                if (letterPreset.LettersPanel && letterPreset.LettersPanelEnd)
                    UpdateLineMaterials(line.BackTransform, letterPreset.MaterialPresets[MaterialPanelPresetID].Materials);
            }
        }


        void UpdateLineMaterials(Transform line, List<Material> materials)
        {
            foreach (Transform child in line)
            {
                Renderer mRenderer = child.GetComponent<MeshRenderer>();
                if (mRenderer)
                {
                    if (mRenderer.sharedMaterials.Length==materials.Count)
                    {
                        mRenderer.sharedMaterials = materials.ToArray();
                    }
                }
            }
        }
#endif
    }


}
