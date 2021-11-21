using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACNHPoker
{
    public partial class variationSpawn : Form
    {
        inventorySlot[,] mainSlot = new inventorySlot[1 ,3];
        inventorySlot[,] subSlot = new inventorySlot[1, 3];
        inventorySlot[,] allSlot = new inventorySlot[2, 3];

        inventorySlot[,] mainHSlot = new inventorySlot[3, 1];
        inventorySlot[,] subHSlot = new inventorySlot[3, 1];
        inventorySlot[,] allHSlot = new inventorySlot[2, 3];


        int main;
        int sub;

        public variationSpawn(inventorySlot[,] variationList)
        {
            InitializeComponent();
            map.numOfColumn = (int)columnBox.Value;
            map.numOfRow = (int)rowBox.Value;

            mainSlot[0, 0] = main00;
            mainSlot[0, 1] = main01;
            mainSlot[0, 2] = main02;
            subSlot[0, 0] = sub00;
            subSlot[0, 1] = sub01;
            subSlot[0, 2] = sub02;
            allSlot[0, 0] = all00;
            allSlot[0, 1] = all01;
            allSlot[0, 2] = all02;
            allSlot[1, 0] = all10;
            allSlot[1, 1] = all11;
            allSlot[1, 2] = all12;

            mainHSlot[0, 0] = mainH00;
            mainHSlot[1, 0] = mainH01;
            mainHSlot[2, 0] = mainH02;
            subHSlot[0, 0] = subH00;
            subHSlot[1, 0] = subH01;
            subHSlot[2, 0] = subH02;

            allHSlot[0, 0] = allH00;
            allHSlot[0, 1] = allH01;
            allHSlot[0, 2] = allH02;
            allHSlot[1, 0] = allH10;
            allHSlot[1, 1] = allH11;
            allHSlot[1, 2] = allH12;


            main = variationList.GetLength(0);
            sub = variationList.GetLength(1);

            if (main <= 1)
            {
                mainOnly.Enabled = false;
                mainPanel.Enabled = false;
                all.Enabled = false;
                allPanel.Enabled = false;

                mainOnly.Checked = false;
                subOnly.Checked = true;
                okBtn.DialogResult = DialogResult.Yes;

                mainHOnly.Enabled = false;
                mainHPanel.Enabled = false;
                allH.Enabled = false;
                allHPanel.Enabled = false;

                mainHOnly.Checked = false;
                subHOnly.Checked = true;
                okHBtn.DialogResult = DialogResult.No;
            }
            else if (sub <= 1)
            {
                subOnly.Enabled = false;
                subPanel.Enabled = false;
                all.Enabled = false;
                allPanel.Enabled = false;

                mainOnly.Checked = true;
                subOnly.Checked = false;
                okBtn.DialogResult = DialogResult.OK;

                subHOnly.Enabled = false;
                subHPanel.Enabled = false;
                allH.Enabled = false;
                allHPanel.Enabled = false;

                mainHOnly.Checked = true;
                subHOnly.Checked = false;
                okHBtn.DialogResult = DialogResult.Abort;
            }

            //main
            for (int i = 0; i < main; i++)
            {
                if (i >= 3)
                    break;
                mainSlot[0, i].setup(variationList[i,0]);
                mainHSlot[i, 0].setup(variationList[i, 0]);
            }

            //sub
            for (int j = 0; j < sub; j++)
            {
                if (j >= 3)
                    break;
                subSlot[0, j].setup(variationList[0, j]);
                subHSlot[j, 0].setup(variationList[0, j]);
            }

            for (int m = 0; m < main; m++)
            {
                if (m >= 2)
                    break;
                for (int n = 0; n < sub; n++)
                {
                    if (n >= 3)
                        continue;
                    allSlot[m, n].setup(variationList[m, n]);
                }
            }

            for (int m = 0; m < main; m++)
            {
                if (m >= 3)
                    break;
                for (int n = 0; n < sub; n++)
                {
                    if (n >= 2)
                        continue;
                    allHSlot[n, m].setup(variationList[m, n]);
                }
            }

            updateSize();
        }

        private void mainOnly_CheckedChanged(object sender, EventArgs e)
        {
            okBtn.DialogResult = DialogResult.OK; // 1
            columnPanel.Enabled = true;
            updateSize();
        }

        private void subOnly_CheckedChanged(object sender, EventArgs e)
        {
            okBtn.DialogResult = DialogResult.Yes; // 6
            columnPanel.Enabled = true;
            updateSize();
        }

        private void all_CheckedChanged(object sender, EventArgs e)
        {
            okBtn.DialogResult = DialogResult.Ignore; // 5
            columnPanel.Enabled = false;
            updateSize();
        }

        private void mainHOnly_CheckedChanged(object sender, EventArgs e)
        {
            okHBtn.DialogResult = DialogResult.Abort; // 3
            rowPanel.Enabled = true;
            updateSize();
        }

        private void subHOnly_CheckedChanged(object sender, EventArgs e)
        {
            okHBtn.DialogResult = DialogResult.No; // 7
            rowPanel.Enabled = true;
            updateSize();
        }

        private void allH_CheckedChanged(object sender, EventArgs e)
        {
            okHBtn.DialogResult = DialogResult.Retry; // 4
            rowPanel.Enabled = false;
            updateSize();
        }

        private void columnBox_ValueChanged(object sender, EventArgs e)
        {
            map.numOfColumn = (int)columnBox.Value;
            timesLabel1.Text = "× " + columnBox.Value;
            timesLabel2.Text = "× " + columnBox.Value;
            updateSize();
        }
        private void rowBox_ValueChanged(object sender, EventArgs e)
        {
            map.numOfRow = (int)rowBox.Value;
            timesHLabel1.Text = "× " + rowBox.Value;
            timesHLabel2.Text = "× " + rowBox.Value;
            updateSize();
        }

        private void updateSize()
        {
            if (mainOnly.Checked)
            {
                size.Text = main.ToString() + " × " + columnBox.Value;
            }
            else if (subOnly.Checked)
            {
                size.Text = sub.ToString() + " × " + columnBox.Value;
            }
            else
            {
                size.Text = sub.ToString() + " × " + main.ToString();
            }

            if (mainHOnly.Checked)
            {
                sizeH.Text = rowBox.Value + " × " + main.ToString();
            }
            else if (subHOnly.Checked)
            {
                sizeH.Text = rowBox.Value + " × " + sub.ToString();
            }
            else
            {
                sizeH.Text = main.ToString() + " × " + sub.ToString();
            }
        }

        private void toggleBtn_Click(object sender, EventArgs e)
        {
            if (toggleBtn.Tag.ToString().Equals("Vertical"))
            {
                toggleBtn.Tag = "Horizontal";
                toggleBtn.Text = "Horizontal";
                toggleBtn.BackColor = Color.Orange;
                horiPanel.Visible = true;
                vertPanel.Visible = false;
            }
            else
            {
                toggleBtn.Tag = "Vertical";
                toggleBtn.Text = "Vertical";
                toggleBtn.BackColor = Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
                horiPanel.Visible = false;
                vertPanel.Visible = true;
            }
        }
    }
}
