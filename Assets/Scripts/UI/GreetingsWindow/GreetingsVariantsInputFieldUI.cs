using TMPro;
using UnityEngine;

namespace UI.GreetingsWindow
{
    public class GreetingsVariantsInputFieldUI : MonoBehaviour
    {
    
        TMP_InputField m_inputField;


        void Start()
        {
            m_inputField = GetComponent<TMP_InputField>();
            m_inputField.text = string.Join(",", LocalStorage.GetGreetingsVariants());
        }

        public void SaveData(string data)
        {
            LocalStorage.SaveGreetingsVariants(data);
        }
        
    }
}