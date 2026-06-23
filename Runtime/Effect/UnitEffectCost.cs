namespace LLib
{
    public struct UnitEffectCost
    {
        public int VitalTypeID;
        public float Value;

        public UnitEffectCost(int vitalTypeID, float value)
        {
            VitalTypeID = vitalTypeID;
            Value = value;
        }
    }
}