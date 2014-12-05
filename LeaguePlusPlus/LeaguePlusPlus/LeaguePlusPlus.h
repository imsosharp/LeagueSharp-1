#using "LeagueSharp.dll"
#using "LeagueSharp.Common.dll"

using namespace System;
using namespace LeagueSharp;
using namespace LeagueSharp::Common;

namespace LeaguePlusPlus
{
	public ref class LPP
	{
	internal:
		static MenuWrapper ^Config;
		static Spell ^Q = gcnew Spell(SpellSlot::Q, 600);
		static Spell ^W = gcnew Spell(SpellSlot::W, 600);
		static Spell ^E = gcnew Spell(SpellSlot::E, 600);
		static Spell ^R = gcnew Spell(SpellSlot::R, 600);
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