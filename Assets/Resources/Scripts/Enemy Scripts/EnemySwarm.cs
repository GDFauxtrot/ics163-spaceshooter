﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarm : MonoBehaviour {

    /* This class will control:
     *      Spawning Enemy Waves
     *      When Enemies Move
     *      Notifiying when the wave is destroyed / finished
     */


    public float formation_width, formation_height;
    public GameObject enemy_prefab;
    public float minX;
    public float maxX;

    // Sets direction of movement (left if true, right if false)
    // TODO: Sync these across all spawned enemies
    public bool move_left = true;
    public float speed = 5f;
    public float spawn_delay = 0.2f;


    // Use this for initialization
    void Start () {
        SpawnUntilFull();
	}
	
	// Update is called once per frame
	void Update () {
        MoveFormationSideToSide();
        if (AllMembersDead())
        {
            Debug.Log("Empty Formation");
            SpawnUntilFull();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(this.transform.position,
                            new Vector3(formation_width, formation_height, 0));
    }


        /* Spawning
         * --------
         * Each enemy needs to be given an end point
         * Calculate start point based on movement path
         * Spawn
         */

    void SpawnAll()
    {
        foreach (Transform child in this.transform)
        {
            GameObject enemy = Instantiate(enemy_prefab, child.transform.position,
                                            Quaternion.identity) as GameObject;
            enemy.transform.parent = child;
        }
    }

    void MoveFormationSideToSide()
    {
        // Move left and right, until it reaches the edge, then change direciton
        if (move_left)
        {
            // Direction is moving left
            this.transform.position += Vector3.left * speed * Time.deltaTime;
        }
        else
        {
            // Direction is moving right
            this.transform.position += Vector3.right * speed * Time.deltaTime;
        }
        CheckPositionAndDirection();
    }

    void CheckPositionAndDirection()
    {
        // Restricts the enemy to the gamespace
        float newX = Mathf.Clamp(transform.position.x, minX, maxX);
        this.transform.position = new Vector3(newX, this.transform.position.y,
                                                  this.transform.position.z);

        if (newX <= minX)
        {
            move_left = false;
        }
        else
        if (newX >= maxX)
        {
            move_left = true;
        }
    }

    bool AllMembersDead()
    {
        foreach (Transform child_position_game_object in this.transform)
        {
            if (child_position_game_object.childCount > 0)
            {
                return false;
            }
        }
        return true;
    }

    Transform NextFreePosition()
    {
        foreach (Transform child_position_game_object in this.transform)
        {
            if (child_position_game_object.childCount == 0)
            {
                return child_position_game_object.transform;
            }
        }
        return null;
    }

    void SpawnUntilFull()
    {
        Transform free_position = NextFreePosition();
        if (free_position)
        {
            GameObject enemy = Instantiate(enemy_prefab, free_position.position,
                                       Quaternion.identity) as GameObject;
            enemy.transform.parent = free_position;
        }

        if (NextFreePosition())
        {
            Invoke("SpawnUntilFull", spawn_delay);
        }
    }

}