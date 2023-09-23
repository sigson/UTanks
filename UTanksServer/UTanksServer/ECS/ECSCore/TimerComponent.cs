using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace UTanksServer.ECS.ECSCore
{
    [TypeUid(199449741865219700)]
    public class TimerComponent : ECSComponent
    {
        static public new long Id { get; set; }
        static public new System.Collections.Generic.List<System.Action> StaticOnChangeHandlers { get; set; }
        [NonSerialized]
        TimerEx componentTimer = new TimerEx();
        public double timerAwait = 0;
		private double timeRemaining;
        public double TimeRemaining { 
            get {
                if (this.componentTimer.inited)
                    return this.componentTimer.RemainingToElapsedTime();
                else
                    return timeRemaining;
            }
            set
            {
                if(!componentTimer.inited)
                {
                    timeRemaining = value;
                }
            }
        }
        [NonSerialized]
        public Action<ECSEntity, ECSComponent> onStart;
        [NonSerialized]
        public Action<ECSEntity, ECSComponent> onEnd;
        public List<object> args;

        public virtual ECSComponent TimerStart(double newUpdatedTime, ECSEntity entity, bool inSeconds = false, bool loop = false)
        {
            if(componentTimer.Dead)
            {
                componentTimer = new TimerEx(componentTimer);
            }
            if (newUpdatedTime == 0)
            {
                if(timerAwait != 0)
                {
                    componentTimer.Interval = (inSeconds ? timerAwait*1000 : timerAwait);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                timerAwait = (inSeconds ? newUpdatedTime * 1000 : newUpdatedTime);
                componentTimer.Interval = (inSeconds ? newUpdatedTime * 1000 : newUpdatedTime);
            }
            if(entity != null)
            {
                ownerEntity = entity;
            }
            if(onStart != null)
            {
                onStart(ownerEntity, this);
            }
            componentTimer.Elapsed += async (sender, e) => await Task.Run(()=>TimerEnd());
            componentTimer.AutoReset = loop;
            componentTimer.Start();
            return this;
        }

        public virtual void TimerEnd()
        {
            if(!componentTimer.AutoReset)
            {
                componentTimer.Stop();
                //componentTimer.Enabled = false;
            }
                
            if (ownerEntity != null && onEnd != null)
            {
                onEnd(ownerEntity, this);
            }
        }

        public virtual void TimerStop()
        {
            componentTimer.Stop();
        }

        public virtual void TimerReset()
        {
            componentTimer.Reset();
        }

        public virtual void TimerPause()
        {
            componentTimer.Pause();
        }

        public virtual void TimerResume()
        {
            componentTimer.Resume();
        }
    }
}
