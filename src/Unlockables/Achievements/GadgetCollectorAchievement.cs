using System.Linq;

namespace Aotenjo
{
    public class GadgetCollectorAchievement : Achievement
    {
        public GadgetCollectorAchievement(string id) : base(id)
        {
        }

        [SubscribeToEvent]
        private void PostRunEnd(RunStatusEvent.End.Post eventData)
        {
            if (Gadgets.GadgetCompleteList().All(g => eventData.stats.GetGadgetObtainedCount(g) > 0))
                SetComplete();
        }
    }
}