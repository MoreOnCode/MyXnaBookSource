// BrickBreakerEditor.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Chapter24;

namespace BrickBreakerLevelEditor
{
    public partial class EditorForm : Form
    {
        // the collection of levels for this project
        protected List<LevelData> m_Levels = new List<LevelData>();

        // filename of the currently-loaded project
        protected string m_Filename = null;

        // standard constructor
        public EditorForm()
        {
            InitializeComponent();

            // init the popup dialogs with security-friendly paths
            openFileDialog1.InitialDirectory = Application.UserAppDataPath;
            saveFileDialog1.InitialDirectory = Application.UserAppDataPath;

            // init the popup dialogs with default filter
            openFileDialog1.FileName = "*.bbp";
            saveFileDialog1.FileName = "*.bbp";

            // add a single level to the default, empty project
            AddNewLevel();

            // init the toolbar image with the current HitsToClear selection
            toolHitsToClear.Image = toolStripMenuItem2.Image;
        }

        // toggle the editor grid
        private void cmdDrawGrid_CheckedChanged(object sender, EventArgs e)
        {
            this.brickBreakerEditorControl1.DrawGrid = cmdDrawGrid.Checked;
        }

        // create a new project
        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        // give the user a chance to save any changed work
        // returns true if the action should be canceled, false otherwise
        private bool CheckForChanges()
        {
            bool cancel = false;
            foreach (LevelData level in m_Levels)
            {
                // has the current level been edited?
                if (level.Changed)
                {
                    // offer the user a chance to save their work
                    DialogResult result = MessageBox.Show(
                        "The project has changed. Save changes?",
                        "Save Changes?",
                        MessageBoxButtons.YesNoCancel);

                    // they clicked Yes, save the file
                    if (result == DialogResult.Yes)
                    {
                        cancel = !SaveProject(false);
                    }
                    // they clicked Cancel for the current action
                    else if (result == DialogResult.Cancel)
                    {
                        cancel = true;
                    }
                    
                    // stop searching, save applies to all levels 
                    // in the project; save one, save all
                    break;
                }
            }
            return cancel;
        }

        // add a new level to this project
        protected void AddNewLevel()
        {
            // create and add the level
            LevelData level = new LevelData();
            m_Levels.Add(level);

            // update the navigation tree and select the new level
            RebuildTree();
            SelectNode(level);
        }

        // add a new level to this project, as a copy of another level
        protected void CopyLevel()
        {
            // get a reference to the current tree node
            TreeNode node = treeView1.SelectedNode;

            // is it a valid level?
            if (IsLevelNode(node))
            {
                // create a copy of the level
                LevelData levelSrc = (LevelData)node.Tag;
                LevelData levelNew = new LevelData(levelSrc.ToString());

                // insert the new level just after its source
                int index = m_Levels.IndexOf(levelSrc);
                m_Levels.Insert(index + 1, levelNew);

                // update the navigation tree and select the new level
                RebuildTree();
                SelectNode(levelNew);
            }
        }

        // build the navigation tree, based on the collection of levels
        protected void RebuildTree()
        {
            // disable redraws while updating tree nodes
            treeView1.BeginUpdate();

            // identify the root, and clear all existing child nodes
            TreeNode root = treeView1.Nodes[0];
            root.Nodes.Clear();

            // add a node to the root for every level in our collection
            int num = 1;
            foreach (LevelData level in m_Levels)
            {
                // name incrementally
                TreeNode node = root.Nodes.Add("Level" + num.ToString("00"));

                // store reference to the level in the tag
                node.Tag = level;

                // set the images for the new node
                node.ImageIndex = 2;
                node.SelectedImageIndex = 2;

                // increment the name counter
                num++;
            }
            
            // allow redraws for tree control
            treeView1.EndUpdate();
        }

        // locate the tree node that refers to the given level
        // and select it in our navigation tree control
        protected void SelectNode(LevelData level)
        {
            // locate root node, clear selection
            TreeNode root = treeView1.Nodes[0];
            treeView1.SelectedNode = null;

            // reset editor control's level data reference
            brickBreakerEditorControl1.LevelData = null;

            // scan the navigation tree, looking for the given level
            foreach (TreeNode node in root.Nodes)
            {
                if (node.Tag == level)
                {
                    // simply selecting the node will trigger the 
                    // selection events on the navigation tree control
                    // which will call our handler for AfterSelect
                    // and tell the editor control to display the level
                    // data for the newly-selected node
                    treeView1.SelectedNode = node;

                    // stop looking, we found it
                    break;
                }
            }
        }

        // the root isn't a level, but its children are
        protected bool IsLevelNode(TreeNode node)
        {
            TreeNode root = treeView1.Nodes[0];
            return root.Nodes.Contains(node);
        }

        // a navigation tree view node was selected, update the editor
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            brickBreakerEditorControl1.LevelData = (LevelData)e.Node.Tag;
        }

