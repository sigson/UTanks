using SecuredSpace.ClientControl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;

namespace SecuredSpace.Common.Animation
{
    public class TweenerService : IService
    {
        static public TweenerService instance => IService.getInstance<TweenerService>();

        private Dictionary<object, TweenObject> tweenDB = new Dictionary<object, TweenObject>();

        public override void InitializeProcess()
        {

        }

        public void To(object targetObject, Action<object> doAction, Func<object, bool> checkAction, Action callbackAction = null, int frameskip = 0, bool fixedUpdate = false)
        {
            if(!tweenDB.ContainsKey(targetObject))
                tweenDB.Add(targetObject, new TweenObject()
                {
                    workaroundObject = targetObject,
                    doAction = doAction,
                    checkAction = checkAction,
                    callbackAction = callbackAction,
                    fixedUpdate = fixedUpdate,
                    frameskip = frameskip
                });
        }

        public void ExcludeObjectImmediate(object target)
        {
            //ULogger.Log("audio rty delete" + tweenDB.Count);
            if(tweenDB.ContainsKey(target) || tweenDB.Count > 0)
                tweenDB.Remove(target);
        }

        private void Update()
        {
            for (int i = 0; i < tweenDB.Count; i++)//no lock needed because thread is one
            {
                var tweenObject = tweenDB.ElementAt(i);
                if(!tweenObject.Value.fixedUpdate)
                {
                    if (tweenObject.Key.ToString() == "null")
                    {
                        tweenDB.Remove(tweenObject.Key);
                        i--;
                        continue;
                    }
                    TweenProcess(tweenObject.Value);
                }
            }
            toDeleteCache.ForEach(x =>
            {
                tweenDB.Remove(x.workaroundObject);
                if (x.callbackAction != null)
                {
                    x.callbackAction();
                }
            });
            toDeleteCache.Clear();
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < tweenDB.Count; i++)//no lock needed because thread is one
            {
                var tweenObject = tweenDB.ElementAt(i);
                if (tweenObject.Value.fixedUpdate)
                {
                    if (tweenObject.Key.ToString() == "null")
                    {
                        tweenDB.Remove(tweenObject.Key);
                        i--;
                        continue;
                    }
                    TweenProcess(tweenObject.Value);
                }
            }
            toDeleteCache.ForEach(x =>
            {
                tweenDB.Remove(x.workaroundObject);
                if (x.callbackAction != null)
                {
                    x.callbackAction();
                }
            });
            toDeleteCache.Clear();
        }
        private List<TweenObject> toDeleteCache = new List<TweenObject>();
        private void TweenProcess(TweenObject tweenObject)
        {
            if(!tweenObject.finished)
                tweenObject.doAction(tweenObject.workaroundObject);
            if (tweenObject.checkAction(tweenObject.workaroundObject))
            {
                tweenObject.finished = true;
                if (tweenObject.frameskip > 0)
                    tweenObject.frameskip--;
                else
                    toDeleteCache.Add(tweenObject);
            }
        }

        public override void OnDestroyReaction()
        {

        }

        public override void PostInitializeProcess()
        {
            
        }
    }

    public static class TweenExtensions
    {

        public static float MoveValue(this float value, float target, float step)
        {
            return target < value
                ? Mathf.Max(value - step, target)
                : Mathf.Min(value + step, target);
        }

        public static AudioSource DOFade(this AudioSource target, float endValue, float duration, Action callbackAction = null)
        {
            if (endValue < 0) endValue = 0;
            else if (endValue > 1) endValue = 1;
            TweenerService.instance.To(target, (target) =>
            {
                var targetType = target as AudioSource;
                var newValue = targetType.volume.MoveValue(endValue, duration + Time.deltaTime);
                if (newValue < 0f)
                    newValue = 0f;
                if (newValue > 1f)
                    newValue = 1f;
                targetType.volume = newValue;
                //ULogger.Log("audio decrease to " + targetType.volume);
            },
            (target) =>
            {
                var result = Math.Abs((target as AudioSource).volume - endValue) < 0.001f ? true : false;
                return result;
            },
            callbackAction, 3);
            return target;
            //TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.volume, x => target.volume = x, endValue, duration);
            //t.SetTarget(target);
            //return t;
        }
    }

    public sealed class TweenObject
    {
        public long id = Guid.NewGuid().GuidToLong();
        public object workaroundObject;
        public Action<object> doAction;
        public Func<object, bool> checkAction;
        public Action callbackAction;
        public bool fixedUpdate = false;
        public int frameskip = 0;
        public bool finished = false;

        public bool isValid()
        {
            if(workaroundObject == null || doAction == null || checkAction == null)
            {
                return false;
            }
            return true;
        }
    }
}
