using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

//JaxRelentlessAssaultAS -- When jax is auto attacking
//JaxEvasion -- When Jax is casting E
//EmpoweredTwo --When Jax presses W
//MasterySpellWeaving -- When Jax is using W on target
//ItemPhageSpeed
namespace TAC_Jax
{
    class EventHandler
    {
        internal static void AntiGapCloser(ActiveGapcloser gapcloser)
        {
            if (ObjectManager.Player.Distance(gapcloser.Sender) < MenuHandler.Config.Item("gapcloseRange_E").GetValue<Slider>().Value && MenuHandler.Config.Item("gapclose_E").GetValue<bool>())
            {
                SkillHandler.E.Cast(Jax.packetCast);
            }
        }
        internal static void Game_OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!unit.IsMe) return;
            //Game.PrintChat("spell.SData.Name.ToLower: -- " + spell.SData.Name.ToLower());
            if(spell.SData.Name.ToLower() == "jaxcounterstrike")
            {
                Game.PrintChat("Helicopter active!");
                // Get enemy surounded :P
                Obj_AI_Hero targets = SimpleTs.GetTarget(SkillHandler.E.Range,SimpleTs.DamageType.Physical);
                // Check if our target is going escape and current mode is combo
                if (Jax.orb.ActiveMode == Orbwalking.OrbwalkingMode.Combo 
                        && ObjectManager.Player.Distance(targets) < (SkillHandler.E.Range - 30f))
                {
                    SkillHandler.E.Cast(Jax.packetCast);
                }
            }
            //jaxcounterstrike
            //jaxleapstrike
        }
        internal static bool canDieFromLeaping(Obj_AI_Hero target)
        {
            return target.Health < (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) + ObjectManager.Player.GetSpellDamage(target,SpellSlot.W));
        }
        internal static void onCombo()
        {
            // TODO: Make E cast when in range
            // make better w logic
            // need more tests on E out of range

            // TODO: Make mixed damage in TS and orbwalker
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.Q.Range,SimpleTs.DamageType.Physical);

            if(target != null)
            {
                if(SkillHandler.E.IsReady() && SkillHandler.Q.IsReady())
                {
                    SkillHandler.E.Cast(Jax.packetCast);
                    SkillHandler.Q.Cast(target, Jax.packetCast);
                }
                // Check W mode and only use Q + W if it's available or when out of actual range.
                switch(Jax.w_mode)
                {
                    case 3:
                    default:
                    // Let's check if we need to gap close to target
                    // Now if our target isn't in range of our E and we don't have phage buff,
                    // means we have to use helicopter
                    if (target.IsValidTarget(SkillHandler.E.Range) && SkillHandler.Q.IsReady() && !ObjectManager.Player.HasBuff("ItemPhageSpeed"))
                    {
                        if (!ObjectManager.Player.IsDashing() && SkillHandler.W.IsReady() && ObjectManager.Player.Distance(target) <= SkillHandler.W.Range)
                        {
                            SkillHandler.W.Cast(Jax.packetCast);
                        }
                        SkillHandler.Q.Cast(target, Jax.packetCast);
                        if (ObjectManager.Player.IsDashing() && canDieFromLeaping(target)) SkillHandler.W.Cast(Jax.packetCast);
                        Utility.DelayAction.Add(5, () =>
                            {
                                SkillHandler.W.Cast(Jax.packetCast);
                            });
                    }
                    break;
                // means if our W mode is helicopter, we will only use helicopter if Q available
                case 2:
                    if(target.IsValidTarget(SkillHandler.Q.Range) && SkillHandler.Q.IsReady() && SkillHandler.W.IsReady())
                    {
                        SkillHandler.W.Cast(target, Jax.packetCast);
                        SkillHandler.Q.Cast(target, Jax.packetCast);
                    }
                break;
                // means if our W mode is AA reset only we will use when we leap to 
                //actual target if were out of range and then use w
                case 1:
                    if(ObjectManager.Player.Distance(target) < SkillHandler.E.Range)
                        SkillHandler.Q.Cast(target, Jax.packetCast);
                    if (Jax.buffCount > 0)
                        SkillHandler.W.Cast(Jax.packetCast);
                break;
            }

                // if we are out of distance we need to cast our Q to get near him.
                // if he is not valid target in our range we have to cast q to get near him.
                if(!target.IsValidTarget(SkillHandler.E.Range))
                {
                    SkillHandler.Q.Cast(target,Jax.packetCast);
                }

                // Check if our flash and q is ready 
                if(SkillHandler.Q.IsReady() && SkillHandler.Flash.IsReady())
                {
                    // Check if it is actually worth the flash
                    // Only Flash Q if target is going to die, or else it's not worth
                    // TODO: Do another checkup on priority stuff
                    if (MenuHandler.Config.Item("acQ_useIfWorth").GetValue<bool>()
                            && target.ChampionsKilled > target.Deaths
                                && target.CountEnemysInRange((int)(SkillHandler.Q.Range + SkillHandler.Flash.Range)) < MenuHandler.Config.Item("acQ_useIfWorthEnemy").GetValue<Slider>().Value)
                    {
                        SkillHandler.Flash.Cast(target.Position,Jax.packetCast);
                        Utility.DelayAction.Add(10, () =>
                        {
                            // Since flash is packet casting, we need to delay it for atleast 0.01 second
                            SkillHandler.Q.Cast(target.Position, Jax.packetCast);
                        });
                    }
                }
            }
        }
        internal static double comboDamage(Obj_AI_Hero target)
        {
            /**
             * Done:
             * Check if it's a valid target in our range.
             * Check if next AA is available
             * Check ignite damage
             * Count witch stack it is now.
             * Check Q land + W + 3'rd R + E
             * */
            /**
             * In progress:
             * Check item damage.
             * */
            double damage = 0;
            /* Check if target is in valid Q range
             * Check if target is in valid flash Q range
             */
            if(target.IsValidTarget(SkillHandler.Q.Range) 
                    || (SkillHandler.Flash.IsReady() && target.IsValidTarget(SkillHandler.Q.Range + SkillHandler.Flash.Range)))
            {
                // Check if the auto is ready and only then add it to dmg bar ?
                if (Orbwalking.CanAttack()) damage += ObjectManager.Player.GetAutoAttackDamage(target);
                // Check if Q is ready
                if (SkillHandler.Q.IsReady()) damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
                // Check if W is ready
                if (SkillHandler.W.IsReady()) damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
                // Check if R 3rd stack is ready
                if (Jax.buffCount > 0 && Jax.buffCount % 3 == 0) damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);
                // Check ignite damage
                if (SkillHandler.Ignite.IsReady()) damage += SkillHandler.Ignite.GetDamage(target);
            }
            return damage;
        }
    }
}
