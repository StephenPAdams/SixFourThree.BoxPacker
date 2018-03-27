using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SixFourThree.BoxPacker.Exceptions;
using SixFourThree.BoxPacker.Helpers;
using SixFourThree.BoxPacker.Model;

namespace SixFourThree.BoxPacker.Tests
{
    [TestFixture]
    public class PackerTests
    {

        [Test]
        public void CanPackByVolume()
        {
            var item1 = new Item()
            {
                Description = "My Cube 5x5x5",
                Depth = 5,
                Length = 5,
                Weight = 5,
                Width = 5
            };

            var box = new Box()
            {
                Description = "My Box 230x300x240",
                OuterDepth = 230,
                OuterLength = 300,
                OuterWidth = 240,
                EmptyWeight = 160,
                InnerDepth = 230,
                InnerLength = 300,
                InnerWidth = 240,
                MaxWeight = 15000
            };

            var packer = new Packer();
            packer.AddBox(box);
            packer.AddItem(item1, 200);

            var packedBoxes = packer.PackByVolume();

            Assert.NotNull(packedBoxes);
            Assert.Greater(packedBoxes.GetCount(), 0);
        }

        [Test]
        public void CanPackByVolumeFor6OfSameItem()
        {
            var item1 = new Item()
            {
                Description = "6x6x7",
                Depth = 178,
                Length = 152,
                Weight = 272,
                Width = 152
            };

            var box = new Box()
            {
                Description = "20x20x7",
                OuterDepth = 178,
                OuterLength = 508,
                OuterWidth = 508,
                EmptyWeight = 1,
                InnerDepth = 178,
                InnerLength = 508,
                InnerWidth = 508,
                MaxWeight = 32000
            };

            var packer = new Packer();
            packer.AddBox(box);
            packer.AddItem(item1, 6);

            var packedBoxes = packer.PackByVolume();
            
            TestContext.WriteLine(packedBoxes.GetCount());
        }

        [Test]
        public void CanPackBoxes()
        {
            var item1 = new Item()
            {
                Description = "My Cube 5x5x5",
                Depth = 5,
                Length = 5,
                Weight = 5,
                Width = 5
            };

            var box1 = new Box()
            {
                Description = "My Box 230x300x240",
                OuterDepth = 230,
                OuterLength = 300,
                OuterWidth = 240,
                EmptyWeight = 10,
                InnerDepth = 230,
                InnerLength = 300,
                InnerWidth = 240,
                MaxWeight = 250
            };

            var box2 = new Box()
            {
                Description = "My Box 100x200x300",
                OuterDepth = 100,
                OuterLength = 200,
                OuterWidth = 300,
                EmptyWeight = 10,
                InnerDepth = 100,
                InnerLength = 200,
                InnerWidth = 300,
                MaxWeight = 150
            };

            var box3 = new Box()
            {
                Description = "My Box 75x250x100",
                OuterDepth = 75,
                OuterLength = 250,
                OuterWidth = 100,
                EmptyWeight = 10,
                InnerDepth = 75,
                InnerLength = 250,
                InnerWidth = 100,
                MaxWeight = 150
            };

            var packer = new Packer();

            packer.AddBox(box1);
            packer.AddBox(box2);
            packer.AddBox(box3); 
            packer.AddItem(item1, 120);

            var packedBoxes = packer.Pack();

            Assert.NotNull(packedBoxes);
            Assert.Greater(packedBoxes.GetCount(), 0);
        }

