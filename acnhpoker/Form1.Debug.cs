﻿using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ACNHPoker
{
    public partial class Form1 : Form
    {
        public const int MapTileCount16x16 = 16 * 16 * 7 * 6;
        public const int TerrainTileSize = 0xE;

        public const int AllTerrainSize = MapTileCount16x16 * TerrainTileSize;

        private const int BuildingSize = 0x14;
        private const int AllBuildingSize = 46 * BuildingSize;

        public const int AcreWidth = 7 + (2 * 1); // 1 on each side cannot be traversed
        private const int AcreHeight = 6 + (2 * 1); // 1 on each side cannot be traversed
        private const int AcreMax = AcreWidth * AcreHeight;
        private const int AllAcreSize = AcreMax * 2;
        private const int AcrePlusAdditionalParams = AllAcreSize + 2 + 4 + 8 + sizeof(uint); // MainFieldParamUniqueID + EventPlazaLeftUpX + EventPlazaLeftUpZ

        byte[] save = null;

        private void PokeBtn_Click(object sender, EventArgs e)
        {
            Utilities.pokeAddress(s, bot, debugAddress.Text, debugAmount.Text);
        }
        private void PeekBtn_Click(object sender, EventArgs e)
        {
            byte[] AddressBank = Utilities.peekAddress(s, bot, Convert.ToInt32(debugAddress.Text, 16), 160);

            byte[] firstBytes = new byte[4];
            byte[] secondBytes = new byte[4];
            byte[] thirdBytes = new byte[4];
            byte[] fourthBytes = new byte[4];

            Buffer.BlockCopy(AddressBank, 0x0, firstBytes, 0x0, 0x4);
            Buffer.BlockCopy(AddressBank, 0x4, secondBytes, 0x0, 0x4);
            Buffer.BlockCopy(AddressBank, 0x8, thirdBytes, 0x0, 0x4);
            Buffer.BlockCopy(AddressBank, 0x12, fourthBytes, 0x0, 0x4);

            string firstResult = Utilities.ByteToHexString(firstBytes);
            string secondResult = Utilities.ByteToHexString(secondBytes);
            string thirdResult = Utilities.ByteToHexString(thirdBytes);
            string fourthResult = Utilities.ByteToHexString(fourthBytes);
            string FullResult = Utilities.ByteToHexString(AddressBank);

            PokeResult1.Text = Utilities.flip(firstResult);
            PokeResult2.Text = Utilities.flip(secondResult);
            PokeResult3.Text = Utilities.flip(thirdResult);
            PokeResult4.Text = Utilities.flip(fourthResult);
            FullAddress.Text = FullResult;
        }

        UInt32 loopID = 0x00002200;
        UInt32 loopData = 0x00000000;

        private void debugBtn_Click(object sender, EventArgs e)
        {
            //showWait();

            byte[] b1 = new byte[160];
            byte[] b2 = new byte[160];
            //byte[] ID = Utilities.stringToByte(Utilities.flip(Utilities.precedingZeros(loopID.ToString("X"), 8)));
            byte[] Data = Utilities.stringToByte(Utilities.flip(Utilities.precedingZeros(loopData.ToString("X"), 8)));

            //Debug.Print(Utilities.precedingZeros(itemID, 8));
            //Debug.Print(Utilities.precedingZeros(itemAmount, 8));

            for (int i = 0; i < b1.Length; i += 8)
            {
                byte[] ID = Utilities.stringToByte(Utilities.flip(Utilities.precedingZeros(loopID.ToString("X"), 8)));
                for (int j = 0; j < 4; j++)
                {
                    b1[i + j] = ID[j];
                    b1[i + j + 4] = Data[j];
                }
                loopID++;
            }

            for (int i = 0; i < b2.Length; i += 8)
            {
                byte[] ID = Utilities.stringToByte(Utilities.flip(Utilities.precedingZeros(loopID.ToString("X"), 8)));
                for (int j = 0; j < 4; j++)
                {
                    b2[i + j] = ID[j];
                    b2[i + j + 4] = Data[j];
                }
                loopID++;
            }

            //string result1 = Encoding.ASCII.GetString(Utilities.transform(b1));
            //string result2 = Encoding.ASCII.GetString(Utilities.transform(b2));
            //Debug.Print(result1 + "\n" + result2);

            Utilities.OverwriteAll(s, bot, b1, b2, ref counter);

            /*
            foreach (inventorySlot btn in this.inventoryPanel.Controls.OfType<inventorySlot>())
            {
                Invoke((MethodInvoker)delegate
                {
                    btn.setup(GetNameFromID(Utilities.turn2bytes(itemID), itemSource), Convert.ToUInt16("0x" + Utilities.turn2bytes(itemID), 16), Convert.ToUInt32("0x" + itemAmount, 16), GetImagePathFromID(Utilities.turn2bytes(itemID), itemSource, Convert.ToUInt32("0x" + itemAmount, 16)), "", selectedItem.getFlag1(), selectedItem.getFlag2());
                });
            }
            */

            //Thread.Sleep(1000);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
            //hideWait();
        }

        private void ChaseBtn_Click(object sender, EventArgs e)
        {
            UInt32 startAddress = 0xACDA0000;
            UInt32 endAddress = 0xAE000000;

            UInt32 diff = ((endAddress - startAddress) / 10);

            //Debug.Print(diff.ToString("X") + " " + (startAddress + diff).ToString("X"));


            //Debug.Print(Encoding.Default.GetString(Utilities.peekAddress(s, "0x" + (startAddress).ToString("X"))));


            //Debug.Print(Encoding.Default.GetString(output));

            //Debug.Print("Searching...");

            string value = FullAddress.Text;

            Thread SearchThread1 = new Thread(delegate () { SearchAddress(startAddress, endAddress, value); });
            SearchThread1.Start();

            /*
            Thread SearchThread2 = new Thread(delegate () { SearchAddress(startAddress + diff * 1, startAddress + diff * 2); });
            SearchThread2.Start();

            Thread SearchThread3 = new Thread(delegate () { SearchAddress(startAddress + diff * 2, startAddress + diff * 3); });
            SearchThread3.Start();

            Thread SearchThread4 = new Thread(delegate () { SearchAddress(startAddress + diff * 3, startAddress + diff * 4); });
            SearchThread4.Start();

            Thread SearchThread5 = new Thread(delegate () { SearchAddress(startAddress + diff * 4, startAddress + diff * 5); });
            SearchThread5.Start();

            Thread SearchThread6 = new Thread(delegate () { SearchAddress(startAddress + diff * 5, startAddress + diff * 6); });
            SearchThread6.Start();

            Thread SearchThread7 = new Thread(delegate () { SearchAddress(startAddress + diff * 6, startAddress + diff * 7); });
            SearchThread7.Start();

            Thread SearchThread8 = new Thread(delegate () { SearchAddress(startAddress + diff * 7, startAddress + diff * 8); });
            SearchThread8.Start();

            Thread SearchThread9 = new Thread(delegate () { SearchAddress(startAddress + diff * 8, startAddress + diff * 9); });
            SearchThread9.Start();

            Thread SearchThread10 = new Thread(delegate () { SearchAddress(startAddress + diff * 9, endAddress); });
            SearchThread10.Start();
            */
        }

        private void addressDebug_Click(object sender, EventArgs e)
        {
            Debug.Print(Utilities.player1SlotBase.ToString("X"));
            Debug.Print(Utilities.playerOffset.ToString("X"));
            Debug.Print(Utilities.Slot21Offset.ToString("X"));
            Debug.Print(Utilities.HomeOffset.ToString("X"));
            Debug.Print(Utilities.ReactionOffset.ToString("X"));
            Debug.Print(Utilities.VillagerAddress.ToString("X"));
            Debug.Print(Utilities.VillagerSize.ToString("X"));
            Debug.Print(Utilities.VillagerHouseAddress.ToString("X"));
            Debug.Print(Utilities.VillagerHouseSize.ToString("X"));
            Debug.Print(Utilities.VillagerHouseBufferDiff.ToString("X"));
            Debug.Print(Utilities.MasterRecyclingBase.ToString("X"));
            Debug.Print(Utilities.TurnipPurchasePriceAddr.ToString("X"));
            Debug.Print(Utilities.staminaAddress.ToString("X"));
            Debug.Print(Utilities.wSpeedAddress.ToString("X"));
            Debug.Print(Utilities.aSpeedAddress.ToString("X"));
            Debug.Print(Utilities.CollisionAddress.ToString("X"));
            Debug.Print(Utilities.freezeTimeAddress.ToString("X"));
            Debug.Print(Utilities.readTimeAddress.ToString("X"));
            Debug.Print(Utilities.InsectAppearPointer.ToString("X"));
            Debug.Print(Utilities.TownNameddress.ToString("X"));
            Debug.Print(Utilities.weatherSeed.ToString("X"));
        }

        private void SearchAddress(UInt32 startAddress, UInt32 endAddress, string value)
        {
            Debug.Print("Thread Start " + startAddress.ToString("X") + " " + endAddress.ToString("X"));

            byte[] result = Utilities.stringToByte(value);
            /*
            BoyerMoore boi = new BoyerMoore(result);

            //bank = new byte[0];

            for (UInt32 i = 0x0; startAddress + i <= endAddress; i += 8000)
            {
                if (offsetFound)
                {
                    return;
                }

                byte[] b = Utilities.peekAddress(s, bot, startAddress + i, 8000);
                //bank = Utilities.add(bank, b);

                Debug.Print(Utilities.ByteToHexString(b));
                int NUM = boi.Search(b);

                if (NUM >= 0)
                {
                    myMessageBox.Show(">> " + (startAddress + i + NUM / 2).ToString("X") + " << DONE : " + (NUM / 2).ToString("X"));
                    Debug.Print(">> " + (startAddress + i + NUM / 2).ToString("X") + " << DONE : " + (NUM / 2).ToString("X"));
                    //offsetFound = true;
                    return;
                }
            }
            */
        }

        private void dumpBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                byte[] Data = Utilities.peekAddress(s, bot, Utilities.player1SlotBase + (i * Utilities.playerOffset), (int)Utilities.playerOffset);
                File.WriteAllBytes("dump" + i + ".bin", Data);
            }
        }

        private void AddNHIBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog()
            {
                Filter = "New Horizons Inventory(*.nhi) | *.nhi|All files (*.*)|*.*",
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

            if (save == null)
                save = File.ReadAllBytes(file.FileName);
            else
            {
                byte[] read = File.ReadAllBytes(file.FileName);
                save = Utilities.add(save, read);
            }
        }

        private void saveNHBSbtn_Click(object sender, EventArgs e)
        {
            if (save == null)
                return;
            else
            {
                SaveFileDialog file = new SaveFileDialog()
                {
                    Filter = "New Horizons Bulk Spawn (*.nhbs)|*.nhbs",
                };

                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

                string savepath;

                if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                    savepath = Directory.GetCurrentDirectory() + @"\save";
                else
                    savepath = config.AppSettings.Settings["LastSave"].Value;

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


                string dataStr = Utilities.ByteToHexString(save).Replace("FEFF000000000000", string.Empty);
                byte[] final = Utilities.stringToByte(dataStr);

                File.WriteAllBytes(file.FileName, final);
                if (sound)
                    System.Media.SystemSounds.Asterisk.Play();
            }
        }

        private void unhideBtn_Click(object sender, EventArgs e)
        {
            if (currentPanel == itemModePanel)
            {
                itemGridView.Columns["id"].Visible = true;
                itemGridView.Columns["iName"].Visible = true;
            }
            else if (currentPanel == recipeModePanel)
            {
                recipeGridView.Columns["id"].Visible = true;
                recipeGridView.Columns["iName"].Visible = true;
            }
            else
                return;
        }

        private void nhbsBtn_Click(object sender, EventArgs e)
        {
            string bank = "";
            for (int i = 0; i < itemGridView.Rows.Count; i++)
            {
                string id = itemGridView.Rows[i].Cells["id"].Value.ToString();
                bank = bank + Utilities.flip(id) + "000000000000";
            }

            byte[] save = Utilities.stringToByte(bank);


            SaveFileDialog file = new SaveFileDialog()
            {
                Filter = "New Horizons Bulk Spawn (*.nhbs)|*.nhbs",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            string savepath;

            if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + @"\save";
            else
                savepath = config.AppSettings.Settings["LastSave"].Value;

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

            File.WriteAllBytes(file.FileName, save);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }


        private void nhbsIDBtn_Click(object sender, EventArgs e)
        {
            string bank = "";
            recipeGridView.Sort(recipeGridView.Columns["id"], ListSortDirection.Ascending);

            for (int i = 0; i < recipeGridView.Rows.Count; i++)
            {
                string recipeid = recipeGridView.Rows[i].Cells["id"].Value.ToString();

                bank = bank + Utilities.flip("16A2") + "0000" + Utilities.flip(recipeid) + "0000";
            }

            byte[] save = Utilities.stringToByte(bank);


            SaveFileDialog file = new SaveFileDialog()
            {
                Filter = "New Horizons Bulk Spawn (*.nhbs)|*.nhbs",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            string savepath;

            if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + @"\save";
            else
                savepath = config.AppSettings.Settings["LastSave"].Value;

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

            File.WriteAllBytes(file.FileName, save);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void nhbsAZBtn_Clicked(object sender, EventArgs e)
        {
            string bank = "";
            recipeGridView.Sort(recipeGridView.Columns["eng"], ListSortDirection.Ascending);

            for (int i = 0; i < recipeGridView.Rows.Count; i++)
            {
                string recipeid = recipeGridView.Rows[i].Cells["id"].Value.ToString();

                bank = bank + Utilities.flip("16A2") + "0000" + Utilities.flip(recipeid) + "0000";
            }

            byte[] save = Utilities.stringToByte(bank);


            SaveFileDialog file = new SaveFileDialog()
            {
                Filter = "New Horizons Bulk Spawn (*.nhbs)|*.nhbs",
            };

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            string savepath;

            if (config.AppSettings.Settings["LastSave"].Value.Equals(string.Empty))
                savepath = Directory.GetCurrentDirectory() + @"\save";
            else
                savepath = config.AppSettings.Settings["LastSave"].Value;

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

            File.WriteAllBytes(file.FileName, save);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void DiffIDBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog loadfile = new OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*",
            };

            if (loadfile.ShowDialog() != DialogResult.OK)
                return;

            string[] temp = loadfile.FileName.Split('\\');
            string path = "";
            for (int i = 0; i < temp.Length - 1; i++)
                path = path + temp[i] + "\\";

            DataTable tempSource = loadItemCSV(loadfile.FileName);

            string bank = "";
            tempSource.DefaultView.Sort = "id ASC";
            tempSource.DefaultView.ToTable();

            for (int i = 0; i < tempSource.DefaultView.ToTable().Rows.Count; i++)
            {
                string recipeid = tempSource.DefaultView.ToTable().Rows[i].ItemArray[0].ToString();

                bank = bank + Utilities.flip("16A2") + "0000" + Utilities.flip(recipeid) + "0000";
            }

            byte[] save = Utilities.stringToByte(bank);


            SaveFileDialog savefile = new SaveFileDialog()
            {
                Filter = "New Horizons Bulk Spawn (*.nhbs)|*.nhbs",
            };

            if (savefile.ShowDialog() != DialogResult.OK)
                return;

            File.WriteAllBytes(savefile.FileName, save);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void DiffAtoZBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog loadfile = new OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*",
            };

            if (loadfile.ShowDialog() != DialogResult.OK)
                return;

            string[] temp = loadfile.FileName.Split('\\');
            string path = "";
            for (int i = 0; i < temp.Length - 1; i++)
                path = path + temp[i] + "\\";

            DataTable tempSource = loadItemCSV(loadfile.FileName);

            string bank = "";
            tempSource.DefaultView.Sort = "eng ASC";

            for (int i = 0; i < tempSource.DefaultView.ToTable().Rows.Count; i++)
            {
                string recipeid = tempSource.DefaultView.ToTable().Rows[i].ItemArray[0].ToString();

                bank = bank + Utilities.flip("16A2") + "0000" + Utilities.flip(recipeid) + "0000";
            }

            byte[] save = Utilities.stringToByte(bank);


            SaveFileDialog savefile = new SaveFileDialog()
            {
                Filter = "New Horizons Bulk Spawn (*.nhbs)|*.nhbs",
            };

            if (savefile.ShowDialog() != DialogResult.OK)
                return;

            File.WriteAllBytes(savefile.FileName, save);
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void SysbotbaseVersionBtn_Click(object sender, EventArgs e)
        {
            myMessageBox.Show(Utilities.getVersion(s), "Sys-botbase Version");
        }

    }
}
