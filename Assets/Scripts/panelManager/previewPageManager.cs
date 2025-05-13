using UnityEngine;

public class previewPageManager : MonoBehaviour
{
    public GameObject filePanel;
    public GameObject discussPanel;
    public GameObject outlinePanel;
    public GameObject secondElement;



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        filePanel.SetActive(false);
        discussPanel.SetActive(true);
        outlinePanel.SetActive(false);
        //secondElement.SetActive(false);
    }
}
