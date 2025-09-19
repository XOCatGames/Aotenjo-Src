using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class WoodMaterialSet : MaterialSet
    {
        public WoodMaterialSet() : base(4, "woods", new List<string>
        {
            "nanmu_wood", "pale_wood", "emerald_wood", "mist_wood", "hell_wood", "jacaranda_wood", "pao_rosa_wood"
        })
        {
        }
    }
}