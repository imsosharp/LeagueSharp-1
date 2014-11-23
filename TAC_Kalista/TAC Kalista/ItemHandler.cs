using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;
using xSLx_Orbwalker;
/**
 * Code was written by Esk0r (xQx)
 * I redesigned the code for my needs.
 */
namespace TAC_Kalista
{
    public class BuffList
    {
        public string ChampionName { get; set; }
        public string DisplayName { get; set; }
        public string BuffName { get; set; }
        public bool DefaultValue { get; set; }
        public int Delay { get; set; }
    }
    public enum PotionType { Health, Mana };
    public class Potion
    {
        public string Name { get; set; }
        public int MinCharges { get; set; }
        public ItemId ItemId { get; set; }
        public int Priority { get; set; }
        public List<PotionType> TypeList { get; set; }
    }
    class ItemHandler
    {
        public static double ActivatorTime;
        public static List<BuffList> BuffList = new List<BuffList>();
        public static List<Potion> potions;
        public static void init()
        {
            BuffList.Clear();
            BuffList.Add(new BuffList { ChampionName = "Darius", DisplayName = "Darius (W)", BuffName = "DariusNoxianTacticsONH", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Diana", DisplayName = "Diana (Q)", BuffName = "DianaArc", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Fizz", DisplayName = "Fizz (R)", BuffName = "FizzMarinerDoom", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Galio", DisplayName = "Galio (R)", BuffName = "GalioIdolOfDurand", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "LeBlanc", DisplayName = "LeBlanc (E)", BuffName = "LeblancSoulShackle", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Malzahar", DisplayName = "Malzahar (R)", BuffName = "AlZaharNetherGrasp", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Mordekaiser", DisplayName = "Mordekaiser (R)", BuffName = "MordekaiserChildrenOfTheGrave", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Nocturne", DisplayName = "Nocturne (R)", BuffName = "NocturneParanoia", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Poppy", DisplayName = "Poppy (R)", BuffName = "PoppyDiplomaticImmunity", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Rammus", DisplayName = "Rammus (E)", BuffName = "PuncturingTaunt", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "TwistedFate", DisplayName = "Twisted Fate (R)", BuffName = "Destiny", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Skarner", DisplayName = "Skarner (R)", BuffName = "SkarnerImpale", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Urgot", DisplayName = "Urgot (R)", BuffName = "UrgotSwap2", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Vladimir", DisplayName = "Vladimir (R)", BuffName = "VladimirHemoplague", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Warwick", DisplayName = "Warwick (R)", BuffName = "InfiniteDuress", DefaultValue = true, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Zilean", DisplayName = "Zilean (Q)", BuffName = "timebombenemybuff", DefaultValue = false, Delay = 0 });
            BuffList.Add(new BuffList { ChampionName = "Zed", DisplayName = "Zed (R)", BuffName = "zedulttargetmark", DefaultValue = true, Delay = 3 });
            BuffList.Add(new BuffList { ChampionName = "Morgana", DisplayName = "Morgana (Q)", BuffName = "DarkBindingMissile", DefaultValue = true, Delay = 0 });
            potions = new List<Potion> { 
                new Potion { Name = "ItemCrystalFlask", MinCharges = 1, ItemId = (ItemId)2041, Priority = 1, TypeList = new List<PotionType> { PotionType.Health, PotionType.Mana } }, 
                new Potion { Name = "RegenerationPotion", MinCharges = 0, ItemId = (ItemId)2003, Priority = 2, TypeList = new List<PotionType> { PotionType.Health } }, 
                new Potion { Name = "ItemMiniRegenPotion", MinCharges = 0, ItemId = (ItemId)2010, Priority = 4, TypeList = new List<PotionType> { PotionType.Health, PotionType.Mana } }, 
                new Potion { Name = "FlaskOfCrystalWater", MinCharges = 0, ItemId = (ItemId)2004, Priority = 3, TypeList = new List<PotionType> { PotionType.Mana } } };
        }
        public static void PotionHandler()
        {
            if (ObjectManager.Player.HasBuff("Recall") || Utility.InFountain() && Utility.InShopRange()) return;

            if (MenuHandler.Config.Item("HealthPotion").GetValue<bool>())
            {
                if (MathHandler.GetPlayerHealthPercentage() <= MenuHandler.Config.Item("HealthPercent").GetValue<Slider>().Value)
                {
                    var healthSlot = GetPotionSlot(PotionType.Health);
                    if (!IsBuffActive(PotionType.Health))
                        healthSlot.UseItem();
                }
            }
            if (MenuHandler.Config.Item("ManaPotion").GetValue<bool>())
            {
                if (MathHandler.GetPlayerManaPercentage() <= MenuHandler.Config.Item("ManaPercent").GetValue<Slider>().Value)
                {
                    var manaSlot = GetPotionSlot(PotionType.Mana);
                    if (!IsBuffActive(PotionType.Mana))
                        manaSlot.UseItem();
                }
            }
        }

        public static InventorySlot GetPotionSlot(PotionType type)
        {
            return (from potion in potions
                    where potion.TypeList.Contains(type)
                    from item in ObjectManager.Player.InventoryItems
                    where item.Id == potion.ItemId && item.Charges >= potion.MinCharges
                    select item).FirstOrDefault();
        }
        public static bool IsBuffActive(PotionType type)
        {
            return (from potion in potions
                    where potion.TypeList.Contains(type)
                    from buff in ObjectManager.Player.Buffs
                    where buff.Name == potion.Name && buff.IsActive
                    select potion).Any();
        }
        public static void useItem()
        {
            Obj_AI_Hero target = SimpleTs.GetTarget(xSLxOrbwalker.GetAutoAttackRange(ObjectManager.Player), SimpleTs.DamageType.Physical);
            if (MenuHandler.Config.Item("BOTRK").GetValue<bool>())
            {
                if (target != null
                    && target.Type == ObjectManager.Player.Type
                        && target.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 450)
                {
                    var hasCutGlass = Items.HasItem(3144);
                    var hasBotrk = Items.HasItem(3153);
                    if (hasBotrk || hasCutGlass)
                    {
                        var itemId = hasCutGlass ? 3144 : 3153;
                        var damage = ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                        if (hasCutGlass || ObjectManager.Player.Health + damage < ObjectManager.Player.MaxHealth)
                            Items.UseItem(itemId, target);
                    }
                }
            }
            if (MenuHandler.Config.Item("GHOSTBLADE").GetValue<bool>()
                && target != null
                    && target.Type == ObjectManager.Player.Type
                        && Orbwalking.InAutoAttackRange(target))
            {
                Items.UseItem(3142);
            }
            if (MenuHandler.Config.Item("SWORD").GetValue<bool>()
                    && target != null
                        && target.Type == ObjectManager.Player.Type
                            && Orbwalking.InAutoAttackRange(target))
            {
                Items.UseItem(3131);
            }
            if (target != null
                && MenuHandler.Config.Item("IGNITE").GetValue<bool>()
                    && ObjectManager.Player.GetSpellSlot("SummonerDot") != SpellSlot.Unknown
                        && ObjectManager.Player.SummonerSpellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("SummonerDot")) == SpellState.Ready
                            && ObjectManager.Player.Distance(target) < 650 &&
                ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
            {
                ObjectManager.Player.SummonerSpellbook.CastSpell(ObjectManager.Player.GetSpellSlot("SummonerDot"), target);
            }
        }
        public static void CheckChampionBuff()
        {
            var QuickSilverMenu = MenuHandler.Config.SubMenu("Items").SubMenu("QuickSilverSash");
            foreach (var t1 in ObjectManager.Player.Buffs)
            {
                foreach (var t in QuickSilverMenu.Items)
                {
                    if (QuickSilverMenu.Item(t.Name).GetValue<bool>())
                    {
                        if (t1.Name.ToLower().Contains(t.Name.ToLower()))
                        {
                            foreach (var bx in BuffList.Where(bx => bx.BuffName == t1.Name))
                            {
                                if (bx.Delay > 0)
                                {
                                    if (ActivatorTime + bx.Delay < (int)Game.Time)
                                        ActivatorTime = (int)Game.Time;
                                    if (ActivatorTime + bx.Delay <= (int)Game.Time)
                                    {
                                        if (Items.HasItem(3139)) Items.UseItem(3139);
                                        if (Items.HasItem(3140)) Items.UseItem(3140);
                                        ActivatorTime = (int)Game.Time;
                                    }
                                }
                                else
                                {
                                    if (Items.HasItem(3139)) Items.UseItem(3139);
                                    if (Items.HasItem(3140)) Items.UseItem(3140);
                                }
                            }
                        }
                    }
                    if (QuickSilverMenu.Item("AnySnare").GetValue<bool>() &&
                    ObjectManager.Player.HasBuffOfType(BuffType.Snare))
                    {
                        if (Items.HasItem(3139)) Items.UseItem(3139);
                        if (Items.HasItem(3140)) Items.UseItem(3140);
                    }
                    if (QuickSilverMenu.Item("AnyStun").GetValue<bool>() &&
                    ObjectManager.Player.HasBuffOfType(BuffType.Stun))
                    {
                        if (Items.HasItem(3139)) Items.UseItem(3139);
                        if (Items.HasItem(3140)) Items.UseItem(3140);
                    }
                    if (QuickSilverMenu.Item("AnyTaunt").GetValue<bool>() &&
                    ObjectManager.Player.HasBuffOfType(BuffType.Taunt))
                    {
                        if (Items.HasItem(3139)) Items.UseItem(3139);
                        if (Items.HasItem(3140)) Items.UseItem(3140);
                    }
                }
            }
        }
    }
}