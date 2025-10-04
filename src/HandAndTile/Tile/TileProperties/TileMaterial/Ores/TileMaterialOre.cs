using System;

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


        public override Effect[] GetEffects(Player player, Permutation permutation)
        {
            Effect[] baseEffects = base.GetEffects();
            Array.Resize(ref baseEffects, baseEffects.Length + 1);
            baseEffects[baseEffects.Length - 1] =
                new TransformEffect(player.GetAllTiles().Find(t => t.properties.material == this));
            return baseEffects;
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