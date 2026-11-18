using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Timeline;

public class Player : MonoBehaviour
{
    public float moveSpeed = 30f;
    private Tile currentTile;
    private Stage stage;
    private Animator animator;
    private bool isMoving;
    private Tile targetTilePrevious;
    private Tile lastTowardsTile;

    // private int lastIdx = 0;

    private void Start()
    {
        var mapObject = GameObject.FindWithTag("Map");
        stage = mapObject.GetComponent<Stage>();

        currentTile = stage.Map.startTile;
        transform.position = stage.GetTilePos(currentTile.id);
        stage.OnTileVisited(currentTile.id);

        animator = GetComponent<Animator>();
        animator.speed = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            stage.ResetStage();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            int targetTileId = stage.ScreenPosToTileId(Input.mousePosition);
            Tile targetTile = stage.Map.tiles[targetTileId];

            List<Tile> path = isMoving ?
            stage.Map.PathFinding(lastTowardsTile, targetTile) :
            stage.Map.PathFinding(currentTile, targetTile);

            if (!isMoving)
            {
                AdoptNewPath(path, targetTile);
            }

            if (targetTile != targetTilePrevious && path != null)
            {
                StopAllCoroutines();
                path.Insert(0, lastTowardsTile);
                Debug.Log($"lastStepped: {lastTowardsTile.id}, currenttile: {currentTile.id}");
                AdoptNewPath(path, targetTile);
            }
        }
    }

    private void AdoptNewPath(List<Tile> path, Tile target)
    {
        if (path != null && path.Count > 0)
        {
            targetTilePrevious = target;
            Debug.Log($"new target = {target.id}");
            StartCoroutine(CoMoveOnPath(path));
        }
        else return;
    }

    private IEnumerator CoMoveOnPath(List<Tile> path)
    {
        foreach (Tile tile in path)
        {
            lastTowardsTile = tile;
            yield return StartCoroutine(CoMoveOneStep(tile));
        }
    }

    private IEnumerator CoMoveOneStep(Tile next)
    {
        isMoving = true;
        animator.speed = 1f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = stage.GetTilePos(next.id);
        float moveDuration = Vector3.Distance(startPos, targetPos) / moveSpeed;
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        currentTile = next;
        transform.position = targetPos;
        animator.speed = 0f;
        stage.OnTileVisited(currentTile.id);
        isMoving = false;
    }
}