        // move the selected level node up one slot in the project
        private void toolMoveLevelUp_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (IsLevelNode(node))
            {
                LevelData level = (LevelData)node.Tag;
                int index = m_Levels.IndexOf(level);
                if (index > 0)
                {
                    // modify level order in the collection, rebuild 
                    // the navigation tree, and (re)select the level
                    m_Levels.Remove(level);
                    m_Levels.Insert(index-1, level);
                    RebuildTree();
                    SelectNode(level);
                }
            }
        }

        // move the selected level node down one slot in the project
        private void toolMoveLevelDown_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (IsLevelNode(node))
            {
                LevelData level = (LevelData)node.Tag;
                int index = m_Levels.IndexOf(level);
                if (index < m_Levels.Count - 1)
                {
                    // modify level order in the collection, rebuild 
                    // the navigation tree, and (re)select the level
                    m_Levels.Remove(level);
                    m_Levels.Insert(index + 1, level);
                    RebuildTree();
                    SelectNode(level);
                }
            }
        }

        // remove the selected level from the project
        private void toolDeleteLevel_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (IsLevelNode(node))
            {
                LevelData level = (LevelData)node.Tag;
                if (m_Levels.Contains(level))
                {
                    if (DialogResult.Yes == 
                        MessageBox.Show(
                            "Delete the selected level?", 
                            "Delete Level?", MessageBoxButtons.YesNo))
                    {
                        // remove the level, assume there are no 
                        // more levels in the collection
                        int index = m_Levels.IndexOf(level);
                        m_Levels.Remove(level);
                        level = null;

                        // make sure index isn't out of bounds
                        if (index >= m_Levels.Count)
                        {
                            index = m_Levels.Count - 1;
                        }

                        // if there's at least one level left, select it
                        if (index >= 0)
                        {
                            level = m_Levels[index];
                        }

                        // rebuild our navigation tree and select the level
                        RebuildTree();
                        SelectNode(level);
                    }
                }
            }
        }

        // add a new level to this project's collection
        private void toolAddNewLevel_Click(object sender, EventArgs e)
        {
            AddNewLevel();
        }

        // user has asked us to shutdown
        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // user has requested that we open a project file
        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        // actually add a new, empty level to the project
        protected void NewProject()
        {
            // give user a chance to save any unsaved data
            if (!CheckForChanges())
            {
                // clear the collection, add a single new project
                m_Levels.Clear();
                AddNewLevel();
                m_Filename = null;
            }
        }

        // actually open a project file from disk
        protected void OpenProject()
        {
            // give user a chance to save any unsaved data
            if (!CheckForChanges())
            {
                // prompt user for filename
                if (DialogResult.OK == openFileDialog1.ShowDialog(this))
                {
                    try
                    {
                        // remember the filename, open the file, 
                        // read the first line
                        m_Filename = openFileDialog1.FileName;
                        StreamReader reader = 
                            new StreamReader(
                                new FileStream(m_Filename, FileMode.Open));
                        string line = reader.ReadLine();

                        // clear any existing level data from our 
                        // in-memory collection
                        m_Levels.Clear();

                        // for every line in the file, read it in,
                        // create a level from the data, and add the
                        // newly-created level to our collection
                        while (line != null)
                        {
                            m_Levels.Add(new LevelData(line));
                            line = reader.ReadLine();
                        }

                        // after reading all the lines, close the file
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                    }

                    // rebuild our navigation tree and select the 
                    // first level, if there is one
                    RebuildTree();
                    if (m_Levels.Count > 0)
                    {
                        SelectNode(m_Levels[0]);
                    }
                }
            }
        }
    
        // actualy save the project to disk
        protected bool SaveProject(bool saveAs)
        {
            // report success or failure
            bool saved = false;

            // if this is the first save, prompt the user for a filename
            if (m_Filename == null || saveAs)
            {
                if (DialogResult.OK == saveFileDialog1.ShowDialog(this))
                {
                    m_Filename = saveFileDialog1.FileName;
                }
                else
                {
                    // user canceled the save as
                    return false;
                }
            }

            // if we have a filename, we're ready to save our level data
            if (m_Filename != null)
            {
                try
                {
                    // create the file (or truncate it, if it already exists)
                    StreamWriter writer = 
                        new StreamWriter(
                            new FileStream(m_Filename,FileMode.Create));

                    // write out each level, and reset it's changed flag
                    foreach (LevelData level in m_Levels)
                    {
                        writer.WriteLine(level.ToString());
                        level.Changed = false;
                    }

                    // close the file
                    writer.Flush();
                    writer.Close();

                    // report success to caller
                    saved = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            }

            return saved;
        }

        // user requested a save, and wants to be prompted for the filename
        private void mnuFileSaveAs_Click(object sender, EventArgs e)
        {
            SaveProject(true);
        }

        // user requested a save, and wants to forego the filename propmpt
        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            SaveProject(false);
        }

        // user has selected a new HitsToClear default for new bricks
        // this event is tracked for all 9 HitsToClear toolbar options
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // set the main toolbar icon to the selected item's icon
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            toolHitsToClear.Image = item.Image;

            // update the editor control with the select HitsToClear value
            // the HitsToClear value is encoded in the toolbar item's tag
            // property
            brickBreakerEditorControl1.NumHitsToClear = 
                int.Parse(item.Tag.ToString());
        }

        // user has requested that the editor shutdown 
        // (via menu, X button on title bar, or ALT+F4 keypress)
        private void BrickBreakerEditor_FormClosing(
            object sender, FormClosingEventArgs e)
        {
            // give user a chance to save any unsaved data
            if (CheckForChanges())
            {
                e.Cancel = true;
            }
        }

        // user has requested that we create a new project
        private void toolNew_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        // user has requested that we load a saved project
        private void toolOpen_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        // user has requested that we save the current project
        private void toolSave_Click(object sender, EventArgs e)
        {
            SaveProject(false);
        }

        // user asked us to duplicate the selected level
        private void toolCopyLevel_Click(object sender, EventArgs e)
        {
            CopyLevel();
        }
    }
}