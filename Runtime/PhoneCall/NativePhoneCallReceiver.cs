using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nreal.Natives
{
    public interface IPhoneCallListener
    {
        void OnIncomingCallStart(string number, long startDate);
        void OnOutgoingCallStart(string number, long startDate);
        void OnIncomingCallEnd(string number, long startDate, long endDate);
        void OnOutgoingCallEnd(string number, long startDate, long endDate);
        void OnMissCall(string number, long startDate);
    }

    public class NativePhoneCallReceiver
    {
        public const string NativeClassName = "ai.nreal.android.utilitiesproxy.PhoneCallReceiver";

        private static List<IPhoneCallListener> m_Listeners;
        private static NativePhoneCallListener m_NativeListener;

        private static AndroidJavaClass m_NativeClass;
        public static AndroidJavaClass NativeClass
        {
            get
            {
                if (m_NativeClass == null)
                {
                    m_NativeClass = new AndroidJavaClass(NativeClassName);
                }
                return m_NativeClass;
            }
        }

        public static void RegisterPhoneCallListener(IPhoneCallListener listener)
        {
            if (listener == null)
            {
                Debug.LogError("invalid parameter listener");
                return;
            }

            if (m_Listeners == null)
                m_Listeners = new List<IPhoneCallListener>();

            if (m_Listeners.Contains(listener))
                return;

            m_Listeners.Add(listener);

            if (m_NativeListener == null)
            {
                m_NativeListener = new NativePhoneCallListener();
#if UNITY_ANDROID && !UNITY_EDITOR
                NativeClass.CallStatic("setPhoneCallListener", m_NativeListener);
#endif
            }
        }

        public static void UnregisterDisplayListener(IPhoneCallListener listener)
        {
            if (listener == null)
            {
                Debug.LogError("invalid parameter listener");
                return;
            }

            if (m_Listeners != null)
            {
                m_Listeners.Remove(listener);

                if (m_Listeners.Count == 0)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    NativeClass.CallStatic("setPhoneCallListener", null);
#endif
                    m_NativeListener = null;
                }
            }
        }

        private class NativePhoneCallListener : AndroidJavaProxy
        {
            public NativePhoneCallListener()
                : base("ai.nreal.android.utilitiesproxy.PhoneCallListener")
            { }

            private void onIncomingCallStart(string number, long startDate)
            {
                m_Listeners?.ForEach(listener =>
                    listener.OnIncomingCallStart(number, startDate)
                );
            }

            private void onOutgoingCallStart(string number, long startDate)
            {
                m_Listeners?.ForEach(listener =>
                    listener.OnOutgoingCallStart(number, startDate)
                );
            }

            private void onIncomingCallEnd(string number, long startDate, long endDate)
            {
                m_Listeners?.ForEach(listener =>
                    listener.OnIncomingCallEnd(number, startDate, endDate)
                );
            }

            private void onOutgoingCallEnd(string number, long startDate, long endDate)
            {
                m_Listeners?.ForEach(listener =>
                    listener.OnOutgoingCallEnd(number, startDate, endDate)
                );
            }

            private void onMissCall(string number, long startDate)
            {
                m_Listeners?.ForEach(listener =>
                    listener.OnMissCall(number, startDate)
                );
            }
        }
    }
}