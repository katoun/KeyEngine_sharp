using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace KeyEngine.Player.Desktop;

internal class Program
{
    private static IWindow m_Window;

    public static void Main(string[] args)
    {
        WindowOptions options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "My first Silk.NET program!";

        m_Window = Window.Create(options);

        m_Window.Load += OnLoad;
        m_Window.Update += OnUpdate;
        m_Window.Render += OnRender;

        m_Window.Run();
    }

    private static void OnLoad()
    {
        Console.WriteLine("Load!");

        IInputContext input = m_Window.CreateInput();
        for (int i = 0; i < input.Keyboards.Count; i++)
        {
            input.Keyboards[i].KeyDown += KeyDown;
        }
    }

    // These two methods are unused for this tutorial, aside from the logging we added earlier.
    private static void OnUpdate(double deltaTime)
    {
        Console.WriteLine("Update!");
    }

    private static void OnRender(double deltaTime)
    {
        Console.WriteLine("Render!");
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape) m_Window.Close();
    }
}
