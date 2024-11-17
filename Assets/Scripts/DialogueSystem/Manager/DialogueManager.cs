using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private List<Dialogue> m_dialogues;
        [SerializeField] private float m_lettersPerSecond;
        [SerializeField] private TextMeshProUGUI m_textBox;
        [SerializeField] public Language m_language = Language.ENG;
        [SerializeField] private AudioSource m_source;
        private int m_letterIndex;
        private float m_PercentageIntoLine;
        private string m_line;
        public static DialogueManager GetInstance()
        {
            return m_instance;
        }

        public void MoveToNextLine()
        {
            if (m_currentDialogueIndex < 0 || m_currentDialogueIndex >= m_dialogues.Count) return;
            if (m_dialogues[m_currentDialogueIndex].HaveEnded()) return;
            StopAllCoroutines();
            m_source.Stop();

            m_dialogues[m_currentDialogueIndex].MoveToNext();

            m_textBox.text = "";

            StartCoroutine(DisplayLine());
        }

        public void ChangeLanguage(Language p_language)
        {
            m_textBox.text = "";
            m_language = p_language;

            if (m_letterIndex <= 0) return;

            m_line = m_dialogues[m_currentDialogueIndex].GetCurrentLine(m_language);
            m_letterIndex = (int)(m_PercentageIntoLine * m_line.Length);
            for (int i = 0; i < m_letterIndex; i++)
                m_textBox.text += m_line[i];
        }

        public void ChangeDialogue(int index)
        {
            if (index < 0 || index >= m_dialogues.Count) return;

            m_currentDialogueIndex = index;
        }
        
        public bool DialogueEnded()
        {
            return m_dialogues[m_currentDialogueIndex].HaveEnded();
        }

        public bool LineEnded()
        {
            return m_letterIndex == m_dialogues[m_currentDialogueIndex].GetCurrentLine(m_language).Length;
        }

        private int m_currentDialogueIndex;
        private static DialogueManager m_instance;

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
        }

        public void Init()
        {
            if (PlayerPrefs.HasKey("lang"))
                m_language = (Language)PlayerPrefs.GetInt("lang");
            else
                m_language = Language.ENG;
        }

        private void OnDestroy()
        {
            m_instance = null;
        }

        private IEnumerator DisplayLine()
        {
            m_line = m_dialogues[m_currentDialogueIndex].GetCurrentLine(m_language);
            m_source.PlayOneShot(m_dialogues[m_currentDialogueIndex].GetCurrentClip());
            m_letterIndex = 0;
            while (m_letterIndex < m_line.Length)
            {
                yield return new WaitForSeconds(1.0f / m_lettersPerSecond);

                m_textBox.text = m_textBox.text + m_line[m_letterIndex];
                m_PercentageIntoLine = (float)m_letterIndex / (m_line.Length - 1.0f);
                m_letterIndex++;
            }
        }
    }
}