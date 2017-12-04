using System.Collections.Generic;
using UnityEngine;

public class PlayerShape : MonoBehaviour {

    private Vector3 target;
    private List<Vector3> targetPath;

    private bool interacted;

    private Vector3 originalPosition;
    private Vector3 interactTarget;
    private GameObject interactingShape;
    private GameObject interactedShape;

    private int movement;

    private bool hasMoved;
    private bool hasInteracted;

    // Use this for initialization
    void Start() {
        hasMoved = false;
        hasInteracted = false;
        targetPath = new List<Vector3>();
        if (this.name.Equals("Cube") || this.name.Equals("Plank") || this.name.Equals("Mini Cube")) {
            movement = 2;
        } else {
            movement = 3;
        }

        if(this.name.Equals("Mini Cube")) {
            hasInteracted = true;
        }
    }

    /**
     * 	On update, the figure unpipes the movements accumulated and executes them according to its animator and
     * 	temporal direction
     */
    void Update() {
        Vector3 tempTarget = new Vector3();
        if (this.transform.parent.name.Equals("Selected")) {

            // Movement of a shape
            if (targetPath.Count > 0) {
                tempTarget = targetPath[0];

                if (this.GetComponent<Animator>().GetBool("moving") && Vector3.Distance(transform.parent.position, tempTarget) > 0f) {
                    foreach (AnimatorControllerParameter parameter in this.GetComponent<Animator>().parameters) {
                        if (!parameter.name.Equals("moving")) {
                            this.GetComponent<Animator>().SetBool(parameter.name, false);
                        }
                    }

                    if (tempTarget.x > transform.parent.position.x && tempTarget.x - transform.parent.position.x > 0f) {
                        this.GetComponent<Animator>().SetBool("movingRight", true);
                        transform.parent.position = Vector3.MoveTowards(transform.parent.position, tempTarget, Constants.TILE_GAP * Time.deltaTime);
                    } else if (tempTarget.x < transform.parent.position.x && transform.parent.position.x - tempTarget.x > 0f) {
                        this.GetComponent<Animator>().SetBool("movingLeft", true);
                        transform.parent.position = Vector3.MoveTowards(transform.parent.position, tempTarget, Constants.TILE_GAP * Time.deltaTime);
                    } else if (tempTarget.z > transform.parent.position.z && tempTarget.z - transform.parent.position.z > 0f) {
                        this.GetComponent<Animator>().SetBool("movingForward", true);
                        transform.parent.position = Vector3.MoveTowards(transform.parent.position, tempTarget, Constants.TILE_GAP * Time.deltaTime);
                    } else if (tempTarget.z < transform.parent.position.z && transform.parent.position.z - tempTarget.z > 0f) {
                        this.GetComponent<Animator>().SetBool("movingBackward", true);
                        transform.parent.position = Vector3.MoveTowards(transform.parent.position, tempTarget, Constants.TILE_GAP * Time.deltaTime);
                    }

                } else {
                    if (transform.parent.position == target) {
                        this.GetComponent<Animator>().SetBool("moving", false);
                        GameObject.Find("Main Camera").GetComponent<UIManager>().SetButtons(hasMoved, hasInteracted,name);
                    }
                    targetPath.RemoveAt(0);
                }
            }

        }

        if (this.interacted) {
            switch (interactingShape.name) {
                case "Sphere":
                    if (transform.parent.position != this.interactTarget) {
                        transform.parent.position = Vector3.MoveTowards(transform.parent.position, interactTarget, 2f * Constants.TILE_GAP * Time.deltaTime);
                    } else {
                        this.SetInteracted(false);
                        this.SetInteracting(null);
                    }
                    break;
                case "Pyramid":
                    if (this.GetComponent<Animator>().GetBool("interactLeft") || this.GetComponent<Animator>().GetBool("interactRight") ||
                        this.GetComponent<Animator>().GetBool("interactForward") || this.GetComponent<Animator>().GetBool("interactBackward")) {
                        transform.parent.position = Vector3.MoveTowards(transform.parent.position, interactTarget, 2f * Constants.TILE_GAP * Time.deltaTime);
                        if (transform.parent.position == this.interactTarget) {
                            this.TriggerInteractedAnimation();
                        }
                    } else if (!this.GetComponent<Animator>().GetBool("landingReached") && transform.parent.position != this.interactTarget) {
                        transform.parent.position = Vector3.MoveTowards(transform.parent.position, interactTarget, 2f * Constants.TILE_GAP * Time.deltaTime);
                        if (transform.parent.position == this.interactTarget) {
                            this.GetComponent<Animator>().SetBool("landingReached", true);
                            if (interactTarget != originalPosition) {
                                interactTarget = new Vector3(interactTarget.x, interactTarget.y - 2f, interactTarget.z);
                            }
                        }
                    } else if (this.GetComponent<Animator>().GetBool("landingReached") && transform.parent.position != this.interactTarget) {
                        transform.parent.position = Vector3.MoveTowards(transform.parent.position, interactTarget, 2f * Constants.TILE_GAP * Time.deltaTime);
                    } else if (this.GetComponent<Animator>().GetBool("landingReached") && transform.parent.position == this.interactTarget) {
                        this.SetInteracted(false);
                        this.SetInteracting(null);
                    }
                    break;
            }
        }
    }

