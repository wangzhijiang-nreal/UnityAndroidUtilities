using Nreal;
using Nreal.Natives;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

public class PhoneCallExample : MonoBehaviour, IPhoneCallListener
{
    public const string Permission_READ_PHONE_STATE = "android.permission.READ_PHONE_STATE";

    IEnumerator Start()
    {
        UnityMainThreadDispatcher.Instance.Init();
        
        while (!Permission.HasUserAuthorizedPermission(Permission_READ_PHONE_STATE))
        {
            Debug.Log("[PhoneCallExample] Start: request permission READ_PHONE_STATE");
            Permission.RequestUserPermission(Permission_READ_PHONE_STATE);
            yield return new WaitForSeconds(0.5f);
        }

        NativePhoneCallReceiver.RegisterPhoneCallListener(this);
    }

    void OnDestroy()
    {
        NativePhoneCallReceiver.UnregisterDisplayListener(this);
    }

    #region IPhoneCallListener

    void IPhoneCallListener.OnIncomingCallEnd(string number, long startDate, long endDate)
    {
        UnityMainThreadDispatcher.Instance.ToMainThread(() =>
        {
            Debug.LogFormat("[PhoneCallExample] OnIncomingCallEnd: incoming call from {0}", number);
        });
    }

    void IPhoneCallListener.OnIncomingCallStart(string number, long startDate)
    {
        UnityMainThreadDispatcher.Instance.ToMainThread(() =>
        {
            Debug.LogFormat("[PhoneCallExample] OnIncomingCallStart: incoming call from {0}", number);
        });
    }

    void IPhoneCallListener.OnMissCall(string number, long startDate)
    {
        UnityMainThreadDispatcher.Instance.ToMainThread(() =>
        {
            Debug.LogFormat("[PhoneCallExample] OnMissCall: miss call from {0}", number);
        });
    }

    void IPhoneCallListener.OnOutgoingCallEnd(string number, long startDate, long endDate)
    {
        
    }

    void IPhoneCallListener.OnOutgoingCallStart(string number, long startDate)
    {
        
    }

    #endregion
}
