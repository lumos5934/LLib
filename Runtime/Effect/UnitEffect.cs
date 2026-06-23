using System;
using System.Collections.Generic;

namespace LLib
{
    [Serializable]
    public class UnitEffect
    {
        public int TargetStatID; //Health ..
        public int TargetResourceID; //Health ..


        public int MethodFlags; //Direct, Dot ...
        public int AttributeFlags; //Normal, Fire ...
        public bool IsPositive;


        public float BaseValue;
        public float AdditionalValue;
        public float FinalMultiplier = 1f;


        public float Duration;
        public float TickInterval;


        public List<EffectFactor> Factors = new();


        public float FinalValue
        {
            get
            {
                var value = (BaseValue + AdditionalValue) * FinalMultiplier;
                return IsPositive ? value : value * -1f;
            }
        }


        public void Copy(UnitEffect origin)
        {
            MethodFlags = origin.MethodFlags;
            AttributeFlags = origin.AttributeFlags;
            TargetResourceID = origin.TargetResourceID;
            TargetStatID = origin.TargetStatID;
            IsPositive = origin.IsPositive;
            FinalMultiplier = origin.FinalMultiplier;
            BaseValue = origin.BaseValue;
            Duration = origin.Duration;
            TickInterval = origin.TickInterval;

            Factors.Clear();
            Factors.AddRange(origin.Factors);
        }


        public void Reset()
        {
            MethodFlags = 0;
            AttributeFlags = 0;
            TargetResourceID = 0;
            TargetStatID = 0;
            IsPositive = false;
            FinalMultiplier = 1f;
            BaseValue = 0;
            Duration = 0;
            TickInterval = 0;
            Factors.Clear();
        }
    }
}