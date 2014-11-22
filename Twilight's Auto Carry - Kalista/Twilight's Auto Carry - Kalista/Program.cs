/*
 * 
 * LeagueSharp Kalista assembly
 * Copyright (C) 2014 Harold Schneider
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using System.Globalization;
using System.Threading;

namespace Twilight_s_Auto_Carry___Kalista
{
    class Program
    {
        private static Menu Config;
        public static Menu QuickSilverMenu;
        private static Obj_AI_Hero myHero = ObjectManager.Player;
        public static Orbwalking.Orbwalker Orbwalker;
        private static Spell Q, W, E, R;
        public static int minRange = 100;

        public static Obj_AI_Hero CoopStrikeAlly;
        public static float CoopStrikeAllyRange = 1250f;

        public static bool packetCast = true;
        public static bool debug = true;
        public static bool LaneClearActive;
        public static bool ComboActive;
        public static bool HarassActive;
        public static bool drawings;

        public static Activator AAC = new Activator();
        public static double ActivatorTime;

        public static Items.Item items = new Items.Item(3128, 750);
        private static readonly string[] MinionNames = {"TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith", "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "SRU_Dragon", "SRU_Baron", "Sru_Crab"};

        public static readonly Vector3[] wallhops = new[] { new Vector3(794, 5914, 50), new Vector3(792, 6208, -71), new Vector3(10906, 7498, 52), new Vector3(10872, 7208, 51), new Vector3(11900, 4870, 51), new Vector3(11684, 4694, -71), new Vector3(12046, 5376, 54), new Vector3(12284, 5382, 51), new Vector3(11598, 8676, 62), new Vector3(11776, 8890, 50), new Vector3(8646, 9584, 50), new Vector3(8822, 9406, 51), new Vector3(6606, 11756, 53), new Vector3(6494, 12056, 56), new Vector3(5164, 12090, 56), new Vector3(5146, 11754, 56), new Vector3(5780, 10650, 55), new Vector3(5480, 10620, -71), new Vector3(3174, 9856, 52), new Vector3(3398, 10080, -65), new Vector3(2858, 9448, 51), new Vector3(2542, 9466, 52), new Vector3(3700, 7416, 51), new Vector3(3702, 7702, 52), new Vector3(3224, 6308, 52), new Vector3(3024, 6312, 57), new Vector3(4724, 5608, 50), new Vector3(4610, 5868, 51), new Vector3(6124, 5308, 48), new Vector3(6010, 5522, 51), new Vector3(9322, 4514, -71), new Vector3(9022, 4508, 52), new Vector3(6826, 8628, -71), new Vector3(7046, 8750, 52), };
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Load;
        }
        public static void Load(EventArgs args)
        {
            
            if (myHero.ChampionName != "Kalista") return;
            Game.PrintChat("=========================");
            Game.PrintChat("| Twilight Auto Carry   |");
            Game.PrintChat("=========================");
            Game.PrintChat("Loading kalista plugin!");
            Game.PrintChat("Kalista loaded!");
            Game.PrintChat("=========================");
            Config = new Menu("TAC: Kalista", "Kalista", true);
            
            Q = new Spell(SpellSlot.Q, 1450);
            W = new Spell(SpellSlot.W, 5500);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 1200);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);


            var orbwalking = Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalking);
            
            Config.AddSubMenu(new Menu("AutoCarry options", "ac"));
            Config.SubMenu("ac").AddItem(new MenuItem("UseQAC", "Use Q").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("UseEAC", "Use E").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("E4K", "E only for kill").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("55", "^ false ? then"));
            Config.SubMenu("ac").AddItem(new MenuItem("minE", "Min stacks to E").SetValue(new Slider(1, 1, 20)));

            Config.AddSubMenu(new Menu("Harass options", "harass"));
            Config.SubMenu("harass").AddItem(new MenuItem("stackE", "E stacks to cast").SetValue(new Slider(1, 1, 10)));
            Config.SubMenu("harass").AddItem(new MenuItem("manaPercent", "Mana %").SetValue(new Slider(40, 1, 100)));

            Config.AddSubMenu(new Menu("Wave Clear options", "wc"));
            Config.SubMenu("wc").AddItem(new MenuItem("useEwc", "Use E to clear").SetValue(true));
            Config.SubMenu("wc").AddItem(new MenuItem("enableClear", "WaveClear enabled?").SetValue(false));


            Config.AddSubMenu(new Menu("Smite options", "smite"));
            Config.SubMenu("smite").AddItem(new MenuItem("1", "Cause bugsplat"));
            Config.SubMenu("smite").AddItem(new MenuItem("2", "not working yet"));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Baron", "Baron Enabled").SetValue(true));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Dragon", "Dragon Enabled").SetValue(true));
            Config.SubMenu("smite").AddItem(new MenuItem("smite", "Auto-Smite enabled").SetValue(true));


            Config.AddSubMenu(new Menu("Wall Hop options", "wh"));
            Config.SubMenu("wh").AddItem(new MenuItem("drawSpot", "Draw WallHop spots").SetValue(true));
            Config.SubMenu("wh").AddItem(new MenuItem("dh", "Draw range").SetValue(new Slider(1000,200,10000)));

            var items = Config.AddSubMenu(new Menu("Items", "Items"));
            items.AddItem(new MenuItem("BOTRK", "BOTRK").SetValue(true));
            items.AddItem(new MenuItem("GHOSTBLADE", "Ghostblade").SetValue(true));
            items.AddItem(new MenuItem("SWORD", "Sword of the Divine").SetValue(true));

            QuickSilverMenu = new Menu("QSS", "QuickSilverSash");
            items.AddSubMenu(QuickSilverMenu);
            QuickSilverMenu.AddItem(new MenuItem("AnyStun", "Any Stun").SetValue(true));
            QuickSilverMenu.AddItem(new MenuItem("AnySnare", "Any Snare").SetValue(true));
            QuickSilverMenu.AddItem(new MenuItem("AnyTaunt", "Any Taunt").SetValue(true));
            foreach (var t in AAC.BuffList)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy))
                {
                    if (t.ChampionName == enemy.ChampionName)
                        QuickSilverMenu.AddItem(new MenuItem(t.BuffName, t.DisplayName).SetValue(t.DefaultValue));
                }
            }
            items.AddItem(
            new MenuItem("UseItemsMode", "Use items on").SetValue(
            new StringList(new[] { "No", "Mixed mode", "Combo mode", "Both" }, 2)));
            var extras = new Menu("Extras", "Extras");
            new PotionManager(extras);
            Config.AddSubMenu(extras);
            
//            levelUpManager.AddToMenu(ref Config);

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawConnText", "Draw connection Text").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 0))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawConnSignal", "Draw connection signal").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawText", "Draw damage text").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawHp", "Draw damage HP bar")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("enableDrawings", "Enable all drawings").SetValue(true));

            

            Config.AddItem(new MenuItem("Packets", "Packet Casting").SetValue(true));

            Config.AddItem(new MenuItem("debug", "Debug").SetValue(true));
            Config.AddItem(new MenuItem("showPos", "Server Position").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Press)));
            Config.AddToMainMenu();
            
//            InitializeLevelUpManager();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += OnGameUpdate;
            Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
        }
        private static void CheckChampionBuff()
        {
            foreach (var t1 in ObjectManager.Player.Buffs)
            {
                foreach (var t in QuickSilverMenu.Items)
                {
                    if (QuickSilverMenu.Item(t.Name).GetValue<bool>())
                    {
                        if (t1.Name.ToLower().Contains(t.Name.ToLower()))
                        {
                            foreach (var bx in AAC.BuffList.Where(bx => bx.BuffName == t1.Name))
                            {
                                if (bx.Delay > 0)
                                {
                                    if (ActivatorTime + bx.Delay < (int)Game.Time)
                                        ActivatorTime = (int)Game.Time;

                                    if (ActivatorTime + bx.Delay <= (int)Game.Time)
                                    {
                                        if (Items.HasItem(3139)) Items.UseItem(3139);
                                        if (Items.HasItem(3140)) Items.UseItem(3140);
                                        ActivatorTime = (int)Game.Time;
                                    }
                                }
                                else
                                {
                                    if (Items.HasItem(3139)) Items.UseItem(3139);
                                    if (Items.HasItem(3140)) Items.UseItem(3140);
                                }
                            }
                        }
                    }

                    if (QuickSilverMenu.Item("AnySnare").GetValue<bool>() &&
                        ObjectManager.Player.HasBuffOfType(BuffType.Snare))
                    {
                        if (Items.HasItem(3139)) Items.UseItem(3139);
                        if (Items.HasItem(3140)) Items.UseItem(3140);
                    }
                    if (QuickSilverMenu.Item("AnyStun").GetValue<bool>() &&
                        ObjectManager.Player.HasBuffOfType(BuffType.Stun))
                    {
                        if (Items.HasItem(3139)) Items.UseItem(3139);
                        if (Items.HasItem(3140)) Items.UseItem(3140);
                    }
                    if (QuickSilverMenu.Item("AnyTaunt").GetValue<bool>() &&
                        ObjectManager.Player.HasBuffOfType(BuffType.Taunt))
                    {
                        if (Items.HasItem(3139)) Items.UseItem(3139);
                        if (Items.HasItem(3140)) Items.UseItem(3140);
                    }
                }
            }
        }
        public static Obj_AI_Minion GetNearest(Vector3 pos)
        {
            var minions =
            ObjectManager.Get<Obj_AI_Minion>()
            .Where(minion => minion.IsValid && MinionNames.Any(name => minion.Name.StartsWith(name)) && !MinionNames.Any(name => minion.Name.Contains("Mini")));
            var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
            Obj_AI_Minion sMinion = objAiMinions.FirstOrDefault();
            double? nearest = null;
            foreach (Obj_AI_Minion minion in objAiMinions)
            {
                double distance = Vector3.Distance(pos, minion.Position);
                if (nearest == null || nearest > distance)
                {
                    nearest = distance;
                    sMinion = minion;
                }
            }
            return sMinion;
        }
        /**
         * @author xSLx (Lexxes)
         */
        private static void Cast_Q(Obj_AI_Hero target)
        {
            if (!Q.IsReady() || target == null)
                return;
//            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            var qPred = Q.GetPrediction(target);
            if (qPred.Hitchance >= HitChance.Medium)
                Q.Cast(target, packetCast);
            if (qPred.Hitchance != HitChance.Collision)
                return;
            var coll = qPred.CollisionObjects;
            var goal = coll.FirstOrDefault(obj => Q.GetPrediction(obj).Hitchance >= HitChance.Medium && Q.GetDamage(target) > obj.Health);
            if (goal != null)
                Q.Cast(goal, packetCast);
        }
        public static float getPerValue(bool mana)
        {
            if (mana) return (myHero.Mana / myHero.MaxMana) * 100;
            return (myHero.Health / myHero.MaxHealth) * 100;
        }

        public static void OnGameUpdate(EventArgs args)
        {
            drawings = Config.Item("enableDrawings").GetValue<bool>();
            debug = Config.Item("debug").GetValue<bool>();
            packetCast = Config.Item("Packets").GetValue<bool>();
            
            ComboActive = Config.Item("Orbwalk").GetValue<KeyBind>().Active;
            HarassActive = Config.Item("Farm").GetValue<KeyBind>().Active;
            LaneClearActive = Config.Item("LaneClear").GetValue<KeyBind>().Active;

            if (ComboActive)
            {

                Combo();
            }
            else if (HarassActive)
            {
                Harass();
            }
            else if (LaneClearActive)
            {
                LaneClear();
            }

            var botrk = Config.Item("BOTRK").GetValue<bool>();
            var ghostblade = Config.Item("GHOSTBLADE").GetValue<bool>();
            var sword = Config.Item("SWORD").GetValue<bool>();
            var target = Orbwalker.GetTarget();
            var igniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            if (Config.Item("showPos").GetValue<KeyBind>().Active)
            {
                Game.PrintChat("Position on server: " + myHero.ServerPosition);
            }
            if (Config.Item("smite").GetValue<bool>())
            {
                var mob = GetNearest(myHero.ServerPosition);
                if (mob != null && Config.Item(mob.SkinName).GetValue<bool>())
                {

                    if (mob.Health < GetRendDamage(mob))
                    {
                        E.Cast(packetCast);
                    }
                }
            }
            var useItemModes = Config.Item("UseItemsMode").GetValue<StringList>().SelectedIndex;
            if (
 !((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
 (useItemModes == 2 || useItemModes == 3))
 ||
 (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
 (useItemModes == 1 || useItemModes == 3))))
                return;
            if (botrk)
            {
                if (target != null && target.Type == ObjectManager.Player.Type &&
                target.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 450)
                {
                    var hasCutGlass = Items.HasItem(3144);
                    var hasBotrk = Items.HasItem(3153);
                    if (hasBotrk || hasCutGlass)
                    {
                        var itemId = hasCutGlass ? 3144 : 3153;
                        var damage = ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                        if (hasCutGlass || ObjectManager.Player.Health + damage < ObjectManager.Player.MaxHealth)
                            Items.UseItem(itemId, target);
                    }
                }
            }
            if (ghostblade && target != null && target.Type == ObjectManager.Player.Type &&
            !ObjectManager.Player.HasBuff("ItemSoTD", true) /*if Sword of the divine is not active */
            && Orbwalking.InAutoAttackRange(target))
                Items.UseItem(3142);
            if (sword && target != null && target.Type == ObjectManager.Player.Type &&
            !ObjectManager.Player.HasBuff("spectralfury", true) /*if ghostblade is not active*/
            && Orbwalking.InAutoAttackRange(target))
                Items.UseItem(3131);
            if (target != null && igniteSlot != SpellSlot.Unknown &&
            ObjectManager.Player.SummonerSpellbook.CanUseSpell(igniteSlot) == SpellState.Ready)
            {
                if (ObjectManager.Player.Distance(target) < 650 &&
                ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
                {
                    ObjectManager.Player.SummonerSpellbook.CastSpell(igniteSlot, target);
                }
            }
        }
        public static void LaneClear()
        {
            if(Config.Item("useEwc").GetValue<bool>() && E.IsReady())
            {
                var minions = MinionManager.GetMinions(myHero.Position, E.Range);
                foreach ( var data in minions )
                {
                    if (GetRendDamage(data) > data.Health)
                    {
                        E.Cast();
                    }
                }
            }
        }
        public static void drawConnection()
        {
            if (CoopStrikeAlly == null)
            {
                foreach (
                var ally in
                from ally in ObjectManager.Get<Obj_AI_Hero>().Where(tx => tx.IsAlly && !tx.IsDead && !tx.IsMe)
                where ObjectManager.Player.Distance(ally) <= CoopStrikeAllyRange
                from buff in ally.Buffs
                where buff.Name.Contains("kalistacoopstrikeally")
                select ally)
                {
                    CoopStrikeAlly = ally;
                }
            }
            if (CoopStrikeAlly == null)
            {
                Drawing.DrawText(Drawing.Width * 0.44f, Drawing.Height * 0.80f, Color.Red,
                "Searching Your Friend...");
            }
            else
            {
                var drawConnText = Config.Item("DrawConnText").GetValue<Circle>();
                if (drawConnText.Active)
                {
                    Drawing.DrawText(Drawing.Width * 0.44f, Drawing.Height * 0.80f, drawConnText.Color,
                    "You Connected with " + CoopStrikeAlly.ChampionName);
                }
                var drawConnSignal = Config.Item("DrawConnSignal").GetValue<bool>();
                if (drawConnSignal)
                {
                    if (ObjectManager.Player.Distance(CoopStrikeAlly) > 800 &&
                    ObjectManager.Player.Distance(CoopStrikeAlly) < CoopStrikeAllyRange)
                    {
                        Drawing.DrawText(Drawing.Width * 0.45f, Drawing.Height * 0.82f, Color.Gold,
                        "Connection Signal: Low");
                    }
                    else if (ObjectManager.Player.Distance(CoopStrikeAlly) < 800)
                    {
                        Drawing.DrawText(Drawing.Width * 0.45f, Drawing.Height * 0.82f, Color.GreenYellow,
                        "Connection Signal: Good");
                    }
                    else if (ObjectManager.Player.Distance(CoopStrikeAlly) > CoopStrikeAllyRange)
                    {
                        Drawing.DrawText(Drawing.Width * 0.45f, Drawing.Height * 0.82f, Color.Red,
                        "Connection Signal: None");
                    }
                }
            }
        }
        private static float GetRealDistance(GameObject target)
        {
            return ObjectManager.Player.Position.Distance(target.Position) + ObjectManager.Player.BoundingRadius +
            target.BoundingRadius;
        }
        public static void Combo()
        {
            if (myHero.HasBuff("Recall")) return;
            var useQ = Config.Item("UseQAC").GetValue<bool>();
            var useE = Config.Item("UseEAC").GetValue<bool>();
            if (useQ)
            {
                Cast_Q(SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical));
            }
            /*
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget(E.Range)))
            {
                if (GetRendDamage(enemy) > enemy.Health)
                {
                    E.Cast();
                    break;
                }
            }*/
            
            if (E.IsReady() && useE && Config.Item("E4K").GetValue<bool>())
            {
                if (ObjectManager.Get<Obj_AI_Hero>().Any(hero => hero.IsValidTarget(E.Range) && hero.Health < GetRendDamage(hero)))
                {
                    if(E.Cast())
                    {
                        Game.PrintChat("Target dead.");
                    }
                }
            }

            if (E.IsReady() && Config.Item("E4K").GetValue<bool>() == false && SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical).Buffs.FirstOrDefault(b => b.Name.ToLower() == "kalistaexpungemarker").Count >= Config.Item("minE").GetValue<Slider>().Value)
            {
                Game.PrintChat("fuck me");
                E.Cast(packetCast);
            }
        }
        internal static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "KalistaExpungeWrapper")
                    Utility.DelayAction.Add(250, Orbwalking.ResetAutoAttackTimer);
            }
        }
        public static void Harass()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
            float percentManaAfterQ = 100 * ((myHero.Mana - Q.Instance.ManaCost) / myHero.MaxMana);
            float percentManaAfterE = 100 * ((myHero.Mana - E.Instance.ManaCost) / myHero.MaxMana);
            int minPercentMana = Config.SubMenu("Harass").Item("manaPercent").GetValue<Slider>().Value;


            var first = Q.GetPrediction(target).CollisionObjects[0];
            var second = Q.GetPrediction(target).CollisionObjects[1];

            if (first == target || first.IsMinion && first.IsEnemy && first.Health < Q.GetDamage(first) && second == target && percentManaAfterQ >= minPercentMana)
            {
                Q.Cast(target,packetCast);
            }
            
            if (E.IsReady() && target.Buffs.FirstOrDefault(b => b.Name.ToLower() == "kalistaexpungemarker").Count >= Config.Item("stackE").GetValue<Slider>().Value && percentManaAfterE >= minPercentMana)// && getPerValue(true) >= ManaE)
            {
                E.Cast(packetCast);
            }

        }
        private static int getTotalAttacks(Obj_AI_Hero target, int stage)
        {
            double totalDamageToTarget = GetRendDamage(target);
            double targetHealth = target.Health;
            double skillQdamage = Q.GetDamage(target);
            double baseADDamage = myHero.GetAutoAttackDamage(target);
            double AANeeded = 0;
            if (stage == 1)
            {
                AANeeded = (targetHealth - skillQdamage - E.GetDamage(target)) / baseADDamage;
            }
            else
            {
                AANeeded = (targetHealth - E.GetDamage(target)) / baseADDamage;
            }

            return (int)AANeeded;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (drawings)
            {
                drawConnection();

                if (Config.Item("drawSpot").GetValue<bool>())
                {
                    foreach (Vector3 pos in wallhops)
                    {
                        if (myHero.Distance(pos) <= Config.Item("dh").GetValue<Slider>().Value)
                            Utility.DrawCircle(pos, minRange, Color.Green);
                    }
                }

                if (Config.Item("drawText").GetValue<bool>())
                {
                    var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
                    if (target != null && !target.IsDead && !myHero.IsDead)
                    {
                        var wts = Drawing.WorldToScreen(target.Position);

                        Drawing.DrawText(wts[0] - 40, wts[1] + 70, Color.OrangeRed, "Combo " + getTotalAttacks(target, 1) + " AA + Q + E");
                        Drawing.DrawText(wts[0] - 50, wts[1] + 80, Color.OrangeRed, "Combo " + getTotalAttacks(target, 2) + " AA + E");
                    }

                }
                //Config.Item("drawHp").ValueChanged += (object sender, OnValueChangeEventArgs e) => { Utility.HpBarDamageIndicator.Enabled = e.GetNewValue<bool>(); };
                if (Config.Item("drawHp").GetValue<bool>())
                {
                    //Utility.HpBarDamageIndicator.DamageToUnit = getDamageToTarget;
                    //Utility.HpBarDamageIndicator.Enabled = true;
                }

                var drawQ = Config.Item("QRange").GetValue<Circle>();
                if (drawQ.Active && !myHero.IsDead)
                {
                    Utility.DrawCircle(myHero.Position, Q.Range, drawQ.Color);
                }

                var drawW = Config.Item("WRange").GetValue<Circle>();
                if (drawW.Active && !myHero.IsDead)
                {
                    Utility.DrawCircle(myHero.Position, W.Range, drawW.Color);
                }
                var drawE = Config.Item("ERange").GetValue<Circle>();
                if (drawE.Active && !myHero.IsDead)
                {
                    Utility.DrawCircle(myHero.Position, E.Range, drawE.Color);
                }

                var drawR = Config.Item("RRange").GetValue<Circle>();
                if (drawR.Active && !myHero.IsDead)
                {
                    Utility.DrawCircle(myHero.Position, R.Range, drawR.Color);
                }
            }
        }
        /**
         * @credits Hellsing
         */
        public static double GetRendDamage(Obj_AI_Base target)
        {
            var buff = target.Buffs.FirstOrDefault(b => b.DisplayName.ToLower() == "kalistaexpungemarker");
            if (buff != null)
            {
                double damage = (10 + 10 * myHero.Spellbook.GetSpell(SpellSlot.E).Level) + 0.6 * myHero.FlatPhysicalDamageMod;
                damage += buff.Count * (new double[] { 0, 5, 9, 14, 20, 27 }[myHero.Spellbook.GetSpell(SpellSlot.E).Level] + (0.12 + 0.03 * myHero.Spellbook.GetSpell(SpellSlot.E).Level) * myHero.FlatPhysicalDamageMod);
                double dmg = myHero.CalcDamage(target, Damage.DamageType.Physical, damage);
                return dmg;
            }
            return 0;
        }
    }
}
