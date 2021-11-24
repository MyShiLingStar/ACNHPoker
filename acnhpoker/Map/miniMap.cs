using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows;

namespace ACNHPoker
{
    class miniMap
    {
        private byte[][] ItemMapData;
        private int[][] tilesType;

        private byte[] AcreMapByte;
        private byte[] BuildingByte;
        private byte[][] buildingList = null;

        private const int numOfColumn = 0x70;
        private const int numOfRow = 0x60;
        private const int columnSize = 0xC00;
        private const int numOfTiles = 0x2A00;

        private static int mapSize = 2;
        private static int plazaX = -1;
        private static int plazaY = -1;
        private static int plazaTopX = -1;
        private static int plazaTopY = -1;


        private byte[] AcreData = ACNHPoker.Properties.Resources.acre;
        private static Color[][] floorBackgroundColor;

        public const int MapTileCount16x16 = 16 * 16 * 7 * 6;
        public const int TerrainTileSize = 0xE;

        public const int AllTerrainSize = MapTileCount16x16 * TerrainTileSize;

        private const int BuildingSize = 0x14;
        private const int NumOfBuilding = 46;
        private const int AllBuildingSize = NumOfBuilding * BuildingSize;

        public const int AcreWidth = 7 + (2 * 1);
        private const int AcreHeight = 6 + (2 * 1);
        private const int AcreMax = AcreWidth * AcreHeight;
        private const int AllAcreSize = AcreMax * 2;
        private const int AcrePlusAdditionalParams = AllAcreSize + 2 + 2 + 4 + 4;
        public miniMap(byte[] ItemMapByte, byte[] acreMapByte, byte[] buildingByte, int size = 2)
        {
            AcreMapByte = acreMapByte;
            BuildingByte = buildingByte;

            if (AcreMapByte != null)
            {
                plazaX = AcreMapByte[AcreMax * 2 + 4];
                plazaY = AcreMapByte[AcreMax * 2 + 8];
                plazaTopX = (plazaX - 0x20) / 2;
                plazaTopY = (plazaY - 0x20) / 2;
            }

            ItemMapData = new byte[numOfColumn][];
            mapSize = size;
            for (int i = 0; i < numOfColumn; i++)
            {
                ItemMapData[i] = new byte[columnSize];
                Buffer.BlockCopy(ItemMapByte, i * columnSize, ItemMapData[i], 0x0, columnSize);
            }
            transformItemMap();
        }

        public Bitmap drawBackground()
        {
                byte[] AllAcre = new byte[AcreMax];

                for (int i = 0; i < AcreMax; i++)
                {
                    AllAcre[i] = AcreMapByte[i * 2];
                }

                byte[] AcreWOOutside = new byte[7 * 6];

                for (int i = 1; i <= 6; i++)
                {
                    Buffer.BlockCopy(AllAcre, i * 9 + 1, AcreWOOutside, (i - 1) * 7, 7);
                }


                Bitmap buildingMap = drawBuildingMap();
                buildBackgroundColor(AcreWOOutside);

                Bitmap[] AcreImage = new Bitmap[7 * 6];

                for (int i = 0; i < AcreImage.Length; i++)
                {
                    AcreImage[i] = DrawAcre(GetAcreData(AcreWOOutside[i]));
                    //AcreImage[i].Save(i + ".bmp");
                }

                return combineMap(toFullMap(AcreImage), buildingMap);
            /*
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Bitmap myBitmap = new Bitmap(16 * 7 * mapSize, 16 * 6 * mapSize);
                Graphics g = Graphics.FromImage(myBitmap);
                g.Clear(Color.White);
                return myBitmap;
            }
            */
        }

        public Bitmap drawBuildingMap()
        {
            if (BuildingByte != null)
            {
                buildingList = new byte[NumOfBuilding][];
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    buildingList[i] = new byte[BuildingSize];
                    Buffer.BlockCopy(BuildingByte, i * BuildingSize, buildingList[i], 0x0, BuildingSize);
                }
            }


