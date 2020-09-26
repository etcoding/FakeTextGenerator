FakeTextGenerator
==========

This library allows creating dummy text reminding real one, not dissimilar to lorem ipsum. However, this library works by analyzing real texts and can be used to generate texts in various languages (or, rather, code pages - it does not generate real words). The analyzing algorithm is a bit naive, probably, but it works fairly well for generating text that doesn't look like a fully random collection of characters, yet does not distract the reader.

I've created this code when I needed to populate a bunch of User objects for test purposes and needed to name them with something better than user1, user2, etc.

Right off the bat, this library can be used to generate English-like words, texts, and names - see code samples below. To generate texts in other code pages it can be fed with texts in the desired codepage. The texts should be fairly sizeable, in tens to hundreds Kbs. It will work off smaller texts, but the result will be more like just a random string.

It is also available as a Nuget package.

Usage:
------

Generate names:
```csharp
TextGenerator wc = new TextGenerator(WordTypes.Name);
List<string> namesList = new List<string>(20);
for (int i = 0; i < 20; i++)
{
    namesList.Add(wc.GenerateWord(6));
}

string names = string.Join(", ", namesList);
```
Names: Imiger, Iveace, Ivepha, Chlynz, Ammrra, Oweyel, Neinee, Aurmon, Hakain, Beysar, Callee, Iteaci, Crmrel, Trasal, Habryl, Giphit, Beytha, Eissam, Havala, Girngi


Generate single words:
```csharp
TextGenerator wc = new TextGenerator();
List<string> wordsList = new List<string>(20);
for (int i = 0; i < 20; i++)
{
    wordsList.Add(wc.GenerateWord(6));
}

string words = string.Join(", ", wordsList);
```
Words: ilwlun, camert, onmpow, weatot, thilse, ugonta, scsind, sixesf, sodinb, whotas, wheyap, ixidid, istiti, whamen, uppees, opalda, wwonks, nenfic, upania, clwlti

Text:
```csharp
 TextGenerator wc = new TextGenerator();
 string text = wc.GenerateText(20);
```
Text: Ingussted pou ouluaron neveday gab. Conksoall oneyat grumll we spopt c fixttsitl ilarnd. Tewd confo ixii sundursa b ixprysu p.


Russian and spanish texts:
```csharp
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
```

Russian: Площила мы ахвсо вы вхобый знавикат. Астаютьн здизилав фога ихвст эзыдешь п тойчикии. Месоркан м тарясее тяжаююс веровых вхоизива фимезаль.

Spanish: Eaito noburos nobr bivo. Tos yogr pei alare leovasc tuesp gayni eniguy daynycón unsad. Untalad máxte moud geimmare bidom díbus.


Anyhow, if you find it useful - great. If you have any ideas - drop me a line at evgeni at etcoding dot com.
