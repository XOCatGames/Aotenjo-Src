using System;
using System.Collections.Generic;
using System.Linq;
using Aotenjo;

public class BambooTicketArtifact : LevelingArtifact, IPersistantAura
{
    public BambooTicketArtifact() : base("bamboo_ticket", Rarity.RARE, 0)
    {
    }

    public override void ResetArtifactState()
    {
        base.ResetArtifactState();
        Level = 0;
    }

    public override void SubscribeToPlayer(Player player)
    {
        base.SubscribeToPlayer(player);
        player.PostGenerateDestinationEvent += OnPostGenerateDestinationEvent;
    }

    private void OnPostGenerateDestinationEvent(Player player, List<Destination> list)
    {
        List<int> cands = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].IsOnEvent() && !list[i].onSale)
            {
                continue;
            }

            cands.Add(i);
        }

        for (int i = 0; i < Level; i++)
        {
            if (cands.Count == 0) break;
            int index = player.GenerateRandomInt(cands.Count, "bamboo_ticket");

            int destinationIndex = cands[index];

            if (!list[cands[index]].onSale)
            {
                list[destinationIndex].SetOnSale();
            }
            else
            {
                list[destinationIndex] = list[destinationIndex].GetRandomRedEventVariant(player);
                cands.Remove(destinationIndex);
            }
        }

        Level = 0;
    }

    public override void UnsubscribeToPlayer(Player player)
    {
        base.UnsubscribeToPlayer(player);
        player.PostGenerateDestinationEvent -= OnPostGenerateDestinationEvent;
    }

    public override void AddOnRoundEndEffects(Player player, Permutation permutation, List<IAnimationEffect> effects)
    {
        base.AddOnRoundEndEffects(player, permutation, effects);
        if (permutation == null) return;
        if (permutation.ToTiles().All(t => ((BambooDeckPlayer)player).DetermineDora(t) == 0))
        {
            effects.Add(new BambooTicketEffect(this));
        }
    }

    public bool IsAffecting(Player player)
    {
        return Level > 0;
    }

    private class BambooTicketEffect : Effect
    {
        private BambooTicketArtifact goldenTickertArtifact;

        public BambooTicketEffect(BambooTicketArtifact goldenTickertArtifact)
        {
            this.goldenTickertArtifact = goldenTickertArtifact;
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            return func("effect_bamboo_ticket");
        }

        public override Artifact GetEffectSource()
        {
            return goldenTickertArtifact;
        }

        public override void Ingest(Player player)
        {
            goldenTickertArtifact.Level++;
        }
    }
}