using System;
using System.Collections;
using UnityEngine;

public class Cinematic : MonoBehaviour
{
    [SerializeField]
    private AbstractSequence[] m_sequences;
    private bool m_isPlaying;

    public bool IsPlaying { get => m_isPlaying; }

    public void Play(Action aCallback = null)
    {
        StartCoroutine(PlaySequences(aCallback));
    }

    private IEnumerator PlaySequences(Action aCallback)
    {
        m_isPlaying = true;
        var iterator = m_sequences.GetEnumerator();
        bool isIterating = iterator.MoveNext();
        while(isIterating)
        {
            AbstractSequence currentSequence = iterator.Current as AbstractSequence;
            if (currentSequence.IsAsynchronous)
                yield return currentSequence.DoAction();
            else
                StartCoroutine(currentSequence.DoAction());
            isIterating = iterator.MoveNext();
        }
        m_isPlaying = false;

        if (aCallback != null)
            aCallback();
    }
}
