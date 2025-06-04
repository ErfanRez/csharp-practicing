namespace SOLID.ISP;

public class Document
{
}

public interface IMachine
{
    void Print(Document d);
    void Fax(Document d);
    void Scan(Document d);
}

// ok if you need a multifunction machine
public class MultiFunctionPrinter : IMachine
{
    public void Print(Document d)
    {
        //
    }

    public void Fax(Document d)
    {
        //
    }

    public void Scan(Document d)
    {
        //
    }
}

public class OldFashionedPrinter : IMachine
{
    public void Print(Document d)
    {
        // yep
    }

    public void Fax(Document d)
    {
        throw new NotImplementedException();
    }

    public void Scan(Document d)
    {
        throw new NotImplementedException();
    }
}
