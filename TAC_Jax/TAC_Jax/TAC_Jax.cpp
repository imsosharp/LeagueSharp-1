#include "stdafx.h"
#include "TAC_Jax.h"

using namespace System;
using namespace LeagueSharp;
using namespace LeagueSharp::Common;

namespace TAC_Jax
{
	void LPP::onGame()
	{
		// Finetune spells
		LPP::Q = gcnew Spell(SpellSlot::Q, 680);
		LPP::W = gcnew Spell(SpellSlot::W, 125);
		LPP::E = gcnew Spell(SpellSlot::E, 190);
		LPP::R = gcnew Spell(SpellSlot::R, 125);

//		LPP::Q->SetTargetted(0.5,75,SharpDX::Vector3(),LPP::Q->Range);
	}
	void LPP::onGameLoadMenu()
	{
		Game::PrintChat("onGameLoadMenu issued");
		LPP::Config = gcnew Menu("TAC Jax", "tac_jax", true);

		LPP::TSMenu = gcnew Menu("Target Selector", "ts", false);
		SimpleTs::AddToMenu(LPP::TSMenu);
		LPP::Config->AddSubMenu(LPP::TSMenu);

		LPP::OrbwalkerMenu = gcnew Menu("Orbwalking", "Orbwalking",false);
		LPP::Config->AddSubMenu(LPP::OrbwalkerMenu);
		LPP::Orbwalker = gcnew Orbwalking::Orbwalker(LPP::Config->SubMenu("Orbwalking"));
		LPP::Orbwalker->SetAttack(true);

		LPP::Config->AddSubMenu(gcnew Menu("Auto-Carry", "ac", false));
		LPP::Config->SubMenu("ac")->AddItem(gcnew MenuItem("acQ", "Use Q"))->SetValue(true);
		LPP::Config->SubMenu("ac")->AddItem(gcnew MenuItem("acW", "Use W"))->SetValue(true);
		LPP::Config->SubMenu("ac")->AddItem(gcnew MenuItem("acE", "Use E"))->SetValue(true);
		LPP::Config->SubMenu("ac")->AddItem(gcnew MenuItem("acR", "Use R"))->SetValue(true);

		LPP::Config->AddSubMenu(gcnew Menu("Mixed", "mx", false));
		LPP::Config->SubMenu("mx")->AddItem(gcnew MenuItem("mxQ", "Use Q"))->SetValue(true);
		LPP::Config->SubMenu("mx")->AddItem(gcnew MenuItem("mxW", "Use W"))->SetValue(true);
		LPP::Config->SubMenu("mx")->AddItem(gcnew MenuItem("mxE", "Use E"))->SetValue(true);
		LPP::Config->SubMenu("mx")->AddItem(gcnew MenuItem("mxR", "Try to use 3rd R"))->SetValue(true);

		LPP::Config->AddSubMenu(gcnew Menu("Drawings", "draw", false));
		//LPP::Config->SubMenu("draw")->AddItem(gcnew MenuItem("drawQ", "Draw Q Range")->SetValue(gcnew Circle(true, System::Drawing::Color::FromArgb(255, 255, 255, 255))));
		LPP::Config->SubMenu("draw")->AddItem(gcnew MenuItem("drawE", "Draw E Range"))->SetValue(true);
		LPP::Config->SubMenu("draw")->AddItem(gcnew MenuItem("wjRange", "Draw Ward Jump Range"))->SetValue(true);


		LPP::Config->AddToMainMenu();
	}
	void LPP::onDraw(EventArgs^ args)
	{
		Game::PrintChat("onDraw issued");
	}
	void LPP::onGameUpdate(EventArgs^ args)
	{
		Game::PrintChat("onGameUpdate issued");
	}
	void LPP::onEnemyGapCloser(ActiveGapcloser gapcloser)
	{
		Game::PrintChat("onEnemyGapCloser issued");
	}
	void LPP::onPossibleToInterrupt(Obj_AI_Base^ unit, InterruptableSpell spell)
	{
		Game::PrintChat("onPossibleToInterrupt issued");
	}
	void LPP::onProcessSpellCast(Obj_AI_Base^ sender, GameObjectProcessSpellCastEventArgs^ args)
	{
		Game::PrintChat("onProcessSpellCast issued");
	}
}