using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SixFourThree.BoxPacker.Model;

namespace SixFourThree.BoxPacker.Tests
{
    [TestFixture]
    public class PackerTests
    {
        [Test]
        public void CanAddItem()
        {
			// Add a box
            var item = new Item()
                       {
                           Description = "My Cube",
                           Depth = 5,
                           Length = 5,
                           Weight = 5,
                           Width = 5
                       };

            var packer = new Packer();
            packer.AddItem(item, 1);

            var items = packer.GetItems();
            Assert.AreEqual(1, items.GetCount());

            var castItems = items.GetContent().Cast<Item>().ToList();
            Assert.AreEqual(1, castItems.Count);

            var firstItem = castItems.FirstOrDefault();
            Assert.NotNull(firstItem);
            Assert.AreEqual(item.Description, firstItem.Description);
        }

        [Test]
        public void CanAddItems()
        {
            var item1 = new Item()
            {
                Description = "My Cube 5x5x5",
                Depth = 5,
                Length = 5,
                Weight = 5,
                Width = 5
            };

            var item2 = new Item()
            {
                Description = "My Cube 10x10x10",
                Depth = 10,
                Length = 10,
                Weight = 10,
                Width = 10
            };

            var packer = new Packer();
            var newItems = new List<Item>() {item1, item2};

            packer.AddItems(newItems);

            var items = packer.GetItems();
            Assert.AreEqual(2, items.GetCount());

            var castItems = items.GetContent().Cast<Item>().ToList();
            Assert.AreEqual(2, castItems.Count);            
        }

        [Test]
        public void CanAddBox()
        {
            var box = new Box()
                      {
                          Description = "My Box 5x5x5",
                          OuterDepth = 5,
                          OuterLength = 5,
                          OuterWidth = 5,
                          MaxWeight = 100
                      };

            var packer = new Packer();
            packer.AddBox(box);

            var boxes = packer.GetBoxes();
            Assert.AreEqual(1, boxes.GetCount());

            var castItems = boxes.GetContent().Cast<Box>().ToList();
            Assert.AreEqual(1, castItems.Count);

            var firstItem = castItems.FirstOrDefault();
            Assert.NotNull(firstItem);
            Assert.AreEqual(box.Description, firstItem.Description);
        }

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
