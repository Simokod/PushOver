﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Camera_Controller : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
    void Start() {
        offset = transform.position - player.transform.position;
    }
    void LateUpdate() {
        transform.position = player.transform.position + offset;
    }
}