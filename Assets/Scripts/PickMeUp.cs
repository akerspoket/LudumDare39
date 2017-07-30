using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickMeUp : MonoBehaviour {
    public enum PickupType
    {
        Battery,
        LightBall,
    }
    public PickupType type;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case PickupType.Battery:
                    other.gameObject.GetComponent<FlashLight>().batteryLeft = 1;
                    break;
                case PickupType.LightBall:
                    other.gameObject.GetComponent<LighBallDrop>().m_ballsLeft++;
                    break;
                default:
                    break;
            }
            Destroy(gameObject);
        }
    }
}
