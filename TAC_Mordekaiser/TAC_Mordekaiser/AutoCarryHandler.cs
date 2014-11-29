using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Mordekaiser
{
    class AutoCarryHandler
    {
        internal static void onCombo()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Magical);
            float distance = ObjectManager.Player.Distance(target);
            if(SkillHandler.R.IsReady() && MathHandler.getTotalDamageToTarget(target) > target.Health && distance < ItemHandler.Item.Range)
            {
                ItemHandler.Item.Cast(target);
                SkillHandler.R.Cast(target, Program.packetCast);
                ItemHandler.castIgnite(target);
            }
            if(distance < SkillHandler.E.Range && SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(target, Program.packetCast);
            }
            if (SkillHandler.W.IsReady()) SkillHandler.W.Cast(ObjectManager.Player, Program.packetCast);
            if(distance < SkillHandler.Q.Range && SkillHandler.Q.IsReady())
            {
                SkillHandler.Q.Cast(target, Program.packetCast);
            }
        }

        internal static void Mixed()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Magical);
            float distance = ObjectManager.Player.Distance(target);

            if (distance < SkillHandler.E.Range && SkillHandler.E.IsReady())
            {
                SkillHandler.E.Cast(target, Program.packetCast);
            }
            if (distance < SkillHandler.Q.Range && SkillHandler.Q.IsReady())
            {
                SkillHandler.Q.Cast(target, Program.packetCast);
            }
        }

        internal static void onProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && (sender.Type == GameObjectType.obj_AI_Hero || sender.Type == GameObjectType.obj_AI_Turret))
            {
                if (SpellData.SpellName.Any(Each => Each.Contains(args.SData.Name)) || (args.Target == ObjectManager.Player && ObjectManager.Player.Distance(sender) <= 700))
                {
                    if (SkillHandler.W.IsReady()) SkillHandler.W.Cast(ObjectManager.Player, Program.packetCast);
                }
            }
        }
        internal static void AntiGapCloser(ActiveGapcloser gapcloser)
        {
            if (SkillHandler.W.IsReady() && gapcloser.Sender.IsValidTarget(SkillHandler.W.Range)) SkillHandler.W.CastOnUnit(gapcloser.Sender, Program.packetCast);
            if (SkillHandler.E.IsReady() && gapcloser.Sender.IsValidTarget(SkillHandler.E.Range)) SkillHandler.E.CastOnUnit(gapcloser.Sender, Program.packetCast);
        }
    }
}
