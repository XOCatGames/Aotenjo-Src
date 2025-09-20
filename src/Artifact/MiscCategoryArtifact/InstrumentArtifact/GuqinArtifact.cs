using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aotenjo;

public class GuqinArtifact : InstrumentArtifact
{
    public GuqinArtifact() : base(7, "guqin", Rarity.RARE)
    {
    }

    public override string GetDescription(Player player, Func<string, string> func)
    {
        StringBuilder sb = new();
        SettleRecord rec = GetLastHand(player);
        if (rec == null)
        {
            sb.AppendLine(func("ui_none"));
        }
        else
        {
            List<YakuType> validYakus = rec.ActivatedYakuTypes.Where(y =>
                !rec.ActivatedYakuTypes.Any(t => YakuTester.GetYakuChildsExcludeSelf(t).Contains(y))).ToList();
            foreach (var yakuName in validYakus.Select(y => YakuTester.InfoMap[y].GetFormattedName(func)))
            {
                sb.AppendLine(yakuName);
            }
        }

        int chordNum = maxCounter - currentCounter;
        if (IsActivating())
        {
            return string.Format(func($"artifact_{GetNameID()}_description_ready"), chordNum, sb);
        }

        return string.Format($"{base.GetDescription(func)}", chordNum, sb);
    }

    public override string GetInShopDescription(Player player, Func<string, string> localizer)
    {
        return localizer("artifact_guqin_description_inshop");
    }


    public override void AppendOnTileEffects(Player player, Permutation permutation, Tile tile, List<Effect> effects)
    {
        base.AppendOnTileEffects(player, permutation, tile, effects);
        if (!player.Selecting(tile) || !IsActivating() || !player.GetCurrentSelectedBlocks().First().IsABC()) return;
        effects.Add(new TextEffect("effect_listening_guqin"));
    }

    protected override bool CanPlay(Player player, Permutation perm, List<Effect> lst, Block block)
    {
        return block.IsABC();
    }

    protected override void OnPlay(Player player)
    {
        SettleRecord rec = GetLastHand(player);

        if (rec == null) return;

        List<YakuType> validYakus = rec.ActivatedYakuTypes
            .Where(y => !rec.ActivatedYakuTypes.Any(t => YakuTester.GetYakuChildsExcludeSelf(t).Contains(y))).ToList();
        validYakus.ForEach(y => { player.GetSkillSet().IncreaseLevel(y); });
    }

    private static SettleRecord GetLastHand(Player player)
    {
        SettleRecord rec = null;
        var seq = player.stats.GetPlayedHands();

        if (player.CurrentPlayingStage == 0)
        {
            if (seq.Count == 0) return null;
            rec = seq[^1];
        }
        else
        {
            for (int i = seq.Count - 1; i > 0; i--)
            {
                if (seq[i].level == player.Level - 1)
                {
                    rec = seq[i];
                    break;
                }
            }
        }

        return rec;
    }

    protected override string GetSoundEffectName()
    {
        return "Guqin";
    }
}