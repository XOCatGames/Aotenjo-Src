using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class MonsterBirthCertificateArtifact : Artifact
    {
        public TileMaterial monsterMaterial;

        public MonsterBirthCertificateArtifact() : base("monster_birth_certificate", Rarity.RARE)
        {
            
            SetHighlightRequirement((t, p) => p.DetermineMaterialCompactbility(t, TileMaterial.Nest()));
        }

        public override string Serialize()
        {
            return monsterMaterial.GetRegName();
        }

        public override void Deserialize(string data)
        {
            monsterMaterial = TileMaterial.GetMaterial(data);
        }

        protected override int GetSpriteID()
        {
            string ghostName = TileMaterial.Ghost().GetRegName();
            string succubusName = TileMaterial.Succubus().GetRegName();
            string goldMouseName = TileMaterial.GoldMouse().GetRegName();
            string taotieName = TileMaterial.Taotie().GetRegName();

            //store to a hash map
            Dictionary<string, int> monsterNameToID = new Dictionary<string, int>();

            monsterNameToID.Add(ghostName, 180);
            monsterNameToID.Add(succubusName, 181);
            monsterNameToID.Add(goldMouseName, 182);
            monsterNameToID.Add(taotieName, 183);

            return monsterNameToID[monsterMaterial.GetRegName()];
        }

        public override void PreGameInitialized(Player player)
        {
            monsterMaterial = new LotteryPool<TileMaterial>()
                .AddRange(MaterialSet.Monsters.GetMaterials().Where(m =>
                    m.GetRarity() == Rarity.COMMON && m.GetRegName() != TileMaterial.Nest().GetRegName()))
                .Draw(player.GenerateRandomInt);
        }

        public override string GetDescription(Player player, Func<string, string> loc)
        {
            return string.Format(base.GetDescription(player, loc), loc(monsterMaterial.GetLocalizeKey()));
        }
    }
}