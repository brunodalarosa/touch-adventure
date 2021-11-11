using System.Collections.Generic;

namespace TouchAdventure.Scripts.Controllers
{
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
}