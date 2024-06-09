using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LlamAcademy.Minigolf.MeshSimplifier
{
    /*
     * Original implementation from: https://github.com/dawid-t/Mesh-Combiner
     * Heavily modified for mesh colliders and vertex color splitting by: Chris Kurhan @ LlamAcademy 2024
     * MIT License
     *
     * Copyright (c) 2019 Dawid T
     * Copyright (c) 2024 LlamAcademy
     *
     * Permission is hereby granted, free of charge, to any person obtaining a copy
     * of this software and associated documentation files (the "Software"), to deal
     * in the Software without restriction, including without limitation the rights
     * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
     * copies of the Software, and to permit persons to whom the Software is
     * furnished to do so, subject to the following conditions:
     *
     * The above copyright notice and this permission notice shall be included in all
     * copies or substantial portions of the Software.
     *
     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
     * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
     * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
     * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
     * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
     * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
     * SOFTWARE.
     */
    [DisallowMultipleComponent]
    public class MeshCombiner : MonoBehaviour
    {
        [SerializeField] private bool deactivateCombinedChildren = true,
            deactivateCombinedChildrenMeshRenderers = false,
            destroyCombinedChildren = false;

        [field: SerializeField] public bool CombineInactiveChildren { get; set; } = true;

        public bool DeactivateCombinedChildren
        {
            get => deactivateCombinedChildren;
            set
            {
                deactivateCombinedChildren = value;
                CheckDeactivateCombinedChildren();
            }
        }

        public bool DeactivateCombinedChildrenMeshRenderers
        {
            get => deactivateCombinedChildrenMeshRenderers;
            set
            {
                deactivateCombinedChildrenMeshRenderers = value;
                CheckDeactivateCombinedChildren();
            }
        }

        public bool DestroyCombinedChildren
        {
            get => destroyCombinedChildren;
            set
            {
                destroyCombinedChildren = value;
                CheckDestroyCombinedChildren();
            }
        }

        private void CheckDeactivateCombinedChildren()
        {
            if (deactivateCombinedChildren || deactivateCombinedChildrenMeshRenderers)
            {
                destroyCombinedChildren = false;
            }
        }

        private void CheckDestroyCombinedChildren()
        {
            if (destroyCombinedChildren)
            {
                deactivateCombinedChildren = false;
                deactivateCombinedChildrenMeshRenderers = false;
            }
        }

        public Dictionary<int, PhysicsMaterial> CombineIndexToPhysicMaterial { get; private set; } = new();

        /// <summary>
        /// Combines all child meshes and stores the result into NEW SUB OBJECTS.
        /// There will be N new child gameObjects created with MeshColliders, MeshFilters, and MeshRenderers
        /// where N is the number of unique vertex colors on child objects.
        /// All objects are considered to have a single vertex color.
        /// </summary>
        /// <param name="showCreatedMeshInfo">If we should log the creation information to the console.</param>
        public void CombineMeshes(bool showCreatedMeshInfo = true)
        {
            #region Save our parent scale and our Transform and reset it temporarily:

            // When we are unparenting and get parent again then sometimes scale is a little bit different so save scale before unparenting:
            Vector3 oldScaleAsChild = transform.localScale;

            // If we have parent then his scale will affect to our new combined Mesh scale so unparent us:
            int positionInParentHierarchy = transform.GetSiblingIndex();
            Transform parent = transform.parent;
            transform.parent = null;

            // Thanks to this the new combined Mesh will have same position and scale in the world space like its children:
            Quaternion oldRotation = transform.rotation;
            Vector3 oldPosition = transform.position;
            Vector3 oldScale = transform.localScale;
            transform.rotation = Quaternion.identity;
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;

            #endregion Save Transform and reset it temporarily.

            UnparentExcludedChildren(transform);

            DoCombineMeshes(showCreatedMeshInfo);

            #region Set old Transform values:

            // Bring back the Transform values:
            transform.rotation = oldRotation;
            transform.position = oldPosition;
            transform.localScale = oldScale;

            // Get back parent and same hierarchy position:
            transform.parent = parent;
            transform.SetSiblingIndex(positionInParentHierarchy);

            // Set back the scale value as child:
            transform.localScale = oldScaleAsChild;

            #endregion Set old Transform values.
        }

        private Dictionary<Color, MeshFilter[]> GetMeshFiltersToCombine(out int totalMeshCount)
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(CombineInactiveChildren);
            MeshFilter currentObjectFilter = GetComponent<MeshFilter>();

            // exclude current object's mesh filter and any null values. Ideally you don't have one here.
            meshFilters = meshFilters
                .Where((meshFilter) => meshFilter != currentObjectFilter
                                       && meshFilter.mesh != null
                                       && meshFilter.mesh.colors.Length != 0
                ).ToArray();

            totalMeshCount = meshFilters.Length;

            return meshFilters.GroupBy(meshFilter => meshFilter.mesh.colors[0])
                .ToDictionary(keySelector => keySelector.Key, filters => filters.ToArray());
        }

        private void DoCombineMeshes(bool showCreatedMeshInfo)
        {
            Dictionary<Color, MeshFilter[]> meshFiltersGrouping =
                GetMeshFiltersToCombine(out int totalMeshCount);

            CombineInstance[] combineInstances = new CombineInstance[totalMeshCount];

            // If it will be over 65535 then use the 32 bit index buffer:
            long verticesLength = 0;

            int combineIndex = 0;
            int subMeshIndex = 0; // grouping by this based on vertex color

            foreach (KeyValuePair<Color, MeshFilter[]> grouping in meshFiltersGrouping)
            {
                MeshFilter[] filters = grouping.Value;

                foreach (MeshFilter filter in filters)
                {
                    combineInstances[combineIndex].subMeshIndex = subMeshIndex;
                    combineInstances[combineIndex].mesh = filter.sharedMesh;
                    combineInstances[combineIndex].transform = filter.transform.localToWorldMatrix;
                    verticesLength += combineInstances[combineIndex].mesh.vertices.Length;

                    if (!CombineIndexToPhysicMaterial.ContainsKey(subMeshIndex) &&
                        filter.TryGetComponent(out Collider collider))
                    {
                        Debug.Log(
                            $"Found physic material {collider.sharedMaterial.name} for submesh index {subMeshIndex}");
                        CombineIndexToPhysicMaterial.Add(subMeshIndex, collider.sharedMaterial);
                    }

                    combineIndex++;
                }

                subMeshIndex++;
            }

            // assumes no MeshRenderer on the current object and all children are using the same material.
            MeshRenderer childMeshRenderer = GetComponentInChildren<MeshRenderer>();

            // Create Mesh(es) from combineInstances:
            combineIndex = 0;

            DeactivateCombinedGameObjects(meshFiltersGrouping);
            foreach (KeyValuePair<Color, MeshFilter[]> keyValuePair in meshFiltersGrouping)
            {
                Mesh combinedMesh = new Mesh
                {
                    name = $"Combined Mesh {combineIndex}",
                    indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
                };

                // since Physic Materials cannot be applied to sub-meshes, we need discrete colliders for each!
                CombineInstance[] relevantCombineInstances =
                    combineInstances.Where(instance => instance.subMeshIndex == combineIndex).ToArray();
                for (int i = 0; i < relevantCombineInstances.Length; i++)
                {
                    relevantCombineInstances[i].subMeshIndex = 0; // force back to submesh 0
                }

                combinedMesh.CombineMeshes(relevantCombineInstances);
                // GenerateUV(combinedMesh); // only works in editor

                GameObject child = new($"Child Mesh Index {combineIndex}");
                MeshFilter filter = child.AddComponent<MeshFilter>();
                filter.sharedMesh = combinedMesh;
                MeshCollider collider = child.AddComponent<MeshCollider>();
                collider.sharedMesh = combinedMesh;
                collider.sharedMaterial = CombineIndexToPhysicMaterial[combineIndex];
                MeshRenderer renderer = child.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = childMeshRenderer.sharedMaterial;
                child.transform.SetParent(transform, false);
                child.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                if (showCreatedMeshInfo)
                {
                    Debug.Log($"<color=#00ff00><b>Mesh \"{name}\" was created from {combineInstances.Length} " +
                              $"child meshes and has {verticesLength} vertices. " +
                              $"Combine Mesh Index {combineIndex}</color>");
                }

                combineIndex++;
            }
        }

        private void DeactivateCombinedGameObjects(Dictionary<Color, MeshFilter[]> meshFilterGrouping)
        {
            foreach (KeyValuePair<Color, MeshFilter[]> keyValuePair in meshFilterGrouping)
            {
                for (int i = keyValuePair.Value.Length - 1; i >= 0; i--)
                {
                    if (!destroyCombinedChildren)
                    {
                        if (keyValuePair.Value[i].TryGetComponent(out Collider collider))
                        {
                            collider.enabled =
                                false; // since we're creating a new collider, we definitely don't want the old one
                        }

                        if (deactivateCombinedChildren)
                        {
                            keyValuePair.Value[i].gameObject.SetActive(false);
                        }

                        if (deactivateCombinedChildrenMeshRenderers)
                        {
                            MeshRenderer meshRenderer = keyValuePair.Value[i].gameObject.GetComponent<MeshRenderer>();
                            if (meshRenderer != null)
                            {
                                meshRenderer.enabled = false;
                            }
                        }
                    }
                    else
                    {
#if UNITY_EDITOR
                        DestroyImmediate(keyValuePair.Value[i].gameObject);
#else
                        Destroy(keyValuePair.Value[i].gameObject);
#endif
                    }
                }
            }
        }


        /// <summary>
        /// Recursively checks all children for the "ExcludeFromCombine" tag and unparents them so they are excluded from
        /// combination or accidental destruction / deactivation
        /// </summary>
        /// <param name="parent"></param>
        private void UnparentExcludedChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                if (child.CompareTag("ExcludeFromCombine"))
                {
                    child.transform.SetParent(null, true);
                }
                else if (child.childCount != 0)
                {
                    UnparentExcludedChildren(child);
                }
            }
        }
    }
}
