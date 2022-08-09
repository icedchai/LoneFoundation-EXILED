using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.Patches.Events.Server;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs;
using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;

namespace LoneFoundation
{
    public class EventHandlers
    { 
        private readonly LoneFoundationPlugin plugin;
        internal EventHandlers(LoneFoundationPlugin plugin) => this.plugin = plugin;

        List<Player> guards= new List<Player>();
        
        public bool IsOnSameTeam(Player x, Player y)
        {
            return (x.Role.Team == Team.CHI && y.Role.Team == Team.SCP ||
                x.Role.Team == Team.SCP && y.Role.Team == Team.CHI ||

                x.Role.Team == Team.RSC && y.Role == RoleType.FacilityGuard ||
                x.Role.Team == Team.RSC && y.Role.Team == Team.RSC ||
                x.Role == RoleType.FacilityGuard && y.Role == RoleType.FacilityGuard ||
                x.Role == RoleType.FacilityGuard && y.Role.Team == Team.RSC ||

                x.Role == RoleType.NtfCaptain && y.Role == RoleType.NtfCaptain ||
                x.Role == RoleType.NtfCaptain && y.Role == RoleType.NtfSpecialist ||
                x.Role == RoleType.NtfCaptain && y.Role == RoleType.NtfSergeant ||
                x.Role == RoleType.NtfCaptain && y.Role == RoleType.NtfPrivate ||

                x.Role == RoleType.NtfSpecialist && y.Role == RoleType.NtfCaptain ||
                x.Role == RoleType.NtfSpecialist && y.Role == RoleType.NtfSpecialist ||
                x.Role == RoleType.NtfSpecialist && y.Role == RoleType.NtfSergeant ||
                x.Role == RoleType.NtfSpecialist && y.Role == RoleType.NtfPrivate ||

                x.Role == RoleType.NtfSergeant && y.Role == RoleType.NtfCaptain ||
                x.Role == RoleType.NtfSergeant && y.Role == RoleType.NtfSpecialist ||
                x.Role == RoleType.NtfSergeant && y.Role == RoleType.NtfSergeant ||
                x.Role == RoleType.NtfSergeant && y.Role == RoleType.NtfPrivate ||

                x.Role == RoleType.NtfPrivate && y.Role == RoleType.NtfCaptain ||
                x.Role == RoleType.NtfPrivate && y.Role == RoleType.NtfSpecialist ||
                x.Role == RoleType.NtfPrivate && y.Role == RoleType.NtfSergeant ||
                x.Role == RoleType.NtfPrivate && y.Role == RoleType.NtfPrivate ||


                x.Role.Team == Team.CDP && y.Role.Team == Team.CDP);
        }
        
        //prevents chaos insurgency from damaging or being damaged by scps, and prevents friendly fire for scientists and facility guards, and among class d
        public void PlayerAttack(HurtingEventArgs ev)
        {

            if (IsOnSameTeam(ev.Attacker, ev.Target)){
                    switch (plugin.Config.FriendlyFireWithinTeams)
                    {
                        case true:
                            ev.IsAllowed = false;
                            break;
                        case false:
                            ev.IsAllowed = true;
                            break;
                    }
                }

            //allows suicide
            if (ev.Attacker == ev.Target)
            {
                ev.IsAllowed = true;
            }


        }

