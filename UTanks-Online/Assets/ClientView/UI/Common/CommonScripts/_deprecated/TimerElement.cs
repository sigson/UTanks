using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.UI.Special
{
    public class TimerElement : MonoBehaviour
    {
        public float timerTime = 30.0f;
        protected float timeRemaining = 30.0f;
        public bool running = false;
        public bool loop = false;
        public System.Action<TimerElement> onTimerElapsed = (tm) => { Destroy(tm.gameObject); };

        private void Start()
        {
            StartImpl();
        }

        protected virtual void StartImpl()
        {
            timeRemaining = timerTime;
        }

        void Update()
        {
            UpdateImpl();
        }

        protected virtual void UpdateImpl()
        {
            if (running)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining < 0)
                {
                    if (timeRemaining < -9000)
                        return;
                    onTimerElapsed(this);
                    if (loop)
                    {
                        timeRemaining = timerTime;
                    }
                }
            }
        }

        public void ResetTimer()
        {
            timeRemaining = timerTime;
            running = true;
        }

        public void StopTimer()
        {
            timeRemaining = -1000;
        }
    }
}