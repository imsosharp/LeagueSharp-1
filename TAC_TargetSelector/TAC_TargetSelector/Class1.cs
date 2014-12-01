using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Collections.Generic;
using Color = System.Drawing.Color;

namespace TAC_TargetSelector
{
    public class TS
    {
        private static Menu _Config;
        public static TargetingMode _Mode;
        public static Obj_AI_Hero Target;
        #region Targets

        private static readonly string[] ap =
        {
            "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana",
            "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus", "Kassadin", "Katarina", "Kayle", "Kennen",
            "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna", "Ryze", "Sion",
            "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra",
            "Velkoz"
        };
        private static readonly string[] sup =
        {
            "Blitzcrank", "Janna", "Karma", "Leona", "Lulu", "Nami", "Sona",
            "Soraka", "Thresh", "Zilean"
        };
        private static readonly string[] tank =
        {
            "Amumu", "Chogath", "DrMundo", "Galio", "Hecarim", "Malphite",
            "Maokai", "Nasus", "Rammus", "Sejuani", "Shen", "Singed", "Skarner", "Volibear", "Warwick", "Yorick", "Zac",
            "Nunu", "Taric", "Alistar", "Garen", "Nautilus", "Braum"
        };
        private static readonly string[] ad =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "KogMaw",
            "MissFortune", "Quinn", "Sivir", "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Zed", "Jinx",
            "Yasuo", "Lucian", "Kalista"
        };
        private static readonly string[] bruiser =
        {
            "Darius", "Elise", "Evelynn", "Fiora", "Gangplank", "Gnar", "Jayce",
            "Pantheon", "Irelia", "JarvanIV", "Jax", "Khazix", "LeeSin", "Nocturne", "Olaf", "Poppy", "Renekton",
            "Rengar", "Riven", "Shyvana", "Trundle", "Tryndamere", "Udyr", "Vi", "MonkeyKing", "XinZhao", "Aatrox",
            "Rumble", "Shaco", "MasterYi"
        };
        #endregion
        public enum TargetingMode
        {
            LowHP,
            MostAD,
            MostAP,
            Closest,
            NearMouse,
            AutoPriority,
            LessAttack,
            LessCast,
        }
        static TS()
        {
            Drawing.OnDraw += onDraw;
            Game.OnWndProc += selectTarget;
        }
        private static void onDraw(EventArgs args)
        {
            if (Target.IsValidTarget() && _Config != null && _Config.Item("FocusSelected").GetValue<bool>() &&
               _Config.Item("SelTColor").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(Target.Position, 150, _Config.Item("SelTColor").GetValue<Circle>().Color, 7, true);
            }
        }
        public static Obj_AI_Hero getTarget(float range = 600)
        {
            Obj_AI_Hero newtarget = null;
            if (Target != null)
            {
                Game.PrintChat("Target already selected! " + Target);
                newtarget = Target;
                return newtarget;
            }
            if (_Mode != TargetingMode.AutoPriority)
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget() && ObjectManager.Player.Distance(target) <= range))
                {
                    switch (_Mode)
                    {
                        case TargetingMode.LowHP:
                            if (target.Health < newtarget.Health) newtarget = target;
                            break;
                        case TargetingMode.MostAD:
                            if (target.BaseAttackDamage + target.FlatPhysicalDamageMod < newtarget.BaseAttackDamage + newtarget.FlatPhysicalDamageMod)
                                newtarget = target;
                            break;
                        case TargetingMode.MostAP:
                            if (target.FlatMagicDamageMod < newtarget.FlatMagicDamageMod)
                                newtarget = target;
                            break;
                        case TargetingMode.Closest:
                            if (ObjectManager.Player.Distance(target) < ObjectManager.Player.Distance(newtarget))
                                newtarget = target;
                            break;
                        case TargetingMode.NearMouse:
                            if (SharpDX.Vector2.Distance(Game.CursorPos.To2D(), target.Position.To2D()) + 50 <
                            SharpDX.Vector2.Distance(Game.CursorPos.To2D(), newtarget.Position.To2D()))
                                newtarget = target;
                            break;
                        case TargetingMode.LessAttack:
                            if ((target.Health -
                            ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, target.Health) <
                            (newtarget.Health -
                            ObjectManager.Player.CalcDamage(
                            newtarget, Damage.DamageType.Physical, newtarget.Health))))
                                newtarget = target;
                            break;
                        case TargetingMode.LessCast:
                            if ((target.Health -
                            ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, target.Health) <
                            (newtarget.Health -
                            ObjectManager.Player.CalcDamage(
                            newtarget, Damage.DamageType.Magical, newtarget.Health))))
                                newtarget = target;
                            break;
                    }
                }
            }
            else
            {
                int prio = 5;

                foreach (var target in
                ObjectManager.Get<Obj_AI_Hero>()
                .Where(target => target != null && target.IsValidTarget() && Geometry.Distance(target) <= range))
                {
                    var priority = FindPrioForTarget(target.ChampionName);
                    if (newtarget == null)
                    {
                        newtarget = target;
                        prio = priority;
                    }
                    else
                    {
                        if (priority < prio)
                        {
                            newtarget = target;
                            prio = FindPrioForTarget(target.ChampionName);
                        }
                        else if (priority == prio)
                        {
                            if (!(target.Health < newtarget.Health))
                            {
                                continue;
                            }
                            newtarget = target;
                            prio = priority;
                        }
                    }
                }
            }
            Target = newtarget;
            return newtarget;
        }
        private static int FindPrioForTarget(string ChampionName)
        {
            return ap.Contains(ChampionName) ? 2 : (ad.Contains(ChampionName) ? 1 : (sup.Contains(ChampionName) ? 4 : (bruiser.Contains(ChampionName) ? 3 : 5)));
        }
        public static void createMenu(Menu Config)
        {
            _Config = Config;
            Config.AddSubMenu(new Menu("TAC TargetSelector","tac_targetSelector"));
            Config.SubMenu("tac_targetSelector").AddSubMenu(new Menu("Targets","targets"));

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy))
            {
                Config.SubMenu("tac_targetSelector").SubMenu("targets").AddItem(new MenuItem("priority" + enemy.ChampionName, enemy.ChampionName).SetShared().SetValue(new Slider(FindPrioForTarget(enemy.ChampionName), 5, 1)));
            }
            Config.SubMenu("tac_targetSelector").SubMenu("targets").AddItem(new MenuItem("autoArrange","Auto arrange").SetValue(false));
            Config.SubMenu("tac_targetSelector").SubMenu("targets").Item("autoArrange").ValueChanged += prioritizeChampion;


            Config.SubMenu("tac_targetSelector").AddItem(new MenuItem("currentMode", "Targetting mode:").SetValue(new StringList(Enum.GetNames(typeof(TargetingMode)), 2)));
            Config.SubMenu("tac_targetSelector").Item("currentMode").ValueChanged += currentMode;

            Config.SubMenu("tac_targetSelector").AddItem(new MenuItem("FocusSelected", "Focus selected target").SetShared().SetValue(true));
            Config.SubMenu("tac_targetSelector").AddItem(
                new MenuItem("SelTColor", "Selected target color").SetShared()
                    .SetValue(new Circle(true, System.Drawing.Color.Red)));
        }
        public static TargetingMode getCurrentMode()
        {
            return _Mode;
        }
        private static void currentMode(object sender, OnValueChangeEventArgs e)
        {
            switch(e.GetNewValue<StringList>().SelectedIndex)
            {
                case 0:
                default:
                    _Mode = TargetingMode.LowHP;
                    break;
                case 1:
                    _Mode = TargetingMode.MostAD;
                    break;
                case 2:
                    _Mode = TargetingMode.MostAP;
                    break;
                case 3:
                    _Mode = TargetingMode.Closest;
                    break;
                case 4:
                    _Mode = TargetingMode.NearMouse;
                    break;
                case 5:
                    _Mode = TargetingMode.AutoPriority;
                    break;
                case 6:
                    _Mode = TargetingMode.LessAttack;
                    break;
                case 7:
                    _Mode = TargetingMode.LessCast;
                    break;
            }
        }
        private static void prioritizeChampion(object sender, OnValueChangeEventArgs e)
        {
            if (!e.GetNewValue<bool>()) return;
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Team != ObjectManager.Player.Team))
            {
                _Config.Item("priority" + enemy.ChampionName).SetValue(new Slider(FindPrioForTarget(enemy.ChampionName), 5, 1));
            }
        }
        private static void selectTarget(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN) return;
            Target = null;
            foreach (var enemy in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero.IsValidTarget())
                    .OrderByDescending(h => h.Distance(Game.CursorPos))
                    .Where(enemy => enemy.Distance(Game.CursorPos) < 200))
            {
                Target = enemy;
            }
        }
    }
}