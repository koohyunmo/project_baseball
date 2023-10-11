using System;
using UnityEngine;

public class VibrationManager
{

    private bool isVibrate;
    string settingPath = "";

    public void Init()
    {
        settingPath = Application.persistentDataPath + "/SettingData.json";

        try
        {
         isVibrate = ES3.Load<bool>("isVibrate", settingPath);

        }catch(Exception e)
        {
            isVibrate = true;
            ES3.Save<bool>("isVibrate", isVibrate, settingPath);
        }

    }

    public bool GetVibrate()
    {
        return isVibrate;
    }

    public void ClickVibrateButton()
    {
        isVibrate = isVibrate == true ? isVibrate = false : isVibrate = true;

        ES3.Save<bool>("isVibrate", isVibrate,settingPath);

    }

    public void Vibrate(long milliseconds)
    {
        if (isVibrate == false)
            return;

        if(milliseconds.Equals(0))
        {
            return;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
                    {
                        vibrator.Call("vibrate", milliseconds);
                    }
                }
            }
        }
    }
}
