using UnityEngine;
using UnityMeshSimplifier;

namespace LlamAcademy.Minigolf.MeshSimplifier
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshCombiner))]
    public class CombineAndSimplifyChildren : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f), Tooltip("The desired quality of the simplified mesh.")]
        private float Quality = 0.5f;

        [SerializeField] private SimplificationOptions SimplificationOptions = SimplificationOptions.Default;

        private MeshCombiner Combiner;

        private void Awake()
        {
            Combiner = GetComponent<MeshCombiner>();
        }

        public void Simplify()
        {
            // Combines all child meshes, with configurable exclusions. This can result in duplicate vertices due to
            // our tile placement. These will be removed later by the UnityMeshSimplifier.
            // This also overrides the current MeshFilter's mesh, so we need to update the MeshCollider mesh to match!
            Combiner.CombineMeshes();
            MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter filter in filters)
            {
                SimplifyMeshFilter(filter);
            }
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

            // Simplifies the mesh, combining vertices close enough together to be considered a single vertex
            // this eliminates the "bounce" from connecting multiple different tiles
            meshSimplifier.SimplifyMesh(Quality);

            // Create our final mesh and apply it back to our mesh filter
            meshFilter.sharedMesh = meshSimplifier.ToMesh();

            if (meshFilter.TryGetComponent(out MeshCollider collider))
            {
                collider.sharedMesh = meshFilter.sharedMesh;
            }
        }
    }
}
