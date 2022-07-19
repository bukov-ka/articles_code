using NCalc;

public class Calculator
{
    Node root;
    /// <summary>
    /// Initializes a new instance of the <see cref="Calculator"/> class.
    /// </summary>
    public Calculator(Node node)
    {
        this.root = node;
    }


    /// <summary>
    /// Get all possible variants to get the result.
    /// </summary>
    public IEnumerable<string> Calculate(int res)
    {
        var resultTarget = Convert.ToDouble(res);
        foreach (var v in this.root.GetVariants())
        {

            Expression e = new Expression(v, NCalc.EvaluateOptions.NoCache);
            if (!e.HasErrors())
            {
                var resultCurrent = Math.Round(Convert.ToDouble(e.Evaluate()), 4);
                if (Math.Abs(resultCurrent - resultTarget) < 0.00001)
                {
                    yield return v;
                }
            }
            else
            {
                Console.WriteLine($"Error: {e.Error}");
                Console.WriteLine($"Input: {v}");
            }

        }
    }

    public override string ToString()
    {
        return this.root.ToString();
    }
}