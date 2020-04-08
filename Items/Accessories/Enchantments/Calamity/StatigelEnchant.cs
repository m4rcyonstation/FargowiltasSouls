﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Summon;

namespace FargowiltasSouls.Items.Accessories.Enchantments.Calamity
{
    public class StatigelEnchant : ModItem
    {
        private readonly Mod calamity = ModLoader.GetMod("CalamityMod");

        public override bool Autoload(ref string name)
        {
            return ModLoader.GetMod("CalamityMod") != null;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statigel Enchantment");
            Tooltip.SetDefault(
@"'Statis’ mystical power surrounds you…'
When you take over 100 damage in one hit you become immune to damage for an extended period of time
Grants an extra jump and increased jump height
Effects of Counter Scarf and Fungal Symbiote");
            DisplayName.AddTranslation(GameCulture.Chinese, "斯塔提斯魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"'斯塔提斯的神秘力量环绕着你...'
一次性承受超过100点伤害时, 加长无敌时间
能够多跳跃一次, 增加跳跃高度
召唤迷你史莱姆之神为你而战, 种类视世界而定
拥有反击围巾和真菌共生体的效果");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = 5;
            item.value = 200000;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(181, 0, 156);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!Fargowiltas.Instance.CalamityLoaded) return;

            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            calamity.Call("SetSetBonus", player, "statigel", true);
            player.doubleJumpSail = true;
            player.jumpBoost = true;

            //idk even
            /*if (SoulConfig.Instance.GetValue("Slime God Minion"))
            {
                //summon
                calamity.Call("SetSetBonus", player, "statigel_summon", true);
                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.FindBuffIndex(ModContent.BuffType<StatigelSummonSetBuff>()) == -1)
                    {
                        player.AddBuff(ModContent.BuffType<StatigelSummonSetBuff>(), 3600, true);
                    }
                    if (WorldGen.crimson && player.ownedProjectileCounts[ModContent.ProjectileType<CrimsonSlimeGodMinion>()] < 1)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<CrimsonSlimeGodMinion>(), (int)(33f * player.minionDamage), 0f, Main.myPlayer, 0f, 0f);
                        return;
                    }
                    if (!WorldGen.crimson && player.ownedProjectileCounts[ModContent.ProjectileType<CorruptionSlimeGodMinion>()] < 1)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<CorruptionSlimeGodMinion>(), (int)(33f * player.minionDamage), 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }*/

            calamity.GetItem("FungalSymbiote").UpdateAccessory(player, hideVisual);
            calamity.GetItem("CounterScarf").UpdateAccessory(player, hideVisual);
        }

        public override void AddRecipes()
        {
            if (!Fargowiltas.Instance.CalamityLoaded) return;

            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddRecipeGroup("FargowiltasSouls:AnyStatigelHelmet");
            recipe.AddIngredient(ModContent.ItemType<StatigelArmor>());
            recipe.AddIngredient(ModContent.ItemType<StatigelGreaves>());
            recipe.AddIngredient(ModContent.ItemType<CounterScarf>());
            recipe.AddIngredient(ModContent.ItemType<ManaOverloader>());
            recipe.AddIngredient(ModContent.ItemType<FungalSymbiote>());
            recipe.AddIngredient(ModContent.ItemType<Carnage>());
            recipe.AddIngredient(ModContent.ItemType<ClothiersWrath>());
            recipe.AddIngredient(ModContent.ItemType<CinderBlossomStaff>());
            recipe.AddRecipeGroup("FargowiltasSouls:AnyEvilEffigy");

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
