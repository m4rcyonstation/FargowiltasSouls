﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SmallStinger : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Stinger");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.HornetStinger);
            aiType = ProjectileID.Bullet;
            projectile.penetrate = -1;
            projectile.minion = false;
            projectile.ranged = true;
            projectile.timeLeft = 120;
            projectile.width = 10;
            projectile.height = 18;
            projectile.scale *= 1.5f;
            projectile.height = (int)(projectile.height * 1.5f);
            projectile.width = (int)(projectile.width * 1.5f);
        }

        public override void AI()
        {
            //stuck in enemy
            if(projectile.ai[0] == 1)
            {
                projectile.aiStyle = -1;

                projectile.ignoreWater = true;
                projectile.tileCollide = false;

                int secondsStuck = 15;
                bool kill = false;
  
                projectile.localAI[0] += 1f;

                int npcIndex = (int)projectile.ai[1];
                if (projectile.localAI[0] >= (float)(60 * secondsStuck))
                {
                    kill = true;
                }
                else if (npcIndex < 0 || npcIndex >= 200)
                {
                    kill = true;
                }
                else if (Main.npc[npcIndex].active && !Main.npc[npcIndex].dontTakeDamage)
                {
                    projectile.Center = Main.npc[npcIndex].Center - projectile.velocity * 2f;
                    projectile.gfxOffY = Main.npc[npcIndex].gfxOffY;
                }
                else
                {
                    kill = true;
                }

                if (kill)
                {
                    projectile.Kill();
                }
            }
            else
            {
                projectile.position += projectile.velocity * 0.5f;

                //dust from stinger
                if (Main.rand.NextBool())
                {
                    int num92 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 18, 0f, 0f, 0, default(Color), 0.9f);
                    Main.dust[num92].noGravity = true;
                    Main.dust[num92].velocity *= 0.5f;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            crit = false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for(int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                if(p.active && p.type == projectile.type && p != projectile && projectile.Hitbox.Intersects(p.Hitbox))
                {
                    target.StrikeNPC(damage / 2, 0, 0, true); //normal damage but looks like a crit ech
                    target.AddBuff(BuffID.Poisoned, 600);
                    DustRing(p, 16);
                    p.Kill();
                    Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 27, 1f, -0.4f);
                    break;
                }
            }

            projectile.ai[0] = 1;
            projectile.ai[1] = (float)target.whoAmI;
            projectile.velocity = (Main.npc[target.whoAmI].Center - projectile.Center) * 1f; //distance it sticks out
            projectile.aiStyle = -1;
            projectile.damage = 0;
            projectile.timeLeft = 300;
            projectile.netUpdate = true;
        }

        public override void Kill(int timeLeft)
        {
            for(int i = 0; i < 10; i++)
            {
                int num92 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 18, projectile.velocity.X, projectile.velocity.Y, 0, default(Color), 0.9f);
                Main.dust[num92].noGravity = true;
                Main.dust[num92].velocity *= 0.25f;
                Main.dust[num92].fadeIn = 1.3f;
            }
            Main.PlaySound(SoundID.Item10, projectile.Center);
        }

        private void DustRing(Projectile proj, int max)
        {
            //dust
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = Vector2.UnitY * 5f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + proj.Center;
                Vector2 vector7 = vector6 - proj.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, 18, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            Color color25 = Lighting.GetColor((int)(projectile.position.X + projectile.width * 0.5) / 16, (int)((projectile.position.Y + projectile.height * 0.5) / 16.0));
            Texture2D texture2D3 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y3 = num156 * projectile.frame;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 7;
            int num159 = 0;
            float num160 = 0f;


            int num161 = num159;
            while (projectile.ai[0] != 1 && num161 < num157) //doesnt draw trail while stuck in enemy
            {
                Color color26 = color25;
                color26 = projectile.GetAlpha(color26);
                float num164 = (num157 - num161);
                color26 *= num164 / (ProjectileID.Sets.TrailCacheLength[projectile.type] * 1.5f);
                color26 *= 0.75f;
                Vector2 value4 = projectile.oldPos[num161];
                float num165 = projectile.rotation;
                SpriteEffects effects = spriteEffects;
                Main.spriteBatch.Draw(texture2D3, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + projectile.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, projectile.scale * 0.8f, effects, 0f);
                num161++;
            }

            Color color29 = projectile.GetAlpha(color25);
            Main.spriteBatch.Draw(texture2D3, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color29, projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            //smaller tile hitbox
            width = 8;
            height = 8;
            return true;
        }
    }
}