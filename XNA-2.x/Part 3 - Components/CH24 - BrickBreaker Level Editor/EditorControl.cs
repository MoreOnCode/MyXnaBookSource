// BrickBreakerEditorControl.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Chapter24;

namespace BrickBreakerLevelEditor
{
    public partial class EditorControl : UserControl
    {
        // default constructor
        public EditorControl()
        {
            InitializeComponent();

            // init the main HitsToClear menu icon with the icon
            // of the default HitsToClear selection
            mnuNumHits.Image = toolStripMenuItem4.Image;
        }

        // the level data that we're currently editing
        protected LevelData m_LevelData = new LevelData();
        public LevelData LevelData
        {
            get { return m_LevelData; }
            set
            {
                m_LevelData = value;
                
                // the level changed, redraw our edit window
                this.Invalidate();
            }
        }

        // the HitsToClear value to be used with new bricks
        protected int m_NumHitsToClear = 1;
        public int NumHitsToClear
        {
            get { return m_NumHitsToClear; }
            set { m_NumHitsToClear = value; }
        }

        // enable and disable the grid snap feature
        protected bool m_DrawGrid = true;
        public bool DrawGrid
        {
            get { return m_DrawGrid; }
            set { m_DrawGrid = value; this.Invalidate(); }
        }

        // used when no level is actively being edited
        protected Font m_font = new Font(FontFamily.GenericSansSerif, 12.0f);
        protected const string 
            MSG_NO_LEVEL = "[No data. Select a level to edit.]";

        // bounds of the game screen, and the playable area
        protected Rectangle m_ScreenRect = new Rectangle(0, 0, 640, 480);
        protected Rectangle m_GameRect = new Rectangle(40, 40, 400, 400);

        // update our editor window
        private void BrickBreakerEditorControl_Paint(
            object sender, PaintEventArgs e)
        {
            // local temp variable to save some typing
            Graphics g = e.Graphics;

            // if there is no level selected, draw a generic message
            if (LevelData == null)
            {
                SizeF dimensions = g.MeasureString(MSG_NO_LEVEL, m_font);
                PointF loc = new PointF(
                    this.Width / 2.0f - dimensions.Width / 2.0f,
                    this.Height / 2.0f - dimensions.Height / 2.0f);
                g.DrawString(
                    MSG_NO_LEVEL, 
                    m_font, 
                    SystemBrushes.ControlText, 
                    loc);
            }
            // looks like we have a level to edit, render its current state
            else
            {
                DrawLevelBorder(g);
                DrawLevelGrid(g);
                DrawLevelData(g, LevelData);
                DrawGhostBrick(g, LevelData);
            }
        }

        // fill in the screen and playable areas
        protected void DrawLevelBorder(Graphics g)
        {
            g.FillRectangle(SystemBrushes.ControlDark, m_ScreenRect);
            g.FillRectangle(Brushes.White, m_GameRect);
        }

        // draw the grid, if enabled
        protected void DrawLevelGrid(Graphics g)
        {
            // is the grid enabled?
            if (DrawGrid)
            {
                // calc change in y from grid cell to grid cell, draw lines
                int dy = LevelData.BrickHeight / 2;
                for (int y = m_GameRect.Top; y <= m_GameRect.Bottom; y += dy)
                {
                    g.DrawLine(
                        Pens.Black,
                        m_GameRect.Left,
                        y,
                        m_GameRect.Right,
                        y);
                }

                // calc change in x from grid cell to grid cell, draw lines
                int dx = LevelData.BrickWidth / 2;
                for (int x = m_GameRect.Left; x <= m_GameRect.Right; x += dx)
                {
                    g.DrawLine(
                        Pens.Black,
                        x,
                        m_GameRect.Top,
                        x,
                        m_GameRect.Bottom);
                }
            }
        }

