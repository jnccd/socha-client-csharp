using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SochaClient.Backend;

namespace SochaClient.Tests
{
    [TestClass()]
    public class BoardTests
    {
        [TestMethod()]
        public void BoardIsCorrectlyCloned()
        {
            //Board board = new();
            //board.GetField(1, 1).fishes = 2;

            //var clone = (Board)board.Clone();
            //clone.GetField(2, 2).fishes = 3;

            //Assert.AreEqual(board.GetField(1, 1).fishes, 2);
            //Assert.AreEqual(board.GetField(2, 2).fishes, 0);

            //Assert.AreEqual(clone.GetField(1, 1).fishes, 2);
            //Assert.AreEqual(clone.GetField(2, 2).fishes, 3);
        }
    }
}