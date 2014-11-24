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

namespace TAC_Kalista
{
    class EnemyMarker
    {
        public string ChampionName { get; set; }
        public double ExpireTime { get; set; }
        public int BuffCount { get; set; }
    }
    class DrawingHandler
    {
        public static SharpDX.Direct3D9.Device dxDevice = Drawing.Direct3DDevice;
        public static SharpDX.Direct3D9.Line dxLine;
        public static Obj_AI_Hero unit { get; set; }
        public static float width = 104;
        public static float hight = 9;
        public static readonly Vector3[] wallhops = new[] { new Vector3(794, 5914, 50), new Vector3(792, 6208, -71), new Vector3(10906, 7498, 52), new Vector3(10872, 7208, 51), new Vector3(11900, 4870, 51), new Vector3(11684, 4694, -71), new Vector3(12046, 5376, 54), new Vector3(12284, 5382, 51), new Vector3(11598, 8676, 62), new Vector3(11776, 8890, 50), new Vector3(8646, 9584, 50), new Vector3(8822, 9406, 51), new Vector3(6606, 11756, 53), new Vector3(6494, 12056, 56), new Vector3(5164, 12090, 56), new Vector3(5146, 11754, 56), new Vector3(5780, 10650, 55), new Vector3(5480, 10620, -71), new Vector3(3174, 9856, 52), new Vector3(3398, 10080, -65), new Vector3(2858, 9448, 51), new Vector3(2542, 9466, 52), new Vector3(3700, 7416, 51), new Vector3(3702, 7702, 52), new Vector3(3224, 6308, 52), new Vector3(3024, 6312, 57), new Vector3(4724, 5608, 50), new Vector3(4610, 5868, 51), new Vector3(6124, 5308, 48), new Vector3(6010, 5522, 51), new Vector3(9322, 4514, -71), new Vector3(9022, 4508, 52), new Vector3(6826, 8628, -71), new Vector3(7046, 8750, 52), };
        public static int minRange = 100;
        public static Dictionary<Vector3, Vector3> jumpPos;
        public static readonly List<EnemyMarker> xEnemyMarker = new List<EnemyMarker>();

