using System.Collections.Generic;
using System.Linq;

namespace Aotenjo
{
    public class PatchouliKnowledgeAchievement : Achievement
    {
        public PatchouliKnowledgeAchievement(string id) : base(id)
        {
        }
        
        [SubscribeToEvent]
        private void PostRunEnd(RunStatusEvent.End.Post eventData)
        {
            Player player = eventData.player;
            bool won = eventData.won;
            PlayerStats stats = eventData.stats;
            if (YakuTester.YAKUS_PREDICATE_MAP.Keys.All(stats.PlayedYaku)) 
                SetComplete();
        }
    }
}