using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Aotenjo
{
    [Serializable]
    public class BambooDeck : MahjongDeck
    {
        public BambooDeck() : base("bamboo_deck", "bamboo", "standard", "intermediate", 3)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel)
        {
            Player player = new BambooDeckPlayer(seed, this, set, asensionLevel);
            return player;
        }

        public override bool IsUnlocked(PlayerStats stats)
        {
            return stats.GetWonNumberByDeck("green_deck") > 0 || stats.GetWonNumberByDeck("blue_deck") > 0 ||
                   stats.GetWonNumberByDeck("galaxy") > 0 || Constants.DEBUG_MODE;
        }
    }

    [Serializable]
    public class BambooDeckPlayer : Player
    {
        [SerializeReference] private List<IndicatorTile> DoraIndicators;

        [SerializeField] private int DoraIndicatorMax;

        public int DefaultIndicatorMax = 5;

        public event Action<PlayerDetermineDoraEvent> DetermineDoraEvent;
        public event Action<PlayerEvent> RevealIndicatorEvent;

        public BambooDeckPlayer(string seed, MahjongDeck deck, MaterialSet materialSet, int ascensionLevel) : base(
            Hand.PlainFullHand().tiles, seed,
            deck, PlayerProperties.DEFAULT, 0, SkillSet.StandardSkillSet(), materialSet, ascensionLevel)
        {
            DoraIndicators = new List<IndicatorTile>();
            DoraIndicatorMax = DefaultIndicatorMax;
        }

        public override bool HasExtraInfo()
        {
            return true;
        }

        public override string GetExtraInformationFromTile(Tile tile, Func<string, string> loc)
        {
            int doraCount = DetermineDora(tile);
            if (doraCount > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format(loc("dora_header_format"), doraCount));
                sb.AppendLine(string.Format(loc("dora_description_format"), CalculateDoraFan(doraCount)));
                return sb.ToString();
            }

            return "";
        }

        public override List<Tile> GetAllTiles()
        {
            return base.GetAllTiles().Union(DoraIndicators.Select(i => i.tile)).ToList();
        }

        public List<IndicatorTile> GetIndicators()
        {
            return new List<IndicatorTile>(DoraIndicators);
        }

        public List<IndicatorTile> GetRevealedIndicator()
        {
            return DoraIndicators.Where(i => i.isRevealed).ToList();
        }

        public override List<IAnimationEffect> GetBaseEffectFromTile(Tile tile)
        {
            List<IAnimationEffect> effects = base.GetBaseEffectFromTile(tile);
            int doraCount = DetermineDora(tile);
            if (doraCount > 0)
            {
                effects.Add(ScoreEffect.AddFan(CalculateDoraFan(doraCount), null).OnTile(tile));
            }
            return effects;
        }

        private int CalculateDoraFan(int doraCount)
        {
            return doraCount * (3 + 2 * ((Level - 1) / 4));
        }

        public override void OnKong(Block block, Permutation perm)
        {
            base.OnKong(block, perm);
            RevealIndicator(1);
        }

        public override void InitHandDeck()
        {
            //再抽手牌
            base.InitHandDeck();
            if (Level % 4 != 1) return;
            //先抽指示牌
            List<Tile> draws = DrawTilesFromPool(1);
            DoraIndicators.AddRange(draws.Select(t => new IndicatorTile(t)));
            if (DoraIndicators.Count > 0)
                DoraIndicators[0].isRevealed = true;
            draws.ForEach(t => TilePool.Remove(t));
        }

        public override void ResetTilePool()
        {
            base.ResetTilePool();
            if (usedBoost && Level % 4 != 0) return;
            TilePool.AddRange(DoraIndicators.Select(i => i.tile));
            DoraIndicators.Clear();
        }

        public int DetermineDora(Tile tile)
        {
            return GetIndicators().Count(i => DetermineDora(tile, i));
        }

        public bool DetermineDora(Tile tile, IndicatorTile indicator)
        {
            if (tile == null) return false;
            PlayerDetermineDoraEvent evt = new PlayerDetermineDoraEvent(this, tile, indicator);
            if (indicator.IsCorrespondingDora(tile)) evt.yes = true;
            DetermineDoraEvent?.Invoke(evt);
            return evt.yes;
        }

        public void RevealIndicator(int v)
        {
            for (int i = 0; i < v; i++)
            {
                PlayerEvent evt = new PlayerEvent(this);
                RevealIndicatorEvent?.Invoke(evt);

                if (evt.canceled) return;

                if (DoraIndicators.Count >= DefaultIndicatorMax) return;
                List<Tile> draws = DrawTilesFromPool(1);
                if (draws.Count == 0) return;
                IndicatorTile newIndicator = new IndicatorTile(draws[0]);
                newIndicator.Reveal(this);

                stats.RecordCustomStats("indicator_revealed", 1);

                DoraIndicators.Add(newIndicator);
                draws.ForEach(t => TilePool.Remove(t));
            }
        }

        public void SwapIndicator(Tile tile, int pos)
        {
            Tile originalIndicator = GetIndicators()[pos].tile;
            GetIndicators()[pos].tile = tile;
            HandDeck.Remove(tile);
            HandDeck.Add(originalIndicator);
        }

        [Serializable]
        public class IndicatorTile
        {
            [SerializeReference] public Tile tile;
            public bool isRevealed;

            public IndicatorTile(Tile tile)
            {
                this.tile = tile;
                isRevealed = false;
            }

            public void Reveal(Player player)
            {
                isRevealed = true;
            }

            public bool IsCorrespondingDora(Tile cand)
            {
                if (!tile.IsSameCategory(cand)) return false;
                if (tile.IsNumbered())
                {
                    if (tile.GetOrder() < 9)
                    {
                        return tile.Succ(cand);
                    }

                    return cand.GetOrder() == 1;
                }

                if (tile.GetCategory() == Tile.Category.Feng)
                {
                    if (tile.GetOrder() < 4)
                    {
                        return cand.GetOrder() == tile.GetOrder() + 1;
                    }

                    return cand.GetOrder() == 1;
                }

                if (tile.GetCategory() == Tile.Category.Jian)
                {
                    if (tile.GetOrder() < 7)
                    {
                        return cand.GetOrder() == tile.GetOrder() + 1;
                    }

                    return cand.GetOrder() == 5;
                }

                throw new ArgumentException("ILLEAGAL ARGUMENT PASSED: " + cand);
            }
        }
    }
}