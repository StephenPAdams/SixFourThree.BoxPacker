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
    public class BoxListTests
    {

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
        public void CanAddBoxesInProperOrder()
        {
            var box = new Box()
            {
                Description = "Small",
                OuterWidth = 21,
                OuterLength = 21,
                OuterDepth = 3,
                EmptyWeight = 1,
                InnerWidth = 20,
                InnerLength = 20,
                InnerDepth = 2,
                MaxWeight = 100
            };

            var box2 = new Box()
            {
                Description = "Large",
                OuterWidth = 201,
                OuterLength = 201,
                OuterDepth = 21,
                EmptyWeight = 1,
                InnerWidth = 200,
                InnerLength = 200,
                InnerDepth = 20,
                MaxWeight = 1000
            };

            var box3 = new Box()
            {
                Description = "Medium",
                OuterWidth = 101,
                OuterLength = 101,
                OuterDepth = 11,
                EmptyWeight = 5,
                InnerWidth = 100,
                InnerLength = 100,
                InnerDepth = 10,
                MaxWeight = 500
            };

            var boxes = new BoxList();
            boxes.Insert(box);
            boxes.Insert(box2);
            boxes.Insert(box3);
            
            var orderedItems = new List<Box>();

            while (!boxes.IsEmpty())
            {
                var bestItem = boxes.GetBest();
                orderedItems.Add(bestItem);
                boxes.ExtractBest();
            }

            var expectedOutcome = new List<Box>() { box, box3, box2 };

            for (var counter = 0; counter < expectedOutcome.Count; counter++)
            {
                Assert.AreEqual(orderedItems[counter], expectedOutcome[counter]);
            }
        }
    }
}
