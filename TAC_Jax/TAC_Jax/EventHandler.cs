using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

//JaxRelentlessAssaultAS -- When jax is auto attacking
//JaxEvasion -- When Jax is casting E
//EmpoweredTwo --When Jax presses W
//MasterySpellWeaving -- When Jax is using W on target
//ItemPhageSpeed
namespace TAC_Jax
{
    class EventHandler
    {
        internal static Vector3 lastWardPos;
        internal static int lastPlaced;
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
                /*if (LXOrbwalker.CurrentMode == LXOrbwalker.Mode.Combo 
                        && ObjectManager.Player.Distance(targets) < (SkillHandler.E.Range - 30f))
                {
                    SkillHandler.E.Cast(Jax.packetCast);
                }*/
            }
            //jaxcounterstrike
            //jaxleapstrike
        }
        internal static bool canDieFromLeaping(Obj_AI_Hero target)
        {
            return target.Health < (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) + ObjectManager.Player.GetSpellDamage(target,SpellSlot.W));
        }
        internal static void onHarass()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.Q.Range, SimpleTs.DamageType.Physical);
            if(target != null)
            {
                /**
                 * Check if my Q is available
                 * Check if my 3'rd R is available
                 * Check if my W is available
                 */
                if(SkillHandler.Q.IsReady() && SkillHandler.W.IsReady())
                {
                    if(ObjectManager.Player.Level >= 6 && Jax.buffCount > 0 && Jax.buffCount % 3 == 0)
                    {
                        SkillHandler.E.Cast(Jax.packetCast);
                        SkillHandler.Q.Cast(target,Jax.packetCast);
                        Utility.DelayAction.Add(5, () =>
                        {
                            SkillHandler.W.Cast(Jax.packetCast);
                        });
                    }
                    else
                    {
                        Jax.isCastingE = true;
                        Jax.lastTickE = Environment.TickCount;
                        SkillHandler.E.Cast(Jax.packetCast);
                        SkillHandler.W.Cast(Jax.packetCast);
                        SkillHandler.Q.Cast(Jax.packetCast);
                    }
                }
            }
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
                if(SkillHandler.E.IsReady() && SkillHandler.Q.IsReady() && ObjectManager.Player.Distance(target) < SkillHandler.Q.Range)
                {
                    Jax.lastTickE = Environment.TickCount;
                    Jax.isCastingE = true;
                    SkillHandler.E.Cast(Jax.packetCast);
                    Jax.buffCountBeforeQ = Jax.buffCount;
                    SkillHandler.Q.Cast(target, Jax.packetCast);
                }
                if(ObjectManager.Player.Distance(target) < SkillHandler.E.Range && !Jax.isCastingE && SkillHandler.E.IsReady())
                {
                    Jax.lastTickE = Environment.TickCount;
                    Jax.isCastingE = true;
                    Jax.canCastE = false;
                }
                if(Jax.isCastingE && Jax.canCastE && ObjectManager.Player.Distance(target) < (SkillHandler.E.Range - 30f))
                {
                    SkillHandler.E.Cast(Jax.packetCast);
                    Jax.canCastE = false;
                    Jax.isCastingE = false;
                }


                // if we are out of distance we need to cast our Q to get near him.
                // if he is not valid target in our range we have to cast q to get near him.
                if(!target.IsValidTarget(SkillHandler.E.Range))
                {
                    Jax.buffCountBeforeQ = Jax.buffCount;
                    SkillHandler.Q.Cast(target,Jax.packetCast);
                }

                /* Check if player Health is below W damage,
                 * so then we can use it without auto-attacking
                 * or else it's useless to waste W + auto-attack,
                 * since W is a Auto-Attack reset.
                 * Also check if I just used Q and auto-attacked 
                 * for biggest damage possible
                 */
                if ((target.Health < ObjectManager.Player.GetSpellDamage(target,SpellSlot.W)) 
                    || (!ObjectManager.Player.IsDashing() && SkillHandler.W.IsReady() && Jax.buffCountBeforeQ != Jax.buffCount))
                {
                    SkillHandler.W.Cast(Jax.packetCast);
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
                            Jax.buffCountBeforeQ = Jax.buffCount;
                            // Since flash is packet casting, we need to delay it for atleast 0.01 second
                            SkillHandler.Q.Cast(target.Position, Jax.packetCast);
                        });
                    }
                }
            }
        }
        /**
         * @author xQx
         * */
        internal static void onLaneClear()
        {
            if (MenuHandler.Config.Item("lane_enabled").GetValue<bool>())
            {
                List<Obj_AI_Base> vMinions;
                vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, SkillHandler.E.Range, MinionTypes.All, MinionTeam.NotAlly);
                if(vMinions.Count < 1) // reset to jungle clear
                    vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, SkillHandler.E.Range, MinionTypes.All, MinionTeam.Neutral);
                foreach (var vMinion in vMinions)
                {
                    if (MenuHandler.Config.Item("UseWLaneClear").GetValue<bool>() && SkillHandler.W.IsReady() && ObjectManager.Player.Distance(vMinion) < SkillHandler.E.Range)
                        SkillHandler.W.Cast(Jax.packetCast);

                    if (MenuHandler.Config.Item("UseWLaneClear").GetValue<bool>() && SkillHandler.E.IsReady() && ObjectManager.Player.Distance(vMinion) < SkillHandler.E.Range)
                        SkillHandler.E.Cast(Jax.packetCast);
                }
            }
        }
        internal static double comboDamage(Obj_AI_Hero target)
        {
            /**
             * TODO:
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

        /**
         * @author xSalice
         */

        internal static void WardJump()
        {
            foreach (Obj_AI_Minion ward in ObjectManager.Get<Obj_AI_Minion>().Where(ward =>
                ward.Name.ToLower().Contains("ward") && ward.Distance(Game.CursorPos) < 250))
            {
                if (SkillHandler.E.IsReady())
                {
                    SkillHandler.E.CastOnUnit(ward);
                    return;
                }
            }

            foreach (
                Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Distance(Game.CursorPos) < 250 && !hero.IsDead))
            {
                if (SkillHandler.Q.IsReady())
                {
                    SkillHandler.Q.CastOnUnit(hero);
                    return;
                }
            }

            foreach (Obj_AI_Minion minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion =>
                minion.Distance(Game.CursorPos) < 250))
            {
                if (SkillHandler.Q.IsReady())
                {
                    SkillHandler.Q.CastOnUnit(minion);
                    return;
                }
            }

            if (Environment.TickCount <= lastPlaced + 3000 || !SkillHandler.E.IsReady()) return;

            Vector3 cursorPos = Game.CursorPos;
            Vector3 myPos = ObjectManager.Player.Position;

            Vector3 delta = cursorPos - myPos;
            delta.Normalize();

            Vector3 wardPosition = myPos + delta * (600 - 5);

            InventorySlot invSlot = FindBestWardItem();
            if (invSlot == null) return;

            Items.UseItem((int)invSlot.Id, wardPosition);
            lastWardPos = wardPosition;
            lastPlaced = Environment.TickCount;
        }

        private static InventorySlot FindBestWardItem()
        {
            InventorySlot slot = Items.GetWardSlot();
            if (slot == default(InventorySlot)) return null;
            return slot;
        }
    }
}
