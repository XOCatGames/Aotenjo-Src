namespace Aotenjo
{
    public class HeavyLifterAchievement : Achievement
    {
        public HeavyLifterAchievement(string id) : base(id)
        {
        }

        [SubscribeToEvent]
        private void OnWonGame(RunStatusEvent.End eventData)
        {
            bool won = eventData.won;
            Player player = eventData.player;
            if (won && player.Level >= 16)
            {
                if (player.GetAllTiles().Count > 200)
                {
                    SetComplete();
                }
            }
        }
    }
}