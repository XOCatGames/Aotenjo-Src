using System;

namespace Aotenjo
{
    public class RustCrookArtifact : Artifact
    {
        private const int AMOUNT = 1; //光环level提升量

        public RustCrookArtifact() : base("rust_crook", Rarity.RARE)
        {
            
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);

            if (player is ScarletPlayer scarlet)
            {
                scarlet.GetScarletCoreLevelEvent += OnGetCoreLevel;
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);

            if (player is ScarletPlayer scarlet)
            {
                scarlet.GetScarletCoreLevelEvent -= OnGetCoreLevel;
            }
        }

        private void OnGetCoreLevel(PlayerGetScarletCoreLevelEvent evt)
        {
            if (evt.player is not ScarletPlayer scarlet) return;

            if (!scarlet.IsCompatibleWithSubCategory(evt.cat)) return;

            evt.level += AMOUNT;
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), AMOUNT);
        }
    }
}