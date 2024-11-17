using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class PlayModeSceneSelector
{
    [MenuItem("Assets/Set this scene as playmode scene")]
    public static void SelectSceneForPlayMode()
    {
        string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        SceneAsset selectedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        if (selectedScene == null) Debug.LogError("Something went wrong, the selected scene was not loaded");
        EditorSceneManager.playModeStartScene = selectedScene;
    }

    [MenuItem("Assets/Set this scene as playmode scene", true, 0)]
    public static bool SelectSceneForPlayModeValidation()
    {
        return Selection.activeObject is SceneAsset;
    }

    [MenuItem("Assets/Set this scene as playmode scene from root")]
    public static void SelectSceneForPlayModeFromRoot()
    {
        SceneAsset root = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Root.unity");
        if (root == null) Debug.LogError("Something went wrong, the selected scene was not loaded");
        EditorSceneManager.playModeStartScene = root;
        EditorSceneData so = AssetDatabase.LoadAssetAtPath<EditorSceneData>("Assets/Scripts/StateController/EditorSceneData.asset");
        so.m_sceneName = Selection.activeObject.name;
    }

    [MenuItem("Assets/Set this scene as playmode scene from root", true, 0)]
    public static bool SelectSceneForPlayModeFromRootValidation()
    {
        return Selection.activeObject is SceneAsset;
    }
}
