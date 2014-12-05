#using "LeagueSharp.dll"
#using "LeagueSharp.Common.dll"

using namespace System;
using namespace LeagueSharp;
using namespace LeagueSharp::Common;

ref struct Globals
{
	// Initialize variables here
	static Menu ^Config;
};
ref struct Skills
{
	static Spell^ Q = gcnew Spell(SpellSlot::Q, 600);
	static Spell^ W = gcnew Spell(SpellSlot::W, 600);
	static Spell^ E = gcnew Spell(SpellSlot::E, 600);
	static Spell^ R = gcnew Spell(SpellSlot::R, 600);
};