        public void PlayerSpawningBlood(PlacingBloodEventArgs ev)
        {

        }
        public void GeneratorUnlock(UnlockingGeneratorEventArgs ev)
        {
            if (ev.Player.Role.Team == Team.CHI)
            {
                ev.IsAllowed = false;
            }
        }
        public void GeneratorActivate(ActivatingGeneratorEventArgs ev)
        {
            if (ev.Player.Role.Team == Team.CHI)
            {
                ev.IsAllowed = false;
            }
        }
        public void FemurBreakerActivate(EnteringFemurBreakerEventArgs ev)
        {
            if (ev.Player.Role.Team == Team.CHI && !ev.Player.IsCuffed)
            {
                ev.IsAllowed = false;
            }
        }
        public void MTFAnnouncment(AnnouncingNtfEntranceEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        public void PlayerSpawning(SpawningEventArgs ev)
        {
            Player player = ev.Player;
            if (ev.Player.Role == RoleType.FacilityGuard)
            {
                player.ClearInventory();
                player.AddItem(ItemType.GrenadeFlash);
                player.AddItem(ItemType.Radio);
                player.AddItem(ItemType.Painkillers);
                player.AddItem(ItemType.GunCOM18);
                player.AddItem(ItemType.KeycardGuard);
                player.AddItem(ItemType.ArmorLight);
                player.AddAmmo(AmmoType.Nato9, 30);
                guards.Add(player);
                Log.Info(guards);
            }
            if (ev.Player.Role.Team == Team.RSC || ev.Player.Role.Team == Team.CDP)
            {
                player.AddItem(ItemType.Flashlight);
            }
        }
        public void PlayerEscaping(EscapingEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        //win conditions
        public void ServerRoundEnding(EndingRoundEventArgs ev)
        {

            bool FoundMilitant = false;
            bool FoundGoodGuy = false;
            bool FoundCivilian = false;
            bool FoundPrisoner = false;
            bool FoundPersonnel = false;

            bool GOCMilitant = false;

            bool SCPMilitant = false;
            bool SCPMelee = false;
            bool SCP = false;
            foreach (Player player in Player.List)
            {
                if (player == null)
                {
                    Log.Debug("Skipping a null player.");
                    continue;
                }

                if (player.Role == RoleType.FacilityGuard)
                {
                    Log.Debug("Found a Facility Guard.");
                    FoundMilitant = true;
                    FoundGoodGuy = true;
                    FoundPersonnel = true;
                }
                if (player.Role.Team == Team.RSC)
                {
                    Log.Debug("Found a Scientist");
                    FoundCivilian = true;
                    FoundGoodGuy = true;
                    FoundPersonnel = true;
                }
                if (player.Role.Team == Team.CDP)
                {
                    Log.Debug("Found a Class-D");
                    FoundPrisoner = true;
                    FoundPersonnel = true;
                }

                if (player.Role.Team == Team.MTF && player.Role != RoleType.FacilityGuard)
                {
                    Log.Debug("Found an MTF/GOC");
                    GOCMilitant = true;
                }

                if (player.Role.Team == Team.CHI)
                {
                    Log.Debug("Found a Chaos");
                    SCPMilitant = true;
                    SCP = true;
                }
                if (player.Role.Team == Team.SCP)
                {
                    Log.Debug("Found an SCP.");
                    SCPMelee = true;
                    SCP = true;

                }
            }
            if (GOCMilitant && FoundPersonnel ||
                SCP && FoundPersonnel)
            {
                ev.IsAllowed = false;
            }
            if (FoundGoodGuy && !FoundPrisoner && !GOCMilitant && !SCP)
            {
                ev.LeadingTeam = LeadingTeam.FacilityForces;
                Map.Broadcast(plugin.Config.EndCardTime, plugin.Config.FoundationWinString);

            }
            if (FoundPrisoner && !FoundGoodGuy && !SCP && !GOCMilitant)
            {
                ev.LeadingTeam = LeadingTeam.Draw;
                Map.Broadcast(plugin.Config.EndCardTime, plugin.Config.DClassWinString);
            }

            if (SCPMelee && !FoundPersonnel && !GOCMilitant)
            {
                ev.LeadingTeam = LeadingTeam.Anomalies;

                Map.Broadcast(plugin.Config.EndCardTime, plugin.Config.ChaosWinString);


            }
            if (GOCMilitant && !FoundPersonnel && !SCP)
            {
                ev.LeadingTeam = LeadingTeam.ChaosInsurgency;
                Map.Broadcast(plugin.Config.EndCardTime, plugin.Config.GOCWinString);
            }
        }

    }
}


