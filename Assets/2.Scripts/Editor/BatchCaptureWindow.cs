using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using static BatchCaptureWindow;
using Unity.VisualScripting;

public class BatchCaptureWindow : EditorWindow
{
    private List<GameObject> prefabs = new List<GameObject>();
    public List<GameObject> objectsToCapture = new List<GameObject>();
    public string savePath = "Assets/Captures/";
    public string modelFolderPath = "Assets/Prefabs/";  // �⺻ ���� ��� ����
    // �߰�: ���� ������ �ʵ�
    private Vector3 rotationOffset = Vector3.zero;
    private Vector3 scaleOffset = Vector3.one; // 1, 1, 1�� �⺻�� ����

    // ��ũ�ѹٿ� �ʿ��� ���� �߰�
    Vector2 scrollPosition;

    PaddingRatio paddingRatio = PaddingRatio.Twenty; // �⺻���� Twenty�� ����

    private enum Resolution
    {
        Low = 512,
        Medium = 1024,
        High = 2048,
        VeryHigh = 4096
    }

    public enum PaddingRatio
    {
        Zero = 0,
        Five = 5,
        Ten = 10,
        Twenty = 20,
        TwentyFive = 25,
        Fifty = 50
    }


    private Resolution selectedResolution = Resolution.Medium;


    [MenuItem("Tools/Batch Capture")]
    public static void ShowWindow()
    {
        GetWindow<BatchCaptureWindow>("Batch Capture");
    }
    void OnGUI()
    {
        GUILayout.Label("Batch Capture Settings", EditorStyles.boldLabel);

        // ��ũ�Ѻ� ����
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // �߰�: ���� ��ο� ���� ��θ� ������ �� �ִ� �ʵ�
        savePath = EditorGUILayout.TextField("[���� ����]Save Path:", savePath);
        modelFolderPath = EditorGUILayout.TextField("[�� ����]Folder Path:", modelFolderPath);

        // �߰�: ���� �������� ������ �� �ִ� �ʵ�
        rotationOffset = EditorGUILayout.Vector3Field("[�� ����]Rotation Offset:", rotationOffset);
        // OnGUI �Լ� ���ο� �̸� �߰��մϴ�.
        scaleOffset = EditorGUILayout.Vector3Field("[�� ũ��]Scale Offset", scaleOffset);

        // �ػ� ���� ��Ӵٿ� �޴�
        selectedResolution = (Resolution)EditorGUILayout.EnumPopup("[ĸó ȭ��]Capture Resolution", selectedResolution);

        GUILayout.Label("[�̱���]Padding Ratio:", EditorStyles.boldLabel);
        paddingRatio = (PaddingRatio)EditorGUILayout.EnumPopup("Ratio", paddingRatio);
        EditorGUILayout.Space();

        if (GUILayout.Button("[������ ������ �о����]Load Prefabs from Folder"))
        {
            LoadPrefabsFromFolder();
        }

        EditorGUILayout.Space();



        if (GUILayout.Button("[������ �� �߰�]Add Prefab"))
        {
            GameObject selectedPrefab = Selection.activeGameObject;
            if (selectedPrefab)
            {
                prefabs.Add(selectedPrefab);
            }
            else
            {
                Debug.LogWarning("No prefab selected!");
            }
        }

        EditorGUILayout.Space();

        for (int i = 0; i < prefabs.Count; i++)
        {
            GUILayout.BeginHorizontal();
            prefabs[i] = (GameObject)EditorGUILayout.ObjectField(prefabs[i], typeof(GameObject), false);
            if (GUILayout.Button("X"))
            {
                prefabs.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("[ĸó]Capture"))
        {
            foreach (GameObject prefab in prefabs)
            {
                CaptureObject(prefab);

            }
        }

        // ��ũ�Ѻ� ����
        EditorGUILayout.EndScrollView();
    }

    void LoadPrefabsFromFolder()
    {
        string[] prefabFiles = Directory.GetFiles(modelFolderPath, "*.prefab", SearchOption.AllDirectories);
        foreach (string path in prefabFiles)
        {
            GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (loadedPrefab && !prefabs.Contains(loadedPrefab))
            {
                prefabs.Add(loadedPrefab);
            }
        }
    }

    /*
    private void CaptureObject(GameObject obj)
    {

        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Instantiate the object and apply the rotation offset
        GameObject instance = Instantiate(obj);
        instance.transform.Rotate(rotationOffset);  // Apply the rotation offset

        Camera cam = new GameObject("CaptureCam").AddComponent<Camera>();
        cam.backgroundColor = Color.clear;
        cam.clearFlags = CameraClearFlags.SolidColor;

        RenderTexture rt = new RenderTexture((int)selectedResolution, (int)selectedResolution, 24);
        cam.targetTexture = rt;

        cam.transform.position = obj.transform.position + new Vector3(0, 0, -5);
        cam.transform.LookAt(obj.transform);

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;
        cam.Render();

        Texture2D image = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        image.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToPNG();
        string objSavePath = $"{savePath}{obj.name}.png";
        File.WriteAllBytes(objSavePath, bytes);

        RenderTexture.active = null; // Ȱ��ȭ�� RenderTexture�� null�� ����
        DestroyImmediate(cam.gameObject);
        RenderTexture.active = currentRT;

        DestroyImmediate(instance);  // Destroy the instantiated object
    }

    */


    private void CaptureObject(GameObject obj)
    {
        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Instantiate the object and apply the rotation and scale offset
        GameObject instance = Instantiate(obj);
        instance.transform.Rotate(rotationOffset);  // Apply the rotation offset
        instance.transform.localScale = Vector3.Scale(instance.transform.localScale, scaleOffset); // Apply the scale offset

        Camera cam = new GameObject("CaptureCam").AddComponent<Camera>();
        cam.backgroundColor = Color.clear;
        cam.clearFlags = CameraClearFlags.SolidColor;

        RenderTexture rt = new RenderTexture((int)selectedResolution, (int)selectedResolution, 24);
        cam.targetTexture = rt;

        // Bounding Box�� ����Ͽ� ī�޶� ��ġ ����
        Renderer rend = instance.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("The object does not have a Renderer component!");
            return;
        }
        Bounds bounds = rend.bounds;

        float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float cameraDistance = objectSize / (2 * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad));
        Vector3 cameraPosition = bounds.center - cam.transform.forward * cameraDistance;

        cam.transform.position = cameraPosition;
        cam.transform.LookAt(bounds.center);

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;
        cam.Render();

        Texture2D image = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        image.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToPNG();
        string objSavePath = $"{savePath}{obj.name}.png";
        File.WriteAllBytes(objSavePath, bytes);

        RenderTexture.active = null;
        DestroyImmediate(cam.gameObject);
        RenderTexture.active = currentRT;

        DestroyImmediate(instance);
    }



}
