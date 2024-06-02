using UnityEngine;
using UnityMeshSimplifier;

namespace LlamAcademy.Minigolf.MeshSimplifier
{
    [RequireComponent(typeof(MeshCombiner))]
    public class CombineAndSimplifyChildren : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f), Tooltip("The desired quality of the simplified mesh.")]
        private float Quality = 0.5f;

        // TODO: handle this and split meshes by type. That **may** make it where seams bounce again though.
        [SerializeField] private MaterialPhysicMaterial[] MaterialsToPhysicMaterials;

        [SerializeField] private SimplificationOptions SimplificationOptions = SimplificationOptions.Default;

        private MeshCombiner Combiner;

        private void Awake()
        {
            Combiner = GetComponent<MeshCombiner>();
        }

        private void Start()
        {
            Simplify();
        }

        private void Simplify()
        {
            Combiner.CombineMeshes(true);
            MeshFilter filter = GetComponent<MeshFilter>();

            if (!TryGetComponent(out MeshCollider collider))
            {
                collider = gameObject.AddComponent<MeshCollider>();
            }

            SimplifyMeshFilter(filter);
            collider.sharedMesh = filter.sharedMesh;
        }

        private void SimplifyMeshFilter(MeshFilter meshFilter)
        {
            Mesh sourceMesh = meshFilter.sharedMesh;
            if (sourceMesh == null) // verify that the mesh filter actually has a mesh
                return;

            // Create our mesh simplifier and setup our entire mesh in it
            UnityMeshSimplifier.MeshSimplifier meshSimplifier = new();
            meshSimplifier.SimplificationOptions = SimplificationOptions;
            meshSimplifier.Initialize(sourceMesh);

            // This is where the magic happens, lets simplify!
            meshSimplifier.SimplifyMesh(Quality);

            // Create our final mesh and apply it back to our mesh filter
            meshFilter.sharedMesh = meshSimplifier.ToMesh();
        }


    }
}
