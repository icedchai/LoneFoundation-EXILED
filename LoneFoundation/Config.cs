using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneFoundation
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool RefineryFunniesEnabled { get; set; } = true;
        public bool FriendlyFireWithinTeams { get; set; } = false;
        public int GOCtoNTFSpawnChance { get; set; } = 20;
        public float[] ChildSizeMultiplier { get; set; } = new float[] { 0.9f, 0.9f, 0.9f };
        public int ChildHealthMultiplierNumerator = 3;
        public int ChildHealthMultiplierDenominator = 4;
        public ushort EndCardTime { get; set; } = 8;
        public string DClassWinString { get; set; }= "The Class-D are free! All threats, like the Chaos Insurgency and GOC are dead, and the SCPs have been terminated.";
        public string GOCWinString { get; set; } = "The GOC have completed their mission. All SCP subjects have been terminated, and all threats such as Chaos Insurgency and Foundation Personnel are gone.";
        public string ChaosWinString{ get; set; }= "The SCPs won! All Foundation and GOC members have been terminated.";
        public string FoundationWinString { get; set; }= "The Foundation is victorious! All threats such as GOC and Class-D have been eliminated. All SCPs have been contained or otherwise removed from the site.";
    }
}
