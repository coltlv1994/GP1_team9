using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Grid;

[CustomEditor(typeof(GridManager))]
public class GridCustomInspector : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        // Create a new VisualElement to be the root of our Inspector UI.
        VisualElement myInspector = new VisualElement();

        VisualElement defaultInspector = new VisualElement();
        InspectorElement.FillDefaultInspector(defaultInspector, new SerializedObject(target), this);

        myInspector.Add(defaultInspector);

        //Button to bake tile
        Button bakeTileButton = new Button()
        {
            text = "Bake tile"
        };
        GridManager gridManager = target as GridManager;
        bakeTileButton.clicked += () =>
        {
            gridManager.BakeTiles();
            SceneView.RepaintAll();
        };

        myInspector.Add(bakeTileButton);

        // Return the finished Inspector UI.
        return myInspector;
    }
}