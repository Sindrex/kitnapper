using TMPro;
using UnityEngine;

namespace Assets.Features.Util.GlobalException
{
    public class GlobalExceptionManager : MonoBehaviour
    {
        public GameObject ExceptionWindow;
        public TMP_Text ExceptionText;

        //Singleton pattern
        public static GlobalExceptionManager Instance;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                Application.logMessageReceived += HandleException;
                ExceptionWindow.SetActive(false);
                return;
            }
            Destroy(this);
        }

        private void HandleException(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                Debug.Log($"Type: {type}, logString: {logString}, stackTrace: {stackTrace}");
                ExceptionText.text = logString + "\r\n" + stackTrace;
                ExceptionWindow.SetActive(true);
            }
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleException;
        }

        public void CloseWindow()
        {
            ExceptionWindow.SetActive(false);
        }
    }
}