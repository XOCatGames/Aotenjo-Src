using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo.TileMaterialGlobalEffect;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class MaterialSet : IUnlockable
    {
        [SerializeField] public int id;

        [SerializeField] public string regName;

        [SerializeField] public List<string> availableMaterials;

        protected MaterialSet(int id, string regName, List<string> availableMaterials)
        {
            this.id = id;
            this.regName = regName;
            this.availableMaterials = availableMaterials;
        }

        public List<TileMaterial> GetMaterials()
        {
            List<TileMaterial> materials = new List<TileMaterial>();
            foreach (string material in availableMaterials)
            {
                materials.Add(TileMaterial.GetMaterial(material));
            }

            return materials;
        }
        
        public string GetRegName()
        {
            return regName;
        }

        public virtual string GetName(Func<string, string> loc)
        {
            return loc($"material_set_{regName}_name");
        }

        public virtual bool IsUnlocked(PlayerStats globalStats)
        {
            return Constants.DEBUG_MODE ||
                   GetUnlockRequirement().IsUnlocked(globalStats);
        }

        public virtual void SubscribeToPlayerEvents(Player player)
        {
            foreach (var mat in availableMaterials)
            {
                var subscriberMap = TileMaterialEntryEffect.TileMaterialEntryEffectMap;
                if (subscriberMap.ContainsKey(mat.Replace("_material", "")))
                {
                    subscriberMap[mat.Replace("_material", "")].SubscribeToPlayerEvents(player);
                }
            }
        }

        public virtual void UnsubscribeToPlayerEvents(Player player)
        {
            foreach (var mat in availableMaterials)
            {
                var subscriberMap = TileMaterialEntryEffect.TileMaterialEntryEffectMap;
                if (subscriberMap.TryGetValue(mat.Replace("_material", ""), out var effect))
                {
                    effect.UnsubscribeToPlayerEvents(player);
                }
            }
        }

        public virtual LotteryPool<TileMaterial> GenerateCommonMaterialPool()
        {
            LotteryPool<TileMaterial> pool = new LotteryPool<TileMaterial>();
            pool.AddRange(GetMaterialsWithRarity(Rarity.COMMON));
            return pool;
        }

        public virtual LotteryPool<TileMaterial> GenerateRareMaterialPool()
        {
            LotteryPool<TileMaterial> pool = new LotteryPool<TileMaterial>();
            pool.AddRange(GetMaterialsWithRarity(Rarity.RARE));
            return pool;
        }

        private List<TileMaterial> GetMaterialsWithRarity(Rarity rarity)
        {
            return TileMaterial.Materials()
                .Where(m => m.GetRarity() == rarity && availableMaterials
                    .Select(a => a.EndsWith("_material")? a : ( a + "_material"))
                    .Contains(m.GetRegName())).ToList();
        }

        public static MaterialSet GetMaterialSet(string materialSet)
        {
            switch (materialSet)
            {
                case "basic":
                    return Basic;
                case "ore":
                    return Ore;
                case "porcelain":
                    return Porcelain;
                case "monsters":
                    return Monsters;
                case "woods":
                    return Wood;
                case "desserts":
                    return Dessert;
                default:
                    return null;
            }
        }

        public static MaterialSet Basic = new MaterialSet(0, "basic", new List<string>
        {
            "ore", "copper", "golden", "crystal", "jade", "dessert_chocolate", "demon"
        });

        public static MaterialSet Ore = new MaterialSet(1, "ore", new List<string>
        {
            "ore", "copper", "golden", "crystal", "agate", "jade", "voidstone"
        });

        public static MaterialSet Porcelain = new MaterialSet(2, "porcelain", new List<string>
        {
            "blue_and_white_porcelain", "bone_porcelain", "green_porcelain", "famille_verte_porcelain",
            "white_porcelain", "pink_porcelain", "secret_color_porcelain"
        });

        public static MaterialSet Monsters = new MaterialSet(3, "monsters", new List<string>
        {
            "demon", "ghost", "mo", "nest", "taotie", "succubus", "gold_mouse"
        });

        public static MaterialSet Wood = new WoodMaterialSet();

        public static MaterialSet Dessert = new MaterialSet(5, "desserts", new List<string>
        {
            "dessert_butter", "dessert_chocolate", "dessert_ice_cream", "dessert_jelly", "dessert_lollipop", "dessert_mille_feuille", "dessert_sugar_cube"
        });
        
        // public static MaterialSet MechPart = new MaterialSet(6, "mech_parts", new List<string>
        // {
        //     "gear"
        // });
        //
        // public static MaterialSet Custom = new MaterialSet(7, "custom", new List<string>
        // {
        //     "green_porcelain", "golden", "dessert_butter", "demon", "mo",
        //     "succubus", "gold_mouse"
        // });
        
        public static MaterialSet[] materialSets = new []
        {
            Basic, Ore, Porcelain, Monsters, Wood, Dessert
        };

        public virtual UnlockRequirement GetUnlockRequirement()
        {
            return MaterialSetUnlockRequirements.matSetRequirements[this];
        }
    }
}