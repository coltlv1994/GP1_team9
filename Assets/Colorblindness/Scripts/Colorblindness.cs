using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;

namespace SOHNE.Accessibility.Colorblindness
{
    public enum ColorblindTypes
    {
        Normal = 0,
        Protanopia,
        Protanomaly,
        Deuteranopia,
        Deuteranomaly,
        Tritanopia,
        Tritanomaly,
        Achromatopsia,
        Achromatomaly,
    }

    public class Colorblindness : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown _dropdown;

        Volume[] volumes;
        VolumeComponent lastFilter;

        int maxType;
        int _currentType = 0;
        int currentType
        {
            get => _currentType;

            set
            {
                if (_currentType > maxType) _currentType = 0;
                else _currentType = value;
            }
        }

        void SearchVolumes() => volumes = GameObject.FindObjectsOfType<Volume>();

        #region Enable/Disable
        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        #endregion

        public static Colorblindness Instance { get; private set; }

        private static void OnScriptsReloaded()
        {
#if !RENDERPIPELINE
            Debug.LogError("There is no type of <b>SRP</b> included in this project.");
#endif
        }

        void Awake()
        {
            PopulateList();

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            maxType = (int) System.Enum.GetValues(typeof(ColorblindTypes)).Cast<ColorblindTypes>().Last();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SearchVolumes();

            if (volumes == null || volumes.Length <= 0) return;

            Change(-1);
        }

        void Start()
        {
            if (PlayerPrefs.HasKey("Accessibility.ColorblindType"))
                currentType = PlayerPrefs.GetInt("Accessibility.ColorblindType");
            else
                PlayerPrefs.SetInt("Accessibility.ColorblindType", 0);

            SearchVolumes();
            StartCoroutine(ApplyFilter());
        }

        void Update()
        {
            
        }

        public void Change(int filterIndex = -1)
        {
            filterIndex = filterIndex <= -1 ? PlayerPrefs.GetInt("Accessibility.ColorblindType") : filterIndex;
            currentType = Mathf.Clamp(filterIndex, 0, maxType);
            StartCoroutine(ApplyFilter());
        }

        public void InitChange()
        {
            if (volumes == null) return;
#if UNITY_EDITOR
            Debug.Log($"Color changed to <b>{(ColorblindTypes)currentType} {currentType}</b>/{maxType}");
#endif

            StartCoroutine(ApplyFilter());

            PlayerPrefs.SetInt("Accessibility.ColorblindType", currentType);
            currentType++;
        }

        IEnumerator ApplyFilter()
        {
            currentType = _dropdown.value;

            ResourceRequest loadRequest = Resources.LoadAsync<VolumeProfile>($"Colorblind/{(ColorblindTypes)currentType}");

            do yield return null; while (!loadRequest.isDone);

            var filter = loadRequest.asset as VolumeProfile;

            if (filter == null)
            {
                Debug.LogError("An error has occured! Please, report");
                yield break;
            }

            if (lastFilter != null)
            {
                foreach (var volume in volumes)
                {
                    volume.profile.components.Remove(lastFilter);

                    foreach (var component in filter.components)
                        volume.profile.components.Add(component);
                }
            }

            lastFilter = filter.components[0];
        }

        //Added stuff (not in the stolen script)

        void PopulateList()
        {
            _dropdown.ClearOptions();
            string[] enumNames = Enum.GetNames(typeof(ColorblindTypes));
            List<string> names = new List<string>(enumNames);
            
            _dropdown.AddOptions(names);
        }

        public void DropdownChange()
        {
            currentType = _dropdown.value;
            StartCoroutine(ApplyFilter());
            Debug.Log($"Color changed to <b>{(ColorblindTypes)currentType} {currentType}</b>/{maxType}");
        }
    }
}