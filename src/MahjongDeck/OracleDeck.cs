using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public class OracleDeck : MahjongDeck
    {
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
            return Constants.DEBUG_MODE ||
                   stats.GetUnseededRunRecords().Any(rec => rec.acsensionLevel >= 4 && rec.won);
        }
    }

    [Serializable]
    public class OraclePlayer : Player
    {

        [SerializeField] public Block oracleBlock;
        
        public OraclePlayer(List<Tile> tiles, string seed, OracleDeck oracleDeck, MaterialSet set, int ascensionLevel) : 
            base(tiles, seed, oracleDeck,PlayerProperties.DEFAULT, 0, SkillSet.ShortenedSkillSet(), set, ascensionLevel)
        {
        }
        

        public override void PostRoundStart()
        {
            base.PostRoundStart();
            Block randomNewBlock = GenerateRandomBlock();
            Tile jiangTile = GenerateRandomTileWithEffects(1, true)[0];
            var jiang = new Block.Jiang(jiangTile, new Tile(jiangTile));
            
            var drawEvent = new PlayerSetOracleEvent.Draw(this, CurrentAccumulatedBlock, randomNewBlock, jiang);
            EventBus.Publish(drawEvent);

            var preEvt = new PlayerSetOracleEvent.Pre(this, CurrentAccumulatedBlock, drawEvent.block, drawEvent.jiang);
            EventBus.Publish(preEvt);
            
            CurrentAccumulatedBlock =
                new Permutation(new[] { preEvt.block }, preEvt.jiang);
            
            oracleBlock = preEvt.block;
            
            var postEvt = new PlayerSetOracleEvent.Post(this, CurrentAccumulatedBlock);
            EventBus.Publish(postEvt);
            
            var finalEvt = new PlayerSetOracleEvent.Final(this, CurrentAccumulatedBlock);
            EventBus.Publish(finalEvt);

            CurrentPlayingStage++;
        }

        public override bool EraseBlock(Block block)
        {
            if(block == oracleBlock) return false;
            return base.EraseBlock(block);
        }

        public override double GetYakuMultiplier(YakuType yakuType)
        {
            double baseMul = base.GetYakuMultiplier(yakuType);
            Permutation permutation = GetCurrentSelectedPerm();
            if (permutation == null || !inRound) return baseMul;
            
            YakuTester.TestYaku(permutation, yakuType, this, out var relatedTiles);
            
            if(oracleBlock.Any(t => relatedTiles.Contains(t))) return baseMul * 2;
            return baseMul;
        }

        public class PlayerSetOracleEvent : PlayerPermutationEvent
        {
            public PlayerSetOracleEvent(Player player, Permutation permutation) : base(player, permutation)
            {
            }
            
            public class Draw : PlayerSetOracleEvent
            {
                public Block block;
                public Block.Jiang jiang;

                public Draw(Player player, Permutation permutation, Block block, Block.Jiang jiang) : base(player, permutation)
                {
                    this.block = block;
                    this.jiang = jiang;
                }
            }
            
            public class Pre : PlayerSetOracleEvent
            {
                public Block block;
                public Block.Jiang jiang;

                public Pre(Player player, Permutation permutation, Block block, Block.Jiang jiang) : base(player, permutation)
                {
                    this.block = block;
                    this.jiang = jiang;
                }
            }
            
            public class Post : PlayerSetOracleEvent
            {
                public Post(Player player, Permutation permutation) : base(player, permutation)
                {
                }
            }
        
            public class Final : PlayerSetOracleEvent
            {
                public Final(Player player, Permutation permutation) : base(player, permutation)
                {
                }
            }
        }
    }
}