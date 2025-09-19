using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public abstract class StarterBoostEffect
{
    private readonly string name;
    public Tier tier;

    private List<MahjongDeck> deckBlackList = new List<MahjongDeck>();

    public StarterBoostEffect(string name)
    {
        this.name = name;
        tier = Tier.A;
    }

    public StarterBoostEffect WithTier(Tier t)
    {
        tier = t;
        return this;
    }

    public abstract void Boost(Player player);

    public virtual string GetLocalizedName(Player _, Func<string, string> loc)
    {
        return GetLocalizedName(loc);
    }

    public virtual bool IsAvailable(Player player)
    {
        return deckBlackList.All(d => player.deck.regName != d.regName);
    }

    protected void BlockFromDeck(MahjongDeck deck)
    {
        deckBlackList.Add(deck);
    }

    public virtual string GetLocalizedName(Func<string, string> loc)
    {
        return loc($"player_effect_{name}_name");
    }

    public virtual string GetLocalizedDesc(Player _, Func<string, string> loc)
    {
        return GetLocalizedDesc(loc);
    }

    public virtual string GetLocalizedDesc(Func<string, string> loc)
    {
        return loc($"player_effect_{name}_desc");
    }


    public static StarterBoostEffect RandomArtifactEffect = new ArtifactStarterPlayerEffect().WithTier(Tier.A);
    public static StarterBoostEffect RandomArtifactMoreEffect = new ArtifactStarterPlayerEffect.More().WithTier(Tier.S);

    public static StarterBoostEffect RandomArtifactExtremeEffect =
        new ArtifactStarterPlayerEffect.Extreme().WithTier(Tier.SS);

    public static StarterBoostEffect AugmentedNumberedTileEffect = new AugmentStarterPlayerEffect().WithTier(Tier.A);

    public static StarterBoostEffect EraserEffect = new EraserStarterPlayerEffect().WithTier(Tier.SS);

    public static StarterBoostEffect RandomGadgetEffect = new GadgetsStarterPlayerEffect().WithTier(Tier.A);
    public static StarterBoostEffect RandomGadgetLiteEffect = new GadgetsStarterPlayerEffect.Lite().WithTier(Tier.C);
    public static StarterBoostEffect GadgetSlotEffect = new GadgetsStarterPlayerEffect.Slot().WithTier(Tier.B);

    public static StarterBoostEffect MoneyStarterEffect = new MoneyStarterPlayerEffect().WithTier(Tier.A);

    public static StarterBoostEffect MoneyStarterMoreEffect =
        new MoneyStarterPlayerEffect(25, "starter_money_plus").WithTier(Tier.SS);

    public static StarterBoostEffect ChopOneSuitEffect = new OneVoidedSuitPlayerEffect().WithTier(Tier.S);
    public static StarterBoostEffect ChopOneSuitLiteEffect = new OneVoidedSuitPlayerEffect.Lite().WithTier(Tier.B);
    public static StarterBoostEffect ChopOneSuitMoreEffect = new OneVoidedSuitPlayerEffect.More().WithTier(Tier.SSS);

    public static StarterBoostEffect SlotStarterEffect = new SlotStarterPlayerEffect().WithTier(Tier.S);
    public static StarterBoostEffect SlotStarterMoreEffect = new SlotStarterPlayerEffect.More().WithTier(Tier.SSS);

    public static StarterBoostEffect TrimmedDeckEffect = new TrimmedDeckPlayerEffect().WithTier(Tier.A);

    public static StarterBoostEffect HonorStarterEffect = new HonorStarterPlayerEffect().WithTier(Tier.C);
    public static StarterBoostEffect HonorStarterMoreEffect = new HonorStarterPlayerEffect.More().WithTier(Tier.A);

    public static StarterBoostEffect PatternStarterEffect = new PatternStarterPlayerEffect().WithTier(Tier.B);
    public static StarterBoostEffect PatternStarterLiteEffect = new PatternStarterPlayerEffect.Lite().WithTier(Tier.C);

    public static StarterBoostEffect NeonStarterEffect = new NeonStarterPlayerEffect().WithTier(Tier.A);
    public static StarterBoostEffect NeonStarterLiteEffect = new NeonStarterPlayerEffect.Lite().WithTier(Tier.B);

    public static StarterBoostEffect RookStarterEffect = new RookStarterPlayerEffect().WithTier(Tier.A);

    public static StarterBoostEffect FlowerStarterEffect = new FlowerStarterPlayerEffect().WithTier(Tier.B);
    public static StarterBoostEffect FlowerStarterPlusEffect = new FlowerStarterPlayerEffect.Plus().WithTier(Tier.SS);

    public static StarterBoostEffect[][] StarterBoostEffects =
    {
        new[] { RandomArtifactEffect, RandomArtifactMoreEffect, RandomArtifactExtremeEffect },
        new[] { AugmentedNumberedTileEffect },
        new[] { EraserEffect },
        new[] { RandomGadgetEffect, RandomGadgetLiteEffect, GadgetSlotEffect },
        new[] { MoneyStarterEffect, MoneyStarterMoreEffect },
        new[] { ChopOneSuitEffect, ChopOneSuitLiteEffect, ChopOneSuitMoreEffect },
        new[] { SlotStarterEffect, SlotStarterMoreEffect },
        new[] { TrimmedDeckEffect },
        new[] { HonorStarterEffect, HonorStarterMoreEffect },
        new[] { PatternStarterEffect, PatternStarterLiteEffect },
        new[] { RookStarterEffect },
        new[] { FlowerStarterEffect, FlowerStarterPlusEffect },
        new[] { NeonStarterEffect, NeonStarterLiteEffect }
    };

    //SSS
    public static StarterBoostEffect LessGadgetSlotEffect = new LessGadgetSlotEffect();

    public static StarterBoostEffect HyperLoseMoeneyEffect = new LoseMoeneyPlayerEffect(15);

    //public static StarterBoostEffect LoseFreePatternPackEffect = new LoseFreePatternPackEffect();
    //SS
    public static StarterBoostEffect HardPunishedTargetEffect = new PunishedTargetPlayerEffect(20);

    public static StarterBoostEffect CorruptionEffect = new CorruptionPlayerEffect(12);

    //S
    public static StarterBoostEffect PunishedTargetEffect = new PunishedTargetPlayerEffect(10);

    public static StarterBoostEffect LoseMoeneyEffect = new LoseMoeneyPlayerEffect(5);

    //B
    public static StarterBoostEffect GainMoneyEffect = new MoneyStarterPlayerEffect(5);
    public static StarterBoostEffect LessenTargetEffect = new LessenTargetPlayerEffect(10);

    public static StarterBoostEffect CommonArtifactEffect = new ArtifactStarterPlayerEffect.Common();

    //C
    public static StarterBoostEffect GainMoreMoneyEffect = new MoneyStarterPlayerEffect(10);

    public static Dictionary<Tier, StarterBoostEffect[]> SideEffects = new()
    {
        { Tier.SSS, new[] { LessGadgetSlotEffect, HyperLoseMoeneyEffect } },
        { Tier.SS, new[] { HardPunishedTargetEffect, CorruptionEffect } },
        { Tier.S, new[] { PunishedTargetEffect, LoseMoeneyEffect } },
        { Tier.A, new StarterBoostEffect[] { } },
        { Tier.B, new[] { GainMoneyEffect, LessenTargetEffect, CommonArtifactEffect } },
        { Tier.C, new[] { GainMoreMoneyEffect, GadgetSlotEffect } }
    };

    public enum Tier
    {
        SSS,
        SS,
        S,
        A,
        B,
        C
    }
}