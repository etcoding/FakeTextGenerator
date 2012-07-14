using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ET.FakeText;
using System.IO;

namespace UnitTests
{
    [TestClass()]
   public class CodeSamples
    {
        #region Code samples

        [TestMethod()]
        public void MakeNames()
        {
            TextGenerator wc = new TextGenerator(WordTypes.Name);
            List<string> namesList = new List<string>(20);
            for (int i = 0; i < 20; i++)
            {
                namesList.Add(wc.GenerateWord(6));
            }

            string names = string.Join(", ", namesList);
        }

        [TestMethod()]
        public void MakeWords()
        {
            TextGenerator wc = new TextGenerator();
            List<string> wordsList = new List<string>(20);
            for (int i = 0; i < 20; i++)
            {
                wordsList.Add(wc.GenerateWord(6));
            }

            string words = string.Join(", ", wordsList);
        }

        [TestMethod()]
        public void MakeText()
        {
            TextGenerator wc = new TextGenerator();
            string text = wc.GenerateText(20);
        }

        [TestMethod()]
        [DeploymentItem(@"Corpuses\rus.txt", "Corpuses")]
        [DeploymentItem(@"Corpuses\spanish.txt", "Corpuses")]
        public void MakeRussianAndSpanishText()
        {
            string s = File.ReadAllText(@"Corpuses\rus.txt", Encoding.UTF8);
            Corpus c = Corpus.CreateFromText(s);

            // Corpus can be serialized for later usage; the resulting file can be embedded as a resource; see
            //c.SerializeToFile(string) and Corpus.DeserializeFromEmbeddedResource() methods.

            TextGenerator wc = new TextGenerator(c);
            string russian = wc.GenerateText(20);

            s = File.ReadAllText(@"Corpuses\spanish.txt", Encoding.UTF8);
            c = Corpus.CreateFromText(s);
            wc = new TextGenerator(c);
            string spanish = wc.GenerateText(20);
        }
        
        #endregion
    }
}
