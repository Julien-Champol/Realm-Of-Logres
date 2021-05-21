﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BaseSim2021
{
    public partial class GameView : Form
    {
        private readonly WorldState theWorld;
        List<IndexedValueView> polViews;
        List<IndexedValueView> groupsViews;

        /// <summary>
        /// The constructor for the main window
        /// </summary>
        public GameView(WorldState world)
        {
            InitializeComponent();
            theWorld = world;
        }

        /// <summary>
        /// Method called by the controler whenever some text should be displayed
        /// </summary>
        /// <param name="s"></param>
        public void WriteLine(string s)
        {
            List<string> strs = s.Split('\n').ToList();
            strs.ForEach(str => outputListBox.Items.Add(str));
            if (outputListBox.Items.Count > 0)
            {
                outputListBox.SelectedIndex = outputListBox.Items.Count - 1;
            }
            outputListBox.Refresh();
        }

        /// <summary>
        /// Method called by the controler whenever a confirmation should be asked
        /// </summary>
        /// <returns>Yes iff confirmed</returns>
        public bool ConfirmDialog()
        {
            string message = "Confirmer ?";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            return MessageBox.Show(message, "", buttons) == DialogResult.Yes;
        }
        #region Event handling
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                e.SuppressKeyPress = true; // Or beep.
                GameController.Interpret(inputTextBox.Text);
            }
        }

        /// <summary>
        /// The method handling the MouseDown event of the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //If the cursor is placed on one of the IndexedValueView displayed.
            {
                IndexedValue policy = Sélection(e.Location)?.IndexedValue;
                PolicyModification policyModification = new PolicyModification(policy);
                if (policyModification.ShowDialog() == DialogResult.OK)
                {
                    policy.Influence(policyModification.Value);
                    policy.Update();
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Returns the policy corresponding to the mouse's location, null if there 
        /// is no such policy.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private IndexedValueView Sélection(Point p)
        {
            if (polViews.FirstOrDefault(c => c.Contient(p)) != null)
            {
                return polViews.FirstOrDefault(c => c.Contient(p));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The paint method of the main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameView_Paint(object sender, PaintEventArgs e)
        {
            diffLabel.Text = "Difficulté : " + theWorld.TheDifficulty;
            turnLabel.Text = "Tour " + (theWorld.Turns + 1); //ou turns + 1 ?
            moneyLabel.Text = "Trésor : " + theWorld.Money + " pièces d'or";
            gloryLabel.Text = "Gloire : " + theWorld.Glory;
            nextButton.Visible = true;
            ListInitialization();
            ViewsDisplay(e.Graphics);
        }
        #endregion

        private void NextButton_Click(object sender, EventArgs e)
        {
            GameController.Interpret("suivant");
        }

        /// <summary>
        /// Method called from the GameController class used to display a MessageBox in a Losing situation.
        /// </summary>
        /// <param name="indexedValue"></param>
        public void LoseDialog(IndexedValue indexedValue)
        {
            if (indexedValue == null)
            {
                MessageBox.Show("Partie perdue : dette insurmontable.");
            }
            else
            {
                MessageBox.Show("Partie perdue : "
                + indexedValue.CompletePresentation());
            }
            nextButton.Enabled = false;
        }

        /// <summary>
        /// Method called from the GameController class used to display a MessageBox in a wining situation.
        /// </summary>
        public void WinDialog()
        {
            MessageBox.Show("Partie gagnée.");
            nextButton.Enabled = false;
        }

        private void gloryLabel_Click(object sender, EventArgs e)
        {

        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        #region Initialization of the lists and screen display of the IndexedValueViews

        /// <summary>
        /// Method called to add all the IndexedValues of a category
        /// to a list of IndexedValueViews so that the coordinates giving is managed here.
        /// </summary>
        public void ListInitialization()
        {
            // POLITICS
            // PolRectangle:0,20, 800, 641; w:80, h:80, margin:10
            Rectangle PolRectangle = new Rectangle(0, 20, 800, 641);
            int margin = 10, w = 80, h = 80;
            int x = PolRectangle.X + margin, y = PolRectangle.Y + margin;
            polViews = new List<IndexedValueView>();
            foreach (IndexedValue p in theWorld.Policies)
            {
                polViews.Add(new IndexedValueView(p, new Point(x, y)));
                x += w + margin;
                if (x > PolRectangle.Right)
                {
                    x = PolRectangle.X + margin;
                    y += h + margin;
                }
            }

            //GROUPS
            // GroupsRectangle:200,20, 800, 641; w:80, h:80, margin:10
            Rectangle GroupsRectangle = new Rectangle(1200, 20, 800, 641);
            int groupsMargin = 10, groupsW = 80, groupsH = 80;
            int groupsX = GroupsRectangle.X + groupsMargin, groupsY = GroupsRectangle.Y + groupsMargin;
            groupsViews = new List<IndexedValueView>();
            foreach (IndexedValue p in theWorld.Groups)
            {
                groupsViews.Add(new IndexedValueView(p, new Point(groupsX, groupsY)));
                groupsX += groupsW + groupsMargin;
                if (groupsX > GroupsRectangle.Right)
                {
                    groupsX = GroupsRectangle.X + groupsMargin;
                    groupsY += groupsH + groupsMargin;
                }
            }
        }

        /// <summary>
        /// Method used to display all the indexed values on screen.
        /// </summary>
        /// <param name="g"></param>
        public void ViewsDisplay(Graphics g)
        {
            foreach (IndexedValueView q in polViews)
            {
                q.IndexedValueView_Draw(g);
            }

            foreach (IndexedValueView q in groupsViews)
            {
                q.IndexedValueView_Draw(g);
            }
        }

        #endregion
    }
}