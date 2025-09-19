using System;

namespace Aotenjo
{
    public class RustAxeArtifact : Artifact
    {
        public RustAxeArtifact() : base("rust_axe", Rarity.RARE)
        {
            
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            if (player is ScarletPlayer scarlet)
            {
                scarlet.DetermineScarletCoreCategoryEvent += OnDetermineCategory;
            }
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            if (player is ScarletPlayer scarlet)
            {
                scarlet.DetermineScarletCoreCategoryEvent -= OnDetermineCategory;
            }
        }

        private void OnDetermineCategory(PlayerDetermineScarletCoreCategoryEvent evt)
        {
            if (evt.player is not ScarletPlayer scarlet) return;

            var core = scarlet.GetScarletCore().data;

            if (evt.type == ScarletCategoryType.Main && evt.category == core.subCategory)
            {
                evt.rawResult = true;
            }
            else if (evt.type == ScarletCategoryType.Sub && evt.category == core.mainCategory)
            {
                evt.rawResult = true;
            }
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer));
        }
    }
}