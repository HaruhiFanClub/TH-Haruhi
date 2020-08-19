
using UnityEngine;

public static class MathUtility
{
    public const float PI = 3.1415926f;
    public const float PI2 = PI * 2f;
    public const float halfPI = 1.5707963f;
    public const float nan = 0f / 0f;
    public const float infinity = 1.0f / 0.0f;
    public const float negativeInfinity = -1.0f / 0.0f;
    public const float zeroClose = 1e-07f;
    public const float large = 1e08f;
    public const float small = -1e08f;
    public const float radianToDegree = 57.2958f;
    public const float degreeToRadian = 0.0174533f;
    public const float inchTocm = 2.54f;
    public const float cmToinch = 0.3937008f;
    public const float closeSq = 0.00001f;
    public const float landSampleDistance = 10f;
    public static Vector3 infinityVector3 = new Vector3(infinity, infinity, infinity);
    public static Vector3 landRaycastOffset = new Vector3(0f, -100f, 0f);


    public static float Abs(float a)
    {
        if (a < 0) return -a;
        return a;
    }

    public static int Abs(int a)
    {
        if (a < 0) return -a;
        return a;
    }
    public static float ToDegree(float radian)
    {
        return radian * radianToDegree;
    }

    public static float ToRadian(float degree)
    {
        return degree * degreeToRadian;
    }

    public static float Radian(Vector3 from, Vector3 to)
    {
        Vector3 _from = from.normalized;
        Vector3 _to = to.normalized;
        return Mathf.Acos(_from.x * _to.x + _from.y * _to.y + _from.z * _to.z);
    }

    public static float TowardRadianXY(Vector3 toward, Vector3 target)
    {
        float radian = Radian(toward, target);
        if (radian == 0f)
            return 0f;
        if (Vector3.Cross(toward, target).z > 0f)
            return -radian;
        else
            return radian;
    }

    public static float TowardRadianXZ(Vector3 toward, Vector3 target)
    {
        float radian = Radian(toward, target);
        if (radian == 0f)
            return 0f;
        if (Vector3.Cross(toward, target).y > 0f)
            return -radian;
        else
            return radian;
    }

    public static Vector2 TowardCoordXZ(Vector3 toward, Vector3 target)
    {
        return RadianXZCoord(TowardRadianXZ(toward, target));
    }

