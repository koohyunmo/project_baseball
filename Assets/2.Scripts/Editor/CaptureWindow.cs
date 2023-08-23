using UnityEngine;
using UnityEditor;
using System.IO;

public class CaptureWindow : EditorWindow
{
    public RenderTexture renderTexture;
    public string savePath = "Assets/Captured.png";

    [MenuItem("Tools/Capture RenderTexture")]
    public static void ShowWindow()
    {
        GetWindow<CaptureWindow>("Capture RenderTexture");
    }

    private void OnGUI()
    {
        renderTexture = (RenderTexture)EditorGUILayout.ObjectField("Render Texture", renderTexture, typeof(RenderTexture), false);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Capture"))
        {
            CaptureRenderTexture(renderTexture, savePath);
        }
    }

    private void CaptureRenderTexture(RenderTexture rt, string path)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D image = new Texture2D(rt.width, rt.height);
        image.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        RenderTexture.active = currentRT;

        AssetDatabase.Refresh(); // 에디터에서 바로 변경 사항을 확인하기 위해 에셋 데이터베이스를 새로고침
    }
}
