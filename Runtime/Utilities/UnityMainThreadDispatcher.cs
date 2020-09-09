using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Nreal
{
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        static UnityMainThreadDispatcher m_Instance;
        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new GameObject("UnityMainThreadDispatcher")
                        .AddComponent<UnityMainThreadDispatcher>();
                }

                return m_Instance;
            }
        }

        void Awake()
        {
            // Don't show in scene hierarchy
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            DontDestroyOnLoad(gameObject);
        }

        private Queue<Action> m_ExecutionQueue = new Queue<Action>();

        private int m_DequeueLimitPerLoop = 20;

        ///<summary>
        ///Put `Init` somewhere that game start.
        ///</summary>
        public void Init()
        {
            
        }

        private void Update()
        {
            lock (m_ExecutionQueue)
            {
                int i = 0;
                while (m_ExecutionQueue.Count > 0 && i < m_DequeueLimitPerLoop)
                {
                    var a = m_ExecutionQueue.Dequeue();
                    if (a != null)
                    {
                        a.Invoke();
                    }

                    i++;
                }
            }
        }

        public void ToMainThread(Action action)
        {
            lock (m_ExecutionQueue)
            {
                m_ExecutionQueue.Enqueue(action);
            }
        }

        public void ToMainThreadAfterDelay(System.Double seconds, Action a)
        {
            Instance.ToMainThread(() =>
            {
                Instance.StartCoroutine(Instance.ExecuteDelayed(seconds, a));
            });
        }

        IEnumerator ExecuteDelayed(System.Double seconds, Action a)
        {
            yield return new WaitForSeconds((float)seconds);
            lock (m_ExecutionQueue)
            {
                m_ExecutionQueue.Enqueue(a);
            }
        }

        public Task TaskToMainThread(Action a)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            lock (m_ExecutionQueue)
            {
                m_ExecutionQueue.Enqueue(() =>
                {
                    a();
                    tcs.SetResult(true);
                });
            }
            return tcs.Task;
        }

        public Task<TResult> TaskToMainThread<TResult>(Func<TResult> f)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            lock (m_ExecutionQueue)
            {
                m_ExecutionQueue.Enqueue(() =>
                {
                    tcs.SetResult(f());
                });
            }
            return tcs.Task;
        }
    }
}