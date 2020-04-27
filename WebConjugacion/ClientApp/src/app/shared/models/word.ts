export class Word {
  infinitive: string;
  tense: string;
  tense_key: string;
  word: string;
  pronoun: string;
  is_irregular: boolean;
  start_idx: number;
  end_idx: number;

  // Constuctor used for deserialization
  constructor(w: Word) {
    // Copy properties
    Object.keys(w).forEach(key => this[key] = w[key]);
  }

  public get first_part(): string {
    if (!this.is_irregular) return this.word;
    var first = this.word.substring(0, this.start_idx);
    return first;

  }
  public get irregular_part(): string {
    if (!this.is_irregular) return "";
    if (this.start_idx >= this.word.length) return ""; // Irregularity is outside the word
    var irregular = this.word.substring(this.start_idx, this.end_idx);
    return irregular;

  }
  public get last_part(): string {
    if (!this.is_irregular) return "";
    if (this.end_idx >= this.word.length) return ""; // Irregularity is outside the word
    var last = this.word.substring(this.end_idx);
    return last;

  }
}
