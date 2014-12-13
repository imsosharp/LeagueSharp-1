using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace TAC_Kalista
{
    class FightHandler
    {
        internal static Obj_AI_Hero Soul;
        public static void OnCombo()
        {
            var targetsInRange = SimpleTs.GetTarget(ObjectManager.Player.AttackRange, SimpleTs.DamageType.Physical);
            if (targetsInRange == null && Utility.CountEnemysInRange((int)SkillHandler.Q.Range) > 0 && MenuHandler.Config.Item("stickToTarget").GetValue<bool>())
                MenuHandler.Orb.ForceTarget(GetDashObject);

            if (MenuHandler.Config.Item("useItems").GetValue<KeyBind>().Active) ItemHandler.UseItem();

            if (MenuHandler.Config.Item("UseQAC").GetValue<bool>() || 
                    (ObjectManager.Get<Obj_AI_Hero>().Any(
                        hero => hero.IsValidTarget(SkillHandler.Q.Range - 50f)
                            && hero.Health < (MathHandler.GetRealDamage(hero) - SkillHandler.Q.GetDamage(hero))
                )))
            {
                CustomQCast(SimpleTs.GetTarget(SkillHandler.Q.Range, SimpleTs.DamageType.Physical));
            }

            if (SkillHandler.E.IsReady() && (( ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsValidTarget(SkillHandler.E.Range)
                && (MathHandler.CheckBuff(hero).Count-1) >= MenuHandler.Config.Item("minE").GetValue<Slider>().Value
                            ) && MenuHandler.Config.Item("minEE").GetValue<bool>())
                            || (MenuHandler.Config.Item("UseEAC").GetValue<bool>()
                    && ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsValidTarget(SkillHandler.E.Range)
                           && hero.Health < MathHandler.GetRealDamage(hero)))
                            || (SkillHandler.Q.IsReady() && MenuHandler.Config.Item("UseEACSlow").GetValue<bool>()
                        && ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsValidTarget(SkillHandler.E.Range) 
                            && ObjectManager.Player.Distance(hero) > (SkillHandler.E.Range - 110)
                                && ObjectManager.Player.Distance(hero) < SkillHandler.E.Range
                                    && hero.CountEnemysInRange((int)SkillHandler.E.Range) <= MenuHandler.Config.Item("UseEACSlowT").GetValue<Slider>().Value
                                    || ((MathHandler.CheckBuff(hero).Count-1) > 2 && (MathHandler.CheckBuff(hero).EndTime - Game.Time) <= 0.15)
                            )

                        )))
            {
                SkillHandler.E.Cast();
            }
            if (SkillHandler.E.IsReady())
                MathHandler.CastMinionE(SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Physical));
        }
        public static void OnHarass()
        {
            var target = SimpleTs.GetTarget(SkillHandler.E.Range, SimpleTs.DamageType.Physical);
            var percentManaAfterQ = 100 * ((ObjectManager.Player.Mana - SkillHandler.Q.Instance.ManaCost) / ObjectManager.Player.MaxMana);
            var percentManaAfterE = 100 * ((ObjectManager.Player.Mana - SkillHandler.E.Instance.ManaCost) / ObjectManager.Player.MaxMana);
            var minPercentMana = MenuHandler.Config.SubMenu("Harass").Item("manaPercent").GetValue<Slider>().Value;

            if (percentManaAfterQ >= minPercentMana && MenuHandler.Config.Item("harassQ").GetValue<bool>() && SkillHandler.Q.IsReady()) CustomQCast(target);
            if (SkillHandler.E.IsReady()
                    && ObjectManager.Get<Obj_AI_Hero>().Any(
                        hero => hero.IsValidTarget(SkillHandler.E.Range) 
                            &&
                                (MathHandler.CheckBuff(hero).Count-1) >= MenuHandler.Config.Item("stackE").GetValue<Slider>().Value                    
                            )
                 &&
                    percentManaAfterE >= minPercentMana)
            {
                SkillHandler.E.Cast(Kalista.PacketCast);
            }
            if(SkillHandler.E.IsReady() && target.IsValidTarget(SkillHandler.E.Range))
            {
                MathHandler.CastMinionE(SimpleTs.GetTarget(SkillHandler.E.Range,SimpleTs.DamageType.Physical));
            }
        }

        public static void SaveSould()
        {
            if (Soul == null)
            {
                foreach (var ally in
                    from ally in ObjectManager.Get<Obj_AI_Hero>().Where(tx => tx.IsAlly && !tx.IsDead && !tx.IsMe)
                    where ObjectManager.Player.Distance(ally) <= SkillHandler.R.Range
                    from buff in ally.Buffs
                    where ally.HasBuff("kalistacoopstrikeally")
                    select ally)
                {
                    Soul = ally;
                    break;
                }
            }
            else
            {
                if((Soul.Health/Soul.MaxHealth) > MenuHandler.Config.Item("soulHP").GetValue<Slider>().Value 
                        && Soul.CountEnemysInRange((int)Orbwalking.GetRealAutoAttackRange(Soul)) >= MenuHandler.Config.Item("soulEnemyCount").GetValue<Slider>().Value)
                {
                    SkillHandler.R.Cast(Kalista.PacketCast);
                }
            }
        }

        internal static void AntiGapCloser(ActiveGapcloser gapcloser)
        {
            if (!SkillHandler.Q.IsReady() ||
                !gapcloser.Sender.IsValidTarget(MenuHandler.Config.Item("antiGapRange").GetValue<Slider>().Value) ||
                ((MenuHandler.Orb.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                  !MenuHandler.Config.Item("antiGapPrevent").GetValue<bool>()) ||
                 !MenuHandler.Config.Item("antiGap").GetValue<bool>() ||
                 !gapcloser.Sender.IsValidTarget(MenuHandler.Config.Item("antiGapRange").GetValue<Slider>().Value)))
                return;
            SkillHandler.Q.CastOnUnit(gapcloser.Sender, Kalista.PacketCast);
            Orbwalking.Orbwalk(gapcloser.Sender, Game.CursorPos);
        }
        public static void CustomQCast(Obj_AI_Hero target)
        {
            if (!SkillHandler.Q.IsReady() || target == null || ObjectManager.Player.IsDashing()) return;
            if ((100 * ((ObjectManager.Player.Mana - SkillHandler.Q.Instance.ManaCost) / ObjectManager.Player.MaxMana)) <= 3) return;

            var po = SkillHandler.Q.GetPrediction(target);
            var canCast = false;
            switch (MenuHandler.Config.Item("UseQACM").GetValue<StringList>().SelectedIndex)
            {
                case 1:
                    if (po.Hitchance >= HitChance.Low) canCast = true;
                    break;
                case 2:
                    if (po.Hitchance >= HitChance.Medium) canCast = true;
                    break;
                case 3:
                    if (po.Hitchance >= HitChance.High) canCast = true;
                    break;
            }
            if (canCast && ObjectManager.Player.Distance(po.UnitPosition) < SkillHandler.Q.Range)
            {
                SkillHandler.Q.Cast(po.CastPosition, Kalista.PacketCast);
            }
            else if (po.Hitchance == HitChance.Collision)
            {
                List<Obj_AI_Base> coll = po.CollisionObjects;
                Obj_AI_Base goal = coll.FirstOrDefault(obj => SkillHandler.Q.GetPrediction(obj).Hitchance >= HitChance.Medium && SkillHandler.Q.GetDamage(target) > obj.Health);
                if (goal != null) SkillHandler.Q.Cast(goal, Kalista.PacketCast);
            }
        }
        #region Hellsing

        public static void OnLaneClear()
        {
            if (!MenuHandler.Config.Item("enableClear").GetValue<bool>()) return;
            if (MenuHandler.Config.Item("wcQ").GetValue<bool>() && SkillHandler.Q.IsReady() && !ObjectManager.Player.IsDashing())
            {
                var minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.BaseSkinName.Contains("Minion") && m.IsValidTarget(SkillHandler.Q.Range)).ToList();
                if (minions.Count >= 3)
                {
                    minions.Sort((m1, m2) => m2.Distance(ObjectManager.Player, true).CompareTo(m1.Distance(ObjectManager.Player, true)));
                    var bestHitCount = 0;
                    PredictionOutput bestResult = null;
                    foreach (var minion in minions)
                    {
                        var prediction = SkillHandler.Q.GetPrediction(minion);
                        var targets = prediction.CollisionObjects;
                        targets.Sort((t1, t2) => t1.Distance(ObjectManager.Player, true).CompareTo(t2.Distance(ObjectManager.Player, true)));
                        targets.Add(minion);
                        for (var i = 0; i < targets.Count; i++)
                        {
                            if (
                                !(ObjectManager.Player.GetSpellDamage(targets[i], SpellSlot.Q)*0.8 < targets[i].Health) &&
                                i != targets.Count) continue;
                            if (i >= 3 && (bestResult == null || bestHitCount < i))
                            {
                                bestHitCount = i;
                                bestResult = prediction;
                            }
                            break;
                        }
                    }
                    if (bestResult != null) SkillHandler.Q.Cast(bestResult.CastPosition);
                }
            }

            if (MenuHandler.Config.Item("wcE").GetValue<bool>() && SkillHandler.E.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, SkillHandler.E.Range);
                if (minions.Count >= 3)
                {
                    var conditionMet = minions.Count(minion => MathHandler.GetRealDamage(minion)*0.9 > minion.Health);
                    if (conditionMet >= 3) SkillHandler.E.Cast(true);
                }
                var minionsBig = MinionManager.GetMinions(ObjectManager.Player.Position, SkillHandler.E.Range).Where(m => m.BaseSkinName.Contains("MinionSiege"));
                if (minionsBig.Any(minion => MathHandler.GetRealDamage(minion) > minion.Health))
                {
                    SkillHandler.E.Cast(true);
                }
            }
        }
        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "KalistaExpungeWrapper")
                Utility.DelayAction.Add(250, Orbwalking.ResetAutoAttackTimer);
        }
        internal static Obj_AI_Base GetDashObject
        {
            get
            {
                var realAArange = Orbwalking.GetRealAutoAttackRange(ObjectManager.Player);

                var objects = ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValidTarget(realAArange));
                var apexPoint = ObjectManager.Player.ServerPosition.To2D() + (ObjectManager.Player.ServerPosition.To2D() - Game.CursorPos.To2D()).Normalized() * realAArange;

                Obj_AI_Base target = null;

                foreach (var obj in objects)
                {
                    if (
                        !IsLyingInCone(obj.ServerPosition.To2D(), apexPoint, ObjectManager.Player.ServerPosition.To2D(),
                            realAArange)) continue;
                    if (target == null || target.Distance(apexPoint, true) > obj.Distance(apexPoint, true))
                        target = obj;
                }

                return target;
            }
        }
        internal static bool IsLyingInCone(Vector2 position, Vector2 apexPoint, Vector2 circleCenter, float aperture)
        {
            var halfAperture = aperture / 2.0f;
            var apexToXVect = apexPoint - position;
            var axisVect = apexPoint - circleCenter;
            var isInInfiniteCone = DotProd(apexToXVect, axisVect) / Magn(apexToXVect) / Magn(axisVect) > Math.Cos(halfAperture);
            if (!isInInfiniteCone)
                return false;
            var isUnderRoundCap = DotProd(apexToXVect, axisVect) / Magn(axisVect) < Magn(axisVect);

            return isUnderRoundCap;
        }
        internal static float DotProd(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
        internal static float Magn(Vector2 a)
        {
            return (float)(Math.Sqrt(a.X * a.X + a.Y * a.Y));
        }
        #endregion
    }
}
