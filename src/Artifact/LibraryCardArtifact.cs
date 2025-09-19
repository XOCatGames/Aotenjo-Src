using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Sirenix.Serialization;
using UnityEngine;

namespace Aotenjo
{
    public class LibraryCardArtifact : Artifact
    {
        #region 数据
        
        [Serializable]
        private class LibraryCardArtifactData
        {
            [SerializeField] public List<string> entries = new();
            [SerializeReference] public List<List<YakuType>> buffedYakuList = new();

            public void PutEntry(string entry, YakuType[] types)
            {
                if (!entries.Contains(entry))
                {
                    entries.Add(entry);
                    buffedYakuList.Add(types.ToList());
                    return;
                }

                buffedYakuList.RemoveAt(entries.IndexOf(entry));
                entries.Remove(entry);
                PutEntry(entry, types);
            }
            
            public int CountRepeats(YakuType type)
            {
                return buffedYakuList.SelectMany(Enumerable.AsEnumerable).Count(t => t == type);
            }
        }

        private LibraryCardArtifactData data = new();

        public override string Serialize()
        {
            return JsonConvert.SerializeObject(data);
        }

        public override void Deserialize(string json)
        {
            data = JsonConvert.DeserializeObject<LibraryCardArtifactData>(json);
        }

        public override void ResetArtifactState()
        {
            base.ResetArtifactState();
            data = new LibraryCardArtifactData();
        }

        #endregion

        public LibraryCardArtifact() : base("committee_license", Rarity.RARE)
        {
        }

        public override string GetDescription(Func<string, string> localizer)
        {
            StringBuilder sb = new StringBuilder();
            List<Yaku> yakus = data.buffedYakuList.SelectMany(Enumerable.AsEnumerable).Select(t => YakuTester.InfoMap[t]).ToList();
            //a, a, b, b, c, a处理为 a*3, b*2, c
            if(yakus.Any()) sb.Append("\n");
            var grouped = yakus.GroupBy(y => y.GetYakuType()).Select(g => new { Yaku = g.First(), Count = g.Count() }).ToList();
            for (int i = 0; i < grouped.Count; i++)
            {
                var item = grouped[i];
                sb.Append(item.Yaku.GetFormattedName(localizer));
                if (item.Count > 1)
                    sb.Append($" *{item.Count}");
                if (i != grouped.Count - 1)
                    sb.Append("\n");
            }
            return string.Format(base.GetDescription(localizer), sb);
        }

        public override string GetInShopDescription(Player player, Func<string, string> localizer)
        {
            return localizer("artifact_committee_license_description_inshop");
        }

        public override void SubscribeToPlayer(Player player)
        {
            base.SubscribeToPlayer(player);
            player.RetrieveYakuMultiplierEvent += PlayerOnRetrieveYakuMultiplierEvent;
            player.PostUpgradeYakuFromIBookEvent += PlayerOnOpenIBookEvent;
        }

        private void PlayerOnOpenIBookEvent(PlayerYakuEvent.ReadBookResult obj)
        {
            data.PutEntry(obj.book.GetRegName(), obj.results.Select(y => y.GetYakuType()).ToArray());
        }

        public override void UnsubscribeToPlayer(Player player)
        {
            base.UnsubscribeToPlayer(player);
            player.RetrieveYakuMultiplierEvent -= PlayerOnRetrieveYakuMultiplierEvent;
            player.PostUpgradeYakuFromIBookEvent -= PlayerOnOpenIBookEvent;
        }

        private void PlayerOnRetrieveYakuMultiplierEvent(PlayerYakuEvent.RetrieveMultiplier yakuEvent)
        {
            YakuType type = yakuEvent.yakuType;
            int count = data.CountRepeats(type);
            yakuEvent.multiplier *= Math.Pow(2, count);
        }
    }
}