using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Nreal
{
    public class PrintLogToScreen : MonoBehaviour
    {
        private StringBuilder m_LogBuilder = new StringBuilder(512);
        private Queue m_LogQueue = new Queue();
        private string m_LogStr;

        void OnEnable()
        {
            Application.logMessageReceived += LogCallback;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= LogCallback;
        }

        private void LogCallback(string logString, string stackTrace, LogType type)
        {
            m_LogBuilder.Clear();
            m_LogBuilder.AppendFormat("\n [{0}]: {1}", type.ToString(), logString);
            if (type == LogType.Exception || type == LogType.Warning)
            {
                m_LogBuilder.AppendFormat("\n {0}", stackTrace);
            }
            m_LogQueue.Enqueue(m_LogBuilder.ToString());

            m_LogStr = string.Empty;
            foreach (string mylog in m_LogQueue)
            {
                m_LogStr += mylog;
            }
        }

        void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 60;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(50, 50, Screen.width - 100, Screen.height - 100), m_LogStr, style);
        }
    }
}