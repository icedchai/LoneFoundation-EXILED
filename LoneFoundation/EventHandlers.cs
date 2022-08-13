using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.Patches.Events.Server;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs;
using Exiled.API.Features.Items;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using static LoneFoundation.API;
namespace LoneFoundation
{
    public class EventHandlers
    { 
        private readonly LoneFoundationPlugin plugin;
        internal EventHandlers(LoneFoundationPlugin plugin) => this.plugin = plugin;
        
        //variables used for a "true" NTF wave-- a wave of tutorials spawn, which are friendly towards Foundation Personnel
        private List<Player> ntfSpawn = new List<Player>();
        private List<Player> ntfPlayers = new List<Player>();
        private bool trueNTFAnnouncment = false;

        private int ntfSpawned = 0;


        public IEnumerator<float> ChildInfiniteStamina()
        {
            for (; ; ) //repeat the loop infinitely
            {
                foreach(Player player in API.GetChildren())
                {
                    player.ResetStamina();
                }
                yield return Timing.WaitForSeconds(0.01f);
            }
        }

        //checks if any two given players are on the same "team"
        public bool IsOnSameTeam(Player x, Player y)
        {
            return (x.Role.Team == Team.CHI && y.Role.Team == Team.SCP ||
                x.Role.Team==Team.CHI&&y.Role.Team==Team.CHI||
                x.Role.Team == Team.SCP && y.Role.Team == Team.CHI ||

                x.Role.Team == Team.RSC && y.Role == RoleType.FacilityGuard ||
                x.Role.Team == Team.RSC && y.Role.Team == Team.RSC ||
                x.Role.Team == Team.RSC && API.IsTrueNTF(y) ||
                x.Role == RoleType.FacilityGuard && y.Role == RoleType.FacilityGuard ||
                x.Role == RoleType.FacilityGuard && y.Role.Team == Team.RSC ||
                x.Role == RoleType.FacilityGuard && API.IsTrueNTF(y) ||
                API.IsTrueNTF(x) && y.Role == RoleType.FacilityGuard  ||
                API.IsTrueNTF(x) && y.Role.Team==Team.RSC||
                API.IsTrueNTF(x) && API.IsTrueNTF(y) ||

                (x.Role.Team == Team.MTF&&x.Role!=RoleType.FacilityGuard) && (y.Role.Team == Team.MTF && y.Role != RoleType.FacilityGuard) ||

                x.Role.Team == Team.CDP && y.Role.Team == Team.CDP);
        }
        public string ScpToNumberString(Player player)
        {
            switch (player.Role.Type)
            {
                case RoleType.Scp049:
                    return "0 4 9";
                case RoleType.Scp079:
                    return "0 7 9";
                case RoleType.Scp096:
                    return "0 9 6";
                case RoleType.Scp106:
                    return "1 0 6";
                case RoleType.Scp173:
                    return "1 7 3";
            }
            if (player.Role.Type.Is939())
            {
                return "9 3 9";
            }
            return null;
        }
        internal static string FormatArguments(System.ArraySegment<string> arguments, int v)
        {
            throw new System.NotImplementedException();
        }

        //checks if a given role is a "starting role"
        public bool IsStartingRole(Player player)
        {
            return player.Role.Type == RoleType.ClassD || player.Role.Type == RoleType.FacilityGuard || player.Role.Type == RoleType.Scientist;
        }
        
        //prevents chaos insurgency from damaging or being damaged by scps, and prevents friendly fire for scientists and facility guards, and among class d
        public void PlayerHurting(HurtingEventArgs ev)
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
                ev.Amount = ev.Target.MaxHealth / 2;
            }

