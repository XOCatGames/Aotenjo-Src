using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialGhost : TileMaterial
    {
        private const double FU = 40f;

        public TileMaterialGhost(int ID) : base(ID, "ghost", null)
        {
        }

        public override TileMaterial Copy()
        {
            return Ghost();
        }

        public override Rarity GetRarity()
        {
            return Rarity.COMMON;
        }

        public override string GetDescription(Func<string, string> localizer, Player player)
        {
            return string.Format(base.GetDescription(localizer), GetFu(player));
        }

        private double GetFu(Player player)
        {
            return FU;
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            effects.Add(ScoreEffect.AddFu(GetFu(player), null));
            if (player.Selecting(tile))
            {
                effects.Add(new TransferEffect(this, tile));
            }
        }

        public class TransferEffect : Effect
        {
            public TileMaterialGhost tileMaterialGhost;
            public Tile fromTile;

            public TransferEffect(TileMaterialGhost tileMaterialGhost, Tile fromTile)
            {
                this.tileMaterialGhost = tileMaterialGhost;
                this.fromTile = fromTile;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                return func("effect_ghost_transfer_name");
            }

            public override Artifact GetEffectSource()
            {
                return null;
            }

            public override void Ingest(Player player)
            {
                List<Tile> cands = player.GetSelectedTilesCopy()
                    .Where(t => t != null && t.CompactWithMaterial(PLAIN, player)).ToList();
                if (cands.Count == 0) return;
                Tile target = cands[player.GenerateRandomInt(cands.Count)];

                fromTile.SetMaterial(PLAIN, player, true);

                //TODO: 更改实现,通过事件告知
                if (player.GetArtifacts().Contains(Artifacts.MiniTomb))
                    fromTile.addonFu += 5;

                target.SetMaterial(tileMaterialGhost, player);
            }
        }
    }
}