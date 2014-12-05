// LeaguePlusPlus.cpp : main project file.

#include "stdafx.h"
#include "LeaguePlusPlus.h"

using namespace System;
using namespace LeagueSharp;
using namespace LeagueSharp::Common;

namespace LeaguePlusPlus
{
	void LPP::onGame()
	{
		Game::PrintChat("onGame issued");
	}
	void LPP::onGameLoadMenu()
	{
		Game::PrintChat("onGameLoadMenu issued");
		LPP::Config = gcnew MenuWrapper("Better menu",true,true);
		LPP::Config->MainMenu->AddBool("It works!",true);
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