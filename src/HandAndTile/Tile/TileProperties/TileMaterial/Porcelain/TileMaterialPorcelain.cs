using System;

namespace Aotenjo
{
    [Serializable]
    public abstract class TileMaterialPorcelain : TileMaterial
    {
        public TileMaterialPorcelain(int id, string name) : base(id, name, null)
        {
        }
    }
}