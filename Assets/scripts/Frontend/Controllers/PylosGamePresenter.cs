using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pylos.Backend.Models;

public class PylosGamePresenter : MonoBehaviour
{
    [SerializeField] private BoardView boardView;
    [SerializeField] private InputController inputController;
    private CameraController cameraController;

    private BoardModel boardModel;
    
    private PlayerState whitePlayer;
    private PlayerState blackPlayer;
    private PlayerState currentPlayer;

    private PhaseState currentPhase = PhaseState.Placement;
    private int placementCountInPhase = 0; // 現在の設置フェーズでの総設置回数 (0..3)

    // 勝敗状態
    private bool isGameOver = false;
    private PlayerState winner = null;

    // AI設定 (0: Manual, 1: Random AI, 2: Custom AI)
    private bool isGameStarted = false;
    private int whitePlacementAIIndex = 0;
    private int whiteRetrievalAIIndex = 0;
    private int blackPlacementAIIndex = 0;
    private int blackRetrievalAIIndex = 0;

    private RandomAI randomAI = new RandomAI();
    private CustomPlacementAI customPlacementAI = new CustomPlacementAI();
    private CustomRetrievalAI customRetrievalAI = new CustomRetrievalAI();

    private bool isRunningAI = false;

    // UI用のスタイル
    private GUIStyle panelStyle;
    private GUIStyle textStyle;

    private class PlayerState
    {
        public BallColor Color;
        public int PlacementCount = 0; // 各プレイヤーの設置回数 (0..3)
        public int RetrievalRights = 1; // 設置フェーズ開始時に+1されるため初期値は1
    }

    void Start()
    {
        boardModel = new BoardModel();

        whitePlayer = new PlayerState { Color = BallColor.White };
        blackPlayer = new PlayerState { Color = BallColor.Black };
        currentPlayer = whitePlayer;

        // Missing参照（偽Null）を回避するため、GetComponentを優先して取得・再アタッチする
        boardView = GetComponent<BoardView>();
        if (boardView == null)
        {
            boardView = FindObjectOfType<BoardView>();
        }
        if (boardView == null)
        {
            boardView = gameObject.AddComponent<BoardView>();
        }

        inputController = GetComponent<InputController>();
        if (inputController == null)
        {
            inputController = FindObjectOfType<InputController>();
        }
        if (inputController == null)
        {
            inputController = gameObject.AddComponent<InputController>();
        }

        // カメラコントローラーの確実な自動取得・アタッチ
        InitializeCameraController();

        inputController.OnBoardClicked += HandleBoardClick;
        
        // 盤面スロットを初期化
        boardView.RecreateBoardAndSlots();
        boardView.UpdateSlotVisibility(boardModel);
    }

