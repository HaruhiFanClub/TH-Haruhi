

using UnityEngine;
using System.Collections;

public static class GLUtility
{
    public static Material material
    {
        get
        {
            if (material_)
                return material_;
            else
                return defaultMaterial;
        }
        set
        {
            material_ = value;
        }
    }
    static Material material_;

    public static Material defaultMaterial
    {
        get
        {
            if (!defaultMaterial_)
            {
                defaultMaterial_ =
                    new Material(Shader.Find("Editor/Colored"));
                defaultMaterial_.color = Color.white;
            }
            return defaultMaterial_;
        }
    }
    static Material defaultMaterial_;

    public static int discSegment;

    static GLUtility()
    {
        discSegment = 36;
    }

    public static void UseDefaultMaterial()
    {
        if (material_)
            Object.DestroyImmediate(material_);
        material_ = null;
    }

    public static void DrawBounds(Bounds bounds, Color color)
    {
        var c = bounds.center;
        var size = bounds.size;

        float rx = size.x / 2f;
        float ry = size.y / 2f;
        float rz = size.z / 2f;
        //获取collider边界的8个顶点位置  
        Vector3 p0, p1, p2, p3;
        Vector3 p4, p5, p6, p7;
        p0 = c + new Vector3(-rx, -ry, rz);
        p1 = c + new Vector3(rx, -ry, rz);
        p2 = c + new Vector3(rx, -ry, -rz);
        p3 = c + new Vector3(-rx, -ry, -rz);
        p4 = c + new Vector3(-rx, ry, rz);
        p5 = c + new Vector3(rx, ry, rz);
        p6 = c + new Vector3(rx, ry, -rz);
        p7 = c + new Vector3(-rx, ry, -rz);

        //画线  
        material.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p0);
        GL.Vertex(p1);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p1);
        GL.Vertex(p2);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p2);
        GL.Vertex(p3);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p0);
        GL.Vertex(p3);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p4);
        GL.Vertex(p5);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p5);
        GL.Vertex(p6);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p6);
        GL.Vertex(p7);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p4);
        GL.Vertex(p7);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p0);
        GL.Vertex(p4);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p1);
        GL.Vertex(p5);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p2);
        GL.Vertex(p6);
        GL.End();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(p3);
        GL.Vertex(p7);
        GL.End();
    }

    public static void DrawRectWire(Vector3[] rect, Color color)
    {
        material.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(rect[0]);
        GL.Color(color);
        GL.Vertex(rect[1]);
        GL.Color(color);
        GL.Vertex(rect[1]);
        GL.Color(color);
        GL.Vertex(rect[2]);
        GL.Color(color);
        GL.Vertex(rect[2]);
        GL.Color(color);
        GL.Vertex(rect[3]);
        GL.Color(color);
        GL.Vertex(rect[3]);
        GL.Color(color);
        GL.Vertex(rect[0]);
        GL.End();
    }

    public static void DrawRect(Vector3[] rect, Color color)
    {
        material.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.Color(color);
        GL.Vertex(rect[0]);
        GL.Color(color);
        GL.Vertex(rect[1]);
        GL.Color(color);
        GL.Vertex(rect[2]);
        GL.Color(color);
        GL.Vertex(rect[3]);
        GL.End();
    }

    public static void DrawRectOutline(Vector3[] rect, Color faceColor, Color outlineColor)
    {
        DrawRect(rect, faceColor);
        DrawRectWire(rect, outlineColor);
    }

    public static void DrawRectWire(
        Vector3 center, Quaternion rotate, float length, float width, Color color)
    {
        float hz = width * 0.5f;
        float hx = length * 0.5f;

        material.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3(-hx, 0f, -hz) + center);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3( hx, 0f, -hz) + center);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3( hx, 0f, -hz) + center);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3( hx, 0f,  hz) + center);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3( hx, 0f,  hz) + center);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3(-hx, 0f,  hz) + center);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3(-hx, 0f,  hz) + center);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3(-hx, 0f, -hz) + center);
        GL.End();
    }

    public static void DrawRect(
        Vector3 center, Quaternion rotate, float length, float width, Color color)
    {
        float hz = width * 0.5f;
        float hx = length * 0.5f;

        material.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3(-hx, 0f, -hz) + center);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3( hx, 0f, -hz) + center);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3( hx, 0f,  hz) + center);
        GL.Color(color);
        GL.Vertex(rotate * new Vector3(-hx, 0f,  hz) + center);
        GL.End();
    }

    public static void DrawRectOutline(
        Vector3 center, Quaternion rotate, float length, float width, Color faceColor, Color outlineColor)
    {
        DrawRect(center, rotate, length, width, faceColor);
        DrawRectWire(center, rotate, length, width, outlineColor);
    }

    public static void DrawDiscWire(Vector3 center, Quaternion rotate, float radius, Color color)
    {
        float curr = 0f;
        float grad = MathUtility.PI2 / (float)discSegment;

        material.SetPass(0);
        GL.Begin(GL.LINES);
        for (int i = 0; i < discSegment; ++i)
        {
            GL.Color(color);
            GL.Vertex(
                rotate * new Vector3(
                    Mathf.Sin(curr) * radius, 0f, Mathf.Cos(curr) * radius) + center);
            GL.Color(color);
            if (i == discSegment - 1)
                GL.Vertex(
                    rotate * new Vector3(0f, 0f, radius) + center);
            else
                GL.Vertex(
                    rotate * new Vector3(
                        Mathf.Sin(curr + grad) * radius, 0f, Mathf.Cos(curr + grad) * radius) + center);
            curr += grad;
        }
        GL.End();
    }

    public static void DrawDisc(Vector3 center, Quaternion rotate, float radius, Color color)
    {
        float curr = 0f;
        float grad = MathUtility.PI2 / (float)discSegment;

        material.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        for (int i = 0; i < discSegment; ++i)
        {
            GL.Color(color);
            GL.Vertex(center);
            GL.Color(color);
            GL.Vertex(
                rotate * new Vector3(
                    Mathf.Sin(curr) * radius, 0f, Mathf.Cos(curr) * radius) + center);
            GL.Color(color);
            if (i == discSegment - 1)
                GL.Vertex(
                    rotate * new Vector3(0f, 0f, radius) + center);
            else
                GL.Vertex(
                    rotate * new Vector3(
                        Mathf.Sin(curr + grad) * radius, 0f, Mathf.Cos(curr + grad) * radius) + center);
            curr += grad;
        }
        GL.End();
    }

    public static void DrawDiscOutline(
        Vector3 center, Quaternion rotate, float radius, Color faceColor, Color outlineColor)
    {
        DrawDisc(center, rotate, radius, faceColor);
        DrawDiscWire(center, rotate, radius, outlineColor);
    }

    public static void DrawTriangles(Vector3[] vertices, int[] indices, Color color)
    {
        material.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        for (int i = 0; i < indices.Length / 3; ++i)
        {
            GL.Color(color);
            GL.Vertex(vertices[indices[i * 3 + 0]]);
            GL.Color(color);
            GL.Vertex(vertices[indices[i * 3 + 1]]);
            GL.Color(color);
            GL.Vertex(vertices[indices[i * 3 + 2]]);
        }
        GL.End();
    }

    public static void DrawArrow(Vector3 position, Vector3 toward, float length, Color color)
    {
        material.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(position);
        GL.Color(color);
        GL.Vertex(position + toward * length);
        GL.End();
    }

    public static void DrawLine(Vector3 starPos, Vector3 endPos, Color color)
    {
        material.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(starPos);
        GL.Color(color);
        GL.Vertex(endPos);
        GL.End();
    }
}
