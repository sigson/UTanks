using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UTanksClient.Extensions;

namespace SecuredSpace.UI.Special
{
    public class LayoutUpdaterService : MonoBehaviour
    {
        public static Dictionary<ContentLayoutRefreshMarker, Dictionary<ContentLayoutRefreshMarker, int>> RegisteredMarkers = new Dictionary<ContentLayoutRefreshMarker, Dictionary<ContentLayoutRefreshMarker, int>>();

        public static Dictionary<ContentLayoutRefreshMarker, int> ChangedMarkers = new Dictionary<ContentLayoutRefreshMarker, int>();

        public static void RegisterMarker(ContentLayoutRefreshMarker marked)
        {
            if (!RegisteredMarkers.ContainsKey(marked))
            {
                Dictionary<ContentLayoutRefreshMarker, int> newparents = new Dictionary<ContentLayoutRefreshMarker, int>();

                foreach (var markerParent in marked.GetComponentsInParent<ContentLayoutRefreshMarker>())
                {
                    newparents[markerParent] = 0;
                }
                RegisteredMarkers[marked] = newparents;
                ChangedMarkers[marked] = 0;
            }
            
        }

        public static void OnMarked(ContentLayoutRefreshMarker marked)
        {
            RegisterMarker(marked);
            TupleList<int, ContentLayoutRefreshMarker> sMarkers = new TupleList<int, ContentLayoutRefreshMarker>();
            foreach(var chMarker in ChangedMarkers.Keys)
            {
                if (chMarker == marked)
                    continue;
                var markerparent = RegisteredMarkers[chMarker];
                Dictionary<ContentLayoutRefreshMarker, int> newparents = new Dictionary<ContentLayoutRefreshMarker, int>();
                if (markerparent.TryGetValue(marked, out _))
                {
                    sMarkers.Add(markerparent.Count, chMarker);
                }
            }
            sMarkers.ReverseSort();
            foreach (var sortMarker in sMarkers)
            {
                RefreshContentFitter(sortMarker.Item2.GetComponent<RectTransform>());
                ChangedMarkers.Remove(sortMarker.Item2);
            }
            RefreshContentFitter(marked.GetComponent<RectTransform>(), true);
            ChangedMarkers.Remove(marked);
        }

        public static void OnDestroyMarker(ContentLayoutRefreshMarker destroyed)
        {
            if (RegisteredMarkers.ContainsKey(destroyed))
            {
                List<ContentLayoutRefreshMarker> removedMarkers = new List<ContentLayoutRefreshMarker>();
                foreach (var markerparent in RegisteredMarkers)
                {
                    Dictionary<ContentLayoutRefreshMarker, int> newparents = new Dictionary<ContentLayoutRefreshMarker, int>();
                    if (markerparent.Value.TryGetValue(destroyed, out _))
                    {
                        removedMarkers.Add(markerparent.Key);
                    }
                }
                removedMarkers.ForEach(x => {
                    RegisteredMarkers.Remove(x);
                    if (ChangedMarkers.ContainsKey(x))
                        ChangedMarkers.Remove(x);
                });
                if (ChangedMarkers.ContainsKey(destroyed))
                    ChangedMarkers.Remove(destroyed);
                RegisteredMarkers.Remove(destroyed);
            }
        }
        public static void RefreshContentFitter(RectTransform transform, bool force = false)
        {
            Profiler.BeginSample("refresh ui");
            if (transform == null || !transform.gameObject.activeSelf)
            {
                return;
            }

            //foreach (RectTransform child in transform)
            //{
            //    RefreshContentFitter(child);
            //}

            var layoutGroup = transform.GetComponent<LayoutGroup>();
            var contentSizeFitter = transform.GetComponent<ContentSizeFitter>();
            if (layoutGroup != null)
            {
                layoutGroup.SetLayoutHorizontal();
                layoutGroup.SetLayoutVertical();
            }

            if (contentSizeFitter != null)
            {
                //LayoutRebuilder.MarkLayoutForRebuild(transform);
                //if (force)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
            }
            Profiler.EndSample();
        }
    }
}