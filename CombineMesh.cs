using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CombineMeshesEditorWindow : EditorWindow
{
    private List<GameObject> objectsToCombine = new List<GameObject>();
    private bool useMultimaterial = false;
    private const string linkURL = "https://www.linkedin.com/in/ibrahim-melih-do%C4%9Fan-633215178/";
    Color backGroundColor;
    string colorHex = "#000A84";
    [MenuItem("Tools/Mesh Combiner")]
    public static void ShowWindow()
    {
        CombineMeshesEditorWindow window = EditorWindow.GetWindow<CombineMeshesEditorWindow>();
        window.titleContent = new GUIContent("Mesh Combiner");
        window.Show();
    }

    private void OnGUI()
    {
        ColorUtility.TryParseHtmlString(colorHex, out backGroundColor);

        GUILayout.Label("Selected Objects to Combine:", EditorStyles.boldLabel);
        Rect backgroundRect = new Rect(0, 0, position.width, position.height);
        EditorGUI.DrawRect(backgroundRect, backGroundColor);

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < objectsToCombine.Count; i++)
        {
            objectsToCombine[i] = EditorGUILayout.ObjectField(objectsToCombine[i], typeof(GameObject), true) as GameObject;
        }
        GUILayout.Space(20);
        if (GUILayout.Button("Add Selected Object"))
        {
            AddSelectedObject();
        }
        GUILayout.Space(20);
        if (GUILayout.Button("Clear List"))
        {
            ClearObjectList();
        }

        EditorGUILayout.Space();
        useMultimaterial = EditorGUILayout.Toggle("Use Multimaterial", useMultimaterial);
        EditorGUILayout.Space();

        GUILayout.Space(20);
        if (GUILayout.Button("Combine Meshes"))
        {
            if (useMultimaterial)
            {
                MultimaterialCombineSelectedMeshes();
            }
            else
            {
                CombineSelectedMeshes();
            }
        }
        GUILayout.Space(20);
        if (GUILayout.Button("Contact Me-LinkedIn"))
        {
            Application.OpenURL(linkURL);
        }
    }

    private void AddSelectedObject()
    {
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject != null && !objectsToCombine.Contains(selectedObject))
        {
            objectsToCombine.Add(selectedObject);
        }
    }

    private void ClearObjectList()
    {
        objectsToCombine.Clear();
    }
        
    private void CombineSelectedMeshes()
    {
        GameObject combinedObject = new GameObject("CombinedMesh");
        combinedObject.transform.position = Vector3.zero;
        combinedObject.transform.rotation = Quaternion.identity;
        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        List<Material> materials = new List<Material>();

        foreach (GameObject obj in objectsToCombine)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilters.Add(meshFilter);
                materials.Add(obj.GetComponent<MeshRenderer>().sharedMaterial);
            }
        }
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Count];
        for (int i = 0; i < meshFilters.Count; i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, true, true);
        combinedMeshFilter.sharedMesh = combinedMesh;
        combinedMeshRenderer.sharedMaterials = materials.ToArray();
    }

    private void MultimaterialCombineSelectedMeshes()
    {
        GameObject combinedObject = new GameObject("CombinedMesh");
        combinedObject.transform.position = Vector3.zero;
        combinedObject.transform.rotation = Quaternion.identity;
        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        List<Material[]> materials = new List<Material[]>();

        foreach (GameObject obj in objectsToCombine)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilters.Add(meshFilter);
                materials.Add(obj.GetComponent<MeshRenderer>().sharedMaterials);
            }
        }
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Count];
        for (int i = 0; i < meshFilters.Count; i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, false, true);
        combinedMeshFilter.sharedMesh = combinedMesh;
        combinedMeshRenderer.sharedMaterials = CombineMaterials(materials);
    }
    private Material[] CombineMaterials(List<Material[]> materials)
    {
        List<Material> combinedMaterials = new List<Material>();

        foreach (Material[] materialArray in materials)
        {
            combinedMaterials.AddRange(materialArray);
        }
        return combinedMaterials.ToArray();
    }
}