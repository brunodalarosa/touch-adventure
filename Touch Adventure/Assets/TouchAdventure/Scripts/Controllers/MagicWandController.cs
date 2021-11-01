using System;
using System.Collections.Generic;
using System.Linq;
using TouchAdventure.Scripts.Controllers.TouchInput;
using UnityEngine;
using Random = System.Random;

namespace TouchAdventure.Scripts.Controllers
{
    public class MagicWandController : MonoBehaviour, ITouchInputListener
    {
        private const float CloudRadius = 0.17f;
        [field: SerializeField] private Camera MainCamera { get; set; }
        [field: SerializeField] private List<CloudController> CloudsPrefabs { get; set; }
        [field: SerializeField] private TouchInputController TouchInputController { get; set; }
        [field: SerializeField] private Transform CloudsParent { get; set; }
        
        private Dictionary<Guid, List<CloudController>> CloudsPathDict { get; set; }

        private Random Random { get; set; }

        private void Awake()
        {
            Random = new Random();
        }

        private void Start()
        {
            TouchInputController.Init(MainCamera, this);
            CloudsPathDict = new Dictionary<Guid, List<CloudController>>();
        }

        
        //todo implementar nova abordagem que salva os pontos no espaço e instancia nuvens no caminho, para matar o pproblema de arrastar o dedo devagar e deiixar fazer overlap!
        public void OnTouch(TouchCommand touchCommand)
        {
            CloudsPathCleanup();
            
            var worldPosition = touchCommand.WorldPosition;

            if (CloudsPathDict.TryGetValue(touchCommand.InputId, out var cloudPath))
            {
                var collisions = Physics2D.OverlapCircleAll(touchCommand.WorldPosition, CloudRadius, LayerMask.GetMask("Clouds"));
            
                var cloudsGameObjects = cloudPath.Select(controller => controller.gameObject);
                if (collisions.Any(collision => cloudsGameObjects.Contains(collision.gameObject)))
                {
                    Debug.Log($"didnt instantiate collided cloud!");
                }
                else
                {
                    var cloud = CreateNewCloud(worldPosition);
                    CloudsPathDict[touchCommand.InputId].Add(cloud);
                }
            }
            else
            {
                Debug.Log($"New cloud path {touchCommand.InputId}");
                var cloud = CreateNewCloud(worldPosition);
                CloudsPathDict.Add(touchCommand.InputId, new List<CloudController> {cloud});
            }
        }

        private void CloudsPathCleanup()
        {
            var pathsToRemove = new List<Guid>();
            
            foreach (var kvp in CloudsPathDict)
            {
                kvp.Value.RemoveAll(controller => controller == null);

                if (kvp.Value.Count == 0) pathsToRemove.Add(kvp.Key);
            }

            foreach (var key in pathsToRemove)
            {
                CloudsPathDict.Remove(key);
            }
        }

        private CloudController CreateNewCloud(Vector2 worldPosition)
        {
            var cloud = Instantiate(GetRandomCloudPrefab(), new Vector3(worldPosition.x, worldPosition.y), Quaternion.identity, CloudsParent);
            //todo init Cloud?
            return cloud;
        }

        private CloudController GetRandomCloudPrefab()
        {
            return CloudsPrefabs.ElementAt(Random.Next(CloudsPrefabs.Count));
        }
    }
}