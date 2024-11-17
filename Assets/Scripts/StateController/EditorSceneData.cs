using UnityEngine;

[CreateAssetMenu(fileName = "EditorSceneData", menuName = "ScriptableObjects/EditorSceneData", order = 1)]
public class EditorSceneData : ScriptableObject
{
    [SerializeField] public string m_sceneName;
}
