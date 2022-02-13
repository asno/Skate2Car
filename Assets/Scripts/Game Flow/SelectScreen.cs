using UnityEngine;

public class SelectScreen : Screen
{
    [SerializeField]
    private Skateboard[] m_skateboards;
    [SerializeField]
    private SpriteRenderer m_cursor;
    [SerializeField]
    private Row<Transform>[] m_selectGridPosition;
    [SerializeField]
    private Row<SkateColor>[] m_selectColorEnum;
    private int m_currentX;
    private int m_currentY;
    private int m_dirX;
    private int m_dirY;
    private bool m_toBypass;
    private int m_numberOfRows;

    private Timer m_timer;

    public override void Initialize()
    {
        base.Initialize();
        m_currentX = 0;
        m_currentY = 0;
        m_dirX = 0;
        m_dirY = 0;
        m_numberOfRows = m_selectGridPosition.Length;
        m_timer = GetComponent<Timer>();
        Deactivate();
    }

    protected override void Deactivate()
    {
        m_timer.StopAndUnbind();
        gameObject.SetActive(false);
    }

    public override void Begin()
    {
        m_isSkipped = false;
        gameObject.SetActive(true);
        m_timer.RegisterCallbackEveryTick(0.25f, PollGridDirection);
        m_timer.Restart();
    }

    protected override void Exit()
    {
        base.Exit();
        Deactivate();
    }

    public override void DoUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if ((x != 0 && m_dirX == 0) || (y != 0 && m_dirY == 0))
        {
            m_toBypass = false;
            PollGridDirection();
            m_timer.Resume();
        }
        else if (x == 0 && y == 0 && !m_timer.IsPaused)
        {
            m_dirX = m_dirY = 0;
            m_toBypass = true;
            m_timer.Pause();
        }
        if (!Input.GetButtonDown("Fire1"))
            return;
        if (m_selectColorEnum == null && m_currentY >= m_selectColorEnum.Length || m_selectColorEnum[m_currentY] == null)
            return;
        var row = m_selectColorEnum[m_currentY];
        if (row.m_column == null || m_currentX >= row.m_column.Length || row.m_column[m_currentX] == SkateColor.None)
            return;

        AudioManager.Instance.PlayColorSelect();
        m_skateboards[0].SetColor(row.m_column[m_currentX]);
        foreach (Skateboard skate in m_skateboards)
            skate.SetColor(m_selectColorEnum[m_currentY].m_column[m_currentX]);
        m_isSkipped = true;
        Exit();
    }

    private void PollGridDirection()
    {
        if (m_toBypass)
            return;

        m_dirX = (int)Input.GetAxisRaw("Horizontal");
        m_dirY = (int)Input.GetAxisRaw("Vertical");
        var row = GetNearestRow(m_dirY);
        var item = GetNearestColumn(m_dirX, row);
        m_cursor.transform.position = item.position;
    }

    private Row<Transform> GetNearestRow(int aDirection)
    {
        Row<Transform> row;
        do
        {
            m_currentY += aDirection;
            m_currentY = ((m_currentY + m_numberOfRows) % m_numberOfRows);
            row = m_selectGridPosition[m_currentY];
        } while (row == null);

        return row;
    }

    private Transform GetNearestColumn(int aDirection, Row<Transform> aRow)
    {
        int numberOfColumns = aRow.m_column.Length;
        Transform column;
        do
        {
            m_currentX += aDirection;
            m_currentX = ((m_currentX + numberOfColumns) % numberOfColumns);
            column = aRow.m_column[m_currentX];
        } while (column == null);

        return column;
    }
}

[System.Serializable]
public class Row<T>
{
    public T[] m_column;
}
