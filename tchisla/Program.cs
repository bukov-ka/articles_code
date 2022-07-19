
void solutions(char digit, int count, int result)
{

    var generator = new TreeGenerator();
    foreach (var node in generator.Generate(digit, count))
    {
        Calculator calc = new Calculator(node);
        var res = calc.Calculate(result).OrderByDescending(o => o.Length).ToArray();
        foreach (var v in res)
        {
            Console.WriteLine($"{v}");
        }
    }

}
 solutions('8', 3, 1024);
//solutions('3', 4, 11);
// solutions('3', 4, 10);
//solutions('3', 2, 2);
 //solutions('2', 5, 100);
Console.WriteLine("Finished.");