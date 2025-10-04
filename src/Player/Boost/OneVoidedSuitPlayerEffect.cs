using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class OneVoidedSuitPlayerEffect : StarterBoostEffect
{
    public OneVoidedSuitPlayerEffect() : base("starter_one_voided_suit")
    {
    }

    public override void Boost(Player player)
    {
        Tile.Category cat = (Tile.Category)player.GenerateRandomInt(3);

        List<Tile> removedList = new List<Tile>();

        for (int i = 0; i < 18; i++)
        {
            Tile[] cands = player.GetAllTiles().Where(t => t.GetCategory() == cat).ToArray();
            Tile toRemove = cands[player.GenerateRandomInt(cands.Count())];
            player.RemoveTileFromPool(toRemove);
            removedList.Add(toRemove);
        }

        removedList.Sort();

        EventManager.Instance.OnRemoveTileEvent(removedList);
    }

    public class Lite : StarterBoostEffect
    {
        public Lite() : base("starter_one_voided_suit_lite")
        {
        }

        public override void Boost(Player player)
        {
            Tile.Category cat = (Tile.Category)player.GenerateRandomInt(3);
            List<Tile> removedList = new List<Tile>();
            for (int i = 0; i < 12; i++)
            {
                Tile[] cands = player.GetAllTiles().Where(t => t.GetCategory() == cat).ToArray();
                Tile toRemove = cands[player.GenerateRandomInt(cands.Count())];
                player.RemoveTileFromPool(toRemove);
                removedList.Add(toRemove);
            }

            removedList.Sort();
            EventManager.Instance.OnRemoveTileEvent(removedList);
        }
    }

    public class More : StarterBoostEffect
    {
        public More() : base("starter_one_voided_suit_plus")
        {
        }

        public override void Boost(Player player)
        {
            Tile.Category cat = (Tile.Category)player.GenerateRandomInt(3);
            List<Tile> removedList = player.GetAllTiles().Where(t => t.GetCategory() == cat).ToList();
            removedList.ForEach(t => player.RemoveTileFromPool(t));
            removedList.Sort();
            EventManager.Instance.OnRemoveTileEvent(removedList);
        }
    }
}