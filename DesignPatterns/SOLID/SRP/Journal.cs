namespace SOLID.SRP;

public class Journal
{
    private static int count = 0;
    private readonly List<string> entries = new List<string>();

    public int add(string text)
    {
        entries.Add($"{++count}: {text}");
        return count;
    }

    public void remove(int index)
    {
        entries.RemoveAt(index);
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, entries);
    }

    // The methods below breaks single responsibility principle
    //public void Save(string filename, bool overwrite = false)
    //{
    //    File.WriteAllText(filename, ToString());
    //}

    //public void Load(string filename)
    //{

    //}

    //public void Load(Uri uri)
    //{

    //}
}