        public static void init()
        {
            dxLine = new Line(dxDevice) { Width = 9 };
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }
        public static void OnDraw(EventArgs args)
        {
            if(Kalista.drawings)
            {
                Spell[] spellList = { SkillHandler.Q, SkillHandler.W, SkillHandler.E, SkillHandler.R };

                foreach (var spell in spellList)
                {
                    var menuItem = MenuHandler.Config.Item(spell.Slot+"Range").GetValue<Circle>();
                    if (menuItem.Active && spell.Level > 0)
                        Utility.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
                }
                if(MenuHandler.Config.Item("drawESlow").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, SkillHandler.E.Range - 100, Color.Pink);
                }
                bool drawHp = MenuHandler.Config.Item("drawHp").GetValue<bool>();
                bool drawStacks = MenuHandler.Config.Item("drawStacks").GetValue<bool>();
                if (drawHp || drawStacks)
                {
                    int stacks;
                    foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                    {
                        if (drawHp)
                        {
                            unit = enemy;
                            drawDmg(MathHandler.getDamageToTarget(enemy), enemy.Health < ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.E) ? Color.Red : Color.Yellow);
                        }
                        if (drawStacks)
                        {
                            stacks = enemy.Buffs.FirstOrDefault(b => b.Name.ToLower() == "kalistaexpungemarker").Count;
                            if (stacks > 0)
                            {
                                Drawing.DrawText(enemy.HPBarPosition.X, enemy.HPBarPosition.Y-5, Color.Red, "E:" + stacks + "H:"+(int)enemy.Health+"/D:"+(int)ObjectManager.Player.GetSpellDamage(enemy,SpellSlot.E),enemy);
                            }
                        }
                    }
                }
                var drawJumpPos = MenuHandler.Config.Item("DrawJumpPos").GetValue<Circle>();
                if (drawJumpPos.Active)
                {
                    var drawJumpPosRange = MenuHandler.Config.Item("DrawJumpPosRange").GetValue<StringList>().SelectedIndex;
                    var drawWhom = drawJumpPosRange == 1 ? SkillHandler.Q.Range : (drawJumpPosRange == 2 ? SkillHandler.E.Range : (drawJumpPosRange == 3 ? SkillHandler.R.Range : Orbwalking.GetRealAutoAttackRange(ObjectManager.Player)));
                    foreach (KeyValuePair<Vector3, Vector3> pos in jumpPos)
                    {
                        if (ObjectManager.Player.Distance(pos.Key) <= drawWhom)
                        {
                            Utility.DrawCircle(pos.Key, 20f, Color.GreenYellow);
                            Utility.DrawCircle(pos.Value, 20f, Color.GreenYellow);
                        }
                    }
                }
            }
        }
        public static void OnEndScene(EventArgs args)
        {
            if (MenuHandler.Config.Item("drawHp").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    unit = enemy;
                    drawDmg(MathHandler.getDamageToTarget(enemy), enemy.Health < ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.E) ? Color.Red : Color.Yellow);
                }
            }
        }

        private static Vector2 Offset
        {
            get
            {
                if (unit != null)
                {
                    return unit.IsAlly ? new Vector2(34, 9) : new Vector2(10, 20);
                }

                return new Vector2();
            }
        }

        public static Vector2 startPosition
        {

            get { return new Vector2(unit.HPBarPosition.X + Offset.X, unit.HPBarPosition.Y + Offset.Y); }
        }


        private static float getHpProc(float dmg = 0)
        {
            float health = ((unit.Health - dmg) > 0) ? (unit.Health - dmg) : 0;
            return (health / unit.MaxHealth);
        }

        private static Vector2 getHpPosAfterDmg(float dmg)
        {
            float w = getHpProc(dmg) * width;
            return new Vector2(startPosition.X + w, startPosition.Y);
        }

        public static void drawDmg(float dmg, System.Drawing.Color color)
        {
            var hpPosNow = getHpPosAfterDmg(0);
            var hpPosAfter = getHpPosAfterDmg(dmg);

            fillHPBar(hpPosNow, hpPosAfter, color);
        }

        private static void fillHPBar(int to, int from, System.Drawing.Color color)
        {
            Vector2 sPos = startPosition;

            for (int i = from; i < to; i++)
            {
                Drawing.DrawLine(sPos.X + i, sPos.Y, sPos.X + i, sPos.Y + 9, 1, color);
            }
        }

        private static void fillHPBar(Vector2 from, Vector2 to, System.Drawing.Color color)
        {
            dxLine.Begin();

            dxLine.Draw(new[]
                                    {
                                        new Vector2((int)from.X, (int)from.Y + 4f),
                                        new Vector2( (int)to.X, (int)to.Y + 4f)
                                    },new ColorBGRA(255,255,00,90));
            dxLine.End();
        }

        public static Obj_AI_Base GetDashObject()
        {
            float realAArange = Orbwalking.GetRealAutoAttackRange(ObjectManager.Player);
            var objects = ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValidTarget(realAArange));
            Vector2 apexPoint = ObjectManager.Player.ServerPosition.To2D() + (ObjectManager.Player.ServerPosition.To2D() - Game.CursorPos.To2D()).Normalized() * realAArange;
            Obj_AI_Base target = null;
            foreach (var obj in objects)
            {
                if (IsLyingInCone(obj.ServerPosition.To2D(), apexPoint, ObjectManager.Player.ServerPosition.To2D(), realAArange))
                {
                    if (target == null || target.Distance(apexPoint, true) > obj.Distance(apexPoint, true))
                        target = obj;
                }
            }
            return target;
        }

        public static bool IsLyingInCone(Vector2 position, Vector2 apexPoint, Vector2 circleCenter, float aperture)
        {
            float halfAperture = aperture / 2.0f;
            Vector2 apexToXVect = apexPoint - position;
            Vector2 axisVect = apexPoint - circleCenter;
            bool isInInfiniteCone = DotProd(apexToXVect, axisVect) / Magn(apexToXVect) / Magn(axisVect) > Math.Cos(halfAperture);
            if (!isInInfiniteCone) return false;
            return DotProd(apexToXVect, axisVect) / Magn(axisVect) < Magn(axisVect);
        }

        public static float DotProd(Vector2 a, Vector2 b){ return a.X * b.X + a.Y * b.Y; }
        public static float Magn(Vector2 a){ return (float)(Math.Sqrt(a.X * a.X + a.Y * a.Y)); }
        public static void fillPositions()
        {
            jumpPos = new Dictionary<Vector3, Vector3>();
            jumpPos.Add(new Vector3(4988f, 2820f, 51.11841f), new Vector3(4652f, 2756f, 95.74805f));
            jumpPos.Add(new Vector3(6074f, 5208f, 48.79716f), new Vector3(6022f, 5570f, 51.78092f));
            jumpPos.Add(new Vector3(4718f, 5612f, 50.23357f), new Vector3(4644f, 5882f, 51.61236f));
            jumpPos.Add(new Vector3(3260f, 6212f, 52.44749f), new Vector3(3024f, 6108f, 57.04759f));
            jumpPos.Add(new Vector3(3824f, 7706f, 51.87675f), new Vector3(3856f, 7408f, 51.60954f));
            jumpPos.Add(new Vector3(2596f, 9354f, 51.77317f), new Vector3(2824f, 9404f, 50.92833f));
            jumpPos.Add(new Vector3(3124f, 9906f, 52.92557f), new Vector3(3404f, 10122f, -65.31182f));
            jumpPos.Add(new Vector3(5714f, 10786f, 56.32285f), new Vector3(5472f, 10606f, -71.2406f));
            jumpPos.Add(new Vector3(5066f, 11844f, 56.82598f), new Vector3(5126f, 12178f, 56.4768f));
            jumpPos.Add(new Vector3(8272f, 3208f, 51.88526f), new Vector3(8344f, 2882f, 51.13f));
            jumpPos.Add(new Vector3(9772f, 3058f, 63.05689f), new Vector3(9738f, 2816f, 49.2229f));
            jumpPos.Add(new Vector3(9036f, 4506f, 52.42088f), new Vector3(9378f, 4524f, -71.2406f));
            jumpPos.Add(new Vector3(7882f, 5984f, 50.83319f), new Vector3(8070f, 6204f, -71.2406f));
            jumpPos.Add(new Vector3(11922f, 4758f, 51.7294f), new Vector3(11688f, 4606f, -71.2406f));
            jumpPos.Add(new Vector3(11972f, 5558f, 53.8434f), new Vector3(12306f, 5528f, 52.46079f));
            jumpPos.Add(new Vector3(10772f, 7208f, 51.7226f), new Vector3(10614f, 7462f, 51.83911f));
            jumpPos.Add(new Vector3(11622f, 8656f, 62.33571f), new Vector3(11776f, 8878f, 50.30732f));
            jumpPos.Add(new Vector3(8706f, 9604f, 50.38402f), new Vector3(8760f, 9380f, 52.18862f));
            jumpPos.Add(new Vector3(6974f, 8806f, 52.87261f), new Vector3(6832f, 8572f, -71.2406f));
            jumpPos.Add(new Vector3(6614f, 11736f, 53.83092f), new Vector3(6574f, 12006f, 56.4768f));

        }
    }
}