    private void InitializeCameraController()
    {
        cameraController = FindObjectOfType<CameraController>();
        if (cameraController == null)
        {
            var mainCam = Camera.main;
            if (mainCam != null)
            {
                cameraController = mainCam.GetComponent<CameraController>();
                if (cameraController == null)
                {
                    cameraController = mainCam.gameObject.AddComponent<CameraController>();
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (inputController != null)
        {
            inputController.OnBoardClicked -= HandleBoardClick;
        }
    }

    // 100%確実に表示される OnGUI による情報表示
    void OnGUI()
    {
        if (panelStyle == null)
        {
            InitializeStyles();
        }

        // 実行時に参照が切れた場合への安全対策
        if (cameraController == null)
        {
            InitializeCameraController();
        }

        // 画面右上にカメラ回転・リセットボタンを配置 (常に利用可能)
        float camBtnX = Screen.width - 160;
        GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        GUIStyle camBtnStyle = new GUIStyle(GUI.skin.button);
        camBtnStyle.fontSize = 13;
        camBtnStyle.normal.textColor = Color.white;

        if (GUI.RepeatButton(new Rect(camBtnX, 20, 140, 35), "◀ カメラ左回転", camBtnStyle))
        {
            if (cameraController != null) cameraController.Rotate(-90f * Time.deltaTime);
        }
        if (GUI.RepeatButton(new Rect(camBtnX, 60, 140, 35), "カメラ右回転 ▶", camBtnStyle))
        {
            if (cameraController != null) cameraController.Rotate(90f * Time.deltaTime);
        }
        if (GUI.Button(new Rect(camBtnX, 100, 140, 35), "カメラ位置リセット", camBtnStyle))
        {
            if (cameraController != null) cameraController.ResetCamera();
        }

        // ゲーム未開始時はセットアップ画面を表示
        if (!isGameStarted)
        {
            DrawSetupUI();
            return;
        }

        // 見切れないようサイズを拡張 (320 x 180)
        GUILayout.BeginArea(new Rect(20, 20, 320, 180), panelStyle);
        
        string info = "";

        if (isGameOver)
        {
            string winnerStr = (winner.Color == BallColor.White) 
                ? "<color=#FFFFFF><b>白 (WHITE)</b></color>" 
                : "<color=#808080><b>黒 (BLACK)</b></color>";
            info = $"<b>【GAME OVER - ゲーム終了】</b>\n\n" +
                   $"勝者: {winnerStr} の勝利です！\n\n" +
                   $"ピラミッドの最上段を制しました。";
        }
        else
        {
            string phaseStr = (currentPhase == PhaseState.Placement) 
                ? $"設置フェーズ (白: {whitePlayer.PlacementCount}/3, 黒: {blackPlayer.PlacementCount}/3)" 
                : "回収フェーズ";
            
            string turnStr = (currentPlayer.Color == BallColor.White) 
                ? "<color=#FFFFFF><b>白 (WHITE)</b></color>" 
                : "<color=#808080><b>黒 (BLACK)</b></color>";

            info = $"<b>【PYLOS GAME INFO】</b>\n\n" +
                   $"<color=#00FFFF>◆ ターン:</color> {turnStr}\n" +
                   $"<color=#FFD700>◆ フェーズ:</color> {phaseStr}\n" +
                   $"<color=#ADFF2F>◆ 回収権 (白):</color> {whitePlayer.RetrievalRights} 個\n" +
                   $"<color=#ADFF2F>◆ 回収権 (黒):</color> {blackPlayer.RetrievalRights} 個";
        }

        GUILayout.Label(info, textStyle);
        GUILayout.EndArea();

        // 回収フェーズ中のみ、「回収をスキップ」ボタンを配置 (人間のターンのみ)
        if (!isGameOver && currentPhase == PhaseState.Retrieval)
        {
            bool isCurrentPlayerAI = false;
            if (currentPlayer.Color == BallColor.White)
            {
                isCurrentPlayerAI = (whiteRetrievalAIIndex != 0);
            }
            else
            {
                isCurrentPlayerAI = (blackRetrievalAIIndex != 0);
            }

            if (!isCurrentPlayerAI)
            {
                GUI.backgroundColor = new Color(0.7f, 0.3f, 0.3f, 0.9f);
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = 14;
                buttonStyle.normal.textColor = Color.white;

                if (GUI.Button(new Rect(20, 210, 140, 35), "回収をスキップ", buttonStyle))
                {
                    HandleSkipAction();
                }
            }
        }

        // ゲームオーバー時は「もう一度遊ぶ」ボタンを表示
        if (isGameOver)
        {
            GUI.backgroundColor = new Color(0.2f, 0.6f, 0.2f, 0.9f);
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 14;
            buttonStyle.normal.textColor = Color.white;

            if (GUI.Button(new Rect(20, 210, 140, 35), "もう一度遊ぶ", buttonStyle))
            {
                ResetGame();
            }
        }
    }

    private void InitializeStyles()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f, 0.85f));
        texture.Apply();

        panelStyle = new GUIStyle();
        panelStyle.normal.background = texture;
        panelStyle.padding = new RectOffset(15, 15, 15, 15);

        textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.white;
        textStyle.fontSize = 15;
        textStyle.richText = true; // マークアップタグを有効化
    }

