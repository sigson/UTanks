using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTanksClient;
using UTanksClient.Core.Logging;
using UTanksClient.ECS.ECSCore;
using UTanksClient.Extensions;

namespace SecuredSpace.ClientControl.Model
{
    public abstract class IManager : ProxyBehaviour
    {
        public long instanceId = Guid.NewGuid().GuidToLong();
        #region typeId
        private static Type _managerTypeValue = null; 
        public static Type ManagerType
        {
            get
            {
                if (_managerTypeValue == null)
                {
                    var AllDirtyManagers = ECSAssemblyExtensions.GetAllSubclassOf(typeof(IManager)).Where(x => !x.IsAbstract).Select(x => (IManager)Activator.CreateInstance(x)).ToList();
                    foreach (var manager in AllDirtyManagers)
                    {
                        var field = manager.GetType().GetField("_managerTypeValue", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                        field.SetValue(manager.GetType(), manager.GetType());
                    }
                }
                return _managerTypeValue;
            }
        }
        #endregion
        private IECSObject _ConnectPoint;
        public IECSObject ConnectPoint { 
            get
            {
                return _ConnectPoint;
            }
            set
            {
                if(this is IEntityManager && value is ECSEntity)
                {
                    (this as IEntityManager).ManagerEntity = value as ECSEntity;
                }
                value.connectPoints.Add(this);
                _ConnectPoint = value;
            }
        }

        [SerializeField]
        private bool NoSetupChild = false;
        public bool isNoSetupChild
        {
            get
            {
                return NoSetupChild;
            }
            set
            {
                NoSetupChild = value;
            }
        }

        [SerializeField]
        private bool PrefabScript = false;
        public bool isPrefabScript
        {
            get
            {
                return PrefabScript;
            }
            set
            {
                PrefabScript = value;
            }
        }

        #region mockFunctions

        protected virtual void Awake()
        {
            if (!isPrefabScript)
            {
                OnAwakeManager();
            }
        }

        protected virtual void OnEnable()
        {
            if (!isPrefabScript)
            {
                OnActivateManager();
            }
        }

        protected virtual void Start()
        {
            if (!isPrefabScript)
            {
                if (!NoSetupChild)
                {
                    if (this is IEntityManager)
                        ManagerSpace.InstantiatedProcess(this.gameObject, (IEntityManager)this);
                    if (SetupAction != null)
                        SetupAction(this);
                }
                OnStartManager();
            }
        }

        protected virtual void Reset()
        {
            if (!isPrefabScript)
                ResetManager();
        }

        protected virtual void ResetManager()
        {

        }

        protected virtual void FixedUpdate()
        {
            if (!isPrefabScript)
            {
                this.FixedUpdateManager();
            }

        }
        protected virtual void FixedUpdateManager()
        {

        }

        protected virtual void Update()
        {
            if (!isPrefabScript)
            {
                this.UpdateManager();
            }

        }
        protected virtual void UpdateManager()
        {

        }



        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!isPrefabScript)
            {
                this.OnCollisionEnterManager(collision);
            }
        }

        protected virtual void OnCollisionEnterManager(Collision collision)
        {

        }

        protected virtual void OnCollisionExit(Collision collisionInfo)
        {
            if (!isPrefabScript)
            {
                this.OnCollisionExitManager(collisionInfo);
            }
        }

        protected virtual void OnCollisionExitManager(Collision collisionInfo)
        {

        }

        protected virtual void OnCollisionStay(Collision collisionInfo)
        {
            if (!isPrefabScript)
            {
                this.OnCollisionStayManager(collisionInfo);
            }
        }

        protected virtual void OnCollisionStayManager(Collision collisionInfo)
        {

        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!isPrefabScript)
            {
                this.OnTriggerEnterManager(other);
            }
        }

        protected virtual void OnTriggerEnterManager(Collider other)
        {

        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!isPrefabScript)
            {
                this.OnTriggerExitManager(other);
            }
        }

        protected virtual void OnTriggerExitManager(Collider other)
        {

        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (!isPrefabScript)
            {
                this.OnTriggerStayManager(other);
            }
        }

        protected virtual void OnTriggerStayManager(Collider other)
        {

        }

        protected virtual void OnApplicationPause()
        {
            if (!isPrefabScript)
            {
                this.OnApplicationPauseManager();
            }
        }

        protected virtual void OnApplicationPauseManager()
        {

        }

        protected virtual void OnDisable()
        {
            if (!isPrefabScript)
                OnDeactivateManager();
        }

        protected virtual void OnDestroy()
        {
            if (!isPrefabScript)
            {
                OnRemoveManager();
                if (this is IEntityManager)
                    ((IEntityManager)this).ClearManagable();
                if (RemoveAction != null)
                    RemoveAction(this);
            }
        }

        #endregion

        public abstract void AddManager();
        protected abstract void OnStartManager();
        protected abstract void OnAwakeManager();
        protected abstract void OnRemoveManager();
        protected abstract void OnActivateManager();
        protected abstract void OnDeactivateManager();

        public Action<IManager> SetupAction;
        public Action<IManager> RemoveAction;
        public virtual void ActivateManager()
        {
            this.enabled = true;
        }
        public virtual void DeactivateManager()
        {
            this.enabled = false;
        }
        public virtual void RemoveManager()
        {
            try
            {
                this.ExecuteInstruction(() => Destroy(this));
            }
            catch(Exception ex)
            {
                ULogger.Error("Error destroy " + this.GetType() + "  " + ex.Message + " ____ " + ex.StackTrace);
            }
        }
    }
}