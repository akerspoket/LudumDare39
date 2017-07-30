using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillPlayerScript : MonoBehaviour {
    public float killDistance = 3.0f;
    PlayAnimation anim;
    GameObject player;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponentInChildren<PlayAnimation>();
	}
	
	// Update is called once per frame
	void Update () {
        // To avoid different Y problems
        Vector2 myVec2Pos = new Vector2(transform.position.x, transform.position.z);
        Vector2 playersVec2Pos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 vectorBetween = myVec2Pos - playersVec2Pos; 

        if (vectorBetween.magnitude < killDistance)
        {
            // See if we see player
            Vector3 direction = new Vector3(vectorBetween.x, 0, vectorBetween.y);
            Vector3 origin = transform.position;
            origin.y = player.transform.position.y;
            direction.Normalize();
            RaycastHit[] targetsHit = Physics.RaycastAll(origin, -direction, killDistance + 1);
            int length = targetsHit.Length;
            for (int i = 0; i < length; i++)
            {
                // To avoid hitting self
                if (targetsHit[i].transform.gameObject == gameObject)
                {
                    continue;
                }
                else if (targetsHit[i].transform.gameObject != player.gameObject)
                {
                    break;
                }
                // We hit the player KILL!!!
                else if (targetsHit[i].transform.gameObject == player.gameObject)
                {
                    anim.PlayGivenAnimation("Kill");
                    // The player cant move with controllers
                    player.GetComponent<Movement>().enabled = false;
                    player.GetComponent<PlayerDeath>().ActivateDeathAnimation(gameObject);
                    GetComponent<EnemyFollowPlayer>().enabled = false;
                    direction.Normalize();
                    transform.rotation = Quaternion.LookRotation(-direction);
                    GetComponent<AudioSource>().Stop();
                    this.enabled = false;
                }
            }
        }
	}
}
