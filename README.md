將android的語音功能與chatgpt進行串接
可透過android的語音辨識與chatgpt說話，並且chatgpt的回話內容也會透過android的語音合成說出來

使用需要額外在 assets/resource 底下建置 api_key.txt，裡面放oepnAI的key，需到https://platform.openai.comg 申請才能使用

透過關鍵字 喚醒語音辨識 功能則需要到 https://console.picovoice.ai/ 聲請他的api key(沒有此功能app也能正常運行)
api_key.txt的內部為json格式，大致如下：
{
  "openai_key":"sk-proj-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "porcupine_key":"xxxxxxxxxxxxxxxxxxxxxxxxxxx=="
}

目前定義的喚醒關鍵字為 hey google，hey siri 跟 你好電腦 ，picovoice本身不支援中文，所以最後一個是嘗試用英文接近發音組成的(Ni-hao-dein-nao)
＊注意picovoice免費版的key只能綁定一次，如果在電腦上作測試後，在手機上跑可能就會無法啟動
