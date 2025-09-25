using System;
using System.Collections.Generic;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialOre : TileMaterial
    {
        public TileMaterialOre(int ID) : base(ID, "ore", null)
        {
        }

        public override TileMaterial Copy()
        {
            return new TileMaterialOre(spriteID);
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            effects.Add(new TransformEffect(tile));
        }

        public class TransformEffect : Effect
        {
            private Tile tile;

            public TransformEffect(Tile tile)
            {
                this.tile = tile;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_transformed_ore");
            }

            public override string GetSoundEffectName()
            {
                return "Ore";
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }

            public override void Ingest(Player player)
            {
                LotteryPool<TileMaterial> pool = new LotteryPool<TileMaterial>();

                pool.Add(COPPER, 20);
                pool.Add(CRYSTAL, 10);
                pool.Add(GOLDEN, 10);
                pool.Add(Agate(), 10);
                pool.Add(Jade(), 2);
                pool.Add(Voidstone(), 3);

                TileMaterial newMaterial = pool.Draw(player.GenerateRandomInt);

                if (player.GetArtifacts().Contains(Artifacts.AgateIdentification))
                {
                    newMaterial = Agate();
                }

                tile.SetMaterial(newMaterial, player);
            }
        }
    }
}