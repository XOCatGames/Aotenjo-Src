using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class YinYangJadeFluteArtifact : InstrumentArtifact, IJade, IMultiplierProvider
    {
        private const float BASE_MUL = 1.5f; // 基础倍率
        private const float INC_STEP = 0.3f; // 每次升级增加的倍率
        private const int CHARGE_N = 2; // 每形态需要次数

        private enum Mode
        {
            Yin,
            Yang
        }

        protected override int GetSpriteID()
        {
            return base.GetSpriteID() + (mode == Mode.Yang ? 1 : 0);
        }

        private Mode mode = Mode.Yin; // 当前形态
        public int Level { get; set; }

        public int GetLevel(Player player)
        {
            return Level;
        }
        public YinYangJadeFluteArtifact() : base(CHARGE_N, "yin_yang_jade_flute", Rarity.EPIC)
        {
        }

        public override void AppendOnSelfEffects(Player player, Permutation perm, List<Effect> effects)
        {
            //先触发加番
            effects.Add(ScoreEffect.MulFan(GetMul(player), this));
            base.AppendOnSelfEffects(player, perm, effects);
        }

        protected override bool CanPlay(Player player, Permutation perm, List<Effect> _, Block block)
        {
            bool isYin = block.IsYinSeq();
            bool isYang = block.IsABC() && !isYin;

            return (mode == Mode.Yin && isYin) ||
                   (mode == Mode.Yang && isYang);
        }

        protected override void OnPlay(Player player)
        {
            if (mode == Mode.Yin) // 阴 → (倍率 +0.3) & 转阳
            {
                Level++;
                mode = Mode.Yang;
            }
            else // 阳 → 补充道具 & 转阴
            {
                foreach (var g in player.GetGadgets().Where(g => !g.IsConsumable()))
                {
                    if (g is ReusableGadget rg)
                    {
                        int count = rg.maxUseCount;
                        //TODO: 需要换实现，这个实现耦合太高了
                        if (player.GetArtifacts().Contains(Artifacts.SpringArm)) count++;
                        
                        if(g.uses < count)
                            g.uses = Math.Min(g.uses + 1, count);
                    }
                }

                mode = Mode.Yin;
            }
        }

        protected override string GetEffectDisplay(Player p, Func<string, string> loc)
        {
            int chordNum = maxCounter - currentCounter;
            string key = mode == Mode.Yin ? "yin" : "yang";
            //例: effect_instrument_yin_yang_jade_flute_yin
            return string.Format(loc($"effect_instrument_{GetNameID()}_{key}"), chordNum);
        }

        public override string GetDescription(Player p, Func<string, string> loc)
        {
            int chordNum = maxCounter - currentCounter;
            bool ready = IsActivating();
            bool inYinMode = mode == Mode.Yin;

            string modeKey = inYinMode ? "yin" : "yang";

            string readyKey = ready ? "_ready" : "";

            //例：artifact_yin_yang_jade_flute_description_yin_ready
            string fullKey = $"artifact_{GetNameID()}_description_{modeKey}{readyKey}";

            return string.Format(loc(fullKey), chordNum, GetMul(p));
        }

        protected override string GetSoundEffectName() => "Flute";

        public override string Serialize()
        {
            return $"{base.Serialize()}|{Level}|{(int)mode}";
        }

        public override void Deserialize(string data)
        {
            var parts = data.Split('|');
            base.Deserialize(parts[0]);

            if (parts.Length >= 2 && int.TryParse(parts[1], out int lv))
                Level = lv;

            if (parts.Length >= 3 && int.TryParse(parts[2], out int md))
                mode = (Mode)md;
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            Level = 0;
            mode = Mode.Yin;
        }

        public double GetMul(Player player) => BASE_MUL + this.GetEffectiveJadeStack(player) * INC_STEP;
    }
}