    /**
	 * 	Changes the colour of the shape when moving the mouse into it if not selected previously
	 */
    public void OnMouseOver() {
        if (!this.transform.parent.name.Equals("Selected")) {
            this.GetComponent<Renderer>().material.color = Constants.COLOR_SHAPE_OVER;
        }
    }

    /** 
	 * 	Changes the colour of the shape when moving the mouse away of it if not selected
	 */
    public void OnMouseExit() {
        if (!this.transform.parent.name.Equals("Selected")) {
            this.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    /**
	 * 	Changes the colour of the shape after selecting it, sets this as the selected shape and calls for the UI elements to be set accordingly
	 */
    public void OnMouseDown() {
        if (GameObject.Find("Selected") != null) {
            if (!GameObject.Find("Selected").GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                return;
            }
            GameObject.Find("Selected").transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Color.white;
            GameObject.Find("Selected").name = "Player";
        }

        this.transform.parent.name = "Selected";
        GameObject.Find("Main Camera").GetComponent<UIManager>().SetButtons(hasMoved, hasInteracted, name);
        this.GetComponent<Renderer>().material.color = Constants.COLOR_SHAPE_SELECTED;
    }

    /**
	 * 	Sets the necessary parameters to start the movement of the shape, along with its animation
	 * 	and pathfinding.
	 */
    public void moveAnimation(Vector3 targetTile) {
        this.GetComponent<Animator>().SetBool("moving", true);
        hasMoved = true;
        GameObject.Find("Main Camera").GetComponent<UIManager>().SetButtons(hasMoved, hasInteracted,name);

        target = new Vector3(targetTile.x, targetTile.y + Constants.UNIT_TILE_DIFF, targetTile.z);
        targetPath = Pathing.AStar(this.transform.parent.position, target);
    }

    public void InteractAnimation(Vector3 targetTile) {

        hasInteracted = true;

        Vector3 centre = new Vector3(targetTile.x, targetTile.y + Constants.UNIT_TILE_DIFF, targetTile.z);

        if (centre.x - transform.parent.position.x > 0) {
            GameObject.Find("Selected").GetComponentInChildren<Animator>().SetBool("interactRight", true);
        } else if (centre.x - transform.parent.position.x < 0) {
            GameObject.Find("Selected").GetComponentInChildren<Animator>().SetBool("interactLeft", true);
        } else if (centre.z - transform.parent.position.z > 0) {
            GameObject.Find("Selected").GetComponentInChildren<Animator>().SetBool("interactForward", true);
        } else if (centre.z - transform.parent.position.z < 0) {
            GameObject.Find("Selected").GetComponentInChildren<Animator>().SetBool("interactBackward", true);
        }

        // Special condition for pyramid shapes, as they perform their animations after updating first its height and interact with
        // themselves instead of other shapes
        if (this.name == "Pyramid") {
            GameObject.Find("Selected").GetComponentInChildren<Animator>().SetBool("landingReached", false);
            interactedShape = this.gameObject;
            originalPosition = this.transform.parent.position;
            interactTarget = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            interactedShape.GetComponentInChildren<PlayerShape>().SetInteracting(this.gameObject);
            interactedShape.GetComponentInChildren<PlayerShape>().SetInteracted(true);
        } else if(name != "Plank"){
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Unit")) {
                if (g.transform.position == centre) {
                    interactedShape = g;
                }
            }
        } else {
            interactTarget = targetTile;
        }

        GameObject.Find("Main Camera").GetComponent<UIManager>().SetButtons(hasMoved, hasInteracted,name);
    }

    public void TriggerInteractedAnimation() {
        bool left = false, right = false, forward = false, backward = false, occupied = false;

        foreach (AnimatorControllerParameter parameter in this.GetComponent<Animator>().parameters) {
            switch (parameter.name) {
                case "interactLeft":
                    if (this.GetComponent<Animator>().GetBool(parameter.name)) left = true;
                    this.GetComponent<Animator>().SetBool(parameter.name, false);
                    break;
                case "interactRight":
                    if (this.GetComponent<Animator>().GetBool(parameter.name)) right = true;
                    this.GetComponent<Animator>().SetBool(parameter.name, false);
                    break;
                case "interactForward":
                    if (this.GetComponent<Animator>().GetBool(parameter.name)) forward = true;
                    this.GetComponent<Animator>().SetBool(parameter.name, false);
                    break;
                case "interactBackward":
                    if (this.GetComponent<Animator>().GetBool(parameter.name)) backward = true;
                    this.GetComponent<Animator>().SetBool(parameter.name, false);
                    break;
            }
        }

        // This big switch for the interactions needs a big optimization and cleaning as there is too
        // much duplicate code that can be avoided
        switch (this.name) {
            case "Sphere":
                if (right) {
                    for (float move = 2.01f; move <= 6.01f; move += 2.01f) {
                        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) {
                            if (tile.transform.position == new Vector3(interactedShape.transform.position.x + move, interactedShape.transform.position.y - Constants.UNIT_TILE_DIFF, interactedShape.transform.position.z)) {
                                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
                                    for (float moveAux = move; moveAux > 0f; moveAux -= 2.01f) {
                                        if (unit.transform.position == new Vector3(interactedShape.transform.position.x + moveAux, interactedShape.transform.position.y, interactedShape.transform.position.z)) {
                                            occupied = true;
                                        }
                                    }
                                }
                                if (!occupied) {
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(new Vector3(interactedShape.transform.position.x + move, interactedShape.transform.position.y, interactedShape.transform.position.z));
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteracting(this.gameObject);
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteracted(true);
                                }
                                occupied = false;
                            }
                        }
                    }
                } else if (left) {
                    for (float move = 2.01f; move <= 6.01f; move += 2.01f) {
                        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) {
                            if (tile.transform.position == new Vector3(interactedShape.transform.position.x - move, interactedShape.transform.position.y - Constants.UNIT_TILE_DIFF, interactedShape.transform.position.z)) {
                                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
                                    for (float moveAux = move; moveAux >= 2.01f; moveAux -= 2.01f) {
                                        if (unit.transform.position == new Vector3(interactedShape.transform.position.x - moveAux, interactedShape.transform.position.y, interactedShape.transform.position.z)) {
                                            occupied = true;
                                        }
                                    }
                                }
                                if (!occupied) {
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(new Vector3(interactedShape.transform.position.x - move, interactedShape.transform.position.y, interactedShape.transform.position.z));
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteracting(this.gameObject);
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteracted(true);
                                }
                                occupied = false;
                            }
                        }
                    }
                } else if (forward) {
                    for (float move = 2.01f; move <= 6.01f; move += 2.01f) {
                        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) {
                            if (tile.transform.position == new Vector3(interactedShape.transform.position.x, interactedShape.transform.position.y - Constants.UNIT_TILE_DIFF, interactedShape.transform.position.z + move)) {
                                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
                                    for (float moveAux = move; moveAux > 0f; moveAux -= 2.01f) {
                                        if (unit.transform.position == new Vector3(interactedShape.transform.position.x, interactedShape.transform.position.y, interactedShape.transform.position.z + moveAux)) {
                                            occupied = true;
                                        }
                                    }
                                }
                                if (!occupied) {
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(new Vector3(interactedShape.transform.position.x, interactedShape.transform.position.y, interactedShape.transform.position.z + move));
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteracting(this.gameObject);
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteracted(true);
                                }
                                occupied = false;
                            }
                        }
                    }
                } else if (backward) {
                    for (float move = 2.01f; move <= 6.01f; move += 2.01f) {
                        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) {
                            if (tile.transform.position == new Vector3(interactedShape.transform.position.x, interactedShape.transform.position.y - Constants.UNIT_TILE_DIFF, interactedShape.transform.position.z - move)) {
                                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
                                    for (float moveAux = move; moveAux > 0f; moveAux -= 2.01f) {
                                        if (unit.transform.position == new Vector3(interactedShape.transform.position.x, interactedShape.transform.position.y, interactedShape.transform.position.z - moveAux)) {
                                            occupied = true;
                                        }
                                    }
                                }
                                if (!occupied) {
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(new Vector3(interactedShape.transform.position.x, interactedShape.transform.position.y, interactedShape.transform.position.z - move));
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteracting(this.gameObject);
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteracted(true);
                                }
                                occupied = false;
                            }
                        }
                    }
                }
                break;

            case "Pyramid":
                occupied = true;
                if (right) {
                    for (float move = 2.01f; move <= 2.01 * GameObject.FindGameObjectsWithTag("Unit").Length; move += 2.01f) {
                        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) {
                            if (tile.transform.position == new Vector3(originalPosition.x + move, originalPosition.y - Constants.UNIT_TILE_DIFF, originalPosition.z)) {
                                occupied = false;
                                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
                                    if (unit.transform.position == new Vector3(originalPosition.x + move, originalPosition.y, originalPosition.z)) {
                                        occupied = true;
                                    }
                                }
                                if (!occupied) {
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(new Vector3(originalPosition.x + move, originalPosition.y + 2f, originalPosition.z));
                                }
                            }
                        }
                        if (!occupied) break;
                    }
                    if (occupied) interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(originalPosition);
                } else if (left) {
                    for (float move = 2.01f; move <= 2.01 * GameObject.FindGameObjectsWithTag("Unit").Length; move += 2.01f) {
                        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) {
                            if (tile.transform.position == new Vector3(originalPosition.x - move, originalPosition.y - Constants.UNIT_TILE_DIFF, originalPosition.z)) {
                                occupied = false;
                                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
                                    if (unit.transform.position == new Vector3(originalPosition.x - move, originalPosition.y, originalPosition.z)) {
                                        occupied = true;
                                    }
                                }
                                if (!occupied) {
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(new Vector3(originalPosition.x - move, originalPosition.y + 2f, originalPosition.z));

                                }
                            }
                        }
                        if (!occupied) break;
                    }
                    if (occupied) interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(originalPosition);
                } else if (forward) {
                    for (float move = 2.01f; move <= 2.01 * GameObject.FindGameObjectsWithTag("Unit").Length; move += 2.01f) {
                        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) {
                            if (tile.transform.position == new Vector3(originalPosition.x, originalPosition.y - Constants.UNIT_TILE_DIFF, originalPosition.z + move)) {
                                occupied = false;
                                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
                                    if (unit.transform.position == new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + move)) {
                                        occupied = true;
                                    }
                                }
                                if (!occupied) {
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(new Vector3(originalPosition.x, originalPosition.y + 2f, originalPosition.z + move));
                                }
                            }
                        }
                        if (!occupied) break;
                    }
                    if (occupied) interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(originalPosition);
                } else if (backward) {
                    for (float move = 2.01f; move <= 2.01 * GameObject.FindGameObjectsWithTag("Unit").Length; move += 2.01f) {
                        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile")) {
                            if (tile.transform.position == new Vector3(originalPosition.x, originalPosition.y - Constants.UNIT_TILE_DIFF, originalPosition.z - move)) {
                                occupied = false;
                                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit")) {
                                    if (unit.transform.position == new Vector3(originalPosition.x, originalPosition.y, originalPosition.z - move)) {
                                        occupied = true;
                                    }
                                }
                                if (!occupied) {
                                    interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(new Vector3(originalPosition.x, originalPosition.y + 2f, originalPosition.z - move));
                                }
                            }
                        }
                        if (!occupied) break;
                    }
                    if (occupied) interactedShape.GetComponentInChildren<PlayerShape>().SetInteractTarget(originalPosition);
                }
                break;

            case "Plank":
                GameObject buildTile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                buildTile.transform.position = interactTarget;
                buildTile.GetComponent<Renderer>().material.color = Constants.COLOR_TILE_NORMAL;
                buildTile.AddComponent<Tile>();
                GameObject.Find("Main Camera").GetComponent<UIManager>().ClearUI();
                Destroy(this.transform.parent.gameObject);
                break;
        }



        interactedShape = null;
    }

    /**
     *  Special trigger for the cube's stomp, which needs to handle the stomped shapes reaction (animation, creation of shapes,
     *  destruction of other shapes, etc...)
     */
    public void TriggerStomp() {
        GameObject interacted = interactedShape.transform.GetChild(0).gameObject;
        int i = 0;

        switch (interacted.name) {
            case "Sphere":
                interacted.GetComponent<Animator>().SetBool("stomped", true);
                break;
            case "Cube":
                interacted.GetComponent<Animator>().SetBool("stomped", true);
                break;
            case "Plank":
                interacted.GetComponent<Animator>().SetBool("stomped", true);
                break;
            case "Pyramid":
                List<Vector3> freeTiles = Pathing.NeighbouringFree(interacted.transform.parent.position);
                if(freeTiles.Count >= 2) {
                    while(i < 2) {
                        GameObject parent = new GameObject("Player");
                        GameObject miniCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        parent.tag = "Unit";
                        parent.transform.position = freeTiles[i];
                        miniCube.name = "Mini Cube";
                        miniCube.transform.parent = parent.transform;
                        miniCube.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                        miniCube.transform.localPosition = new Vector3(0f,-0.25f,0f);
                        miniCube.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                        miniCube.GetComponent<Renderer>().material = Resources.Load("Materials/Material_002", typeof(Material)) as Material;
                        miniCube.AddComponent<PlayerShape>();
                        miniCube.AddComponent<Animator>();
                        miniCube.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)RuntimeAnimatorController.Instantiate(Resources.Load("Animations/Mini Cube/Mini Cube"));
                        
                        i++;
                    }
                    GameObject.Find("Main Camera").GetComponent<UIManager>().ClearUI();
                    Destroy(transform.parent.gameObject);
                }
                break;
        }
    }

    public void TriggerAfterStomp(){
        GameObject interacted = interactedShape.transform.GetChild(0).gameObject;
        GameObject newObject;

        switch (interacted.name) {
            case "Sphere":
                interacted.GetComponent<Animator>().SetBool("stomped", false);
                break;
            case "Cube":
                interacted.GetComponent<Animator>().SetBool("stomped", false);
                newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newObject.name = "Plank";
                newObject.transform.parent = interacted.transform.parent;
                newObject.transform.localPosition = Vector3.zero;
                newObject.transform.localScale = new Vector3(1f, 0.01f, 1f);
                newObject.transform.eulerAngles = new Vector3(0f,0f,-90f);
                newObject.GetComponent<Renderer>().material = Resources.Load("Materials/Material_002", typeof(Material)) as Material;
                newObject.AddComponent<PlayerShape>();
                newObject.AddComponent<Animator>();
                newObject.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)RuntimeAnimatorController.Instantiate(Resources.Load("Animations/Plank/Plank"));
                Destroy(interacted);
                break;
            case "Plank":
                interacted.GetComponent<Animator>().SetBool("stomped", false);
                break;
        }
    }

    public void SetInteracted(bool state)
    {
        this.interacted = state;

    }

    public void SetInteractTarget(Vector3 target) {
        this.interactTarget = target;

    }

    public void SetInteracting(GameObject interactingObject) {
        this.interactingShape = interactingObject;

    }

    public bool GetInteracted() {
        return interacted;
    }

    public int getMovement(){
		return movement;
	}

    public bool HasInteracted() {
        return hasInteracted;
    }

    public bool HasMoved() {
        return hasMoved;
    }

    public void ResetTurnFlags() {
        hasMoved = false;
        hasInteracted = false;
        if(name == "Mini Cube") {
            hasInteracted = true;
        }
    }
}