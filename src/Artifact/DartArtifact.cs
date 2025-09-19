using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Aotenjo
{
    public class DartArtifact : Artifact
    {
        private const double MUL = 1.5D;
        private Tile.Category bounty = Tile.Category.Suo;
        
        public DartArtifact() : base("dart", Rarity.RARE)
        {
            
        }
        
        public override string GetDescription(Func<string, string> localizer)
        {
            string typeFormat = localizer(Tile.CategoryToNameKey(bounty));
            return string.Format(base.GetDescription(localizer), typeFormat, MUL.ToShortString());
        }
        
        public override void ResetArtifactState()
        {
            bounty = Tile.Category.Suo;
        }

        public override string Serialize()
        {
            return $"{(int)bounty}";
        }

        public override void Deserialize(string data)
        {
            int categoryIndex = int.Parse(data);
            bounty = (Tile.Category)categoryIndex;
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PreAddScoringAnimationEffectEvent += OnPreAddScoringEffect;
            player.OnAddSingleAnimationEffectEvent += PlayerOnOnAddSingleAnimationEffectEvent;
        }

        private void PlayerOnOnAddSingleAnimationEffectEvent(Player player, List<IAnimationEffect> effects, IAnimationEffect eff)
        {
            if (eff.GetEffect().HasTag(EffectTag.CyclingSuit))
            {
                effects.Add(ScoreEffect.MulFan(MUL, this));
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PreAddScoringAnimationEffectEvent -= OnPreAddScoringEffect;
            player.OnAddSingleAnimationEffectEvent -= PlayerOnOnAddSingleAnimationEffectEvent;
        }

        private void OnPreAddScoringEffect(Permutation perm, Player player, List<IAnimationEffect> effects)
        {
            Block block =
                perm.blocks.FirstOrDefault(b => b.SelectingBy(player) && (b.IsAAA() || b.IsABC()) && b.IsNumbered());
            if (block == null) return;

            effects.Add(new SimpleEffect("effect_artifact_dart", this, player =>
            {
                foreach (var tile in block.tiles)
                {
                    tile.AddTransform(new TileTransformTrivial(this.bounty, tile.GetOrder()), player);
                }
                this.bounty = this.bounty switch
                {
                    Tile.Category.Wan => Tile.Category.Bing,
                    Tile.Category.Bing => Tile.Category.Suo,
                    _ => Tile.Category.Wan
                };
            }).OnBlock(block));
        }
    }

}