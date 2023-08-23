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
    public string modelFolderPath = "Assets/Prefabs/";  // 기본 폴더 경로 설정
    // 추가: 각도 오프셋 필드
    private Vector3 rotationOffset = Vector3.zero;
    private Vector3 scaleOffset = Vector3.one; // 1, 1, 1로 기본값 설정

    // 스크롤바에 필요한 변수 추가
    Vector2 scrollPosition;

    PaddingRatio paddingRatio = PaddingRatio.Twenty; // 기본값을 Twenty로 설정

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

        // 스크롤뷰 시작
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 추가: 저장 경로와 폴더 경로를 수정할 수 있는 필드
        savePath = EditorGUILayout.TextField("[저장 폴더]Save Path:", savePath);
        modelFolderPath = EditorGUILayout.TextField("[모델 폴더]Folder Path:", modelFolderPath);

        // 추가: 각도 오프셋을 수정할 수 있는 필드
        rotationOffset = EditorGUILayout.Vector3Field("[모델 각도]Rotation Offset:", rotationOffset);
        // OnGUI 함수 내부에 이를 추가합니다.
        scaleOffset = EditorGUILayout.Vector3Field("[모델 크기]Scale Offset", scaleOffset);

        // 해상도 설정 드롭다운 메뉴
        selectedResolution = (Resolution)EditorGUILayout.EnumPopup("[캡처 화질]Capture Resolution", selectedResolution);

        GUILayout.Label("[미구현]Padding Ratio:", EditorStyles.boldLabel);
        paddingRatio = (PaddingRatio)EditorGUILayout.EnumPopup("Ratio", paddingRatio);
        EditorGUILayout.Space();

        if (GUILayout.Button("[폴더에 모델파일 읽어오기]Load Prefabs from Folder"))
        {
            LoadPrefabsFromFolder();
        }

        EditorGUILayout.Space();



        if (GUILayout.Button("[선택한 모델 추가]Add Prefab"))
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

        if (GUILayout.Button("[캡처]Capture"))
        {
            foreach (GameObject prefab in prefabs)
            {
                CaptureObject(prefab);

            }
        }

        // 스크롤뷰 종료
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

        RenderTexture.active = null; // 활성화된 RenderTexture를 null로 설정
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

        // Bounding Box를 사용하여 카메라 위치 조절
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
