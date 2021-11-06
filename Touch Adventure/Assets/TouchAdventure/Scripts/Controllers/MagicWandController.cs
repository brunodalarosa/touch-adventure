using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        [field: SerializeField] private List<CloudController> CloudsPrefabs { get; set; }
        [field: SerializeField] private TouchInputController TouchInputController { get; set; }
        [field: SerializeField] private Transform CloudsParent { get; set; }
        
        private Dictionary<int, List<CloudPathPoint>> CloudsPathDict { get; set; }

        private Random Random { get; set; }

        private void Awake()
        {
            Random = new Random();
        }

        private void Start()
        {
            TouchInputController.Init(MainCamera, this);
            CloudsPathDict = new Dictionary<int, List<CloudPathPoint>>();
        }
        
        public void OnTouch(TouchCommand touchCommand)
        {
            CloudsPathCleanup();
            
            var worldPosition = touchCommand.WorldPosition;

            if (CloudsPathDict.TryGetValue(touchCommand.InputId, out var cloudPath))
            {
                var lastCloudController = cloudPath.Last(point => point.HasCloud).Controller;

                var originalRadius = lastCloudController.Collider.radius;
                
                lastCloudController.Collider.radius *= 2;
                
                if (lastCloudController.Collider.OverlapPoint(worldPosition))
                {
                    // No cloud created
                    CloudsPathDict[touchCommand.InputId].Add(new CloudPathPoint(worldPosition, null));
                }
                else
                {
                    // New Cloud created
                    var cloudController = CreateNewCloud(worldPosition);
                    CloudsPathDict[touchCommand.InputId].Add(new CloudPathPoint(worldPosition, cloudController));
                    CloudsPathExcessRemoval();
                }
                
                lastCloudController.Collider.radius -= originalRadius;
            }
            else
            {
                // New Cloud (and path) created
                Debug.Log($"New cloud path {touchCommand.InputId}");
                
                var cloudController = CreateNewCloud(worldPosition);
                
                CloudsPathDict.Add(touchCommand.InputId,
                    new List<CloudPathPoint> {new CloudPathPoint(worldPosition, cloudController)});
                
                CloudsPathExcessRemoval();
            }
        }

        private void CloudsPathCleanup()
        {
            var pathsToRemove = new List<int>();
            
            foreach (var pair in CloudsPathDict)
            {
                var cloudPathPoints = pair.Value;

                if (cloudPathPoints.Where(point => point.HasCloud).All(point => point.Poofed)) 
                    pathsToRemove.Add(pair.Key);
            }

            foreach (var key in pathsToRemove)
            {
                CloudsPathDict.Remove(key);
            }
        }

        private void CloudsPathExcessRemoval()
        {
            var allValidClouds = new List<CloudPathPoint>();
            
            foreach (var cloudPath in CloudsPathDict.Select(kvp => kvp.Value))
            {
                var validClouds = cloudPath.Where(point => point.HasCloud && !point.Poofed).ToList();
                
                allValidClouds.AddRange(validClouds);
                
            }
            
            if (allValidClouds.Count > MAXClouds)
            {
                allValidClouds[0].InstantPoof();
            }
        }

        private CloudController CreateNewCloud(Vector2 worldPosition)
        {
            var cloud = Instantiate(GetRandomCloudPrefab(),
                new Vector3(worldPosition.x, worldPosition.y), Quaternion.identity, CloudsParent);
            
            //todo init Cloud?
            return cloud;
        }

        private CloudController GetRandomCloudPrefab()
        {
            return CloudsPrefabs.ElementAt(Random.Next(CloudsPrefabs.Count));
        }
    }
}