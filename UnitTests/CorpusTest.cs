using ET.FakeText;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using FluentAssertions;
using System.Linq;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for CorpusTest and is intended
    ///to contain all CorpusTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CorpusTest
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


        [TestMethod()]
        [DeploymentItem(@"Corpuses\eng.txt", "Corpuses")]
        public void ProcessTextFile_ShouldProduceCorpus()
        {
            Corpus c = Corpus.CreateFromText(File.ReadAllText(@"Corpuses\eng.txt"));
            c.Should().NotBeNull();
            c.Letters.Count.Should().BeGreaterThan(1);
        }

        /// <summary>
        ///A test for SerializeToFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Corpuses\eng.txt", "Corpuses")]
        [DeploymentItem(@"Corpuses\names.txt", "Corpuses")]
        public void SerializeToFileTest()
        {
            Corpus c = Corpus.CreateFromText(File.ReadAllText(@"Corpuses\names.txt"));
            c.SerializeToFile(@"text.bin");
            File.Exists(@"text.bin").Should().BeTrue();
            new FileInfo("text.bin").Length.Should().BeGreaterThan(0);
        }

        [TestMethod()]
        public void CorpusShouldContainDistinctLetters()
        {
            Corpus corpus = Corpus.DeserializeFromEmbeddedResource("names.bin");
            var letters = from l in corpus.Letters
                          group l by new {l.Letter, l.PreviousLetter, l.LetterPositionInWord } into g
                          select new {g.Key, Count = g.Count()};
            var list = letters.ToList();
            letters.Any(x => x.Count > 1).Should().BeFalse();
        }


        /// <summary>
        ///A test for DeserializeFromFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Corpuses\eng.txt", "Corpuses")]
        public void DeserializeFromFile_ShouldRecreateCorpus()
        {
            Corpus c = Corpus.CreateFromText(File.ReadAllText(@"Corpuses\eng.txt"));
            c.Should().NotBeNull();
            c.SerializeToFile("text.bin");

            Corpus c2 = Corpus.DeserializeFromFile("text.bin");
            c2.Should().NotBeNull();
            c.Letters.Count.Should().Be(c2.Letters.Count);
            c.WordCount.Should().Be(c2.WordCount);
        }
    }
}