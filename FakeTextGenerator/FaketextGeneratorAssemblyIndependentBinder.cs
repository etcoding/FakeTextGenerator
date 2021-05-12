using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ET.FakeText;

namespace FakeTextGenerator
{
    sealed class FaketextGeneratorAssemblyIndependentBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName == typeof (Corpus).FullName)
            {
                return typeof (Corpus);
            }
            if (typeName == typeof (LetterStats).FullName)
            {
                return typeof (LetterStats);
            }
            if (typeName.Contains("List`1") && typeName.Contains("LetterStats"))
            {
                return typeof (List<LetterStats>);
            }
            return null;
        }
    }
}