        [Test]
        public void PackedBoxesCanGenerateId()
        {
            var item1 = new Item()
            {
                Description = "My Cube 5x5x5",
                Depth = 5,
                Length = 5,
                Weight = 5,
                Width = 5
            };

            var box1 = new Box()
            {
                Description = "My Box 230x300x240",
                OuterDepth = 230,
                OuterLength = 300,
                OuterWidth = 240,
                EmptyWeight = 10,
                InnerDepth = 230,
                InnerLength = 300,
                InnerWidth = 240,
                MaxWeight = 250
            };

            var box2 = new Box()
            {
                Description = "My Box 100x200x300",
                OuterDepth = 100,
                OuterLength = 200,
                OuterWidth = 300,
                EmptyWeight = 10,
                InnerDepth = 100,
                InnerLength = 200,
                InnerWidth = 300,
                MaxWeight = 150
            };

            var box3 = new Box()
            {
                Description = "My Box 75x250x100",
                OuterDepth = 75,
                OuterLength = 250,
                OuterWidth = 100,
                EmptyWeight = 10,
                InnerDepth = 75,
                InnerLength = 250,
                InnerWidth = 100,
                MaxWeight = 150
            };

            var packer = new Packer();

            packer.AddBox(box1);
            packer.AddBox(box2);
            packer.AddBox(box3);
            packer.AddItem(item1, 120);

            var packedBoxes = packer.Pack();

            Assert.NotNull(packedBoxes);
            Assert.Greater(packedBoxes.GetCount(), 0);

            foreach (var packedBox in packedBoxes.GetContent().Cast<PackedBox>())
            {
                Assert.NotNull(packedBox.GeneratedId);
            }
        }

        [Test]
        public void CanPackBoxThreeItemsFitEasily()
        {   
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Box",
                OuterWidth = 300,
                OuterLength = 300,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 296,
                InnerLength = 296,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 250, Length = 250, Depth = 2, Weight = 200 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 250, Length = 250, Depth = 2, Weight = 200 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 250, Length = 250, Depth = 2, Weight = 200 });

            var packedItems = packer.PackIntoBox(box, items);

