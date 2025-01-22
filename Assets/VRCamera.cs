using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public class VRCamera : MonoBehaviour
{
    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
#if UNITY_EDITOR
        SetGameViewToLandscape();
#endif
    }

#if UNITY_EDITOR
    private void SetGameViewToLandscape()
    {
        var gameView = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
        var content = new GUIContent("1920x1080 Landscape");
        var svModeEnum = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameViewSizeGroupType");
        var scriptableSingleton = typeof(ScriptableSingleton<>).MakeGenericType(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameViewSizes"));
        var instance = scriptableSingleton.GetProperty("instance").GetValue(null);
        var currentGroup = instance.GetType().GetProperty("currentGroup").GetValue(instance);
        var getTotalCount = currentGroup.GetType().GetMethod("GetTotalCount");
        var getGameViewSize = currentGroup.GetType().GetMethod("GetGameViewSize");
        var totalCount = (int)getTotalCount.Invoke(currentGroup, null);

        for (int i = 0; i < totalCount; i++)
        {
            var size = getGameViewSize.Invoke(currentGroup, new object[] { i });
            if (size.GetType().GetProperty("baseText").GetValue(size).ToString() == content.text)
            {
                var selectedSizeIndex = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView")
                    .GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                selectedSizeIndex.SetValue(gameView, i);
                break;
            }
        }
    }
#endif
}