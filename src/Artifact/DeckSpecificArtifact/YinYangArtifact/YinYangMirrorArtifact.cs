namespace Aotenjo
{
    public class YinYangMirrorArtifact : Artifact
    {
        public YinYangMirrorArtifact() : base("yin_yang_mirror", Rarity.RARE)
        {
        }


        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.DetermineShiftedPairEvent += OnDetermineShiftedPair;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.DetermineShiftedPairEvent -= OnDetermineShiftedPair;
        }


        private void OnDetermineShiftedPair(PlayerDetermineShiftedPairEvent evt)
        {
            if (evt.res || evt.step != 0) return;

            if (!evt.b1.IsABC() || !evt.b2.IsABC()) return;

            Tile mid1 = evt.b1.tiles[1];
            Tile mid2 = evt.b2.tiles[1];

            bool sameMiddle =
                mid1.GetCategory() == mid2.GetCategory() &&
                mid1.GetOrder() == mid2.GetOrder();

            if (!sameMiddle) return;

            evt.res = true;
        }
    }
}