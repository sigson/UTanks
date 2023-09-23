using SecuredSpace.ClientControl.Model;
using SecuredSpace.Effects;
using SecuredSpace.Effects.Lighting;
using SecuredSpace.UnityExtend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Battle.Tank.Turret
{
    public class IProjectile : IManagable
    {
        public float Speed;
        public Vector3 StartPosition;
        public Vector3 Direction;
        public float MaxDistance;
        public float Influence;
        protected bool checkedDistance = false;
        public Dictionary<TankManager, float> hitedTanks = new Dictionary<TankManager, float>();
        public SerializableDictionary<string, AnimationScript> bulletAnimations = new SerializableDictionary<string, AnimationScript>();
        public Action<IProjectile, Collider> OnTriggerEnterAction = (projectile, collider) => { };
        public Action<GameObject> OnDistanceExcess = (projectileObject) => { };
        public ILight projectileLight;
        public float projectileLightSize = -1f;
        public Color projectileLightColor;

        public virtual IProjectile Init()
        {
            checkedDistance = false;
            return this;
        }
    }
}