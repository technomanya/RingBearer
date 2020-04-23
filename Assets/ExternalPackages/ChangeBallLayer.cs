using Assets.Scripts;
using UnityEngine;

namespace Assets.ExternalPackages
{
    public class ChangeBallLayer : MonoBehaviour {
        
        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                other.gameObject.tag = "ScoredPlayer";
            }
        }

    }
}