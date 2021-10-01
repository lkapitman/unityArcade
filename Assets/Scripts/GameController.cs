using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private CubePos _nowCube = new CubePos(0, 1, 0);
    private const float CubeChangePlaceSpeed = 0.5f;
    public GameObject[] canvasStartPage;
    
    public Transform cubeToPlace;
    private Rigidbody _allCubesRb;

    public Color[] bgColors;
    
    private bool _isLose, _fistCube;

    private float _camMoveToYPosition;
    
    public GameObject cubeToCreate, allCubes; 
    
    private readonly List<Vector3> _allCubesPositions = new List<Vector3>
    {
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1)
    };

    private int _prevCountMaxHorizontal;
    
    private Coroutine _showCubePlace;
    
    private void Start()
    {
        _allCubesRb = allCubes.GetComponent<Rigidbody>();
        
        _showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null && !EventSystem.current.IsPointerOverGameObject() && !EventSystem.current.IsPointerOverGameObject())
        {
            #if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
            {
                return;
            }
            #endif
            
            if (!_fistCube)
            {
                _fistCube = true;
                foreach (var obj in canvasStartPage)
                {
                    Destroy(obj);                    
                }
            }
            
            var newCube = Instantiate(cubeToCreate, cubeToPlace.position, Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            
            _nowCube.SetVector(cubeToPlace.position);
            _allCubesPositions.Add(_nowCube.GETVector());

            _allCubesRb.isKinematic = true;
            _allCubesRb.isKinematic = false;
            
            SpawnPositions();
            MoveCameraChangeBg();
        }

        if (_isLose || !(_allCubesRb.velocity.magnitude > 0.1f)) return;
        
        Destroy(cubeToPlace.gameObject);
        _isLose = true;
        StopCoroutine(_showCubePlace);
    }

    private IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();

            yield return new WaitForSeconds(CubeChangePlaceSpeed);
        }        
        // ReSharper disable once IteratorNeverReturns
    }

    private void SpawnPositions()
    {
        var positions = new List<Vector3>();
        if (IsPositionEmpty(new Vector3(_nowCube.X + 1, _nowCube.Y, _nowCube.Z)) && _nowCube.X +1 != cubeToPlace.position.x)
            positions.Add(new Vector3(_nowCube.X + 1, _nowCube.Y, _nowCube.Z));
        if (IsPositionEmpty(new Vector3(_nowCube.X - 1, _nowCube.Y, _nowCube.Z)) && _nowCube.X - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(_nowCube.X - 1, _nowCube.Y, _nowCube.Z));
        
        if (IsPositionEmpty(new Vector3(_nowCube.X, _nowCube.Y + 1, _nowCube.Z)) && _nowCube.Y + 1!= cubeToPlace.position.y)
            positions.Add(new Vector3(_nowCube.X, _nowCube.Y + 1, _nowCube.Z));
        
        if (IsPositionEmpty(new Vector3(_nowCube.X, _nowCube.Y, _nowCube.Z + 1))  && _nowCube.Z + 1!= cubeToPlace.position.z)
            positions.Add(new Vector3(_nowCube.X, _nowCube.Y, _nowCube.Z + 1));
        if (IsPositionEmpty(new Vector3(_nowCube.X, _nowCube.Y, _nowCube.Z - 1)) && _nowCube.Z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(_nowCube.X, _nowCube.Y, _nowCube.Z - 1));

        if (positions.Count > 1)
        {
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        }
        else if (positions.Count == 0)
        {
            _isLose = true;
        }
        else
        {
            cubeToPlace.position = positions[0];
        }
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        return _allCubesPositions.Any(pos => pos.x == targetPos.x || pos.y == targetPos.y || pos.z == targetPos.z);
    }

    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;

        foreach (var pos in _allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Convert.ToInt32(pos.x);
            
            if (Convert.ToInt32(pos.y) > maxY)
                maxY = Convert.ToInt32(pos.y);
            
            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);
        }
        
        var mainCamera = Camera.main.transform;
        _camMoveToYPosition = 5.9f + _nowCube.Y - 1f;
        
        mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, _camMoveToYPosition, mainCamera.localPosition.z);

        maxHor = maxX > maxZ ? maxX : maxZ;
        
        if (maxHor % 3 != 0 || _prevCountMaxHorizontal == maxHor) return;
        
        mainCamera.localPosition -= new Vector3(0, 0, 2f);
        _prevCountMaxHorizontal = maxHor;

        if (maxY >= 7)
        {
            Camera.main.backgroundColor = bgColors[2];
        }
        else if (maxY >= 5)
        {
            Camera.main.backgroundColor = bgColors[1];
        }
        else if (maxY >= 2)
        {
            Camera.main.backgroundColor = bgColors[0];
        }
        
    }
}

internal struct CubePos
{
    public int X;
    public int Y;
    public int Z;

    public CubePos(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Vector3 GETVector()
    {
        return new Vector3(X, Y, Z);
    }

    public void SetVector(Vector3 pos)
    {
        X = Convert.ToInt32(pos.x);
        Y = Convert.ToInt32(pos.y);
        Z = Convert.ToInt32(pos.z);
    }
    
}
