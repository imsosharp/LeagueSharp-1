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
        private static Obj_AI_Hero myHero = ObjectManager.Player;
        public static Orbwalking.Orbwalker Orbwalker;
        private static Spell Q = new Spell(SpellSlot.Q, 1450);
        private static Spell W = new Spell(SpellSlot.W, 5500);
        private static Spell E = new Spell(SpellSlot.E, 1200);
        private static Spell R = new Spell(SpellSlot.R, 1200);
        public static int minRange = 100;

        public static Obj_AI_Hero CoopStrikeAlly;
        public static float CoopStrikeAllyRange = 1250f;

        public static bool packetCast = true;
        public static bool debug = true;
        public static bool LaneClearActive;
        public static bool ComboActive;
        public static bool HarassActive;
        public static bool drawings;
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

            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);


            var orbwalking = Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalking);
            
            Config.AddSubMenu(new Menu("AutoCarry options", "ac"));
            Config.SubMenu("ac").AddItem(new MenuItem("UseQAC", "Use Q").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("UseEAC", "Use E").SetValue(true));
            
            Config.AddSubMenu(new Menu("Harass options", "harass"));
            Config.SubMenu("harass").AddItem(new MenuItem("stackE", "E stacks to cast").SetValue(new Slider(1, 1, 10)));
            Config.SubMenu("harass").AddItem(new MenuItem("manaPercent", "Mana %").SetValue(new Slider(40, 1, 100)));


            Config.AddSubMenu(new Menu("Smite options", "smite"));
            Config.SubMenu("smite").AddItem(new MenuItem("1", "Cause bugsplat"));
            Config.SubMenu("smite").AddItem(new MenuItem("2", "not working yet"));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Baron", "Baron Enabled").SetValue(true));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Dragon", "Dragon Enabled").SetValue(true));
            Config.SubMenu("smite").AddItem(new MenuItem("smite", "Auto-Smite enabled").SetValue(true));


            Config.AddSubMenu(new Menu("Wall Hop options", "wh"));
            Config.SubMenu("wh").AddItem(new MenuItem("drawSpot", "Draw WallHop spots").SetValue(true));
            Config.SubMenu("wh").AddItem(new MenuItem("dh", "Draw range").SetValue(new Slider(1000,200,10000)));
            
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
        public static float getPerValue(bool mana)
        {
            if (mana) return (myHero.Mana / myHero.MaxMana) * 100;
            return (myHero.Health / myHero.MaxHealth) * 100;
        }
        public static int KalistaMarkerCountMinion
        {
            get
            {
                var xbuffCount = 0;
                foreach (
                    var buff in from enemy in ObjectManager.Get<Obj_AI_Base>().Where(tx => tx.IsEnemy && !tx.IsDead)
                                where ObjectManager.Player.Distance(enemy) < E.Range
                                from buff in enemy.Buffs
                                where buff.Name.Contains("kalistaexpungemarker")
                                select buff)
                {
                    xbuffCount = buff.Count;
                }
                return xbuffCount;
            }
        }

        public static void OnGameUpdate(EventArgs args)
        {
            
            drawings = Config.Item("enableDrawings").GetValue<bool>();
            debug = Config.Item("debug").GetValue<bool>();
            packetCast = Config.Item("Packets").GetValue<bool>();
//            if (myHero.IsDead) return;
//            if (Config.Item("whk").GetValue<bool>()) WallHop();


            ComboActive = Config.Item("Orbwalk").GetValue<KeyBind>().Active;
            HarassActive = Config.Item("Farm").GetValue<KeyBind>().Active;
            LaneClearActive = Config.Item("LaneClear").GetValue<KeyBind>().Active;

            if (ComboActive)
            {
                Combo();
            }
            if (HarassActive)
            {
                Harass();
            }
            drawConnection();
            if(Config.Item("showPos").GetValue<KeyBind>().Active)
            {
                Game.PrintChat("Positino on server: "+myHero.ServerPosition);
            } 

            if (Config.Item("smite").GetValue<bool>())
            {
                Obj_AI_Base mob = GetNearest(myHero.ServerPosition);
                if (mob != null && Config.Item(mob.SkinName).GetValue<bool>())
                {
                    if (mob.Health < getDamageToMinion(mob))
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
        public static int KalistaMarkerCount
        {
            get
            {
                var xbuffCount = 0;
                foreach (
                    var buff in from enemy in ObjectManager.Get<Obj_AI_Hero>().Where(tx => tx.IsEnemy && !tx.IsDead)
                                where ObjectManager.Player.Distance(enemy) < E.Range
                                from buff in enemy.Buffs
                                where buff.Name.Contains("kalistaexpungemarker")
                                select buff)
                {
                    xbuffCount = buff.Count;
                }
                return xbuffCount;
            }
        }
        public static void Combo()
        {
            if (myHero.HasBuff("Recall")) return;
            //var ManaQ = Config.Item("QManaMinAC").GetValue<Slider>().Value;
            //var ManaE = Config.Item("EManaMinAC").GetValue<Slider>().Value;
            var useQ = Config.Item("UseQAC").GetValue<bool>();
            var useE = Config.Item("UseEAC").GetValue<bool>();

            Obj_AI_Hero target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (Orbwalking.CanMove(100) && target != null)
            {
                if (Q.IsReady() && useQ)
                {
                    Q.Cast(target, packetCast);
                }
                if (E.IsReady() && useE)
                {
                    if (target.Health < getDamageToTarget(target))
                    {
                        Game.PrintChat("Casting E");
                        E.Cast();
                    }
                }
            }
            else
            {
                Game.PrintChat("Target not found!");
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
            float percentManaAfterE = 100 * ((myHero.Mana - E.Instance.ManaCost) / myHero.MaxMana);
            int minPercentMana = Config.SubMenu("Harass").Item("manaPercent").GetValue<Slider>().Value;
            if (E.IsReady() && KalistaMarkerCount >= Config.Item("stackE").GetValue<Slider>().Value && percentManaAfterE >= minPercentMana)// && getPerValue(true) >= ManaE)
            {
                E.Cast(target,packetCast);
            }

        }
        public static float getDamageToTarget(Obj_AI_Hero target)
        {
            int levelSkill = E.Level;
            var stacks = KalistaMarkerCount;
            double AD = myHero.FlatPhysicalDamageMod;
            double baseDamagePerStack = new double[] { 5, 9, 14, 20, 27 }[levelSkill];
            double scalingDamagePerStack = new double[] { 0.15, 0.18, 0.21, 0.24, 0.27 }[levelSkill];
            double baseDamage = new double[] { 20, 30, 40, 50, 60 }[levelSkill];
            double totalDamageToTarget = (baseDamage + baseDamagePerStack + scalingDamagePerStack * AD) * target.Buffs.FirstOrDefault(b => b.DisplayName.ToLower() == "kalistaexpungemarker").Count; //stacks
            return (float)totalDamageToTarget;
        }
        public static int getDamageToMinion(Obj_AI_Base target)
        {
            int levelSkill = E.Level;
            var stacks = KalistaMarkerCountMinion;
            double AD = myHero.FlatPhysicalDamageMod;
            double baseDamagePerStack = new double[] { 5, 9, 14, 20, 27 }[levelSkill];
            double scalingDamagePerStack = new double[] { 0.15, 0.18, 0.21, 0.24, 0.27 }[levelSkill];
            double baseDamage = new double[] { 20, 30, 40, 50, 60 }[levelSkill];
            double totalDamageToTarget = (baseDamage + baseDamagePerStack + scalingDamagePerStack * AD) * target.Buffs.FirstOrDefault(b => b.DisplayName.ToLower() == "kalistaexpungemarker").Count; //stacks
            return (int)totalDamageToTarget;
        }
        public static int simulateDamage(int stacks)
        {
            int levelSkill = E.Level;
            double AD = myHero.FlatPhysicalDamageMod;
            double baseDamagePerStack = new double[] { 5, 9, 14, 20, 27 }[levelSkill];
            double scalingDamagePerStack = new double[] { 0.15, 0.18, 0.21, 0.24, 0.27 }[levelSkill];
            double baseDamage = new double[] { 20, 30, 40, 50, 60 }[levelSkill];
            double totalDamageToTarget = baseDamage + (baseDamagePerStack + scalingDamagePerStack * AD) * stacks;

            //            return (int)myHero.GetSpellDamage(target, SpellSlot.E,1) * KalistaMarkerCount;
            return (int)totalDamageToTarget;
        }

        private static int getTotalAttacks(Obj_AI_Hero target, int stage)
        {
            float totalDamageToTarget = getDamageToTarget(target);
            float targetHealth = target.Health;
            float skillQdamage = Q.GetDamage(target);
            float baseADDamage = (float)myHero.GetAutoAttackDamage(target);
            float AANeeded = 0;
            if (stage == 1)
            {
                AANeeded = (targetHealth - skillQdamage - E.GetDamage(target)) / baseADDamage;
            }
            else
            {
                AANeeded = (targetHealth - E.GetDamage(target)) / baseADDamage;
            }

//            int sex = ((int)targetHealth - (int)skillQdamage) - simulateDamage((int)AANeeded);

            return (int)AANeeded;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if(Config.Item("drawSpot").GetValue<bool>())
            {
                foreach (Vector3 pos in wallhops)
                {
                    if(myHero.Distance(pos) <= Config.Item("dh").GetValue<Slider>().Value)
                        Utility.DrawCircle(pos, minRange, Color.Green);
                }
            }

            if(Config.Item("drawText").GetValue<bool>())
            {
                var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
                if (target != null && !target.IsDead && !myHero.IsDead)
                {
                    var wts = Drawing.WorldToScreen(target.Position);

                    Drawing.DrawText(wts[0] - 40, wts[1] + 70, Color.OrangeRed, "Combo " + getTotalAttacks(target, 1) + " AA + Q + E");
                    Drawing.DrawText(wts[0] - 50, wts[1] + 80, Color.OrangeRed, "Combo " + getTotalAttacks(target, 2) + " AA + E");
                }

            }
            Config.Item("drawHp").ValueChanged += (object sender, OnValueChangeEventArgs e) => { Utility.HpBarDamageIndicator.Enabled = e.GetNewValue<bool>(); };
            if (Config.Item("drawHp").GetValue<bool>())
            {
                Utility.HpBarDamageIndicator.DamageToUnit = getDamageToTarget;
                Utility.HpBarDamageIndicator.Enabled = true;
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

        public static float CalculateRendDamage(Obj_AI_Hero eTarget)
        {
            if (E.IsReady())
            {
                if (eTarget.IsValidTarget(E.Range))
                {
                    foreach (var buff in eTarget.Buffs.Where(buff => buff.DisplayName.ToLower() == "kalistaexpungemarker").Where(buff => buff.Count == 6))
                    {
                        if(debug)
                            Game.PrintChat("Total stacks on target " + eTarget.ChampionName + " Count: " + buff.Count + " Total Damage: " + eTarget.GetSpellDamage(myHero,SpellSlot.E)*buff.Count);
                        return (float)eTarget.GetSpellDamage(myHero, SpellSlot.E) * buff.Count;
//                        E.Cast();
                    }
                }

            }
            return (float)0;
        }
    }
}
