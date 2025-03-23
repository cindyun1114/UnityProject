using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    private LevelSelector levelSelector;
    [SerializeField] private Button levelButton;
    public void RegisterLevelSelector(LevelSelector levelSelector)
    {
        this.levelSelector = levelSelector;
        levelButton.onClick.AddListener(() =>
        {
            levelSelector.MoveLevelToBottom(int.Parse(gameObject.name));

        });
    }


}