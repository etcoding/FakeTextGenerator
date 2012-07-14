using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.FakeText
{
    [Serializable]
    public struct LetterStats : IEquatable<LetterStats>
    {
        public char Letter { get; private set; }
        public int LetterPositionInWord { get; private set; }
        public char PreviousLetter { get; private set; }
        public int Count { get; private set; }

        public LetterStats(char letter, char previousLetter, int wordPosition)
            : this()
        {
            this.Letter = letter;
            this.PreviousLetter = previousLetter;
            this.LetterPositionInWord = wordPosition;
            this.Count = 1;
        }

        public int IncrementCount()
        {
            this.Count++;
            return this.Count;
        }

        public bool Equals(LetterStats other)
        {
            return this.Letter == other.Letter && this.PreviousLetter == other.PreviousLetter && this.LetterPositionInWord == other.LetterPositionInWord;
        }

        public override int GetHashCode()
        {
            // taken from SO Jon Skeet's answer
            int hash = 17;
            hash = hash * 31 + this.Letter;
            hash = hash * 31 + this.PreviousLetter;
            hash = hash * 31 + this.LetterPositionInWord;
            return hash;
        }

        public override string ToString()
        {
            return this.Letter + " - " + this.PreviousLetter + " - " + this.LetterPositionInWord;
        }
    }
}