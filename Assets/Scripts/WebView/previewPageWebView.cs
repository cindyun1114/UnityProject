using UnityEngine;

public class previewPageWebView : MonoBehaviour
{
    public GameObject filePanel; // 指到你的 UI Panel
    public RectTransform filePanelRect;
    private WebViewObject webViewObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowWebView()
    {
        // filePanel.SetActive(true); // 顯示 UI 介面
        Debug.Log("Open WebView");
        if (webViewObject == null)
        {
            webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();

            // 取得 panel 在螢幕的四個角的座標
            Vector3[] corners = new Vector3[4];
            filePanelRect.GetWorldCorners(corners);

            // 計算 margin（像素）
            float left = corners[0].x;
            float top = Screen.height - corners[1].y;
            float right = Screen.width - corners[2].x;
            float bottom = corners[0].y;

            webViewObject.Init();
            webViewObject.LoadURL("https://docs.google.com/gview?embedded=true&url=https://res.cloudinary.com/dni1rb4zi/raw/upload/v1746721184/feyndora/discrete_math_ch1.pdf");
            //AndroidManifest.xml 要加<uses-permission android:name="android.permission.INTERNET" />
            webViewObject.SetMargins((int)left, (int)top + 130, (int)right, (int)bottom + 230); // 調整到 panel 對應位置
            webViewObject.SetVisibility(true);
        }
        else
        {
            webViewObject.SetVisibility(true);
        }
    }

    public void HideWebView()
    {
        Debug.Log("Close WebView");
        //filePanel.SetActive(false); // 關掉 UI Panel
        if (webViewObject != null)
        {
            webViewObject.SetVisibility(false);
        }
    }

    public void ToCToFilePage()
    {
        Debug.Log("Show WebView");
        if(filePanel.activeInHierarchy)
        {
            webViewObject.SetVisibility(true);
        }
        
    }

    public void DestoryWebView()
    {
        Debug.Log("Destory WebView");
        Destroy(webViewObject.gameObject);  // 完全刪掉
        webViewObject = null;               // 清掉變數引用（可選但好習慣）
    }
}
