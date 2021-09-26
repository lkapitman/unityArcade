using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private CubePos _nowCube = new CubePos(0, 1, 0);
    private const float CubeChangePlaceSpeed = 0.5f;
    public Transform cubeToPlace;
    private Rigidbody _allCubesRb;
    
    public GameObject cubeToCreate, allCubes; 
    
    private readonly List<Vector3> _allCubesPositions = new List<Vector3>
    {
        new Vector3(0, 0, 0),
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
    
    private void Start()
    {
        _allCubesRb = allCubes.GetComponent<Rigidbody>();
        
        StartCoroutine(ShowCubePlace());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            #if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
            {
                return;
            }
            #endif

            var newCube = Instantiate(cubeToCreate, cubeToPlace.position, Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            
            _nowCube.SetVector(cubeToPlace.position);
            _allCubesPositions.Add(_nowCube.GETVector());

            _allCubesRb.isKinematic = true;
            _allCubesRb.isKinematic = false;
            
            SpawnPositions();
            
        }        
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
        if (IsPositionEmpty(new Vector3(_nowCube.X, _nowCube.Y - 1, _nowCube.Z)) && _nowCube.Y - 1!= cubeToPlace.position.y)
            positions.Add(new Vector3(_nowCube.X, _nowCube.Y - 1, _nowCube.Z));
        
        if (IsPositionEmpty(new Vector3(_nowCube.X, _nowCube.Y, _nowCube.Z + 1))  && _nowCube.Z + 1!= cubeToPlace.position.z)
            positions.Add(new Vector3(_nowCube.X, _nowCube.Y, _nowCube.Z + 1));
        if (IsPositionEmpty(new Vector3(_nowCube.X, _nowCube.Y, _nowCube.Z - 1)) && _nowCube.Z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(_nowCube.X, _nowCube.Y, _nowCube.Z - 1));

        cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        
        
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        return _allCubesPositions.Any(pos => pos.x == targetPos.x || pos.y == targetPos.y || pos.z == targetPos.z);
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
