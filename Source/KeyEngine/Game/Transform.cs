using System;
using System.Numerics;

namespace KeyEngine.Game;
using Core;

public class Transform : Component
{
    private Transform parent;

    private Vector3 Position = Vector3.Zero;
    private Quaternion Rotation = Quaternion.Identity;
    private Vector3 Scale = Vector3.One;

    public Matrix4x4 WorldMatrix = Matrix4x4.Identity;
    public Matrix4x4 LocalMatrix = Matrix4x4.Identity;


    public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);
    public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);
    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Rotation);

    public Transform Parent
    {
        get { return parent; }
        set
        {
            if (parent == value) return;
        }
    }

    public void UpdateLocalMatrix()
    {
        MathUtils.Transformation(ref Scale, ref Rotation, ref Position, out LocalMatrix);
    }
}