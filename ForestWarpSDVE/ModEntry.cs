using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;

namespace ForestWarp
{
    /// <summary>The mod entry point.</summary>

    public class ModEntry : StardewModdingAPI.Mod
    {
        private int warpTotemID;

        /*********
        Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.modifyTotem;
            helper.Events.Input.ButtonPressed += this.OnTotemConsumed;
        }


        /*********
        Private methods
        *********/

        /// <summary>Warps Player to Forest Tile when Warp Totem: Forest is clicked in inventory.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// 

        private void modifyTotem(object sender, GameLaunchedEventArgs e)
        {
            IMyJsonAssetsInterface jaApi = Helper.ModRegistry.GetApi<IMyJsonAssetsInterface>("spacechase0.JsonAssets");
            this.warpTotemID = jaApi.GetObjectId("Forest Warp Totem");
            if (this.warpTotemID == -1)
                this.Monitor.Log("Can't get ID for Forest Totem. Some functionality will be lost.", (LogLevel)3);
            else
            {
                this.Monitor.Log(string.Format("Forest Warp Totem ID is {0}.", (object)this.warpTotemID), (LogLevel)2);
                StardewValley.Object warpTotemObj = Object()
            }
        }

        private void OnTotemConsumed(object sender, ButtonPressedEventArgs e)
        {
            // ignore button presses for certain cases
            if (!Context.IsWorldReady || Game1.currentLocation == null || Game1.activeClickableMenu != null || !SButtonExtensions.IsActionButton(e.Button))
                return;

            Game1.currentLocation.Objects.TryGetValue(e.Cursor.GrabTile, out StardewValley.Object clickedObj);
            Item heldItem = Game1.player.ActiveObject;

            // for testing

            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);

            if (clickedObj != null)
                this.Monitor.Log($"{Game1.player.Name} clicked {clickedObj.DisplayName}.", LogLevel.Debug);
            if (heldItem != null)
                this.Monitor.Log($"{Game1.player.Name} held {heldItem.DisplayName} whose ID is {heldItem.ParentSheetIndex}.", LogLevel.Debug);


            // case where totem is held and right clicked

            if (heldItem.DisplayName == "Forest Warp Totem")
            {
                this.Helper.Reflection.GetMethod(Game1.player, "reduceActiveItemByOne", true).Invoke();
                useTotem(this.warpTotemID);                
            }
            return;
        }

        private void useTotem(int warpTotemID)
        {
            Game1.player.jitterStrength = 1f;
            Color glowColor = Color.Purple;
            Game1.player.currentLocation.playSound("warrior", NetAudio.SoundContext.Default);
            Game1.player.faceDirection(2);
            Game1.player.CanMove = false;
            Game1.player.temporarilyInvincible = true;
            Game1.player.temporaryInvincibilityTimer = -4000;
            Game1.changeMusicTrack("none", false, Game1.MusicContext.Default);
            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
            {
                  new FarmerSprite.AnimationFrame(57, 2000, false, false, (AnimatedSprite.endOfAnimationBehavior) null, false),
                  new FarmerSprite.AnimationFrame((int) (short) Game1.player.FarmerSprite.CurrentFrame, 0, false, false, delegate { customTotemWarp(); }, true)
            }, (AnimatedSprite.endOfAnimationBehavior)null);
            this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue()
                    .broadcastSprites(Game1.player.currentLocation,
                    new TemporaryAnimatedSprite(warpTotemID, 9999f, 1, 999, Game1.player.Position + new Vector2(0.0f, -96f), false, false, false, 0.0f)
            {
                motion = new Vector2(0.0f, -1f),
                scaleChange = 0.01f,
                alpha = 1f,
                alphaFade = 0.0075f,
                shakeIntensity = 1f,
                initialPosition = Game1.player.Position + new Vector2(0.0f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 1f
            });
             this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue()
                     .broadcastSprites(Game1.player.currentLocation,
                     new TemporaryAnimatedSprite(warpTotemID, 9999f, 1, 999, Game1.player.Position + new Vector2(-64f, -96f), false, false, false, 0.0f)
            {
                motion = new Vector2(0.0f, -0.5f),
                scaleChange = 0.005f,
                scale = 0.5f,
                alpha = 1f,
                alphaFade = 0.0075f,
                shakeIntensity = 1f,
                delayBeforeAnimationStart = 10,
                initialPosition = Game1.player.Position + new Vector2(-64f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 0.9999f
            });
            this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue()
                    .broadcastSprites(Game1.player.currentLocation,
                    new TemporaryAnimatedSprite(warpTotemID, 9999f, 1, 999, Game1.player.Position + new Vector2(64f, -96f), false, false, false, 0.0f)
            {
                motion = new Vector2(0.0f, -0.5f),
                scaleChange = 0.005f,
                scale = 0.5f,
                alpha = 1f,
                alphaFade = 0.0075f,
                delayBeforeAnimationStart = 20,
                shakeIntensity = 1f,
                initialPosition = Game1.player.Position + new Vector2(64f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 0.9988f
            });
            Game1.screenGlowOnce(glowColor, false, 0.005f, 0.3f);
            Utility.addSprinklesToLocation(Game1.player.currentLocation, Game1.player.getTileX(), Game1.player.getTileY(), 16, 16, 1300, 20, Color.White, (string)null, true);
            return;
        }

        private void customTotemWarp()
        {
            for (int index = 0; index < 12; ++index)
                this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue()
                    .broadcastSprites(Game1.player.currentLocation, new TemporaryAnimatedSprite(354, (float)Game1.random.Next(25, 75), 6, 1, new Vector2((float)Game1.random.Next((int)Game1.player.Position.X - 256, (int)Game1.player.Position.X + 192), (float)Game1.random.Next((int)Game1.player.Position.Y - 256, (int)Game1.player.Position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
            Game1.player.currentLocation.playSound("wand", NetAudio.SoundContext.Default);
            Game1.displayFarmer = false;
            Game1.player.temporaryInvincibilityTimer = -2000;
            Game1.player.freezePause = 1000;
            Game1.flashAlpha = 1f;
            DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(customTotemWarpForReal), 1000);
            new Rectangle(Game1.player.GetBoundingBox().X, Game1.player.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
            int num = 0;
            for (int index = Game1.player.getTileX() + 8; index >= Game1.player.getTileX() - 8; --index)
            {
                this.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue()
                    .broadcastSprites(Game1.player.currentLocation,
                    new TemporaryAnimatedSprite(6, new Vector2((float)index, (float)Game1.player.getTileY()) * 64f, Color.White, 8, false, 50f, 0, -1, -1f, -1, 0)
                    {
                        layerDepth = 1f,
                        delayBeforeAnimationStart = num * 25,
                        motion = new Vector2(-0.25f, 0.0f)
                    });
                ++num;
            }
            return;
        }

        private void customTotemWarpForReal()
        {
            Game1.warpFarmer("Forest", 14, 82, false);
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.player.temporarilyInvincible = false;
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
            return;
        }
    }
}