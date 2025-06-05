using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using StbImageSharp;

namespace KeyEngine.Player.Desktop;

internal class Program
{
    private static IWindow m_Window;
    private static GL m_Gl;

    private static uint m_VertexArrayObject;
    private static uint m_VertexBufferObject;
    private static uint m_ElementBufferObject;
    
    private static uint m_Program;
    private static uint m_Texture;
    
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

    private static void OnLoad()
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
        Span<float> vertices =
        [
            //       aPosition | aTexCoords
            //X    Y      Z    | U, V
            0.5f,  0.5f, 0.0f,  1.0f, 1.0f,
            0.5f, -0.5f, 0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, 0.0f,  0.0f, 1.0f
        ];
        
        m_VertexArrayObject = m_Gl.GenVertexArray();
        m_Gl.BindVertexArray(m_VertexArrayObject);

        //Initializing a vertex buffer that holds the vertex data.
        m_VertexBufferObject = m_Gl.GenBuffer(); //Creating the buffer.
        m_Gl.BindBuffer(BufferTargetARB.ArrayBuffer, m_VertexBufferObject); //Binding the buffer.
        ReadOnlySpan<byte> vertexBytes = MemoryMarshal.AsBytes(vertices);
        m_Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)vertexBytes.Length, vertexBytes, BufferUsageARB.StaticDraw);

        //Initializing an element buffer that holds the index data.
        Span<uint> indices =
        [
            0u, 1u, 3u,
            1u, 2u, 3u
        ];
        
        m_ElementBufferObject = m_Gl.GenBuffer(); //Creating the buffer.
        m_Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, m_ElementBufferObject); //Binding the buffer.
        ReadOnlySpan<byte> indexBytes = MemoryMarshal.AsBytes(indices);
        m_Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)indexBytes.Length, indexBytes, BufferUsageARB.StaticDraw);

        //Creating a vertex shader.
        const string vertexCode = 
        """
        #version 330 core

        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec2 aTextureCoord;
        
        out vec2 frag_texCoords;

        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
            frag_texCoords = aTextureCoord;
        }
        """;

        var vertexShader = m_Gl.CreateShader(ShaderType.VertexShader);
        m_Gl.ShaderSource(vertexShader, vertexCode);
        m_Gl.CompileShader(vertexShader);

        //Checking the shader for compilation errors.
        m_Gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out var vStatus);
        if (vStatus != (int)GLEnum.True) Console.WriteLine($"Error compiling vertex shader {m_Gl.GetShaderInfoLog(vertexShader)}");

        //Creating a fragment shader.
        const string fragmentCode = 
        """
        #version 330 core
        
        in vec2 frag_texCoords;

        out vec4 out_color;
        
        uniform sampler2D uTexture;

        void main()
        {
            out_color = texture(uTexture, frag_texCoords);
        }
        """;
        
        var fragmentShader = m_Gl.CreateShader(ShaderType.FragmentShader);
        m_Gl.ShaderSource(fragmentShader, fragmentCode);
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
        const uint stride = (3 * sizeof(float)) + (2 * sizeof(float));
        
        // Enable the "aPosition" attribute in our vertex array, providing its size and stride too.
        const uint positionLoc = 0;
        m_Gl.EnableVertexAttribArray(positionLoc);
        m_Gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, stride, 0);
        
        // Now we need to enable our texture coordinates! We've defined that as location 1 so that's what we'll use
        // here. The code is very similar to above, but you must make sure you set its offset to the **size in bytes**
        // of the attribute before.
        const uint textureLoc = 1;
        m_Gl.EnableVertexAttribArray(textureLoc);
        m_Gl.VertexAttribPointer(textureLoc, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        
        // Unbind everything as we don't need it.
        m_Gl.BindVertexArray(0);
        m_Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        m_Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        
        // Now we create our texture!
        m_Texture = m_Gl.GenTexture();
        m_Gl.ActiveTexture(TextureUnit.Texture0);
        m_Gl.BindTexture(TextureTarget.Texture2D, m_Texture);


        // Upload our texture data to the GPU.
        // Let's go over each parameter used here:
        // 1. Tell OpenGL that we want to upload to the texture bound in the Texture2D target.
        // 2. We are uploading the "base" texture level, therefore, this value should be 0. You don't need to
        //    worry about texture levels for now.
        // 3. We tell OpenGL that we want the GPU to store this data as RGBA formatted data on the GPU itself.
        // 4. The image's width.
        // 5. The image's height.
        // 6. This is the image's border. This value MUST be 0. It is a leftover component from legacy OpenGL, and
        //    it serves no purpose.
        // 7. Our image data is formatted as RGBA data, therefore, we must tell OpenGL we are uploading RGBA data.
        // 8. StbImageSharp returns this data as a byte[] array, therefore, we must tell OpenGL we are uploading
        //    data in the unsigned byte format.
        // 9. The actual pointer to our data!
        var imageResult = ImageResult.FromMemory(File.ReadAllBytes("silk.png"), ColorComponents.RedGreenBlueAlpha);

        ReadOnlySpan<byte> imageData = imageResult.Data.AsSpan();
        m_Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)imageResult.Width, (uint)imageResult.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, imageData);

        m_Gl.TextureParameter(m_Program, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
        m_Gl.TextureParameter(m_Program, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
        
        m_Gl.TextureParameter(m_Program, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.LinearMipmapLinear);
        m_Gl.TextureParameter(m_Program, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
        
        m_Gl.GenerateMipmap(TextureTarget.Texture2D);
        
        // Unbind the texture as we no longer need to update it any further.
        m_Gl.BindTexture(TextureTarget.Texture2D, 0);
        
        var location = m_Gl.GetUniformLocation(m_Program, "uTexture");
        m_Gl.Uniform1(location, 0);
        
        m_Gl.Enable(EnableCap.Blend);
        m_Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        m_Gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (nint)0);
    }

    private static unsafe void OnRender(double deltaTime)
    {
        //Clear the color channel.
        m_Gl.Clear(ClearBufferMask.ColorBufferBit);

        m_Gl.BindVertexArray(m_VertexArrayObject);
        m_Gl.UseProgram(m_Program);
        
        m_Gl.ActiveTexture(TextureUnit.Texture0);
        m_Gl.BindTexture(TextureTarget.Texture2D, m_Texture);

        //Draw the geometry.
        m_Gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);  
    }

    private static void OnUpdate(double deltaTime) { }

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
        m_Gl.DeleteTexture(m_Texture);
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape) m_Window.Close();
    }
}
