using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
namespace TAC_Kalista
{
    class DrawingHandler
    {
        public static Device DxDevice = Drawing.Direct3DDevice;
        public static Line DxLine;
        public static Obj_AI_Hero Unit { get; set; }
        public static float Width = 104;
        public static float Hight = 9;
        public static int MinRange = 100;

        public static void Init()
        {
            DxLine = new Line(DxDevice) { Width = 9 };
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }
        public static void OnDraw(EventArgs args)
        {
            if(Kalista.Drawings)
            {

                if (MenuHandler.Config.Item("JumpTo").GetValue<KeyBind>().Active)
                {
                    GetAvailableJumpSpots();
                }
                Spell[] spellList = { SkillHandler.Q, SkillHandler.W, SkillHandler.E, SkillHandler.R };

                foreach (var spell in spellList)
                {
                    var menuItem = MenuHandler.Config.Item(spell.Slot+"Range").GetValue<Circle>();
                    if (menuItem.Active && spell.Level > 0)
                        Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
                }
                bool drawHp = MenuHandler.Config.Item("drawHp").GetValue<bool>();
                bool drawStacks = MenuHandler.Config.Item("drawStacks").GetValue<bool>();
                if (drawHp || drawStacks)
                {
                    var stacks = 0;
                    foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                    {
                        if (drawHp)
                        {
                            Unit = enemy;
                            DrawDmg(MathHandler.GetDamageToTarget(enemy), enemy.Health < MathHandler.GetRealDamage(enemy) ? Color.Red : Color.Yellow);
                        }
                        if (drawStacks)
                        {
                            var firstOrDefault = enemy.Buffs.FirstOrDefault(b => b.Name.ToLower() == "kalistaexpungemarker");
                            if (firstOrDefault != null)
                                stacks = firstOrDefault.Count;
                            if (stacks > 0)
                            {
                                Drawing.DrawText(enemy.HPBarPosition.X, enemy.HPBarPosition.Y - 5, Color.Red, "E:" + stacks + "H:" + (int)enemy.Health + "/D:" + (int)MathHandler.GetRealDamage(enemy), enemy);
                            }
                        }
                    }
                }
                if (MenuHandler.Config.Item("drawESlow").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, SkillHandler.E.Range - 110, Color.Pink);
                }
            }
        }
        /**
         * @author detuks
         * */
        public static void OnEndScene(EventArgs args)
        {
            if (MenuHandler.Config.Item("drawHp").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    Unit = enemy;
                    DrawDmg(MathHandler.GetDamageToTarget(enemy), enemy.Health < MathHandler.GetRealDamage(enemy) ? Color.Red : Color.Yellow);
                }
            }
        }

        private static Vector2 Offset
        {
            get
            {
                if (Unit != null)
                {
                    return Unit.IsAlly ? new Vector2(34, 9) : new Vector2(10, 20);
                }

                return new Vector2();
            }
        }

        public static Vector2 StartPosition
        {
            get { return new Vector2(Unit.HPBarPosition.X + Offset.X, Unit.HPBarPosition.Y + Offset.Y); }
        }


        private static float GetHpProc(float dmg = 0)
        {
            var health = ((Unit.Health - dmg) > 0) ? (Unit.Health - dmg) : 0;
            return (health / Unit.MaxHealth);
        }

        private static Vector2 GetHpPosAfterDmg(float dmg)
        {
            var w = GetHpProc(dmg) * Width;
            return new Vector2(StartPosition.X + w, StartPosition.Y);
        }

        public static void DrawDmg(float dmg, Color color)
        {
            var hpPosNow = GetHpPosAfterDmg(0);
            var hpPosAfter = GetHpPosAfterDmg(dmg);

            fillHPBar(hpPosNow, hpPosAfter, color);
        }

        public static ColorBGRA ColorBgra(Color c)
        {
            return ColorBGRA.FromRgba(c.ToArgb());
		}
        private static void fillHPBar(Vector2 from, Vector2 to, Color c)
        {
            DxLine.Begin();

            DxLine.Draw(new[]
                                    {
                                        new Vector2((int)from.X, (int)from.Y + 4f),
                                        new Vector2( (int)to.X, (int)to.Y + 4f)
                                    },ColorBgra(c));
            DxLine.End();
        }

        public static bool IsLyingInCone(Vector2 position, Vector2 apexPoint, Vector2 circleCenter, float aperture)
        {
            var halfAperture = aperture / 2.0f;
            var apexToXVect = apexPoint - position;
            var axisVect = apexPoint - circleCenter;
            var isInInfiniteCone = DotProd(apexToXVect, axisVect) / Magn(apexToXVect) / Magn(axisVect) > Math.Cos(halfAperture);
            if (!isInInfiniteCone) return false;
            return DotProd(apexToXVect, axisVect) / Magn(axisVect) < Magn(axisVect);
        }

        public static float DotProd(Vector2 a, Vector2 b){ return a.X * b.X + a.Y * b.Y; }
        public static float Magn(Vector2 a){ return (float)(Math.Sqrt(a.X * a.X + a.Y * a.Y)); }



        public static void GetAvailableJumpSpots()
        {
            const int size = 295;
            const int n = 15;

            if (!SkillHandler.Q.IsReady())
            {
                Drawing.DrawText(Drawing.Width * 0.44f, Drawing.Height * 0.80f, Color.Red,
                "Jumping mode active, but Q isn't.");
            }
            else
            {
                Drawing.DrawText(Drawing.Width * 0.39f, Drawing.Height * 0.80f, Color.White,
                    "Hover the spot to jump to ");
            }
            var playerPosition = ObjectManager.Player.Position;
            Drawing.DrawCircle(ObjectManager.Player.Position, size, Color.RoyalBlue);
            var target = SimpleTs.GetTarget(SkillHandler.Q.Range, SimpleTs.DamageType.Physical);
            for (var i = 1; i <= n; i++)
            {
                var x = size * Math.Cos(2 * Math.PI * i / n);
                var y = size * Math.Sin(2 * Math.PI * i / n);
                var drawWhere = new Vector3((int)(playerPosition.X + x), (float)(playerPosition.Y + y), playerPosition.Z);
                if (drawWhere.IsWall()) continue;
                if (SkillHandler.Q.IsReady() && Game.CursorPos.Distance(drawWhere) <= 80f)
                {
                    if (target != null)
                        FightHandler.CustomQCast(target);
                    else
                        SkillHandler.Q.Cast(new Vector2(drawWhere.X, drawWhere.Y), true);

                    Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(drawWhere.X, drawWhere.Y)).Send();
                    return;
                }
                Utility.DrawCircle(drawWhere, 20, Color.Red);
            }

        }
    }
}
