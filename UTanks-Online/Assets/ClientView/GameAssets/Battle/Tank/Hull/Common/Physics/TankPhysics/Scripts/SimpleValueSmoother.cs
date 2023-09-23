using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SecuredSpace.Important.Raven
{
    public class SimpleValueSmoother
    {
        private float targetValue;
        private float smoothingSpeedUp;
        private float smoothingSpeedDown;

        public SimpleValueSmoother(float smoothingSpeedUp, float smoothingSpeedDown, float targetValue, float currentValue)
        {
            this.smoothingSpeedUp = smoothingSpeedUp;
            this.smoothingSpeedDown = smoothingSpeedDown;
            this.targetValue = targetValue;
            this.CurrentValue = currentValue;
        }

        public float GetTargetValue() =>
            this.targetValue;

        public void Reset(float value)
        {
            this.CurrentValue = value;
            this.targetValue = value;
        }

        public void SetTargetValue(float value)
        {
            this.targetValue = value;
        }

        public float Update(float dt)
        {
            if (this.CurrentValue < this.targetValue)
            {
                this.CurrentValue += this.smoothingSpeedUp * dt;
                if (this.CurrentValue > this.targetValue)
                {
                    this.CurrentValue = this.targetValue;
                }
            }
            else if (this.CurrentValue > this.targetValue)
            {
                this.CurrentValue -= this.smoothingSpeedDown * dt;
                if (this.CurrentValue < this.targetValue)
                {
                    this.CurrentValue = this.targetValue;
                }
            }
            return this.CurrentValue;
        }

        public float CurrentValue { get; private set; }
    }
}