using System;
using System.Linq;

namespace Aotenjo
{
    public class DemonStatueArtifact : Artifact
    {
        private static readonly int CHANCE = 3;

        public DemonStatueArtifact() : base("demon_statue", Rarity.EPIC)
        {
            
            
            SetHighlightRequirement((tile, _) =>
                tile.properties.mask.GetRegName() == TileMask.Corrupted().GetRegName());
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), GetChanceMultiplier(player));
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PreSetMaskEvent += OnChangeMask;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PreSetMaskEvent -= OnChangeMask;
        }

        private void OnChangeMask(PlayerSetAttributeEvent evt)
        {
            if (!evt.player.GetUnusedTilesInHand().Contains(evt.tile)) return;
            if (evt.attributeToReceive.GetRegName() == TileMask.NONE.GetRegName() &&
                evt.tile.properties.mask.GetRegName() == TileMask.Corrupted().GetRegName())
            {
                Player player = evt.player;
                LotteryPool<Tile> cands = new();
                cands.AddRange(player.GetUniqueFullDeck().Where(t => t.IsYaoJiu(player)));
                Tile to = cands.Draw(player.GenerateRandomInt);
                evt.tile.ModifyCarvedDesign(to, player);
                if (player.GenerateRandomDeterminationResult(CHANCE))
                {
                    evt.tile.SetMaterial(TileMaterial.Demon(), player);
                }
            }
        }
    }
}