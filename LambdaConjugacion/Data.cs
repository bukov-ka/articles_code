using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace LambdaConjugacion
{
    public class Data
    {
        private static List<WordViewModel> Verbs { get; }
        static Data()
        {
            var verbString = File.ReadAllText("verbsForLambda.json");
            Verbs = JsonConvert.DeserializeObject<List<WordViewModel>>(verbString);

        }

        public static List<WordViewModel> TenRandomVerbs(string[] tenseKeys)
        {
            var rnd = new Random();
            var allWordsModels = Data.Verbs
                // Filter for the selected tenses only
                .Where(w => tenseKeys == null || tenseKeys.Length == 0 || // Include all verbs if nothing is selected
                tenseKeys.Contains(w.tense_key))
                // Shuffle
                .OrderBy(o => rnd.NextDouble())
                .Take(10)
                .ToList();
            var words = allWordsModels;
            return words;
        }


    }
}
