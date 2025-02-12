using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

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
    ClickHandler speechInputBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textInputBtn.onClick.AddListener(OnTextInputBtnClick);
        speechInputBtn.downEvent.AddListener(StartUserSpeaking);
        speechInputBtn.upEvent.AddListener(StopUserSpeaking);
        InitSpeechSystem(LANG_CODE);
    }

    private void OnDestroy()
    {
        textInputBtn.onClick.RemoveAllListeners();
        speechInputBtn.downEvent.RemoveAllListeners();
        speechInputBtn.upEvent.RemoveAllListeners();
    }

    void InitSpeechSystem(string code)
    {
        TextToSpeech.Instance.Setting(code, 1, 1);
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

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTextInputBtnClick()
    {
        string inputText = userInput.text;
        StartAISpeaking(inputText);
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
        Debug.LogError("OnUserFinalSpeachResult");
        StartAISpeaking(result);
    }
    void OnUserSpeachError(string result)
    {
        userInput.text = result;
        Debug.LogError("OnUserSpeachError");
        //StartAISpeaking(result);
    }

    void OnUserPartialSpeechResult(string result)
    {
        Debug.LogError("OnUserPartialSpeechResult: " + result);
        userInput.text = result;
    }

}