    public static Vector2 RadianXZCoord(float radian)
    {
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));
    }

    public static Vector3 SwapYZ(Vector3 vec3)
    {
        return new Vector3(vec3.x, vec3.z, vec3.y);
    }

    public static bool FloatEqual(float from, float to)
    {
        float f = from - to;
        if (f > 0)
            return f <= 0.000001f;
        else
            return f >= -0.000001f;
    }

    public static bool IsVectorZero(Vector3 from)
    {
        return FloatEqual(from.x, 0) && FloatEqual(from.y, 0) && FloatEqual(from.z, 0);
    }

    public static float SqrDistance(Vector3 from, Vector3 to)
    {
        float x = from.x - to.x;
        float y = from.y - to.y;
        float z = from.z - to.z;
        return x * x + y * y + z * z;
    }

    public static float SqrDistanceXZ(Vector3 from, Vector3 to)
    {
        float x = from.x - to.x;
        float z = from.z - to.z;
        return x * x + z * z;
    }
    public static float SqrDistanceXY(Vector3 from, Vector3 to)
    {
        float x = from.x - to.x;
        float y = from.y - to.y;
        return x * x + y * y;
    }

    public static float Distance(Vector3 from, Vector3 to)
    {
        float x = from.x - to.x;
        float y = from.y - to.y;
        float z = from.z - to.z;
        return Mathf.Sqrt(x * x + y * y + z * z);
    }

    public static bool VertexClose(Vector3 left, Vector3 rigt)
    {
        return (left - rigt).sqrMagnitude < 0.00001f;
    }

    public static float CompareDistance(Vector3 target, Vector3 first, Vector3 second)
    {
        float firstSq = SqrDistance(first, target);
        float secondSq = SqrDistance(second, target);
        if (firstSq < secondSq)
            return -1;
        else if (firstSq > secondSq)
            return 1;
        else
            return 0;
    }

    public static float CompareDistance(Vector3 source, Vector3 target, float distance)
    {
        float distsq = SqrDistance(source, target);
        distance *= distance;
        if (distsq < distance)
            return -1;
        else if (distsq > distance)
            return 1;
        else
            return 0;
    }

    public static float DistanceXZ(Vector3 source, Vector3 target)
    {
        source.y = target.y;
        return Vector3.Distance(source, target);
    }

    public static float DistanceXY(Vector3 source, Vector3 target)
    {
        source.z = target.z;
        return Vector3.Distance(source, target);
    }

    public static Vector4 PlaneFromPoints(Vector3 t0, Vector3 t1, Vector3 t2)
    {
        Vector3 d0 = t1 - t0;
        Vector3 d1 = t2 - t0;
        Vector3 nl = Vector3.Cross(d0, d1).normalized;
        float dt = Vector3.Dot(nl, t0);
        return new Vector4(nl.x, nl.y, nl.z, -dt);
    }

    public static Vector3 VerticalProject(Vector4 plane, Vector3 pt)
    {
        return new Vector3(
           pt.x, (-plane.x * pt.x - plane.z * pt.z - plane.w) / plane.y, pt.z);
    }

    public static Vector3 VerticalProject(Vector3 t0, Vector3 t1, Vector3 t2, Vector3 pt)
    {
        Vector3 d0 = t1 - t0;
        Vector3 d1 = t2 - t0;
        Vector3 nl = Vector3.Cross(d0, d1).normalized;
        float dt = Vector3.Dot(nl, t0);
        return new Vector3(
            pt.x, (-nl.x * pt.x - nl.z * pt.z + dt) / nl.y, pt.z);
    }

    public static bool PointInTriangle(Vector3 t0, Vector3 t1, Vector3 t2, Vector3 pt)
    {
        Vector3 v0 = t1 - t0;
        Vector3 v1 = t2 - t0;
        Vector3 v2 = pt - t0;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float inver =
            1f / (dot00 * dot11 - dot01 * dot01);

        float u =
            (dot11 * dot02 - dot01 * dot12) * inver;
        if (u < 0f || u > 1f)
            return false;

        float v =
            (dot00 * dot12 - dot01 * dot02) * inver;
        if (v < 0f || v > 1f)
            return false;

        return u + v <= 1f;
    }

    public static float Distance(Vector4 plane, Vector3 pt)
    {
        return Vector3.Dot(plane, pt) + plane.w;
    }

    public static float Distance(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 pt)
    {
        float distance = -1f;

        Vector3 e0 = p0 - p2;
        Vector3 e1 = p1 - p0;
        Vector3 e2 = p2 - p1;

        Vector3 t0 = pt - p2;
        Vector3 t1 = pt - p0;
        Vector3 t2 = pt - p1;

        float d;
        float dot = Vector3.Dot(t0, e0);
        if (dot >= 0f && dot <= e0.sqrMagnitude)
        {
            dot /= e0.magnitude;
            d = t0.sqrMagnitude - dot * dot;
            if (d < distance || distance < 0f)
                distance = d;
        }

        dot = Vector3.Dot(t1, e1);
        if (dot >= 0f && dot <= e1.sqrMagnitude)
        {
            dot /= e1.magnitude;
            d = t1.sqrMagnitude - dot * dot;
            if (d < distance || distance < 0f)
                distance = d;
        }

        dot = Vector3.Dot(t2, e2);
        if (dot >= 0f && dot <= e2.sqrMagnitude)
        {
            dot /= e2.magnitude;
            d = t2.sqrMagnitude - dot * dot;
            if (d < distance || distance < 0f)
                distance = d;
        }

        d = (pt - p0).sqrMagnitude;
        if (d < distance || distance < 0f)
            distance = d;

        d = (pt - p1).sqrMagnitude;
        if (d < distance || distance < 0f)
            distance = d;

        d = (pt - p2).sqrMagnitude;
        if (d < distance || distance < 0f)
            distance = d;

        if (distance > 0f)
            return Mathf.Sqrt(distance);
        else
            return -1f;
    }

    public static float Raycast(
        Vector3 origin, Vector3 direction, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0);
        float d = Vector3.Dot(normal, direction);

        if (d >= -Mathf.Epsilon && d <= Mathf.Epsilon)
            return -1f;
        float t =
            Vector3.Dot(normal, p0 - origin) / d;
        if (t < 0f)
            return -1f;

        int i0 = 1;
        int i1 = 2;

        float n0 = normal.x < 0f ? -normal.x : normal.x;
        float n1 = normal.y < 0f ? -normal.y : normal.y;
        float n2 = normal.z < 0f ? -normal.z : normal.z;

        if (n1 > n2)
        {
            if (n1 > n0)
                i0 = 0;
        }
        else if (n2 > n0)
            i1 = 0;

        float u1 = p1[i0] - p0[i0];
        float v1 = p1[i1] - p0[i1];
        float u2 = p2[i0] - p0[i0];
        float v2 = p2[i1] - p0[i1];
        float u0 = t * direction[i0] + origin[i0] - p0[i0];
        float v0 = t * direction[i1] + origin[i1] - p0[i1];

        float alpha = u0 * v2 - u2 * v0;
        float beta = u1 * v0 - u0 * v1;
        float area = u1 * v2 - u2 * v1;

        const float epsilon = 1e-6f;
        float tolerance = -epsilon * area;

        if (area > 0)
        {
            if (alpha < tolerance || beta < tolerance || alpha + beta > area - tolerance)
                return -1f;
        }
        else
        {
            if (alpha > tolerance || beta > tolerance || alpha + beta < area - tolerance)
                return -1f;
        }

        return t;
    }

    public static float Raycast(
        Vector3 origin, Vector3 direction, Vector3[] vertices, int[] triangles)
    {
        float distance = -1f;
        if (triangles != null)
        {
            int triNum = triangles.Length / 3;

            for (int i = 0; i < triNum; ++i)
            {
                float d = Raycast(
                    origin,
                    direction,
                    vertices[triangles[i * 3 + 0]],
                    vertices[triangles[i * 3 + 1]],
                    vertices[triangles[i * 3 + 2]]);
                if (d >= 0 && (d < distance || distance < 0))
                    distance = d;
            }
        }
        return distance;
    }

    public static float RaycastNearest(
        Vector3 origin, Vector3 direction, Vector3[] vertices, int[] triangles)
    {
        if (triangles == null)
            return -1f;

        float distance = -1f;
        int triNum = triangles.Length / 3;

        for (int i = 0; i < triNum; ++i)
        {
            float d = Raycast(
                origin,
                direction,
                vertices[triangles[i * 3 + 0]],
                vertices[triangles[i * 3 + 1]],
                vertices[triangles[i * 3 + 2]]);
            if (d >= 0 && (d < distance || distance < 0))
                distance = d;
        }
        if (distance >= 0f)
            return distance;

        Ray ray =
            new Ray(origin, direction);
        Vector3 nearest = Vector3.zero;

        for (int i = 0; i < triNum; ++i)
        {
            Plane plane = new Plane(
                vertices[triangles[i * 3 + 0]],
                vertices[triangles[i * 3 + 1]],
                vertices[triangles[i * 3 + 2]]);

            float enter;
            if (plane.Raycast(ray, out enter))
            {
                float d = Distance(
                    vertices[triangles[i * 3 + 0]],
                    vertices[triangles[i * 3 + 1]],
                    vertices[triangles[i * 3 + 2]],
                    ray.GetPoint(enter));
                if (d >= 0 && (d < distance || distance < 0))
                {
                    distance = d;
                    nearest = ray.GetPoint(enter);
                }
            }
        }

        if (distance >= 0)
            return (nearest - origin).magnitude;
        else
            return -1f;
    }


    public static float Lerp(float a, float b, float t)
    {
        t = Mathf.Clamp01(t);
        return a + (b - a) * t;
    }

    public static float Angle(Vector3 from, Vector3 to)
    {
        return Mathf.Acos(Mathf.Clamp(Vector3.Dot(from, to), -1F, 1F)) * Mathf.Rad2Deg;
    }


    public static Vector3 RotateY(Vector3 point, float radian)
    {
        float sin = Mathf.Sin(radian);
        float cos = Mathf.Cos(radian);
        return new Vector3(
                point.x * cos + point.z * sin,
                point.y,
                point.z * cos - point.x * sin
                );
    }

    public static void RotateY(Vector3[] pointList, float radian)
    {
        float sin = Mathf.Sin(radian);
        float cos = Mathf.Cos(radian);
        for (int i = 0; i < pointList.Length; ++i)
        {
            Vector3 point = pointList[i];
            pointList[i] = new Vector3(
                point.x * cos + point.z * sin,
                point.y,
                point.z * cos - point.x * sin
                );
        }
    }

    public static Vector3 RotateY(Vector3 point, Vector3 center, float radian)
    {
        float sin = Mathf.Sin(radian);
        float cos = Mathf.Cos(radian);
        point -= center;
        point = new Vector3(
            point.x * cos + point.z * sin,
            point.y,
            point.z * cos - point.x * sin
            );
        return point + center;
    }
    public static Vector3 AntiRotateYByAngle(Vector3 center, Vector3 forward, float distance, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return AntiRotateY(center, forward, distance, radian);
    }
    public static Vector3 AntiRotateY(Vector3 center, Vector3 forward, float distance, float radian)
    {
        float sin = Mathf.Sin(radian);
        float cos = Mathf.Cos(radian);
        Vector3 direction = new Vector3(
            forward.x * cos - forward.z * sin,
            forward.y,
            forward.z * cos + forward.x * sin
            );
        return direction * distance + center;
    }
    public static Vector3 AntiRotateYByAngle(Vector3 point, Vector3 center, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return AntiRotateY(point, center, radian);
    }
    public static Vector3 AntiRotateY(Vector3 point, Vector3 center, float radian)
    {
        float sin = Mathf.Sin(radian);
        float cos = Mathf.Cos(radian);
        point -= center;
        point = new Vector3(
            point.x * cos - point.z * sin,
            point.y,
            point.z * cos + point.x * sin
            );
        return point + center;
    }
    public static void RotateY(Vector3[] pointList, Vector3 center, float radian)
    {
        float sin = Mathf.Sin(radian);
        float cos = Mathf.Cos(radian);
        for (int i = 0; i < pointList.Length; ++i)
        {
            Vector3 point = pointList[i] - center;
            pointList[i] = new Vector3(
                point.x * cos + point.z * sin,
                point.y,
                point.z * cos - point.x * sin
                ) + center;
        }
    }

    public static bool Intersect(float s1_x, float s1_z,
                                    float s2_x, float s2_z,
                                    float e1_x, float e1_z,
                                    float e2_x, float e2_z)
    {
        float delta = Determinant(s2_x - s1_x, e1_x - e2_x, s2_z - s1_z, e1_z - e2_z);
        if (delta <= (1e-6) && delta >= -(1e-6))  // delta=0，表示两线段重合或平行  
        {
            return false;
        }
        float namenda = Determinant(e1_x - s1_x, e1_x - e2_x, e1_z - s1_z, e1_z - e2_z) / delta;
        if (namenda > 1 || namenda < 0)
        {
            return false;
        }
        float miu = Determinant(s2_x - s1_x, e1_x - s1_x, s2_z - s1_z, e1_z - s1_z) / delta;
        if (miu > 1 || miu < 0)
        {
            return false;
        }
        return true;
    }



    private static float Determinant(float v1, float v2, float v3, float v4)  // 行列式  
    {
        return (v1 * v3 - v2 * v4);
    }

    //2个degree的顺时针角度差
    public static int PositiveMod(int num, int mod)
    {
        num %= mod;
        num += mod;
        num %= mod;
        return num;
    }

    /// <summary>
    /// 矩阵行列式的值
    /// </summary>
    /// <param name="m">矩阵</param>
    /// <returns></returns>
    public static float Det(this Matrix4x4 m)
    {
        return
          m.m03 * m.m12 * m.m21 * m.m30 - m.m02 * m.m13 * m.m21 * m.m30 - m.m03 * m.m11 * m.m22 * m.m30 + m.m01 * m.m13 * m.m22 * m.m30 +
          m.m02 * m.m11 * m.m23 * m.m30 - m.m01 * m.m12 * m.m23 * m.m30 - m.m03 * m.m12 * m.m20 * m.m31 + m.m02 * m.m13 * m.m20 * m.m31 +
          m.m03 * m.m10 * m.m22 * m.m31 - m.m00 * m.m13 * m.m22 * m.m31 - m.m02 * m.m10 * m.m23 * m.m31 + m.m00 * m.m12 * m.m23 * m.m31 +
          m.m03 * m.m11 * m.m20 * m.m32 - m.m01 * m.m13 * m.m20 * m.m32 - m.m03 * m.m10 * m.m21 * m.m32 + m.m00 * m.m13 * m.m21 * m.m32 +
          m.m01 * m.m10 * m.m23 * m.m32 - m.m00 * m.m11 * m.m23 * m.m32 - m.m02 * m.m11 * m.m20 * m.m33 + m.m01 * m.m12 * m.m20 * m.m33 +
          m.m02 * m.m10 * m.m21 * m.m33 - m.m00 * m.m12 * m.m21 * m.m33 - m.m01 * m.m10 * m.m22 * m.m33 + m.m00 * m.m11 * m.m22 * m.m33;
    }

    static Vector4[] hsv = new Vector4[3];

    public static Vector4[] MakeHSVTransformArray(float hueScale, float saturationScale, float valueScale)
    {
        float hh = (1f - hueScale) * PI2;
        float vs = valueScale * saturationScale;
        float vsu = vs * Mathf.Cos(hh);
        float vsw = vs * Mathf.Sin(hh);

        hsv[0] = new Vector4(
            0.299f * valueScale + 0.701f * vsu + 0.168f * vsw,
            0.578f * valueScale - 0.578f * vsu + 0.330f * vsw,
            0.114f * valueScale - 0.114f * vsu - 0.497f * vsw,
            1f);
        hsv[1] = new Vector4(
           0.299f * valueScale - 0.299f * vsu - 0.328f * vsw,
           0.587f * valueScale + 0.413f * vsu + 0.035f * vsw,
           0.114f * valueScale - 0.114f * vsu + 0.292f * vsw,
           1f);
        hsv[2] = new Vector4(
            0.299f * valueScale - 0.300f * vsu + 1.250f * vsw,
            0.587f * valueScale - 0.588f * vsu - 1.050f * vsw,
            0.114f * valueScale + 0.886f * vsu - 0.203f * vsw,
            1f);
        return hsv;
    }

    private static Vector3[] s_Corners = new Vector3[4];
    public static Bounds CalculateRelativeRectTransformSelfBounds(Transform root, Transform child)
    {
        RectTransform t = child.GetComponent<RectTransform>();
        if (!t)
            return new Bounds(Vector3.zero, Vector3.zero);

        Vector3 rhs = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
        t.GetWorldCorners(s_Corners);
        for (int i = 0; i < 4; i++)
        {
            Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(s_Corners[i]);
            rhs = Vector3.Min(lhs, rhs);
            vector2 = Vector3.Max(lhs, vector2);
        }
        Bounds bounds = new Bounds(rhs, Vector3.zero);
        bounds.Encapsulate(vector2);
        return bounds;
    }
}