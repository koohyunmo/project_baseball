using UnityEngine;
using System.Collections.Generic;

public class BackgroundCreator : MonoBehaviour
{



    private List<GameObject> spawnedObjects = new List<GameObject>(); // ������ ������Ʈ ����Ʈ
    [Header("����ī�޶�")]
    public Camera mainCamera;
    [Header("������ ������Ʈ ���")]
    public List<GameObject> objectsToSpawn = new List<GameObject>(); // ������ ������Ʈ ����Ʈ
    [Header("������ ������Ʈ ����")]
    public int objectCount = 50; // ������ ������Ʈ�� ��
    [Header("������Ʈ ���� �θ�")]
    public GameObject root;
    [Header("������Ʈ ũ�� (Min~Max)")]
    public float minScale = 0.75f; // ������Ʈ�� �ּ� ũ��
    public float maxScale = 1.25f; // ������Ʈ�� �ִ� ũ��

    [Header("X ȸ�� ���� (Min~Max)")]
    public float xRoateMin = 0;
    public float xRoateMax = 0;

    [Header("Y ȸ�� ���� (Min~Max)")]
    public float yRoateMin = 0;
    public float yRoateMax = 0;

    [Header("Z ȸ�� ���� (Min~Max)")]
    public float zRoateMin = 0;
    public float zRoateMax = 0;

    // ��� ������Ʈ ���� �Լ�
    public void CreateBackground()
    {
        ClearBackground();

        for (int i = 0; i < objectCount; i++)
        {
            GameObject objToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];
            //Vector3 position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            //GameObject spawnedObj = Instantiate(objToSpawn, position, Quaternion.identity, transform);


            // ����Ʈ ��ǥ���� ���� ��ġ ����
            Vector3 viewportPosition = new Vector3(Random.value, Random.value, Random.Range(5f, 10f));

            // ����Ʈ ��ǥ���� ��ġ�� ���� ��ǥ���� ��ġ�� ��ȯ
            Vector3 worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

            // ������Ʈ ����
            GameObject obj = Instantiate(objToSpawn, worldPosition, Quaternion.Euler(Random.Range(xRoateMin, xRoateMax), Random.Range(yRoateMin, yRoateMax), Random.Range(zRoateMin, zRoateMax)));

            spawnedObjects.Add(obj);

            // �ұ�Ģ�� ũ��� ������Ʈ ũ�� ����
            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = new Vector3(scale, scale, scale);

            obj.transform.parent = root.transform;
        }
    }

    // ��� ������Ʈ ���� �Լ�
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

    // ��� ������Ʈ�� ���������� �����ϴ� �Լ�
    public void SaveBackground()
    {
        // ���⿡ ������ ���� ������ �߰��մϴ�.
    }
}
