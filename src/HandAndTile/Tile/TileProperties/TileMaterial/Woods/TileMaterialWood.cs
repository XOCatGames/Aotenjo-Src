using System;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialWood : TileMaterial
    {
        public TileMaterialWood(int ID, string name) : base(ID, name, null)
        {
        }

        public override int GetShadowID()
        {
            return 52;
        }
    }
}