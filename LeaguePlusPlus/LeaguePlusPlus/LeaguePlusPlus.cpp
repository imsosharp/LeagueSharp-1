// LeaguePlusPlus.cpp : main project file.

#include "stdafx.h"
#include "LeaguePlusPlus.h"

using namespace System;
using namespace LeagueSharp;
using namespace LeagueSharp::Common;

extern "C" __declspec(dllexport) void __stdcall onGame()
{
	// Finetune spells if needed or load some shit
	// Skills::Q->SetTargetted(0.5f, 1400);
	Game::PrintChat("Loading game setting handler!");
}
extern "C" __declspec(dllexport) void __stdcall onGameLoadMenu()
{
	Game::PrintChat("Loading Menu handler!");
	Globals::Config = gcnew Menu("It works", "It works", true);
	Globals::Config->AddItem(gcnew MenuItem("hi", "Hi world!"))->SetValue(true);
	Globals::Config->AddToMainMenu();
}
void __clrcall onDraw(EventArgs args)
{
	// Do drawings here
	Game::PrintChat("Loading Drawing handler!");
}
void __clrcall onGameUpdate(EventArgs args)
{
	// Do game update logic here
	Game::PrintChat("Loading Game Update handler!");
}
void __clrcall onEnemyGapCloser(ActiveGapcloser gapcloser)
{
	// Do gap closing logic here
	Game::PrintChat("Loading Gap Closer handler!");
}

void __clrcall onPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
{
	// Do gap closing logic here
	Game::PrintChat("Loading Interrupt handler!");
}
void __clrcall onProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
{
	// Do spell on progress spell cast logic here
	Game::PrintChat("Loading Spell Cast handler!");
}