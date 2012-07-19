using ET.FakeText;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace UnitTests
{
    /// <summary>
    ///This is a test class for WordCreatorTest and is intended
    ///to contain all TextGenerator Unit Tests
    ///</summary>
    [TestClass()]
    public class TextGeneratorTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for WordCreator Constructor
        ///</summary>
        [TestMethod()]
        public void WordCreatorConstructors_ShouldNotTrowExceptions()
        {
            TextGenerator wc = new TextGenerator();
            wc = new TextGenerator(WordTypes.Name);
            Corpus c = Corpus.DeserializeFromEmbeddedResource("names.bin");
            wc = new TextGenerator(c);
        }

        [TestMethod()]
        public void WordCreator_ShouldMakeWords()
        {
            TextGenerator wc = new TextGenerator();
            wc.GenerateWord(6).Should().NotBeNullOrEmpty().And.HaveLength(6);
        }

        [TestMethod()]
        public void WordCreator_ShouldMakeNames()
        {
            TextGenerator wc = new TextGenerator(WordTypes.Name);
            for (int i = 0; i < 100; i++)
            {
                string name = wc.GenerateWord(6);
                name.Should().NotBeNullOrEmpty().And.HaveLength(6);
                name[0].Should().Be(char.ToUpper(name[0]));
            }
        }

        [TestMethod()]
        public void WordCreator_ShouldMakeLongWords()
        {
            TextGenerator wc = new TextGenerator();
            wc.GenerateWord(100).Should().NotBeNullOrEmpty();
            string s = wc.GenerateWord(100);
        }

        /// <summary>
        ///A test for CreateText
        ///</summary>
        [TestMethod()]
        public void CreateTextTest()
        {
            TextGenerator wc = new TextGenerator();
            string text = wc.GenerateText(200);
            text.Split(' ').Length.Should().Be(200);
            text.Should().EndWith(".");
        }


        [TestMethod()]
        [DeploymentItem(@"Corpuses\rus.txt", "Corpuses")]
        public void CreateNonEnglishTextTest()
        {
            string s = File.ReadAllText(@"Corpuses\rus.txt", Encoding.UTF8);

            Corpus c = Corpus.CreateFromText(s);

            TextGenerator wc = new TextGenerator(c);
            string text = wc.GenerateText(200);
            text.Split(' ').Length.Should().Be(200);
            text.Should().EndWith(".");
        }

        [TestMethod()]
        [DeploymentItem(@"Corpuses\spanish.txt", "Corpuses")]
        public void CreateNonEnglishTextTest2()
        {
            string s = File.ReadAllText(@"Corpuses\spanish.txt", Encoding.UTF8);

            Corpus c = Corpus.CreateFromText(s);

            TextGenerator wc = new TextGenerator(c);

            string text = wc.GenerateText(200);
            text.Split(' ').Length.Should().Be(200);
            text.Should().EndWith(".");
        }

        /// <summary>
        ///A test for CreateText
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Corpuses\short.txt", "Corpuses")]
        public void CreateText_ShouldWorkOffSmallCorpus()
        {
            string s = File.ReadAllText(@"Corpuses\short.txt", Encoding.UTF8);
            Corpus c = Corpus.CreateFromText(s);
            TextGenerator wc = new TextGenerator(c);
            string text = wc.GenerateText(2000);
            text.Split(' ').Length.Should().Be(2000);
            text.Should().EndWith(".");
        }

        [TestMethod()]
        public void WordCreator_ShouldNotGenerateWords_ThatHave3SameAdjacentLetters()
        {
            TextGenerator wc = new TextGenerator();
            string text = wc.GenerateText(10000);

            string[] split = text.Split(' ', '.');
            split.Any(x => Regex.Match(x, "(.)\\1{2,}").Groups.Count > 1).Should().BeFalse();
        }

        [TestMethod()]
        public void Text_ShouldNotHaveSentences_LongerThan_MaxSentenceLength()
        {
            TextGenerator wc = new TextGenerator();
            string text = wc.GenerateText(10000);

            string[] split = text.Split('.');
            split.Count(x => x.Split(' ').Length > wc.MaxSentenceLength).Should().Be(0);
        }

        /// <summary>
        /// Tested on i5 2.3GHz
        /// </summary>
        [TestMethod()]
        public void IWant_TextGenerator_ToTakeLessThan_500ms_ToGenerate_1000Words()
        {
            TextGenerator wc = new TextGenerator();
            Stopwatch sw = Stopwatch.StartNew();
            string text = wc.GenerateText(1000);
            Debug.WriteLine("Total elapsed: " + sw.ElapsedMilliseconds + " ms");
            sw.ElapsedMilliseconds.Should().BeLessOrEqualTo(500);
        }
    }
}