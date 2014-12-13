using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Linq;

namespace TAC_Kalista
{
    /**
     * @author metaphorce 
     */
    class SmiteHandler
    {
        private static readonly string[] MinionNames = { 
              "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith", 
              "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", 
              "SRU_Red", "SRU_Krug", "SRU_Dragon", "SRU_Baron", "Sru_Crab" };

        public static Obj_AI_Minion GetNearest(Vector3 pos)
        {
            var minions =
            ObjectManager.Get<Obj_AI_Minion>()
            .Where(minion => minion.IsValid && MinionNames.Any(name => minion.Name.StartsWith(name)) && !MinionNames.Any(name => minion.Name.Contains("Mini")));
            var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
            var sMinion = objAiMinions.FirstOrDefault();
            double? nearest = null;
            foreach (var minion in objAiMinions)
            {
                double distance = Vector3.Distance(pos, minion.Position);
                if (nearest != null && !(nearest > distance)) continue;
                nearest = distance;
                sMinion = minion;
            }
            return sMinion;
        }
        public static void Init()
        {
            if (MenuHandler.Config.Item("smite").GetValue<KeyBind>().Active)
            {
                var mob = GetNearest(ObjectManager.Player.ServerPosition);
                if (mob != null && MenuHandler.Config.Item(mob.SkinName).GetValue<bool>())
                {
                    if (mob.Health < MathHandler.GetRealDamage(mob))
                    {
                        SkillHandler.E.Cast(Kalista.PacketCast);
                    }
                }
            }
        }
    }
}
