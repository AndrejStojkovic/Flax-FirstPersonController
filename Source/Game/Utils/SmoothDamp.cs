using System;
using System.Collections.Generic;
using System.ComponentModel;
using FlaxEngine;

namespace Game
{
    public class Utils
    {
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
        {
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, Time.DeltaTime);
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
        {
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, float.PositiveInfinity, Time.DeltaTime);
        }

        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, [DefaultValue("float.PositiveInfinity")] float maxSpeed, [DefaultValue("Time.DeltaTime")] float deltaTime)
        {
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            float a = 2f / smoothTime;
            float b = a * deltaTime;
            float e = 1f / (1f + b + 0.48f * b * b + 0.235f * b * b * b);

            float change_x = current.X - target.X;
            float change_y = current.Y - target.Y;
            float change_z = current.Z - target.Z;
            Vector3 originalTo = target;

            float maxChangeSpeed = maxSpeed * smoothTime;
            float changeSq = maxChangeSpeed * maxChangeSpeed;
            float sqrMag = change_x * change_x + change_y * change_y + change_z * change_z;
            if (sqrMag > changeSq)
            {
                var mag = (float)Math.Sqrt(sqrMag);
                change_x = change_x / mag * maxChangeSpeed;
                change_y = change_y / mag * maxChangeSpeed;
                change_z = change_z / mag * maxChangeSpeed;
            }

            target.X = current.X - change_x;
            target.Y = current.Y - change_y;
            target.Z = current.Z - change_z;

            float temp_x = (currentVelocity.X + a * change_x) * deltaTime;
            float temp_y = (currentVelocity.Y + a * change_y) * deltaTime;
            float temp_z = (currentVelocity.Z + a * change_z) * deltaTime;

            currentVelocity.X = (currentVelocity.X - a * temp_x) * e;
            currentVelocity.Y = (currentVelocity.Y - a * temp_y) * e;
            currentVelocity.Z = (currentVelocity.Z - a * temp_z) * e;

            float output_x = target.X + (change_x + temp_x) * e;
            float output_y = target.Y + (change_y + temp_y) * e;
            float output_z = target.Z + (change_z + temp_z) * e;

            float x1 = originalTo.X - current.X;
            float y1 = originalTo.Y - current.Y;
            float z1 = originalTo.Z - current.Z;

            float x2 = output_x - originalTo.X;
            float y2 = output_y - originalTo.Y;
            float z2 = output_z - originalTo.Z;

            if (x1 * x2 + y1 * y2 + z1 * z2 > 0)
            {
                output_x = originalTo.X;
                output_y = originalTo.Y;
                output_z = originalTo.Z;

                currentVelocity.X = (output_x - originalTo.X) / deltaTime;
                currentVelocity.Y = (output_y - originalTo.Y) / deltaTime;
                currentVelocity.Z = (output_z - originalTo.Z) / deltaTime;
            }

            return new Vector3(output_x, output_y, output_z);
        }
    }
}
