using System;
using UnityEngine;

public class ApikeyLoader
{
    static ApikeyLoader instance;
    private ApikeyJson apikeyJson;
    public static ApikeyLoader Instance {  
        get {
            if (instance == null)
            {
                instance = new ApikeyLoader();
            }
            return instance; 
        } 
    }
    ApikeyLoader()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("api_key");
        apikeyJson = JsonUtility.FromJson<ApikeyJson>(textAsset.text);
    }

    public string GetOpenAIkey()
    {
        return apikeyJson.openai_key;
    }

    public string GetPorcupineKey()
    {
        return apikeyJson.porcupine_key;
    }


}
[Serializable]
public class ApikeyJson
{
    public string openai_key;

    public string porcupine_key;

}