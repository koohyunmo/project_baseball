using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Utils
{
	public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
	{
		T component = go.GetComponent<T>();
		if (component == null)
			component = go.AddComponent<T>();
		return component;
	}

	public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
	{
		Transform transform = FindChild<Transform>(go, name, recursive);
		if (transform == null)
			return null;

		return transform.gameObject;
	}

	public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
	{
		if (go == null)
			return null;

		if (recursive == false)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				Transform transform = go.transform.GetChild(i);
				if (string.IsNullOrEmpty(name) || transform.name == name)
				{
					T component = transform.GetComponent<T>();
					if (component != null)
						return component;
				}
			}
		}
		else
		{
			foreach (T component in go.GetComponentsInChildren<T>())
			{
				if (string.IsNullOrEmpty(name) || component.name == name)
					return component;
			}
		}

		return null;
	}

	public static Vector2 GenerateMonsterSpawnPosition(Vector2 characterPosition, float minDistance = 10.0f, float maxDistance = 20.0f)
	{
		float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
		float distance = Random.Range(minDistance, maxDistance);

		float xDist = Mathf.Cos(angle) * distance;
		float yDist = Mathf.Sin(angle) * distance;

		Vector2 spawnPosition = characterPosition + new Vector2(xDist, yDist);

		return spawnPosition;
	}

    public static (Vector3 Center, Vector3 TopLeft, Vector3 TopRight, Vector3 BottomLeft, Vector3 BottomRight) GetScreenRectanglePoints(Vector3 randomPoint,float height = 0.5f,float width = 0.5f)
    {
        // randomPoint를 기준으로 범위 설정
        float range = width; // 원하는 범위를 설정
        Vector3 center = randomPoint;
        Vector3 topLeft = new Vector3(center.x - range, center.y + height, center.z);
        Vector3 topRight = new Vector3(center.x + range, center.y + height, center.z);
        Vector3 bottomLeft = new Vector3(center.x - range, center.y - height, center.z);
        Vector3 bottomRight = new Vector3(center.x + range, center.y - height, center.z);

        // 각 포인트를 스크린 좌표로 변환
        Vector2 screenCenter = Camera.main.WorldToScreenPoint(center);
        Vector2 screenTopLeft = Camera.main.WorldToScreenPoint(topLeft);
        Vector2 screenTopRight = Camera.main.WorldToScreenPoint(topRight);
        Vector2 screenBottomLeft = Camera.main.WorldToScreenPoint(bottomLeft);
        Vector2 screenBottomRight = Camera.main.WorldToScreenPoint(bottomRight);

        // 스크린 좌표를 튜플로 반환
        return (screenCenter, screenTopLeft, screenTopRight, screenBottomLeft, screenBottomRight);
    }

    public static Color GetColor(League league)
	{
        switch (league)
        {
            case League.Bronze:
                return Define.Bronze;
            case League.Silver:
                return Define.Silver;
            case League.Gold:
                return Define.Gold; 
            case League.Platinum:
                return Define.Platinum; 
            case League.Diamond:
                return Define.Diamond; 
            case League.Master:
                return Define.Master; 

        }

		return Color.white;
    }

    public static string ColorToHex(Color color)
    {

        // 알파값을 1로 설정
        color.a = 1f;

        // 색상의 밝기 계산
        float brightness = 0.3f * color.r + 0.59f * color.g + 0.11f * color.b;

        if (brightness > 0.5f)
        {
            // 밝기가 높으면 글자의 색상을 어둡게 만들기
            color.r *= 0.7f;
            color.g *= 0.7f;
            color.b *= 0.7f;
        }
        else
        {
            // 밝기가 낮으면 글자의 색상을 밝게 만들기
            color.r = Mathf.Clamp(color.r * 1.5f, 0f, 1f);
            color.g = Mathf.Clamp(color.g * 1.5f, 0f, 1f);
            color.b = Mathf.Clamp(color.b * 1.5f, 0f, 1f);
        }

        return $"{Mathf.RoundToInt(color.r * 255):X2}{Mathf.RoundToInt(color.g * 255):X2}{Mathf.RoundToInt(color.b * 255):X2}{Mathf.RoundToInt(color.a * 255):X2}";
    }
}
