using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DartsGames.CUT.MathExtensions
{
    public static class MathExtensions
    {
        public static float DistanceToLine(this Vector3 point, Vector3 lineA, Vector3 lineB)
        {
            var d = (lineB - lineA).normalized;
            var CA = point - lineA;

            var t = Vector3.Dot(d, CA);
            var P = lineA + t * d;
            return Vector3.Distance(P, point);


        }

        public static float DistanceToLine(this Vector2 point, Vector2 lineA, Vector2 lineB) =>
            DistanceToLine((Vector3)point, (Vector3)lineA, (Vector3)lineB);

        public static float SinAlphaToPoint(this Vector2 from, Vector2 to)
        {
            return (to.y - from.y) / Vector2.Distance(from, to);
        }
    }
}