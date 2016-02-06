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
    public class ItemListTests
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
            var newItems = new List<Item>() { item1, item2 };

            packer.AddItems(newItems);

            var items = packer.GetItems();
            Assert.AreEqual(2, items.GetCount());

            var castItems = items.GetContent().Cast<Item>().ToList();
            Assert.AreEqual(2, castItems.Count);
        }

        [Test]
        public void CanAddItemsInProperOrder()
        {
            var item = new Item() { Id = "Small", Description = "Small", Width = 20, Length = 20, Depth = 2, Weight = 100 };
            var item2 = new Item() { Id = "Large", Description = "Large", Width = 200, Length = 200, Depth = 20, Weight = 1000 };
            var item3 = new Item() { Id = "Medium", Description = "Medium", Width = 100, Length = 100, Depth = 10, Weight = 500 };
            
            var items = new ItemList();
            items.Insert(item);
            items.Insert(item2);
            items.Insert(item3);
            
            var orderedItems = new List<Item>();

            while (!items.IsEmpty())
            {
                var bestItem = items.GetBest();
                orderedItems.Add(bestItem);
                items.ExtractBest();
            }

            var expectedOutcome = new List<Item>() {item2, item3, item};

            for (var counter = 0; counter < expectedOutcome.Count; counter++)
            {
                Assert.AreEqual(orderedItems[counter], expectedOutcome[counter]);
            }
        }
    }
}
