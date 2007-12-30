using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Imagine.GUI;

namespace GUI_Unit_Tests
{
    [TestFixture]
    public class NonOverlapTest
    {
        [Test]
        public void test1()
        {
            CirkularGeometricDistributor<String> dist = new CirkularGeometricDistributor<String>(10.0, 1.0);
            Dictionary<String, double> pos;

            String s1 = "1";
            dist.AddUnit(s1, 0.0);
            pos = dist.getPositions();
            Assert.AreEqual(0.0, pos[s1], 0.01);

            String s2 = "2";
            dist.AddUnit(s2, 2.5);
            pos = dist.getPositions();
            Assert.AreEqual(0.0, pos[s1], 0.01);
            Assert.AreEqual(2.5, pos[s2], 0.01);

            String s3 = "3";
            dist.AddUnit(s3, 0.0);
            pos = dist.getPositions();
            Assert.AreEqual(9.5, pos[s1], 0.01);
            Assert.AreEqual(2.5, pos[s2], 0.01);
            Assert.AreEqual(0.5, pos[s3], 0.01);

            String s4 = "4";
            dist.AddUnit(s4, 1.0);
            pos = dist.getPositions();
            Assert.AreEqual(9.33, pos[s1], 0.01);
            Assert.AreEqual(2.5, pos[s2], 0.01);
            Assert.AreEqual(0.33, pos[s3], 0.01);
            Assert.AreEqual(1.33, pos[s4], 0.01);

            String s5 = "5";
            dist.AddUnit(s5, 2.5);
            pos = dist.getPositions();
            Assert.AreEqual(9.2, pos[s1], 0.01);
            Assert.AreEqual(2.2, pos[s2], 0.01);
            Assert.AreEqual(0.2, pos[s3], 0.01);
            Assert.AreEqual(1.2, pos[s4], 0.01);
            Assert.AreEqual(3.2, pos[s5], 0.01);
        }

        [Test]
        public void test_fringe_cases()
        {
            CirkularGeometricDistributor<String> dist = new CirkularGeometricDistributor<String>(10.0, 1.0);
            Dictionary<String, double> pos;

            String s1 = "1";
            String s2 = "2";
            dist.AddUnit(s1, 0.1);
            dist.AddUnit(s2, 9.9);
            pos = dist.getPositions();
            Assert.AreEqual(0.5, pos[s1], 0.01);
            Assert.AreEqual(9.5, pos[s2], 0.01);

            String s3 = "3";
            dist.AddUnit(s3, 0.0);
            pos = dist.getPositions();
            Assert.AreEqual(1.0, pos[s1], 0.01);
            Assert.AreEqual(9.0, pos[s2], 0.01);
            Assert.AreEqual(0.0, pos[s3], 0.01);

            String s4 = "4";
            dist.AddUnit(s4, 5.0);
            pos = dist.getPositions();
            Assert.AreEqual(1.0, pos[s1], 0.01);
            Assert.AreEqual(9.0, pos[s2], 0.01);
            Assert.AreEqual(0.0, pos[s3], 0.01);
            Assert.AreEqual(5.0, pos[s4], 0.01);
            
            String s5 = "5";
            dist.AddUnit(s5, 5.0);
            pos = dist.getPositions();
            Assert.AreEqual(1.0, pos[s1], 0.01);
            Assert.AreEqual(9.0, pos[s2], 0.01);
            Assert.AreEqual(0.0, pos[s3], 0.01);
            Assert.AreEqual(4.5, pos[s4], 0.01);
            Assert.AreEqual(5.5, pos[s5], 0.01);

            String s6 = "6";
            String s7 = "7";
            dist.AddUnit(s6, 0.0);
            dist.AddUnit(s7, 0.0);
            pos = dist.getPositions();
            Assert.AreEqual(2.0, pos[s1], 0.01);
            Assert.AreEqual(8.0, pos[s2], 0.01);
            Assert.AreEqual(9.0, pos[s3], 0.01);
            Assert.AreEqual(4.5, pos[s4], 0.01);
            Assert.AreEqual(5.5, pos[s5], 0.01);
            Assert.AreEqual(0.0, pos[s6], 0.01);
            Assert.AreEqual(1.0, pos[s7], 0.01);
        }
    }
}
