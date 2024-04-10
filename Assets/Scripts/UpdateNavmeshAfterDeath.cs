using DG.Tweening.Core.Easing;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;

namespace Prototype
{
    public class UpdateNavmeshAfterDeath : MonoBehaviour
    {
        private NavmeshCut bounds;
        public int opentag = 0;
        public int closedtag = 1;

        private void Awake()
        {
            bounds = GetComponent<NavmeshCut>();
          
            var heath = GetComponent<HealthData>();

            heath.onDeath += () =>
            {
                bounds.rectangleSize = new Vector2();
                bounds.ForceUpdate();
            };
        }
    }
}
