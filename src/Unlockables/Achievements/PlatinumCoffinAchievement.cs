namespace Aotenjo
{
    public class PlatinumCoffinAchievement : Achievement
    {
        public PlatinumCoffinAchievement(string id) : base(id)
        {
        }

        [SubscribeToEvent]
        private void OnLostGame(RunStatusEvent.End eventData)
        {
            Player player = eventData.player;
            bool won = eventData.won;
            if (!won || player.Level > 16)
            {
                if (player.GetMoney() >= 100)
                {
                    SetComplete();
                }
            }
        }
    }
}