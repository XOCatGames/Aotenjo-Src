using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialPaleWood : TileMaterialWood
    {
        [SerializeReference] public List<Tile> queue = new List<Tile>();

        public TileMaterialPaleWood(int ID) : base(ID, "pale_wood")
        {
        }

        public bool IsActive()
        {
            return queue.Count > 0;
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer));
        }

        public override void AppendToListDiscardEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile, bool withForce, bool isClone)
        {
            base.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);
            effects.Add(new PaleWoodEffect(this, tile).OnTile(tile));
            if (withForce)
            {
                effects.Add(new PaleWoodEffect(this, tile).OnTile(tile));
            }
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
            player.OnPrePostAddOnTileAnimationEffectEvent += Player_OnPrePostAddOnTileAnimationEffectEvent;
            player.DetermineSelectingTileEvent += Player_DetermineSelectingTileEvent;
        }

        private void Player_DetermineSelectingTileEvent(DeterminePlayerSelectingTileEvent evt)
        {
            if (queue.Count == 0 || evt.res) return;
            if (!evt.player.GetRiverTiles().Contains(evt.tile)) return;
            if (queue.Contains(evt.tile))
            {
                evt.res = true;
            }
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(PostRoundEnd);
            player.OnPrePostAddOnTileAnimationEffectEvent -= Player_OnPrePostAddOnTileAnimationEffectEvent;
            player.DetermineSelectingTileEvent -= Player_DetermineSelectingTileEvent;
        }

        private void Player_OnPrePostAddOnTileAnimationEffectEvent(Permutation perm, Player player,
            List<IAnimationEffect> effects)
        {
            int i = 0;
            while (queue.Any())
            {
                Tile tile = queue[0];
                if (tile != null && player.GetRiverTiles().Contains(tile))
                {
                    effects.Add(new PaleWoodOnTileAppendEffect(player.playHandEffectStack, 
                        new TileScoringEffectAppendEffect(player, tile, perm, player.playHandEffectStack), tile, i));
                    i++;
                }

                queue.RemoveAt(0);
            }
        }

        private class PaleWoodOnTileAppendEffect : ScoringEffectAppendEffect
        {
            private readonly ScoringEffectAppendEffect innerEffect;
            private readonly Tile tile;
            private readonly int i;

            public PaleWoodOnTileAppendEffect(Stack<IAnimationEffect> effectStack, ScoringEffectAppendEffect innerEffect, Tile tile, int i) : base(effectStack)
            {
                this.innerEffect = innerEffect;
                this.tile = tile;
                this.i = i;
            }

            public override List<IAnimationEffect> GetEffects()
            {
                List<IAnimationEffect> effects = innerEffect.GetEffects();
                return effects.Select(e =>
                {
                    if (e is OnTileAnimationEffect)
                    {
                        return new OnPalewoodAnimationEffect(tile, e.GetEffect(), i != 0);
                    }

                    if (e is not ScoringEffectAppendEffect)
                    {
                        return e;
                    }

                    return new PaleWoodOnTileAppendEffect(effectStack, (ScoringEffectAppendEffect)e, tile, i);
                }).ToList();
            }
        }

        private void PostRoundEnd(PlayerEvent playerEvent)
        {
            queue.Clear();
        }
    }

    internal class PaleWoodEffect : TextEffect
    {
        private TileMaterialPaleWood tileMaterialPaleWood;
        private readonly Tile tile;

        public PaleWoodEffect(TileMaterialPaleWood tileMaterialPaleWood, Tile tile) : base("effect_pale_wood")
        {
            this.tileMaterialPaleWood = tileMaterialPaleWood;
            this.tile = tile;
        }

        public override void Ingest(Player player)
        {
            base.Ingest(player);
            tileMaterialPaleWood.queue.Add(tile);
        }
    }
}