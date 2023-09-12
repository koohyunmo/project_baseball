using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    public GameObject objectPrefab; // 생성할 오브젝트의 프리팹
    public int objectCount = 50; // 생성할 오브젝트의 수
    public Camera mainCamera; // 메인 카메라

    public float minScale = 0.5f; // 오브젝트의 최소 크기
    public float maxScale = 1.5f; // 오브젝트의 최대 크기

    private void Start()
    {
        SpawnBackgroundObjects();
    }

    private void SpawnBackgroundObjects()
    {
        for (int i = 0; i < objectCount; i++)
        {
            // 뷰포트 좌표계의 랜덤 위치 생성
            Vector3 viewportPosition = new Vector3(Random.value, Random.value, Random.Range(5f, 10f));

            // 뷰포트 좌표계의 위치를 월드 좌표계의 위치로 변환
            Vector3 worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

            // 오브젝트 생성
            GameObject obj = Instantiate(objectPrefab, worldPosition, Quaternion.Euler(0, Random.Range(0, 360), 0));

            // 불규칙한 크기로 오브젝트 크기 설정
            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = new Vector3(scale, scale, scale);

            obj.transform.parent = transform;
        }
    }
}
