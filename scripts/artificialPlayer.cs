using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class artificialPlayer : MonoBehaviour
{
    public GameObject snake;
    public GameObject apple;
    public BoxCollider2D GridArea; 
    private List<Transform> posSegments;

    private List<List<int>> Grid;
    private Vector2 headPos;
    private Vector2 tailPos;
    private Vector2 applePos;
    private int GridX;
    private int GridY;

    public Text aiText;
    public Button setManhattan; // Val 0
    public Button setEuclidian; // Val 1
    public Button setHamilton; // Val 2
    public Button setBFS; // Val 3
    public Button setSafeBFS; // Val 4
    private int selectedAlgorithm = 0;

    void Start() {
        setManhattan.onClick.AddListener(delegate { setAlgorithm(0); });
        setEuclidian.onClick.AddListener(delegate { setAlgorithm(1); });
        setHamilton.onClick.AddListener(delegate { setAlgorithm(2); });
        setBFS.onClick.AddListener(delegate { setAlgorithm(3); });
        setSafeBFS.onClick.AddListener(delegate { setAlgorithm(4); });
    }

    void FixedUpdate() {
        bool AIplayer = !snake.GetComponent<snake>().humanPlayer;
        if(AIplayer) {
            switch (selectedAlgorithm) {
            case 0:
                ExecuteManhattan();
                break;
            case 1:
                ExecuteEuclidian();
                break;
            case 2:
                ExecuteHamilton();
                break;
            case 3:
                ExecuteBFS();
                break;
            case 4:
                ExecuteSafeBFS();
                break;
            }
        } else {
            aiText.text = "";
        }
    }

    private void setAlgorithm(int num) {
        selectedAlgorithm = num;
    }

    void InitializeGrid() {
        posSegments = snake.GetComponent<snake>().segments;
        Grid = new List<List<int>>();
        for (int i = 0; i < GridArea.transform.position.x * 2 + 1; i++) {
            Grid.Add(new List<int>());
            for (int j = 0; j < GridArea.transform.position.y * 2 + 1; j++) {
                Grid[i].Add(0);
            }
        }
        for (int i = 0; i < posSegments.Count; i++) {
            Grid[(int)posSegments[i].position.x][(int)posSegments[i].position.y] = 1;
        }
        GridX = Grid.Count;
        GridY = Grid[0].Count;
        headPos = new Vector2(snake.GetComponent<snake>().GetHead().position.x, snake.GetComponent<snake>().GetHead().position.y);
        tailPos = new Vector2(snake.GetComponent<snake>().GetTail().position.x, snake.GetComponent<snake>().GetTail().position.y);
        applePos = new Vector2(apple.transform.position.x, apple.transform.position.y);
    }

    Vector3 MovementDirection(Vector3 newPosition) { // DEvuelve la direccion de la posicion nueva
        Vector3 head = snake.GetComponent<snake>().GetHead().position;
        int newX = (int)head.x - (int)newPosition.x;
        int newY = (int)head.y - (int)newPosition.y;

        if(newX == 1) {
            return Vector3.left;
        }
        if(newX == -1) {
            return Vector3.right;
        }
        if(newY == 1) {
            return Vector3.down;
        }
        return Vector3.up; // Unico caso posible restante
    }

    List<Vector3> GetAdjacent() { // Devuelve lista de posiciones adyacente a la actual
        List<Vector3> adjacent = new List<Vector3>();
        Vector3 head = snake.GetComponent<snake>().GetHead().position;
        int row = (int)head.x;
        int column = (int)head.y;

        // Verificar vecino superior
        if(row > 0 && Grid[row - 1][column] == 0) {
            adjacent.Add(new Vector3(row - 1, column));
        }
        // Verificar vecino inferior
        if(row < GridX - 1 && Grid[row + 1][column] == 0) {
            adjacent.Add(new Vector3(row + 1, column));
        }
        // Verificar vecino izquierdo
        if(column > 0 && Grid[row][column - 1] == 0) {
            adjacent.Add(new Vector3(row, column - 1));
        }
        // Verificar vecino derecho
        if(column < GridY - 1 && Grid[row][column + 1] == 0) {
            adjacent.Add(new Vector3(row, column + 1));
        }

        return adjacent;
    }

    List<Vector3> GetAdjacent(Vector3 head) { // Devuelve lista de posiciones adyacente a la actual
        List<Vector3> adjacent = new List<Vector3>();
        int row = (int)head.x;
        int column = (int)head.y;

        // Verificar vecino superior
        if (row > 0 && Grid[row - 1][column] == 0) {
            adjacent.Add(new Vector3(row - 1, column));
        }
        // Verificar vecino inferior
        if (row < GridX - 1 && Grid[row + 1][column] == 0) {
            adjacent.Add(new Vector3(row + 1, column));
        }
        // Verificar vecino izquierdo
        if (column > 0 && Grid[row][column - 1] == 0) {
            adjacent.Add(new Vector3(row, column - 1));
        }
        // Verificar vecino derecho
        if (column < GridY - 1 && Grid[row][column + 1] == 0) {
            adjacent.Add(new Vector3(row, column + 1));
        }

        return adjacent;
    }

    void ExecuteManhattan() {
        InitializeGrid();
        aiText.text = "Manhattan";
        Vector3 newDirection = MovementDirection(ComputeAlgorithm(apple.transform.position));
        snake.GetComponent<snake>()._direction = newDirection;
    }

    void ExecuteEuclidian() {
        InitializeGrid();
        aiText.text = "Euclidian";
        Vector3 newDirection = MovementDirection(ComputeAlgorithm(apple.transform.position, 1));
        snake.GetComponent<snake>()._direction = newDirection;
    }

    void ExecuteHamilton() {
        InitializeGrid();
        aiText.text = "Hamilton";
        Vector3 newDirection = MovementDirection(ComputeAlgorithm(apple.transform.position, 2));
        snake.GetComponent<snake>()._direction = newDirection;
    }

    void ExecuteBFS() {
        InitializeGrid();
        aiText.text = "BFS";
        Vector3 newDirection = MovementDirection(ComputeAlgorithm(apple.transform.position, 3));
        snake.GetComponent<snake>()._direction = newDirection;
    }

    void ExecuteSafeBFS() {
        InitializeGrid();
        aiText.text = "Safe BFS";
        Vector3 newDirection = MovementDirection(ComputeAlgorithm(apple.transform.position, 4));
        snake.GetComponent<snake>()._direction = newDirection;
    }

    Vector3 ComputeAlgorithm(Vector3 apple, int algorithm = 0) {
        List<Vector3> adjacents = GetAdjacent();
        float min = Mathf.Infinity;
        Vector3 newPosition = Vector3.left;

        for(int i = 0; i < adjacents.Count; i++) {
            float distanceIteration = 0;
            switch (algorithm) {
                case 0:
                    distanceIteration = CalculateManhattan(adjacents[i], apple);
                    break;
                case 1:
                    distanceIteration = CalculateEuclidian(adjacents[i], apple);
                    break;
                case 2:
                    distanceIteration = CalculateHamilton(adjacents[i]);
                    break;
                case 3:
                    distanceIteration = CalculateBFS(adjacents[i]);
                    break;
                case 4:
                    distanceIteration = CalculateSafeBFS(adjacents[i]);
                    break;
            }
            if(min > distanceIteration) {
                newPosition = adjacents[i];
                min = distanceIteration;
            }
        }

        return newPosition;
    }

    float CalculateManhattan(Vector3 a, Vector3 b) {
        return math.abs(a.x - b.x) + math.abs(a.y - b.y);
    }

    float CalculateEuclidian(Vector3 a, Vector3 b) {
        return math.sqrt(math.pow((a.x - b.x), 2) + math.pow((a.y - b.y), 2));
    }

    float CalculateHamilton(Vector3 cell) {
        Vector3 dirCell = MovementDirection(cell);
        Vector3 dirSnake = MovementDirection(posSegments[1].position) * -1;
        if (dirSnake == Vector3.right) {
            if ((headPos.x < GridX - 2 || headPos.y == GridY) && dirCell == dirSnake) {
                return 0;
            }
            if (headPos.x >= GridX - 2 && dirCell == Vector3.down) {
                return 0;
            }
        }
        if (dirSnake == Vector3.left) {
            if ((headPos.x > 1 || headPos.y == GridY) && dirCell == dirSnake) {
                return 0;
            }
            if (headPos.x <= 1 && dirCell == Vector3.down) {
                return 0;
            }
        }
        if (dirSnake == Vector3.left || dirSnake == Vector3.right) {
            if ((headPos.x == 0 || headPos.x == GridX) && headPos.y == GridY && dirCell == Vector3.up) {
                return 0;
            }
        }
        if (dirSnake == Vector3.down) {
            if (headPos.x < GridX / 2 && dirCell == Vector3.right) {
                return 0;
            }
            if (headPos.x > GridX / 2 && dirCell == Vector3.left) {
                return 0;
            }
        }
        if (dirSnake == Vector3.up) {
            if (dirCell == Vector3.up) {
                return 0;
            }
            if ((headPos.y == 0 || headPos.y == GridY) && headPos.x == 0 && (dirCell == Vector3.right || dirCell == Vector3.left)) {
                return 0;
            }
        }
        return 999;
    }

    float CalculateBFS(Vector3 cell) {
        List<Vector3> snakePositions = posSegments.Select(x => x.position).ToList();
        return BFS(cell, applePos, snakePositions).Count;
    }


    float CalculateSafeBFS(Vector3 cell) {
        List<Vector3> snakePositions = posSegments.Select(x => x.position).ToList();
        int size = GridX * GridY * 2;

        if (Includes(snakePositions, cell)) return size * 2;

        List<Vector3> pathToPoint = BFS(cell, applePos, snakePositions);

        // Si hay camino a la manzana y una vez que la coma haya camino a la cola
        if (pathToPoint.Count > 0) {

        List<Vector3> snakeAtPoint = shift(snakePositions, pathToPoint, true);
            foreach (Vector3 next in Difference(GetAdjacent(snakeAtPoint[0]), snakeAtPoint)) {
                foreach (Vector3 tailNext in Difference(GetAdjacent(snakeAtPoint.Last()), snakeAtPoint)) {
                    if (BFS(next, tailNext, snakeAtPoint).Count > 0) { // Hay camino al final de la cola
                        // Debug.log("Camino a la manzana y al final de la cola.");
                        return pathToPoint.Count;
                    }
                }
            }

        }

        foreach (Vector3 tailNext in Difference(GetAdjacent(tailPos), snakePositions)) {
            List<Vector3> pathToTail = BFS(cell, tailNext, snakePositions);
            if (pathToTail.Count > 0) { // No hay camino a la manzana pero si al final de la cola
                // Debug.log("Camino a la cola pero no a la manzana.");
                return size - pathToTail.Count;
            }
        }
        // Debug.log("Sin camino a la manzana y tampoco a la cola.");
        return size * 2;
    }

    List<Vector3> BFS(Vector3 start, Vector3 end, List<Vector3> snake) {
        Queue<Node> queue = new Queue<Node>();
        HashSet<Vector3> visited = new HashSet<Vector3>();
        visited.Add(start);
        queue.Enqueue(new Node(start));

        while (queue.Count > 0) {
            Node current = queue.Dequeue();
            if (current.position == end) {
                List<Vector3> path = new List<Vector3>();
                while (current.parent != null) {
                    path.Add(current.position);
                    current = current.parent;
                }
                if (path.Count == 0) path.Add(start);
                // Debug.log("test;");
                path.Reverse();
                return path;
                // Devuelve el camino
            }
            foreach (Vector3 next in GetAdjacent(current.position)) {
                if (!visited.Contains(next) && !Includes(snake, next)) {
                    visited.Add(next);
                    queue.Enqueue(new Node(next, current));
                }
            }
        }
        return new List<Vector3>();
    }


    List<Vector3> shift(List<Vector3> a, List<Vector3> b, bool collect = false) {
        return Enumerable.Concat(b, a).Take(b.Count + a.Count - b.Count + (collect ? 1 : 0)).ToList();
    }
    List<Vector3> Difference(List<Vector3> a, List<Vector3> b) {
        return a.Where(item => !Includes(b, item)).ToList();
    }
    bool Includes(List<Vector3> a, Vector3 b) {
        return a.Any(item => item == b);
    }

}

class Node {
    public Vector3 position { get; set;}
    public Node parent {get; set;}

    public Node(Vector3 position, Node parent = null) {
        this.position = position;
        this.parent = parent;
    }
}