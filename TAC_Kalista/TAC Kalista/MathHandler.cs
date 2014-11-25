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
            if (SkillHandler.Q.IsReady())
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            if (SkillHandler.E.IsReady())
                damage += getRealDamage(target);
            return (float)damage;
        }
        public static int getTotalAttacks(Obj_AI_Hero target, int stage)
        {
            double totalDamageToTarget = MathHandler.getRealDamage(target);
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
        /*
         * @author InCube
         * @description:
         * Kalista rips the spears from nearby targets, 
         * dealing 20/30/40/50/60 (+0.6) physical damage 
         * and slowing their Movement Speed by 25/30/35/40/45% for 2 seconds. 
         * >>Each extra spear increases the damage by 25/30/35/40/45%<<
         * , but not the slow.
         * http://ddragon.leagueoflegends.com/tool/euw/en
         **/
        public static double getRealDamage(Obj_AI_Base target)
        {

            return ObjectManager.Player.CalcDamage(target,
                Damage.DamageType.Physical,
                    target.Buffs.FirstOrDefault(b => 
                        b.DisplayName == "KalistaExpungeMarker").Count > 1 ? 
                            (new double[] { 0, 20, 30, 40, 50, 60 }[SkillHandler.E.Level] 
                                + 
                                    (0.6 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod)) 
                                        * new double[] { 0, 1.25, 1.30, 1.35, 1.40, 1.45 }[SkillHandler.E.Level]) 
                                            * target.Buffs.FirstOrDefault(b => b.DisplayName == "KalistaExpungeMarker").Count 
                        : new double[] { 0, 20, 30, 40, 50, 60 }[SkillHandler.E.Level] 
                            + (0.6 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod)));
        }
    }
}
