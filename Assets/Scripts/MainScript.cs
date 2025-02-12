using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    InputField userInput;
    [SerializeField]
    InputField AIInput;

    [SerializeField]
    Button textInputBtn;
    [SerializeField]
    Button speechInputBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textInputBtn.onClick.AddListener(OnTextInputBtnClick);
        speechInputBtn.onClick.AddListener(OnSpeechInputBtnClick);
    }

    private void OnDestroy()
    {
        textInputBtn.onClick.RemoveAllListeners();
        speechInputBtn.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTextInputBtnClick()
    {
        string inputText = userInput.text;
    }

    void OnSpeechInputBtnClick()
    {

    }

}
