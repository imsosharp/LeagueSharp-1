using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace TAC_Kalista
{
    class Kalista
    {
        public static bool PacketCast;
        public static bool Debug;
        public static bool Drawings;
        public static bool Canexport = true;
        static void Main(string[] args)
        {
            Game.PrintChat("---------------------------");
            Game.PrintChat("[<font color='#FF0000'>v4.1</font>]<font color='#7A6EFF'>Twilight's Auto Carry:</font> <font color='#86E5E1'>Kalista</font>");
            CustomEvents.Game.OnGameLoad += Load;
        }
        public static void Load(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Kalista") return;
            SkillHandler.Init();
            ItemHandler.Init();
            MenuHandler.Init();
            DrawingHandler.Init();
            Game.OnGameUpdate += OnGameUpdateModes;
            Game.OnGameSendPacket += Game_OnGameSendPacket;
            AntiGapcloser.OnEnemyGapcloser += FightHandler.AntiGapCloser;
            Obj_AI_Base.OnProcessSpellCast += FightHandler.OnProcessSpellCast;
        }
        #region Hellsing
        static void Game_OnGameSendPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] == Packet.C2S.Cast.Header && ObjectManager.Player.IsDashing() && Packet.C2S.Cast.Decoded(args.PacketData).Slot == SpellSlot.Q)
            {
                args.Process = false;
            }
        }
        #endregion
        public static void OnGameUpdateModes(EventArgs args)
        {
            Drawings = MenuHandler.Config.Item("enableDrawings").GetValue<bool>();
            Debug = MenuHandler.Config.Item("debug").GetValue<bool>();
            PacketCast = MenuHandler.Config.Item("Packets").GetValue<bool>();
            if (ObjectManager.Player.HasBuff("Recall") || ObjectManager.Player.IsDead) return;
            switch (MenuHandler.Orb.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    FightHandler.OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    FightHandler.OnHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    FightHandler.OnLaneClear();
                    break;
            }
            if (MenuHandler.Config.Item("saveSould").GetValue<bool>()) FightHandler.SaveSould();
            SmiteHandler.Init();

        }
    }
}
