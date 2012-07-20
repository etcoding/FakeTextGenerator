using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

namespace ET.FakeText
{
    [Serializable]
    public class Corpus
    {
        /// <summary>
        /// Gets the number of words in corpus.
        /// </summary>
        public int WordCount { get; private set; }
        /// <summary>
        /// Gets the number of characters in corpus.
        /// </summary>
        public int CharCount { get; private set; }
        List<LetterStats> letters = new List<LetterStats>();
        /// <summary>
        /// Gets the letters data collection in corpus.
        /// </summary>
        public ReadOnlyCollection<LetterStats> Letters { get { return this.letters.AsReadOnly(); } }

        private Corpus() { }

        /// <summary>
        /// Creates the corpus from text.
        /// Text should be long enough to generate decent results (e.g. 10Kb+).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static Corpus CreateFromText(string text)
        {
            Dictionary<LetterStats, LetterStats> letterData = new Dictionary<LetterStats, LetterStats>();
            int charCount = 0;

            // clean text
            string clean = Regex.Replace(text, "[^\\p{L} \t\n]", string.Empty).ToLower();

            string[] words = clean.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                char prevLetter = '\0';
                for (int l = 0; l < words[i].Length; l++)
                {
                    LetterStats ls = new LetterStats(words[i][l], prevLetter, l);
                    if (letterData.ContainsKey(ls))
                        letterData[ls].IncrementCount();
                    else
                        letterData.Add(ls, ls);
                    prevLetter = words[i][l];
                    charCount++;
                }
            }

            Corpus c = new Corpus() { CharCount = charCount, WordCount = words.Length };
            c.letters.AddRange(letterData.Keys);
            return c;
        }

        /// <summary>
        /// Serializes corpus to a binary file.
        /// </summary>
        /// <param name="fileName">Name of the file, including the path.</param>
        public void SerializeToFile(string fileName)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        /// <summary>
        /// Deserializes corpus from a binary file.
        /// </summary>
        /// <param name="fileName">Name of the file, including the path.</param>
        /// <returns></returns>
        public static Corpus DeserializeFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                throw new ArgumentException("Specified file (" + fileName + ") does not exist");
            BinaryFormatter serializer = new BinaryFormatter();
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                return serializer.Deserialize(stream) as Corpus;
            }
        }

        /// <summary>
        /// Deserializes corpus from a file, which contains serialized corpus and was embedded  as a resource.
        /// </summary>
        /// <param name="name">The name of the file. Only needs to contain the file name with extension.</param>
        /// <returns></returns>
        public static Corpus DeserializeFromEmbeddedResource(string name)
        {
            Assembly executingAssm = Assembly.GetExecutingAssembly();
            // get actual name, based on provided name
            name = executingAssm.GetManifestResourceNames().SingleOrDefault(x => x.ToLower().EndsWith(name));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Invalid resource name (" + name + ")");

            using (Stream stream = executingAssm.GetManifestResourceStream(name))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                return serializer.Deserialize(stream) as Corpus;
            }
        }
    }
}