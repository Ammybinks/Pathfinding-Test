using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {


    public float speed = 20;

    Transform target;
    Vector2 oldTargetPosition;
	Vector2[] path;
	int targetIndex;
    bool findingPath;

	void Start() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        oldTargetPosition = target.position;

		PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
        findingPath = true;
	}

    void Update()
    {
        if(oldTargetPosition != (Vector2)target.position && !findingPath)
        {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            findingPath = true;
        }
    }

	public void OnPathFound(Vector2[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
			path = newPath;
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
        }

        findingPath = false;
    }

	IEnumerator FollowPath() {
		Vector3 currentWaypoint = path[0];
		while (true) {
			if (transform.position == currentWaypoint) {
				targetIndex ++;
				if (targetIndex >= path.Length) {
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

			transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);
			yield return null;

		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}
}
