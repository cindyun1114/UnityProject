using UnityEngine;

public class LeavePreview : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject exitPanel;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LeavePreviewPagePanel()
    {
        exitPanel.SetActive(false);
    }
}
