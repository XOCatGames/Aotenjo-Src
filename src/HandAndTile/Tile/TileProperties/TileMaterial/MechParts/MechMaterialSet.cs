using System.Collections.Generic;

namespace Aotenjo
{
    public class MechMaterialSet : MaterialSet
    {
        public MechMaterialSet() : base(6, "mech_parts", new List<string>
        {
            "mech_gear",
            "mech_drive_rod",
            "mech_network_card",
            "mech_led",
            "mech_shield",
            "mech_integrated_chip",
            "mech_reactor"
        })
        {
        }

        public override void SubscribeToPlayerEvents(Player player)
        {
            base.SubscribeToPlayerEvents(player);
            EventBus.Subscribe<PlayerEvents.PreSetTilePropertiesEvent>(player, InstallIncomingPart, int.MinValue);
            EventBus.Subscribe<PlayerRoundEvent.End.Post>(player, RemoveTemporaryParts);
        }

        public override void UnsubscribeToPlayerEvents(Player player)
        {
            base.UnsubscribeToPlayerEvents(player);
            EventBus.Unsubscribe<PlayerEvents.PreSetTilePropertiesEvent>(player, InstallIncomingPart);
            EventBus.Unsubscribe<PlayerRoundEvent.End.Post>(player, RemoveTemporaryParts);
        }

        public static void InstallIncomingPart(PlayerSetPropertiesEvent evt)
        {
            TileMaterialMechPart.ResolveIncomingMaterial(evt.tile.properties, evt.propertiesToChange);
        }

        private static void RemoveTemporaryParts(PlayerRoundEvent.End.Post evt)
        {
            foreach (Tile tile in evt.player.GetAllTiles())
            {
                if (tile?.properties?.material is not TileMaterialMechPart material) continue;
                int removed = material.RemoveTemporaryParts();
                if (removed > 0)
                {
                    EventBus.Publish(new MechanicalPartChangedEvent(evt.player, tile,
                        MechanicalPartChangeKind.TemporaryExpired, default, null, removed));
                    if (material.GetParts().Count == 0)
                    {
                        tile.SetMaterial(material.TakeMaterialBeforeTemporaryConversion(), evt.player);
                    }
                }
            }
        }
    }
}