        // create brushes for each of our brick colors
        protected SolidBrush[] m_BrickBrushes = 
        {
            new SolidBrush(Color.FromArgb(255,128,128)), // 1 hit
            new SolidBrush(Color.FromArgb(128,255,128)), // 2 hits
            new SolidBrush(Color.FromArgb(128,128,255)), // 3 hits
            new SolidBrush(Color.FromArgb(255,128,255)), // 4 hits
            new SolidBrush(Color.FromArgb(255,255,128)), // 5 hits
            new SolidBrush(Color.FromArgb(255,194,129)), // 6 hits
            new SolidBrush(Color.FromArgb(192,192,192)), // 7 hits
            new SolidBrush(Color.FromArgb(255,192,192)), // 8 hits
            new SolidBrush(Color.FromArgb(192,255,255)), // 9 hits
        };

        // draw the bricks
        protected void DrawLevelData(Graphics g, LevelData level)
        {
            foreach (Brick b in level.Bricks)
            {
                g.FillRectangle( // fill brick
                    m_BrickBrushes[b.HitsToClear - 1], 
                    m_GameRect.Left + b.X, 
                    m_GameRect.Top + b.Y, 
                    level.BrickWidth, 
                    level.BrickHeight);
                g.DrawRectangle( // outline brick
                    Pens.Black, 
                    m_GameRect.Left + b.X, 
                    m_GameRect.Top + b.Y, 
                    level.BrickWidth, 
                    level.BrickHeight);
            }
        }

        // position of new brick, based on grid settings
        protected int m_NewBrickX = -1;
        protected int m_NewBrickY = -1;
        
        // calc position of new brick, based on grid settings
        // called whenever the mouse moves, while over our editor control
        protected void CalcNewBrickXY(Point location)
        {
            // make sure mouse is within the playable area and 
            // that it's not over an existing brick
            if (m_GameRect.Contains(location) && m_ClickedBrick == null)
            {
                // determine raw top, left for brick
                m_NewBrickX = location.X - m_GameRect.Left;
                m_NewBrickY = location.Y - m_GameRect.Top;

                // snap to grid?
                if (DrawGrid)
                {
                    // snap to nearest grid cell's top, left
                    m_NewBrickX = 
                        (m_NewBrickX / LevelData.HalfWidth) * 
                        LevelData.HalfWidth;
                    m_NewBrickY = 
                        (m_NewBrickY / LevelData.HalfHeight) * 
                        LevelData.HalfHeight;
                }

                // mske sure that the new brick won't extend 
                // outside of the playable area
                m_NewBrickX = 
                    Math.Min(
                        m_NewBrickX, 
                        m_GameRect.Width - LevelData.BrickWidth);
                m_NewBrickY = 
                    Math.Min(
                        m_NewBrickY, 
                        m_GameRect.Height - LevelData.BrickHeight);

                // since the intended location may have changed (due to 
                // grid snap or enforcement of playable bounds), make 
                // sure that the new brick isn't about to sit on top of
                // an existing brick
                bool collide = 
                    LevelData.FindBrick(m_NewBrickX, m_NewBrickY) != null;
                collide |= 
                    LevelData.FindBrick(
                        m_NewBrickX + LevelData.BrickWidth - 1, 
                        m_NewBrickY) != null;
                collide |= 
                    LevelData.FindBrick(
                        m_NewBrickX, 
                        m_NewBrickY + LevelData.BrickHeight - 1) != null;
                collide |= 
                    LevelData.FindBrick(
                        m_NewBrickX + LevelData.BrickWidth - 1, 
                        m_NewBrickY + LevelData.BrickHeight - 1) != null;

                // did we collide with another brick?
                if (collide)
                {
                    // disallow placement of new brick
                    m_NewBrickX = -1;
                    m_NewBrickY = -1;
                }

                // redraw the editor so that we can see the ghosted brick
                this.Invalidate();
            }
            // mouse is not within the playable area
            else
            {
                // avoid repainting when the mouse moves outside of the 
                // playable area or moves within an existing brick;
                // we have to redraw when it first moves into one of these
                // forbidden positions to get rid of the ghosted brick
                if (m_NewBrickX != -1 || m_NewBrickY != -1)
                {
                    m_NewBrickX = -1;
                    m_NewBrickY = -1;
                    this.Invalidate();
                }
            }
        }

