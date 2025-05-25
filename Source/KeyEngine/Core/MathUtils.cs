using System.Numerics;

namespace KeyEngine.Core;

public static class MathUtils
{
    public const float ZeroTolerance = 1e-6f; // Value a 8x higher than 1.19209290E-07F

    public const double ZeroToleranceDouble = double.Epsilon * 8;

    public const float Pi = (float)Math.PI;

    public static float RadiansToDegrees(float radians) => (float)(radians * 57.295779513082320876798154814105);//radians * (180.0f / Pi)
   
    public static float DegreesToRadians(float degrees) => (float)(degrees * 0.017453292519943295769236907684886); //degree * (Pi / 180.0f)

    public static float Clamp01(float value) => Math.Clamp(value, 0, 1);

    public static float Lerp(float a, float b, float t) => a + (b - a) * Clamp01(t);

    public static Vector3 ToEulerAngles(Quaternion q)
    {
        var m = Matrix4x4.CreateFromQuaternion(Quaternion.Normalize(q));

        float pitch = MathF.Asin(-m.M31);       // pitch (X)       
        float yaw = MathF.Atan2(m.M21, m.M11);  // yaw   (Y)
        float roll = MathF.Atan2(m.M32, m.M33); // roll  (Z)

        return new Vector3(RadiansToDegrees(pitch), RadiansToDegrees(yaw), RadiansToDegrees(roll));
    }

    public static void Transformation(ref readonly Vector3 scaling, ref readonly Quaternion rotation, ref readonly Vector3 translation, out Matrix4x4 result)
    {
        var scaleMatrix = Matrix4x4.CreateScale(scaling);
        var rotationMatrix = Matrix4x4.CreateFromQuaternion(rotation);
        var translateMatrix = Matrix4x4.CreateTranslation(translation);

        result = scaleMatrix * rotationMatrix * translateMatrix;
    }
}
