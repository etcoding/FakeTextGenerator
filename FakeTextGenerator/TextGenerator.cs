using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ET.FakeText
{
    public class TextGenerator
    {
        private Corpus corpus;
        private WordTypes wordType = WordTypes.Word;
        private Random rand = new Random();

        private static readonly TextGenerator wordCreator = new TextGenerator();

        public TextGenerator(WordTypes wordType = WordTypes.Word)
        {
            if (wordType == WordTypes.Name)
                this.corpus = Corpus.DeserializeFromEmbeddedResource("names.bin");
            else
                this.corpus = Corpus.DeserializeFromEmbeddedResource("text.bin");

            this.wordType = wordType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextGenerator"/> class.
        /// Will generate text based on provided corpus. Use this to generate fake text based on different languages.
        /// </summary>
        /// <param name="corpus">The corpus.</param>
        public TextGenerator(Corpus corpus)
        {
            if (corpus == null)
                throw new ArgumentException("Corpus is not provided.");
            this.corpus = corpus;
        }

        /// <summary>
        /// Creates a fake word.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public string GenerateWord(int length)
        {
            char[] chars = new char[length];
            char prevLetter = '\0';
            for (int i = 0; i < length; i++)
            {
                LetterStats[] letters = this.corpus.Letters.Where(x => x.LetterPositionInWord == i
                       && (i == 0 ? true : x.PreviousLetter == prevLetter)
                       ).ToArray();

                // if the requested word is too long, we may not have letters with given word position; in this case specify min position that this letter was found in
                if (letters.Length == 0)
                {
                    // select min position for this letter info
                    LetterStats lettersByPrev = this.corpus.Letters.Where(x => x.PreviousLetter == prevLetter).OrderBy(x => x.LetterPositionInWord).Take(1).SingleOrDefault();
                    if (lettersByPrev.Equals(default(LetterStats)))
                    {
                        letters = this.corpus.Letters.Where(x => (i == 0 ? true : x.PreviousLetter == prevLetter) && x.LetterPositionInWord == lettersByPrev.LetterPositionInWord).ToArray();
                    }
                }

                // if we still don't have a set of letters to choose from (will happen when the corpus is too small), select a random letter from whole corpus; each letter is selected once, ordered by weight.
                if (letters.Length == 0)
                {
                    letters = this.corpus.Letters.GroupBy(x => x.Letter).Select((x, y) => x.OrderByDescending(o => o.Count).Take(1).SingleOrDefault()).ToArray();
                }

                int weightSum = letters.Sum(x => x.Count);

                Dictionary<char, Range> ranged = MakeCharactersRange(letters);

                // now pick a random number...
                int rw = rand.Next(weightSum);
                char selected = ranged.Single(x => x.Value.From <= rw && x.Value.To >= rw).Key;

                prevLetter = selected;
                chars[i] = selected;
            }

            if (this.wordType == WordTypes.Name)
                chars[0] = Char.ToUpper(chars[0]);

            return new string(chars);
        }

        /// <summary>
        /// This method will create a dictionary with characters as key, and a range for this character. Range is derived from character's frequency.
        /// </summary>
        /// <param name="letters">The letters.</param>
        /// <returns></returns>
        private static Dictionary<char, Range> MakeCharactersRange(LetterStats[] letters)
        {
            Dictionary<char, Range> ranged = new Dictionary<char, Range>();
            int rangeStart = 0;
            foreach (var letter in letters)
            {
                if (ranged.ContainsKey(letter.Letter))
                    continue;
                Range r = new Range() { From = rangeStart, To = rangeStart + letter.Count };
                ranged.Add(letter.Letter, r);
                rangeStart = rangeStart + letter.Count + 1;
            }
            return ranged;
        }

        public string GenerateText(int wordCount)
        {
            StringBuilder sbText = new StringBuilder();
            int numOfWords = 0;
            int sentenceLen = rand.Next(12);
            int numOfWordsInSentence = 0;

            while (numOfWords < wordCount)
            {
                int wordLen = rand.Next(1, 10);
                string word = this.GenerateWord(wordLen);
                Debug.Assert(!string.IsNullOrWhiteSpace(word));
                if (numOfWordsInSentence == 0)
                    word = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word);

                sbText.Append(word);
                numOfWords++;

                numOfWordsInSentence++;
                if (numOfWordsInSentence == sentenceLen)
                {
                    sbText.Append(". ");
                    numOfWordsInSentence = 0;
                    sentenceLen = rand.Next(12);
                }
                else
                {
                    sbText.Append(" ");
                }
            }

            string text = sbText.ToString();
            if (!text.EndsWith("."))
                text = text.TrimEnd() + ".";

            return text;
        }


        private class Range
        {
            public int From { get; set; }
            public int To { get; set; }
            public override string ToString()
            {
                return this.From + " - " + this.To;
            }
        }
    }
}