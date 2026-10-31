namespace Aotenjo
{
    /// <summary>
    /// 此遗物的收益由矿石材质结算入口查询持有关系；这里只保留身份和矿石高亮，不额外计分。
    /// </summary>
    public sealed class AgateIdentificationArtifact : Artifact
    {
        public AgateIdentificationArtifact() : base("agate_identification", Rarity.RARE)
        {
            SetHighlightRequirement(MatchesTile);
        }

        private bool MatchesTile(Tile tile, Player player)
        {
            return tile.CompatWithMaterial(TileMaterial.Ore(), player);
        }
    }
}
