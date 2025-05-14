using UnityEngine;

public class coursePageManager : MonoBehaviour
{
    public GameObject coursePage1;
    public GameObject coursePage2;
    public GameObject coursePage3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hideCoursePageWhenStart()
    {
        coursePage1.SetActive(false);
        coursePage2.SetActive(false);
        coursePage3.SetActive(false);
    }
}
