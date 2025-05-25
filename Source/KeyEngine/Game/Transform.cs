using System;
using System.Numerics;
using Friflo.Engine.ECS;

namespace KeyEngine.Game;

public struct Transform : IComponent
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
}