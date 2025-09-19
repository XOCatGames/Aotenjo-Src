using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class AddFlowerTileDestination : Destination
{
    public AddFlowerTileDestination(Player player) : base("add_flower_tile_shop", false, player)
    {
    }

    public override bool IsOnEvent()
    {
        return true;
    }

    public override Destination GetRandomRedEventVariant(Player player)
    {
        return new AddFlowerTileDestination(player);
    }

    public List<List<Tile>> drawFlowers()
    {
        List<Tile> seasons = new Hand("1234f").tiles;
        List<Tile> flowers = new Hand("1234g").tiles;
        List<Tile> arts = new Hand("1234h").tiles;
        List<Tile> jobs = new Hand("1234i").tiles;

        List<List<Tile>> collection = new List<List<Tile>>
        {
            seasons,
            flowers,
            arts,
            jobs
        };

        List<List<Tile>> cands = new List<List<Tile>>();

        foreach (List<Tile> list in collection)
        {
            foreach (Tile tile in list)
            {
                if (player.GetAllTiles().Any(t => t.ToString() == tile.ToString()))
                {
                    goto end;
                }
            }

            cands.Add(list);

            end: ;
        }

        LotteryPool<List<Tile>> pool = new LotteryPool<List<Tile>>();
        pool.AddRange(cands);

        List<List<Tile>> res = new List<List<Tile>>();

        for (int i = 0; i < Math.Min(2, cands.Count); i++)
        {
            res.Add(pool.Draw(player.GenerateRandomInt, false));
        }

        while (res.Count < 2)
        {
            List<List<Tile>> list = collection.Where(l => !res.Contains(l)).ToList();
            res.Add(list[player.GenerateRandomInt(list.Count)]);
        }

        return res;
    }
}