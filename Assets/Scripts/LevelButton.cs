using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelButton : MonoBehaviour
{
    private LevelSelector levelSelector;
    [SerializeField] private Button levelButton;
    [SerializeField] private TextMeshProUGUI descriptionText; // ��ܴy�z�� UI

    private static Dictionary<string, string> levelDescriptions = new Dictionary<string, string>()
    {
        { "0", "�o�O�@�i�]�k�d��" },
        { "1", "�o�O�@���u�����Q��" },
        { "2", "�������t�z������" },
        { "3", "�֦��j�Ѵ��z�����" },
        { "4", "�i�����C�h" },
        { "5", "�ǻ������e�P�v" }
    };

    public void Setup(int levelIndex)
    {
        gameObject.name = levelIndex.ToString(); // �]�w�W�٬� Index�A��K�޲z
        if (levelDescriptions.ContainsKey(gameObject.name))
        {
            descriptionText.text = levelDescriptions[gameObject.name]; // �]�w�������y�z
        }
        else
        {
            descriptionText.text = "�������_�I��"; // �w�]�y�z
        }
    }

    public void RegisterLevelSelector(LevelSelector levelSelector)
    {
        this.levelSelector = levelSelector;
        levelButton.onClick.AddListener(() =>
        {
            levelSelector.MoveLevelToBottom(int.Parse(gameObject.name));
        });
    }
}
