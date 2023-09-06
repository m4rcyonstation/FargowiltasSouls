﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;
using System.Collections.Generic;
using FargowiltasSouls.Content.Bosses.Lieflight;
using Terraria.DataStructures;
using System.IO;

namespace FargowiltasSouls.Content.Projectiles
{
    /// <summary>
    /// ai0 determines the rotation, ai1 the cone width, ai2 the length
    /// </summary>
    public class ArcTelegraph : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_" + ExtrasID.MartianProbeScanWave;//this is unrelated actually
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 60 * 5;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.width = 1;
            Projectile.height = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(npc);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc = reader.Read7BitEncodedInt();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        int npc;
        public override void OnSpawn(IEntitySource source)
        {
            switch (Projectile.ai[0])
            {
                case 1:
                    {
                        if (source is EntitySource_Parent parent && parent.Entity is NPC parentNpc && parentNpc.type == ModContent.NPCType<LifeChallenger>())
                        {
                            npc = parentNpc.whoAmI;
                            float angleToMe = Projectile.velocity.ToRotation();
                            float angleToPlayer = (Main.player[parentNpc.target].Center - parentNpc.Center).ToRotation();
                            Projectile.localAI[1] = MathHelper.WrapAngle(angleToMe - angleToPlayer);
                        }
                        break;
                    }
            }
        }
        public override void AI()
        {
            Projectile.position -= Projectile.velocity;

            switch (Projectile.ai[0])
            {
                case 1:
                    {
                        NPC parent = FargoSoulsUtil.NPCExists(npc);
                        if (parent != null)
                        {
                            Projectile.Center = parent.Center;
                            Vector2 offset = Main.player[parent.target].Center - parent.Center;
                            Projectile.rotation = offset.RotatedBy(Projectile.localAI[1]).ToRotation();
                            telegraphColor = Color.Cyan;
                        }
                        break;
                    }
            }
        }
        public float WidthFunction(float progress)
        {
            return Projectile.ai[2];
        }
        public Color ColorFunction(float progress)
        {
            return telegraphColor;
        }
        public void SetEffectParameters(Effect effect)
        {
            effect.Parameters["WorldViewProjection"].SetValue(GetWorldViewProjectionMatrixIdioticVertexShaderBoilerplate());
        }
        public static Matrix GetWorldViewProjectionMatrixIdioticVertexShaderBoilerplate()
        {
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(Main.graphics.GraphicsDevice.Viewport.Width / 2, Main.graphics.GraphicsDevice.Viewport.Height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1f);
            Matrix projection = Matrix.CreateOrthographic(Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height, 0, 1000);
            return view * projection;
        }
        Effect shader;
        public Color telegraphColor;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if(shader == null)
                shader = ModContent.Request<Effect>("FargowiltasSouls/Assets/Effects/Vertex_ArcTelegraph", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            SetEffectParameters(shader);
            shader.CurrentTechnique.Passes[0].Apply();
            VertexStrip vertexStrip = new();
            List<Vector2> positions = new();
            List<float> rotations = new();
            for (float i = 0; i < 1; i += 0.005f)
            {
                float rotation = Projectile.rotation - Projectile.ai[1] * 0.5f + Projectile.ai[1] * i;
                positions.Add(rotation.ToRotationVector2() * Projectile.ai[2] + Projectile.Center);
                rotations.Add(rotation + MathHelper.PiOver2);
            } 
            vertexStrip.PrepareStrip(positions.ToArray(), rotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);
            vertexStrip.DrawTrail();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);         
            return false; 
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
    }
}
