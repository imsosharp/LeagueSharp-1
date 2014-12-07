using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
namespace TAC_Jax
{
    class MenuHandler
    {
        internal static Menu Config;
        internal static void load()
        {
            Config = new Menu("TAC Jax", "TAC_Jax", true);
            Menu targetSelector = new Menu("Target selector", "ts");
            SimpleTs.AddToMenu(targetSelector);
            Config.AddSubMenu(targetSelector);

            Menu orbwalker = new Menu("Orbwalker", "orbwalker");
            Jax.orb = new Orbwalking.Orbwalker(orbwalker);
            Config.AddSubMenu(orbwalker);

            Config.AddSubMenu(new Menu("Auto Carry", "ac"));
            Config.SubMenu("ac").AddItem(new MenuItem("acQ", "Use Q").SetValue(true));
            Config.SubMenu("ac").AddSubMenu(new Menu("Use W", "w_menu"));
            Config.SubMenu("ac").SubMenu("w_menu").AddItem(new MenuItem("acW_mode", "W mode").SetValue(new StringList(new[] {"AA Reset","Helicopter","Smart"}, 3)));
            Config.SubMenu("ac").SubMenu("w_menu").AddItem(new MenuItem("acW", "Enabled").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("acE", "Smart E").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("acR", "Smart R").SetValue(true));

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("rangeq", "Q range").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("rangee", "E range").SetValue(new Circle(true, Color.FromArgb(100, Color.BlueViolet))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawHp", "Draw combo damage").SetValue(true));
        }
    }
}
