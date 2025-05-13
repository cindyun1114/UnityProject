using UnityEngine;

public class reviewPageManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject filePanel;
    public GameObject recordPanel;
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
        recordPanel.SetActive(true);
        discussPanel.SetActive(false);
        outlinePanel.SetActive(false);
        //secondElement.SetActive(false);
    }
}
