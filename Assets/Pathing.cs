using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathing{

	public static List<Vector3> AStar(Vector3 start, Vector3 goal){
		// The set of nodes already evaluated
		List<Vector3> closedSet = new List<Vector3>();

		// The set of currently discovered nodes that are not evaluated yet.
		// Initially, only the start node is known.
		List<Vector3> openSet = new List<Vector3>() {start};

		// For each node, which node it can most efficiently be reached from.
		// If a node can be reached from many nodes, cameFrom will eventually contain the
		// most efficient previous step.
		Dictionary<Vector3,Vector3> cameFrom = new Dictionary<Vector3,Vector3>();

		// For each node, the cost of getting from the start node to that node.
		Dictionary<Vector3,float> gScore = new Dictionary<Vector3,float>(){};  // map with default value of Infinity

		// For each node, the total cost of getting from the start node to the goal
		// by passing by that node. That value is partly known, partly heuristic.
		Dictionary<Vector3,float> fScore = new Dictionary<Vector3,float>(){};  //map with default value of Infinity

		foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) {
			if (tile.transform.position == new Vector3(start.x,start.y - Constants.UNIT_TILE_DIFF, start.z)) {	
				gScore.Add (start, 0);								// The cost of going from start to start is zero.
				fScore.Add(start,Vector3.Distance(start, goal));	// For the first node, fscore value is completely heuristic.
			} else {
				gScore.Add (new Vector3(tile.transform.position.x,tile.transform.position.y + Constants.UNIT_TILE_DIFF, tile.transform.position.z), 1000f);
				fScore.Add (new Vector3(tile.transform.position.x,tile.transform.position.y + Constants.UNIT_TILE_DIFF, tile.transform.position.z), 1000f);
			}
		}


		while(openSet.Count != 0){
			float min = 10000f;
			Vector3 current = new Vector3();
			foreach (var pair in fScore){
				if(openSet.Contains(pair.Key)){
					if (pair.Value < min) {
						min = pair.Value;
						current = pair.Key; //the node in openSet having the lowest fScore[] value
					}
				}
			}

			if (current == goal){
				return reconstructPath (cameFrom, current);
			}

			openSet.Remove (current);
			closedSet.Add (current);

			foreach (Vector3 neighbour in neighbours(current)){
				if (closedSet.Contains(neighbour)){
					continue;		// Ignore the neighbor which is already evaluated.
				}

				if (!openSet.Contains(neighbour)){	// Discover a new node
					openSet.Add(neighbour);
				}

				// The distance from start to a neighbour
				float tentative_gScore = gScore[current] + Constants.TILE_GAP;

				if (tentative_gScore >= gScore [neighbour]) {
					continue;		// This is not a better path.
				}

				// This path is the best until now.
				cameFrom.Add(neighbour, current);
				gScore[neighbour] = tentative_gScore;
				fScore[neighbour] = gScore[neighbour] + Vector3.Distance(neighbour, goal);
			}
		}

		return null;
	}

	public static List<Vector3> neighbours(Vector3 centre){
		List<Vector3> near = new List<Vector3>();
		bool right = false, up = false, left = false, down = false;

		// First check there's no neighbouring units
		foreach(GameObject g in GameObject.FindGameObjectsWithTag("Unit")){

			if (g.transform.position == new Vector3 (centre.x + Constants.TILE_GAP, centre.y, centre.z)) {
				right = true;
			}

			if (g.transform.position == new Vector3 (centre.x - Constants.TILE_GAP, centre.y, centre.z)) {
				left = true;
			}

			if (g.transform.position == new Vector3 (centre.x, centre.y, centre.z + Constants.TILE_GAP)) {
				up = true;
			}

			if (g.transform.position == new Vector3 (centre.x, centre.y, centre.z - Constants.TILE_GAP)) {
				down = true;
			}

		}

		Vector3 airPos = new Vector3 ();

		//Then select non occupied close tiles as neighbours
		foreach(GameObject current in GameObject.FindGameObjectsWithTag("Tile")){

			if(current.transform.position == new Vector3(centre.x + Constants.TILE_GAP , centre.y - Constants.UNIT_TILE_DIFF , centre.z) && right == false){ //	X+ check
				airPos = current.transform.position;
				airPos.y += Constants.UNIT_TILE_DIFF;
				near.Add(airPos);
			}

			if(current.transform.position == new Vector3(centre.x - Constants.TILE_GAP , centre.y - Constants.UNIT_TILE_DIFF , centre.z) && left == false){ //	X- check
				airPos = current.transform.position;
				airPos.y += Constants.UNIT_TILE_DIFF;
				near.Add(airPos);
			}

			if(current.transform.position == new Vector3(centre.x , centre.y - Constants.UNIT_TILE_DIFF , centre.z + Constants.TILE_GAP) && up == false){ //	Z+ check
				airPos = current.transform.position;
				airPos.y += Constants.UNIT_TILE_DIFF;
				near.Add(airPos);
			}

			if(current.transform.position == new Vector3(centre.x, centre.y - Constants.UNIT_TILE_DIFF , centre.z - Constants.TILE_GAP) && down == false){ //	Z- check
				airPos = current.transform.position;
				airPos.y += Constants.UNIT_TILE_DIFF;
				near.Add(airPos);
			}

		}

		return near;
	}


	public static List<Vector3> reconstructPath (Dictionary<Vector3,Vector3> cameFrom , Vector3 end){
		List<Vector3> path = new List <Vector3>();

		path.Add (end);

		while (cameFrom.ContainsKey(end)){
			end = cameFrom[end];
			path.Add(end);
		}

		path.Reverse();
		return path;
	}


    /**
     *  Returns a list with vectors from the tiles that have a unit on top, that are neighbouring centre
     */ 
    public static List<Vector3> NeighbouringUnits(Vector3 centre) {
        List<Vector3> unit = new List<Vector3>();

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Unit")) {

            if (g.transform.position == new Vector3(centre.x + Constants.TILE_GAP, centre.y, centre.z)) {
                unit.Add(new Vector3(g.transform.position.x, g.transform.position.y - Constants.UNIT_TILE_DIFF, g.transform.position.z));
            }

            if (g.transform.position == new Vector3(centre.x - Constants.TILE_GAP, centre.y, centre.z)) {
                unit.Add(new Vector3(g.transform.position.x, g.transform.position.y - Constants.UNIT_TILE_DIFF, g.transform.position.z));
            }

            if (g.transform.position == new Vector3(centre.x, centre.y, centre.z + Constants.TILE_GAP)) {
                unit.Add(new Vector3(g.transform.position.x, g.transform.position.y - Constants.UNIT_TILE_DIFF, g.transform.position.z));
            }

            if (g.transform.position == new Vector3(centre.x, centre.y, centre.z - Constants.TILE_GAP)) {
                unit.Add(new Vector3(g.transform.position.x, g.transform.position.y - Constants.UNIT_TILE_DIFF, g.transform.position.z));
            }

        }

        return unit;
    }

    /**
     *  Returns a list with vectors from the empty spaces (no tiles) that are neighbouring centre
     */
    public static List<Vector3> NeighbouringSpaces(Vector3 centre)
    {
        List<Vector3> spaces = new List<Vector3>();
        bool emptyRight = true, emptyLeft = true, emptyForward = true, emptyBackward = true;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Tile")) {
            if (g.transform.position == new Vector3(centre.x + Constants.TILE_GAP, centre.y - Constants.UNIT_TILE_DIFF, centre.z)) {
                emptyRight = false;
            }

            if (g.transform.position == new Vector3(centre.x - Constants.TILE_GAP, centre.y - Constants.UNIT_TILE_DIFF, centre.z)) {
                emptyLeft = false;
            }

            if (g.transform.position == new Vector3(centre.x, centre.y - Constants.UNIT_TILE_DIFF, centre.z + Constants.TILE_GAP)) {
                emptyForward = false;
            }

            if (g.transform.position == new Vector3(centre.x, centre.y - Constants.UNIT_TILE_DIFF, centre.z - Constants.TILE_GAP)) {
                emptyBackward = false;
            }
        }

        if (emptyRight) {
            spaces.Add(new Vector3(centre.x + Constants.TILE_GAP, centre.y - Constants.UNIT_TILE_DIFF, centre.z));
        }

        if (emptyLeft) {
            spaces.Add(new Vector3(centre.x - Constants.TILE_GAP, centre.y - Constants.UNIT_TILE_DIFF, centre.z));
        }

        if (emptyForward) {
            spaces.Add(new Vector3(centre.x, centre.y - Constants.UNIT_TILE_DIFF, centre.z + Constants.TILE_GAP));
        }

        if (emptyBackward) {
            spaces.Add(new Vector3(centre.x, centre.y - Constants.UNIT_TILE_DIFF, centre.z - Constants.TILE_GAP));
        }

        return spaces;
    }

}
