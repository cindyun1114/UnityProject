using UnityEngine;

public class PageManager : MonoBehaviour
{
    public GameObject[] panels;        // 存放所有页面的 Panel
    public GameObject navBarPanel;     // 导览列 Panel

    // 定义需要显示导航栏的页面
    private GameObject[] pagesWithNavBar;

    private void Start()
    {
        pagesWithNavBar = new GameObject[]
        {
            panels[0],  // HomePagePanel
            panels[1],  // RankPagePanel
            panels[2],  // UpdatePagePanel
            panels[3],  // CoursePagePanel
            panels[4]   // LotteryPagePanel
        };
    }

    public void ShowPage(GameObject pageToShow)
    {
        // 切换页面并隐藏不需要的 Panel
        foreach (var panel in panels)
        {
            panel.SetActive(panel == pageToShow);
        }

        // 控制导航栏是否可见
        navBarPanel.SetActive(ShouldShowNavBar(pageToShow));
    }

    private bool ShouldShowNavBar(GameObject page)
    {
        // 判断当前页面是否需要显示导航栏
        foreach (var navPage in pagesWithNavBar)
        {
            if (page == navPage)
            {
                return true;
            }
        }
        return false;
    }
}
