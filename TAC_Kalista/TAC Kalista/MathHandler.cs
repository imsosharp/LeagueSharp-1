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
         * >>Damage per Additional Spear: 5 / 9 / 14 / 20 / 27 (+15 / 18 / 21 / 24 / 27% AD) <<
         * , but not the slow.
         * http://ddragon.leagueoflegends.com/tool/euw/en
         * Apperantly hellsing sucks at math, so i had to do this code on my own.
         **/
        public static double getRealDamage(Obj_AI_Base target)
        {
            int skillLevel = SkillHandler.E.Level;
            int stacks = target.Buffs.FirstOrDefault(b => b.DisplayName == "KalistaExpungeMarker").Count;
            double basicDamagex = new double[] { 0, 20, 30, 40, 50, 60 }[skillLevel];
            //double extraDamage = new double[] { 0, 0.25, 0.30, 0.35, 0.40, 0.45 }[skillLevel];


            double basicDamage = new double[] { 0, 20, 30, 40, 50, 60 }[skillLevel] + (0.6 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod));
            double addidentionalDamage = new double[] { 0, 5, 9, 14, 20, 27 }[skillLevel] + (new double[] { 0, 0.15, 0.18, 0.21, 0.24, 0.27 }[skillLevel] * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod));


            double realDamage = ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, stacks > 1 ? basicDamage + addidentionalDamage * (stacks - 1) : basicDamage);
            //double realDamage = ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, stacks > 1 ? basicDamage + (basicDamage * extraDamage) * (stacks - 1) : basicDamage);
            if (Kalista.debug)
            {
                Game.PrintChat("Target: " + target.SkinName + " Total to target: " + (int)realDamage + " || Dealing " + basicDamagex + "(+" + (int)(0.6 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod)) + ") Will do: " + (int)basicDamage + addidentionalDamage * (stacks - 1) + " (" + stacks + ")");
//                Game.PrintChat("Target: " + target.SkinName + " Total to target: " + (int)realDamage + " || Dealing " + basicDamagex + "(+" + (int)(0.6 * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod)) + ") Will do: " + (int)basicDamage + (basicDamage * extraDamage) * (stacks - 1) + " (" + stacks + ")");
            }
            return realDamage;
        }
    }
}
