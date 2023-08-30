using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MemoryDisplayScene : MonoBehaviour
{
    public Text displayText;

    void Update()
    {
        float memoryInMB = System.GC.GetTotalMemory(false) / (1024.0f * 1024.0f);
        float fps = 1.0f / Time.deltaTime;

        displayText.text = $"Memory: {memoryInMB:F2} MB\nFPS: {fps:F2}";
    }
}
