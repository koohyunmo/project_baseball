using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ViewportLineDrawer : MonoBehaviour
{
#if UNITY_EDITOR
    private Camera cam;
    private LineRenderer lineRenderer;

    // 패딩을 퍼센트로 지정
    private float[] paddingsPercentage = { 0.10f, 0.20f, 0.30f, 0.40f, 0.50f }; // 10%, 20%, ...
    private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta };

    private void Start()
    {
        cam = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
        DrawViewportRectangle(paddingsPercentage[0], colors[0]); // 첫 번째 패딩과 색상으로 테스트
    }

    private void DrawViewportRectangle(float paddingPercentage, Color color)
    {
        float paddingX = cam.pixelWidth * paddingPercentage;
        float paddingY = cam.pixelHeight * paddingPercentage;

        Vector3[] worldCorners = new Vector3[5];

        worldCorners[0] = cam.ScreenToWorldPoint(new Vector3(paddingX, paddingY, cam.nearClipPlane));
        worldCorners[1] = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth - paddingX, paddingY, cam.nearClipPlane));
        worldCorners[2] = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth - paddingX, cam.pixelHeight - paddingY, cam.nearClipPlane));
        worldCorners[3] = cam.ScreenToWorldPoint(new Vector3(paddingX, cam.pixelHeight - paddingY, cam.nearClipPlane));
        worldCorners[4] = worldCorners[0]; // 다시 시작점으로 돌아가서 사각형을 완성

        if (lineRenderer.positionCount != worldCorners.Length)
        {
            lineRenderer.positionCount = worldCorners.Length;
        }

        lineRenderer.SetPositions(worldCorners);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
#endif
}
