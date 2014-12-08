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

            var orbwalkerMenu = new Menu("LX Orbwalker", "my_Orbwalker");
            LXOrbwalker.AddToMenu(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);
            /*
            Menu orbwalker = new Menu("Orbwalker", "orbwalker");
            Jax.orb = new Orbwalking.Orbwalker(orbwalker);
            Config.AddSubMenu(orbwalker);*/

            Config.AddSubMenu(new Menu("Auto Carry", "ac"));
            Config.SubMenu("ac").AddSubMenu(new Menu("Use Q", "q_menu"));
            Config.SubMenu("ac").SubMenu("q_menu").AddItem(new MenuItem("acQ_useIfWorth", "Use F+Q if worth").SetValue(true));
            Config.SubMenu("ac").SubMenu("q_menu").AddItem(new MenuItem("acQ_useIfWorthEnemy", "Maximum enemies in range: ").SetValue(new Slider(2,1,5)));
            Config.SubMenu("ac").SubMenu("q_menu").AddItem(new MenuItem("acQ", "Enabled").SetValue(true));

            // todo fix this shit
            Config.SubMenu("ac").AddSubMenu(new Menu("Use W", "w_menu"));
            Config.SubMenu("ac").SubMenu("w_menu").AddItem(new MenuItem("acW_mode", "W mode").SetValue(new StringList(new[] { "AA Reset", "Helicopter", "Smart" }, 2)));
            Config.SubMenu("ac").SubMenu("w_menu").AddItem(new MenuItem("acW", "Enabled").SetValue(true));

            Config.SubMenu("ac").AddItem(new MenuItem("acE", "Smart E").SetValue(true));
            Config.SubMenu("ac").AddItem(new MenuItem("acR", "Smart R").SetValue(true));


            Config.AddSubMenu(new Menu("Advanced", "advanced"));

            Config.SubMenu("advanced").AddSubMenu(new Menu("Smart E", "e_menu"));
            Config.SubMenu("advanced").SubMenu("e_menu").AddItem(new MenuItem("gapclose_E", "Prevent gap-closing").SetValue(true));
            Config.SubMenu("advanced").SubMenu("e_menu").AddItem(new MenuItem("gapcloseRange_E", "Gap-close range").SetValue(new Slider(250,200,400)));

            Config.SubMenu("advanced").AddItem(new MenuItem("packetCast", "Packet Casting").SetValue(true));
            Config.SubMenu("advanced").AddItem(new MenuItem("debug", "Debugging").SetValue(true));

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("rangeQ", "Q range").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("rangeE", "E range").SetValue(new Circle(true, Color.FromArgb(100, Color.BlueViolet))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawHp", "Draw combo damage").SetValue(true));
            Config.AddToMainMenu();
        }
    }
}
