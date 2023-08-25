using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ViewportGizmoDrawer : MonoBehaviour
{
    private Camera cam;

    // 패딩과 색상을 지정
    private int[] paddings = { 10, 20, 30, 40, 50 };
    private Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta };

    private void OnDrawGizmos()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        for (int i = 0; i < paddings.Length; i++)
        {
            DrawViewportRectangle(paddings[i], colors[i]);
        }
    }

    private void DrawViewportRectangle(int padding, Color color)
    {
        Vector3[] viewportCorners = new Vector3[4];

        viewportCorners[0] = cam.ScreenToWorldPoint(new Vector3(padding, padding, cam.nearClipPlane));
        viewportCorners[1] = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth - padding, padding, cam.nearClipPlane));
        viewportCorners[2] = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth - padding, cam.pixelHeight - padding, cam.nearClipPlane));
        viewportCorners[3] = cam.ScreenToWorldPoint(new Vector3(padding, cam.pixelHeight - padding, cam.nearClipPlane));

        Gizmos.color = color;
        Gizmos.DrawLine(viewportCorners[0], viewportCorners[1]);
        Gizmos.DrawLine(viewportCorners[1], viewportCorners[2]);
        Gizmos.DrawLine(viewportCorners[2], viewportCorners[3]);
        Gizmos.DrawLine(viewportCorners[3], viewportCorners[0]);
    }
}
