using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.Patches.Events.Server;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs;
using Exiled.API.Features.Items;

namespace LoneFoundation
{
    public class EventHandlers
    { 
        LoneFoundationPlugin plugin;
        //prevents chaos insurgency from damaging or being damaged by scps, and prevents friendly fire for scientists and facility guards, and among class d
        public void PlayerAttack(HurtingEventArgs ev)
        {

            if (ev.Attacker.Role.Team == Team.CHI && ev.Target.Role.Team == Team.SCP ||
                ev.Attacker.Role.Team == Team.SCP && ev.Target.Role.Team == Team.CHI ||

                ev.Attacker.Role.Team == Team.RSC && ev.Target.Role == RoleType.FacilityGuard ||
                ev.Attacker.Role.Team == Team.RSC && ev.Target.Role.Team == Team.RSC ||
                ev.Attacker.Role == RoleType.FacilityGuard && ev.Target.Role == RoleType.FacilityGuard||
                ev.Attacker.Role == RoleType.FacilityGuard && ev.Target.Role.Team == Team.RSC ||

                ev.Attacker.Role == RoleType.NtfCaptain && ev.Target.Role == RoleType.NtfCaptain ||
                ev.Attacker.Role == RoleType.NtfCaptain && ev.Target.Role == RoleType.NtfSpecialist ||
                ev.Attacker.Role == RoleType.NtfCaptain && ev.Target.Role == RoleType.NtfSergeant ||
                ev.Attacker.Role == RoleType.NtfCaptain && ev.Target.Role == RoleType.NtfPrivate ||

                ev.Attacker.Role == RoleType.NtfSpecialist && ev.Target.Role == RoleType.NtfCaptain ||
                ev.Attacker.Role == RoleType.NtfSpecialist && ev.Target.Role == RoleType.NtfSpecialist ||
                ev.Attacker.Role == RoleType.NtfSpecialist && ev.Target.Role == RoleType.NtfSergeant ||
                ev.Attacker.Role == RoleType.NtfSpecialist && ev.Target.Role == RoleType.NtfPrivate ||

                ev.Attacker.Role == RoleType.NtfSergeant && ev.Target.Role == RoleType.NtfCaptain ||
                ev.Attacker.Role == RoleType.NtfSergeant && ev.Target.Role == RoleType.NtfSpecialist ||
                ev.Attacker.Role == RoleType.NtfSergeant && ev.Target.Role == RoleType.NtfSergeant ||
                ev.Attacker.Role == RoleType.NtfSergeant && ev.Target.Role == RoleType.NtfPrivate ||

                ev.Attacker.Role == RoleType.NtfPrivate && ev.Target.Role == RoleType.NtfCaptain ||
                ev.Attacker.Role == RoleType.NtfPrivate && ev.Target.Role == RoleType.NtfSpecialist ||
                ev.Attacker.Role == RoleType.NtfPrivate && ev.Target.Role == RoleType.NtfSergeant ||
                ev.Attacker.Role == RoleType.NtfPrivate && ev.Target.Role == RoleType.NtfPrivate ||


                ev.Attacker.Role.Team == Team.CDP && ev.Target.Role.Team == Team.CDP)
            {
                switch (plugin.Config.FriendlyFireWithinTeams)
                {
                    case true:
                        ev.IsAllowed = true;
                        break;
                    case false:
                        ev.IsAllowed = false;
                        break;
                }


            }
          
            //allows suicide
            if (ev.Attacker==ev.Target)
            {
                ev.IsAllowed = true;
            }

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
            Log.Info("NTF " + ev.UnitName + "-" + ev.UnitNumber + " spawned in. There are "+ev.ScpsLeft+ " SCPs left.");
        }
        public void PlayerSpawning(SpawningEventArgs ev)
        {
            if (ev.Player.Role == RoleType.FacilityGuard) 
            {
                ev.Player.AddAmmo(AmmoType.Nato9, 90);
                ev.Player.AddAmmo(AmmoType.Nato556, 80);
            }
            if (ev.Player.Role.Team == Team.RSC||ev.Player.Role.Team==Team.CDP)
            {
                ev.Player.AddItem(ItemType.Flashlight);
            }
        }
        public void PlayerEscaping(EscapingEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        //win conditions
        public void ServerRoundEnding(EndingRoundEventArgs ev)
        {
            
            bool FoundMilitant=false;
            bool FoundGoodGuy = false;
            bool FoundCivilian=false;
            bool FoundPrisoner=false;
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

                if (player.Role==RoleType.FacilityGuard)
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
                if (player.Role.Team==Team.SCP)
                {
                    Log.Debug("Found an SCP.");
                    SCPMelee = true;
                    SCP = true;

                }
            }
            if (GOCMilitant && FoundPersonnel||
                SCP && FoundPersonnel)
            {
                ev.IsAllowed = false;
            }
            if (FoundGoodGuy && !FoundPrisoner && !GOCMilitant && !SCP)
            {
                ev.LeadingTeam = LeadingTeam.FacilityForces;
                Map.Broadcast(plugin.Config.EndCardTime, plugin.Config.FoundationWinString);

            }
            if (FoundPrisoner && !FoundGoodGuy&&!SCP&&!GOCMilitant)
            {
                ev.LeadingTeam = LeadingTeam.Draw;
                Map.Broadcast(plugin.Config.EndCardTime, plugin.Config.DClassWinString);
            }
            
            if (SCPMelee&& !FoundPersonnel && !GOCMilitant)
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


