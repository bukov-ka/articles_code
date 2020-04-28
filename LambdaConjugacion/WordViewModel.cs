using System;
using System.Collections.Generic;
using System.Text;

namespace LambdaConjugacion
{
    public class WordViewModel
    {
        public string infinitive { get; set; }
        public string tense_key { get; set; }
        public string word { get; set; }
        public string pronoun { get; set; }
        public bool is_irregular { get; set; }
        public int start_idx { get; set; }
        public int end_idx { get; set; }

        public WordViewModel()
        { }
    }
}
