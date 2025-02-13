using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Networking;
using System.Collections;

public class MainScript : MonoBehaviour
{
    const string LANG_CODE = "cmn-Hant-TW";

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Input.backButtonLeavesApp = true;

        textInputBtn.onClick.AddListener(OnTextInputBtnClick);
        speechInputBtn.onClick.AddListener(StartUserSpeaking);
        InitSpeechSystem(LANG_CODE);
        TextAsset textAsset = Resources.Load<TextAsset>("api_key");
        apiKey = textAsset.text;
    }
    private void OnDestroy()
    {
        textInputBtn.onClick.RemoveAllListeners();
        speechInputBtn.onClick.RemoveAllListeners();
    }

    void InitSpeechSystem(string code)
    {
        TextToSpeech.Instance.Setting(code, 1, 1.5f);
        SpeechToText.Instance.Setting(code);
        
        SpeechToText.Instance.onResultCallback = OnUserFinalSpeachResult;
        SpeechToText.Instance.onErrorCallback = OnUserSpeachError;
        SpeechToText.Instance.onPartialResultsCallback = OnUserPartialSpeechResult;

        TextToSpeech.Instance.onStartCallBack = OnAISpeakStart;
        TextToSpeech.Instance.onDoneCallback = OnAISpeakStop;
        CheckPermission();
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

    }

    public void StartUserSpeaking()
    {
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
