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
            Board board = new();
            board.SetField(new CubeCoords(1, 1), new Field(FieldType.island, false, new CubeCoords(1, 1), board));

            var clone = (Board)board.Clone();
            clone.SetField(new CubeCoords(1, 1), new Field(FieldType.water, false, new CubeCoords(1, 1), board));

            Assert.AreEqual(board.GetField(1, 1).FType, FieldType.island);

            Assert.AreEqual(clone.GetField(1, 1).FType, FieldType.water);
        }
    }
}