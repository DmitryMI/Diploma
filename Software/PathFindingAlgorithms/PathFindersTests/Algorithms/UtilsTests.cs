using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathFinders.Algorithms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Algorithms.Tests
{
    [TestClass()]
    public class UtilsTests
    {
        private class IntComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                if (x < y)
                    return -1;
                if (x > y)
                    return 1;
                return 0;
            }
        }

        private bool CompareLists<T>(IList<T> arrayA, IList<T> arrayB)
        {
            if (arrayA.Count != arrayB.Count)
            {
                return false;
            }

            for (int i = 0; i < arrayA.Count; i++)
            {
                if (!arrayA[i].Equals(arrayB[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private void PrintCollection<T>(ICollection<T> collection)
        {
            Debug.Write('[');
            foreach (var item in collection)
            {
                Debug.Write(item.ToString() + ' ');
            }
            Debug.WriteLine(']');
        }

        private void TestLists(List<int> source, List<int> correct, int value, string testName)
        {
            Debug.WriteLine($"{testName} running...");

            Utils.InsertSortedDescending(source, value, new IntComparer());

            bool areEqual = CompareLists(source, correct);

            if (!areEqual)
            {
                Debug.WriteLine($"{testName} failed");
                Debug.WriteLine("Result: \t");
                PrintCollection(source);
                Debug.WriteLine("Correct: \t");
                PrintCollection(correct);
            }

            Assert.IsTrue(areEqual);
        }

        private void TestNormal()
        {
            List<int> source = new List<int>() { 10, 9, 8, 6, 5};
            List<int> correct = new List<int>() { 10, 9, 8, 7, 6, 5 };
            int value = 7;
            TestLists(source, correct, value, "NormalTest");
        }

        private void TestEmpty()
        {
            List<int> source = new List<int>() { };
            List<int> correct = new List<int>() {7};
            int value = 7;
            TestLists(source, correct, value, "EmptyTest");
        }

        private void TestSingleLesser()
        {
            List<int> source = new List<int>() { 6};
            List<int> correct = new List<int>() { 7, 6 };
            int value = 7;
            TestLists(source, correct, value, "SingleLesserTest");
        }

        private void TestSingleGreater()
        {
            List<int> source = new List<int>() { 8 };
            List<int> correct = new List<int>() { 8, 7 };
            int value = 7;
            TestLists(source, correct, value, "SingleGreater");
        }

        private void TestHead()
        {
            List<int> source = new List<int>() { 6, 5, 4, 3 };
            List<int> correct = new List<int>() { 7, 6, 5, 4, 3 };
            int value = 7;
            TestLists(source, correct, value, "HeadTest");
        }
        private void TestTail()
        {
            List<int> source = new List<int>() { 10, 9, 8 };
            List<int> correct = new List<int>() { 10, 9, 8, 7 };
            int value = 7;
            TestLists(source, correct, value, "TailTail");
        }


        [TestMethod()]
        public void InsertSortedDescendingTest()
        {
            TestEmpty();
            TestSingleLesser();
            TestSingleGreater();
            TestHead();
            TestTail();
            TestNormal();
        }
    }
}