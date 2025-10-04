using System.Collections.Generic;

namespace Aotenjo
{
    public class OracleDeck : MahjongDeck
    {
        //TODO: 实装神谕面子
        public OracleDeck() : base("oracle", "shortened", "all", "expert", 9)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel)
        {
            Player player = new OraclePlayer(Hand.PlainFullHand().tiles, seed, this, set, asensionLevel);
            return player;
        }
        


        public override bool IsUnlocked(PlayerStats stats)
        {
            //TODO: 解锁条件
            return true;
        }

        public override MaterialSet[] GetAvailableMaterialSets()
        {
            return new MaterialSet[] { MaterialSet.Ore, MaterialSet.Porcelain, MaterialSet.Monsters, MaterialSet.Wood, MaterialSet.Dessert };
        }
    }

    public class OraclePlayer : Player
    {
        public OraclePlayer(List<Tile> tiles, string seed, OracleDeck oracleDeck, MaterialSet set, int asensionLevel) : base(tiles, seed, oracleDeck, set, asensionLevel)
        {
        }

        public override void OnRoundStart()
        {
            base.OnRoundStart();
            Block randomNewBlock = new Block(GenerateRandomTileGroupWithEffects(3, 100, 0, 0, 0).ToArray());
            Tile jiangTile = GenerateRandomTileWithEffects(1, true)[0];
            CurrentAccumulatedBlock =
                new Permutation(new[] { randomNewBlock }, new Block.Jiang(jiangTile, new Tile(jiangTile)));
        }
    }
}