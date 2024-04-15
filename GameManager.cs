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

    //그리드 내 3개 이상 같은 타일이 붙어있지 않도록 생성
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

    //타일 교환 메서드
    public void Exchange()
    {
        if (firstSelectedTile != null && SecondSelectedTile != null)
        {
            // 교환할 타일의 인덱스
            Vector2Int firstTileIndex = GetTileIndex(firstSelectedTile);
            Vector2Int secondTileIndex = GetTileIndex(SecondSelectedTile);

            // 교환할 타일의 위치
            Vector3 firstTilePosition = firstSelectedTile.transform.position;
            Vector3 secondTilePosition = SecondSelectedTile.transform.position;

            // 타일 위치 교환
            StartCoroutine(AnimateExchange(firstSelectedTile.transform, secondTilePosition));
            StartCoroutine(AnimateExchange(SecondSelectedTile.transform, firstTilePosition));

            // 타일 정보 교환
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

    //타일 교환 애니메이션
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

    //그리드 내 같은 타일이 3개이상 있는지 검사하는 메서드
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

                    Debug.Log($"{row + 1}번째 행에서 {matchLength}개가 붙어있는 수평 매치가 발견되었습니다.");

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

                    Debug.Log($"{column + 1}번째 열에서 {matchLength}개가 붙어있는 수직 매치가 발견되었습니다.");

                    for (int i = 0; i < matchLength; i++)
                    {
                        matchedTiles.Add(Grid[column, row + i]);
                    }

                    row += matchLength - 1;
                }
            }
        }

        // 매치가 없는 경우
        if (!matchFound)
        {
            Debug.Log("매치 없음");
            Originally();
        }
        else
        {
            // 매치된 타일 삭제
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
    //교환 후 매치가 없으면 다시 되돌려 놓는 메서드
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

    //과일이 삭제가 된 뒤 빈칸을 확인하는 시스템
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
                Debug.Log($"{column + 1}번 열에 {emptyTileCount}개의 빈 타일이 있습니다.");

                for (int i = 0; i < emptyTileCount; i++)
                {
                    GameObject newTile = Instantiate(TilePrefab);
                    SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();
                    renderer.sprite = Sprites[Random.Range(0, Sprites.Count)];
                    newTile.transform.parent = transform;

                    // 새로운 타일을 현재 위치에서 그리드 높이만큼 위로 생성
                    newTile.transform.position = new Vector3(column * Distance, (GridDimension + i) * Distance, 0) - new Vector3(GridDimension * Distance / 2.0f, GridDimension * Distance / 2.0f, 0);

                    // 내려가는 애니메이션 시작
                    StartCoroutine(MoveTileDown(newTile, emptyTileCount));
                    Grid[column, GridDimension - 1 - i] = newTile;

                }
            }
        }
    }

    //타일이 내려오는 애니메이션
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
        Debug.Log("확인");
        StartCoroutine(DelayedDebugMessage());
    }
}

