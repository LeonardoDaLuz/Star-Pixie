
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MatchManager : MonoBehaviour {
	public static MatchManager i;

	[Header("Dimensions")]
	public int columns=5;
	public int rows=5;

	public float PiecesMovementSpeed = 5f;
	public float mergeEffectAnticipationDistance = 0.5f;
	[RelativeToTransform]
	public HandledBox WorldSpaceArea;
	[Header("Stars Rating")]
	public Image[] Stars;

 

	public RandomNewPieces[] piecesSpawn;
	[Header("Counter")]
	public Text movesCounter;
	public int RemaingMoves=50;


	[Header("Debug")]
	public float squareLengthX;
	public float squareLengthY;
	public Vector2 FirstPoint;
	public row[] Rows;
	Coroutine coroutine;
	public Vector3 wMin;
	public bool disableControls;
	public bool FinishMatchWhenThirdStarIsAchieved;
	[System.Serializable]
    public class RandomNewPieces 
	{
		public GameObject Piece;
		[Range(0f,100f)]
		public float Probability;
		public Fallback[] fallbacks;

		[System.Serializable]
        public class Fallback {
			[Range(0f, 100f)]
			public float Probability;
			public GameObject Piece;
		}

	}

	// Use this for initialization
	void Awake () {
		i = this;
		Rows = new row[5];
        for (int i = 0; i < Rows.Length; i++)
        {
			Rows[i] = new row();
			Rows[i].columns = new Piece[columns];
		}

		wMin = WorldSpaceArea.GetWorldSpaceMin();
		movesCounter.text = RemaingMoves.ToString();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("f"))
        {
			FinalScore.i.ShowFinalScore();
        }

		if (disableControls)
			return;

		if (coroutine == null) {
			HandlePCInput();
			HandleTouch();
		}
	}

	void HandlePCInput()
    {
		if (Input.GetButtonDown("Horizontal"))
		{
			var XAdvance = Input.GetAxisRaw("Horizontal") > 0f ? 1 : -1;
			coroutine = StartCoroutine(MovePieces(XAdvance, 0));
		}
		else if (Input.GetButtonDown("Vertical"))
		{
			var YAdvance = Input.GetAxisRaw("Vertical") > 0f ? 1 : -1;
			coroutine = StartCoroutine(MovePieces(0, YAdvance));
		}
	}

	Vector2 TouchBeganPosition;

	int GetTouchCount()
    {
#if UNITY_EDITOR
		if(Input.GetMouseButtonDown(0)||Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
			return 1;
        } else
        {
			return 0;
        }
#else
	return Input.touchCount;
#endif
	}
	Touch GetTouch(int i)
    {

#if UNITY_EDITOR
		var touch = new Touch();
		touch.position = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
			touch.phase = TouchPhase.Began;
		else if (Input.GetMouseButton(0))
			touch.phase = TouchPhase.Moved;
		else if (Input.GetMouseButtonUp(0))
			touch.phase = TouchPhase.Ended;
		else
			touch.phase = TouchPhase.Stationary;

		return touch;       

#else
		return Input.GetTouch(i);
#endif

	}
	bool NewSwipeStarted;
	void HandleTouch()
    {
        if (GetTouchCount() > 0)
        {
			Touch touch = GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
				Debug.Log("BEGAN");
				NewSwipeStarted = true;
				TouchBeganPosition = touch.position;
			}
			else if (NewSwipeStarted && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Moved))
			{
				var Direction = touch.position - TouchBeganPosition;
				if (Direction.magnitude > 10f)
				{
					Debug.Log("newMOve");
					NewSwipeStarted = false;
					if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
					{
						var XAdvance = Direction.x > 0f ? 1 : -1;
						coroutine = StartCoroutine(MovePieces(XAdvance, 0));
					}
					else
					{
						var YAdvance = Direction.y > 0f ? 1 : -1;
						coroutine = StartCoroutine(MovePieces(0, YAdvance));
					}
				}

			}
		}
    }
	IEnumerator MovePieces(int XAdvance, int YAdvance)
    {
		RemaingMoves--;
		movesCounter.text = RemaingMoves.ToString();
		//yield return new WaitForEndOfFrame();

		bool movingThroughSquares = true;
		while (movingThroughSquares)
		{
			movingThroughSquares = false;
			for (int i = 0; i < Rows.Length; i++)
			{
				for (int ib = 0; ib < Rows[i].columns.Length; ib++)
				{
					if (Rows[i].columns[ib] != null)
					{
						if (MoveToNextSquare(Rows[i].columns[ib], XAdvance, YAdvance))
						{
							MovesWithTableFull = 0;
							movingThroughSquares = true;
						}
					}
				}
			}

			bool movingTransform = true;
			while (movingTransform && movingThroughSquares)
			{
				movingTransform = false;
				for (int i = 0; i < Rows.Length; i++)
				{
					for (int ib = 0; ib < Rows[i].columns.Length; ib++)
					{
						if (Rows[i].columns[ib] != null)
						{
							if (MoveTransformToTargetPosition(Rows[i].columns[ib]))
							{
								movingTransform = true;
							}
						}
					}
				}
				if(movingTransform)
					yield return new WaitForEndOfFrame();
			}
			/**Finish merges*
			for (int i = 0; i < Rows.Length; i++)
			{
				for (int ib = 0; ib < Rows[i].columns.Length; ib++)
				{
					if (Rows[i].columns[ib] != null)
					{
						if (MoveToNextSquare(Rows[i].columns[ib], XAdvance, YAdvance))
						{
							movingThroughSquares = true;
						}
					}
				}
			}*/

		}

		GenerateNewPieces();
        if (TableIsFull())
        {
			MovesWithTableFull++;
			if (MovesWithTableFull>3)
			{
				FinalScore.i.ShowFinalScore();
			}			
		} else
        {
			MovesWithTableFull = 0;
		}
		coroutine = null;
	}

	int MovesWithTableFull;
	void GenerateNewPieces()
    {
		int attempts = 105;

        for (int i = 0; i < piecesSpawn.Length; i++)
        {
			var _randomPieces = piecesSpawn[i];

			GameObject ObjToCreate = null;
			if (Random.Range(0f, 100f) <= _randomPieces.Probability)
				ObjToCreate = _randomPieces.Piece;
			else
            {
                for (int f = 0; f < _randomPieces.fallbacks.Length; f++)
                {
					var fallback = _randomPieces.fallbacks[f];
					if (Random.Range(0f, 100f) <= fallback.Probability)
					{
						ObjToCreate = fallback.Piece;
						break;
					}
				}
            }


			if (ObjToCreate != null) {
				for (int a = 0; a < attempts; a++)
				{
					int x = Random.Range(0, columns);
					int y = Random.Range(0, rows);

					if (Rows[y].columns[x] == null)
					{
						GameObject obj = (GameObject)Instantiate(ObjToCreate, new Vector3(wMin.x + x * squareLengthX, wMin.y + y * squareLengthY, 0f), Quaternion.identity, transform);
						obj.GetComponent<Piece>().Start();
						var animator = obj.GetComponent<Animator>();
						if (animator != null)
							animator.SetTrigger("TriggerIntroAnimation");
						break;
					}
				}			
            }
		}
    }

	bool TableIsFull()
    {
        for (int i = 0; i < Rows.Length; i++)
        {
			for (int ib = 0; ib < Rows[i].columns.Length; ib++)
			{
                if (Rows[i].columns[ib] == null)
                {
					return false;
                }
			}
		}
		return true;
    }
	void InsertPieceToMatrixPosition(Piece piece, int row, int column)
    {
		//if (Rows[row].columns[column] != null)
		//	Debug.Log("EPA, nao deveria haver objeto(" + piece.name + ")  aqui " + row+","+column);

		Rows[row].columns[column] = piece;

		//Debug.Log(piece.name + " inserted " + row + " - " + column);
	}

	void RemovePieceFromMatrixPosition(Piece piece, int row, int column)
	{
		//if (Rows[row].columns[column] == null)
		//	Debug.Log("EPA, deveria haver objeto("+ piece.name + ") aqui " + row + "," + column);

		//Debug.Log(piece.name + " removed " + row + " - " + column);
		Rows[row].columns[column] = null;
	}


	bool MoveToNextSquare(Piece piece, int XAdvance, int YAdvance)
    {
		if (piece.LastComputation == Time.frameCount)
			return false;

		int ColumnToTest = piece.positionX + XAdvance;
		int rowToTest = piece.positionY + YAdvance;


		if (rowToTest>-1 && ColumnToTest>-1 && Rows.Length> rowToTest && Rows[rowToTest].columns.Length> ColumnToTest)
        {
			piece.LastComputation = Time.frameCount;
			if (Rows[rowToTest].columns[ColumnToTest] == null)
			{
				RemovePieceFromMatrixPosition(piece, piece.positionY, piece.positionX);
				piece.positionX += XAdvance;
				piece.positionY += YAdvance;
				InsertPieceToMatrixPosition(piece, piece.positionY, piece.positionX);
				//Rows[piece.positionY].columns[piece.positionX] = piece;
				return true;

			} else
            {
				if (MoveToNextSquare(Rows[rowToTest].columns[ColumnToTest], XAdvance, YAdvance))
				{
					if (Rows[rowToTest].columns[ColumnToTest] == null)
					{
						RemovePieceFromMatrixPosition(piece, piece.positionY, piece.positionX);
						piece.positionX += XAdvance;
						piece.positionY += YAdvance;
						InsertPieceToMatrixPosition(piece, piece.positionY, piece.positionX);				


					} 
					return true;
				}
				else if (piece.checkToMerge(Rows[rowToTest].columns[ColumnToTest]))
				{
					RemovePieceFromMatrixPosition(piece, piece.positionY, piece.positionX);
					piece.positionX += XAdvance;
					piece.positionY += YAdvance;
					InsertPieceToMatrixPosition(piece, piece.positionY, piece.positionX);
					return true;
				}
			}
			//UpdatePiecePosition(piece);
		}

		piece.LastComputation = Time.frameCount;
		return false;
	}


	bool MoveTransformToTargetPosition(Piece piece)
    {
		var targetPosition = new Vector3(wMin.x + piece.positionX * squareLengthX + squareLengthX * 0.5f, wMin.y + piece.positionY * squareLengthY + +squareLengthY * 0.5f, piece.transform.position.z);

		if (piece.transform.position == targetPosition)
		{
            if (piece.willMergeTo != null)
            {
				piece.merge();
            }
			return false;
		}

		if (piece.willMergeTo != null && piece.mergeEffect != null && Vector3.Distance(piece.transform.position, targetPosition)< mergeEffectAnticipationDistance)
        {
			//piece.merge();
			piece.InstantiateMergeEffect();
        }

		var direction = (targetPosition - piece.transform.position).normalized;
		var newPos = piece.transform.position + direction * PiecesMovementSpeed * Time.deltaTime;
		piece.transform.position = new Vector3(Mathf.Clamp(newPos.x, (direction.x>0?-Mathf.Infinity: targetPosition.x), (direction.x > 0 ? targetPosition.x: Mathf.Infinity)),
			Mathf.Clamp(newPos.y, (direction.y > 0 ? -Mathf.Infinity : targetPosition.y), (direction.y > 0 ? targetPosition.y : Mathf.Infinity)), 0);
		return true;
	}
	void UpdatePiecePosition(Piece piece)
	{
		piece.transform.position = new Vector3(wMin.x + piece.positionX * squareLengthX + squareLengthX * 0.5f, wMin.y + piece.positionY * squareLengthY + +squareLengthY * 0.5f, piece.transform.position.z);
	}

	public void AddPiece(Piece piece)
    {

		var relativePosX = piece.transform.position.x - wMin.x;
		var relativePosY = piece.transform.position.y - wMin.y;
		piece.positionX = Mathf.FloorToInt(relativePosX / squareLengthX);
		piece.positionY = Mathf.FloorToInt(relativePosY / squareLengthY);
		InsertPieceToMatrixPosition(piece, piece.positionY, piece.positionX);
		UpdatePiecePosition(piece);

	}
	public void RemovePiece(Piece piece)
	{
		Rows[piece.positionY].columns[piece.positionX] = null;
	}

	void OnDrawGizmosSelected()
    {
		piecesSpawn = piecesSpawn;
		squareLengthX = WorldSpaceArea.bounds.size.x / columns;
		squareLengthY = WorldSpaceArea.bounds.size.y / rows;

		FirstPoint = WorldSpaceArea.GetWorldSpaceMin();
	}

	[System.Serializable]
    public class row {
		public string label = "";
		public Piece[] columns;
	}

}

