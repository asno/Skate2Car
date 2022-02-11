using System.Collections;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    private static CinematicManager _instance;

    [SerializeField]
    private Cinematic[] m_cinematics;

    private IEnumerator m_cinematicIterator;
    private Coroutine m_currentCinematicProcess;
    private Game m_game;

    public static CinematicManager Instance { get => _instance ??= FindObjectOfType<CinematicManager>(); }

    void Awake()
    {
        m_game = Game.Instance;
        Debug.Assert(m_game != null, "Unexpected null reference to m_game");
        Debug.Assert(m_cinematics != null, "Unexpected null reference to m_cinematics");
        Debug.Assert(m_cinematics.Length > 0, "Empty container m_cinematics");

        m_cinematicIterator = m_cinematics.GetEnumerator();
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        m_cinematicIterator.Reset();
    }

    public void StartNextCinematic()
    {
        if (!m_cinematicIterator.MoveNext())
            return;

        m_game.CharacterController.CanControl = false;
        m_game.Timer.Pause();

        if (m_currentCinematicProcess != null)
            StopCoroutine(m_currentCinematicProcess);

        m_currentCinematicProcess = StartCoroutine(PlayingCinematic());
    }

    private IEnumerator PlayingCinematic()
    {
        while (!m_game.CharacterController.CanMove())
            yield return null;

        Cinematic cinematic = m_cinematicIterator.Current as Cinematic;
        cinematic.Play();
        while(cinematic.IsPlaying)
            yield return null;

        ResumeGame();
    }

    private void ResumeGame()
    {
        m_game.CharacterController.CanControl = true;
        m_game.Timer.Resume();
    }
}