            Assert.AreEqual(3, packedItems.GetItems().GetCount());
        }

        [Test]
        public void CanPackBoxThreeItemsFitExactly()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Box",
                OuterWidth = 300,
                OuterLength = 300,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 296,
                InnerLength = 296,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 296, Length = 296, Depth = 2, Weight = 200 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 296, Length = 296, Depth = 2, Weight = 500 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 296, Length = 296, Depth = 4, Weight = 290 });

            var packedItems = packer.PackIntoBox(box, items);

            Assert.AreEqual(3, packedItems.GetItems().GetCount());
        }

        [Test]
        public void CanPackBoxTwoItemsFitExactlyNoRotation()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Box",
                OuterWidth = 300,
                OuterLength = 300,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 296,
                InnerLength = 296,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 296, Length = 148, Depth = 2, Weight = 200 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 296, Length = 148, Depth = 2, Weight = 500 });

            var packedItems = packer.PackIntoBox(box, items);

            Assert.AreEqual(2, packedItems.GetItems().GetCount());
        }

        [Test]
        public void CanPackBoxTwoItemsFitSizeButOverweight()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Box",
                OuterWidth = 300,
                OuterLength = 300,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 296,
                InnerLength = 296,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 250, Length = 250, Depth = 2, Weight = 400 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 250, Length = 250, Depth = 2, Weight = 500 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 250, Length = 250, Depth = 2, Weight = 200 });

            var packedItems = packer.PackIntoBox(box, items);

            Assert.AreEqual(2, packedItems.GetItems().GetCount());
        }

        [Test]
        public void CanPackBoxTwoItemsFitWeightBut2Oversize()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Box",
                OuterWidth = 300,
                OuterLength = 300,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 296,
                InnerLength = 296,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 297, Length = 296, Depth = 2, Weight = 200 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 297, Length = 296, Depth = 2, Weight = 500 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 296, Length = 296, Depth = 4, Weight = 290 });

            var packedItems = packer.PackIntoBox(box, items);

            Assert.AreEqual(1, packedItems.GetItems().GetCount());
        }
        [Test]
        public void CanPackThreeItemsFitEasilyInSmallerOfTwoBoxes()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Petite Box",
                OuterWidth = 300,
                OuterLength = 300,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 296,
                InnerLength = 296,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var box2 = new Box()
            {
                Description = "Le Grande Box",
                OuterWidth = 3000,
                OuterLength = 3000,
                OuterDepth = 100,
                EmptyWeight = 100,
                InnerWidth = 2960,
                InnerLength = 2960,
                InnerDepth = 80,
                MaxWeight = 10000
            };
            
            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 250, Length = 250, Depth = 2, Weight = 200 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 250, Length = 250, Depth = 2, Weight = 200 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 250, Length = 250, Depth = 2, Weight = 200 });

            packer.AddBox(box);
            packer.AddBox(box2);
            packer.AddItems(items);

            var packedBoxes = packer.Pack();

            Assert.AreEqual(1, packedBoxes.GetCount());
            Assert.AreEqual(3, packedBoxes.GetBest().GetItems().GetCount());
            Assert.AreEqual(box, packedBoxes.GetBest().GetBox());
            Assert.AreEqual(610, packedBoxes.GetBest().GetWeight());
        }
        [Test]
        public void CanPackThreeItemsFitEasilyInLargerOfTwoBoxes()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Petite Box",
                OuterWidth = 300,
                OuterLength = 300,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 296,
                InnerLength = 296,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var box2 = new Box()
            {
                Description = "Le Grande Box",
                OuterWidth = 3000,
                OuterLength = 3000,
                OuterDepth = 100,
                EmptyWeight = 100,
                InnerWidth = 2960,
                InnerLength = 2960,
                InnerDepth = 80,
                MaxWeight = 10000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });

            packer.AddBox(box);
            packer.AddBox(box2);
            packer.AddItems(items);

            var packedBoxes = packer.Pack();

            Assert.AreEqual(1, packedBoxes.GetCount());
            Assert.AreEqual(3, packedBoxes.GetBest().GetItems().GetCount());
            Assert.AreEqual(box2, packedBoxes.GetBest().GetBox());
            Assert.AreEqual(6100, packedBoxes.GetBest().GetWeight());
        }

        [Test]
        public void CanPackFiveItemsTwoLargeOneSmallBox()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Petite Box",
                OuterWidth = 600,
                OuterLength = 600,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 596,
                InnerLength = 596,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var box2 = new Box()
            {
                Description = "Le Grande Box",
                OuterWidth = 3000,
                OuterLength = 3000,
                OuterDepth = 50,
                EmptyWeight = 100,
                InnerWidth = 2960,
                InnerLength = 2960,
                InnerDepth = 40,
                MaxWeight = 10000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 2500, Length = 2500, Depth = 20, Weight = 500 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 550, Length = 550, Depth = 2, Weight = 500 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 2500, Length = 2500, Depth = 20, Weight = 500 });
            items.Insert(new Item() { Id = "Item 4", Description = "Item 4", Width = 2500, Length = 2500, Depth = 20, Weight = 500 });
            items.Insert(new Item() { Id = "Item 5", Description = "Item 5", Width = 2500, Length = 2500, Depth = 20, Weight = 500 });

            packer.AddBox(box);
            packer.AddBox(box2);
            packer.AddItems(items);

            var packedBoxes = packer.Pack();
            
            Assert.AreEqual(3, packedBoxes.GetCount());

            Assert.AreEqual(1, packedBoxes.GetBest().GetItems().GetCount());
            Assert.AreEqual(box, packedBoxes.GetBest().GetBox());
            Assert.AreEqual(510, packedBoxes.GetBest().GetWeight());

            packedBoxes.ExtractBest();
            Assert.AreEqual(2, packedBoxes.GetBest().GetItems().GetCount());
            Assert.AreEqual(box2, packedBoxes.GetBest().GetBox());
            Assert.AreEqual(1100, packedBoxes.GetBest().GetWeight());
            
            packedBoxes.ExtractBest();
            Assert.AreEqual(2, packedBoxes.GetBest().GetItems().GetCount());
            Assert.AreEqual(box2, packedBoxes.GetBest().GetBox());
            Assert.AreEqual(1100, packedBoxes.GetBest().GetWeight());
        }

        [Test]
        public void CanPackFiveItemsTwoLargeOneSmallBoxButThreeAfterRepack()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Petite Box",
                OuterWidth = 600,
                OuterLength = 600,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 596,
                InnerLength = 596,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var box2 = new Box()
            {
                Description = "Le Grande Box",
                OuterWidth = 3000,
                OuterLength = 3000,
                OuterDepth = 50,
                EmptyWeight = 100,
                InnerWidth = 2960,
                InnerLength = 2960,
                InnerDepth = 40,
                MaxWeight = 10000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 550, Length = 550, Depth = 2, Weight = 200 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });
            items.Insert(new Item() { Id = "Item 4", Description = "Item 4", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });
            items.Insert(new Item() { Id = "Item 5", Description = "Item 5", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });

            packer.AddBox(box);
            packer.AddBox(box2);
            packer.AddItems(items);

            var packedBoxes = packer.Pack();

            Assert.AreEqual(3, packedBoxes.GetCount());

            Assert.AreEqual(1, packedBoxes.GetBest().GetItems().GetCount());
            Assert.AreEqual(box, packedBoxes.GetBest().GetBox());
            Assert.AreEqual(210, packedBoxes.GetBest().GetWeight());

            packedBoxes.ExtractBest();
            Assert.AreEqual(2, packedBoxes.GetBest().GetItems().GetCount());
            Assert.AreEqual(box2, packedBoxes.GetBest().GetBox());
            Assert.AreEqual(4100, packedBoxes.GetBest().GetWeight());

            packedBoxes.ExtractBest();
            Assert.AreEqual(2, packedBoxes.GetBest().GetItems().GetCount());
            Assert.AreEqual(box2, packedBoxes.GetBest().GetBox());
            Assert.AreEqual(4100, packedBoxes.GetBest().GetWeight());            
        }

        [Test]
        public void PackThreeItemsOneDoesntFitInAnyBoxThrowsTooLargeException()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Petite Box",
                OuterWidth = 300,
                OuterLength = 300,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 296,
                InnerLength = 296,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var box2 = new Box()
            {
                Description = "Le Grande Box",
                OuterWidth = 3000,
                OuterLength = 3000,
                OuterDepth = 100,
                EmptyWeight = 100,
                InnerWidth = 2960,
                InnerLength = 2960,
                InnerDepth = 80,
                MaxWeight = 10000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 25000, Length = 25000, Depth = 20, Weight = 2000 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });

            packer.AddBox(box);
            packer.AddBox(box2);
            packer.AddItems(items);
            
            Assert.Throws<ItemTooLargeException>(() => packer.Pack());
        }

        [Test]
        public void PackItemsWithoutABoxThrowsException()
        {
            var packer = new Packer();

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 25000, Length = 25000, Depth = 20, Weight = 2000 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 2500, Length = 2500, Depth = 20, Weight = 2000 });
;
            packer.AddItems(items);

            Assert.Throws<ItemTooLargeException>(() => packer.Pack());
        }

        [Test]
        public void CanPackTwoItemsFitExactlySideBySide()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Box",
                OuterWidth = 300,
                OuterLength = 400,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 296,
                InnerLength = 496,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 296, Length = 248, Depth = 8, Weight = 200 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 248, Length = 296, Depth = 8, Weight = 200 });
            
            var packedItems = packer.PackIntoBox(box, items);

            Assert.AreEqual(2, packedItems.GetItems().GetCount());
        }

        [Test]
        public void CanPackThreeItemsBottom2FitSideBySideOneExactlyOnTop()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Box",
                OuterWidth = 300,
                OuterLength = 400,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 296,
                InnerLength = 496,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 248, Length = 148, Depth = 4, Weight = 200 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 148, Length = 248, Depth = 4, Weight = 200 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 296, Length = 296, Depth = 4, Weight = 200 });

            var packedItems = packer.PackIntoBox(box, items);

            Assert.AreEqual(3, packedItems.GetItems().GetCount());
        }

        [Test]
        public void CanPackThreeItemsBottom2FitSideBySideWithSpareSpaceOneOverhangSlightlyOnTop()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Box",
                OuterWidth = 250,
                OuterLength = 250,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 248,
                InnerLength = 248,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 200, Length = 200, Depth = 4, Weight = 200 });
            items.Insert(new Item() { Id = "Item 2", Description = "Item 2", Width = 110, Length = 110, Depth = 4, Weight = 200 });
            items.Insert(new Item() { Id = "Item 3", Description = "Item 3", Width = 110, Length = 110, Depth = 4, Weight = 200 });

            var packedItems = packer.PackIntoBox(box, items);

            Assert.AreEqual(3, packedItems.GetItems().GetCount());
        }

        [Test]
        public void CanPackSingleItemFitsBetterRotated()
        {
            var packer = new Packer();
            var box = new Box()
            {
                Description = "Le Box",
                OuterWidth = 400,
                OuterLength = 300,
                OuterDepth = 10,
                EmptyWeight = 10,
                InnerWidth = 396,
                InnerLength = 296,
                InnerDepth = 8,
                MaxWeight = 1000
            };

            var items = new ItemList();
            items.Insert(new Item() { Id = "Item 1", Description = "Item 1", Width = 250, Length = 290, Depth = 2, Weight = 200 });

            var packedItems = packer.PackIntoBox(box, items);

            Assert.AreEqual(1, packedItems.GetItems().GetCount());
        }

        [Test]
        public void CanPackMultipleSameItemsInSingleBox()
        {
            var item = new Item()
            {
                Description = "My Cube 6x6x6",
                Depth = (Int32)ConversionHelper.ConvertInchesToMillimeters(6),
                Length = (Int32)ConversionHelper.ConvertInchesToMillimeters(6),
                Weight = (Int32)ConversionHelper.ConvertPoundsToGrams(6),
                Width = (Int32)ConversionHelper.ConvertInchesToMillimeters(6)
            };

            var box = new Box()
            {
                Description = "12x12x8",
                OuterDepth = (Int32)ConversionHelper.ConvertInchesToMillimeters(8),
                OuterLength = (Int32)ConversionHelper.ConvertInchesToMillimeters(12),
                OuterWidth = (Int32)ConversionHelper.ConvertInchesToMillimeters(12),
                EmptyWeight = (Int32)ConversionHelper.ConvertPoundsToGrams(1),
                InnerDepth = (Int32)ConversionHelper.ConvertInchesToMillimeters(8),
                InnerLength = (Int32)ConversionHelper.ConvertInchesToMillimeters(12),
                InnerWidth = (Int32)ConversionHelper.ConvertInchesToMillimeters(12),
                MaxWeight = (Int32)ConversionHelper.ConvertPoundsToGrams(70)
            };

            var packer = new Packer();

            packer.AddBox(box);
            packer.AddItem(item, 6);

            var packedBoxes = packer.Pack();

            Assert.NotNull(packedBoxes);
            Assert.Greater(packedBoxes.GetCount(), 0);
            
            TestContext.WriteLine("Total packed boxes: " + packedBoxes.GetCount());
        }

        [Test]
        public void CustomBoxesGeneratesIds()
        {
            var item1 = new Item()
            {
                Description = "My Cube 5x5x5",
                Depth = 5,
                Length = 5,
                Weight = 5,
                Width = 5
            };

            var box1 = new Box()
            {
                Description = "My Box 1x1x1",
                OuterDepth = 1,
                OuterLength = 1,
                OuterWidth = 1,
                EmptyWeight = 1,
                InnerDepth = 1,
                InnerLength = 1,
                InnerWidth = 1,
                MaxWeight = 2
            };

            var packer = new Packer(true);

            packer.AddBox(box1);
            packer.AddItem(item1, 1);

            var packedBoxes = packer.Pack();

            foreach (var packedBox in packedBoxes.GetContent().Cast<PackedBox>())
            {
                var box = packedBox.GetBox();
                Assert.IsNotEmpty(box.GeneratedId);
            }
        }

        [Test]
        public void OversizedItemsCanCreateCustomBoxesIfEnabled()
        {
            var item1 = new Item()
            {
                Description = "My Cube 5x5x5",
                Depth = 5,
                Length = 5,
                Weight = 5,
                Width = 5
            };

            var box1 = new Box()
            {
                Description = "My Box 1x1x1",
                OuterDepth = 1,
                OuterLength = 1,
                OuterWidth = 1,
                EmptyWeight = 1,
                InnerDepth = 1,
                InnerLength = 1,
                InnerWidth = 1,
                MaxWeight = 2
            };

            var packer = new Packer(true);

            packer.AddBox(box1);
            packer.AddItem(item1, 1);

            var packedBoxes = packer.Pack();

            Assert.NotNull(packedBoxes);
            Assert.Greater(packedBoxes.GetCount(), 0);
        }
    }
}
