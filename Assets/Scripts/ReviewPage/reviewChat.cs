using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.Networking;

public class reviewChat : MonoBehaviour
{
    private string apiUrl = "https://feynman-server.onrender.com/chat";
    public TMP_InputField inputField;
    public GameObject playerMessagePrefab;
    public GameObject feyndoraMessagePrefab;
    public GameObject Content;

    public GameObject ToCSecondMenu_1;
    public GameObject ToCTabPrefab;

    public VRLessonManager lessonManager;


    private string assistantID;
    private string threadID;
    private int course_id;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

        GameObject welcomeMessage1 = Instantiate(feyndoraMessagePrefab, Content.transform);
        welcomeMessage1.GetComponent<Message>().MessageText.text = "嗨! 這裡是複習空間";
        GameObject welcomeMessage2 = Instantiate(feyndoraMessagePrefab, Content.transform);
        welcomeMessage2.GetComponent<Message>().MessageText.text = "複習過程中遇到問題，歡迎向我提問";

        //StartCoroutine(InitializeReviewChat(course_id));
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
        StartCoroutine(SendMessageToChatGPT($"{message}"));
    }

    public IEnumerator SendMessageToChatGPT(string message)
    {

        string jsonData = JsonUtility.ToJson(new messageRequest
        {
            action = "message",
            message = message,
            assistant_id = PlayerPrefs.GetString("Assistant1_ID"),
            thread_id = PlayerPrefs.GetString("Thread1_ID")
        });


        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");


            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<messageResponse>(request.downloadHandler.text);
                Debug.Log("Review Reply:" + response.message);
                GameObject newMessage = Instantiate(feyndoraMessagePrefab, Content.transform);// ��ܦ^��
                newMessage.GetComponent<Message>().MessageText.text = response.message;
            }
            else
            {

                GameObject newMessage = Instantiate(feyndoraMessagePrefab, Content.transform);// ��ܦ^��
                newMessage.GetComponent<Message>().MessageText.text = "Error: " + request.error;
            }
        }
    }

    // IEnumerator InitializeReviewChat(int courseId)
    // {
    //     // 等 LoadAssistant 完成
    //     yield return StartCoroutine(lessonManager.LoadAssistant(courseId)); 

    //     // 等 LoadAssistant 結束後，再來拿 PlayerPrefs
    //     course_id = PlayerPrefs.GetInt("Course_ID");
    //     assistantID = PlayerPrefs.GetString("Assistant1_ID");
    //     threadID = PlayerPrefs.GetString("Thread1_ID");
        
    //     Debug.Log("ReviewChat 拿到 CourseID: " + course_id);
    //     Debug.Log("ReviewChat 拿到 assistantID: " + assistantID);
    //     Debug.Log("ReviewChat 拿到 threadID: " + threadID);
        

    //     //    這裡你就可以繼續做下一步，比如打開聊天室畫面
    // }
}
