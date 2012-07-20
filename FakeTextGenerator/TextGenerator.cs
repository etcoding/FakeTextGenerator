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

        protected int maxSentenceLength = 25;
        /// <summary>
        /// Gets or sets the length of the maximum number of words in sentence. Default is 25.
        /// </summary>
        /// <value>
        /// The length of the max sentence.
        /// </value>
        public int MaxSentenceLength
        {
            get { return this.maxSentenceLength; }
            set { if (value <= 0) throw new ArgumentException("Must be greater than 0."); this.maxSentenceLength = value; }
        }

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
        /// <param name="length">The desired word length.</param>
        /// <returns></returns>
        public string GenerateWord(int length)
        {
            char[] chars = new char[length];
            char[] last2Letters = new char[2] { '\0', '\0' };
            for (int i = 0; i < length; i++)
            {
                char selected = PickNextLetter(last2Letters, i);
                last2Letters[0] = last2Letters[1];
                last2Letters[1] = selected;
                chars[i] = selected;
            }

            if (this.wordType == WordTypes.Name)
                chars[0] = Char.ToUpper(chars[0]);

            return new string(chars);
        }

        /// <summary>
        /// Picks the next letter.
        /// Method looks at a set of letters that have same previous letters and are in same position in the word.
        /// If the word that we're trying to generate is too long, and we don't have any letters with required stats - will look at previous letters only.
        /// If that doesn't help - will choose random letter.
        /// If last 2 letters are same - will try not to generate same letter 3rd time.
        /// </summary>
        /// <param name="prev2Letters">The 2 previously selected letter.</param>
        /// <param name="position">The position in the word.</param>
        /// <returns></returns>
        private char PickNextLetter(char[] prev2Letters, int position)
        {
            List<LetterStats> letters = new List<LetterStats>(this.corpus.Letters.Count / 90);  // /90 seems to work well to define capacity
            //// This is about 30% faster than an equivalent LINQ statement
            foreach (var letter in this.corpus.Letters)
            {
                if (letter.LetterPositionInWord == position && (position == 0 ? true : letter.PreviousLetter == prev2Letters[1]))
                    letters.Add(letter);
            }

            // if the requested word is too long, we may not have letters with given word position; in this case specify min position that this letter was found in
            if (letters.Count == 0)
            {
                // select min position for this letter info
                LetterStats lettersByPrev = this.corpus.Letters.Where(x => x.PreviousLetter == prev2Letters[1]).OrderBy(x => x.LetterPositionInWord).Take(1).SingleOrDefault();
                if (lettersByPrev.Equals(default(LetterStats)))
                {
                    letters = this.corpus.Letters.Where(x => (position == 0 ? true : x.PreviousLetter == prev2Letters[1]) && x.LetterPositionInWord == lettersByPrev.LetterPositionInWord).ToList();
                }
            }

            // if we still don't have a set of letters to choose from (will happen when the corpus is too small), select a random letter from whole corpus; each letter is selected once, ordered by weight.
            if (letters.Count == 0)
            {
                letters = this.corpus.Letters.GroupBy(x => x.Letter).Select((x, y) => x.OrderByDescending(o => o.Count).Take(1).SingleOrDefault()).ToList();
            }

            // if last 2 letters are same - try to eliminate these letters from the set
            if ((prev2Letters[0] == prev2Letters[1]) && prev2Letters[0] != '\0')
            {
                if (letters.Where(x => x.Letter == prev2Letters[1]).Count() < letters.Count()) // see if this is not the only choice.. shouldn't happen a lot.
                    letters = letters.Where(x => x.Letter != prev2Letters[1]).ToList();
            }

            int weightSum = letters.Sum(x => x.Count);

            Dictionary<char, Range> ranged = MakeCharactersRange(letters.ToArray());

            // now pick a random number...
            int rw = rand.Next(weightSum);
            char selected = ranged.Single(x => x.Value.From <= rw && x.Value.To >= rw).Key;
            return selected;
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

        /// <summary>
        /// Generates the random text.
        /// </summary>
        /// <param name="wordCount">Number of words to generate.</param>
        /// <returns></returns>
        public string GenerateText(int wordCount)
        {
            StringBuilder sbText = new StringBuilder();
            int numOfWords = 0;
            int sentenceLen = rand.Next(this.MaxSentenceLength);
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
                    sentenceLen = rand.Next(1, MaxSentenceLength);
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