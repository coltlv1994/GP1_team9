using Grid;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Dialogue;
using System.Web;
using System.Collections.Generic;

//[CustomEditor(typeof(Dialogue.Dialogue))]
public class DialogueInspector : Editor
{
    public class DictionaryEntry
    {
        public string key;
        public List<string> line;

        public DictionaryEntry(string p_key, List<string> p_line)
        {
            key = p_key;
            line = p_line;
        }
    }

    MultiColumnListView m_listView;
    List<DictionaryEntry> entries = new List<DictionaryEntry>();

    public override VisualElement CreateInspectorGUI()
    {
        entries.Add(new DictionaryEntry("1", new List<string>()));

        // Create a new VisualElement to be the root of our Inspector UI.
        VisualElement myInspector = new VisualElement();

        m_listView = new MultiColumnListView();        

        Column keyColumn = new Column();
        keyColumn.name = "key";
        keyColumn.title = "Key";
        keyColumn.stretchable = true;
        keyColumn.makeCell = () => new Label();
        keyColumn.bindCell = (VisualElement element, int index) =>
        {
            (element as Label).text = entries[index].key;
        };

        Column valueColumn = new Column();
        valueColumn.name = "value";
        valueColumn.title = "Value";
        valueColumn.stretchable = true;
        valueColumn.makeCell = () => new Label();
        valueColumn.bindCell = (VisualElement element, int index) =>
        {
            (element as Label).text = entries[index].line[0];
        };

        m_listView.columns.Add(keyColumn);
        m_listView.columns.Add(valueColumn);

        m_listView.itemsSource = entries;

        myInspector.Add(m_listView);
        myInspector.Add(new Label("End"));

        // Return the finished Inspector UI.
        return myInspector;
    }

    private void UpdateItems()
    {
        Dialogue.Dialogue targetDialogue = target as Dialogue.Dialogue;
        if (targetDialogue != null)
        {

        }
    }
}
