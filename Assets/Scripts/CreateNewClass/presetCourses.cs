using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using TMPro;

public class presetCourses : MonoBehaviour
{
    private string apiUrl = "https://feynman-server.onrender.com";
    
    public TMP_Text classNameText;
    public TMP_Text classDateText;
    public GameObject previewPagePanel;
    public GameObject coursePagePanel;

    [Header("課程參數")]
    public string courseName;
    public int courseChoice;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startPresetCourse()
    {
        Debug.Log($"Starting the preset course: {courseName}");
        StartCoroutine(UploadPresetCourse());
    }

    IEnumerator UploadPresetCourse()
    {

        classNameText.text = courseName;
        classDateText.text = DateTime.Now.ToString("yyyy-MM-dd");

        WWWForm form = new WWWForm();
        form.AddField("class_name", courseName);
        form.AddField("user_id", PlayerPrefs.GetInt("UserID"));
        form.AddField("course_type", courseChoice);
        Debug.Log("User ID: " + PlayerPrefs.GetInt("UserID"));
        Debug.Log("Class Name: " + courseName);

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl + "/create", form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<newAssistantCreateResponse>(request.downloadHandler.text);
                PlayerPrefs.SetInt("Course_ID", response.course_id);
                PlayerPrefs.SetString("Assistant1_ID", response.assistant_id_1);
                PlayerPrefs.SetString("Assistant2_ID", response.assistant_id_2);
                PlayerPrefs.SetString("Thread1_ID", response.thread_id_1);
                PlayerPrefs.SetString("Thread2_ID", response.thread_id_2);
                Debug.Log("Upload Success: " + request.downloadHandler.text);
                Debug.Log("Assistant ID: " + response.assistant_id_1);
                Debug.Log("Thread ID: " + response.thread_id_1);
                Debug.Log("Assistant ID: " + response.assistant_id_2);
                Debug.Log("Thread ID: " + response.thread_id_2);

                if (previewPagePanel != null)
                {
                    previewPagePanel.SetActive(true);
                    coursePagePanel.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Upload Failed: " + request.error);
            }
        }
    }
}
