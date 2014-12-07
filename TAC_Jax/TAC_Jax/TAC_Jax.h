#using "LeagueSharp.dll"
#using "LeagueSharp.Common.dll"
#using "SharpDX.dll"
#using "SharpDX.Direct3D9.dll"

using namespace System;
using namespace LeagueSharp;
using namespace LeagueSharp::Common;

namespace TAC_Jax
{
	public ref class LPP
	{
	internal:
		static Menu^ Config;
		static Menu^ TSMenu;
		static Menu^ OrbwalkerMenu;
		static Orbwalking::Orbwalker^ Orbwalker;
		static Spell^ Q;
		static Spell^ W;
		static Spell^ E;
		static Spell^ R;
	public:
		static void onGame();
		static void onGameLoadMenu();
		static void onDraw(EventArgs^ args);
		static void onGameUpdate(EventArgs^ args);
		static void onEnemyGapCloser(ActiveGapcloser gapcloser);
		static void onPossibleToInterrupt(Obj_AI_Base^ unit, InterruptableSpell spell);
		static void onProcessSpellCast(Obj_AI_Base^ sender, GameObjectProcessSpellCastEventArgs^ args);
	};
}