            //allows suicide
            if (ev.Attacker == ev.Target)
            {
                ev.IsAllowed = true;
            }


        }
        //the next three are to prevent chaos insurgency from utilizing the various unique ways to kill certain SCPs, unless forced to
        public void PlayerUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (ev.Player.Role.Team == Team.CHI)
            {
                ev.IsAllowed = false;
            }
        }
        public void PlayerActivatingGenerator(ActivatingGeneratorEventArgs ev)
        {
            if (ev.Player.Role.Team == Team.CHI)
            {
                ev.IsAllowed = false;
            }
        }
        public void PlayerEnteringFemurBreaker(EnteringFemurBreakerEventArgs ev)
        {
            if (ev.Player.Role.Team == Team.CHI && !ev.Player.IsCuffed)
            {
                ev.IsAllowed = false;
            }
        }
        public void PlayerDying(DyingEventArgs ev)
        {
            ev.Target.Scale = new Vector3(1, 1, 1);
            ev.Target.SessionVariables.Remove("Child");
            ev.Target.SessionVariables.Remove("TrueNTF");

        }
        public void PlayerChangingRole(ChangingRoleEventArgs ev)
        {
            ev.Player.Scale = new Vector3(1, 1, 1);
            ev.Player.SessionVariables.Remove("Child");
            ev.Player.SessionVariables.Remove("TrueNTF");
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
                ntfPlayers.Add(ev.Player);
                ev.Player.SessionVariables.Add("TrueNTF", null);
                ntfSpawned++;
            }
        }
        public void PlayerEscaping(EscapingEventArgs ev)
        {

            if (API.IsTrueNTF(ev.Player))
            {
                ntfSpawn.Add(ev.Player);
                ev.NewRole = RoleType.Tutorial;
            }
            ev.IsAllowed = false;
            
        }

        public void Scp096AddingTarget(AddingTargetEventArgs ev)
        {
            if (ev.Target.Role.Team == Team.CHI)
            {
                ev.IsAllowed = false;
            }}
        public void Scp914UpgradingPlayer(UpgradingPlayerEventArgs ev)
        {
            if (plugin.Config.RefineryFunniesEnabled)
            {
                if (ev.KnobSetting == Scp914.Scp914KnobSetting.Coarse && !API.IsChild(ev.Player) && ev.Player.IsHuman)
                {
                    ev.Player.Scale = new Vector3(plugin.Config.ChildSizeMultiplier[0], plugin.Config.ChildSizeMultiplier[1], plugin.Config.ChildSizeMultiplier[2]);
                    ev.Player.MaxHealth = ev.Player.MaxHealth * plugin.Config.ChildHealthMultiplierNumerator / plugin.Config.ChildHealthMultiplierDenominator;
                    ev.Player.Health = ev.Player.Health * plugin.Config.ChildHealthMultiplierNumerator / plugin.Config.ChildHealthMultiplierDenominator;
                    ev.Player.SessionVariables.Add("Child", null);
                    ev.Player.EnableEffect(EffectType.Concussed, 10);
                    ev.Player.ResetStamina();
                }
                else if (ev.KnobSetting == Scp914.Scp914KnobSetting.Fine && API.IsChild(ev.Player))
                {
                    ev.Player.Scale = new Vector3(1, 1, 1);
                    ev.Player.MaxHealth = ev.Player.MaxHealth * plugin.Config.ChildHealthMultiplierDenominator / plugin.Config.ChildHealthMultiplierNumerator;
                    ev.Player.Health = ev.Player.Health * plugin.Config.ChildHealthMultiplierDenominator / plugin.Config.ChildHealthMultiplierNumerator;
                    ev.Player.SessionVariables.Remove("Child");
                }

            }

        }
        public void MapAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
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
                    else if(scps>1)
                    {
                        threatoverview = "awaitingrecontainment " + scps + " scpsubjects";
                    }
                    else
                    {
                        threatoverview = "awaitingrecontainment 1 scpsubject";
                    }
                    Cassie.Message("MTFunit Epsilon 11 designated NineTailedFox HasEntered allremaining "+ threatoverview);
                        trueNTFAnnouncment = false;
                    break;

            }
        }
        public void MapAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (ev.Killer.Role.Team == Team.MTF && ev.Killer.Role != RoleType.FacilityGuard)
            {
                Cassie.Message("scp " + ScpToNumberString(ev.Player) + " containedsuccessfuly by g o c");
            } else if (ev.Killer.Role == RoleType.FacilityGuard)
            {
                ev.IsAllowed = true;
            } else if (ev.Killer.SessionVariables.ContainsKey("TrueNTF"))
            {
                int i = ntfPlayers.IndexOf(ev.Killer);
                Cassie.Message("scp " + ScpToNumberString(ev.Player) + " containedsuccessfully by nato_f unit "+i);
            }
        }
        public void ServerRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
            {

            }
            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox)
            {
                int i = Random.Range(0, 99);
                Log.Info("i < " + plugin.Config.GOCtoNTFSpawnChance + " == " + (i < plugin.Config.GOCtoNTFSpawnChance));
                if (i < plugin.Config.GOCtoNTFSpawnChance)
                {
                    ntfSpawn.AddRange(ev.Players);
                    trueNTFAnnouncment = true;
                }

            }
        }
        public void ServerRoundStarted()
        {
            LoneFoundationPlugin.Coroutines.Add(Timing.RunCoroutine(ChildInfiniteStamina()));
        }
        public void ServerRoundEnding(EndingRoundEventArgs ev)
        {
            foreach (CoroutineHandle item in LoneFoundationPlugin.Coroutines)
                Timing.KillCoroutines(item);
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


