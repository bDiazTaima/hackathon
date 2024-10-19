using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Oculus.Voice;
using System.Reflection;
using Meta.WitAi.CallbackHandlers;
using UnityEngine.UI;
using System.Collections;

public class VoiceManager : MonoBehaviour
{
    [Header("Wit Configuration")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;
    [SerializeField] private WitResponseMatcher responseMatcher;
    [SerializeField] private TextMeshProUGUI transcriptionText;
    [SerializeField] private Image statusImage;

    [Header("Voice Events")]
    [SerializeField] private UnityEvent wakeWordDetected;
    [SerializeField] private UnityEvent<string> completeTranscription;

    [SerializeField] private bool _voiceCommandReady;
    private VoiceStatus _status = VoiceStatus.ACTIVE;
    /*
     * 
     */
    private void Awake()
    {
        appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(ReactiveVoice);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscription);

        var eventField = typeof(WitResponseMatcher).GetField("onMultiValueEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        if (eventField != null && eventField.GetValue(responseMatcher) is MultiValueEvent onMultiValueEvent)
        {
            onMultiValueEvent.AddListener(WakeWordDetected);
        }
        appVoiceExperience.Activate();
        SetStatus(VoiceStatus.ACTIVE);
    }

    private void OnDestroy()
    {
        appVoiceExperience.VoiceEvents.OnRequestCompleted.RemoveListener(ReactiveVoice);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);
        appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnFullTranscription);

        var eventField = typeof(WitResponseMatcher).GetField("onMultiValueEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        if (eventField != null && eventField.GetValue(responseMatcher) is MultiValueEvent onMultiValueEvent)
        {
            onMultiValueEvent.RemoveListener(WakeWordDetected);
        }
    }

    private void ReactiveVoice() => appVoiceExperience.Activate();

    private void WakeWordDetected(string[] args)
    {
        Debug.Log("WakeWordDetected");
        _voiceCommandReady = true;
        wakeWordDetected?.Invoke();
        SetStatus(VoiceStatus.LISTENTING);

    }

    private void OnPartialTranscription(string transcription)
    {
        Debug.Log("Partial Analyzing");
        if (!_voiceCommandReady) return;
        transcriptionText.text = transcription;
        SetStatus(VoiceStatus.ANALYZING);
    }

    private void OnFullTranscription(string transcription)
    {
        if (!_voiceCommandReady) return;
        completeTranscription?.Invoke(transcription);
        SetStatus(VoiceStatus.ANALYZING);
        Debug.Log("Full Analyzing");
        StartCoroutine(ListentingEnumerator());
    }

    private IEnumerator ListentingEnumerator()
    {
        _voiceCommandReady = false;
        yield return new WaitForSeconds(5.0f);
        _voiceCommandReady = true;
        SetStatus(VoiceStatus.ACTIVE);

    }

    private void SetStatus(VoiceStatus new_status)
    {
        _status = new_status;
        switch(_status)
        {
            case VoiceStatus.ACTIVE:
                statusImage.color = Color.green;
                break;
            case VoiceStatus.ANALYZING:
                statusImage.color = Color.yellow;
                break;
            case VoiceStatus.DEACTIVE:
                statusImage.color = Color.red;
                break;
            case VoiceStatus.LISTENTING:
                statusImage.color = Color.blue;
                break;
        }
    }
}

public enum VoiceStatus { ACTIVE, LISTENTING, ANALYZING, DEACTIVE };