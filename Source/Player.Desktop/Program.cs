using System;
using System.Drawing;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace KeyEngine.Player.Desktop;

internal class Program
{
    private static IWindow m_Window;
    private static GL m_Gl;

    private static uint m_VertexArrayObject;
    private static uint m_VertexBufferObject;
    private static uint m_ElementBufferObject;
    
    private static uint m_Program;

    //Vertex shaders are run on each vertex.
    private const string VertexShaderSource = @"
    #version 330 core

    layout (location = 0) in vec3 aPosition;

    void main()
    {
        gl_Position = vec4(aPosition, 1.0);
    }";

    //Fragment shaders are run on each fragment/pixel of the geometry.
    private const string FragmentShaderSource = @"
    #version 330 core

    out vec4 out_color;

    void main()
    {
        out_color = vec4(1.0, 0.5, 0.2, 1.0);
    }";

    //Vertex data, uploaded to the VBO.
    private static readonly float[] Vertices =
    [
        //X    Y      Z
        0.5f,  0.5f, 0.0f,
        0.5f, -0.5f, 0.0f,
        -0.5f, -0.5f, 0.0f,
        -0.5f,  0.5f, 0.5f
    ];

    //Index data, uploaded to the EBO.
    private static readonly uint[] Indices =
    [
        0u, 1u, 3u,
        1u, 2u, 3u
    ];

    public static void Main(string[] args)
    {
        var options = WindowOptions.Default;
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
        var input = m_Window.CreateInput();
        for (int i = 0; i < input.Keyboards.Count; i++)
        {
            input.Keyboards[i].KeyDown += KeyDown;
        }

        //Getting the opengl api for drawing to the screen.
        m_Gl = m_Window.CreateOpenGL();

        m_Gl.ClearColor(Color.CornflowerBlue);

        //Creating a vertex array.
        m_VertexArrayObject = m_Gl.GenVertexArray();
        m_Gl.BindVertexArray(m_VertexArrayObject);

        //Initializing a vertex buffer that holds the vertex data.
        m_VertexBufferObject = m_Gl.GenBuffer(); //Creating the buffer.
        m_Gl.BindBuffer(BufferTargetARB.ArrayBuffer, m_VertexBufferObject); //Binding the buffer.

        fixed (float* buf = Vertices)
        {
            m_Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(Vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
        }

        //Initializing an element buffer that holds the index data.
        m_ElementBufferObject = m_Gl.GenBuffer(); //Creating the buffer.
        m_Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, m_ElementBufferObject); //Binding the buffer.
        fixed (uint* buf = Indices)
        {
            m_Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
        }

        //Creating a vertex shader.
        var vertexShader = m_Gl.CreateShader(ShaderType.VertexShader);
        m_Gl.ShaderSource(vertexShader, VertexShaderSource);
        m_Gl.CompileShader(vertexShader);

        //Checking the shader for compilation errors.
        m_Gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out var vStatus);
        if (vStatus != (int)GLEnum.True) Console.WriteLine($"Error compiling vertex shader {m_Gl.GetShaderInfoLog(vertexShader)}");

        //Creating a fragment shader.
        var fragmentShader = m_Gl.CreateShader(ShaderType.FragmentShader);
        m_Gl.ShaderSource(fragmentShader, FragmentShaderSource);
        m_Gl.CompileShader(fragmentShader);

        //Checking the shader for compilation errors.
        m_Gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
        if (fStatus != (int)GLEnum.True) Console.WriteLine($"Error compiling fragment shader {m_Gl.GetShaderInfoLog(fragmentShader)}");

        //Combining the shaders under one shader program.
        m_Program = m_Gl.CreateProgram();
        m_Gl.AttachShader(m_Program, vertexShader);
        m_Gl.AttachShader(m_Program, fragmentShader);
        m_Gl.LinkProgram(m_Program);

        //Checking the linking for errors.
        m_Gl.GetProgram(m_Program, GLEnum.LinkStatus, out var status);
        if (status != (int)GLEnum.True) Console.WriteLine($"Error linking shader {m_Gl.GetProgramInfoLog(m_Program)}");

        //Delete the no longer useful individual shaders;
        m_Gl.DetachShader(m_Program, vertexShader);
        m_Gl.DetachShader(m_Program, fragmentShader);
        m_Gl.DeleteShader(vertexShader);
        m_Gl.DeleteShader(fragmentShader);

        //Tell opengl how to give the data to the shaders.
        const uint positionLoc = 0;
        m_Gl.EnableVertexAttribArray(positionLoc);
        m_Gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
    }

    private static unsafe void OnRender(double deltaTime)
    {
        //Clear the color channel.
        m_Gl.Clear(ClearBufferMask.ColorBufferBit);

        m_Gl.BindVertexArray(m_VertexArrayObject);
        m_Gl.UseProgram(m_Program);

        //Draw the geometry.
        m_Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, (void*)0);
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
        m_Gl.DeleteBuffer(m_VertexBufferObject);
        m_Gl.DeleteBuffer(m_ElementBufferObject);
        m_Gl.DeleteVertexArray(m_VertexArrayObject);
        m_Gl.DeleteProgram(m_Program);
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape) m_Window.Close();
    }
}
