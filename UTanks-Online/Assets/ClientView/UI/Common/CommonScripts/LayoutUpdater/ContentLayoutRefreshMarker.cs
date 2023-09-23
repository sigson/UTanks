using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UTanksClient;

namespace SecuredSpace.UI.Special
{
    public class ContentLayoutRefreshMarker : MonoBehaviour
    {
        public bool UpdateOnEnable = false;
        public bool UpdateOnStart = false;
        public bool DirectveUpdate = false;
        public void DirectUpdate() => this.GetComponentsInChildren<ContentLayoutRefreshMarker>().ForEach(x => x.RegisterForUpdating());
        public ContentLayoutRefreshMarker MarkForUpdateOnEnable = null;
        public ContentLayoutRefreshMarker MarkForUpdateOnStart = null;

        public void Awake()
        {
            RegisterForUpdating();
        }

        public void OnEnable()
        {
            RegisterForUpdating();
            if (DirectveUpdate)
                this.GetComponentsInChildren<ContentLayoutRefreshMarker>().ForEach(x => x.RegisterForUpdating());
            if (UpdateOnEnable)
                MarkForUpdate();
            if (MarkForUpdateOnEnable != null)
                MarkForUpdateOnEnable.MarkForUpdate();
        }

        public void Start()
        {
            RegisterForUpdating();
            if (DirectveUpdate)
                this.GetComponentsInChildren<ContentLayoutRefreshMarker>().ForEach(x => x.RegisterForUpdating());
            if (UpdateOnStart)
                MarkForUpdate();
            if (MarkForUpdateOnStart != null)
                MarkForUpdateOnStart.MarkForUpdate();
        }

        public void OnDestroy()
        {
            LayoutUpdaterService.OnDestroyMarker(this);
        }

        public void RegisterForUpdating()
        {
            LayoutUpdaterService.RegisterMarker(this);
        }

        public void MarkForUpdate()
        {
            LayoutUpdaterService.OnMarked(this);
        }
    }
}