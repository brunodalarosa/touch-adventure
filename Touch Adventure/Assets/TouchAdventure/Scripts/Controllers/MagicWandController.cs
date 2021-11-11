using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TouchAdventure.Scripts.Controllers.TouchInput;
using UnityEngine;
using Random = System.Random;

namespace TouchAdventure.Scripts.Controllers
{
    public class MagicWandController : MonoBehaviour, ITouchInputListener
    {
        private const int MAXClouds = 128;
        private readonly WaitForSeconds CloudsPathCleanupInterval = new WaitForSeconds(5);
        
        [field: SerializeField] private Camera MainCamera { get; set; }
        [field: SerializeField] private List<CloudPathPoint> CloudsPrefabs { get; set; }
        [field: SerializeField] private TouchInputController TouchInputController { get; set; }
        [field: SerializeField] private Transform CloudsParent { get; set; }
        
        private Random Random { get; set; }
        private Dictionary<Guid, CloudPath> CloudsPathDict { get; set; }
        private Queue<CloudPathPoint> CloudsQueue { get; set; }

        private void Awake()
        {
            Random = new Random();
            CloudsPathDict = new Dictionary<Guid, CloudPath>();
            CloudsQueue = new Queue<CloudPathPoint>();
        }

        private void Start()
        {
            TouchInputController.Init(MainCamera, this);
            StartCoroutine(CloudsPathCleanup());
        }
        
        public void OnTouch(TouchCommand touchCommand)
        {
            if (touchCommand.FinishingInputCommand)
            {
                if (CloudsPathDict.TryGetValue(touchCommand.InputId, out var finishingCloudPath))
                {
                    finishingCloudPath.FinishInput();
                }

                return;
            }

            var worldPosition = touchCommand.WorldPosition;

            if (CloudsPathDict.TryGetValue(touchCommand.InputId, out var cloudPath))
            {
                var lastCloudController = cloudPath.PathPoints.Last(point => point.CloudEnabled).Controller;
                if (lastCloudController == null || !lastCloudController.gameObject.activeInHierarchy) return;
                
                lastCloudController.Collider.radius *= 2;
                
                if (lastCloudController.Collider.OverlapPoint(worldPosition))
                {
                    lastCloudController.Collider.radius /= 2;

                    // Cloud disabled
                    var newPathPoint = CreateNewPathPoint(worldPosition, false);
                    CloudsPathDict[touchCommand.InputId].AddPoint(newPathPoint);
                }
                else
                {
                    lastCloudController.Collider.radius /= 2;

                    // Cloud enabled
                    var newPathPoint = CreateNewPathPoint(worldPosition, true);
                    CloudsPathDict[touchCommand.InputId].AddPoint(newPathPoint);
                    CloudsQueue.Enqueue(newPathPoint);
                    CloudQueueCleanup();
                }
            }
            else
            {
                // New Path created
                Debug.Log($"New cloud path {touchCommand.InputId}");
                
                var cloudPathPoint = CreateNewPathPoint(worldPosition, true);
                CloudsPathDict.Add(touchCommand.InputId, new CloudPath(cloudPathPoint));
                CloudsQueue.Enqueue(cloudPathPoint);
                
                CloudQueueCleanup();
            }
        }

        private IEnumerator CloudsPathCleanup()
        {
            while (true)
            {
                yield return CloudsPathCleanupInterval;
            
                var pathsToRemove = new List<Guid>();
            
                foreach (var pair in CloudsPathDict)
                {
                    var path = pair.Value;

                    if (path.InputFinished && path.PathPoints.Where(point => point.CloudEnabled).All(point => point.Poofed)) 
                        pathsToRemove.Add(pair.Key);
                }

                foreach (var key in pathsToRemove)
                {
                    Debug.Log($"Cleanup -> removing path {key}");
                    CloudsPathDict.Remove(key);
                }
            }
        }

        private void CloudQueueCleanup()
        {
            if (CloudsQueue.Count > MAXClouds)
            {
                CloudPathPoint cloudToPoof;
                
                do cloudToPoof = CloudsQueue.Dequeue();
                while (cloudToPoof.Poofed);

                cloudToPoof.Poof();
            }
        }

        private CloudPathPoint CreateNewPathPoint(Vector2 worldPosition, bool enableCloud)
        {
            var cloud = Instantiate(GetRandomCloudPrefab(),
                new Vector3(worldPosition.x, worldPosition.y), Quaternion.identity, CloudsParent);
            
            cloud.Init(worldPosition, enableCloud);
            return cloud;
        }

        private CloudPathPoint GetRandomCloudPrefab()
        {
            return CloudsPrefabs.ElementAt(Random.Next(CloudsPrefabs.Count));
        }

        private void OnDestroy()
        {
            StopCoroutine(CloudsPathCleanup());
        }
    }
}