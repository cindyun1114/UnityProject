using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    [Header("���U UI ����")]
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public Button registerButton;

    [Header("�n�J UI ����")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public Button loginButton;

    [Header("���� Panel")]
    public GameObject SigninPanel;   // ���U��
    public GameObject LoginPanel;    // �n�J��
    public GameObject HomePagePanel; // �D�� (�n�J���\�����)

    private string baseUrl = "http://127.0.0.1:8000";  // Flask ���A���B�椤

    void Start()
    {
        registerButton.onClick.AddListener(() => StartCoroutine(RegisterUser()));
        loginButton.onClick.AddListener(() => StartCoroutine(LoginUser()));

        // �T�O�i�J�C���ɥu��ܵn�J/���U����
        SigninPanel.SetActive(true);
        LoginPanel.SetActive(false);
        HomePagePanel.SetActive(false);
    }

    IEnumerator RegisterUser()
    {
        string username = registerUsernameInput.text;
        string email = registerEmailInput.text;
        string password = registerPasswordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("�Ҧ����һݶ�g");
            yield break;
        }

        string jsonData = $"{{\"username\": \"{username}\", \"email\": \"{email}\", \"password\": \"{password}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/register", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("���U���\�I�����n�J��");

                // **���õ��U���A��ܵn�J��**
                SigninPanel.SetActive(false);
                LoginPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("���U���ѡG" + request.downloadHandler.text);
            }
        }
    }

    IEnumerator LoginUser()
    {
        string email = loginEmailInput.text;
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("�Ҧ����һݶ�g");
            yield break;
        }

        string jsonData = $"{{\"email\": \"{email}\", \"password\": \"{password}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("�n�J���\�I�����D��");

                // **���õn�J���A��ܥD��**
                LoginPanel.SetActive(false);
                HomePagePanel.SetActive(true);
            }
            else
            {
                Debug.LogError("�n�J���ѡG" + request.downloadHandler.text);
            }
        }
    }
}