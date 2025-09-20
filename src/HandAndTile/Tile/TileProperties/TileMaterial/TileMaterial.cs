using System;
using System.Linq;

namespace Aotenjo
{
    [Serializable]
    public class TileMaterial : TileAttribute
    {
        public TileMaterial(int ID, string nameKey, Effect effect) : base(ID, nameKey + "_material", effect)
        {
        }

        public virtual int GetOrnamentSpriteID(Player player)
        {
            return -1;
        }
        
        public virtual int GetShadowID()
        {
            return 19;
        }

        protected override string GetSpriteSheetName()
        {
            return "TileMaterials";
        }

        protected override string GetSpriteNamespaceID(Player player, string nmSpace = "aotenjo")
        {
            return $"tile_material:{nmSpace}:{GetRegName()}";
        }

        public override string GetShortLocalizeKey()
        {
            return base.GetShortLocalizeKey() + "_short";
        }

        public virtual TileMaterial Copy()
        {
            return this;
        }

        public virtual Rarity GetRarity()
        {
            return Rarity.COMMON;
        }

        public static readonly TileMaterial PLAIN = new(0, "plain", null);

        // Ore
        public static TileMaterial Chocolate() => new TileMaterialChocolate(4);
        public static readonly TileMaterial GOLDEN = new TileMaterialGolden(2);
        public static readonly TileMaterial CRYSTAL = new(3, "crystal", ScoreEffect.AddFu(30, null));
        public static TileMaterial Ore() => new TileMaterialOre(7);
        public static readonly TileMaterial COPPER = new(8, "copper", ScoreEffect.AddFan(4, null));
        public static TileMaterial Jade() => new TileMaterialJade(6);

        public static TileMaterial Agate() => new TileMaterialAgate(16);
        public static TileMaterial Voidstone() => new TileMaterialVoidstone(18);

        // Porcelain
        public static TileMaterial BlueAndWhitePorcelain() => new TileMaterialBlueAndWhite(26);

        public static TileMaterial BonePorcelain() => new TileMaterialBonePorcelain(24);

        public static TileMaterial GREEN_PORCELAIN = new TileMaterialGreenPorcelain(25);

        public static TileMaterial FAMILLE_VERTE_PORCELAIN = new TileMaterialFamilleVertePorcelain(28);

        public static TileMaterial WHITE_PORCELAIN = new TileMaterialWhitePorcelain(27);

        public static TileMaterial PINK_PORCELAIN = new TileMaterialPinkPorcelain(23);
        public static TileMaterial MysteriousColorPorcelain() => new TileMaterialSecretColorPorcelain(22);

        // Monsters
        public static TileMaterial Ghost() => new TileMaterialGhost(36);
        public static TileMaterial GoldMouse() => new TileMaterialGoldMouse(32);
        public static TileMaterial Taotie() => new TileMaterialTaotie(33);
        public static TileMaterial Succubus() => new TileMaterialSuccubus(38);
        public static TileMaterial Nest() => new TileMaterialNest(37);
        public static TileMaterial Mo() => new TileMaterialMo(34);
        public static TileMaterial Demon() => new TileMaterialDemon(5);

        //Wood
        public static TileMaterial NanmuWood() => new TileMaterialNanmuWood(42);
        public static TileMaterial PaleWood() => new TileMaterialPaleWood(43);
        public static TileMaterial EmeraldWood() => new TileMaterialEmeraldWood(44);
        public static TileMaterial MistWood() => new TileMaterialMistWood(45);
        public static TileMaterial HellWood() => new TileMaterialHellWood(46);
        public static TileMaterial JacarandaWood() => new TileMaterialJacarandaWood(47);
        public static TileMaterial PaoRosaWood() => new TileMaterialPaoRosaWood(48);

        //Dessert
        public static TileMaterial Butter() => new TileMaterialButter(214);
        public static TileMaterial ChocolateDessert() => new TileMaterialChocolateDessert(211);
        public static TileMaterial Jelly() => new TileMaterialJelly(215);
        public static TileMaterial MilleFeuille() => new TileMaterialMilleFeuille(212);
        public static TileMaterial SugarCube() => new TileMaterialSugarCube(213);
        public static TileMaterial IceCream() => new TileMaterialIceCream(216);
        public static TileMaterial Lollipop() => new TileMaterialLollipop(210);
        
        public static TileMaterial[] Materials() => MaterialProviders.Select(p => p()).ToArray();
        
        public static Func<TileMaterial>[] MaterialProviders = new Func<TileMaterial>[]
        {
            () => PLAIN, Chocolate, () => GOLDEN, () => CRYSTAL, Ore, () => COPPER, Jade, Demon, Agate, Voidstone,
            BlueAndWhitePorcelain, BonePorcelain, () => GREEN_PORCELAIN, () => FAMILLE_VERTE_PORCELAIN, () => WHITE_PORCELAIN,
            () => PINK_PORCELAIN, MysteriousColorPorcelain, Ghost, GoldMouse, Taotie, Succubus, Nest, Mo,
            NanmuWood, PaleWood, EmeraldWood, MistWood, HellWood, JacarandaWood, PaoRosaWood,
            Butter, ChocolateDessert, Jelly, MilleFeuille, SugarCube, IceCream, Lollipop
        };

        public static TileMaterial GetMaterial(string material)
        {
            string toSearch = material.EndsWith("_material") ? material : $"{material}_material";
            if (Materials().Any(m => m.GetRegName() == toSearch))
                return Materials().First(m => m.GetRegName() == toSearch);
            throw new InvalidOperationException($"Material {toSearch} not found");
        }

    }
}