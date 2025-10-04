using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aotenjo
{
    public class SneakyDeck : MahjongDeck
    {
        public SneakyDeck() : base("sneaky", "standard", "ore", "insane", 5)
        {
        }

        public override Player CreateNewPlayer(string seed, MaterialSet set, int asensionLevel)
        {
            return new SneakyPlayer(Hand.PlainFullHand().tiles, seed, this, set, asensionLevel);
        }

        public override bool IsUnlocked(PlayerStats stats)
        {
            return Constants.DEBUG_MODE || 
                   stats.GetUnseededRunRecords().Any(rec => rec.acsensionLevel >= 4 && rec.won) ||
                   stats.GetCustomStats(PlayerStatsType.SAME_TILES_TWO_IN_A_ROW) > 0;
        }

        public override string GetAscensionDescription(int ascensionIndex, Func<string, string> loc)
        {
            if (ascensionIndex == 3) return loc("ascension_3_description_schemer");
            return base.GetAscensionDescription(ascensionIndex, loc);
        }
    }

    [Serializable]
    public class SneakyPlayer : Player
    {
        public event PlayerTileEventListener DetermineSneakabilityEvent;
        public event PlayerTileEventListener PreSneakTileEvent;
        public event PlayerTileEventListener PostSneakTileEvent;

        [SerializeReference] public List<Tile> sneakedTiles;

        [SerializeReference] protected List<Tile> sneakedTilesAtRoundStart;

        public SneakyPlayer(List<Tile> tilePool, string randomSeed, MahjongDeck deck, MaterialSet set,
            int ascensionLevel) : base(tilePool, randomSeed, deck, set, ascensionLevel)
        {
            sneakedTiles = new List<Tile>();
            sneakedTilesAtRoundStart = new List<Tile>();
            CloudGlovesGadget glove = new();
            if (GetAscensionLevel() >= 3)
            {
                glove.maxUseCount--;
                glove.uses = glove.maxUseCount;
            }

            AddGadget(glove);
        }

        public override List<Tile> GetAllTiles()
        {
            return base.GetAllTiles().Union(sneakedTiles).ToList();
        }

        public bool SneakedLastRound(Tile tile)
        {
            return sneakedTilesAtRoundStart.Contains(tile);
        }

        public virtual bool CanSneakTile(Tile tile)
        {
            PlayerTileEvent evt = new(this, tile);
            DetermineSneakabilityEvent?.Invoke(evt);
            return !evt.canceled;
        }

        /// <summary>
        /// 藏匿一枚手牌
        /// </summary>
        /// <param name="tile">藏匿的手牌</param>
        /// <returns>摸入的手牌</returns>
        public int SneakHandTile(Tile tile)
        {
            HandDeck.Remove(tile);
            SneakTile(tile);
            return DrawTileToHandDeck();
        }

        /// <summary>
        /// 藏匿一枚牌
        /// </summary>
        /// <param name="tile">藏匿的牌</param>
        public void SneakTile(Tile tile)
        {
            PreSneakTileEvent?.Invoke(new(this, tile));
            sneakedTiles.Add(tile);
            PostSneakTileEvent?.Invoke(new(this, tile));
        }

        public override void InitHandDeck()
        {
            List<Tile> tiles = new(sneakedTiles);
            sneakedTilesAtRoundStart = new List<Tile>(tiles);
            foreach (Tile tile in tiles)
            {
                HandDeck.Add(tile);
                sneakedTiles.Remove(tile);
                if (HandDeck.Count >= GetHandLimit())
                {
                    return;
                }
            }

            MessageManager.Instance.OnTilesEnterHand(tiles);

            base.InitHandDeck();
        }

        public override void OnRoundStart()
        {
            if (Level % 8 == 1 && Level <= 16 && Level != 1)
            {
                Gadget gadget = GetGadgets().FirstOrDefault(g => g is CloudGlovesGadget);
                if (gadget != null)
                    ((CloudGlovesGadget)gadget).Evolve();
            }

            base.OnRoundStart();

            if (Level > 1)
                DiscardLeft = 0;
        }
    }
}