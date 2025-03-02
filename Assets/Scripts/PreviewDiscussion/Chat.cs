using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;

public class Chat : MonoBehaviour
{
    private string apiUrl = "https://feynman-server.onrender.com/chat";
    public TMP_InputField inputField;
    public GameObject playerMessagePrefab;
    public GameObject feyndoraMessagePrefab;
    public GameObject Content;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playerSendMessage()
    {
        string message = inputField.text;
        GameObject newMessage = Instantiate(playerMessagePrefab, Content.transform);
        newMessage.GetComponent<Message>().MessageText.text = message;
        StartCoroutine(SendMessageToChatGPT(message));

    }

    public IEnumerator SendMessageToChatGPT(string message)
    {
        // 構建請求的 JSON 資料
        string jsonData = JsonUtility.ToJson(new messageRequest
        {
            action = "message",
            message = message,
            assistant_id = PlayerPrefs.GetString("Assistant1_ID"),
            thread_id = PlayerPrefs.GetString("Thread1_ID")
        });

        // 發送 POST 請求
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData); //先轉換成位元組數組
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);  //放上資料
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 發送請求並等待回應
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 解析回覆 JSON 並顯示內容
                var response = JsonUtility.FromJson<messageResponse>(request.downloadHandler.text);
                GameObject newMessage = Instantiate(feyndoraMessagePrefab, Content.transform);// 顯示回覆
                newMessage.GetComponent<Message>().MessageText.text = response.message;
            }
            else
            {
                // 顯示錯誤
                GameObject newMessage = Instantiate(feyndoraMessagePrefab, Content.transform);// 顯示回覆
                newMessage.GetComponent<Message>().MessageText.text = "Error: " + request.error;
            }
        }
    }
}


[System.Serializable]
public class messageRequest
{
    public string action;
    public string assistant_id;
    public string thread_id;
    public string message;
}

[System.Serializable]//這個類是 可序列化的
public class messageResponse
{
    public string action;
    public string message;
}