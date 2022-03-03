using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _220223KCore
{
    public class VectorMath 
    {
        //Direction to Rotation credtis to Splak#9999

        public static float DegToRad(float degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }
        public static float RadToDeg(float radians)
        {
            return (float)(radians * 180 / Math.PI);
        }
        public static Vector3 RotAnglesToVector(Vector3 rotation)
        {
            double z = DegToRad(rotation.Z);
            double x = DegToRad(rotation.X);
            double num = Math.Abs(Math.Cos(x));

            return new Vector3(
                    (float)-(Math.Sin(z) * num),
                    (float)(Math.Cos(z) * num),
                    (float)Math.Sin(x)
                    );
        }
        public static Vector3 NormalVectorToGroundRotAngles(Vector3 normal)
        {
            normal = Vector3.Normalize(normal);
            Vector3 rotVal;
            rotVal.Z = -RadToDeg((float)Math.Atan2(normal.X, normal.Y));
            Vector3 rotPos = Vector3.Normalize(new Vector3(normal.Z, new Vector3(normal.X,normal.Y,0.0f).Length(), 0.0f));
            rotVal.X = RadToDeg((float)Math.Atan2(rotPos.X, rotPos.Y));
            rotVal.Y = 0;
            rotVal.X -= 90f;
            return rotVal;
        }

        public static float HeadingToPosition(Position pos, Position goalPos)
        {
            goalPos.Z = 0;
            pos.Z = 0;
            Vector3 dif = Vector3.Normalize(pos - goalPos);

            Vector3 rotVal = NormalVectorToGroundRotAngles(dif);
            
            float heading = DegToRad(rotVal.Z + 180f);
            if (heading > Math.PI)
            {
                heading += -(float)(2 * Math.PI);
            }

            return heading;
           
        }
    }
}
