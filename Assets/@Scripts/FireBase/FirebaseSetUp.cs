using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseSetUp : MonoBehaviour
{
	/*
	제목 : 파이어베이스 붙임
	기능 : 파이어베이스 붙임
	플랫폼 : 모바일 / PC
	상세 내용 : 파이어베이스 A/B테스트를 위한 애널리틱스 연결
	참고 유튜브 : https://www.youtube.com/watch?v=y2GQx--69q8
	참고 자료 : https://firebase.google.com/docs/unity/setup#confirm-google-play-version
	필요한 패키지 : 파이어베이스 파일 2개 패키지에서 애널리틱스
	만든사람 : 권범수
	↓↓↓↓↓↓↓↓↓↓
	 */
	//세로 고정
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
