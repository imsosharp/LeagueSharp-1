using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Kalista
{
    class MathHandler
    {
        public static float getDamageToTarget(Obj_AI_Base target)
        {
            double damage = 0;//ObjectManager.Player.GetAutoAttackDamage(target);
            if (SkillHandler.Q.IsReady() && ObjectManager.Player.Distance(target) < SkillHandler.Q.Range)
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            if (SkillHandler.E.IsReady() && ObjectManager.Player.Distance(targte) < SkillHandler.E.Range)
                damage += getRealDamage(target);
            return (float)damage;
        }

        public static float GetPlayerHealthPercentage()
        {
            return ObjectManager.Player.Health * 100 / ObjectManager.Player.MaxHealth;
        }
        public static float GetPlayerManaPercentage()
        {
            return ObjectManager.Player.Mana * 100 / ObjectManager.Player.MaxMana;
        }

        public static double getRealDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.GetSpellDamage(target, SpellSlot.E) * 0.9;
        }
    }
}
