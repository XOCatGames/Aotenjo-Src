using System;

namespace Aotenjo
{
    [Serializable]
    public abstract class TileMaterialPorcelain : TileMaterial
    {
        public TileMaterialPorcelain(int ID, string name) : base(ID, name, null)
        {
        }
    }
}