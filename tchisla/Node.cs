public class Node
{
    public int? val;
    public Node? left;
    public Node? right;

    public Node(int val)
    {
        this.val = val;
    }

    public Node(Node left, Node right)
    {
        this.left = left;
        this.right = right;
    }

    public string[] GetVariants()
    {
        string[] prefixes = new string[] { "", "Sqrt", "-"};
        if (val.HasValue)
        {
            return prefixes.Select(s => $"{s}({val.Value})").ToArray();
        }
        else
        {
            var leftVariants = left!.GetVariants();
            var rightVariants = right!.GetVariants();
            return prefixes.SelectMany(s => leftVariants.SelectMany(l => rightVariants.SelectMany(r =>
            new string[]{
                $"{s}({l}+{r})",
                $"{s}({l}-{r})",
                $"{s}({l}*{r})",
                $"{s}({l}/{r})",
                $"{s}(Pow({l},{r}))",
                })
        )).ToArray();
        }
    }

    public override string ToString()
    {
        if (val.HasValue)
        {
            return val.Value.ToString();
        }
        else
        {
            return $"({left!.ToString()}-{right!.ToString()})";
        }
    }
}