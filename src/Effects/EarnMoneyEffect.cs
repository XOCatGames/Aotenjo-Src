using System;

namespace Aotenjo
{
    [Serializable]
    public class EarnMoneyEffect : Effect
    {
        public readonly int amount;
        public readonly Artifact artifact;

        public EarnMoneyEffect(int amount)
        {
            this.amount = amount;
            artifact = null;
        }

        public EarnMoneyEffect(int amount, Artifact artifact)
        {
            this.amount = amount;
            this.artifact = artifact;
        }

        public override bool NoDefaultSound()
        {
            return true;
        }

        public override string GetSoundEffectName()
        {
            if (amount >= 0)
                return "EarnMoney";
            return "SpendMoney";
        }

        public override string GetEffectDisplay(Func<string, string> func)
        {
            if (amount >= 0)
                return string.Format(func("effect_earn_money"), amount);
            return string.Format(func("effect_lose_money"), -amount);
        }

        public override Artifact GetEffectSource()
        {
            return artifact;
        }

        public override void Ingest(Player player)
        {
            player.EarnMoney(amount);
        }
    }
}