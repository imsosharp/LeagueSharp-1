using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Kalista
{
    class FightHandler
    {
        public static void OnCombo()
        {
            // Cast Q if out of range with E

            if (MenuHandler.Config.Item("UseQAC").GetValue<bool>() || 
                    (ObjectManager.Get<Obj_AI_Hero>().Any(
                        hero => hero.IsValidTarget(SkillHandler.E.Range+400)
                            && hero.Health < (MathHandler.getRealDamage(hero) - SkillHandler.Q.GetDamage(hero))
                )))
            {

                customQCast(SimpleTs.GetTarget(SkillHandler.Q.Range, SimpleTs.DamageType.Physical));
            }

            if (SkillHandler.E.IsReady() && (( ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsValidTarget(SkillHandler.E.Range)
                && hero.Buffs.FirstOrDefault(b => b.Name.ToLower() == "kalistaexpungemarker").Count >= MenuHandler.Config.Item("minE").GetValue<Slider>().Value
                            ) && MenuHandler.Config.Item("minEE").GetValue<bool>()) 
                            // auto e
                            || (MenuHandler.Config.Item("UseEAC").GetValue<bool>()
                    && ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsValidTarget(SkillHandler.E.Range)
                           && hero.Health < MathHandler.getRealDamage(hero)))
                            || (SkillHandler.Q.IsReady() && MenuHandler.Config.Item("UseEACSlow").GetValue<bool>()
                        && ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsValidTarget(SkillHandler.E.Range) 
                            && ObjectManager.Player.Distance(hero) > (SkillHandler.E.Range - 110)
                                && ObjectManager.Player.Distance(hero) < SkillHandler.E.Range
                                    && hero.CountEnemysInRange((int)SkillHandler.E.Range) <= MenuHandler.Config.Item("UseEACSlowT").GetValue<Slider>().Value
                            )

                        )))
            {
                SkillHandler.E.Cast();
            }

            if (MenuHandler.Config.Item("useItems").GetValue<KeyBind>().Active
                /*// Auto all in at X champions    
                    || ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player))
                            && hero.CountEnemysInRange((int)Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)) <= MenuHandler.Config.Item("allInAt").GetValue<Slider>().Value)*/)
            {
                ItemHandler.useItem();
            }
        }
        public static void OnHarass()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Physical);
            float percentManaAfterQ = 100 * ((ObjectManager.Player.Mana - SkillHandler.Q.Instance.ManaCost) / ObjectManager.Player.MaxMana);
            float percentManaAfterE = 100 * ((ObjectManager.Player.Mana - SkillHandler.E.Instance.ManaCost) / ObjectManager.Player.MaxMana);
            int minPercentMana = MenuHandler.Config.SubMenu("Harass").Item("manaPercent").GetValue<Slider>().Value;

            var first = SkillHandler.Q.GetPrediction(target).CollisionObjects[0];
            var second = SkillHandler.Q.GetPrediction(target).CollisionObjects[1];
            if (first == target || first.IsMinion && first.IsEnemy && first.Health < SkillHandler.Q.GetDamage(first) && second == target && percentManaAfterQ >= minPercentMana) FightHandler.customQCast(target);
            if (SkillHandler.E.IsReady()
                    && ObjectManager.Get<Obj_AI_Hero>().Any(
                        hero => hero.IsValidTarget(SkillHandler.E.Range) 
                            &&
                                hero.Buffs.FirstOrDefault(b => b.Name.ToLower() == "kalistaexpungemarker").Count >= MenuHandler.Config.Item("stackE").GetValue<Slider>().Value                    
                            )
                 &&
                    percentManaAfterE >= minPercentMana)
            {
                SkillHandler.E.Cast(Kalista.packetCast);
            }
        }
        public static void OnLaneClear()
        {
            if (MenuHandler.Config.Item("enableClear").GetValue<bool>() && MenuHandler.Config.Item("useEwc").GetValue<bool>() && SkillHandler.E.IsReady())
            {
                List<Obj_AI_Base> minions = MinionManager.GetMinions(ObjectManager.Player.Position, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player),MinionTypes.All,MinionTeam.Enemy,MinionOrderTypes.Health);
                foreach (var data in minions)
                {
                    if (MathHandler.getRealDamage(data) >= data.Health)
                        SkillHandler.E.Cast();
                }
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "KalistaExpungeWrapper") 
                Utility.DelayAction.Add(250,Orbwalking.ResetAutoAttackTimer);
        }

        public static void customQCast(Obj_AI_Hero target)
        {
            if (!SkillHandler.Q.IsReady() || target == null) return;

            PredictionOutput po = SkillHandler.Q.GetPrediction(target);
            int canCast = 0;
            switch (MenuHandler.Config.Item("UseQACM").GetValue<StringList>().SelectedIndex)
            {
                case 1:
                    if (po.Hitchance >= HitChance.Low) canCast = 1;
                    break;
                case 2:
                    if (po.Hitchance >= HitChance.Medium) canCast = 1;
                    break;
                case 3:
                    if (po.Hitchance >= HitChance.High) canCast = 1;
                    break;
            }
            if (canCast > 0 && ObjectManager.Player.Distance(po.UnitPosition) < SkillHandler.Q.Range )
            {
                SkillHandler.Q.Cast(po.CastPosition, Kalista.packetCast);
            } 
                /*
            else if (po.Hitchance == HitChance.Collision)
            {
                var collisions = SkillHandler.Q.GetCollision(ObjectManager.Player.Position.To2D(), new List<SharpDX.Vector2> { po.CastPosition.To2D() });
                foreach (var col in collisions.Where(x => x.IsMinion))
                {
                    if (col.Health > (float)Damage.GetSpellDamage(ObjectManager.Player, col, SpellSlot.Q) * 0.9f)
                    {
                        SkillHandler.Q.Cast(po.CastPosition,Kalista.packetCast);
                        break;
                    }
                }
            }*/
        }
    }
}
