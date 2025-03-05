using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleButtonColor : MonoBehaviour
{
    [Header("���s�Ѧ�")]
    public Button dayButton;   // ����s
    public Button monthButton; // ����s

    [Header("�C��]�w")]
    public Color activeButtonColor = new Color(0.5f, 0, 0.5f); // ����A�ҥΪ��A
    public Color inactiveButtonColor = Color.white;           // �զ�A�D�ҥΪ��A

    public Color activeTextColor = Color.white;   // �ҥΪ��A��r�C��
    public Color inactiveTextColor = Color.black;   // �D�ҥΪ��A��r�C��

    private void Start()
    {
        // ��l���A�G����s���ҥΡA����s���D�ҥ�
        SetButtonAndTextColors(dayButton, activeButtonColor, activeTextColor);
        SetButtonAndTextColors(monthButton, inactiveButtonColor, inactiveTextColor);

        // �����s�]�w�I���ƥ�
        dayButton.onClick.AddListener(OnDayButtonClicked);
        monthButton.onClick.AddListener(OnMonthButtonClicked);
    }

    void OnDayButtonClicked()
    {
        SetButtonAndTextColors(dayButton, activeButtonColor, activeTextColor);
        SetButtonAndTextColors(monthButton, inactiveButtonColor, inactiveTextColor);
        // ��L�����޿�b�o�̳B�z�K
    }

    void OnMonthButtonClicked()
    {
        SetButtonAndTextColors(monthButton, activeButtonColor, activeTextColor);
        SetButtonAndTextColors(dayButton, inactiveButtonColor, inactiveTextColor);
        // ��L�����޿�b�o�̳B�z�K
    }

    // ����k�P�ɧ�s���s���C��M��l���󤤪�TMP_Text�C��
    void SetButtonAndTextColors(Button button, Color buttonColor, Color textColor)
    {
        // ��s���s�I���C�� (ColorBlock)
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = buttonColor;
        colors.pressedColor = buttonColor * 0.9f; // ���t��ܫ��U���A
        colors.selectedColor = buttonColor;
        button.colors = colors;

        // �����s����TMP_Text�ç�s��r�C��
        TMP_Text textComponent = button.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.color = textColor;
        }
    }
}