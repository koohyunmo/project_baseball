using UnityEngine;
using System.Collections.Generic;

public class BackgroundCreator : MonoBehaviour
{



    private List<GameObject> spawnedObjects = new List<GameObject>(); // 생성된 오브젝트 리스트
    [Header("메인카메라")]
    public Camera mainCamera;
    [Header("생성할 오브젝트 목록")]
    public List<GameObject> objectsToSpawn = new List<GameObject>(); // 생성할 오브젝트 리스트
    [Header("생성할 오브젝트 갯수")]
    public int objectCount = 50; // 생성할 오브젝트의 수
    [Header("오브젝트 생성 부모")]
    public GameObject root;
    [Header("오브젝트 크기 (Min~Max)")]
    public float minScale = 0.75f; // 오브젝트의 최소 크기
    public float maxScale = 1.25f; // 오브젝트의 최대 크기

    [Header("X 회전 범위 (Min~Max)")]
    public float xRoateMin = 0;
    public float xRoateMax = 0;

    [Header("Y 회전 범위 (Min~Max)")]
    public float yRoateMin = 0;
    public float yRoateMax = 0;

    [Header("Z 회전 범위 (Min~Max)")]
    public float zRoateMin = 0;
    public float zRoateMax = 0;

    // 배경 오브젝트 생성 함수
    public void CreateBackground()
    {
        ClearBackground();

        for (int i = 0; i < objectCount; i++)
        {
            GameObject objToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];
            //Vector3 position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            //GameObject spawnedObj = Instantiate(objToSpawn, position, Quaternion.identity, transform);


            // 뷰포트 좌표계의 랜덤 위치 생성
            Vector3 viewportPosition = new Vector3(Random.value, Random.value, Random.Range(5f, 10f));

            // 뷰포트 좌표계의 위치를 월드 좌표계의 위치로 변환
            Vector3 worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

            // 오브젝트 생성
            GameObject obj = Instantiate(objToSpawn, worldPosition, Quaternion.Euler(Random.Range(xRoateMin, xRoateMax), Random.Range(yRoateMin, yRoateMax), Random.Range(zRoateMin, zRoateMax)));

            spawnedObjects.Add(obj);

            // 불규칙한 크기로 오브젝트 크기 설정
            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = new Vector3(scale, scale, scale);

            obj.transform.parent = root.transform;
        }
    }

    // 배경 오브젝트 삭제 함수
    public void ClearBackground()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            DestroyImmediate(obj.gameObject);
        }

        foreach (Transform child in root.transform)
            DestroyImmediate(child.gameObject);

        spawnedObjects.Clear();
    }

    // 배경 오브젝트를 프리팹으로 저장하는 함수
    public void SaveBackground()
    {
        // 여기에 프리팹 저장 로직을 추가합니다.
    }
}
