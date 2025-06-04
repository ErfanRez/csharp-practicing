namespace SOLID.LSP;

public class Square : Rectangle
{
    //public new int Width
    //{
    //  set { base.Width = base.Height = value; }
    //}

    //public new int Height
    //{ 
    //  set { base.Width = base.Height = value; }
    //}

    public override int Width // nasty side effects
    {
        set { base.Width = base.Height = value; }
    }

    public override int Height
    {
        set { base.Width = base.Height = value; }
    }

}

