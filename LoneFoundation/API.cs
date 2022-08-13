using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneFoundation
{
    public static class API
    {
        public static bool IsTrueNTF(Player player)
        {
            return player.SessionVariables.ContainsKey("TrueNTF");
        }
        public static List<Player> GetTrueNTF()
        {
            return Player.List.Where(x => x.SessionVariables.ContainsKey("TrueNTF")).ToList();
        }
        public static bool IsChild(Player player)
        {
            return player.SessionVariables.ContainsKey("Child");
        }
        public static List<Player> GetChildren()
        {
            return Player.List.Where(x => x.SessionVariables.ContainsKey("Child")).ToList();
        }
    }
}
