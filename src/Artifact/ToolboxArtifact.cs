using System;

namespace Aotenjo
{
    public class ToolboxArtifact : Artifact
    {
        private const int AMOUNT = 1;

        public ToolboxArtifact() : base("toolbox", Rarity.COMMON)
        {
        }

        public override string GetDescription(Player player, Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(player, localizer), AMOUNT);
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostUseGadgetEvent += OnUseTool;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostUseGadgetEvent -= OnUseTool;
        }

        public void OnUseTool(PlayerGadgetEvent evt)
        {
            evt.player.EarnMoney(AMOUNT);
            MessageManager.Instance.OnArtifactEarnMoney(AMOUNT, this);
        }
    }
}