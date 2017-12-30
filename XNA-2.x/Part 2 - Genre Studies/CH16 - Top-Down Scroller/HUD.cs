// HUD.cs
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter16
{
    public class HUD
    {
        // texture rectangles for the HUD (Heads Up Display) components
        protected static Rectangle m_RectPressStart = 
            new Rectangle(544, 320, 160, 32);
        protected static Rectangle m_RectHealth = 
            new Rectangle(672, 288, 32, 32);
        protected static Rectangle m_RectShield = m_RectHealth;

        // flash between score and "press start" if player's game is over
        protected bool FlashState = true;
        protected double TotalElapsed = 0;
        protected double m_PromptFlashInterval = 1.5;
        public double PromptFlashInterval
        {
            get { return m_PromptFlashInterval; }
            set { m_PromptFlashInterval = value; }
        }

        // update the flash state of the "press start" prompt
        public void Update(double elapsed)
        {
            TotalElapsed += elapsed;
            if (TotalElapsed > PromptFlashInterval)
            {
                TotalElapsed = 0;
                FlashState = !FlashState;
            }
        }

        // actually draw the HUD
        public void Draw(SpriteBatch batch, Player player)
        {
            // top, left of the three health lights
            Vector2 locHealthHUD = player.HudPosition;

            // top, left of the three shield lights
            Vector2 locShieldHUD = player.HudPosition;
            locShieldHUD.X += 3 * 32;

            // top, left of the score text and "press start" prompt
            Vector2 locPressStart = player.HudPosition;
            locPressStart.X += 16;
            locPressStart.Y += 32;

            if (FlashState && !player.IsActive)
            {
                // alternate between score and prompt, 
                // as long as player's game is over
                batch.Draw(Game1.Texture, locPressStart, 
                    m_RectPressStart, Color.White);
            }
            else
            {
                // if player is active, draw their score every time
                player.NumberSprite.Draw(batch, locPressStart);
            }

            // render the health and shield lights
            for (int i = 0; i < 3; i++)
            {
                // render the shield lights
                if (player.Shield > i && player.IsActive)
                {
                    // active! draw green light
                    batch.Draw(Game1.Texture, locShieldHUD, 
                        m_RectShield, Color.PaleGreen);
                }
                else
                {
                    // inactive, draw dim light
                    batch.Draw(Game1.Texture, locShieldHUD, 
                        m_RectShield, Color.LightSlateGray);
                }

                // render the health lights
                if (player.Health > i && player.IsActive)
                {
                    // active! draw red light
                    batch.Draw(Game1.Texture, locHealthHUD, 
                        m_RectHealth, Color.LightCoral);
                }
                else
                {
                    // inactive, draw dim light
                    batch.Draw(Game1.Texture, locHealthHUD, 
                        m_RectHealth, Color.LightSlateGray);
                }

                // move to the next light's location
                locShieldHUD.X += 32;
                locHealthHUD.X += 32;
            }
        }
    }
}
