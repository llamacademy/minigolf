using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LlamAcademy.Minigolf.MeshSimplifier;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace LlamAcademy.Minigolf.LevelManagement
{
    public class LevelSpawner : MonoBehaviour
    {
        [SerializeField] private CombineAndSimplifyChildren Combiner;
        [SerializeField] private PlayerController Controller;
        [SerializeField] private LevelDataSO LevelData;
        [SerializeField] [Range(0.5f, 5)] private float TotalAnimationTime = 2f;
        [SerializeField] [Range(0.1f, 5)] private float AnimationSpeed = 1f;
        [SerializeField] private Rigidbody Ball;
        [SerializeField] private AnimationCurve AnimateInCurve;

        private Tilemap Tilemap;
        private Grid Grid;

        private void Awake()
        {
            if (LevelData == null || LevelData.Level.Par.Length != 4 || LevelData.Level.WorldObjectPositions.Count == 0)
            {
                Debug.LogError($"Level could not be initialized because LevelData was misconfigured! LevelData: {LevelData?.name}: {JsonUtility.ToJson(LevelData?.Level)}");
                SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
                return;
            }

            Ball.useGravity = false;
            Ball.GetComponent<Collider>().enabled = false;

            SetupLevel();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(TotalAnimationTime + 1 * AnimationSpeed);
            yield return null;
            Combiner.Simplify();
            Ball.useGravity = true;
            Ball.GetComponent<Collider>().enabled = true;
            Controller.enabled = true;

            GameObject levelBoundsGO = new GameObject("Level Bounds");
            levelBoundsGO.AddComponent<BoxCollider>();
            LevelBounds levelBounds = levelBoundsGO.AddComponent<LevelBounds>();
            levelBounds.Resize(Tilemap.transform);
        }

        private void SetupLevel()
        {
            FindTilemap();
            ClearTilemap();

            int tileCount = LevelData.Level.WorldObjectPositions.Count;
            float baseDelay = TotalAnimationTime / tileCount;
            int index = 0;

            List<PrefabSpawnData> spawnData = new(LevelData.Level.WorldObjectPositions);
            spawnData.Sort();

            foreach (PrefabSpawnData data in spawnData)
            {
                GameObject prefab = Resources.Load<GameObject>(data.PrefabResourcePath);

                Vector3 spawnLocation = data.Position + Vector3.up * AnimateInCurve.Evaluate(0);
                GameObject instance = Instantiate(prefab, spawnLocation, data.Rotation, Tilemap.transform);
                instance.transform.localScale = Vector3.zero;

                StartCoroutine(AnimateIn(instance.transform, data.Scale, baseDelay * index));
                index++;
            }
        }

        private IEnumerator AnimateIn(Transform transform, Vector3 targetScale, float delay)
        {
            Vector3 targetLocation = transform.position + Vector3.up;
            yield return new WaitForSeconds(delay);
            float time = 0;
            while (time < 1)
            {
                time += Time.deltaTime * AnimationSpeed;
                float yOffset = AnimateInCurve.Evaluate(time);
                transform.position = targetLocation + Vector3.up * yOffset;
                transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, time);
                yield return null;
            }

            transform.position = targetLocation;
            transform.localScale = targetScale;
        }

        private void FindTilemap()
        {
            Tilemap = FindObjectsByType<Tilemap>(FindObjectsSortMode.None).FirstOrDefault();
            Grid = FindObjectsByType<Grid>(FindObjectsSortMode.None).FirstOrDefault();

            if (Tilemap == null || Grid == null)
            {
                Debug.LogError($"Scene {SceneManager.GetActiveScene().name} is invalid! Level requires a Tilemap and Grid!");
            }
        }

        private void ClearTilemap()
        {
            for (int i = Tilemap.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(Tilemap.transform.GetChild(i).gameObject);
            }
        }
    }
}
