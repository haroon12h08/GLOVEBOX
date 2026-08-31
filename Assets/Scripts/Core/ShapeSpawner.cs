using UnityEngine;
using Glovebox.Physics;

namespace Glovebox.Core
{
    /// <summary>
    /// Spawns 5 distinct shapes of 5 distinct vibrant colors inside the enlarged glovebox enclosure:
    /// 1. Red Sphere
    /// 2. Cyan Cube
    /// 3. Emerald Green Cylinder
    /// 4. Golden Yellow Capsule
    /// 5. Purple Octahedron / Diamond (Custom 3D Mesh shape)
    /// </summary>
    public static class ShapeSpawner
    {
        private const string RootName = "MicrogravityShapes";

        public static void SpawnShapes()
        {
            // Remove existing shapes container if present
            GameObject existing = GameObject.Find(RootName);
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject root = new GameObject(RootName);

            // 1. Red Sphere
            CreateShape(
                root,
                "Shape_RedSphere",
                PrimitiveType.Sphere,
                null,
                new Vector3(-3.2f, 1.2f, 1.0f),
                new Vector3(1.2f, 1.2f, 1.2f),
                new Color(1.0f, 0.22f, 0.36f), // Bright Red/Crimson
                0.3f, 0.8f);

            // 2. Cyan Cube
            CreateShape(
                root,
                "Shape_CyanCube",
                PrimitiveType.Cube,
                null,
                new Vector3(3.4f, -0.8f, 1.8f),
                new Vector3(1.1f, 1.1f, 1.1f),
                new Color(0.0f, 0.90f, 1.0f), // Neon Cyan
                0.2f, 0.7f);

            // 3. Emerald Green Cylinder
            CreateShape(
                root,
                "Shape_GreenCylinder",
                PrimitiveType.Cylinder,
                null,
                new Vector3(-1.8f, -1.2f, -1.2f),
                new Vector3(0.9f, 0.9f, 0.9f),
                new Color(0.0f, 0.90f, 0.46f), // Emerald Green
                0.4f, 0.6f);

            // 4. Golden Yellow Capsule
            CreateShape(
                root,
                "Shape_YellowCapsule",
                PrimitiveType.Capsule,
                null,
                new Vector3(2.4f, 1.6f, -0.8f),
                new Vector3(0.9f, 1.2f, 0.9f),
                new Color(1.0f, 0.77f, 0.0f), // Golden Yellow
                0.1f, 0.9f);

            // 5. Purple Octahedron / Diamond (Custom 3D Mesh)
            Mesh octaMesh = CreateOctahedronMesh();
            CreateShape(
                root,
                "Shape_PurpleDiamond",
                PrimitiveType.Quad, // Mesh replaced
                octaMesh,
                new Vector3(0.0f, 0.4f, 0.0f),
                new Vector3(1.3f, 1.3f, 1.3f),
                new Color(0.83f, 0.0f, 0.98f), // Deep Purple/Magenta
                0.5f, 0.8f);

            Debug.Log("[ShapeSpawner] Successfully spawned 5 distinct shapes of 5 distinct colors!");
        }

        private static void CreateShape(
            GameObject parent,
            string name,
            PrimitiveType primitiveType,
            Mesh customMesh,
            Vector3 position,
            Vector3 scale,
            Color color,
            float metallic,
            float smoothness)
        {
            GameObject obj;
            if (customMesh != null)
            {
                obj = new GameObject(name);
                MeshFilter mf = obj.AddComponent<MeshFilter>();
                mf.sharedMesh = customMesh;

                MeshRenderer mr = obj.AddComponent<MeshRenderer>();

                MeshCollider mc = obj.AddComponent<MeshCollider>();
                mc.sharedMesh = customMesh;
                mc.convex = true;
            }
            else
            {
                obj = GameObject.CreatePrimitive(primitiveType);
                obj.name = name;
            }

            obj.transform.SetParent(parent.transform, false);
            obj.transform.localPosition = position;
            obj.transform.localScale = scale;

            // Runtime Material creation with Standard Shader
            Shader standardShader = Shader.Find("Standard");
            if (standardShader == null) standardShader = Shader.Find("Diffuse");
            Material mat = new Material(standardShader);
            mat.name = name + "_Mat";
            mat.color = color;
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);

            obj.GetComponent<Renderer>().material = mat;

            // Physics & Microgravity Setup
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null) rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.mass = 1.0f;
            rb.linearDamping = 0.05f;
            rb.angularDamping = 0.05f;

            if (obj.GetComponent<MicrogravityObject>() == null)
            {
                obj.AddComponent<MicrogravityObject>();
            }
        }

        /// <summary>
        /// Procedurally generates a 3D Octahedron / Diamond Mesh.
        /// </summary>
        private static Mesh CreateOctahedronMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = "OctahedronMesh";

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, 1, 0),   // Top
                new Vector3(0, -1, 0),  // Bottom
                new Vector3(-1, 0, 0), // Left
                new Vector3(1, 0, 0),  // Right
                new Vector3(0, 0, 1),  // Front
                new Vector3(0, 0, -1)  // Back
            };

            int[] triangles = new int[]
            {
                // Top 4 faces
                0, 4, 3,
                0, 3, 5,
                0, 5, 2,
                0, 2, 4,
                // Bottom 4 faces
                1, 3, 4,
                1, 5, 3,
                1, 2, 5,
                1, 4, 2
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
