using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Drawing;

namespace KeyEngine.Player.Desktop;

internal class Program
{
    private static IWindow m_Window;
    private static GL m_Gl;

    public static void Main(string[] args)
    {
        WindowOptions options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "Player.Desktop w/ Silk.NET";

        m_Window = Window.Create(options);

        m_Window.Load += OnLoad;
        m_Window.Render += OnRender;
        m_Window.Update += OnUpdate;
        m_Window.Closing += OnClose;
        m_Window.FramebufferResize += OnFramebufferResize;
        

        m_Window.Run();

        m_Window.Dispose();
    }

    private static unsafe void OnLoad()
    {
        IInputContext input = m_Window.CreateInput();
        for (int i = 0; i < input.Keyboards.Count; i++)
        {
            input.Keyboards[i].KeyDown += KeyDown;
        }

        //Getting the opengl api for drawing to the screen.
        m_Gl = m_Window.CreateOpenGL();

        m_Gl.ClearColor(Color.CornflowerBlue);
    }

    private static unsafe void OnRender(double deltaTime)
    {
        //Clear the color channel.
        m_Gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    private static void OnUpdate(double deltaTime)
    {
       
    }

    private static void OnFramebufferResize(Vector2D<int> newSize)
    {
        m_Gl.Viewport(newSize);
    }

    private static void OnClose()
    {
        //Remember to delete the buffers.
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape) m_Window.Close();
    }
}
