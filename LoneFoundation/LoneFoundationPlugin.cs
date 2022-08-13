using Exiled.API.Features;
using Exiled.Events;
using System;
using Player = Exiled.Events.Handlers.Player;
using Map = Exiled.Events.Handlers.Map;
using Server = Exiled.Events.Handlers.Server;
using Scp096 = Exiled.Events.Handlers.Scp096;
using ScpUpgrade = Exiled.Events.Handlers.Scp914;
using MEC;
using System.Collections.Generic;

namespace LoneFoundation
{

    public class LoneFoundationPlugin : Plugin<Config>
    {
        
        EventHandlers EventHandler;
        public static List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();
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

            Player.Hurting += EventHandler.PlayerHurting;
            Player.UnlockingGenerator += EventHandler.PlayerUnlockingGenerator;
            Player.ActivatingGenerator += EventHandler.PlayerActivatingGenerator;
            Player.Spawning += EventHandler.PlayerSpawning;
            Player.EnteringFemurBreaker += EventHandler.PlayerEnteringFemurBreaker;
            Player.Escaping += EventHandler.PlayerEscaping;
            Player.Dying += EventHandler.PlayerDying;
            Scp096.AddingTarget += EventHandler.Scp096AddingTarget;
            ScpUpgrade.UpgradingPlayer += EventHandler.Scp914UpgradingPlayer;
            Map.AnnouncingNtfEntrance += EventHandler.MapAnnouncingNtfEntrance;
            Map.AnnouncingScpTermination += EventHandler.MapAnnouncingScpTermination;
            Server.RespawningTeam += EventHandler.ServerRespawningTeam;
            Server.RoundStarted += EventHandler.ServerRoundStarted;
            Server.EndingRound += EventHandler.ServerRoundEnding;
        }
        private void UnregisterEvents()
        { 
            Player.Hurting -= EventHandler.PlayerHurting;
            Player.UnlockingGenerator -= EventHandler.PlayerUnlockingGenerator;
            Player.ActivatingGenerator -= EventHandler.PlayerActivatingGenerator;
            Player.Spawning -= EventHandler.PlayerSpawning;
            Player.EnteringFemurBreaker -= EventHandler.PlayerEnteringFemurBreaker;
            Player.Escaping -= EventHandler.PlayerEscaping;
            Player.Dying -= EventHandler.PlayerDying;
            Scp096.AddingTarget -= EventHandler.Scp096AddingTarget;
            ScpUpgrade.UpgradingPlayer -= EventHandler.Scp914UpgradingPlayer;
            Map.AnnouncingNtfEntrance -= EventHandler.MapAnnouncingNtfEntrance;
            Map.AnnouncingScpTermination -= EventHandler.MapAnnouncingScpTermination;
            Server.RespawningTeam -= EventHandler.ServerRespawningTeam;
            Server.RoundStarted -= EventHandler.ServerRoundStarted;
            Server.EndingRound -= EventHandler.ServerRoundEnding;
            EventHandler = null;
        }

    }
}
