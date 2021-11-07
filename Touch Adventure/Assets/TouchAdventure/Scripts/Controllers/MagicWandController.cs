using System;
using System.Collections.Generic;
using System.Linq;
using TouchAdventure.Scripts.Controllers.TouchInput;
using UnityEngine;
using Random = System.Random;

namespace TouchAdventure.Scripts.Controllers
{
    public partial class MagicWandController : MonoBehaviour, ITouchInputListener
    {
        private const int MAXClouds = 128;
        [field: SerializeField] private Camera MainCamera { get; set; }
        [field: SerializeField] private List<CloudPathPoint> CloudsPrefabs { get; set; }
        [field: SerializeField] private TouchInputController TouchInputController { get; set; }
        [field: SerializeField] private Transform CloudsParent { get; set; }
        
        private Dictionary<Guid, CloudPath> CloudsPathDict { get; set; }

        private Random Random { get; set; }

        public class CloudPath
        {
            public bool InputFinished { get; private set; }
            public List<CloudPathPoint> PathPoints { get; }

            public CloudPath(CloudPathPoint firstPathPoint)
            {
                PathPoints = new List<CloudPathPoint> {firstPathPoint};
                InputFinished = false;
            }

            public void FinishInput()
            {
                InputFinished = true;
            }

            public void AddPoint(CloudPathPoint newPathPoint)
            {
                PathPoints.Add(newPathPoint);
            }
        }

        private void Awake()
        {
            Random = new Random();
        }

        private void Start()
        {
            TouchInputController.Init(MainCamera, this);
            CloudsPathDict = new Dictionary<Guid, CloudPath>();
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
            
            CloudsPathCleanup();

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
                    CloudsPathExcessRemoval();
                }
            }
            else
            {
                // New Path created
                Debug.Log($"New cloud path {touchCommand.InputId}");
                
                var cloudPathPoint = CreateNewPathPoint(worldPosition, true);
                CloudsPathDict.Add(touchCommand.InputId, new CloudPath(cloudPathPoint));
                
                CloudsPathExcessRemoval();
            }
        }

        private void CloudsPathCleanup()
        {
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

        private void CloudsPathExcessRemoval()
        {
            var allValidClouds = new List<CloudPathPoint>();
            
            foreach (var cloudPath in CloudsPathDict.Select(kvp => kvp.Value))
            {
                var validClouds = cloudPath.PathPoints.Where(point => point.CloudEnabled && !point.Poofed).ToList();
                
                allValidClouds.AddRange(validClouds);
            }
            
            if (allValidClouds.Count > MAXClouds)
            {
                allValidClouds.OrderBy(point => point.TimeStamp).First().Poof();
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
    }
}