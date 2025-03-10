﻿using NAudio.Wave;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPoker
{
    public partial class Form1 : Form
    {
        private string UpdateTownID()
        {
            byte[] townID = Utilities.GetTownID(s, null);
            IslandName = Utilities.GetString(townID, 0x04, 10);
            return "  |  Island Name : " + IslandName;
        }

        private void readWeatherSeed()
        {
            byte[] b = Utilities.GetWeatherSeed(s, bot);
            string result = Utilities.ByteToHexString(b);
            UInt32 decValue = Convert.ToUInt32(Utilities.flip(result), 16);
            UInt32 Seed = decValue - 2147483648;
            SeedTextbox.Text = Seed.ToString();
        }

        private void eatBtn_Click(object sender, EventArgs e)
        {
            Utilities.setStamina(s, bot, "0A");

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();

            setEatBtn();
        }

        private void poopBtn_Click(object sender, EventArgs e)
        {
            Utilities.setStamina(s, bot, "00");

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void setEatBtn()
        {
            eatBtn.Visible = true;
            Random rnd = new Random();
            int dice = rnd.Next(1, 8);

            switch (dice)
            {
                case 1:
                    eatBtn.Text = "Eat 10 Apples";
                    break;
                case 2:
                    eatBtn.Text = "Eat 10 Oranges";
                    break;
                case 3:
                    eatBtn.Text = "Eat 10 Cherries";
                    break;
                case 4:
                    eatBtn.Text = "Eat 10 Pears";
                    break;
                case 5:
                    eatBtn.Text = "Eat 10 Peaches";
                    break;
                case 6:
                    eatBtn.Text = "Eat 10 Coconuts";
                    break;
                default:
                    eatBtn.Text = "Eat 10 Turnips";
                    break;
            }
        }

        private void selectedItem_Click(object sender, EventArgs e)
        {
            if (selectedButton == null)
            {
                int Slot = findEmpty();
                if (Slot > 0)
                {
                    selectedSlot = Slot;
                    updateSlot(Slot);
                }
            }

            if (currentPanel == itemModePanel)
            {
                customIdBtn_Click(sender, e);
            }
            else if (currentPanel == recipeModePanel)
            {
                spawnRecipeBtn_Click(sender, e);
            }
            else if (currentPanel == flowerModePanel)
            {
                spawnFlowerBtn_Click(sender, e);
            }
            int firstSlot = findEmpty();
            if (firstSlot > 0)
            {
                selectedSlot = firstSlot;
                updateSlot(firstSlot);
            }
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void updateSlot(int select)
        {
            foreach (inventorySlot btn in this.inventoryPanel.Controls.OfType<inventorySlot>())
            {
                if (btn.Tag == null)
                    continue;
                btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
                if (int.Parse(btn.Tag.ToString()) == select)
                {
                    selectedButton = btn;
                    btn.BackColor = System.Drawing.Color.LightSeaGreen;
                }
            }
        }

        public void updateSelectedItemInfo(string Name, string ID, string Data)
        {
            selectedItemName.Text = Name;
            selectedID.Text = ID;
            selectedData.Text = Data;
            selectedFlag1.Text = selectedItem.getFlag1();
            selectedFlag2.Text = selectedItem.getFlag2();
        }

        public string GetNameFromID(string itemID, DataTable source)
        {
            if (source == null)
            {
                return "";
            }

            DataRow row = source.Rows.Find(itemID);

            if (row == null)
            {
                return ""; //row not found
            }
            else
            {
                //row found set the index and find the name
                return (string)row[languageSetting];
            }
        }

        public static string GetImagePathFromID(string itemID, DataTable source, UInt32 data = 0)
        {
            if (source == null)
            {
                return "";
            }

            DataRow row = source.Rows.Find(itemID);
            DataRow VarRow = null;
            if (variationSource != null)
                VarRow = variationSource.Rows.Find(itemID);

            if (row == null)
            {
                return ""; //row not found
            }
            else
            {

                string path;
                if (VarRow != null & source != recipeSource)
                {
                    string main = (data & 0xF).ToString();
                    string sub = (((data & 0xFF) - (data & 0xF)) / 0x20).ToString();
                    //Debug.Print("data " + data.ToString("X") + " Main " + main + " Sub " + sub);
                    path = Utilities.imagePath + VarRow["iName"] + "_Remake_" + main + "_" + sub + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }

                    path = Utilities.imagePath + VarRow["iName"] + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }

                string imageName = row[1].ToString();

                if (OverrideDict.ContainsKey(imageName))
                {
                    path = Utilities.imagePath + OverrideDict[imageName] + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }

                path = Utilities.imagePath + imageName + "_Remake_0_0.png";
                if (File.Exists(path))
                {
                    return path;
                }
                else
                {
                    path = Utilities.imagePath + imageName + ".png";
                    if (File.Exists(path))
                    {
                        return path;
                    }
                    else
                    {
                        path = Utilities.imagePath + removeNumber(imageName) + ".png";
                        if (File.Exists(path))
                        {
                            return path;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
        }

        private void copyItemBtn_Click(object sender, EventArgs e)
        {
            /*
            if ((s == null || s.Connected == false) & bot == null)
            {
                MessageBox.Show("Please connect to the switch first");
                return;
            }
            */

            ToolStripItem item = (sender as ToolStripItem);
            if (item != null)
            {
                if (item.Owner is ContextMenuStrip owner)
                {
                    itemModeBtn_Click(sender, e);
                    if (hexModeBtn.Tag.ToString() == "Normal")
                    {
                        hexMode_Click(sender, e);
                    }
                    var btn = (inventorySlot)owner.SourceControl;

                    selectedItem.setup(btn);

                    if (selectedItem.fillItemID() == "FFFE")
                    {
                        hexMode_Click(sender, e);
                        customAmountTxt.Text = "";
                        customIdTextbox.Text = "";
                    }
                    else
                    {
                        customAmountTxt.Text = Utilities.precedingZeros(selectedItem.fillItemData(), 8);
                        customIdTextbox.Text = Utilities.precedingZeros(selectedItem.fillItemID(), 4);
                    }

                    string hexValue = Utilities.precedingZeros(customAmountTxt.Text, 8);

                    if (selection != null)
                    {
                        selection.receiveID(Utilities.precedingZeros(selectedItem.fillItemID(), 4), languageSetting, Utilities.precedingZeros(hexValue, 8));
                    }
                    updateSelectedItemInfo(selectedItem.displayItemName(), selectedItem.displayItemID(), selectedItem.displayItemData());
                    if (sound)
                        System.Media.SystemSounds.Asterisk.Play();
                }
            }
        }

        private void wrapItemBtn_Click(object sender, EventArgs e)
        {
            if (wrapSetting.SelectedIndex < 0)
                wrapSetting.SelectedIndex = 0;

            string[] flagBuffer = wrapSetting.SelectedItem.ToString().Split(' ');
            byte flagByte = Utilities.stringToByte(flagBuffer[flagBuffer.Length - 1])[0];

            ToolStripItem item = (sender as ToolStripItem);
            if (item != null)
            {
                if (item.Owner is ContextMenuStrip owner)
                {
                    string flag = "00";
                    if (RetainNameCheckBox.Checked)
                    {
                        flag = Utilities.precedingZeros((flagByte + 0x40).ToString("X"), 2);
                    }
                    else
                    {
                        flag = Utilities.precedingZeros((flagByte).ToString("X"), 2);
                    }

                    if (!offline)
                    {
                        byte[] Bank01to20 = Utilities.GetInventoryBank(s, bot, 1);
                        byte[] Bank21to40 = Utilities.GetInventoryBank(s, bot, 21);

                        int slot = int.Parse(owner.SourceControl.Tag.ToString());
                        byte[] slotBytes = new byte[2];

                        int slotOffset;
                        if (slot < 21)
                        {
                            slotOffset = ((slot - 1) * 0x8);
                        }
                        else
                        {
                            slotOffset = ((slot - 21) * 0x8);
                        }

                        if (slot < 21)
                        {
                            Buffer.BlockCopy(Bank01to20, slotOffset, slotBytes, 0x0, 0x2);
                        }
                        else
                        {
                            Buffer.BlockCopy(Bank21to40, slotOffset, slotBytes, 0x0, 0x2);
                        }

                        string slotID = Utilities.flip(Utilities.ByteToHexString(slotBytes));

                        if (slotID == "FFFE")
                        {
                            return;
                        }

                        Utilities.setFlag1(s, bot, slot, flag);

                        var btnParent = (inventorySlot)owner.SourceControl;
                        btnParent.setFlag1(flag);
                        btnParent.refresh(false);
                    }
                    else
                    {
                        var btnParent = (inventorySlot)owner.SourceControl;
                        if (btnParent.fillItemID() != "FFFE")
                        {
                            btnParent.setFlag1(flag);
                            btnParent.refresh(false);
                        }
                    }
                    if (sound)
                        System.Media.SystemSounds.Asterisk.Play();
                }
            }
        }

        private void hexMode_Click(object sender, EventArgs e)
        {
            if (hexModeBtn.Tag.ToString() == "Normal")
            {
                AmountLabel.Text = "Hex Value";
                hexModeBtn.Tag = "Hex";
                hexModeBtn.Text = "Normal Mode";
                if (customAmountTxt.Text != "")
                {
                    int decValue = int.Parse(customAmountTxt.Text) - 1;
                    string hexValue;
                    if (decValue < 0)
                        hexValue = "0";
                    else
                        hexValue = decValue.ToString("X");
                    customAmountTxt.Text = Utilities.precedingZeros(hexValue, 8);
                }
            }
            else
            {
                AmountLabel.Text = "Amount";
                hexModeBtn.Tag = "Normal";
                hexModeBtn.Text = "Hex Mode";
                if (customAmountTxt.Text != "")
                {
                    string hexValue = customAmountTxt.Text;
                    int decValue = Convert.ToInt32(hexValue, 16) + 1;
                    customAmountTxt.Text = decValue.ToString();
                }
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            /*
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to save your inventory?\n[Warning] Your previous save will be overwritten!", "Load inventory", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {*/
            try
            {
                SaveFileDialog file = new SaveFileDialog()
                {
                    Filter = "New Horizons Inventory (*.nhi)|*.nhi",
                    //FileName = "items.nhi",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

                string savepath;

                if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + @"\save";
                else
                    savepath = config.AppSettings.Settings["LastSave"].Value;

                //Debug.Print(savepath);
                if (Directory.Exists(savepath))
                {
                    file.InitialDirectory = savepath;
                }
                else
                {
                    file.InitialDirectory = @"C:\";
                }

                if (file.ShowDialog() != DialogResult.OK)
                    return;

                string[] temp = file.FileName.Split('\\');
                string path = "";
                for (int i = 0; i < temp.Length - 1; i++)
                    path = path + temp[i] + "\\";

                config.AppSettings.Settings["LastSave"].Value = path;
                config.Save(ConfigurationSaveMode.Minimal);


                string Bank = "";

                if (!offline)
                {
                    byte[] Bank01to20 = Utilities.GetInventoryBank(s, bot, 1);
                    byte[] Bank21to40 = Utilities.GetInventoryBank(s, bot, 21);
                    Bank = Utilities.ByteToHexString(Bank01to20) + Utilities.ByteToHexString(Bank21to40);


                    byte[] save = new byte[320];

                    Array.Copy(Bank01to20, 0, save, 0, 160);
                    Array.Copy(Bank21to40, 0, save, 160, 160);

                    File.WriteAllBytes(file.FileName, save);
                }
                else
                {
                    inventorySlot[] SlotPointer = new inventorySlot[40];
                    foreach (inventorySlot btn in this.inventoryPanel.Controls.OfType<inventorySlot>())
                    {
                        int slotId = int.Parse(btn.Tag.ToString());
                        SlotPointer[slotId - 1] = btn;
                    }
                    for (int i = 0; i < SlotPointer.Length; i++)
                    {
                        string first = Utilities.flip(Utilities.precedingZeros(SlotPointer[i].getFlag1() + SlotPointer[i].getFlag2() + Utilities.precedingZeros(SlotPointer[i].fillItemID(), 4), 8));
                        string second = Utilities.flip(Utilities.precedingZeros(SlotPointer[i].fillItemData(), 8));
                        //Debug.Print(first + " " + second + " " + SlotPointer[i].getFlag1() + " " + SlotPointer[i].getFlag2() + " " + SlotPointer[i].fillItemID());
                        Bank = Bank + first + second;
                    }

                    byte[] save = new byte[320];

                    for (int i = 0; i < Bank.Length / 2 - 1; i++)
                    {
                        string data = String.Concat(Bank[(i * 2)].ToString(), Bank[((i * 2) + 1)].ToString());
                        //Debug.Print(i.ToString() + " " + data);
                        save[i] = Convert.ToByte(data, 16);
                    }

                    File.WriteAllBytes(file.FileName, save);
                }
                //Debug.Print(Bank);
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
                /*
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                config.AppSettings.Settings["save01"].Value = Bank1.Substring(0, 320);
                config.AppSettings.Settings["save21"].Value = Bank2.Substring(0, 320);
                config.Save(ConfigurationSaveMode.Minimal);
                MessageBox.Show("Inventory Saved!");
                */
            }
            catch
            {
                if (s != null)
                {
                    s.Close();
                }
                return;
            }
            //}
        }

        private void loadNHIBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog()
                {
                    Filter = "New Horizons Inventory (*.nhi)|*.nhi|All files (*.*)|*.*",
                    FileName = "items.nhi",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

                string savepath;

                if (config.AppSettings.Settings["LastLoad"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + @"\save";
                else
                    savepath = config.AppSettings.Settings["LastLoad"].Value;

                if (Directory.Exists(savepath))
                {
                    file.InitialDirectory = savepath;
                }
                else
                {
                    file.InitialDirectory = @"C:\";
                }

                if (file.ShowDialog() != DialogResult.OK)
                    return;

                string[] temp = file.FileName.Split('\\');
                string path = "";
                for (int i = 0; i < temp.Length - 1; i++)
                    path = path + temp[i] + "\\";

                config.AppSettings.Settings["LastLoad"].Value = path;
                config.Save(ConfigurationSaveMode.Minimal);

                byte[] data = File.ReadAllBytes(file.FileName);

                btnToolTip.RemoveAll();

                Thread LoadThread = new Thread(delegate () { loadInventory(data); });
                LoadThread.Start();
            }
            catch
            {
                if (s != null)
                {
                    s.Close();
                }
                return;
            }
        }

        private void loadInventory(byte[] data)
        {
            showWait();

            byte[][] item = processNHI(data);

            string Bank = "";

            byte[] b1 = new byte[160];
            byte[] b2 = new byte[160];

            if (!offline)
            {
                byte[] Bank01to20 = Utilities.GetInventoryBank(s, bot, 1);
                byte[] Bank21to40 = Utilities.GetInventoryBank(s, bot, 21);

                byte[] currentInventory = new byte[320];

                Array.Copy(Bank01to20, 0, currentInventory, 0, 160);
                Array.Copy(Bank21to40, 0, currentInventory, 160, 160);

                int emptyspace = numOfEmpty(currentInventory);

                if (emptyspace < item.Length)
                {
                    DialogResult dialogResult = myMessageBox.Show("Empty Spaces in your inventory : " + emptyspace + "\n" +
                                                                "Number of items to Spawn : " + item.Length + "\n" +
                                                                "\n" +
                                                                "Press  [Yes]  to clear your inventory and spawn the items " + "\n" +
                                                                "or  [No]  to cancel the spawn." + "\n" + "\n" +
                                                                "[Warning] You will lose your items in your inventory!"
                                                                , "Not enough inventory spaces!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        for (int i = 0; i < b1.Length; i++)
                        {
                            b1[i] = data[i];
                            b2[i] = data[i + 160];
                        }

                        Utilities.OverwriteAll(s, bot, b1, b2, ref counter);
                    }
                    else
                    {
                        hideWait();
                        if (sound)
                            System.Media.SystemSounds.Asterisk.Play();
                        return;
                    }
                }
                else
                {
                    b1 = Bank01to20;
                    b2 = Bank21to40;
                    fillInventory(ref b1, ref b2, item);

                    Utilities.OverwriteAll(s, bot, b1, b2, ref counter);
                }
            }
            else
            {
                inventorySlot[] SlotPointer = new inventorySlot[40];
                foreach (inventorySlot btn in this.inventoryPanel.Controls.OfType<inventorySlot>())
                {
                    int slotId = int.Parse(btn.Tag.ToString());
                    SlotPointer[slotId - 1] = btn;
                }
                for (int i = 0; i < SlotPointer.Length; i++)
                {
                    string first = Utilities.flip(Utilities.precedingZeros(SlotPointer[i].getFlag1() + SlotPointer[i].getFlag2() + Utilities.precedingZeros(SlotPointer[i].fillItemID(), 4), 8));
                    string second = Utilities.flip(Utilities.precedingZeros(SlotPointer[i].fillItemData(), 8));
                    Bank = Bank + first + second;
                }

                byte[] currentInventory = new byte[320];

                for (int i = 0; i < Bank.Length / 2 - 1; i++)
                {
                    string tempStr = String.Concat(Bank[(i * 2)].ToString(), Bank[((i * 2) + 1)].ToString());
                    //Debug.Print(i.ToString() + " " + data);
                    currentInventory[i] = Convert.ToByte(tempStr, 16);
                }

                int emptyspace = numOfEmpty(currentInventory);

                if (emptyspace < item.Length)
                {
                    DialogResult dialogResult = myMessageBox.Show("Empty Spaces in your inventory : " + emptyspace + "\n" +
                                                                "Number of items to Spawn : " + item.Length + "\n" +
                                                                "\n" +
                                                                "Press  [Yes]  to clear your inventory and spawn the new items " + "\n" +
                                                                "or  [No]  to cancel the spawn." + "\n" + "\n" +
                                                                "[Warning] You will lose your items in your inventory!"
                                                                , "Not enough inventory spaces!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        for (int i = 0; i < b1.Length; i++)
                        {
                            b1[i] = data[i];
                            b2[i] = data[i + 160];
                        }
                    }
                    else
                    {
                        hideWait();
                        if (sound)
                            System.Media.SystemSounds.Asterisk.Play();
                        return;
                    }
                }
                else
                {
                    Array.Copy(currentInventory, 0, b1, 0, 160);
                    Array.Copy(currentInventory, 160, b2, 0, 160);

                    fillInventory(ref b1, ref b2, item);
                }
            }

            byte[] newInventory = new byte[320];

            Array.Copy(b1, 0, newInventory, 0, 160);
            Array.Copy(b2, 0, newInventory, 160, 160);

            Invoke((MethodInvoker)delegate
            {
                foreach (inventorySlot btn in this.inventoryPanel.Controls.OfType<inventorySlot>())
                {
                    if (btn.Tag == null)
                        continue;

                    if (btn.Tag.ToString() == "")
                        continue;

                    int slotId = int.Parse(btn.Tag.ToString());

                    byte[] slotBytes = new byte[2];
                    byte[] flag1Bytes = new byte[1];
                    byte[] flag2Bytes = new byte[1];
                    byte[] dataBytes = new byte[4];
                    byte[] recipeBytes = new byte[2];

                    int slotOffset = ((slotId - 1) * 0x8);
                    int flag1Offset = 0x3 + ((slotId - 1) * 0x8);
                    int flag2Offset = 0x2 + ((slotId - 1) * 0x8);
                    int countOffset = 0x4 + ((slotId - 1) * 0x8);

                    Buffer.BlockCopy(newInventory, slotOffset, slotBytes, 0, 2);
                    Buffer.BlockCopy(newInventory, flag1Offset, flag1Bytes, 0, 1);
                    Buffer.BlockCopy(newInventory, flag2Offset, flag2Bytes, 0, 1);
                    Buffer.BlockCopy(newInventory, countOffset, dataBytes, 0, 4);
                    Buffer.BlockCopy(newInventory, countOffset, recipeBytes, 0, 2);

                    string itemID = Utilities.flip(Utilities.ByteToHexString(slotBytes));
                    string itemData = Utilities.flip(Utilities.ByteToHexString(dataBytes));
                    string recipeData = Utilities.flip(Utilities.ByteToHexString(recipeBytes));
                    string flag1 = Utilities.ByteToHexString(flag1Bytes);
                    string flag2 = Utilities.ByteToHexString(flag2Bytes);

                    //Debug.Print("Slot : " + slotId.ToString() + " ID : " + itemID + " Data : " + itemData + " recipeData : " + recipeData + " Flag1 : " + flag1 + " Flag2 : " + flag2);

                    if (itemID == "FFFE") //Nothing
                    {
                        btn.setup("", 0xFFFE, 0x0, "", "00", "00");
                        continue;
                    }
                    else if (itemID == "16A2") //Recipe
                    {
                        btn.setup(GetNameFromID(recipeData, recipeSource), 0x16A2, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag1, flag2);
                        continue;
                    }
                    else if (itemID == "1095") //Delivery
                    {
                        btn.setup(GetNameFromID(recipeData, itemSource), 0x1095, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag1, flag2);
                        continue;
                    }
                    else if (itemID == "16A1") //Bottle Message
                    {
                        btn.setup(GetNameFromID(recipeData, recipeSource), 0x16A1, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, recipeSource), "", flag1, flag2);
                        continue;
                    }
                    else if (itemID == "0A13") // Fossil
                    {
                        btn.setup(GetNameFromID(recipeData, itemSource), 0x0A13, Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(recipeData, itemSource), "", flag1, flag2);
                        continue;
                    }
                    else if (itemID == "114A") // Money Tree
                    {
                        btn.setup(GetNameFromID(itemID, itemSource), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, itemSource, Convert.ToUInt32("0x" + itemData, 16)), GetNameFromID(recipeData, itemSource), flag1, flag2);
                        continue;
                    }
                    else
                    {
                        btn.setup(GetNameFromID(itemID, itemSource), Convert.ToUInt16("0x" + itemID, 16), Convert.ToUInt32("0x" + itemData, 16), GetImagePathFromID(itemID, itemSource, Convert.ToUInt32("0x" + itemData, 16)), "", flag1, flag2);
                        continue;
                    }
                }
            });

            hideWait();
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private byte[][] processNHI(byte[] data)
        {
            byte[] tempItem = new byte[8];
            bool[] isItem = new bool[40];
            int numOfitem = 0;

            for (int i = 0; i < 40; i++)
            {
                Buffer.BlockCopy(data, 0x8 * i, tempItem, 0, 8);
                if (!Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                {
                    isItem[i] = true;
                    numOfitem++;
                }
            }

            byte[][] item = new byte[numOfitem][];
            int itemNum = 0;
            for (int j = 0; j < 40; j++)
            {
                if (isItem[j])
                {
                    item[itemNum] = new byte[8];
                    Buffer.BlockCopy(data, 0x8 * j, item[itemNum], 0, 8);
                    itemNum++;
                }
            }

            return item;
        }

        private int numOfEmpty(byte[] data)
        {
            byte[] tempItem = new byte[8];
            int num = 0;

            for (int i = 0; i < 40; i++)
            {
                Buffer.BlockCopy(data, 0x8 * i, tempItem, 0, 8);
                if (Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                    num++;
            }
            return num;
        }

        private void fillInventory(ref byte[] b1, ref byte[] b2, byte[][] item)
        {
            byte[] tempItem = new byte[8];
            int num = 0;

            for (int i = 0; i < 20; i++)
            {
                Buffer.BlockCopy(b1, 0x8 * i, tempItem, 0, 8);
                if (Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                {
                    Buffer.BlockCopy(item[num], 0, b1, 0x8 * i, 8);
                    num++;
                }
                if (num >= item.Length)
                    return;
            }

            for (int j = 0; j < 20; j++)
            {
                Buffer.BlockCopy(b2, 0x8 * j, tempItem, 0, 8);
                if (Utilities.ByteToHexString(tempItem).Equals("FEFF000000000000"))
                {
                    Buffer.BlockCopy(item[num], 0, b2, 0x8 * j, 8);
                    num++;
                }
                if (num >= item.Length)
                    return;
            }
        }

        private int findEmpty()
        {
            /*
            byte[] Bank01to20 = Utilities.GetInventoryBank(s, bot, 1);
            byte[] Bank21to40 = Utilities.GetInventoryBank(s, bot, 21);
            //string Bank1 = Encoding.ASCII.GetString(Bank01to20);
            //string Bank2 = Encoding.ASCII.GetString(Bank21to40);
            //Debug.Print(Bank1);
            //Debug.Print(Bank2);

            if (Bank01to20 == null | Bank21to40 == null)
            {
                return -1;
            }

            for (int slot = 1; slot <= 40; slot++)
            {
                byte[] slotBytes = new byte[4];

                int slotOffset;
                if (slot < 21)
                {
                    slotOffset = ((slot - 1) * 0x10);
                }
                else
                {
                    slotOffset = ((slot - 21) * 0x10);
                }

                if (slot < 21)
                {
                    Buffer.BlockCopy(Bank01to20, slotOffset, slotBytes, 0x0, 0x4);
                }
                else
                {
                    Buffer.BlockCopy(Bank21to40, slotOffset, slotBytes, 0x0, 0x4);
                }

                string itemID = Utilities.flip(Encoding.ASCII.GetString(slotBytes));

                if (itemID == "FFFE")
                {
                    return slot;
                }
            }
            */
            inventorySlot[] SlotPointer = new inventorySlot[40];

            foreach (inventorySlot btn in this.inventoryPanel.Controls.OfType<inventorySlot>())
            {
                int slotId = int.Parse(btn.Tag.ToString());
                SlotPointer[slotId - 1] = btn;
            }
            for (int i = 0; i < SlotPointer.Length; i++)
            {
                if (SlotPointer[i].fillItemID() == "FFFE")
                    return i + 1;
            }

            return -1;
        }

        private void UpdateTurnipPrices()
        {
            UInt64[] turnipPrices = Utilities.GetTurnipPrices(s, bot);
            turnipBuyPrice.Clear();
            turnipBuyPrice.SelectionAlignment = HorizontalAlignment.Center;
            turnipBuyPrice.Text = String.Format("{0}", turnipPrices[12]);
            UInt64 buyPrice = UInt64.Parse(String.Format("{0}", turnipPrices[12]));

            turnipSell1AM.Clear();
            turnipSell1AM.Text = String.Format("{0}", turnipPrices[0]);
            UInt64 MondayAM = UInt64.Parse(String.Format("{0}", turnipPrices[0]));
            setTurnipColor(buyPrice, MondayAM, turnipSell1AM);

            turnipSell1PM.Clear();
            turnipSell1PM.Text = String.Format("{0}", turnipPrices[1]);
            UInt64 MondayPM = UInt64.Parse(String.Format("{0}", turnipPrices[1]));
            setTurnipColor(buyPrice, MondayPM, turnipSell1PM);

            turnipSell2AM.Clear();
            turnipSell2AM.Text = String.Format("{0}", turnipPrices[2]);
            UInt64 TuesdayAM = UInt64.Parse(String.Format("{0}", turnipPrices[2]));
            setTurnipColor(buyPrice, TuesdayAM, turnipSell2AM);

            turnipSell2PM.Clear();
            turnipSell2PM.Text = String.Format("{0}", turnipPrices[3]);
            UInt64 TuesdayPM = UInt64.Parse(String.Format("{0}", turnipPrices[3]));
            setTurnipColor(buyPrice, TuesdayPM, turnipSell2PM);

            turnipSell3AM.Clear();
            turnipSell3AM.Text = String.Format("{0}", turnipPrices[4]);
            UInt64 WednesdayAM = UInt64.Parse(String.Format("{0}", turnipPrices[4]));
            setTurnipColor(buyPrice, WednesdayAM, turnipSell3AM);

            turnipSell3PM.Clear();
            turnipSell3PM.Text = String.Format("{0}", turnipPrices[5]);
            UInt64 WednesdayPM = UInt64.Parse(String.Format("{0}", turnipPrices[5]));
            setTurnipColor(buyPrice, WednesdayPM, turnipSell3PM);

            turnipSell4AM.Clear();
            turnipSell4AM.Text = String.Format("{0}", turnipPrices[6]);
            UInt64 ThursdayAM = UInt64.Parse(String.Format("{0}", turnipPrices[6]));
            setTurnipColor(buyPrice, ThursdayAM, turnipSell4AM);

            turnipSell4PM.Clear();
            turnipSell4PM.Text = String.Format("{0}", turnipPrices[7]);
            UInt64 ThursdayPM = UInt64.Parse(String.Format("{0}", turnipPrices[7]));
            setTurnipColor(buyPrice, ThursdayPM, turnipSell4PM);

            turnipSell5AM.Clear();
            turnipSell5AM.Text = String.Format("{0}", turnipPrices[8]);
            UInt64 FridayAM = UInt64.Parse(String.Format("{0}", turnipPrices[8]));
            setTurnipColor(buyPrice, FridayAM, turnipSell5AM);

            turnipSell5PM.Clear();
            turnipSell5PM.Text = String.Format("{0}", turnipPrices[9]);
            UInt64 FridayPM = UInt64.Parse(String.Format("{0}", turnipPrices[9]));
            setTurnipColor(buyPrice, FridayPM, turnipSell5PM);

            turnipSell6AM.Clear();
            turnipSell6AM.Text = String.Format("{0}", turnipPrices[10]);
            UInt64 SaturdayAM = UInt64.Parse(String.Format("{0}", turnipPrices[10]));
            setTurnipColor(buyPrice, SaturdayAM, turnipSell6AM);

            turnipSell6PM.Clear();
            turnipSell6PM.Text = String.Format("{0}", turnipPrices[11]);
            UInt64 SaturdayPM = UInt64.Parse(String.Format("{0}", turnipPrices[11]));
            setTurnipColor(buyPrice, SaturdayPM, turnipSell6PM);

            UInt64[] price = { MondayAM, MondayPM, TuesdayAM, TuesdayPM, WednesdayAM, WednesdayPM, ThursdayAM, ThursdayPM, FridayAM, FridayPM, SaturdayAM, SaturdayPM };
            UInt64 highest = findHighest(price);
            setStar(highest, MondayAM, MondayPM, TuesdayAM, TuesdayPM, WednesdayAM, WednesdayPM, ThursdayAM, ThursdayPM, FridayAM, FridayPM, SaturdayAM, SaturdayPM);
        }

        private void setTurnipColor(UInt64 buyPrice, UInt64 comparePrice, RichTextBox target)
        {
            target.SelectionAlignment = HorizontalAlignment.Center;

            if (comparePrice > buyPrice)
            {
                target.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            }
            else if (comparePrice < buyPrice)
            {
                target.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            }
        }

        private UInt64 findHighest(UInt64[] price)
        {
            UInt64 highest = 0;
            for (int i = 0; i < price.Length; i++)
            {
                if (price[i] > highest)
                {
                    highest = price[i];
                }
            }
            return highest;
        }

        private void setStar(UInt64 highest, UInt64 MondayAM, UInt64 MondayPM, UInt64 TuesdayAM, UInt64 TuesdayPM, UInt64 WednesdayAM, UInt64 WednesdayPM, UInt64 ThursdayAM, UInt64 ThursdayPM, UInt64 FridayAM, UInt64 FridayPM, UInt64 SaturdayAM, UInt64 SaturdayPM)
        {
            if (MondayAM >= highest) { mondayAMStar.Visible = true; } else { mondayAMStar.Visible = false; };
            if (MondayPM >= highest) { mondayPMStar.Visible = true; } else { mondayPMStar.Visible = false; };

            if (TuesdayAM >= highest) { tuesdayAMStar.Visible = true; } else { tuesdayAMStar.Visible = false; };
            if (TuesdayPM >= highest) { tuesdayPMStar.Visible = true; } else { tuesdayPMStar.Visible = false; };

            if (WednesdayAM >= highest) { wednesdayAMStar.Visible = true; } else { wednesdayAMStar.Visible = false; };
            if (WednesdayPM >= highest) { wednesdayPMStar.Visible = true; } else { wednesdayPMStar.Visible = false; };

            if (ThursdayAM >= highest) { thursdayAMStar.Visible = true; } else { thursdayAMStar.Visible = false; };
            if (ThursdayPM >= highest) { thursdayPMStar.Visible = true; } else { thursdayPMStar.Visible = false; };

            if (FridayAM >= highest) { fridayAMStar.Visible = true; } else { fridayAMStar.Visible = false; };
            if (FridayPM >= highest) { fridayPMStar.Visible = true; } else { fridayPMStar.Visible = false; };

            if (SaturdayAM >= highest) { saturdayAMStar.Visible = true; } else { saturdayAMStar.Visible = false; };
            if (SaturdayPM >= highest) { saturdayPMStar.Visible = true; } else { saturdayPMStar.Visible = false; };
        }

        private void loadReaction(int player = 0)
        {
            byte[] reactionBank = Utilities.getReaction(s, bot, player);
            //Debug.Print(Encoding.ASCII.GetString(reactionBank));

            byte[] reactions1 = new byte[1];
            byte[] reactions2 = new byte[1];
            byte[] reactions3 = new byte[1];
            byte[] reactions4 = new byte[1];
            byte[] reactions5 = new byte[1];
            byte[] reactions6 = new byte[1];
            byte[] reactions7 = new byte[1];
            byte[] reactions8 = new byte[1];

            Buffer.BlockCopy(reactionBank, 0, reactions1, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 1, reactions2, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 2, reactions3, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 3, reactions4, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 4, reactions5, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 5, reactions6, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 6, reactions7, 0x0, 0x1);
            Buffer.BlockCopy(reactionBank, 7, reactions8, 0x0, 0x1);

            setReactionBox(Utilities.ByteToHexString(reactions1), reactionSlot1);
            setReactionBox(Utilities.ByteToHexString(reactions2), reactionSlot2);
            setReactionBox(Utilities.ByteToHexString(reactions3), reactionSlot3);
            setReactionBox(Utilities.ByteToHexString(reactions4), reactionSlot4);
            setReactionBox(Utilities.ByteToHexString(reactions5), reactionSlot5);
            setReactionBox(Utilities.ByteToHexString(reactions6), reactionSlot6);
            setReactionBox(Utilities.ByteToHexString(reactions7), reactionSlot7);
            setReactionBox(Utilities.ByteToHexString(reactions8), reactionSlot8);
        }

        private void setReactionBox(string reaction, ComboBox box)
        {
            if (reaction == "00")
            {
                box.SelectedIndex = -1;
                return;
            }
            string hexValue = reaction;
            int decValue = Convert.ToInt32(hexValue, 16) - 1;
            if (decValue > 113)
                box.SelectedIndex = -1;
            else
                box.SelectedIndex = decValue;
        }

        private void setReactionBtn_Click(object sender, EventArgs e)
        {
            //DialogResult dialogResult = myMessageBox.Show("Are you sure you want to change your reaction wheel?\n[Warning] Your previous reaction wheel will be overwritten!", "Change Reaction Wheel", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            //if (dialogResult == DialogResult.Yes)
            //{
            int player = playerSelectorOther.SelectedIndex;

            string reactionFirstHalf = (Utilities.precedingZeros((reactionSlot1.SelectedIndex + 1).ToString("X"), 2) + Utilities.precedingZeros((reactionSlot2.SelectedIndex + 1).ToString("X"), 2) + Utilities.precedingZeros((reactionSlot3.SelectedIndex + 1).ToString("X"), 2) + Utilities.precedingZeros((reactionSlot4.SelectedIndex + 1).ToString("X"), 2));
            string reactionSecondHalf = (Utilities.precedingZeros((reactionSlot5.SelectedIndex + 1).ToString("X"), 2) + Utilities.precedingZeros((reactionSlot6.SelectedIndex + 1).ToString("X"), 2) + Utilities.precedingZeros((reactionSlot7.SelectedIndex + 1).ToString("X"), 2) + Utilities.precedingZeros((reactionSlot8.SelectedIndex + 1).ToString("X"), 2));
            Utilities.setReaction(s, bot, player, reactionFirstHalf, reactionSecondHalf);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
            //}
        }

        private void speedX4Btn_Click(object sender, EventArgs e)
        {
            speedX1Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            speedX2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            speedX3Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            speedX4Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));

            Utilities.pokeMainAddress(s, bot, Utilities.wSpeedAddress.ToString("X"), Utilities.wSpeedX4);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void speedX3Btn_Click(object sender, EventArgs e)
        {
            speedX1Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            speedX2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            speedX3Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            speedX4Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.wSpeedAddress.ToString("X"), Utilities.wSpeedX3);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void speedX2Btn_Click(object sender, EventArgs e)
        {
            speedX1Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            speedX2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            speedX3Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            speedX4Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.wSpeedAddress.ToString("X"), Utilities.wSpeedX2);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void speedX1Btn_Click(object sender, EventArgs e)
        {
            speedX1Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            speedX2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            speedX3Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            speedX4Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.wSpeedAddress.ToString("X"), Utilities.wSpeedX1);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void maxSpeedX1Btn_Click(object sender, EventArgs e)
        {
            maxSpeedX1Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            maxSpeedX2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX3Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX5Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX100Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeAddress(s, bot, Utilities.MaxSpeedAddress.ToString("X"), Utilities.MaxSpeedX1);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void maxSpeedX2Btn_Click(object sender, EventArgs e)
        {
            maxSpeedX1Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            maxSpeedX3Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX5Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX100Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeAddress(s, bot, Utilities.MaxSpeedAddress.ToString("X"), Utilities.MaxSpeedX2);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void maxSpeedX3Btn_Click(object sender, EventArgs e)
        {
            maxSpeedX1Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX3Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            maxSpeedX5Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX100Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeAddress(s, bot, Utilities.MaxSpeedAddress.ToString("X"), Utilities.MaxSpeedX3);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void maxSpeedX5Btn_Click(object sender, EventArgs e)
        {
            maxSpeedX1Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX3Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX5Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            maxSpeedX100Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeAddress(s, bot, Utilities.MaxSpeedAddress.ToString("X"), Utilities.MaxSpeedX5);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void maxSpeedX100Btn_Click(object sender, EventArgs e)
        {
            maxSpeedX1Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX2Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX3Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX5Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            maxSpeedX100Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));

            Utilities.pokeAddress(s, bot, Utilities.MaxSpeedAddress.ToString("X"), Utilities.MaxSpeedX100);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void disableCollisionBtn_Click(object sender, EventArgs e)
        {
            disableCollisionBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            enableCollisionBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.CollisionAddress.ToString("X"), Utilities.CollisionDisable);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void enableCollisionBtn_Click(object sender, EventArgs e)
        {
            enableCollisionBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            disableCollisionBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.CollisionAddress.ToString("X"), Utilities.CollisionEnable);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void freezeTimeBtn_Click(object sender, EventArgs e)
        {
            freezeTimeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            unfreezeTimeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.freezeTimeAddress.ToString("X"), Utilities.freezeTimeValue);
            readtime();
            timePanel.Visible = true;

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void unfreezeTimeBtn_Click(object sender, EventArgs e)
        {
            unfreezeTimeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            freezeTimeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.freezeTimeAddress.ToString("X"), Utilities.unfreezeTimeValue);
            timePanel.Visible = false;
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void animationSpdx2_Click(object sender, EventArgs e)
        {
            animationSpdx1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            animationSpdx0_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx50.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.aSpeedAddress.ToString("X"), Utilities.aSpeedX2);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void animationSpdx0_1_Click(object sender, EventArgs e)
        {
            animationSpdx1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx0_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            animationSpdx50.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.aSpeedAddress.ToString("X"), Utilities.aSpeedX01);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void animationSpdx50_Click(object sender, EventArgs e)
        {
            animationSpdx1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx0_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx50.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            animationSpdx5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.aSpeedAddress.ToString("X"), Utilities.aSpeedX50);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void animationSpdx1_Click(object sender, EventArgs e)
        {
            animationSpdx1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            animationSpdx2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx0_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx50.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));

            Utilities.pokeMainAddress(s, bot, Utilities.aSpeedAddress.ToString("X"), Utilities.aSpeedX1);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void animationSpdx5_Click(object sender, EventArgs e)
        {
            animationSpdx1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx0_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx50.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            animationSpdx5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));

            Utilities.pokeMainAddress(s, bot, Utilities.aSpeedAddress.ToString("X"), Utilities.aSpeedX5);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void readtime()
        {
            byte[] b = Utilities.peekAddress(s, bot, Utilities.readTimeAddress, 6);
            string time = Utilities.ByteToHexString(b);

            Debug.Print(time);

            Int32 year = Convert.ToInt32(Utilities.flip(time.Substring(0, 4)), 16);
            Int32 month = Convert.ToInt32((time.Substring(4, 2)), 16);
            Int32 day = Convert.ToInt32((time.Substring(6, 2)), 16);
            Int32 hour = Convert.ToInt32((time.Substring(8, 2)), 16);
            Int32 min = Convert.ToInt32((time.Substring(10, 2)), 16);

            if (year > 3000 || month > 12 || day > 31 || hour > 24 || min > 60) //Try for Chineses
            {
                b = Utilities.peekAddress(s, bot, Utilities.readTimeAddress + Utilities.ChineseLanguageOffset, 6);
                time = Utilities.ByteToHexString(b);

                year = Convert.ToInt32(Utilities.flip(time.Substring(0, 4)), 16);
                month = Convert.ToInt32((time.Substring(4, 2)), 16);
                day = Convert.ToInt32((time.Substring(6, 2)), 16);
                hour = Convert.ToInt32((time.Substring(8, 2)), 16);
                min = Convert.ToInt32((time.Substring(10, 2)), 16);

                if (!(year > 3000 || month > 12 || day > 31 || hour > 24 || min > 60))
                    ChineseFlag = true;
            }

            yearTextbox.Clear();
            yearTextbox.SelectionAlignment = HorizontalAlignment.Center;
            yearTextbox.Text = year.ToString();

            monthTextbox.Clear();
            monthTextbox.SelectionAlignment = HorizontalAlignment.Center;
            monthTextbox.Text = month.ToString();

            dayTextbox.Clear();
            dayTextbox.SelectionAlignment = HorizontalAlignment.Center;
            dayTextbox.Text = day.ToString();

            hourTextbox.Clear();
            hourTextbox.SelectionAlignment = HorizontalAlignment.Center;
            hourTextbox.Text = hour.ToString();

            minTextbox.Clear();
            minTextbox.SelectionAlignment = HorizontalAlignment.Center;
            minTextbox.Text = min.ToString();
        }

        private void settimeBtn_Click(object sender, EventArgs e)
        {
            int decYear = int.Parse(yearTextbox.Text);
            if (decYear > 2060)
            {
                decYear = 2060;
            }
            else if (decYear < 1970)
            {
                decYear = 1970;
            }
            yearTextbox.Text = decYear.ToString();
            string hexYear = decYear.ToString("X");

            int decMonth = int.Parse(monthTextbox.Text);
            if (decMonth > 12)
            {
                decMonth = 12;
            }
            else if (decMonth < 0)
            {
                decMonth = 1;
            }
            monthTextbox.Text = decMonth.ToString();
            string hexMonth = decMonth.ToString("X");

            int decDay = int.Parse(dayTextbox.Text);
            if (decDay > 31)
            {
                decDay = 31;
            }
            else if (decDay < 0)
            {
                decDay = 1;
            }
            dayTextbox.Text = decDay.ToString();
            string hexDay = decDay.ToString("X");

            int decHour = int.Parse(hourTextbox.Text);
            if (decHour > 23)
            {
                decHour = 23;
            }
            else if (decHour < 0)
            {
                decHour = 0;
            }
            hourTextbox.Text = decHour.ToString();
            string hexHour = decHour.ToString("X");


            int decMin = int.Parse(minTextbox.Text);
            if (decMin > 59)
            {
                decMin = 59;
            }
            else if (decMin < 0)
            {
                decMin = 0;
            }
            minTextbox.Text = decMin.ToString();
            string hexMin = decMin.ToString("X");

            if (ChineseFlag)
            {
                Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.flip(Utilities.precedingZeros(hexYear, 4)));
                Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + 0x2 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.precedingZeros(hexMonth, 2) + Utilities.precedingZeros(hexDay, 2) + Utilities.precedingZeros(hexHour, 2) + Utilities.precedingZeros(hexMin, 2));
            }
            else
            {
                Utilities.pokeAddress(s, bot, Utilities.readTimeAddress.ToString("X"), Utilities.flip(Utilities.precedingZeros(hexYear, 4)));
                Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + 0x2).ToString("X"), Utilities.precedingZeros(hexMonth, 2) + Utilities.precedingZeros(hexDay, 2) + Utilities.precedingZeros(hexHour, 2) + Utilities.precedingZeros(hexMin, 2));
            }

            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void minus1HourBtn_Click(object sender, EventArgs e)
        {
            int decHour = int.Parse(hourTextbox.Text) - 1;
            if (decHour < 0)
            {
                decHour = 23;
                string hexHour = decHour.ToString("X");

                int decDay = int.Parse(dayTextbox.Text) - 1;
                string hexDay = decDay.ToString("X");
                if (ChineseFlag)
                    Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + 0x3 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.precedingZeros(hexDay, 2) + Utilities.precedingZeros(hexHour, 2));
                else
                    Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + 0x3).ToString("X"), Utilities.precedingZeros(hexDay, 2) + Utilities.precedingZeros(hexHour, 2));
            }
            else
            {
                string hexHour = decHour.ToString("X");
                if (ChineseFlag)
                    Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + 0x4 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.precedingZeros(hexHour, 2));
                else
                    Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + 0x4).ToString("X"), Utilities.precedingZeros(hexHour, 2));
            }
            readtime();
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void add1HourBtn_Click(object sender, EventArgs e)
        {
            int decHour = int.Parse(hourTextbox.Text) + 1;
            if (decHour >= 24)
            {
                decHour = 0;
                string hexHour = decHour.ToString("X");

                int decDay = int.Parse(dayTextbox.Text) + 1;
                string hexDay = decDay.ToString("X");
                if (ChineseFlag)
                    Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + 0x3 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.precedingZeros(hexDay, 2) + Utilities.precedingZeros(hexHour, 2));
                else
                    Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + 0x3).ToString("X"), Utilities.precedingZeros(hexDay, 2) + Utilities.precedingZeros(hexHour, 2));
            }
            else
            {
                string hexHour = decHour.ToString("X");
                if (ChineseFlag)
                    Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + 0x4 + Utilities.ChineseLanguageOffset).ToString("X"), Utilities.precedingZeros(hexHour, 2));
                else
                    Utilities.pokeAddress(s, bot, (Utilities.readTimeAddress + 0x4).ToString("X"), Utilities.precedingZeros(hexHour, 2));
            }
            readtime();
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void USBconnectBtn_Click(object sender, EventArgs e)
        {

            if (USBconnectBtn.Tag.ToString() == "connect")
            {
                bot = new USBBot();
                if (bot.Connect())
                {
                    Log.logEvent("MainForm", "Connection Succeeded : USB");

                    //playerSelectorInventory.SelectedIndex = updateDropdownBox();
                    /*
                    if (UpdateInventory())
                        return;
                    */
                    //UpdateTownID();
                    //InitTimer();
                    setEatBtn();
                    if (!disableValidation)
                    {
                        UpdateTurnipPrices();
                        loadReaction();
                    }
                    playerSelectorOther.SelectedIndex = 0;

                    readWeatherSeed();
                    this.connectBtn.Visible = false;
                    this.refreshBtn.Visible = true;
                    //this.playerSelectionPanel.Visible = true;
                    //this.autoRefreshCheckBox.Visible = true;
                    this.autoRefreshCheckBox.Checked = false;
                    this.saveBtn.Visible = true;
                    this.loadBtn.Visible = true;
                    this.otherBtn.Visible = true;
                    this.critterBtn.Visible = true;
                    this.villagerBtn.Visible = true;
                    this.wrapSetting.SelectedIndex = 0;
                    //this.selectedItem.setHide(true);
                    this.ipBox.Visible = false;
                    this.IPboxBorder.Visible = false;
                    this.pokeMainCheatPanel.Visible = false;
                    this.configBtn.Visible = false;
                    this.playerSelectorInventory.Visible = true;
                    this.Text += "  |  [Connected via USB]";
                    //this.mapDropperBtn.Visible = true;

                    currentGridView = insectGridView;

                    LoadGridView(InsectAppearParam, insectGridView, ref insectRate, Utilities.InsectDataSize, Utilities.InsectNumRecords);
                    LoadGridView(FishRiverAppearParam, riverFishGridView, ref riverFishRate, Utilities.FishDataSize, Utilities.FishRiverNumRecords, 1);
                    LoadGridView(FishSeaAppearParam, seaFishGridView, ref seaFishRate, Utilities.FishDataSize, Utilities.FishSeaNumRecords, 1);
                    LoadGridView(CreatureSeaAppearParam, seaCreatureGridView, ref seaCreatureRate, Utilities.SeaCreatureDataSize, Utilities.SeaCreatureNumRecords, 1);

                    USBconnectBtn.Text = "Disconnect";
                    USBconnectBtn.Tag = "Disconnect";

                    offline = false;
                }
                else
                {
                    Log.logEvent("MainForm", "Connection Failed : USB");
                    bot = null;
                }
            }
            else
            {
                bot.Disconnect();

                foreach (inventorySlot btn in this.inventoryPanel.Controls.OfType<inventorySlot>())
                {
                    btn.reset();
                }

                this.connectBtn.Visible = true;
                this.connectBtn.Enabled = true;
                this.refreshBtn.Visible = false;
                //this.playerSelectionPanel.Visible = false;
                this.autoRefreshCheckBox.Visible = false;
                this.playerSelectorInventory.Visible = false;
                //this.saveBtn.Visible = false;
                //this.loadBtn.Visible = false;
                inventoryBtn_Click(sender, e);
                this.otherBtn.Visible = false;
                this.critterBtn.Visible = false;
                this.villagerBtn.Visible = false;
                this.ipBox.Visible = true;
                this.IPboxBorder.Visible = true;
                this.configBtn.Visible = true;
                cleanVillagerPage();

                this.USBconnectBtn.Text = "USB";
                this.USBconnectBtn.Tag = "connect";
                this.Text = version;
            }
        }

        private void egg_Click(object sender, EventArgs e)
        {
            if (waveOut == null)
            {
                Log.logEvent("MainForm", "EasterEgg Started");
                string path = AppDomain.CurrentDomain.BaseDirectory;
                AudioFileReader audioFileReader = new AudioFileReader(path + Utilities.villagerPath + "Io.nhv2");
                LoopStream loop = new LoopStream(audioFileReader);
                waveOut = new WaveOut();
                waveOut.Init(loop);
                waveOut.Play();
            }
            else
            {
                Log.logEvent("MainForm", "EasterEgg Stopped");
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
        }

        private void playerSelectorOther_SelectedIndexChanged(object sender, EventArgs e)
        {
            int player = playerSelectorOther.SelectedIndex;

            loadReaction(player);
        }
    }
}
