using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public struct Movement
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 AngularVelocity { get; set; }
        public Quaternion Orientation { get; set; }
        public override string ToString() =>
            $"[Movement Position: {this.Position}, Velocity: {this.Velocity}, AngularVelocity: {this.AngularVelocity}, Orientation: {this.Orientation}]";
    }
}