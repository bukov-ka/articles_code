using Translator;
using System.IO;

string folderPath = @"russian";
string searchMask = "*.tex"; // Example mask to find text files

string[] filesMatchingMask = Directory.GetFiles(folderPath, searchMask);

 var translator = new ChatGptTranslator(apiKey: "sk-XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
foreach(string file in filesMatchingMask)
{
     await translator.TranslateAsync(inputFile: file, outputFile: "english\\"+Path.GetFileName(file));
}