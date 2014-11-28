using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Kassadin
{
    class AutoCarryHandler
    {
        internal static Items.Item item = Utility.Map.GetMap()._MapType == Utility.Map.MapType.TwistedTreeline || Utility.Map.GetMap()._MapType == Utility.Map.MapType.CrystalScar ? new Items.Item(3188, 750) : new Items.Item(3128, 750);
        internal static void AutoCarry()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Physical);
            float distance = ObjectManager.Player.Distance(target.Position);
            if (target == null) return;
            
            if (MenuHandler.menu.Item("useDFGFull").GetValue<bool>()
                && SkillHandler.Q.IsReady() && SkillHandler.W.IsReady() && SkillHandler.E.IsReady() && SkillHandler.R.IsReady()
                    && MenuHandler.menu.Item("useDFG").GetValue<bool>() 
                        && MathHandler.getComboDamage(target) > target.Health && item.IsReady()) item.Cast(target);
            else if (!MenuHandler.menu.Item("useDFGFull").GetValue<bool>() 
                        && MenuHandler.menu.Item("useDFG").GetValue<bool>() && MathHandler.getComboDamage(target) > target.Health && item.IsReady()) item.Cast(target);

            if (MenuHandler.menu.Item("acR").GetValue<bool>() && SkillHandler.R.IsReady() && distance < (SkillHandler.R.Range + SkillHandler.W.Range)) MathHandler.castR(target);
            if (MenuHandler.menu.Item("acQ").GetValue<bool>() && SkillHandler.Q.IsReady() && distance < SkillHandler.Q.Range) SkillHandler.Q.Cast(target, Program.packetCast);
            if (MenuHandler.menu.Item("acW").GetValue<bool>() && SkillHandler.W.IsReady()) SkillHandler.W.Cast(Program.packetCast);
            if (MenuHandler.menu.Item("acE").GetValue<bool>() && SkillHandler.E.IsReady() && SkillHandler.E.InRange(target.Position)) MathHandler.castE(target);
        }
        internal static void Mixed()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Physical);
            float distance = ObjectManager.Player.Distance(target.Position);
            if (target == null) return;
            if (MenuHandler.menu.Item("lcQ").GetValue<bool>() && SkillHandler.Q.IsReady() && distance < SkillHandler.Q.Range) SkillHandler.Q.Cast(target, Program.packetCast);
            if (MenuHandler.menu.Item("lcE").GetValue<bool>() && SkillHandler.E.IsReady() && SkillHandler.E.InRange(target.Position)) MathHandler.castE(target);
        }
        internal static void AntiGapCloser(ActiveGapcloser gapcloser)
        {
            if (MenuHandler.menu.Item("antiGap").GetValue<bool>())
            {
                if (gapcloser.Sender.FlatMagicDamageMod > 0 && SkillHandler.Q.IsReady() && gapcloser.Sender.IsValidTarget(SkillHandler.Q.Range)) SkillHandler.Q.Cast(gapcloser.Sender, Program.packetCast);
                if (SkillHandler.E.IsReady() && gapcloser.Sender.IsValidTarget(SkillHandler.E.Range)) SkillHandler.E.CastOnUnit(gapcloser.Sender, Program.packetCast);
            }
        }
        internal static void CancelChanneledSpells(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (MenuHandler.menu.Item("interruptSpells").GetValue<bool>())
            {
                if (SkillHandler.Q.IsReady() && unit.IsValidTarget(SkillHandler.Q.Range)) SkillHandler.Q.CastOnUnit(unit, Program.packetCast);
            }
        }

        internal static void onProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && (sender.Type == GameObjectType.obj_AI_Hero || sender.Type == GameObjectType.obj_AI_Turret))
            {
                if (sender.FlatMagicDamageMod > 0 && MenuHandler.menu.Item("blockMD").GetValue<bool>() && ObjectManager.Player.Distance(sender) <= SkillHandler.Q.Range)
                {
                    SkillHandler.Q.Cast(sender, Program.packetCast);
                }
                if (SpellData.SpellName.Any(Each => Each.Contains(args.SData.Name)) || (args.Target == ObjectManager.Player && ObjectManager.Player.Distance(sender) <= 700))
                    Program.UseShield = true;
            }
        }
        internal static void KillSecure()
        {
            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(h => 
                (h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.Q))
                    || (h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.E))
                        || (h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.R))))
            {
                if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.E) > target.Health)
                {
                    SkillHandler.Q.Cast(target, Program.packetCast);
                    break;
                }
                else if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.E) > target.Health)
                {
                    MathHandler.castE(target);
                    break;
                }
                else if(ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) > target.Health)
                {
                    MathHandler.castR(target);
                    break;
                }
            }
        }
    }
}
