using Exiled.API.Features;
using Exiled.Events;
using System;
using Player = Exiled.Events.Handlers.Player;
using Map = Exiled.Events.Handlers.Map;
using Server = Exiled.Events.Handlers.Server;



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

        private void UnregisterEvents()
        { 
            Player.Hurting -= EventHandler.PlayerAttack;
            Player.UnlockingGenerator -= EventHandler.GeneratorUnlock;
            Player.ActivatingGenerator -= EventHandler.GeneratorActivate;
            Player.Spawning -= EventHandler.PlayerSpawning;
            Player.EnteringFemurBreaker -= EventHandler.FemurBreakerActivate;
            Player.Escaping -= EventHandler.PlayerEscaping;
            Map.AnnouncingNtfEntrance -= EventHandler.MTFAnnouncment;
            Server.EndingRound -= EventHandler.ServerRoundEnding;
            EventHandler = null;
        }

        void RegisterEvents()
        {
            EventHandler = new EventHandlers(this);

            Player.Hurting += EventHandler.PlayerAttack;
            Player.UnlockingGenerator += EventHandler.GeneratorUnlock;
            Player.ActivatingGenerator += EventHandler.GeneratorActivate;
            Player.Spawning += EventHandler.PlayerSpawning;
            Player.EnteringFemurBreaker += EventHandler.FemurBreakerActivate;
            Player.Escaping += EventHandler.PlayerEscaping;
            Map.AnnouncingNtfEntrance += EventHandler.MTFAnnouncment;
            Server.EndingRound += EventHandler.ServerRoundEnding;
        }
    }
}
