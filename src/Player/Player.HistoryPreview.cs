using System;
using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public partial class Player
    {
        // 历史上下文使用隔离的展示目录，不执行普通开局、随机预初始化或订阅。
        // 历史未保存等级，因此这些对象只承担默认说明；真实名称和图片优先使用历史快照。
        public static Player CreateHistoryPreview(RoundRecord record) => new Player(record);

        private Player(RoundRecord record)
        {
            stats = record.roundStats ?? PlayerStats.New();
            deck = MahjongDeck.decks.FirstOrDefault(d => d.regName == record.deckName) ?? MahjongDeck.BlueDeck;
            materialSet = MaterialSet.MaterialSets.FirstOrDefault(s => s.regName == record.materialSet) ?? MaterialSet.Basic;
            ascensionLevel = record.acsensionLevel;
            Level = Math.Max(1, stats.maxLevel);
            properties = PlayerProperties.DEFAULT;
            inRound = false;
            won = record.won;
            randomSeed = record.seed;
            HeldArtifacts = new List<int>();
            NewHeldArtifacts = (record.GetHistorySnapshot()?.artifacts ?? new List<HistoryArtifact>())
                .Where(a => a?.content?.Available("artifact") == true).Select(a => a.content.id).ToList();
            artifactPreview = true;
            InitializeArtifactRuntime();
            RestoreHeldArtifactReferences(NewHeldArtifacts);
            HeldGadgets = new List<Gadget>();
            ArtifactBank = new List<string>();
            TilePool = new List<Tile>();
            HandDeck = new List<Tile>(stats.bestSettleRecord?.AllTiles ?? new List<Tile>());
            Discarded = new List<Tile>();
            CurrentSelectedTiles = new List<Tile>();
            RoundAccumulatedScore = Score.Base();
            skillMap = new SerializableMap<Skill.SkillType, int>();
            skillSet = SkillSet.StandardSkillSet();
            nativeYakus = skillSet.GetYakus();
            var levels = stats.maxYakuLevelMap;
            if (levels != null)
                foreach (var yaku in levels.GetKeys()) skillSet.SetLevel(yaku, levels.Get(yaku));
            skillSet.SetPlayer(this);
        }
    }
}
