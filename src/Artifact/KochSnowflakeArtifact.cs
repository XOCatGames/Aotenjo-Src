using System.Linq;

namespace Aotenjo
{
    public class KochSnowflakeArtifact : Artifact
    {
        public KochSnowflakeArtifact() : base("koch_snowflake", Rarity.EPIC)
        {
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.PostSettlePermutationEvent += OnSettlePermutation;
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.PostSettlePermutationEvent -= OnSettlePermutation;
        }

        private void OnSettlePermutation(PlayerPermutationEvent permutationEvent)
        {
            //融合
            Player player = permutationEvent.player;
            BlockCombinator combinator = player.GetCombinator();
            Permutation perm = permutationEvent.permutation;
            if (perm.GetPermType() != PermutationType.NORMAL) return;
            foreach (Block block in perm.blocks)
            {
                if (block.IsABC())
                {
                    Block nextSeq = perm.blocks.FirstOrDefault(b => b != block && combinator.ASuccB(block, b));
                    if (nextSeq != null)
                    {
                        Block nextNextSeq = perm.blocks.FirstOrDefault(b =>
                            b != block && b != nextSeq && combinator.ASuccB(nextSeq, b));
                        if (nextNextSeq == null)
                        {
                            continue;
                        }

                        Block newBlock = new Block(new[] { block.tiles[0], nextSeq.tiles[0], nextNextSeq.tiles[0] });
                        if (block.CompatWith(new Block("567z")))
                        {
                            newBlock = new Block(new[] { block.tiles[0], nextSeq.tiles[1], nextNextSeq.tiles[2] });
                        }

                        perm.blocks = perm.blocks.Where(b => b != block && b != nextSeq && b != nextNextSeq)
                            .Append(newBlock).ToArray();

                        player.CurrentPlayingStage -= 2;

                        return;
                    }
                }

                if (block.IsAAA())
                {
                    Block nextTri = perm.blocks.FirstOrDefault(b => b != block && block.CompatWith(b));
                    if (nextTri != null)
                    {
                        Block nextNexTri =
                            perm.blocks.FirstOrDefault(b => b != block && b != nextTri && nextTri.CompatWith(b));
                        if (nextNexTri == null)
                        {
                            continue;
                        }

                        Block newBlock = new Block(new[] { block.tiles[0], nextTri.tiles[0], nextNexTri.tiles[0] });

                        perm.blocks = perm.blocks.Where(b => b != block && b != nextTri && b != nextNexTri)
                            .Append(newBlock).ToArray();

                        player.CurrentPlayingStage -= 2;

                        return;
                    }
                }
            }
        }
    }
}