namespace Creational;
public class AbstractFactory
{
    public static void Main()
    {
        var app = new Application(new MacFactory());
        app.RenderUI();
    }

    interface IButton
    {
        void Render();
    }

    interface ICheckbox
    {
        void Render();
    }

    interface IGUIFactory
    {
        IButton CreateButton();
        ICheckbox CreateCheckbox();
    }

    class WinButton : IButton
    {
        public void Render()
        {
            // Render Windows button
        }
    }

    class MacButton : IButton
    {
        public void Render()
        {
            // Render Mac button
        }
    }

    class WinCheckbox : ICheckbox
    {
        public void Render()
        {
            // Render Windows checkbox
        }
    }

    class MacCheckBox : ICheckbox
    {
        public void Render()
        {
            // Render Mac checkbox
        }
    }

    class WinFactory : IGUIFactory
    {
        public IButton CreateButton()
        {
            return new WinButton();
        }
        public ICheckbox CreateCheckbox()
        {
            return new WinCheckbox();
        }
    }

    class MacFactory : IGUIFactory
    {
        public IButton CreateButton()
        {
            return new MacButton();
        }
        public ICheckbox CreateCheckbox()
        {
            return new MacCheckBox();
        }
    }

    class Application
    {
        private readonly IButton _button;
        private readonly ICheckbox _checkBox;
        private readonly IGUIFactory _guiFactory;

        public Application(IGUIFactory guiFactory)
        {
            _guiFactory = guiFactory;
            _button = _guiFactory.CreateButton();
            _checkBox = _guiFactory.CreateCheckbox();
        }

        public void RenderUI()
        {
            _button.Render();
            _checkBox.Render();
        }
    }
}