        // need to repaint when the mouse moves so that we 
        // can draw the ghosted brick
        private void BrickBreakerEditorControl_MouseMove(
            object sender, MouseEventArgs e)
        {
            // if we're actually editing a level
            if (LevelData != null)
            {
                // see if mouse is over an existing brick
                // brick data is zero-based, so we subtract the top, left
                // of the playable area to translate to a 0,0 origin
                m_ClickedBrick = 
                    LevelData.FindBrick(
                    e.X - m_GameRect.Left, 
                    e.Y - m_GameRect.Top);

                // apply grid settings and bounds checking
                CalcNewBrickXY(e.Location);
            }
        }

        // reference to selected brick for context menu (right-click popup)
        // and by logic to deny placement over existing bricks
        protected Brick m_ClickedBrick = null;

        // process mouse clicks
        private void BrickBreakerEditorControl_MouseUp(
            object sender, MouseEventArgs e)
        {
            // if we're actually editing a level
            if (LevelData != null)
            {
                // right-clicked an existing brick, show popup menu
                if (m_ClickedBrick != null && e.Button == MouseButtons.Right)
                {
                    // get HitsToClear for the selected brick
                    string hits = m_ClickedBrick.HitsToClear.ToString();

                    // find the corresponding menu item and set the 
                    // top-level HitsToClear menu's icon to the selected
                    // child menu item's icon
                    foreach (
                        ToolStripMenuItem item in mnuNumHits.DropDownItems)
                    {
                        if (item.Tag.ToString() == hits)
                        {
                            mnuNumHits.Image = item.Image;
                            break;
                        }
                    }

                    // we're ready, show the popup menu
                    this.contextMenuStrip1.Show(MousePosition);
                }

                // left-clicked empty space; try to add a new brick; redraw
                if (m_ClickedBrick == null && e.Button == MouseButtons.Left)
                {
                    LevelData.AddBrick(
                        m_NewBrickX, 
                        m_NewBrickY, 
                        NumHitsToClear);
                    this.Invalidate();
                }
            }
        }

        // create brush for our ghosted brick
        protected Brush m_GhostBrush = 
            new HatchBrush(
                HatchStyle.Percent30, 
                SystemColors.Window, 
                SystemColors.ControlText);

        // draw "ghosted" brick where the new brick would be located
        protected void DrawGhostBrick(Graphics g, LevelData level)
        {
            if (m_NewBrickX >= 0 && m_NewBrickY >= 0)
            {
                g.FillRectangle( // fill brick
                    m_GhostBrush, 
                    m_GameRect.Left + m_NewBrickX, 
                    m_GameRect.Top + m_NewBrickY, 
                    level.BrickWidth, 
                    level.BrickHeight);
                g.DrawRectangle( // outline brick
                    Pens.Black, 
                    m_GameRect.Left + m_NewBrickX, 
                    m_GameRect.Top + m_NewBrickY, 
                    level.BrickWidth, 
                    level.BrickHeight);
            }
        }

        // user requested that we remove the selected brick
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelData.DeleteBrick(m_ClickedBrick);
            this.Invalidate();
        }

        // user changed the HitsToClear value for the selected brick
        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            // has a brick been selected?
            if (m_ClickedBrick != null)
            {
                // update brick's HitsToClear with the menu item's value
                ToolStripMenuItem item = (ToolStripMenuItem)sender;
                m_ClickedBrick.HitsToClear = int.Parse(item.Tag.ToString());
                this.Invalidate();
            }
        }
    }
}
