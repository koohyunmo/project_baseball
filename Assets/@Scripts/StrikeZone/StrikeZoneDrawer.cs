using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class StrikeZoneDrawer : MonoBehaviour
{
    public GameObject targetObject; // BoundingBox를 그릴 대상 오브젝트
    private Camera cam;
    private LineRenderer lineRenderer;

    private void Start()
    {
        cam = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        DrawScaleWithLineRenderer();
    }

    private void DrawScaleWithLineRenderer()
    {
        Vector3 objectScale = targetObject.transform.localScale;

        // 게임 오브젝트의 스케일을 뷰포트 좌표로 변환
        Vector3 bottomLeft = new Vector3(0, 0, 10); // z 값은 카메라와의 거리로 적당한 값을 설정
        Vector3 topRight = new Vector3(objectScale.x, objectScale.y, 10);

        Vector3 worldBottomLeft = cam.ViewportToWorldPoint(bottomLeft);
        Vector3 worldTopRight = cam.ViewportToWorldPoint(topRight);

        Vector3[] positions = new Vector3[5];
        positions[0] = worldBottomLeft;
        positions[1] = new Vector3(worldTopRight.x, worldBottomLeft.y, 10);
        positions[2] = worldTopRight;
        positions[3] = new Vector3(worldBottomLeft.x, worldTopRight.y, 10);
        positions[4] = worldBottomLeft; // 사각형을 완성하기 위해 시작점으로 돌아옴

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    private void DrawBoundingBoxWithLineRenderer()
    {
        if (targetObject == null) return;

        Bounds bounds = targetObject.GetComponent<Renderer>().bounds;

        Vector3 bMin = bounds.min;
        Vector3 bMax = bounds.max;

        Vector3[] corners = new Vector3[8];
        corners[0] = new Vector3(bMin.x, bMin.y, bMin.z);
        corners[1] = new Vector3(bMax.x, bMin.y, bMin.z);
        corners[2] = new Vector3(bMax.x, bMax.y, bMin.z);
        corners[3] = new Vector3(bMin.x, bMax.y, bMin.z);
        corners[4] = new Vector3(bMin.x, bMin.y, bMax.z);
        corners[5] = new Vector3(bMax.x, bMin.y, bMax.z);
        corners[6] = new Vector3(bMax.x, bMax.y, bMax.z);
        corners[7] = new Vector3(bMin.x, bMax.y, bMax.z);

        Vector3[] viewportCorners = new Vector3[corners.Length];
        for (int i = 0; i < corners.Length; i++)
        {
            viewportCorners[i] = cam.WorldToViewportPoint(corners[i]);
        }

        // 라인 렌더러를 사용하여 BoundingBox의 모서리들을 연결
        Vector3[] positions = new Vector3[16];
        positions[0] = viewportCorners[0];
        positions[1] = viewportCorners[1];
        positions[2] = viewportCorners[2];
        positions[3] = viewportCorners[3];
        positions[4] = viewportCorners[0];
        positions[5] = viewportCorners[4];
        positions[6] = viewportCorners[5];
        positions[7] = viewportCorners[6];
        positions[8] = viewportCorners[7];
        positions[9] = viewportCorners[4];
        positions[10] = viewportCorners[7];
        positions[11] = viewportCorners[3];
        positions[12] = viewportCorners[2];
        positions[13] = viewportCorners[6];
        positions[14] = viewportCorners[5];
        positions[15] = viewportCorners[1];

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
