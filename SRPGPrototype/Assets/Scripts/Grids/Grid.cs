using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public abstract class Grid<Obj> : MonoBehaviour, IEnumerable<Obj> where Obj : GridObject
    {
        public static readonly Vector2Int OutOfBounds = new Vector2Int(-100, -100);
        public abstract Vector2Int Dimensions { get; }

        public int MaxGridDistance => Dimensions.x + Dimensions.y - 2;

        public float MaxDistance => Vector2Int.Distance(new Vector2Int(0, 0), Dimensions - Vector2Int.one);

        /// <summary>
        /// Return all the currently empty positions in the grid
        /// O(x*y)
        /// </summary>
        public IEnumerable<Vector2Int> EmptyPositions
        {
            get
            {
                for (int x = 0; x < Dimensions.x; ++x)
                {
                    for (int y = 0; y < Dimensions.y; ++y)
                    {
                        var pos = new Vector2Int(x, y);
                        if (IsEmpty(pos))
                            yield return pos;
                    }
                }
            }
        }
        public Vector2 CenterToVextexOffset => new Vector2((cellSize.x + skewXOffset) * 0.5f, cellSize.y * 0.5f);

        [Header("Grid")]
        public Vector2 cellSize = new Vector2(1, 1);
        public float skewAngle = 0;
        private float skewXOffset;
        [Header("Rendering")]
        public GameObject gridLinePrefab;
        public TileUI tileUIManager;

        private Obj[,] field;
        private readonly List<LineRenderer> lines = new List<LineRenderer>();

        /// <summary>
        /// Display grid lines in-editor when gizmos are drawn
        /// Doesn't run during runtime, even in-editor with gizmos enabled
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;
            InitializeField();
            InitializeSkew();
            Gizmos.color = Color.black;
            Vector2 offset = CenterToVextexOffset;
            for (int y = 0; y <= Dimensions.y; ++y)
            {
                Vector2 point1 = GetSpace(new Vector2Int(0, y)) - offset;
                Vector2 point2 = GetSpace(new Vector2Int(Dimensions.x, y)) - offset;
                Gizmos.DrawLine(point1, point2);
            }
            for (int x = 0; x <= Dimensions.x; ++x)
            {
                Vector2 point1 = GetSpace(new Vector2Int(x, 0)) - offset;
                Vector2 point2 = GetSpace(new Vector2Int(x, Dimensions.y)) - offset;
                Gizmos.DrawLine(point1, point2);
            }
        }

        protected void Initialize()
        {
            InitializeField();
            InitializeSkew();
            InitializeGridLines();
        }

        private void InitializeField()
        {
            field = new Obj[Dimensions.x, Dimensions.y];
        }

        /// <summary>
        /// Precalculate / bake grid skew math to optimize rendering and position calculation
        /// </summary>
        private void InitializeSkew()
        {
            float tan = Mathf.Tan(Mathf.Deg2Rad * skewAngle);
            skewXOffset = cellSize.y * tan;
        }

        #region Rendering

        public void CenterAtPosition(Vector2 worldPos)
        {
            var centerPos = GetSpace(new Vector2Int(Dimensions.x / 2, Dimensions.y / 2));
            if (Dimensions.x % 2 == 0)
            {
                centerPos.x -= CenterToVextexOffset.x;
            }
            if (Dimensions.y % 2 == 0)
            {
                centerPos.y -= CenterToVextexOffset.y;
            }
            var centerOffset = centerPos - new Vector2(transform.position.x, transform.position.y);
            transform.position = worldPos - centerOffset;
            InitializeGridLines();
        }

        /// <summary>
        /// Create and display the grid line objects
        /// Precondition: InitializeSkew has been called
        /// </summary>
        private void InitializeGridLines()
        {
            lines.ForEach((l) => Destroy(l.gameObject));
            lines.Clear();
            Vector2 offset = CenterToVextexOffset;
            // Draw row lines
            for (int y = 0; y <= Dimensions.y; ++y)
            {
                var line = Instantiate(gridLinePrefab, transform).GetComponent<LineRenderer>();
                Vector2 point1 = GetSpace(new Vector2Int(0, y)) - offset;
                Vector2 point2 = GetSpace(new Vector2Int(Dimensions.x, y)) - offset;
                line.SetPositions(new Vector3[] { point1, point2 });
                lines.Add(line);
            }
            // Drawn column lines
            for (int x = 0; x <= Dimensions.x; ++x)
            {
                var line = Instantiate(gridLinePrefab, transform).GetComponent<LineRenderer>();
                Vector2 point1 = GetSpace(new Vector2Int(x, 0)) - offset;
                Vector2 point2 = GetSpace(new Vector2Int(x, Dimensions.y)) - offset;
                line.SetPositions(new Vector3[] { point1, point2 });
                lines.Add(line);
            }
        }

        /// <summary>
        /// Spawns a gridsquare prefab and initializes it's QuadMesh component with the proper vertices (if applicable)
        /// Optionally, applies a given mater to the mesh
        /// Used to display grid-related information such as movement and attack ranges
        /// </summary>
        /// <param name="p"> The grid position to spawn the square at </param>
        /// <param name="mat"> 
        /// The material to apply. The grid contains several references to materials (e.g BattleGrid.main.moveSquareMat) 
        /// </param>
        /// <returns> Returns the created object </returns>
        public TileUI.Entry SpawnTileUI(Vector2Int p, TileUI.Type type)
        {
            if (!IsLegal(p))
                return new TileUI.Entry() { pos = OutOfBounds, type = TileUI.Type.Empty };

            // Calculate Vertices
            Vector2 offset = CenterToVextexOffset;
            var v1 = GetSpace(p) + offset; // Top right corner
            var v2 = v1 + new Vector2(-cellSize.x, 0); // Top left corner
            var v3 = GetSpace(p + Vector2Int.down) + offset; // Bottom right corner
            var v4 = v3 + new Vector2(-cellSize.x, 0); // Bottom left corner
                                                       // Create a new tile
            return tileUIManager.SpawnTileUI(p, type, v1, v2, v3, v4);
        }

        public void RemoveTileUI(TileUI.Entry entry)
        {
            tileUIManager.Remove(entry);
        }

        #endregion

        #region Field Methods

        /// <summary>
        /// Get the world-space coordiante of the center of a square of the grid.
        /// Input does not need to be a legal position (see IsLegal(pos))
        /// </summary>
        public Vector2 GetSpace(Vector2Int pos)
        {
            // If in editor mode and the application is not playing, make sure the skew is initialized
            if (Application.isEditor && !Application.isPlaying)
                InitializeSkew();
            float y = transform.position.y + pos.y * (cellSize.y);
            float x = transform.position.x + pos.x * (cellSize.x);
            x += skewXOffset * (pos.y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Return the grid-space position of the sqaure that contains the given world space coordinate.
        /// May return a non-legal position (see IsLegal(pos))
        /// </summary>
        public Vector2Int GetPos(Vector2 worldSpace)
        {
            float leniency = (cellSize.x / 2);
            float yleniency = cellSize.y / 2;
            int y = -Mathf.FloorToInt(transform.position.y + yleniency - worldSpace.y / (cellSize.y));
            int x = Mathf.FloorToInt((worldSpace.x + leniency - transform.position.x - (skewXOffset * y)) / (cellSize.x));
            return new Vector2Int(x, y);
        }

        public bool ContainsPoint(Vector2 worldSpace)
        {
            return IsLegal(GetPos(worldSpace));
        }

        /// <summary>
        /// Is this grid position legal (a.k.a can that grid position contain field objects and/or event tiles)?
        /// </summary>
        /// <param name="pos"> The position is grid space to test for legality </param>
        /// <returns> Is the space with in the grid's row and column dimensions </returns>
        public bool IsLegal(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < Dimensions.x && pos.y >= 0 && pos.y < Dimensions.y;
        }

        public bool IsLegalAndEmpty(Vector2Int pos) => IsLegal(pos) && IsEmpty(pos);

        #endregion

        #region Field Object Methods

        public bool IsEmpty(Vector2Int pos) => field[pos.x, pos.y] == null;

        public bool NotEmpty(Vector2Int pos) => field[pos.x, pos.y] != null;

        /// <summary>
        /// Return the object of type T at the given grid position, or null if the position is empty or filled with an object that is not of type T
        /// </summary>
        public T Get<T>(Vector2Int pos) where T : Obj
        {
            return Get(pos) as T;
        }

        public Obj Get(Vector2Int pos)
        {
            // Avoid references to destroyed GameObjects not working with the ?. operator (return a true null value)
            if (!IsLegal(pos) || field[pos.x, pos.y] == null)
                return null;
            return field[pos.x, pos.y];
        }

        public T Find<T>(Predicate<T> pred) where T : Obj
        {
            //brute force foreach of field; might optimize later
            foreach (var obj in field)
            {
                if (obj is T)
                {
                    var objT = obj as T;
                    //if object matches the predicate, return it
                    if (pred(objT))
                        return objT;
                }
            }
            return null;
        }

        public List<Obj> FindAll(Predicate<Obj> pred = null)
        {
            var objects = new List<Obj>(Dimensions.x * Dimensions.y);
            //brute force foreach of field; might optimize later
            foreach (var obj in field)
            {
                if (obj == null)
                    continue;
                //if there is no predicate or object matches the predicate, add it to the list
                if (pred == null || pred(obj))
                    objects.Add(obj);
            }
            return objects;
        }

        public Obj Raycast(Vector2Int startPos, Vector2Int direction)
        {
            Vector2Int pos = startPos + direction;
            while (IsLegal(pos))
            {
                if (!IsEmpty(pos))
                    return Get(pos);
                pos += direction;
            }
            return null;
        }

        public IEnumerable<Vector2Int> PositionsUntilRaycastHit(Vector2Int startPos, Vector2Int direction)
        {
            Vector2Int pos = startPos + direction;
            while (IsLegalAndEmpty(pos))
            {
                yield return pos;
                pos += direction;
            }
        }

        public Vector2Int FirstEmptyPositionInDirection(Vector2Int startPos, Vector2Int direction)
        {
            Vector2Int pos = startPos + direction;
            while (IsLegal(pos))
            {
                if (IsEmpty(pos))
                    return pos;
                pos += direction;
            }
            return OutOfBounds;
        }

        public IEnumerator<Obj> GetEnumerator()
        {
            return FieldObjects();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Obj> FieldObjects()
        {
            foreach (var obj in field)
            {
                if (obj != null)
                    yield return obj;
            }
        }

        /// <summary>
        /// Add an object to the grid. Returns true if successful, else false
        /// </summary>
        public virtual bool Add(Vector2Int pos, Obj obj)
        {
            Set(pos, obj);
            obj.Pos = pos;
            return true;
        }

        /// <summary>
        /// Set's the object at the given grid position to the given Obj reference (if pos is a legal Grid Position)
        /// </summary>
        public void Set(Vector2Int pos, Obj obj)
        {
            if (IsLegal(pos))
            {
                // ?? operator not used due to Unity Object wrapper
                field[pos.x, pos.y] = (obj == null ? null : obj);
            }

        }

        /// <summary>
        /// Remove a field object from the grid. does not destroy the object or modify its world position.
        /// Set's the object's grid position to OutOfBounds, a non-legal position.
        /// </summary>
        public virtual void Remove(Obj obj)
        {
            Set(obj.Pos, null);
            obj.Pos = OutOfBounds;
        }

        /// <summary>
        /// Moves a field object to a new grid space and updates the objects world position if the object successfully moved.
        /// See Move(obj, dest) for success and failure conditions.
        /// Preconditdion: obj's Position is legal (see IsLegal())
        /// </summary>
        public bool MoveAndSetWorldPos(Obj obj, Vector2Int dest)
        {
            bool moveSuccess = Move(obj, dest);
            if (moveSuccess)
                obj.transform.position = GetSpace(obj.Pos);
            return moveSuccess;
        }

        /// <summary>
        /// Move a Obj to a new grid space.
        /// Move fails if destination is illegal or occupied.
        /// Preconditdion: obj's Position is legal (see IsLegal())
        /// Does not update the object's world position. Use MoveAndSetWorldPos() to move and set world position.
        /// </summary>
        public bool Move(Obj obj, Vector2Int dest)
        {
            // If the destination is not legal and empty, return
            if (!IsLegalAndEmpty(dest))
                return false;
            Vector2Int src = obj.Pos;
            Set(src, null);
            Set(dest, obj);
            obj.Pos = dest;
            return true;
        }

        /// <summary>
        /// Swap two Field Objects' grid positions and then update their world positions based on the new values.
        /// Preconditdion: both objs' Position are legal (see IsLegal()).
        /// </summary>
        public void SwapAndSetWorldPos(Obj obj1, Obj obj2)
        {
            Swap(obj1, obj2);
            // Set the transforms
            obj1.transform.position = GetSpace(obj1.Pos);
            obj2.transform.position = GetSpace(obj2.Pos);
        }

        /// <summary>
        /// Swap two Field Objects' grid positions.
        /// Preconditdion: both objs' Position are legal (see IsLegal()).
        /// Does not update the objects' world positions. Use SwapAndSetWorldPOs() to swap and set world position.
        /// </summary>
        public void Swap(Obj obj1, Obj obj2)
        {
            // Swap positions
            Vector2Int temp = obj1.Pos;
            obj1.Pos = obj2.Pos;
            obj2.Pos = temp;
            Set(obj1.Pos, obj1);
            Set(obj2.Pos, obj2);
        }

        #endregion

        #region Pathing and Reachablilty

        /// <summary>
        /// Calculates the positions that can be reached from a given position, range, and predicate function defining what objects can be moved through.
        /// Takes into account obstacles, but spaces with objects considered to be traversable will still be returned in the dictionary,
        /// even if some mechanics that utilize this method (such as movement) should exclude those spaces later.
        /// </summary>
        /// <param name="startPos"> The grid position to start looking from </param>
        /// <param name="range"> The movement range to serach </param>
        /// <param name="canMoveThrough"> A predicate function determining if a given object is traversable </param>
        /// <returns> A dictionary of reachable grid positions to ints where the ints are the minimum distance to the position. </returns>
        public Dictionary<Vector2Int, int> Reachable(Vector2Int startPos, int range, Predicate<Obj> canMoveThrough)
        {
            // Initialize distances with the startPosition
            var distances = new Dictionary<Vector2Int, int> { { startPos, 0 } };

            // Inner recursive method
            void ReachableRecursive(Vector2Int p, int currDepth)
            {
                // Inner Traversability method
                bool Traversable(Vector2Int pos)
                {
                    return (!distances.ContainsKey(pos) || currDepth + 1 < distances[pos]) && StdTraversable(pos, canMoveThrough);
                };

                // Log discovery and distance
                if (distances.ContainsKey(p))
                {
                    if (distances[p] > currDepth)
                        distances[p] = currDepth;
                }
                else
                {
                    distances.Add(p, currDepth);
                }
                // If depth is greater than range, end recursion
                if (currDepth >= range)
                    return;
                // Get adjacent nodes (traversability function inverted as the Adj function takes a function for non-traversability)
                var nodes = Adj(p, (travVector2Int) => !Traversable(travVector2Int));
                // Recur
                foreach (var node in nodes)
                    ReachableRecursive(node, currDepth + 1);
            }

            // Start Recursion
            ReachableRecursive(startPos, 0);
            return distances;
        }

        /// <summary>
        /// Returns a minimum distance path from the start to the goal, or null if a path doesn't exist.
        /// Based on the AStar pathfinding defined in AStar.Pathfinf
        /// </summary>
        /// <param name="start"> Start position in grid space </param>
        /// <param name="goal"> Goal position in grid space </param>
        /// <param name="canMoveThrough"> A pedicate function defining which field objects can be moved through </param>
        public List<Vector2Int> Path(Vector2Int start, Vector2Int goal, Predicate<Obj> canMoveThrough)
        {
            List<Vector2Int> NodeAdj(Vector2Int p) => Adj(p, (pos) => StdNonTraversable(pos, canMoveThrough));
            return Pathfinding.AStar.Pathfind(start, goal, NodeAdj, (p, pAdj) => 1, (p1, p2) => Vector2Int.Distance(p1, p2));
        }

        /// <summary>
        /// Helper function for grid adjacency. Used in Reachability and pathfinding tests
        /// </summary>
        private List<Vector2Int> Adj(Vector2Int pos, Predicate<Vector2Int> nonTraversable)
        {
            var positions = new List<Vector2Int>()
        {
            pos + Vector2Int.up,
            pos + Vector2Int.down,
            pos + Vector2Int.left,
            pos + Vector2Int.right,
        };
            positions.RemoveAll(nonTraversable);
            return positions;
        }

        /// <summary>
        /// Standard traversability function for use in pathing and reachability calls
        /// </summary>
        private bool StdTraversable(Vector2Int pos, Predicate<Obj> canMoveThrough)
        {
            if (!IsLegal(pos))
                return false;
            return canMoveThrough(field[pos.x, pos.y]);
        }

        /// <summary>
        /// Standard non-traversability function for use in pathing and reachability calls
        /// </summary>
        private bool StdNonTraversable(Vector2Int pos, Predicate<Obj> canMoveThrough) => !StdTraversable(pos, canMoveThrough);

        /// <summary>
        /// Search an area defined by a reachability range for an object that matches a predicate.
        /// Returns the first object found, or null if none are found.
        /// </summary>
        public Obj SearchArea(Vector2Int p, int range, Predicate<Obj> canMoveThrough, Predicate<Obj> pred)
        {
            foreach (var node in Reachable(p, range, canMoveThrough).Keys)
            {
                var obj = field[node.x, node.y];
                if (pred(obj))
                    return obj;
            }
            return null;
        }

        #endregion
    }
}