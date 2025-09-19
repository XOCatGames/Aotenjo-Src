using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class LightningBallArtifact : Artifact
    {
        private bool status;

        public LightningBallArtifact() : base("lightning_ball", Rarity.COMMON)
        {
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            status = false;
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return localizer("artifact_lightning_ball_description_" + (status ? "break" : "enhance"));
        }

        public override string Serialize()
        {
            return status.ToString();
        }

        public override void Deserialize(string data)
        {
            status = bool.Parse(data);
        }

        public override void AddOnRoundEndEffects(Player player, Permutation permutation,
            List<IAnimationEffect> effects)
        {
            base.AddOnRoundEndEffects(player, permutation, effects);
            effects.Add(new LightningBallEffect(this, status));
        }

        private class LightningBallEffect : Effect
        {
            private LightningBallArtifact lightningBallArtifact;
            private bool status;

            public LightningBallEffect(LightningBallArtifact lightningBallArtifact, bool status)
            {
                this.lightningBallArtifact = lightningBallArtifact;
                this.status = status;
            }

            public override string GetEffectDisplay(Func<string, string> func)
            {
                if (status)
                {
                    return func("effect_lightning_ball_break");
                }

                return func("effect_lightning_ball_enhanced");
            }

            public override Artifact GetEffectSource()
            {
                return lightningBallArtifact;
            }

            public override void Ingest(Player player)
            {
                status = lightningBallArtifact.status;
                List<Tile> cands;
                if (lightningBallArtifact.status)
                {
                    cands = player.GetHandDeckCopy().Where(t => t.IsNumbered() && t.GetOrder() > 6).ToList();
                }
                else
                {
                    cands = player.GetHandDeckCopy().Where(t => t.IsNumbered() && t.GetOrder() < 4).ToList();
                }

                List<Tile> targets = new List<Tile>();
                for (int i = 0; i < 2; i++)
                {
                    if (cands.Count == 0) break;
                    Tile tile = cands[player.GenerateRandomInt(cands.Count)];
                    if (tile == null) continue;
                    targets.Add(tile);
                    cands.Remove(tile);
                }

                if (lightningBallArtifact.status)
                {
                    targets.ForEach(t => t.SetMask(TileMask.Fractured(), player));
                }
                else
                {
                    targets.ForEach(t => t.MergeAndSetProperties(player.GenerateRandomTileProperties(0, 95, 5, 0), player));
                }

                lightningBallArtifact.status = !lightningBallArtifact.status;
            }

            public override string GetSoundEffectName()
            {
                return status ? "Fracture" : "Stimulant";
            }
        }
    }
}