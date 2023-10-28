using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{

    //A queue to hold pathfinding requests
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    //The current pathfinding request being processed
    PathRequest currentPathRequest;


    //Make the manager accessible from other scripts without creating an instance
    //static PathRequestManager instance;
    Pathfinding pathfinding;

    //Check if the manager is currently processing a pathfinding request.
    bool isProcessingPath;

    void Awake()
    {
        pathfinding = GameObject.Find("A*").GetComponent<Pathfinding>();
    }

    //It creates a new PathRequest object, enqueues it in the queue, and tries to process the next request.
    public void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        pathRequestQueue.Enqueue(newRequest);
        TryProcessNext();
    }

    //Process the next request
    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, this);
        }
    }

    //Called by the pathfinding, get if find path successfully
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }


    //encapsulate a pathfinding request with start and end points and a callback
    //It has a constructor to initialize its fields
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }

    }

}
