using System.Collections.Generic;

namespace Aotenjo.TileMaterialGlobalEffect
{
    public class TileMaterialEntryEffect : IPlayerEventSubscriber
    {
        public string regName;
        
        public TileMaterialEntryEffect(string regName)
        {
            this.regName = regName;
        }
        
        public virtual void SubscribeToPlayerEvents(Player player)
        {
        }

        public virtual void UnsubscribeToPlayerEvents(Player player)
        {
        }


        public static Dictionary<string, TileMaterialEntryEffect> TileMaterialEntryEffectMap = new() 
        {
            { "emerald_wood", new EmeraldWoodEntryEffect()}
        };
    }
}