    private void DrawSetupUI()
    {
        // AI設定画面用サイズ (320 x 380)
        GUILayout.BeginArea(new Rect(20, 20, 320, 380), panelStyle);

        GUILayout.Label("<b>【PYLOS GAME SETUP】</b>", textStyle);
        GUILayout.Space(10);

        string[] aiOptions = new string[] { "Manual", "Random", "Custom" };

        // 白の設定
        GUILayout.Label("<color=#FFFFFF><b>◆ 白 (WHITE)</b></color>", textStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label("設置:", textStyle, GUILayout.Width(45));
        whitePlacementAIIndex = DrawSelectionButtons(whitePlacementAIIndex, aiOptions);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("回収:", textStyle, GUILayout.Width(45));
        whiteRetrievalAIIndex = DrawSelectionButtons(whiteRetrievalAIIndex, aiOptions);
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        // 黒の設定
        GUILayout.Label("<color=#808080><b>◆ 黒 (BLACK)</b></color>", textStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label("設置:", textStyle, GUILayout.Width(45));
        blackPlacementAIIndex = DrawSelectionButtons(blackPlacementAIIndex, aiOptions);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("回収:", textStyle, GUILayout.Width(45));
        blackRetrievalAIIndex = DrawSelectionButtons(blackRetrievalAIIndex, aiOptions);
        GUILayout.EndHorizontal();

        GUILayout.Space(25);

        // 開始ボタン
        GUI.backgroundColor = new Color(0.2f, 0.6f, 0.2f, 0.9f);
        if (GUILayout.Button("対戦開始", GUILayout.Height(40)))
        {
            isGameStarted = true;
            TriggerAIIfNeeded();
        }
        GUI.backgroundColor = Color.white; // リセット

        GUILayout.EndArea();
    }

    private int DrawSelectionButtons(int selectedIndex, string[] options)
    {
        int newIndex = selectedIndex;
        for (int i = 0; i < options.Length; i++)
        {
            if (i == selectedIndex)
            {
                // 選択されているボタンは目立つアクアブルー
                GUI.backgroundColor = new Color(0.1f, 0.8f, 0.8f, 1.0f);
            }
            else
            {
                // 未選択のボタンは暗いグレー
                GUI.backgroundColor = new Color(0.35f, 0.35f, 0.35f, 1.0f);
            }

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.fontSize = 11;
            btnStyle.normal.textColor = Color.white;
            btnStyle.active.textColor = Color.white;
            btnStyle.hover.textColor = Color.white;

            if (GUILayout.Button(options[i], btnStyle, GUILayout.Height(25), GUILayout.Width(78)))
            {
                newIndex = i;
            }
        }
        GUI.backgroundColor = Color.white; // リセット
        return newIndex;
    }

    private void HandleBoardClick(PylosCoordinate coord)
    {
        if (isGameOver || !isGameStarted) return; // ゲーム未開始・終了後は入力を受け付けない
        if (isRunningAI) return; // AIが動作中のときは人間の入力を無視する

        // 現在の手番がAIかどうか確認し、AI手番なら人間の操作は無視する
        bool isCurrentPlayerAI = false;
        if (currentPlayer.Color == BallColor.White)
        {
            isCurrentPlayerAI = (currentPhase == PhaseState.Placement) 
                ? (whitePlacementAIIndex != 0) 
                : (whiteRetrievalAIIndex != 0);
        }
        else
        {
            isCurrentPlayerAI = (currentPhase == PhaseState.Placement) 
                ? (blackPlacementAIIndex != 0) 
                : (blackRetrievalAIIndex != 0);
        }
        
        if (isCurrentPlayerAI) return;

        if (currentPhase == PhaseState.Placement)
        {
            HandlePlacementClick(coord);
        }
        else
        {
            HandleRetrievalClick(coord);
        }
    }

    private void HandlePlacementClick(PylosCoordinate coord)
    {
        if (boardModel.BallGrid[coord.Level, coord.X, coord.Y] != BallColor.None) return;

        if (coord.Level > 0)
        {
            int underL = coord.Level - 1;
            bool hasBase = 
                boardModel.BallGrid[underL, coord.X, coord.Y] != BallColor.None &&
                boardModel.BallGrid[underL, coord.X + 1, coord.Y] != BallColor.None &&
                boardModel.BallGrid[underL, coord.X, coord.Y + 1] != BallColor.None &&
                boardModel.BallGrid[underL, coord.X + 1, coord.Y + 1] != BallColor.None;

            if (!hasBase) return;
        }

        boardModel.PlaceBall(coord, currentPlayer.Color);
        if (boardView != null)
        {
            boardView.PlaceBall(coord, currentPlayer.Color);
        }

        // 最上段(Level 3)に球を置いたプレイヤーが勝利
        if (coord.Level == 3)
        {
            isGameOver = true;
            winner = currentPlayer;
            Debug.Log($"Game Over! Winner: {winner.Color}");
            if (boardView != null)
            {
                boardView.UpdateSlotVisibility(boardModel); // 全非表示にする
            }
            return;
        }

        // 2x2 または 1列完成チェック
        bool squareFormed = CheckSquareFormation(coord, currentPlayer.Color);
        bool lineFormed = CheckLineFormation(coord, currentPlayer.Color);

        if (squareFormed || lineFormed)
        {
            currentPlayer.RetrievalRights += 1;
            Debug.Log($"Pattern formed! {currentPlayer.Color} RetrievalRights = {currentPlayer.RetrievalRights}");
        }

        currentPlayer.PlacementCount += 1;

        SwitchTurn();
    }

    private void HandleRetrievalClick(PylosCoordinate coord)
    {
        if (boardModel.BallGrid[coord.Level, coord.X, coord.Y] != currentPlayer.Color) return;

        if (IsSupportingOthers(coord))
        {
            Debug.LogWarning("Cannot retrieve - This ball supports other balls.");
            return;
        }

        boardModel.RemoveBall(coord);
        if (boardView != null)
        {
            boardView.RemoveBall(coord);
        }

        currentPlayer.RetrievalRights -= 1;
        
        SwitchRetrievalTurn();
    }

    private void HandleSkipAction()
    {
        if (currentPhase != PhaseState.Retrieval) return;
        
        currentPlayer.RetrievalRights -= 1;
        
        SwitchRetrievalTurn();
    }

    private void SwitchTurn()
    {
        // 白と黒がそれぞれ3回設置したか？
        if (whitePlayer.PlacementCount >= 3 && blackPlayer.PlacementCount >= 3)
        {
            currentPhase = PhaseState.Retrieval;
            
            // 設置フェーズの最後は黒の設置3回目なので、次は白の番
            currentPlayer = whitePlayer;
            
            // もし白の回収権が0であれば黒に渡す
            if (currentPlayer.RetrievalRights <= 0 && blackPlayer.RetrievalRights > 0)
            {
                currentPlayer = blackPlayer;
            }
        }
        else
        {
            currentPhase = PhaseState.Placement;
            currentPlayer = (currentPlayer == whitePlayer) ? blackPlayer : whitePlayer;
        }

        if (boardView != null)
        {
            boardView.UpdateSlotVisibility(boardModel);
        }

        TriggerAIIfNeeded();
    }

    private void SwitchRetrievalTurn()
    {
        // 両者の回収権が0になったか？
        if (whitePlayer.RetrievalRights <= 0 && blackPlayer.RetrievalRights <= 0)
        {
            // 設置フェーズを再開
            currentPhase = PhaseState.Placement;
            
            // 各プレイヤーの設置カウントをリセット
            whitePlayer.PlacementCount = 0;
            blackPlayer.PlacementCount = 0;
            
            // 設置フェーズ開始なので回収権を1ずつ与える
            whitePlayer.RetrievalRights += 1;
            blackPlayer.RetrievalRights += 1;
            
            // ターンは白から再開
            currentPlayer = whitePlayer;
            
            Debug.Log("Both players finished retrieval. Resuming placement phase!");
        }
        else
        {
            // 相手のターンへ
            var opponent = (currentPlayer == whitePlayer) ? blackPlayer : whitePlayer;
            if (opponent.RetrievalRights > 0)
            {
                currentPlayer = opponent;
            }
            // 相手の回収権が0の場合は交代せず、自分のターンを維持（自分には回収権が残っているため）
        }

        if (boardView != null)
        {
            boardView.UpdateSlotVisibility(boardModel);
        }

        TriggerAIIfNeeded();
    }

    private void ResetGame()
    {
        StopAllCoroutines();
        isRunningAI = false;
        isGameStarted = false; // 設定画面に戻す

        boardModel = new BoardModel();
        
        whitePlayer.PlacementCount = 0;
        whitePlayer.RetrievalRights = 1;
        
        blackPlayer.PlacementCount = 0;
        blackPlayer.RetrievalRights = 1;
        
        currentPlayer = whitePlayer;
        currentPhase = PhaseState.Placement;
        placementCountInPhase = 0;
        
        isGameOver = false;
        winner = null;

        if (boardView != null)
        {
            boardView.RecreateBoardAndSlots();
            boardView.UpdateSlotVisibility(boardModel);
        }
        
        if (cameraController != null)
        {
            cameraController.ResetCamera();
        }
        
        Debug.Log("Game has been reset.");
    }

    private bool CheckSquareFormation(PylosCoordinate coord, BallColor color)
    {
        int l = coord.Level;
        int size = 4 - l;

        int[,] offsets = new int[,] {
            { -1, -1 },
            { 0, -1 },
            { -1, 0 },
            { 0, 0 }
        };

        for (int i = 0; i < 4; i++)
        {
            int startX = coord.X + offsets[i, 0];
            int startY = coord.Y + offsets[i, 1];

            if (startX >= 0 && startX + 1 < size && startY >= 0 && startY + 1 < size)
            {
                if (boardModel.BallGrid[l, startX, startY] == color &&
                    boardModel.BallGrid[l, startX + 1, startY] == color &&
                    boardModel.BallGrid[l, startX, startY + 1] == color &&
                    boardModel.BallGrid[l, startX + 1, startY + 1] == color)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckLineFormation(PylosCoordinate coord, BallColor color)
    {
        int l = coord.Level;
        int size = 4 - l;
        if (size < 2) return false;

        bool rowComplete = true;
        for (int x = 0; x < size; x++)
        {
            if (boardModel.BallGrid[l, x, coord.Y] != color)
            {
                rowComplete = false;
                break;
            }
        }

        bool colComplete = true;
        for (int y = 0; y < size; y++)
        {
            if (boardModel.BallGrid[l, coord.X, y] != color)
            {
                colComplete = false;
                break;
            }
        }

        return rowComplete || colComplete;
    }

    private bool IsSupportingOthers(PylosCoordinate coord)
    {
        int upL = coord.Level + 1;
        if (upL >= 4) return false;

        int upSize = 4 - upL;

        int[] dx = { 0, -1, 0, -1 };
        int[] dy = { 0, 0, -1, -1 };

        for (int i = 0; i < 4; i++)
        {
            int upX = coord.X + dx[i];
            int upY = coord.Y + dy[i];

            if (upX >= 0 && upX < upSize && upY >= 0 && upY < upSize)
            {
                if (boardModel.BallGrid[upL, upX, upY] != BallColor.None)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void TriggerAIIfNeeded()
    {
        if (isGameOver || !isGameStarted) return;
        if (isRunningAI) return;

        bool isAI = false;
        if (currentPlayer.Color == BallColor.White)
        {
            isAI = (currentPhase == PhaseState.Placement) 
                ? (whitePlacementAIIndex != 0) 
                : (whiteRetrievalAIIndex != 0);
        }
        else
        {
            isAI = (currentPhase == PhaseState.Placement) 
                ? (blackPlacementAIIndex != 0) 
                : (blackRetrievalAIIndex != 0);
        }

        if (isAI)
        {
            StartCoroutine(RunAIActionCoroutine());
        }
    }

    private IEnumerator RunAIActionCoroutine()
    {
        isRunningAI = true;
        yield return new WaitForSeconds(0.6f);

        if (isGameOver || !isGameStarted)
        {
            isRunningAI = false;
            yield break;
        }

        if (currentPhase == PhaseState.Placement)
        {
            IAIPlacementAlgorithm placementAI = null;
            int aiIdx = (currentPlayer.Color == BallColor.White) ? whitePlacementAIIndex : blackPlacementAIIndex;
            if (aiIdx == 1) placementAI = randomAI;
            else if (aiIdx == 2) placementAI = customPlacementAI;

            if (placementAI != null)
            {
                PylosCoordinate coord = placementAI.DecidePlacement(boardModel, currentPlayer.Color);
                if (coord.Level != -1)
                {
                    HandlePlacementClick(coord);
                }
                else
                {
                    Debug.LogWarning($"AI ({currentPlayer.Color}) could not find a placement slot. Switching turn.");
                    SwitchTurn();
                }
            }
        }
        else // Retrieval Phase
        {
            IAIRetrievalAlgorithm retrievalAI = null;
            int aiIdx = (currentPlayer.Color == BallColor.White) ? whiteRetrievalAIIndex : blackRetrievalAIIndex;
            if (aiIdx == 1) retrievalAI = randomAI;
            else if (aiIdx == 2) retrievalAI = customRetrievalAI;

            if (retrievalAI != null)
            {
                PylosCoordinate coord = retrievalAI.DecideRetrieval(boardModel, currentPlayer.Color);
                if (coord.Level == -1)
                {
                    HandleSkipAction();
                }
                else
                {
                    HandleRetrievalClick(coord);
                }
            }
        }

        isRunningAI = false;
        TriggerAIIfNeeded();
    }
}
