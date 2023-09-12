using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    public GameObject objectPrefab; // ������ ������Ʈ�� ������
    public int objectCount = 50; // ������ ������Ʈ�� ��
    public Camera mainCamera; // ���� ī�޶�

    public float minScale = 0.5f; // ������Ʈ�� �ּ� ũ��
    public float maxScale = 1.5f; // ������Ʈ�� �ִ� ũ��

    private void Start()
    {
        SpawnBackgroundObjects();
    }

    private void SpawnBackgroundObjects()
    {
        for (int i = 0; i < objectCount; i++)
        {
            // ����Ʈ ��ǥ���� ���� ��ġ ����
            Vector3 viewportPosition = new Vector3(Random.value, Random.value, Random.Range(5f, 10f));

            // ����Ʈ ��ǥ���� ��ġ�� ���� ��ǥ���� ��ġ�� ��ȯ
            Vector3 worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

            // ������Ʈ ����
            GameObject obj = Instantiate(objectPrefab, worldPosition, Quaternion.Euler(0, Random.Range(0, 360), 0));

            // �ұ�Ģ�� ũ��� ������Ʈ ũ�� ����
            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = new Vector3(scale, scale, scale);

            obj.transform.parent = transform;
        }
    }
}
