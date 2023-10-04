using UnityEngine;

public class VibrationManager
{
    public void Vibrate(long milliseconds)
    {
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
