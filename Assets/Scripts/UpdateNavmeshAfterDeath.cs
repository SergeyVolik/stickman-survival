using DG.Tweening.Core.Easing;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;

namespace Prototype
{
    public class UpdateNavmeshAfterDeath : MonoBehaviour
    {
        private NavmeshCut navMeshCut;

        private void Awake()
        {
            navMeshCut = GetComponent<NavmeshCut>();
          
            var heath = GetComponent<HealthData>();

            heath.onDeath += () =>
            {
                navMeshCut.rectangleSize = new Vector2();
                navMeshCut.ForceUpdate();
            };
        }
    }
}
