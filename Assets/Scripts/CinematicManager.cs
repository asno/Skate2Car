using System;
using System.Linq;
using System.Collections;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    private static CinematicManager _instance;

    [SerializeField]
    private Cinematic[] m_cinematics;
    [SerializeField]
    private CharacterController2D m_characterController;
    [SerializeField]
    private Transform[] m_characters;

    private Vector3[] m_initialPositions;
    private IEnumerator m_currentCinematic;
    private IEnumerator m_currentCharacter;
    private Coroutine m_currentCinematicProcess;
    private Game m_game;

    public static CinematicManager Instance { get => _instance; }

    void Awake()
    {
        _instance = this;

        Debug.Assert(m_cinematics != null, "Unexpected null reference to m_cinematics");
        Debug.Assert(m_cinematics.Length > 0, "Empty container m_cinematics");
        Debug.Assert(m_characters != null, "Unexpected null reference to m_characters");
        Debug.Assert(m_characters.Length > 0, "Empty container m_characters");
    }

    void Start()
    {
        m_game = Game.Instance;
        Debug.Assert(m_game != null, "Unexpected null reference to m_game");
        m_currentCinematic = m_cinematics.GetEnumerator();
        m_currentCharacter = m_characters.GetEnumerator();
        m_currentCharacter.MoveNext();
        m_initialPositions = m_characters.Select(a => a.position).ToArray();
    }

    public void Reset()
    {
        m_currentCinematic.Reset();
        m_currentCharacter.Reset();

        for (int i = 0; i < m_initialPositions.Length; i++)
        {
            m_characters[i].position = m_initialPositions[i];
            m_characters[i].gameObject.SetActive(true);
        }
    }

    public void StartNextCinematic()
    {
        if (!m_currentCinematic.MoveNext())
            return;

        m_characterController.CanControl = false;
        m_game.IsTimerPaused = true;

        if (m_currentCinematicProcess != null)
            StopCoroutine(m_currentCinematicProcess);

        m_currentCinematicProcess = StartCoroutine(PlayingCinematic());
    }

    private IEnumerator PlayingCinematic()
    {
        while (!m_characterController.CanMove())
            yield return null;

        if(m_currentCharacter.MoveNext())
        {
            Transform character = m_currentCharacter.Current as Transform;
            Vector3 position = character.position;
            position.y = m_characterController.transform.position.y;
            character.position = position;
        }
        Cinematic cinematic = m_currentCinematic.Current as Cinematic;
        cinematic.Play();
        while(cinematic.IsPlaying)
            yield return null;

        ResumeGame();
    }

    private void ResumeGame()
    {
        Transform character = m_currentCharacter.Current as Transform;
        character.gameObject.SetActive(false);

        m_characterController.SwitchToNextState();
        m_characterController.CanControl = true;
        m_game.IsTimerPaused = false;
    }
}
