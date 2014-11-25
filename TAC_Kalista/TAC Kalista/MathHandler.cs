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
            double damage = ObjectManager.Player.GetAutoAttackDamage(target);
            if (SkillHandler.Q.IsReady())
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            if (SkillHandler.E.IsReady())
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            return (float)damage;
        }
        public static int getTotalAttacks(Obj_AI_Hero target, int stage)
        {
            double totalDamageToTarget = ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            double targetHealth = target.Health;
            double skillQdamage = SkillHandler.Q.GetDamage(target);
            double baseADDamage = ObjectManager.Player.GetAutoAttackDamage(target);
            double AANeeded = 0;
            if (stage == 1)
            {
                AANeeded = (targetHealth - skillQdamage - SkillHandler.E.GetDamage(target)) / baseADDamage;
            }
            else
            {
                AANeeded = (targetHealth - SkillHandler.E.GetDamage(target)) / baseADDamage;
            }

            return (int)AANeeded;
        }

        public static float GetPlayerHealthPercentage()
        {
            return ObjectManager.Player.Health * 100 / ObjectManager.Player.MaxHealth;
        }
        public static float GetPlayerManaPercentage()
        {
            return ObjectManager.Player.Mana * 100 / ObjectManager.Player.MaxMana;
        }
    }
}
