using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public class LuaMaterialSetBuilder
    {
        
        private int id;
        private string regName;
        private List<string> availableMaterials = new();

        private Func<MaterialSet, LotteryPool<TileMaterial>> onGenerateRareMaterialPool;
        private Func<MaterialSet, LotteryPool<TileMaterial>> onGenerateCommonMaterialPool;
        private Action<Player, MaterialSet> onPlayerSubscribe;
        private Action<Player, MaterialSet> onPlayerUnsubscribe;

        private LuaMaterialSetBuilder() { }

        public static LuaMaterialSetBuilder Create(string regName)
        {
            return new LuaMaterialSetBuilder
            {
                id = -5,
                regName = regName
            };
        }

        // --- Chainable setters ---
        public LuaMaterialSetBuilder WithAvailableMaterials(string[] materials)
        {
            availableMaterials = materials.ToList() ?? new List<string>();
            return this;
        }

        public LuaMaterialSetBuilder AddMaterial(string material)
        {
            availableMaterials.Add(material);
            return this;
        }

        public LuaMaterialSetBuilder OnGenerateRare(Func<MaterialSet, LotteryPool<TileMaterial>> f)
        {
            onGenerateRareMaterialPool = f;
            return this;
        }

        public LuaMaterialSetBuilder OnGenerateCommon(Func<MaterialSet, LotteryPool<TileMaterial>> f)
        {
            onGenerateCommonMaterialPool = f;
            return this;
        }

        public LuaMaterialSetBuilder OnSubscribe(Action<Player, MaterialSet> f)
        {
            onPlayerSubscribe = f;
            return this;
        }

        public LuaMaterialSetBuilder OnUnsubscribe(Action<Player, MaterialSet> f)
        {
            onPlayerUnsubscribe = f;
            return this;
        }

        // --- Build ---
        public LuaMaterialSet Build()
        {
            return LuaMaterialSet.Create(
                id,
                regName,
                availableMaterials,
                onGenerateRareMaterialPool,
                onGenerateCommonMaterialPool,
                onPlayerUnsubscribe,
                onPlayerSubscribe
            );
        }

        public LuaMaterialSet BuildAndRegister()
        {
            var set = Build();
            if (MaterialSet.MaterialSets.All(ms => ms.GetRegName() != set.GetRegName()))
            {
                MaterialSet.MaterialSets = MaterialSet.MaterialSets.Append(set).ToArray();
            }
            else
            {
                Debug.LogWarning($"[LuaMaterialSetBuilder] MaterialSet '{set.GetRegName()}' already registered. Skipped duplicate.");
            }
            return set;
        }

    }
}