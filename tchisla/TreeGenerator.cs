public class TreeGenerator
{
    public IEnumerable<Node> Generate(char digit, int count)
    {
        var str = new string(digit, count);
        return GetNodes(str);
    }

    public IEnumerable<Node> GetNodes(string str)
    {
        yield return new Node(Convert.ToInt32(str));
        for (int i = 1; i < str.Length; i++)
        {
            var left = GetNodes(str.Substring(0, i));
            var right = GetNodes(str.Substring(i));
            foreach (var l in left)
            {
                foreach (var r in right)
                {
                    yield return new Node(l, r);
                }
            }
        }

    }
}