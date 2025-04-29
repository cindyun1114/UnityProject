using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TMPro;


public class FinishPreview : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FinishPreviewCourse()
    {
        Debug.Log("Activate VR for course: " + PlayerPrefs.GetInt("Course_ID"));
        StartCoroutine(MarkCourseTeacherReady(PlayerPrefs.GetInt("Course_ID"), PlayerPrefs.GetInt("UserID")));
    }

    IEnumerator MarkCourseTeacherReady(int courseId, int userId)
    {
        string url = "https://feynman-server.onrender.com/activate_VR";
        Debug.Log("The course ID: " + courseId);
        Debug.Log($"The user's Id: {userId}");

        string jsonData = JsonUtility.ToJson(new activateVR
        {
            action = "activate_VR",
            course_id = courseId,
            user_id = userId
        });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Course marked as VR Ready!");
                Debug.Log("A teacher has been assigned!");
            }
            else
                Debug.LogError("Failed to mark course: " + request.error);
        }

    }
}


[System.Serializable]//�o�����O �i�ǦC�ƪ�
public class activateVR
{
    public string action;
    public int course_id;
    public int user_id;
}