            Bitmap myBitmap;

            myBitmap = new Bitmap(numOfColumn * mapSize, numOfRow * mapSize);

            using (Graphics gr = Graphics.FromImage(myBitmap))
            {
                gr.SmoothingMode = SmoothingMode.None;

                //plaza
                for (int m = plazaTopX; m <= plazaTopX + 11; m++)
                {
                    if (m < 0)
                        continue;
                    for (int n = plazaTopY; n <= plazaTopY + 9; n++)
                    {
                        if (n < 0)
                            continue;
                        PutPixel(gr, m * mapSize, n * mapSize, Color.DarkSalmon);
                    }
                }
                //========================================
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    int buildingTopX = 0;
                    int buildingTopY = 0;
                    int buildingBottomX = -1;
                    int buildingBottomY = -1;
                    Color BuildingColor = Color.White;

                    BuildingType CurrentBuilding = (BuildingType)buildingList[i][0];
                    int BuildingX = (buildingList[i][2] - 0x20) / 2;
                    int BuildingY = (buildingList[i][4] - 0x20) / 2;

                    if (CurrentBuilding != BuildingType.None)
                    {
                        PutPixel(gr, BuildingX * mapSize, BuildingY * mapSize, Color.Red);
                        
                        if (CurrentBuilding == BuildingType.ResidentServicesBuilding)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY;
                            BuildingColor = Color.MediumSlateBlue;
                        }
                        else if (CurrentBuilding >= BuildingType.PlayerHouse1 && CurrentBuilding <= BuildingType.PlayerHouse8)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY;
                            BuildingColor = Color.RoyalBlue;
                        }
                        else if (CurrentBuilding >= BuildingType.VillagerHouse1 && CurrentBuilding <= BuildingType.VillagerHouse10)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 1;
                            buildingBottomY = BuildingY;
                            BuildingColor = Color.Chocolate;
                        }
                        else if (CurrentBuilding == BuildingType.NooksCranny)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY + 2;
                            BuildingColor = Color.Gold;
                        }
                        else if (CurrentBuilding == BuildingType.Museum)
                        {
                            buildingTopX = BuildingX - 4;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 3;
                            buildingBottomY = BuildingY + 1;
                            BuildingColor = Color.PapayaWhip;
                        }
                        else if (CurrentBuilding == BuildingType.AblesSisters)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY + 1;
                            BuildingColor = Color.DarkBlue;
                        }
                        else if (CurrentBuilding == BuildingType.Airport)
                        {
                            buildingTopX = BuildingX - 6;
                            buildingTopY = BuildingY - 3;
                            buildingBottomX = BuildingX + 4;
                            buildingBottomY = BuildingY + 3;
                            BuildingColor = Color.IndianRed;
                        }
                        else if (CurrentBuilding == BuildingType.Campsite)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 1;
                            buildingBottomY = BuildingY + 1;
                            BuildingColor = Color.LightGoldenrodYellow;
                        }
                        else if (CurrentBuilding == BuildingType.Bridge || CurrentBuilding == BuildingType.Incline)
                        {
                            buildingTopX = BuildingX - 1;
                            buildingTopY = BuildingY - 1;
                            buildingBottomX = BuildingX;
                            buildingBottomY = BuildingY;
                            BuildingColor = Color.Red;
                        }



                        for (int j = buildingTopX; j <= buildingBottomX; j++)
                        {
                            if (j < 0 || j >= numOfColumn)
                                continue;
                            for (int k = buildingTopY; k <= buildingBottomY; k++)
                            {
                                if (k < 0 || k >= numOfRow)
                                    continue;
                                PutPixel(gr, j * mapSize, k * mapSize, BuildingColor);
                            }
                        }
                    }
                }
            }

            return myBitmap;
        }

        private byte[] GetAcreData(byte Acre)
        {
            byte[] data = new byte[0x100];
            Buffer.BlockCopy(AcreData, Acre * 0x100, data, 0, 0x100);
            return data;
        }

        public Bitmap DrawAcre(byte[] OneAcre)
        {
            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * mapSize, 16 * mapSize);

            using (Graphics gr = Graphics.FromImage(myBitmap))
            {
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        PutPixel(gr, j * mapSize, i * mapSize, Pixel[OneAcre[i * 0x10 + j]]);
                    }
                }
            }

            return myBitmap;
        }

        private Bitmap toFullMap(Bitmap[] AcreImage)
        {
            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * 7 * mapSize, 16 * 6 * mapSize);

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                var cm = new ColorMatrix();
                cm.Matrix33 = 1;

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                int ImageNum = 0;

                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        graphics.DrawImage(AcreImage[ImageNum], new Rectangle(j * 16 * mapSize, i * 16 * mapSize, AcreImage[ImageNum].Width, AcreImage[ImageNum].Height), 0, 0, AcreImage[ImageNum].Width, AcreImage[ImageNum].Height, GraphicsUnit.Pixel, ia);
                        ImageNum++;
                    }
                }
            }

            return myBitmap;
        }

        public Bitmap combineMap(Bitmap bottom, Bitmap top)
        {
            Bitmap myBitmap = bottom;

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                var cm = new ColorMatrix();
                cm.Matrix33 = 1;

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(top, new Rectangle(0, 0, top.Width, top.Height), 0, 0, top.Width, top.Height, GraphicsUnit.Pixel, ia);
            }
            

            return myBitmap;
        }

        public Image refreshItemMap(byte[] itemData)
        {
            ItemMapData = null;
            tilesType = null;

            ItemMapData = new byte[numOfColumn][];

            for (int i = 0; i < numOfColumn; i++)
            {
                ItemMapData[i] = new byte[columnSize];
                Buffer.BlockCopy(itemData, i * columnSize, ItemMapData[i], 0x0, columnSize);
            }
            transformItemMap();

            Image itemMap = drawItemMap();
            Image myBitmap = drawBackground();

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                var cm = new ColorMatrix();
                cm.Matrix33 = 1;

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(itemMap, new Rectangle(0, 0, itemMap.Width, itemMap.Height), 0, 0, itemMap.Width, itemMap.Height, GraphicsUnit.Pixel, ia);
            }

            return myBitmap;
        }

        public Bitmap drawSelectSquare(int x, int y)
        {
            Bitmap square = new Bitmap(7 * mapSize + 2, 7 * mapSize + 2);
            Pen p = new Pen(Color.Red, 2 * mapSize);
            using (Graphics g = Graphics.FromImage(square))
            {
                g.Clear(Color.Transparent);
                g.DrawRectangle(p, new Rectangle(0, 0, square.Width, square.Height));
            }


            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * 7 * mapSize, 16 * 6 * mapSize);

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                graphics.Clear(Color.Transparent);

                var cm = new ColorMatrix();
                cm.Matrix33 = 1;

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(square, new Rectangle(x * mapSize - square.Width / 2 + mapSize / 2, y * mapSize - square.Height / 2 + mapSize / 2, square.Width, square.Height), 0, 0, square.Width, square.Height, GraphicsUnit.Pixel, ia);
            }
            return myBitmap;
        }

        public Bitmap drawMarker(int x, int y)
        {
            Bitmap marker = new Bitmap(ACNHPoker.Properties.Resources.marker);

            Bitmap myBitmap;
            myBitmap = new Bitmap(16 * 7 * mapSize, 16 * 6 * mapSize);

            using (Graphics graphics = Graphics.FromImage(myBitmap))
            {
                graphics.Clear(Color.Transparent);

                var cm = new ColorMatrix();
                cm.Matrix33 = 1;

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(marker, new Rectangle(x * mapSize - marker.Width / 2 + mapSize / 2, y * mapSize - marker.Height + mapSize / 2, marker.Width, marker.Height), 0, 0, marker.Width, marker.Height, GraphicsUnit.Pixel, ia);
            }
            return myBitmap;
        }

        public Bitmap drawPreview(int row, int column, int x, int y, bool right)
        {
            int[][] previewtilesType = null;

            if (right)
            {
                previewtilesType = new int[numOfColumn][];

                for (int i = 0; i < numOfColumn; i++)
                {
                    previewtilesType[i] = new int[numOfRow];

                    for (int j = 0; j < numOfRow; j++)
                    {
                        if (i == x && j == y)
                            previewtilesType[i][j] = 2;
                        else if (i >= x && i < x + column && j >= y && j < y + row)
                            previewtilesType[i][j] = 1;
                        else
                            previewtilesType[i][j] = 0;
                    }
                }
            }
            else
            {
                previewtilesType = new int[numOfColumn][];

                for (int i = numOfColumn - 1; i >= 0; i--)
                {
                    previewtilesType[i] = new int[numOfRow];

                    for (int j = 0; j < numOfRow; j++)
                    {
                        if (i == x && j == y)
                            previewtilesType[i][j] = 2;
                        else if (i <= x && i > x - column && j >= y && j < y + row)
                            previewtilesType[i][j] = 1;
                        else
                            previewtilesType[i][j] = 0;
                    }
                }
            }



            Bitmap myBitmap;

            myBitmap = new Bitmap(numOfColumn * mapSize, numOfRow * mapSize);

            using (Graphics gr = Graphics.FromImage(myBitmap))
            {
                gr.SmoothingMode = SmoothingMode.None;

                for (int i = 0; i < numOfColumn; i++)
                {
                    for (int j = 0; j < numOfRow; j++)
                    {
                        if (previewtilesType[i][j] == 1)
                            PutPixel(gr, i * mapSize, j * mapSize, Color.FromArgb(200, Color.Red));
                        else if (previewtilesType[i][j] == 2)
                            PutPixel(gr, i * mapSize, j * mapSize, Color.LightSkyBlue);

                    }
                }
            }

            return myBitmap;
        }

        private void transformItemMap()
        {
            tilesType = new int[numOfColumn][];

            for (int i = 0; i < numOfColumn; i++)
            {
                tilesType[i] = new int[numOfRow];

                for (int j = 0; j < numOfRow; j++)
                {
                    byte[] tempPart1 = new byte[0x8];
                    byte[] tempPart2 = new byte[0x8];
                    byte[] tempPart3 = new byte[0x8];
                    byte[] tempPart4 = new byte[0x8];
                    byte[] IDByte = new byte[0x2];

                    Buffer.BlockCopy(ItemMapData[i], j * 0x10, tempPart1, 0x0, 0x8);
                    Buffer.BlockCopy(ItemMapData[i], j * 0x10 + 0x8, tempPart2, 0x0, 0x8);
                    Buffer.BlockCopy(ItemMapData[i], j * 0x10 + 0x600, tempPart3, 0x0, 0x8);
                    Buffer.BlockCopy(ItemMapData[i], j * 0x10 + 0x600 + 0x8, tempPart4, 0x0, 0x8);
                    Buffer.BlockCopy(ItemMapData[i], j * 0x10, IDByte, 0x0, 0x2);

                    string strPart1 = Utilities.ByteToHexString(tempPart1);
                    string strPart2 = Utilities.ByteToHexString(tempPart2);
                    string strPart3 = Utilities.ByteToHexString(tempPart3);
                    string strPart4 = Utilities.ByteToHexString(tempPart4);
                    ushort itemID = Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(IDByte)), 16);

                    if (ItemAttr.isTree(itemID))
                    {
                        tilesType[i][j] = 1; // Tree
                    }
                    else if (ItemAttr.hasGenetics(itemID))
                    {
                        tilesType[i][j] = 2; // Flower
                    }
                    else if (ItemAttr.isShell(itemID))
                    {
                        tilesType[i][j] = 3; // Shell
                    }
                    else if (ItemAttr.isWeed(itemID))
                    {
                        tilesType[i][j] = 4; // Weed
                    }
                    else if (ItemAttr.isFence(itemID))
                    {
                        tilesType[i][j] = 5; // Fence
                    }
                    else if (ItemAttr.isStone(itemID))
                    {
                        tilesType[i][j] = 6; // Stone
                    }
                    else if (ItemAttr.hasQuantity(itemID))
                    {
                        tilesType[i][j] = 9; // Material
                    }
                    else if (itemID == 0x16A2)
                    {
                        tilesType[i][j] = 10; // Recipe
                    }
                    else if (itemID == 0x315A || itemID == 0x1618 || itemID == 0x342F) // Wall-mount
                    {
                        tilesType[i][j] = 11;
                    }
                    else if (itemID == 0x12BA) // Hidden Chair
                    {
                        tilesType[i][j] = 12;
                    }
                    else if (strPart1.Equals("FEFF000000000000") && strPart2.Equals("FEFF000000000000") && strPart3.Equals("FEFF000000000000") && strPart4.Equals("FEFF000000000000"))
                    {
                        tilesType[i][j] = 0; // Empty
                    }
                    else
                    {
                        tilesType[i][j] = 69;
                    }
                }
            }
        }

        public Bitmap drawItemMap()
        {
                Bitmap myBitmap;

                myBitmap = new Bitmap(numOfColumn * mapSize, numOfRow * mapSize);

                using (Graphics gr = Graphics.FromImage(myBitmap))
                {
                    gr.SmoothingMode = SmoothingMode.None;

                    for (int i = 0; i < numOfColumn; i++)
                    {
                        for (int j = 0; j < numOfRow; j++)
                        {
                            if (tilesType[i][j] == 0)
                            {
                                //PutPixel(gr, i * mapSize, j* mapSize, Color.White);
                            }
                            else if (tilesType[i][j] == 1)
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.GreenYellow);
                            }
                            else if (tilesType[i][j] == 2)
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.Pink);
                            }
                            else if (tilesType[i][j] == 3)
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.Blue);
                            }
                            else if (tilesType[i][j] == 4)
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.MediumSeaGreen);
                            }
                            else if (tilesType[i][j] == 5)
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.Purple);
                            }
                            else if (tilesType[i][j] == 6)
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.Black);
                            }
                            else if (tilesType[i][j] == 9)
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.Yellow);
                            }
                            else if (tilesType[i][j] == 10)
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.Orange);
                            }
                            else if (tilesType[i][j] == 11)
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.Khaki);
                            }
                            else if (tilesType[i][j] == 12)
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.Maroon);
                            }
                            else
                            {
                                PutPixel(gr, i * mapSize, j * mapSize, Color.Gray);
                            }
                        }
                    }
                }

                return myBitmap;
            /*
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Bitmap myBitmap = new Bitmap(16 * 7 * mapSize, 16 * 6 * mapSize);
                Graphics g = Graphics.FromImage(myBitmap);
                g.Clear(Color.Transparent);
                return myBitmap;
            }
            */
        }

        private void buildBackgroundColor(byte[] AcreWOOutside)
        {
            floorBackgroundColor = new Color[numOfRow][];

            for (int i = 0; i < numOfRow; i++)
            {
                floorBackgroundColor[i] = new Color[numOfColumn];
                for (int j = 0; j < numOfColumn; j++)
                {
                    floorBackgroundColor[i][j] = Color.Tomato;
                }
            }

            for (int i = 0; i < 6; i++) // y
            {
                for (int j = 0; j < 7; j++) // x
                {
                    byte[] Acre = GetAcreData(AcreWOOutside[i * 7 + j]);

                    for (int m = 0; m < 16; m++) // y
                    {
                        for (int n = 0; n < 16; n++) // x
                        {
                            if (floorBackgroundColor[i * 16 + m][j * 16 + n] != Color.Tomato)
                                Debug.Print(i + " " + j + " " + m + " " + n);
                            floorBackgroundColor[i * 16 + m][j * 16 + n] = Pixel[Acre[m * 16 + n]];
                        }
                    }
                }
            }

            for (int m = plazaTopX; m <= plazaTopX + 11; m++)
            {
                if (m < 0 || m >= numOfColumn)
                    continue;
                for (int n = plazaTopY; n <= plazaTopY + 9; n++)
                {
                    if (n < 0 || n >= numOfRow)
                        continue;
                    floorBackgroundColor[n][m] = Color.DarkSalmon;
                }
            }

            if (buildingList != null)
            {
                for (int i = 0; i < NumOfBuilding; i++)
                {
                    int buildingTopX = 0;
                    int buildingTopY = 0;
                    int buildingBottomX = -1;
                    int buildingBottomY = -1;
                    Color BuildingColor = Color.White;

                    BuildingType CurrentBuilding = (BuildingType)buildingList[i][0];
                    int BuildingX = (buildingList[i][2] - 0x20) / 2;
                    int BuildingY = (buildingList[i][4] - 0x20) / 2;

                    if (CurrentBuilding != BuildingType.None)
                    {
                        if (BuildingX < 0 || BuildingX > numOfColumn || BuildingY < 0 || BuildingY > numOfRow)
                        {

                        }
                        else
                            floorBackgroundColor[BuildingY][BuildingX] = Color.Red;

                        if (CurrentBuilding == BuildingType.ResidentServicesBuilding)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY;
                            BuildingColor = Color.MediumSlateBlue;
                        }
                        else if (CurrentBuilding >= BuildingType.PlayerHouse1 && CurrentBuilding <= BuildingType.PlayerHouse8)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY;
                            BuildingColor = Color.RoyalBlue;
                        }
                        else if (CurrentBuilding >= BuildingType.VillagerHouse1 && CurrentBuilding <= BuildingType.VillagerHouse10)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 1;
                            buildingBottomY = BuildingY;
                            BuildingColor = Color.Chocolate;
                        }
                        else if (CurrentBuilding == BuildingType.NooksCranny)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY + 2;
                            BuildingColor = Color.Gold;
                        }
                        else if (CurrentBuilding == BuildingType.Museum)
                        {
                            buildingTopX = BuildingX - 4;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 3;
                            buildingBottomY = BuildingY + 1;
                            BuildingColor = Color.PapayaWhip;
                        }
                        else if (CurrentBuilding == BuildingType.AblesSisters)
                        {
                            buildingTopX = BuildingX - 3;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 2;
                            buildingBottomY = BuildingY + 1;
                            BuildingColor = Color.DarkBlue;
                        }
                        else if (CurrentBuilding == BuildingType.Airport)
                        {
                            buildingTopX = BuildingX - 5;
                            buildingTopY = BuildingY - 5;
                            buildingBottomX = BuildingX + 4;
                            buildingBottomY = BuildingY + 3;
                            BuildingColor = Color.IndianRed;
                        }
                        else if (CurrentBuilding == BuildingType.Campsite)
                        {
                            buildingTopX = BuildingX - 2;
                            buildingTopY = BuildingY - 2;
                            buildingBottomX = BuildingX + 1;
                            buildingBottomY = BuildingY + 1;
                            BuildingColor = Color.LightGoldenrodYellow;
                        }
                        else if (CurrentBuilding == BuildingType.Bridge || CurrentBuilding == BuildingType.Incline)
                        {
                            buildingTopX = BuildingX - 1;
                            buildingTopY = BuildingY - 1;
                            buildingBottomX = BuildingX;
                            buildingBottomY = BuildingY;
                            BuildingColor = Color.Red;
                        }

                        for (int j = buildingTopX; j <= buildingBottomX; j++)
                        {
                            if (j < 0 || j >= numOfColumn)
                                continue;
                            for (int k = buildingTopY; k <= buildingBottomY; k++)
                            {
                                if (k < 0 || k >= numOfRow)
                                    continue;
                                floorBackgroundColor[k][j] = BuildingColor;
                            }
                        }
                    }
                }
            }
        }

        public static Color GetBackgroundColor(int x, int y, bool Layer1 = true)
        {
            if (floorBackgroundColor == null)
                return Color.White;
            else if (Layer1)
                return floorBackgroundColor[y][x];
            else
            {
                Color newColor = Color.FromArgb(150, floorBackgroundColor[y][x]);
                return newColor;
            }
        }

        private void PutPixel(Graphics g, int x, int y, Color c)
        {
            Bitmap Bmp = new Bitmap(mapSize, mapSize);

            using (Graphics gfx = Graphics.FromImage(Bmp))
            using (SolidBrush brush = new SolidBrush(c))
            {
                gfx.SmoothingMode = SmoothingMode.None;
                gfx.FillRectangle(brush, 0, 0, mapSize, mapSize);
            }

            g.DrawImageUnscaled(Bmp, x, y);
        }

        private readonly Dictionary<byte, Color> Pixel = new Dictionary<byte, Color>
        {
                {0x00, Color.FromArgb(70, 116, 71)}, // Grass

                {0x04, Color.FromArgb(228, 216, 156)}, // Sand
                {0x05, Color.FromArgb(128, 200, 175)}, // Sea
                {0x06, Color.FromArgb(187, 121, 109)}, // Studio Bridge
                {0x07, Color.FromArgb(70, 116, 71)}, // Grass

                {0x0C, Color.FromArgb(21, 147, 229)}, // River Mouth

                {0x0F, Color.FromArgb(21, 147, 229)}, // River Mouth - River Edge

                {0x16, Color.FromArgb(187, 121, 109)}, // Studio Bridge
                {0x1D, Color.FromArgb(255, 244, 193)}, // Beach - Sea Edge

                {0x29, Color.FromArgb(109, 113, 124)}, // Walkable Rock
                {0x2A, Color.FromArgb(78, 83, 96)}, // High Rock
                {0x2B, Color.FromArgb(169, 255, 255)}, // Rock pool
                {0x2C, Color.FromArgb(144, 115, 104)}, // Beach - Grass Edge
                {0x2D, Color.FromArgb(187, 121, 109)}, // Pier
                {0x2E, Color.FromArgb(169, 255, 255)}, // Sea - Beach Edge

        };

        private enum BuildingType : byte
        {
            None = 0x0,
            PlayerHouse1 = 0x1,
            PlayerHouse2 = 0x2,
            PlayerHouse3 = 0x3,
            PlayerHouse4 = 0x4,
            PlayerHouse5 = 0x5,
            PlayerHouse6 = 0x6,
            PlayerHouse7 = 0x7,
            PlayerHouse8 = 0x8,
            VillagerHouse1 = 0x9,
            VillagerHouse2 = 0xA,
            VillagerHouse3 = 0xB,
            VillagerHouse4 = 0xC,
            VillagerHouse5 = 0xD,
            VillagerHouse6 = 0xE,
            VillagerHouse7 = 0xF,
            VillagerHouse8 = 0x10,
            VillagerHouse9 = 0x11,
            VillagerHouse10 = 0x12,
            NooksCranny = 0x13,
            ResidentServicesBuilding = 0x14,
            Museum = 0x15,
            Airport = 0x16,
            ResidentServicesTent = 0x17,
            AblesSisters = 0x18,
            Campsite = 0x19,
            Bridge = 0x1A,
            Incline = 0x1B,
            ReddsTreasureTrawler = 0x1C,
            Studio = 0x1D,
        }
    }
}
