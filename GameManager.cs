using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Sprite> Sprites = new List<Sprite>();
    public GameObject TilePrefab;
    public int GridDimension = 8;
    public float Distance = 1.0f;
    private GameObject[,] Grid;
    bool matchFound = false;

    public Tile firstSelectedTile;
    public Tile SecondSelectedTile;
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void StartBt()
    {
        Grid = new GameObject[GridDimension, GridDimension];
        InitGrid();
    }

    //�׸��� �� 3�� �̻� ���� Ÿ���� �پ����� �ʵ��� ����
    void InitGrid()
    {
        Vector3 positionOffset = transform.position - new Vector3(GridDimension * Distance / 2.0f, GridDimension * Distance / 2.0f, 0);

        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++)
            {
                List<Sprite> possibleSprites = new List<Sprite>(Sprites);

                Sprite left1 = GetSpriteAt(column - 1, row);
                Sprite left2 = GetSpriteAt(column - 2, row);
                if (left2 != null && left1 == left2)
                {
                    possibleSprites.Remove(left1);
                }

                Sprite down1 = GetSpriteAt(column, row - 1);
                Sprite down2 = GetSpriteAt(column, row - 2);
                if (down2 != null && down1 == down2)
                {
                    possibleSprites.Remove(down1);
                }

                GameObject newTile = Instantiate(TilePrefab);
                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();
                renderer.sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];
                newTile.transform.parent = transform;
                newTile.transform.position = new Vector3(column * Distance, row * Distance, 0) + positionOffset;

                Grid[column, row] = newTile;
            }
        }
    }
    Sprite GetSpriteAt(int column, int row)
    {
        if (column < 0 || column >= GridDimension
            || row < 0 || row >= GridDimension)
            return null;
        GameObject tile = Grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer.sprite;
    }

    //Ÿ�� ��ȯ �޼���
    public void Exchange()
    {
        if (firstSelectedTile != null && SecondSelectedTile != null)
        {
            // ��ȯ�� Ÿ���� �ε���
            Vector2Int firstTileIndex = GetTileIndex(firstSelectedTile);
            Vector2Int secondTileIndex = GetTileIndex(SecondSelectedTile);

            // ��ȯ�� Ÿ���� ��ġ
            Vector3 firstTilePosition = firstSelectedTile.transform.position;
            Vector3 secondTilePosition = SecondSelectedTile.transform.position;

            // Ÿ�� ��ġ ��ȯ
            StartCoroutine(AnimateExchange(firstSelectedTile.transform, secondTilePosition));
            StartCoroutine(AnimateExchange(SecondSelectedTile.transform, firstTilePosition));

            // Ÿ�� ���� ��ȯ
            GameObject temp = Grid[firstTileIndex.x, firstTileIndex.y];
            Grid[firstTileIndex.x, firstTileIndex.y] = Grid[secondTileIndex.x, secondTileIndex.y];
            Grid[secondTileIndex.x, secondTileIndex.y] = temp;

            StartCoroutine(DelayedDebugMessage());
        }
    }

    Vector2Int GetTileIndex(Tile tile)
    {
        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++)
            {
                if (Grid[column, row] == tile.gameObject)
                {
                    return new Vector2Int(column, row);
                }
            }
        }
        return Vector2Int.zero;
    }

    //Ÿ�� ��ȯ �ִϸ��̼�
    private IEnumerator AnimateExchange(Transform tileTransform, Vector3 targetPosition)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        Vector3 initialPosition = tileTransform.position;

        while (elapsedTime < duration)
        {
            tileTransform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tileTransform.position = targetPosition;
    }
    private IEnumerator DelayedDebugMessage()
    {

        yield return new WaitForSeconds(2f);
        CheckForMatches();
    }

    //�׸��� �� ���� Ÿ���� 3���̻� �ִ��� �˻��ϴ� �޼���
    public void CheckForMatches()
    {
        List<GameObject> matchedTiles = new List<GameObject>();

        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension - 2; column++)
            {
                if (Grid[column, row].GetComponent<Tile>().Sprite == Grid[column + 1, row].GetComponent<Tile>().Sprite && Grid[column, row].GetComponent<Tile>().Sprite == Grid[column + 2, row].GetComponent<Tile>().Sprite)
                {
                    int matchLength = 3;
                    matchFound = true;

                    for (int i = column + 3; i < GridDimension; i++)
                    {
                        if (Grid[column, row].GetComponent<Tile>().Sprite == Grid[i, row].GetComponent<Tile>().Sprite)
                            matchLength++;
                        else
                            break;
                    }

                    Debug.Log($"{row + 1}��° �࿡�� {matchLength}���� �پ��ִ� ���� ��ġ�� �߰ߵǾ����ϴ�.");

                    for (int i = 0; i < matchLength; i++)
                    {
                        matchedTiles.Add(Grid[column + i, row]);
                    }

                    column += matchLength - 1;
                }
            }
        }

        for (int column = 0; column < GridDimension; column++)
        {
            for (int row = 0; row < GridDimension - 2; row++)
            {
                if (Grid[column, row].GetComponent<Tile>().Sprite == Grid[column, row + 1].GetComponent<Tile>().Sprite && Grid[column, row].GetComponent<Tile>().Sprite == Grid[column, row + 2].GetComponent<Tile>().Sprite)
                {
                    int matchLength = 3;
                    matchFound = true;

                    for (int i = row + 3; i < GridDimension; i++)
                    {
                        if (Grid[column, row].GetComponent<Tile>().Sprite == Grid[column, i].GetComponent<Tile>().Sprite)
                            matchLength++;
                        else
                            break;
                    }

                    Debug.Log($"{column + 1}��° ������ {matchLength}���� �پ��ִ� ���� ��ġ�� �߰ߵǾ����ϴ�.");

                    for (int i = 0; i < matchLength; i++)
                    {
                        matchedTiles.Add(Grid[column, row + i]);
                    }

                    row += matchLength - 1;
                }
            }
        }

        // ��ġ�� ���� ���
        if (!matchFound)
        {
            Debug.Log("��ġ ����");
            Originally();
        }
        else
        {
            // ��ġ�� Ÿ�� ����
            foreach (GameObject tile in matchedTiles)
            {
                Destroy(tile);
                for (int row = 0; row < GridDimension; row++)
                {
                    for (int column = 0; column < GridDimension; column++)
                    {
                        if (Grid[column, row] == tile)
                        {
                            Grid[column, row] = null;
                            break;
                        }
                    }
                }
            }
            FillEmptyTiles();

        }

    }
    //��ȯ �� ��ġ�� ������ �ٽ� �ǵ��� ���� �޼���
    public void Originally()
    {
        Vector2Int firstTileIndex = GetTileIndex(firstSelectedTile);
        Vector2Int secondTileIndex = GetTileIndex(SecondSelectedTile);

        Vector3 firstTilePosition = firstSelectedTile.transform.position;
        Vector3 secondTilePosition = SecondSelectedTile.transform.position;

        StartCoroutine(AnimateExchange(firstSelectedTile.transform, secondTilePosition));
        StartCoroutine(AnimateExchange(SecondSelectedTile.transform, firstTilePosition));

        GameObject temp = Grid[firstTileIndex.x, firstTileIndex.y];
        Grid[firstTileIndex.x, firstTileIndex.y] = Grid[secondTileIndex.x, secondTileIndex.y];
        Grid[secondTileIndex.x, secondTileIndex.y] = temp;

        firstSelectedTile = null;
        SecondSelectedTile = null;

        return;
    }

    //������ ������ �� �� ��ĭ�� Ȯ���ϴ� �ý���
    public void FillEmptyTiles()
    {
        for (int column = 0; column < GridDimension; column++)
        {
            int emptyTileCount = 0;

            for (int row = 0; row < GridDimension; row++)
            {
                if (Grid[column, row] == null)
                {
                    emptyTileCount++;
                }
                else if (emptyTileCount > 0)
                {
                    StartCoroutine(MoveTileDown(Grid[column, row], emptyTileCount));

                    Grid[column, row - emptyTileCount] = Grid[column, row];
                    Grid[column, row] = null;

                }

            }

            if (emptyTileCount > 0)
            {
                Debug.Log($"{column + 1}�� ���� {emptyTileCount}���� �� Ÿ���� �ֽ��ϴ�.");

                for (int i = 0; i < emptyTileCount; i++)
                {
                    GameObject newTile = Instantiate(TilePrefab);
                    SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();
                    renderer.sprite = Sprites[Random.Range(0, Sprites.Count)];
                    newTile.transform.parent = transform;

                    // ���ο� Ÿ���� ���� ��ġ���� �׸��� ���̸�ŭ ���� ����
                    newTile.transform.position = new Vector3(column * Distance, (GridDimension + i) * Distance, 0) - new Vector3(GridDimension * Distance / 2.0f, GridDimension * Distance / 2.0f, 0);

                    // �������� �ִϸ��̼� ����
                    StartCoroutine(MoveTileDown(newTile, emptyTileCount));
                    Grid[column, GridDimension - 1 - i] = newTile;

                }
            }
        }
    }

    //Ÿ���� �������� �ִϸ��̼�
    private IEnumerator MoveTileDown(GameObject tileObject, int distance)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        Vector3 initialPosition = tileObject.transform.position;
        Vector3 targetPosition = initialPosition - new Vector3(0, distance * Distance, 0);

        while (elapsedTime < duration)
        {
            tileObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tileObject.transform.position = targetPosition;

        yield return new WaitForSeconds(2f);
        Debug.Log("Ȯ��");
        StartCoroutine(DelayedDebugMessage());
    }
}

