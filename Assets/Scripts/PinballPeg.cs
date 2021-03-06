﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PinballPeg : MonoBehaviour
{

    private Material rendererMaterial;
    public AudioSource audioSource;
    private float elapsedCollisionTime;

    [Inject]
    GameSession gameSession;

    // Use this for initialization
    void Start()
    {
        Physics.defaultContactOffset = 0.1F;
        rendererMaterial = transform.GetComponent<Renderer>().material;
        elapsedCollisionTime = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void pushBall(Collision col)
    {
        var directionOfMotion = col.transform.GetComponent<Rigidbody>().velocity.normalized;
        directionOfMotion = -directionOfMotion;
        if (directionOfMotion.magnitude < 0.05f)
        {
            directionOfMotion = transform.up.normalized;
        }
        col.transform.GetComponent<Rigidbody>().AddForce(directionOfMotion * GameSession.PEG_FORCE);
    }

    void OnCollisionEnter(Collision col)
    {
        elapsedCollisionTime = 0;

        pushBall(col);

        StartCoroutine("rainbowColors");
        audioSource.Play();

        gameSession.raiseScore(GameSession.PEG_SCORE_VALUE);

    }

    private void OnCollisionStay(Collision collision)
    {
        elapsedCollisionTime += Time.deltaTime;
        if (elapsedCollisionTime > 1.0f)
        {
            pushBall(collision);
        }
    }
    
    IEnumerator rainbowColors()
    {
        for (float f = 0.0f; f <= 3.0f; f += Time.deltaTime)
        {
            int tensPlace = ((int)(f * 10)) - ((int)(f));
            int mod = tensPlace % 3;
            Color nextColor = Color.green;
            switch (mod)
            {
                case 0:
                    nextColor = Color.red;
                    break;
                case 1:
                    nextColor = Color.blue;
                    break;
                case 2:
                    nextColor = Color.green;
                    break;
            }

            rendererMaterial.color = nextColor;
            yield return null;
        }

        rendererMaterial.color = new Color(0, 0.5f, 1.0f);
    }

}