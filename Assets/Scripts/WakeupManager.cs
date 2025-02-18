using Pv.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public class WakeupManager : MonoBehaviour
{
    public UnityEvent OnWakeWordDetectedEvent;
    private PorcupineManager porcupineManager;
    const string KEYWORD_FOLDER_PATH = "keyword_files/android";
    private string keywordPath;
    string accessKey = ""; // // AccessKey obtained from Picovoice Console (https://console.picovoice.ai/)
    
    void Start()
    {
        
#if UNITY_ANDROID && !UNITY_EDITOR
        InitializePorcupine(new List<string>()    {
                Path.Combine(Application.streamingAssetsPath, KEYWORD_FOLDER_PATH , "hey google_android.ppn"),
                Path.Combine(Application.streamingAssetsPath, KEYWORD_FOLDER_PATH , "hey siri_android.ppn"),
                Path.Combine(Application.streamingAssetsPath, KEYWORD_FOLDER_PATH , "Ni-hao-dein-nao_en_android_v3_0_0.ppn")
            });
#endif
        
        
    }

   

    private void InitializePorcupine(List<string> keywordPaths)
    {
        try
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
            accessKey = ApikeyLoader.Instance.GetPorcupineKey();
            porcupineManager = PorcupineManager.FromKeywordPaths(
                accessKey,
                keywordPaths,
                OnWakeWordDetected
            );

            porcupineManager.Start();
            Debug.Log(" Porcupine 已啟動，等待喚醒詞...");
        }
        catch (Exception ex)
        {
            Debug.LogError(" Porcupine 初始化失敗: " + ex.Message);
        }
    }
    private void OnWakeWordDetected(int keywordIndex)
    {
        Debug.Log("喚醒詞偵測到！ : " + keywordIndex);
        porcupineManager?.Stop();
        OnWakeWordDetectedEvent?.Invoke();
        // 這裡可以觸發 SpeechRecognizer 或其他動作
    }

    private void OnDestroy()
    {
        porcupineManager?.Stop();
        porcupineManager?.Delete();
    }

    public void ResumeWakeWord()
    {
        porcupineManager?.Start();
    }
    public void StopWakeWord()
    {
        porcupineManager?.Stop();
    }
}
