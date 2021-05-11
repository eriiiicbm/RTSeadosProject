using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FPOverwrite : FirstPersonController
{
   
      private bool isInteracting = false;
 //   private Idamagable dead;
    private bool isInmortal = false;
    private Animator _animator;
    public AudioClip onInventoryOpen;
    public AudioClip onInventoryChange;
    public AudioClip onDead;
    public AudioClip onDeadMusic;

    void Start()
    {
         //  dead = this;
        _animator = GetComponentInParent<Animator>();
      
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_FovKick.Setup(m_Camera);
        m_HeadBob.Setup(m_Camera, m_StepInterval);
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
        m_Jumping = false;
        m_MouseLook.Init(transform, m_Camera.transform);
//        Transform g=  Instantiate(inventory.GetCurrentItem().GetTransform(), transform.GetChild(0));
        //      g.parent = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenuBehaviour.isPaused && !isDead)
        {
            if (!isInteracting)
            {
                isInteracting = Input.GetKeyDown(KeyCode.I);

                if (m_CharacterController.enabled == false && isInteracting)
                {
                    m_CharacterController.enabled = true;
                }
            }


            UpdateStuff();
         }

        if (Input.GetKeyDown(KeyCode.O))
        {
            isInmortal = true;
        }
    }

   

   


    public override void PlayLandingSound()
    {
        SoundManager._instance.PlaySE(m_LandSound, 1);
        m_NextStep = m_StepCycle + .5f;
    }

 

    void FixedUpdate()
    {
        if (!PauseMenuBehaviour.isPaused && !isDead)
        {
            FixedUpdateStuff();
        }
    }

    public override void PlayJumpSound()
    {
        Debug.Log("esto va");
        SoundManager._instance.PlaySE(m_JumpSound, 1);
    }

    public override void PlayFootStepAudio()
    {
        if (!m_CharacterController.isGrounded)
        {
            return;
        }

        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        SoundManager._instance.PlaySE(m_FootstepSounds[n], 1);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_FootstepSounds[0];
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OntriggerStayActivado");
        if (isInteracting)
        {
         //   Iinteractuable interactuable = other.GetComponent<Iinteractuable>();
       //     interactuable?.onInteract(GetComponent<Collider>());
            Debug.Log("I pressed in stay");
            isInteracting = false;
        }
        else
        {
            Debug.Log("Not interactuable");
        }
    }

    private void OnTriggerExit(Collider other)
    {
       
            //Iinteractuable interactuable = other.GetComponent<Iinteractuable>();
           // interactuable?.onStopInteract(GetComponent<Collider>());
            
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Esto va one");
    //   Iinteractuable interactuable = other.GetComponent<Iinteractuable>();
        if (isInteracting)
        {
        //    interactuable?.onInteract(GetComponent<Collider>());
            isInteracting = false;
            Debug.Log("I pressed in enter");
        }

        if (other.tag.Equals("Enemy") && !isInmortal)
        {
            Debug.Log("Esto va oneTrEnemy");

            StartCoroutine(TakeDamage());
        }
    }

    bool isDead = false;

    public IEnumerator TakeDamage()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<CharacterController>().enabled = false;
        Debug.Log("me muero");
        isDead = true;
        SoundManager._instance.PlaySEIfNotPlaying(onDead, 0.2f);
  
        _animator.Play("Dead");
        
        GameManager._instance.UnlockAchievement("Auch");
        SoundManager._instance.PlayBGM(onDeadMusic);
        yield return new WaitForSeconds(4);
        PauseMenuBehaviour._instance.ForcePause();
        PauseMenuBehaviour.isPaused = true;
        Cursor.visible=true;
         GameManager._instance.ChangeLevel(0);
    }
 

    private bool showingByCoroutine = false;

    public IEnumerator ShowInventoryXSeconds()
    {
        showingByCoroutine = true;
      //  uiInventory.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
     //   uiInventory.gameObject.SetActive(false);
        showingByCoroutine = false;
    }

   
}