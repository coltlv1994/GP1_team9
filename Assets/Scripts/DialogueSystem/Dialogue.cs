using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public enum Language
    {
        None = -1,
        ENG,
        SE,
    }

    [Serializable]
    public class LocalizationDialogue
    {
        public Language m_key;
        public List<string> m_lines;

        public LocalizationDialogue(Language key, List<string> lines)
        {
            m_key = key;
            m_lines = lines;
        }
    }

    [Serializable, CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<LocalizationDialogue> m_localization = new List<LocalizationDialogue>() { new LocalizationDialogue(Language.ENG, new List<string>()), new LocalizationDialogue(Language.SE, new List<string>()) };
        [SerializeField] private List<AudioClip> m_audio = new List<AudioClip>();
        [NonSerialized] private int m_currentLineIndex = -1;

        public void MoveToNext()
        {
            m_currentLineIndex++;
        }

        public string GetCurrentLine(Language p_language)
        {
            LocalizationDialogue dialogue = m_localization.Find(element => { return (element.m_key == p_language); });
            if (m_currentLineIndex >= dialogue.m_lines.Count || m_currentLineIndex < 0) return null;
            return dialogue.m_lines[m_currentLineIndex];
        }

        public AudioClip GetCurrentClip()
        {
            if (m_currentLineIndex >= m_audio.Count || m_currentLineIndex < 0) return null;
            return m_audio[m_currentLineIndex];
        }

        public bool HaveEnded()
        {
            return m_currentLineIndex >= m_localization[0]?.m_lines.Count - 1;
        }
    }
}