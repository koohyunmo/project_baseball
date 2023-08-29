using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseSetUp : MonoBehaviour
{
	/*
	���� : ���̾�̽� ����
	��� : ���̾�̽� ����
	�÷��� : ����� / PC
	�� ���� : ���̾�̽� A/B�׽�Ʈ�� ���� �ֳθ�ƽ�� ����
	���� ��Ʃ�� : https://www.youtube.com/watch?v=y2GQx--69q8
	���� �ڷ� : https://firebase.google.com/docs/unity/setup#confirm-google-play-version
	�ʿ��� ��Ű�� : ���̾�̽� ���� 2�� ��Ű������ �ֳθ�ƽ��
	������ : �ǹ���
	�����������
	 */
	//���� ����
	private Firebase.FirebaseApp app;

	// Update is called once per frame
	void Start()
	{
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				// Create and hold a reference to your FirebaseApp,
				// where app is a Firebase.FirebaseApp property of your application class.
				app = Firebase.FirebaseApp.DefaultInstance;

				// Set a flag here to indicate whether Firebase is ready to use by your app.
			}
			else
			{
				UnityEngine.Debug.LogError(System.String.Format(
				  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});
	}
}
