using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CH16___Top_Down_Scroller
{
    public class GameObjectManager
    {
        // these arrays hold all the available game objects.
        // objects are marked as active (on screen) or inactive
        // (available for use). by using a pool of objects, we
        // don't need to keep creating and destroying objects,
        // which should make the garbage collector happier.
        protected static Bullet[] m_Bullets = new Bullet[300];
        protected static Bonus[] m_Bonuses = new Bonus[20];
        protected static Enemy[] m_Enemies = new Enemy[20];
        protected static Splat[] m_Splats = new Splat[20];

        // initialized all game objects
        public static void Init()
        {
            InitBullets();
            InitBonuses();
            InitEnemies();
            InitSplats();
        }

        // initialize all bullets
        protected static void InitBullets()
        {
            for (int i = 0; i < m_Bullets.Length; i++)
            {
                m_Bullets[i] = new Bullet();
            }
        }

        // initialize all bonuses
        protected static void InitBonuses()
        {
            for (int i = 0; i < m_Bonuses.Length; i++)
            {
                m_Bonuses[i] = new Bonus();
            }
        }

        // initialize all enemies
        protected static void InitEnemies()
        {
            for (int i = 0; i < m_Enemies.Length; i++)
            {
                m_Enemies[i] = new Enemy();
            }
        }

        // initialize all splats
        protected static void InitSplats()
        {
            for (int i = 0; i < m_Splats.Length; i++)
            {
                m_Splats[i] = new Splat();
            }
        }

        // take an inactive bullet from the pool and add it to the screen
        public static void AddBullet(GameSprite source, bool up)
        {
            // for each bullet
            for (int i = 0; i < m_Bullets.Length; i++)
            {
                // is this bullet inactive?
                if (!m_Bullets[i].IsActive)
                {
                    // set bullet's direction
                    m_Bullets[i].MoveUp = up;

                    // set bullet's position, based on originator
                    Vector2 pos = Vector2.Zero;
                    pos.X = source.Location.X + source.TextureRect.Width / 2;
                    pos.X -= m_Bullets[i].TextureRect.Width / 2;
                    pos.Y = source.Location.Y + source.TextureRect.Height / 2;
                    pos.Y -= m_Bullets[i].TextureRect.Height / 2;

                    // add bullet to the screen
                    m_Bullets[i].Init();
                    m_Bullets[i].Location = pos;
                    m_Bullets[i].IsActive = true;

                    // if this bullet came from a player, remember
                    // who shot it so that they will get credit if it
                    // hits an enemy
                    if (source.GetType() == typeof(Player))
                    {
                        m_Bullets[i].Shooter = (Player)source;
                    }

                    // we're done, exit the loop
                    break;
                }
            }
        }

        // take an inactive bonus icon from the pool and add it to the screen
        public static void AddBonus(GameSprite source)
        {
            // for each bonus in the pool
            for (int i = 0; i < m_Bonuses.Length; i++)
            {
                // is this bonus inactive?
                if (!m_Bonuses[i].IsActive)
                {
                    // set the bonus' location, based on originator
                    Vector2 pos = Vector2.Zero;
                    pos.X = source.Location.X + source.TextureRect.Width / 2;
                    pos.X -= m_Bonuses[i].TextureRect.Width / 2;
                    pos.Y = source.Location.Y + source.TextureRect.Height / 2;
                    pos.Y -= m_Bonuses[i].TextureRect.Height / 2;

                    // add bonus icon to the screen
                    m_Bonuses[i].Location = pos;
                    m_Bonuses[i].Init();
                    m_Bonuses[i].IsActive = true;

                    // we're done, exit the loop
                    break;
                }
            }
        }

        // take an inactive enemy from the pool and add it to the screen
        public static void AddEnemy()
        {
            // for each enemy in pool
            for (int i = 0; i < m_Enemies.Length; i++)
            {
                // is this enemy inactive?
                if (!m_Enemies[i].IsActive)
                {
                    // add enemy to the screen
                    m_Enemies[i].Init();
                    m_Enemies[i].IsActive = true;

                    // we're done, exit the loop
                    break;
                }
            }
        }

        // take an inactive splat icon from the pool and add it to the screen
        public static void AddSplat(GameSprite source)
        {
            // for each splat icon in the pool
            for (int i = 0; i < m_Splats.Length; i++)
            {
                // is this splat inactive?
                if (!m_Splats[i].IsActive)
                {
                    // set the splat's location, based on originator
                    Vector2 pos = Vector2.Zero;
                    pos.X = source.Location.X + source.TextureRect.Width / 2;
                    pos.X -= m_Splats[i].TextureRect.Width / 2;
                    pos.Y = source.Location.Y + source.TextureRect.Height / 2;
                    pos.Y -= m_Splats[i].TextureRect.Height / 2;

                    // add splat icon to the screen
                    m_Splats[i].Location = pos;
                    m_Splats[i].Init();
                    m_Splats[i].IsActive = true;

                    // return originator to the inactive pool
                    source.IsActive = false;

                    // we're done, exit the loop
                    break;
                }
            }
        }

        // update all game objects, check for collisions
        public static void Update(double elapsed)
        {
            // update all game objects
            UpdateBullets(elapsed);
            UpdateBonuses(elapsed);
            UpdateEnemies(elapsed);
            UpdateSplats(elapsed);

            // check for collisions
            CheckForHitEnemies();
            CheckForHitBonuses(Game1.m_PlayerOne);
            CheckForHitBonuses(Game1.m_PlayerTwo);
            CheckForHitPlayer(Game1.m_PlayerOne);
            CheckForHitPlayer(Game1.m_PlayerTwo);
        }

        // animate active bullets
        protected static void UpdateBullets(double elapsed)
        {
            // create a single vector, used for all calculations
            Vector2 delta = Vector2.Zero;
            // for each bullet
            for (int i = 0; i < m_Bullets.Length; i++)
            {
                // is this bullet active?
                if (m_Bullets[i].IsActive)
                {
                    // determine bullet's new position
                    double distance =
                        m_Bullets[i].MovePixelsPerSecond * elapsed;
                    delta.Y =
                        (float)(m_Bullets[i].MoveUp ? -distance : distance);
                    m_Bullets[i].Location += delta;
                    // remove bullet if it moves off the screen
                    ValidateRoughBounds(m_Bullets[i]);
                }
            }
        }

        // animate active bonus icons
        protected static void UpdateBonuses(double elapsed)
        {
            // create a single vector, used for all calculations
            Vector2 delta = Vector2.Zero;
            // for each bonus icon
            for (int i = 0; i < m_Bonuses.Length; i++)
            {
                // is this bonus icon active?
                if (m_Bonuses[i].IsActive)
                {
                    // determine bonus' new position
                    double distance =
                        m_Bonuses[i].MovePixelsPerSecond * elapsed;
                    delta.Y = (float)distance;
                    m_Bonuses[i].Location += delta;
                    // remove bonus icon if it moves off the screen
                    ValidateRoughBounds(m_Bonuses[i]);
                }
            }
        }

        // animate active enemies
        protected static void UpdateEnemies(double elapsed)
        {
            // for each enemy
            for (int i = 0; i < m_Enemies.Length; i++)
            {
                // is this enemy active?
                if (m_Enemies[i].IsActive)
                {
                    // have enemy update itself
                    m_Enemies[i].Update(elapsed);
                    // remove enemy if it moves off the screen
                    ValidateRoughBounds(m_Enemies[i]);
                }
            }
        }

        // update active splat icons
        protected static void UpdateSplats(double elapsed)
        {
            // for each splat icon
            for (int i = 0; i < m_Splats.Length; i++)
            {
                // is this splat active?
                if (m_Splats[i].IsActive)
                {
                    // have splat update itself
                    m_Splats[i].Update(elapsed);
                    // remove splat if it moves off the screen
                    ValidateRoughBounds(m_Splats[i]);
                }
            }
        }

        // this method provides a quick and dirty way to tell
        // when a game object has left the screen. when it does,
        // we can return it to the pool of available game objects
        public static void ValidateRoughBounds(GameSprite sprite)
        {
            if (sprite.Location.X < -32) // too far left
            {
                sprite.IsActive = false;
            }
            else if (sprite.Location.X > 640) // too far right
            {
                sprite.IsActive = false;
            }

            if (sprite.Location.Y < -32) // too far up
            {
                sprite.IsActive = false;
            }
            else if (sprite.Location.Y > 480) // too far down
            {
                sprite.IsActive = false;
            }
        }

        // see if any enemies have been hit by player bullets
        protected static void CheckForHitEnemies()
        {
            // for each enemy
            for (int iEnemy = 0; iEnemy < m_Enemies.Length; iEnemy++)
            {
                // is this enemy active?
                if (m_Enemies[iEnemy].IsActive)
                {
                    // for each bullet
                    for (int iBullet = 0; iBullet < m_Bullets.Length; iBullet++)
                    {
                        // is this bullet active and did it originate
                        // from a player?
                        if (m_Bullets[iBullet].IsActive &&
                            m_Bullets[iBullet].MoveUp)
                        {
                            // is this bullet touching the enemy?
                            if (PixelPerfectHelper.DetectCollision(
                                m_Enemies[iEnemy], m_Bullets[iBullet]))
                            {
                                // create a new random bonus at the 
                                // destroyed enemy's location
                                AddBonus(m_Enemies[iEnemy]);
                                // create a splat icon to represent
                                // an explosion at enemy's location
                                AddSplat(m_Enemies[iEnemy]);

                                // was this bullet fired by a player?
                                if (m_Bullets[iBullet].Shooter != null)
                                {
                                    // add 5 points to player's score ...
                                    m_Bullets[iBullet].Shooter.Score += 5;
                                    // ... and allow them to shoot again 
                                    // immediately (reset the fire delay)
                                    m_Bullets[iBullet].Shooter.ResetFireDelay();
                                }
                                // return this bullet to the pool of
                                // available bullets
                                m_Bullets[iBullet].IsActive = false;
                            }
                        }
                    }
                }
            }
        }

        // see if a player was hit by any enemy bullets
        protected static void CheckForHitPlayer(Player player)
        {
            // is this player active?
            if (player.IsActive)
            {
                // for each bullet
                for (int iBullet = 0; iBullet < m_Bullets.Length; iBullet++)
                {
                    // is this bullet active and did it originate from an enemy?
                    if (m_Bullets[iBullet].IsActive &&
                        !m_Bullets[iBullet].MoveUp)
                    {
                        // is the bullet touching the player?
                        if (PixelPerfectHelper.DetectCollision(
                            player, m_Bullets[iBullet]))
                        {
                            // register the hit with the player
                            player.TakeHit();

                            // if the player was destroyed by the hit,
                            // display a splat icon at their current
                            // location
                            if (!player.IsActive)
                            {
                                AddSplat(player);
                            }

                            // return this bullet to the pool of available
                            // bullets
                            m_Bullets[iBullet].IsActive = false;
                        }
                    }
                }

                // check for a collision between player and enemy aircraft
                for (int iEnemy = 0; iEnemy < m_Enemies.Length; iEnemy++)
                {
                    // is this enemy active?
                    if (m_Enemies[iEnemy].IsActive)
                    {
                        // is this enemy touching the player?
                        if (PixelPerfectHelper.DetectCollision(
                            player, m_Enemies[iEnemy]))
                        {
                            // register the hit with the player
                            player.TakeHit();

                            // add a new splat icon for the enemy plane
                            AddSplat(m_Enemies[iEnemy]);

                            // if the player was destroyed, display a
                            // splat icon at their location as well
                            if (!player.IsActive)
                            {
                                AddSplat(player);
                            }
                        }
                    }
                }
            }
        }

        // see if the player is touching any bonus icons
        protected static void CheckForHitBonuses(Player player)
        {
            // is this player active?
            if (player.IsActive)
            {
                // for each bonus icon object
                for (int iBonus = 0; iBonus < m_Bonuses.Length; iBonus++)
                {
                    // is this bonus active?
                    if (m_Bonuses[iBonus].IsActive)
                    {
                        // is this bonus touching the player?
                        if (PixelPerfectHelper.DetectCollision(
                            player, m_Bonuses[iBonus]))
                        {
                            // take action based on bonus type
                            switch (m_Bonuses[iBonus].BonusType)
                            {
                                case Bonus.Type.Health: // red
                                    // increase player's health
                                    player.Health += 1;
                                    break;
                                case Bonus.Type.Score: // blue
                                    // add bonus points to player score
                                    player.Score += 25;
                                    break;
                                case Bonus.Type.Shield: // green
                                    // increase player's shield
                                    player.Shield += 1;
                                    break;
                                case Bonus.Type.Weapon: // yellow
                                    // reduce delay between shots
                                    player.FireDelay -= 0.2 * player.FireDelay;
                                    break;
                            }

                            // return this bonus to the pool of available
                            // bonsuses
                            m_Bonuses[iBonus].IsActive = false;
                        }
                    }
                }
            }
        }

        // draw all active game objects
        public static void Draw(SpriteBatch batch)
        {
            DrawBullets(batch);
            DrawBonuses(batch);
            DrawEnemies(batch);
            DrawSplats(batch);
        }

        // draw all active bullets
        protected static void DrawBullets(SpriteBatch batch)
        {
            for (int i = 0; i < m_Bullets.Length; i++)
            {
                if (m_Bullets[i].IsActive)
                {
                    m_Bullets[i].Draw(batch);
                }
            }
        }

        // draw all active bonuses
        protected static void DrawBonuses(SpriteBatch batch)
        {
            for (int i = 0; i < m_Bonuses.Length; i++)
            {
                if (m_Bonuses[i].IsActive)
                {
                    m_Bonuses[i].Draw(batch);
                }
            }
        }

        // draw all active enemies
        protected static void DrawEnemies(SpriteBatch batch)
        {
            for (int i = 0; i < m_Enemies.Length; i++)
            {
                if (m_Enemies[i].IsActive)
                {
                    m_Enemies[i].Draw(batch);
                }
            }
        }

        // draw all active splats
        protected static void DrawSplats(SpriteBatch batch)
        {
            for (int i = 0; i < m_Splats.Length; i++)
            {
                if (m_Splats[i].IsActive)
                {
                    m_Splats[i].Draw(batch);
                }
            }
        }
    }
}
