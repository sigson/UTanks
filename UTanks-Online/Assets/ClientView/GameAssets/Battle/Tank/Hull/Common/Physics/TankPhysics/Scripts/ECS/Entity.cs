using System;
using System.Collections.Generic;
using UTanksClient.ECS.ECSCore;

namespace SecuredSpace.Important.Raven
{
    public class Entity
    {
        public List<Component> storage = new List<Component>();

        public void AddComponent(Component component)
        {
            Type componentClass = component.GetType();
            //if (!this.storage.HasComponent(componentClass) || !this.IsSkipExceptionOnAddRemove(componentClass))
            //{
            //    this.UpdateHandlers(component.GetType());
            //    this.NotifyAttachToEntity(component);
            //    this.storage.AddComponentImmediately(component.GetType(), component);
            //    this.MakeNodes(component.GetType(), component);
            //    if (sendEvent)
            //    {
            //        this.NotifyAddComponent(component);
            //    }
            //    this.PrepareAndSendNodeAddedEvent(component);
            //}
            storage.Add(component);
        }

        public void AddComponent(Type componentType)
        {
            Component component = this.CreateNewComponentInstance(componentType);
            this.AddComponent(component);
        }

        public void AddComponent<T>() where T : Component, new()
        {
            this.AddComponent(typeof(T));
        }

        public Component CreateNewComponentInstance(Type componentType)
        {

            return (Component)Activator.CreateInstance(componentType);
        }
    }
}