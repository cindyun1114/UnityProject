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
        // �c�ؽШD�� JSON ���
        string jsonData = JsonUtility.ToJson(new messageRequest
        {
            action = "message",
            message = message,
            assistant_id = PlayerPrefs.GetString("Assistant1_ID"),
            thread_id = PlayerPrefs.GetString("Thread1_ID")
        });

        // �o�e POST �ШD
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData); //���ഫ���줸�ռƲ�
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);  //��W���
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // �o�e�ШD�õ��ݦ^��
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // �ѪR�^�� JSON ����ܤ��e
                var response = JsonUtility.FromJson<messageResponse>(request.downloadHandler.text);
                GameObject newMessage = Instantiate(feyndoraMessagePrefab, Content.transform);// ��ܦ^��
                newMessage.GetComponent<Message>().MessageText.text = response.message;
            }
            else
            {
                // ��ܿ��~
                GameObject newMessage = Instantiate(feyndoraMessagePrefab, Content.transform);// ��ܦ^��
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

[System.Serializable]//�o�����O �i�ǦC�ƪ�
public class messageResponse
{
    public string action;
    public string message;
}