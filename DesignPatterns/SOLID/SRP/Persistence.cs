namespace SOLID.SRP;

public class Persistence
{
    public void Save(string filename, Journal journal, bool saveToFile = false)
    {

        File.WriteAllText(filename, journal.ToString());

    }
}
