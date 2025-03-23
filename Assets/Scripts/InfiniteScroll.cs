using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewPortTransform;
    public RectTransform contentPanelTransform;
    public HorizontalLayoutGroup HLG;

    public RectTransform[] ItemList;

    // Start is called before the first frame update
    void Start()
    {
        int ItemsToAdd = Mathf.CeilToInt(viewPortTransform.rect.width / (ItemList[0].rect.width + HLG.spacing));

        // 向右新增元素
        for (int i = 0; i < ItemsToAdd; i++)
        {
            RectTransform RT = Instantiate(ItemList[i % ItemList.Length], contentPanelTransform);
            RT.SetAsLastSibling();
        }

        // 向左新增元素
        for (int i = 0; i < ItemsToAdd; i++)  // <- 修正這裡
        {
            int num = ItemList.Length - i - 1;
            while (num < 0)
            {
                num += ItemList.Length;
            }

            RectTransform RT = Instantiate(ItemList[num], contentPanelTransform);
            RT.SetAsFirstSibling();
        } // <- 這裡補上缺少的大括號
        contentPanelTransform.localPosition = new Vector3((0 - (ItemList[0].rect.width + HLG.spacing)* ItemsToAdd),
            contentPanelTransform.localPosition.y,
            contentPanelTransform.localPosition.z);
    } // <- 這裡關閉 Start 方法

    // Update is called once per frame
    void Update()
    {
        if (contentPanelTransform.localPosition.x > 0)
        {
            Canvas.ForceUpdateCanvases();
            contentPanelTransform.localPosition -= new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
        }
        if (contentPanelTransform.localPosition.x < 0- (ItemList.Length * (ItemList[0].rect.width + HLG.spacing)))   
        {
            Canvas.ForceUpdateCanvases();
            contentPanelTransform.localPosition -= new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
        }

    }
}
