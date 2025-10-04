using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterialJacarandaWood : TileMaterialWood
    {
        private const double MUL = 1.5;

        public TileMaterialJacarandaWood(int ID) : base(ID, "jacaranda_wood")
        {
        }

        protected override string GetDescription(Func<string, string> localizer)
        {
            return string.Format(base.GetDescription(localizer), Utils.NumberToFormat(MUL));
        }

        public override void AppendBonusEffects(Player player, Permutation perm, Tile tile, List<Effect> effects)
        {
            base.AppendBonusEffects(player, perm, tile, effects);
            effects.Add(ScoreEffect.MulFan(MUL, null));
        }

        public override void AppendToListDiscardEffect(Player player, Permutation perm, List<IAnimationEffect> effects,
            Tile tile, bool withForce, bool isClone)
        {
            base.AppendToListDiscardEffect(player, perm, effects, tile, withForce, isClone);

            Tile newTile = LotteryPool<Tile>.DrawFromCollection(
                player.GetUniqueFullDeck().Where(t => t.GetCategory() == tile.GetCategory()), player.GenerateRandomInt);
            newTile.properties = player.GenerateRandomTileProperties(0, 90, 10, 0);
            effects.Add(new SimpleEffect("effect_jacaranda_wood", null, p =>
            {
                p.AddTileToDiscarded(newTile);
                EventManager.Instance.OnAddTileEvent(new List<Tile> { newTile });
            }).OnTile(tile));
            if (withForce)
            {
                newTile.AppendDiscardEffects(player, perm, effects, false, tile, false);
            }
        }

        public override Rarity GetRarity()
        {
            return Rarity.RARE;
        }
    }
}