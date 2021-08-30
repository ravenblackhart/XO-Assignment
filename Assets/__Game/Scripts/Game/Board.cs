using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace FG {
    public class Board : MonoBehaviour {

        #region Declarations
        [SerializeField] private GameObject _tilePrefab;
        public Player playerOne;
        public Player playerTwo;

        [Header("Events")] public PlayerEvent switchPlayerEvent;

        public UnityEvent didPlaceEvent;

        private int _boardSize;
        private Tile[,] _tiles;
        private GamePiece[,] _pieces;

        private Transform _tilesTransform;
        private Transform _piecesTransform;
        
        private const float _timeBetweenMarkingWinningTiles = 0.5f;
        private const float _timeToFadeWinningTiles = 0.5f;
        public Player CurrentPlayer { get; private set; }

        public Tile this[int row, int column] => _tiles[row, column];
        
        //Added Code
        private int _score = 0;
        private GameObject _winPiece;
        private int _slotsToWin;
        private bool hasWon = false;
        
        #endregion
        public void Awake() {
            _tilesTransform = transform.GetChild(0);
            _piecesTransform = transform.GetChild(1);
            _boardSize = PlaySettings.BoardSize;
            _slotsToWin = PlaySettings.SlotsToWin;

            _tiles = new Tile[_boardSize, _boardSize];
            _pieces = new GamePiece[_boardSize, _boardSize];

            SetupTiles();

            playerOne.displayName = PlaySettings.PlayerOneName;
            playerTwo.displayName = PlaySettings.PlayerTwoName;

            SetCurrentPlayer();
            
            //Todo Remove Debug
            Debug.Log(_slotsToWin);
        }
        
        public bool PlaceMarkerOnTile(Tile tile) {
            _winPiece = CurrentPlayer.piecePrefab;
            if (ReferenceEquals(CurrentPlayer, null)) {
                return false;
            }
            
            if (ReferenceEquals(_pieces[tile.gridPosition.x, tile.gridPosition.y], null) && !hasWon && _slotsToWin <= _boardSize) {
                GamePiece piece = Instantiate(CurrentPlayer.piecePrefab,
                    new Vector3(tile.gridPosition.x, -tile.gridPosition.y),
                    Quaternion.identity, _piecesTransform)?.GetComponent<GamePiece>();

                if (!ReferenceEquals(piece, null)) {
                    piece.Owner = CurrentPlayer;
                    _pieces[tile.gridPosition.x, tile.gridPosition.y] = piece;
                }

                //didPlaceEvent.Invoke();

                UpdateScore(tile);

                SwitchPlayer();
            }

            return false;
        }
        
        void UpdateScore(Tile tile)
        {
            int x = tile.gridPosition.x;
            int y = tile.gridPosition.y;
            
            if (_pieces[x -1,y] != null &&
                _pieces[x -1, y].Owner == CurrentPlayer)
            {
                //Debug.Log($"found fwen at {tile.gridPosition.x} , {tile.gridPosition.y}");
                Debug.Log("Fwend bellongs to " + CurrentPlayer);
            }
            
            for (x = 0; x < _boardSize - 1 ; x++)
            {
            }
            
            // foreach (GamePiece piece in _pieces)
            // {
            //     if (piece == null || tile.gridPosition.x > _boardSize-1 || tile.gridPosition.y > _boardSize-1 || tile.gridPosition.x < 0 || tile.gridPosition.y < 0)
            //     {
            //         StopCoroutine(CheckWin(tile));
            //         continue;
            //     }
            //
            //     else
            //     {
            //         if (piece.name.Contains(_winPiece.name))
            //         {
            //             
            //             StartCoroutine(CheckWin(tile));
            //             
            //         }
            //         
            //         if (_score == _slotsToWin)
            //         {
            //             hasWon = true;
            //             Debug.Log($"{CurrentPlayer} Wins ! ");
            //             // StartCoroutine(MarkWinningTiles());
            //             // StartCoroutine(FadeTile());
            //             StopAllCoroutines();
            //
            //         }
            //
            //     }
            //
            // }
        }

        IEnumerator CheckWin(Tile tile)
        {
            _score ++ ;
            Debug.Log($"Current Player is {CurrentPlayer} and {_winPiece.name} current score is {_score}. I am at {tile.gridPosition.x} , {tile.gridPosition.y}. ");
            yield return null;
        }
        
        // Todo DO THING HERE
    

        private IEnumerator MarkWinningTiles(List<Vector2Int> winningTiles, Color color) {
            foreach (Vector2Int tile in winningTiles) {
                StartCoroutine(FadeTile(_tiles[tile.x, tile.y], color));
                yield return new WaitForSeconds(_timeBetweenMarkingWinningTiles);
            }
        }
        
        private IEnumerator FadeTile(Tile tile, Color targetColor) {
            SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
            float elapsedTime = 0f;
            Color startColor = tileRenderer.color;
            float fadeTime = _timeToFadeWinningTiles;
            
            while (elapsedTime < fadeTime) {
                elapsedTime += Time.deltaTime;
                float blend = Mathf.Clamp01(elapsedTime / fadeTime);
                tileRenderer.color = Color.Lerp(startColor, targetColor, blend);
                yield return null;
            }
        
            tileRenderer.color = targetColor;
        }

        private void SwitchPlayer()
        {
            _score = 0;
            CurrentPlayer = ReferenceEquals(CurrentPlayer, playerOne) ? playerTwo : playerOne;
            switchPlayerEvent.Invoke(CurrentPlayer);
        }

        private void SetupTiles() {
            for (int x = 0; x < _boardSize; x++) {
                for (int y = 0; y < _boardSize; y++) {
                    GameObject tileGo = Instantiate(_tilePrefab, new Vector3(x, -y, 0f), Quaternion.identity,
                        _tilesTransform);
                    tileGo.name = $"Tile_({x},{y})";

                    Tile tile = tileGo.GetComponent<Tile>();
                    tile.board = this;
                    tile.gridPosition = new Vector2Int(x, y);

                    _tiles[x, y] = tile;
                    
                }
                

            }
        }

        private void SetCurrentPlayer() {
            CurrentPlayer = Random.Range(0, 2) == 0 ? playerOne : playerTwo;
            switchPlayerEvent.Invoke(CurrentPlayer);
        }

    }
}