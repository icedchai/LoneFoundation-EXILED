using Exiled.API.Features;
using Exiled.Events;
using System;
using Player = Exiled.Events.Handlers.Player;
using Map = Exiled.Events.Handlers.Map;
using Server = Exiled.Events.Handlers.Server;
using Scp096 = Exiled.Events.Handlers.Scp096;
using ScpUpgrade = Exiled.Events.Handlers.Scp914;



namespace LoneFoundation
{

    public class LoneFoundationPlugin : Plugin<Config>
    {
        
        EventHandlers EventHandler;
        public override void OnEnabled()
        {
            RegisterEvents();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            EventHandler = new EventHandlers(this);

            Player.Hurting += EventHandler.PlayerAttacking;
            Player.UnlockingGenerator += EventHandler.PlayerGeneratorUnlocking;
            Player.ActivatingGenerator += EventHandler.PlayerGeneratorActivating;
            Player.Spawning += EventHandler.PlayerSpawning;
            Player.EnteringFemurBreaker += EventHandler.PlayerFemurBreakerActivating;
            Player.Escaping += EventHandler.PlayerEscaping;
            Player.Dying += EventHandler.PlayerDying;
            Scp096.AddingTarget += EventHandler.Scp096AddingTo096Targets;
            ScpUpgrade.UpgradingPlayer += EventHandler.Scp914PlayerUpgrading;
            Map.AnnouncingNtfEntrance += EventHandler.MapMTFAnnouncing;
            Server.RespawningTeam += EventHandler.ServerTeamRespawning;
            Server.EndingRound += EventHandler.ServerRoundEnding;
        }
        private void UnregisterEvents()
        { 
            Player.Hurting -= EventHandler.PlayerAttacking;
            Player.UnlockingGenerator -= EventHandler.PlayerGeneratorUnlocking;
            Player.ActivatingGenerator -= EventHandler.PlayerGeneratorActivating;
            Player.Spawning -= EventHandler.PlayerSpawning;
            Player.EnteringFemurBreaker -= EventHandler.PlayerFemurBreakerActivating;
            Player.Escaping -= EventHandler.PlayerEscaping;
            Player.Dying -= EventHandler.PlayerDying;
            Scp096.AddingTarget -= EventHandler.Scp096AddingTo096Targets;
            ScpUpgrade.UpgradingPlayer -= EventHandler.Scp914PlayerUpgrading;
            Map.AnnouncingNtfEntrance -= EventHandler.MapMTFAnnouncing;
            Server.RespawningTeam -= EventHandler.ServerTeamRespawning;
            Server.EndingRound -= EventHandler.ServerRoundEnding;
            EventHandler = null;
        }

    }
}
