﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Duck
{
    public class RailgunBlast : Projectiles.Deathrays.BaseDeathray
    {
        public RailgunBlast() : base(20, "PhantasmalDeathrayML") { }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Railgun Blast");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            cooldownSlot = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.ranged = true;

            projectile.hide = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (!Main.dedServ && Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 2;

            Vector2? vector78 = null;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }

            float num801 = 10f;
            projectile.scale = (float)Math.Cos(projectile.localAI[0] * MathHelper.PiOver2 / maxTime) * num801;
            if (projectile.scale > num801)
                projectile.scale = num801;

            if (player.active && !player.dead && !player.ghost)
            {
                projectile.Center = player.Center + (18f * projectile.scale + 50f) * projectile.velocity + Main.rand.NextVector2Circular(5, 5);
            }
            else
            {
                projectile.Kill();
                return;
            }

            if (projectile.localAI[0] == 0f)
            {
                projectile.frame = Main.rand.Next(15);
                if (!Main.dedServ)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Railgun"), projectile.Center);
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Thunder").WithVolume(0.5f), projectile.Center);
                }

                Vector2 dustPos = player.Center + projectile.velocity * 50f;

                
            }
            
            if (++projectile.localAI[0] >= maxTime)
            {
                projectile.Kill();
                return;
            }
            //projectile.scale = num801;
            //float num804 = projectile.velocity.ToRotation();
            //num804 += projectile.ai[0];
            //projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)projectile.ai[1]].ai[3] - 1.57079637f;
            //if (projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //projectile.rotation = num804;
            //num804 += 1.57079637f;
            //projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)projectile.width;
            Vector2 samplingPoint = projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 3000f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num807, amount);
            /*Vector2 vector79 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, 244, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.Next(5) == 0)
            {
                Vector2 value29 = projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }*/
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            projectile.position -= projectile.velocity;
            projectile.rotation = projectile.velocity.ToRotation() - 1.57079637f;

            if (++projectile.frame > 15)
                projectile.frame = 0;

            const int increment = 100;
            for (int i = 0; i < array3[0]; i += increment)
            {
                float offset = i + Main.rand.NextFloat(-increment, increment);
                if (offset < 0)
                    offset = 0;
                if (offset > array3[0])
                    offset = array3[0];
                float spawnRange = projectile.scale * 32f;
                int d = Dust.NewDust(projectile.position + projectile.velocity * offset + projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-spawnRange, spawnRange),
                    projectile.width, projectile.height, 91, 0f, 0f, 0, default, Main.rand.NextFloat(1f, 4f));
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity += projectile.velocity * 2f;
                Main.dust[d].velocity *= Main.rand.NextFloat(12f, 24f) / 10f * projectile.scale;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 12;
            target.AddBuff(BuffID.Electrified, 600);

            if (projectile.owner == Main.myPlayer)
            {
                const int max = 6;
                for (int i = -max / 2; i <= max / 2; i++)
                {
                    Vector2 vel = Main.rand.NextFloat(15f, 30f) * projectile.velocity.RotatedBy(MathHelper.ToRadians(75) / max * (i + Main.rand.NextFloat(-0.5f, 0.5f)));
                    Projectile.NewProjectile(target.Center, vel, ModContent.ProjectileType<Projectiles.LightningArc>(), 
                        projectile.damage / 10, projectile.knockBack / 10, projectile.owner, vel.ToRotation(), Main.rand.Next(80));
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ItemID.SilverDye);
            shader.Apply(projectile, new Terraria.DataStructures.DrawData?());

            SpriteEffects spriteEffects = Main.rand.NextBool() ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture2D19 = mod.GetTexture("Projectiles/Deathrays/Mutant/MutantDeathray_" + projectile.frame.ToString());
            Texture2D texture2D20 = mod.GetTexture("Projectiles/Deathrays/Mutant/MutantDeathray2_" + projectile.frame.ToString());
            Texture2D texture2D21 = mod.GetTexture("Projectiles/Deathrays/" + texture + "3");
            float num223 = projectile.localAI[1];
            Color color44 = new Color(255, 255, 255, 100) * 0.95f;
            SpriteBatch arg_ABD8_0 = Main.spriteBatch;
            Texture2D arg_ABD8_1 = texture2D19;
            Vector2 arg_ABD8_2 = projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            arg_ABD8_0.Draw(arg_ABD8_1, arg_ABD8_2, sourceRectangle2, color44, projectile.rotation, texture2D19.Size() / 2f, projectile.scale, spriteEffects, 0f);
            num223 -= (texture2D19.Height / 2 + texture2D21.Height) * projectile.scale;
            Vector2 value20 = projectile.Center;
            value20 += projectile.velocity * projectile.scale * texture2D19.Height / 2f;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = new Rectangle(0, 0, texture2D20.Width, 30);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }

                    Main.spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), color44, projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), projectile.scale, spriteEffects, 0f);
                    num224 += rectangle7.Height * projectile.scale;
                    value20 += projectile.velocity * rectangle7.Height * projectile.scale;
                    rectangle7.Y += 30;
                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            SpriteBatch arg_AE2D_0 = Main.spriteBatch;
            Texture2D arg_AE2D_1 = texture2D21;
            Vector2 arg_AE2D_2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;
            arg_AE2D_0.Draw(arg_AE2D_1, arg_AE2D_2, sourceRectangle2, color44, projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}