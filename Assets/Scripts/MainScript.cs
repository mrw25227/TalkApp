using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Events;
using static ApikeyLoader;

public class MainScript : MonoBehaviour
{
    const string STT_LANG_CODE = "cmn-Hant-TW";
    const string TTS_LANG_CODE = "zh-TW";
    WakeupManager wakeupManager;

    [SerializeField]
    InputField userInput;
    [SerializeField]
    InputField AIInput;

    [SerializeField]
    Button textInputBtn;
    [SerializeField]
    Button speechInputBtn;

    string modelName = "gpt-4o-mini";
    const string prompt = "�A�O�@�Ӵ��q�i���ܪ�AI�A�u��ϥΦ۵M�y���P�ϥΪ̹�ܡA���קK���ѵ{���X�B�S��Ÿ��εL�k��Ū�����e�C";
    private string apiKey = "";

    private void Awake()
    {
        CheckPermission();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Input.backButtonLeavesApp = true;

        textInputBtn.onClick.AddListener(OnTextInputBtnClick);
        speechInputBtn.onClick.AddListener(StartUserSpeaking);
        InitSpeechSystem();
        
        apiKey = ApikeyLoader.Instance.GetOpenAIkey();
        wakeupManager = GetComponent<WakeupManager>();
        wakeupManager.OnWakeWordDetectedEvent.AddListener(StartUserSpeaking);
    }
    private void OnDestroy()
    {
        textInputBtn.onClick.RemoveAllListeners();
        speechInputBtn.onClick.RemoveAllListeners();
    }

    void InitSpeechSystem()
    {
        TextToSpeech.Instance.Setting(TTS_LANG_CODE, 1, 1.5f);
        SpeechToText.Instance.Setting(STT_LANG_CODE);
        
        SpeechToText.Instance.onResultCallback = OnUserFinalSpeachResult;
        SpeechToText.Instance.onErrorCallback = OnUserSpeachError;
        SpeechToText.Instance.onPartialResultsCallback = OnUserPartialSpeechResult;

        TextToSpeech.Instance.onStartCallBack = OnAISpeakStart;
        TextToSpeech.Instance.onDoneCallback = OnAISpeakStop;
        //CheckPermission();
    }

    void CheckPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }


    void OnTextInputBtnClick()
    {
        wakeupManager?.StopWakeWord();
        string inputText = userInput.text;
        if(inputText.Length == 0)
        {
            StartAISpeaking("�A������A�ڨSť��?");
            return;
        }
        StartCoroutine(GetAIResponse(inputText, modelName));
        
    }

    

    public void StartAISpeaking(string message)
    {
        TextToSpeech.Instance.StartSpeak(message);
    }

    public void StopAISpeacking()
    {
        TextToSpeech.Instance.StopSpeak();
    }

    void OnAISpeakStart()
    {

    }

    void OnAISpeakStop()
    {
        wakeupManager.ResumeWakeWord();
    }

    public void StartUserSpeaking()
    {
        wakeupManager?.StopWakeWord();
        SpeechToText.Instance.StartRecording();
    }

    public void StopUserSpeaking()
    {
        SpeechToText.Instance.StopRecording();
    }

    void OnUserFinalSpeachResult(string result)
    {
        userInput.text = result;
        OnTextInputBtnClick();
    }
    void OnUserSpeachError(string result)
    {
        userInput.text = result;
        Debug.LogError("OnUserSpeachError: " + result);
        wakeupManager.ResumeWakeWord();
    }

    void OnUserPartialSpeechResult(string result)
    {
        userInput.text = result;
    }

    IEnumerator GetAIResponse(string message, string model)
    {
        AIInput.text = "��Ҥ�...";
        string apiUrl = "https://api.openai.com/v1/chat/completions";
        string requestMessage = prompt + " " + message;
        string requestBody = "{\"model\": \"" + model + "\", \"messages\": [{\"role\": \"user\", \"content\": \"" + requestMessage.Trim() + "\"}],\"max_tokens\": 500}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(responseText);
                Debug.Log("RESONSE: " + responseText);
                AIInput.text = response.choices[0].message.content;
#if UNITY_ANDROID && !UNITY_EDITOR
                StartAISpeaking(AIInput.text);
#endif
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                if(request.error.Contains("Too Many Requests"))
                {
                    AIInput.text = "�w�g�W�L����B�סA�еy��A�աI";
                }
                else
                {
                    AIInput.text = "AI �L�k�^���A�еy��A�աI";
                }
                wakeupManager.ResumeWakeWord();
            }
        }
    }
    [System.Serializable]
    private class OpenAIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string content;
    }
    
}
