using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI.File
{
    public class FileChooserUI : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown dropdown;
        [Header("Data")] [SerializeField] string streamingAssetsFilePath;
        [SerializeField] string[] extensions;

        string[] m_paths;

        void Start()
        {
            m_paths = LocalStorage.GetFilesByExtensions(streamingAssetsFilePath, extensions, true).ToArray();
            SetDropdownOptions(m_paths);
        }

        public void Init(ref string path)
        {
            if (m_paths == null)
            {
                m_paths = LocalStorage.GetFilesByExtensions(streamingAssetsFilePath, extensions, true).ToArray();
                SetDropdownOptions(m_paths);
            }

            if (path.Length == 0)
            {
                path = m_paths[0];
            }
            var p1 = path;
            var index = m_paths.ToList().FindIndex(p => p == p1);
            dropdown.value = index;
        }

        public string[] GetPaths() => m_paths;

        void SetDropdownOptions(IEnumerable<string> paths)
        {
            dropdown.ClearOptions();
            var fileNames = paths.Select(Path.GetFileName).ToList();
            dropdown.AddOptions(fileNames);
            dropdown.RefreshShownValue();
        }
    }
}