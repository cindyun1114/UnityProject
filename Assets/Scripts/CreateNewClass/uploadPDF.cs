using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using TMPro;

public class uploadPDF : MonoBehaviour
{
    private string apiUrl = "https://feynman-server.onrender.com";

    public TMP_InputField classNameInputField;
    public GameObject previewDiscussionPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenFileBrowser()
    {
        if (NativeFilePicker.IsFilePickerBusy())
            return;

        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                Debug.Log("Selected PDF: " + path);
                StartCoroutine(UploadPDF(path));
            }
        }, new string[] { "application/pdf" });
    }

    IEnumerator UploadPDF(string filePath)
    {
        string fileName = Uri.EscapeDataString(Path.GetFileName(filePath));

        byte[] fileData = File.ReadAllBytes(filePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName, "application/pdf");
        form.AddField("class_name", classNameInputField.text);
        form.AddField("user_id", PlayerPrefs.GetInt("UserID"));
        Debug.Log("User ID: " + PlayerPrefs.GetInt("UserID"));
        Debug.Log("Class Name: " + classNameInputField.text);

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl + "/create", form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<newAssistantCreateResponse>(request.downloadHandler.text);
                PlayerPrefs.SetString("Assistant1_ID", response.assistant_id_1);
                PlayerPrefs.SetString("Assistant2_ID", response.assistant_id_2);
                PlayerPrefs.SetString("Thread1_ID", response.thread_id_1);
                PlayerPrefs.SetString("Thread2_ID", response.thread_id_2);
                Debug.Log("Upload Success: " + request.downloadHandler.text);
                Debug.Log("Assistant ID: " + response.assistant_id_1);
                Debug.Log("Thread ID: " + response.thread_id_1);
                Debug.Log("Assistant ID: " + response.assistant_id_2);
                Debug.Log("Thread ID: " + response.thread_id_2);

                if (previewDiscussionPanel != null)
                {
                    previewDiscussionPanel.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Upload Failed: " + request.error);
            }
        }
    }
   
}

[System.Serializable]//這個類是 可序列化的
public class newAssistantCreateResponse
{
    public string action;
    public string assistant_id_1;
    public string thread_id_1;
    public string assistant_id_2;
    public string thread_id_2;
}
