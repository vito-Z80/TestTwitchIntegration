using System;
using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace UI.TopMenu
{
    public class WindowResolutionButtonUI : MonoBehaviour
    {
        const string Label = "Resolution: ";
        [SerializeField] TextAsset resolutionsJson;
        [SerializeField] TextMeshProUGUI label;
        TMP_Dropdown m_dropdown;

        AppSettingsData m_settings;

        List<WindowResolutionData> m_actualResolutions = new();

        void Awake()
        {
            m_settings = LocalStorage.GetSettings();
            m_dropdown = GetComponent<TMP_Dropdown>();
            RestoreWindowResolution();
            GenerateItems();
        }

        void GenerateItems()
        {
            m_dropdown.options.Clear();
            m_actualResolutions = GetActualWindowResolutionsByMonitorResolution();
            SetItems(m_actualResolutions);
            var selectedItemId = m_actualResolutions.FindIndex(data => data.width == m_settings.windowWidth && data.height == m_settings.windowHeight);
            m_dropdown.value = selectedItemId;
        }


        void SetItems(List<WindowResolutionData> actualResolutions)
        {
            foreach (var resolutionData in actualResolutions)
            {
                var line = $"{resolutionData.width}x{resolutionData.height}, {resolutionData.aspectRatio}, '{resolutionData.name}'";
                var od = new TMP_Dropdown.OptionData(line);
                m_dropdown.options.Add(od);
            }
        }

        List<WindowResolutionData> GetActualWindowResolutionsByMonitorResolution()
        {
            var resolutionsList = JsonConvert.DeserializeObject<ResolutionCollection>(resolutionsJson.text);
            var actualResolutions = new List<WindowResolutionData>();
            var monitorWidth = Display.main.systemWidth;
            var monitorHeight = Display.main.systemHeight;
            foreach (var resolutionData in resolutionsList.resolutions)
            {
                if (resolutionData.width > monitorWidth || resolutionData.height > monitorHeight) continue;
                actualResolutions.Add(resolutionData);
            }

            return actualResolutions;
        }


        void RestoreWindowResolution()
        {
            var width = m_settings.windowWidth;
            var height = m_settings.windowHeight;
            Core.Instance.SetResolution(width, height);
            ReplaceLabel(width, height);
        }


        public void UserSetWindowResolution()
        {
            var width = m_actualResolutions[m_dropdown.value].width;
            var height = m_actualResolutions[m_dropdown.value].height;
            m_settings.windowWidth = width;
            m_settings.windowHeight = height;
            Core.Instance.SetResolution(width, height);
            ReplaceLabel(width, height);
        }

        void ReplaceLabel(int width, int height)
        {
            label.text = $"{Label}{width}x{height}";
        }
    }

    [Serializable]
    public class ResolutionCollection
    {
        [SerializeField] public List<WindowResolutionData> resolutions;
    }
}