using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.Patches.Events.Server;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs;
using Exiled.API.Features.Items;
using System.Collections.Generic;
using UnityEngine;

namespace LoneFoundation
{
    public class EventHandlers
    { 
        private readonly LoneFoundationPlugin plugin;
        internal EventHandlers(LoneFoundationPlugin plugin) => this.plugin = plugin;
        
        
        
        private List<Player> children = new List<Player>();
        
        //variables used for a "true" NTF wave-- a wave of tutorials spawn, which are friendly towards Foundation Personnel
        private List<Player> ntfSpawn = new List<Player>();
        private List<Player> ntfMembers = new List<Player>();
        private bool trueNTFAnnouncment = false;






        //checks if any two given players are on the same "team"
        public bool IsOnSameTeam(Player x, Player y)
        {
            return (x.Role.Team == Team.CHI && y.Role.Team == Team.SCP ||
                x.Role.Team == Team.SCP && y.Role.Team == Team.CHI ||

                x.Role.Team == Team.RSC && y.Role == RoleType.FacilityGuard ||
                x.Role.Team == Team.RSC && y.Role.Team == Team.RSC ||
                x.Role.Team == Team.RSC && ntfMembers.Contains(y) ||
                x.Role == RoleType.FacilityGuard && y.Role == RoleType.FacilityGuard ||
                x.Role == RoleType.FacilityGuard && y.Role.Team == Team.RSC ||
                x.Role == RoleType.FacilityGuard && ntfMembers.Contains(y) ||
                ntfMembers.Contains(x) && y.Role == RoleType.FacilityGuard  ||
                ntfMembers.Contains(x) &&y.Role.Team==Team.RSC||
                ntfMembers.Contains(x) && ntfMembers.Contains(y)||

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

        //checks if a given role is a "starting role"
        public bool IsStartingRole(Player player)
        {
            return player.Role.Type == RoleType.ClassD || player.Role.Type == RoleType.FacilityGuard || player.Role.Type == RoleType.Scientist;
        }
        
        //prevents chaos insurgency from damaging or being damaged by scps, and prevents friendly fire for scientists and facility guards, and among class d
        public void PlayerAttacking(HurtingEventArgs ev)
        {

            if (IsOnSameTeam(ev.Attacker, ev.Target)){
                    switch (plugin.Config.FriendlyFireWithinTeams)
                    {
                        case false:
                            ev.IsAllowed = false;
                            break;
                        case true:
                            ev.IsAllowed = true;
                            break;
                    }
                }
            if (ev.Attacker.Role.Type == RoleType.Scp93953 || ev.Attacker.Role.Type == RoleType.Scp93989)
            {
                if (IsStartingRole(ev.Target)&&!children.Contains(ev.Target))
                {
                    ev.Amount = 60;
                }
            }

            //allows suicide
            if (ev.Attacker == ev.Target)
            {
                ev.IsAllowed = true;
            }


        }
        //the next three are to prevent chaos insurgency from utilizing the various unique ways to kill certain SCPs, unless forced to
        public void PlayerGeneratorUnlocking(UnlockingGeneratorEventArgs ev)
        {
            if (ev.Player.Role.Team == Team.CHI)
            {
                ev.IsAllowed = false;
            }
        }
        public void PlayerGeneratorActivating(ActivatingGeneratorEventArgs ev)
        {
            if (ev.Player.Role.Team == Team.CHI)
            {
                ev.IsAllowed = false;
            }
        }
        public void PlayerFemurBreakerActivating(EnteringFemurBreakerEventArgs ev)
        {
            if (ev.Player.Role.Team == Team.CHI && !ev.Player.IsCuffed)
            {
                ev.IsAllowed = false;
            }
        }
        public void PlayerDying(DyingEventArgs ev)
        {
            ev.Target.Scale = new Vector3(1, 1, 1);
            children.Remove(ev.Target);
            ntfMembers.Remove(ev.Target);

        }

        public void PlayerSpawning(SpawningEventArgs ev)
        {
            if (ev.Player.Role == RoleType.FacilityGuard)
            {
                ev.Player.ClearInventory();
                ev.Player.AddItem(ItemType.GrenadeFlash);
                ev.Player.AddItem(ItemType.Radio);
                ev.Player.AddItem(ItemType.Painkillers);
                ev.Player.AddItem(ItemType.GunCOM18);
                ev.Player.AddItem(ItemType.KeycardGuard);
                ev.Player.AddItem(ItemType.ArmorLight);
                ev.Player.AddAmmo(AmmoType.Nato9, 30);
            }
            if (IsStartingRole(ev.Player))
            {
                ev.Player.MaxHealth = 120;
                ev.Player.Health = 120;
            }
            if (ntfSpawn.Contains(ev.Player))
            {
                ev.Player.SetRole(RoleType.Tutorial, SpawnReason.Respawn);
                ev.Player.ClearInventory();
                ev.Player.AddItem(ItemType.Radio);
                ev.Player.AddItem(ItemType.Medkit);
                ev.Player.AddItem(ItemType.GunE11SR);
                ev.Player.AddItem(ItemType.KeycardNTFLieutenant);
                ev.Player.AddItem(ItemType.ArmorCombat);
                ev.Player.AddAmmo(AmmoType.Nato9, 30);
                ev.Player.AddAmmo(AmmoType.Nato556, 80);
                ntfSpawn.Remove(ev.Player);
                ntfMembers.Add(ev.Player);
            }
        }
        public void PlayerEscaping(EscapingEventArgs ev)
        {
            ev.IsAllowed = false;
            
        }

        public void Scp096AddingTo096Targets(AddingTargetEventArgs ev)
        {
            if (ev.Target.Role.Team == Team.CHI)
            {
                ev.IsAllowed = false;
            }}
        public void Scp914PlayerUpgrading(UpgradingPlayerEventArgs ev)
        {
            if (plugin.Config.RefineryFunniesEnabled)
            {
                if (ev.KnobSetting == Scp914.Scp914KnobSetting.Coarse && !children.Contains(ev.Player) && ev.Player.IsHuman)
                {
                    ev.Player.Scale = new Vector3(0.875f, 0.875f, 0.875f);
                    ev.Player.MaxHealth = ev.Player.MaxHealth * 2 / 3;
                    ev.Player.Health = ev.Player.Health * 2 / 3;
                    children.Add(ev.Player);
                }
                else if (ev.KnobSetting == Scp914.Scp914KnobSetting.Fine && children.Contains(ev.Player))
                {
                    ev.Player.Scale = new Vector3(1, 1, 1);
                    ev.Player.MaxHealth = ev.Player.MaxHealth * 3 / 2;
                    ev.Player.Health = ev.Player.Health * 3 / 2;
                    children.Remove(ev.Player);
                }

            }

        }
        public void MapMTFAnnouncing(AnnouncingNtfEntranceEventArgs ev)
        {
            int scps = ev.ScpsLeft;
            string threatoverview=string.Empty;
            ev.IsAllowed = false;

            switch (trueNTFAnnouncment)
            {
                case false:;
                    break;
                case true:
                    if (scps == 0)
                    {
                        threatoverview = "noscpsleft";
                    }
                    else
                    {
                        threatoverview = "awaitingrecontainment " + scps + " scpsubjects";
                    }
                    Cassie.Message("MTFunit Epsilon 11 designated NineTailedFox HasEntered allremaining "+ threatoverview);
                        trueNTFAnnouncment = false;
                    break;

            }
        }
        public void ServerTeamRespawning(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
            {

            }
            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox)
            {
                int i = Random.Range(0,99);
                if (i < plugin.Config.GOCtoNTFSpawnChance)
                {
                    ntfSpawn.AddRange(ev.Players);
                    trueNTFAnnouncment = true;
                }
                Log.Info("i < " + plugin.Config.GOCtoNTFSpawnChance + " == " + (i < plugin.Config.GOCtoNTFSpawnChance));
            }
        }
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
            }}

    }
}


