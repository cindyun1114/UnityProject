using UnityEngine;
using UnityEngine.UI;

public class CircleProgress : MonoBehaviour
{
    public Image progressImage;  // �e���i�ױ�
    public Text percentageText;  // ��ܼƭ�
    [Range(0, 1)] public float progress = 0f; // �i�ס]0~1�^

    void Update()
    {
        // ��s��R�i��
        progressImage.fillAmount = progress;

        // ��s��ܼƦr
        percentageText.text = (progress * 100).ToString("0") + "%";
    }

    // �]�w�i��
    public void SetProgress(float value)
    {
        progress = Mathf.Clamp01(value); // ����d�� 0~1
    }
}