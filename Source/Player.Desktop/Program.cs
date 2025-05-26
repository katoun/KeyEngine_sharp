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

    private static uint Vbo;
    private static uint Ebo;
    private static uint Vao;
    private static uint Shader;

    //Vertex shaders are run on each vertex.
    private static readonly string VertexShaderSource = @"
    #version 330 core //Using version GLSL version 3.3
    layout (location = 0) in vec4 vPos;
        
    void main()
    {
        gl_Position = vec4(vPos, 1.0);
    }
    ";

    //Fragment shaders are run on each fragment/pixel of the geometry.
    private static readonly string FragmentShaderSource = @"
    #version 330 core
    out vec4 FragColor;

    void main()
    {
        FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
    }
    ";

    //Vertex data, uploaded to the VBO.
    private static readonly float[] Vertices =
    {
        //X    Y      Z
        0.5f,  0.5f, 0.0f,
        0.5f, -0.5f, 0.0f,
        -0.5f, -0.5f, 0.0f,
        -0.5f,  0.5f, 0.5f
    };

    //Index data, uploaded to the EBO.
    private static readonly uint[] Indices =
    {
        0, 1, 3,
        1, 2, 3
    };

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

        //Creating a vertex array.
        Vao = m_Gl.GenVertexArray();
        m_Gl.BindVertexArray(Vao);

        //Initializing a vertex buffer that holds the vertex data.
        Vbo = m_Gl.GenBuffer(); //Creating the buffer.
        m_Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo); //Binding the buffer.
        fixed (void* v = &Vertices[0])
        {
            m_Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(Vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
        }

        //Initializing a element buffer that holds the index data.
        Ebo = m_Gl.GenBuffer(); //Creating the buffer.
        m_Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ebo); //Binding the buffer.
        fixed (void* i = &Indices[0])
        {
            m_Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
        }

        //Creating a vertex shader.
        uint vertexShader = m_Gl.CreateShader(ShaderType.VertexShader);
        m_Gl.ShaderSource(vertexShader, VertexShaderSource);
        m_Gl.CompileShader(vertexShader);

        //Checking the shader for compilation errors.
        string infoLog = m_Gl.GetShaderInfoLog(vertexShader);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Console.WriteLine($"Error compiling vertex shader {infoLog}");
        }

        //Creating a fragment shader.
        uint fragmentShader = m_Gl.CreateShader(ShaderType.FragmentShader);
        m_Gl.ShaderSource(fragmentShader, FragmentShaderSource);
        m_Gl.CompileShader(fragmentShader);

        //Checking the shader for compilation errors.
        infoLog = m_Gl.GetShaderInfoLog(fragmentShader);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Console.WriteLine($"Error compiling fragment shader {infoLog}");
        }

        //Combining the shaders under one shader program.
        Shader = m_Gl.CreateProgram();
        m_Gl.AttachShader(Shader, vertexShader);
        m_Gl.AttachShader(Shader, fragmentShader);
        m_Gl.LinkProgram(Shader);

        //Checking the linking for errors.
        m_Gl.GetProgram(Shader, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            Console.WriteLine($"Error linking shader {m_Gl.GetProgramInfoLog(Shader)}");
        }

        //Delete the no longer useful individual shaders;
        m_Gl.DetachShader(Shader, vertexShader);
        m_Gl.DetachShader(Shader, fragmentShader);
        m_Gl.DeleteShader(vertexShader);
        m_Gl.DeleteShader(fragmentShader);

        //Tell opengl how to give the data to the shaders.
        m_Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
        m_Gl.EnableVertexAttribArray(0);
    }

    private static unsafe void OnRender(double deltaTime)
    {
        //Clear the color channel.
        m_Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

        m_Gl.BindVertexArray(Vao);
        m_Gl.UseProgram(Shader);

        //Draw the geometry.
        m_Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
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
        m_Gl.DeleteBuffer(Vbo);
        m_Gl.DeleteBuffer(Ebo);
        m_Gl.DeleteVertexArray(Vao);
        m_Gl.DeleteProgram(Shader);
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape) m_Window.Close();
    }
}
