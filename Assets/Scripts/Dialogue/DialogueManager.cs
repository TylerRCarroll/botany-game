using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private string characterName = "";
    [SerializeField]
    private AudioClip defaultSentenceAudio;


    private int currentSentence;
    private float coolDownTimer;
    private bool dialogueIsOn = false;
    private DialogueTrigger dialogueTrigger;

    public enum TriggerState
    {
        Collision,
        Input
    }

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private AudioSource audioSource;

    [Header("Events")]
    public UnityEvent startDialogueEvent;
    public UnityEvent nextSentenceDialogueEvent;
    public UnityEvent endDialogueEvent;

    [Header("Dialogue")]
    [SerializeField] private TriggerState triggerState;
    [SerializeField] private List<NPC_Sentence> sentences = new List<NPC_Sentence>();

    private void OnEnable()
    {
        playerInput.actions["Interact"].performed += _ => OnInteract();
        playerInput.enabled = true;
    }

    private void Update()
    {
        //Timer
        if(coolDownTimer > 0f)
        {
            coolDownTimer -= Time.deltaTime;
        }
    }

    private void OnInteract()
    {
        //Start dialogue by input
        if (dialogueTrigger != null && !dialogueIsOn)
        {
            //Trigger event inside DialogueTrigger component
            if (dialogueTrigger != null)
            {
                dialogueTrigger.startDialogueEvent.Invoke();
            }

            startDialogueEvent.Invoke();

            //If component found start dialogue
            DialogueUIController.instance.StartDialogue(this);

            //Hide interaction UI
            DialogueUIController.instance.ShowInteractionUI(false);

            dialogueIsOn = true;
        }
    }

    //Start dialogue by trigger
    private void OnTriggerEnter(Collider other)
    {
        if (triggerState == TriggerState.Collision && !dialogueIsOn)
        {
            //Try to find the "DialogueTrigger" component in the crashing collider
            if (gameObject.TryGetComponent<DialogueTrigger>(out DialogueTrigger _trigger))
            {
                Debug.Log("Found Dialog!");
                //Trigger event inside DialogueTrigger component and store refenrece
                dialogueTrigger = _trigger;
                dialogueTrigger.startDialogueEvent.Invoke();

                startDialogueEvent.Invoke();

                //If component found start dialogue
                DialogueUIController.instance.StartDialogue(this);

                dialogueIsOn = true;
            }
        }
    }

    //Start dialogue by pressing DialogueUI action input
    private void OnTriggerStay(Collider other)
    {
        if (dialogueTrigger != null)
            return;

        if (triggerState == TriggerState.Input && dialogueTrigger == null)
        {
            //Try to find the "DialogueTrigger" component in the crashing collider
            if (gameObject.TryGetComponent<DialogueTrigger>(out DialogueTrigger _trigger))
            {
                //Show interaction UI
                DialogueUIController.instance.ShowInteractionUI(true);

                //Store refenrece
                dialogueTrigger = _trigger;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Try to find the "DialogueTrigger" component from the exiting collider
        if (gameObject.TryGetComponent<DialogueTrigger>(out DialogueTrigger _trigger))
        {
            //Hide interaction UI
            DialogueUIController.instance.ShowInteractionUI(false);

            //Stop dialogue
            StopDialogue();
        }
    }

    public void StartDialogue()
    {
        //Start event
        if(dialogueTrigger != null)
        {
            dialogueTrigger.startDialogueEvent.Invoke();
        }

        //Reset sentence index
        currentSentence = 0;

        //Show first sentence in dialogue UI
        ShowCurrentSentence();

        //Play dialogue sound
        if(sentences[currentSentence].sentenceSound != null)
        { 
            PlaySound(sentences[currentSentence].sentenceSound);
        }
        else
        { 
            PlaySound(defaultSentenceAudio);    
        }

        //Cooldown timer
        coolDownTimer = sentences[currentSentence].skipDelayTime;
    }

    public void NextSentence(out bool lastSentence)
    {
        //The next sentence cannot be changed immediately after starting
        if (coolDownTimer > 0f)
        {
            lastSentence = false;
            return;
        }

        //Add one to sentence index
        currentSentence++;

        //Next sentence event
        if (dialogueTrigger != null)
        {
            dialogueTrigger.nextSentenceDialogueEvent.Invoke();
        }

        nextSentenceDialogueEvent.Invoke();

        //If last sentence stop dialogue and return
        if (currentSentence > sentences.Count - 1)
        {
            StopDialogue();

            lastSentence = true;

            endDialogueEvent.Invoke();

            return;
        }

        //If not last sentence continue...
        lastSentence = false;

        //Play dialogue sound
        if (sentences[currentSentence].sentenceSound != null)
        {
            PlaySound(sentences[currentSentence].sentenceSound);
        }
        else
        {
            PlaySound(defaultSentenceAudio);
        }

        //Show next sentence in dialogue UI
        ShowCurrentSentence();

        //Cooldown timer
        coolDownTimer = sentences[currentSentence].skipDelayTime;
    }

    public void StopDialogue()
    {
        //Stop dialogue event
        if (dialogueTrigger != null)
        {
            dialogueTrigger.endDialogueEvent.Invoke();
        }

        //Hide dialogue UI
        DialogueUIController.instance.ClearText();

        //Stop audiosource so that the speaker's voice does not play in the background
        if(audioSource != null)
        {
            audioSource.Stop();
        }

        //Remove trigger refence
        dialogueIsOn = false;
        dialogueTrigger = null;
    }

    private void PlaySound(AudioClip _audioClip)
    {
        //Play the sound only if it exists
        if (_audioClip == null || audioSource == null)
            return;

        //Stop the audioSource so that the new sentence does not overlap with the old one
        audioSource.Stop();

        //Play sentence sound
        audioSource.PlayOneShot(_audioClip);
    }

    private void ShowCurrentSentence()
    {

        //Show sentence on the screen
        DialogueUIController.instance.ShowSentence(characterName, sentences[currentSentence].sentence);

        //Invoke sentence event
        sentences[currentSentence].sentenceEvent.Invoke();
    }

    public int CurrentSentenceLength()
    {
        if(sentences.Count <= 0)
            return 0;

        return sentences[currentSentence].sentence.Length;
    }
}

[System.Serializable]
public class NPC_Sentence
{
    [Header("------------------------------------------------------------")]

    [TextArea(3, 10)]
    public string sentence;

    public float skipDelayTime = 0.5f;

    public AudioClip sentenceSound;

    public UnityEvent sentenceEvent;
}