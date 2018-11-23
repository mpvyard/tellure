using ServiceStack;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace TSProcessor.CLI.IO
{
    static class JsConfigForVectors
    {
        #region Vector3
        public static string Vector3Serializer(Vector3 vector3) =>
             new[] { vector3.X, vector3.Y, vector3.Z }.ToJson();
        #endregion

        #region Vector4
        public static string Vector4Serializer(Vector4 vector4)
        {
            if (float.IsNaN(vector4.Y) &&
                float.IsNaN(vector4.Z) &&
                float.IsNaN(vector4.W))
            {
                return vector4.X.ToString();
            }

            var vc = new List<float>() { vector4.X };
            if (!float.IsNaN(vector4.Y))
            {
                vc.Add(vector4.Y);
            }

            if (!float.IsNaN(vector4.Z))
            {
                vc.Add(vector4.Z);
            }

            if (!float.IsNaN(vector4.W))
            {
                vc.Add(vector4.W);
            }

            return vc.ToJson();
        }

        public static Vector4 Vector4Deserializer(string vector4String)
        {
            var arr = vector4String.FromJson<float[]>();
            if (arr.Length > 4)
            {
                throw new InvalidOperationException("Use Vector intead of Vector4, the size is bigger then 4");
            }

            var vec = new Vector4(float.NaN)
            {
                X = arr[0]
            };
            switch (arr.Length)
            {
                case 2:
                    vec.Y = arr[1];
                    break;
                case 3:
                    vec.Y = arr[1];
                    vec.Z = arr[2];
                    break;
                case 4:
                    vec.Y = arr[1];
                    vec.Z = arr[2];
                    vec.W = arr[3];
                    break;
                default:
                    break;
            }

            return vec;
        }
        #endregion
    }
}
