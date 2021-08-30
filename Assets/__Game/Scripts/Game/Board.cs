using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace FG {
    public class Board : MonoBehaviour
    {

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
        private int _score = 1;
        private GameObject _winPiece;
        private int _slotsToWin;
        private bool hasWon = false;

        #endregion

        public void Awake()
        {
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

        public bool PlaceMarkerOnTile(Tile tile)
        {
            _winPiece = CurrentPlayer.piecePrefab;
            if (ReferenceEquals(CurrentPlayer, null))
            {
                return false;
            }

            if (ReferenceEquals(_pieces[tile.gridPosition.x, tile.gridPosition.y], null) && !hasWon &&
                _slotsToWin <= _boardSize)
            {
                GamePiece piece = Instantiate(CurrentPlayer.piecePrefab,
                    new Vector3(tile.gridPosition.x, -tile.gridPosition.y),
                    Quaternion.identity, _piecesTransform)?.GetComponent<GamePiece>();

                if (!ReferenceEquals(piece, null))
                {
                    piece.Owner = CurrentPlayer;
                    _pieces[tile.gridPosition.x, tile.gridPosition.y] = piece;
                }

                //didPlaceEvent.Invoke();

                UpdateScore(tile);

                SwitchPlayer();
            }

            return false;
        }


        // bool Contains(Vector2 )
        // {
        //     return false;
        //     //Todo
        //
        // }

        void UpdateScore(Tile tile)
        {
            int posX = tile.gridPosition.x;
            int posY = tile.gridPosition.y;
            
            _score = 1;
            //Horizontal
            for (int i = 1; i <= _slotsToWin - 1; i++)
            {
                // L to R
                if (posX + i > _boardSize - 1 || _pieces[posX + i, posY] == null ||
                    _pieces[posX + i, posY].Owner != CurrentPlayer) ;

                else
                {
                    _score++;
                }

                // R to L
                if (posX - i < 0 || _pieces[posX - i, posY] == null || _pieces[posX - i, posY].Owner != CurrentPlayer) ;

                else
                {
                    _score++;
                }
            }
            if (CheckWin())
            {
                return;
            }
            _score = 1;
            //Vertical
            for (int i = 1; i <= _slotsToWin -1 ; i++)
            {
                // D to U
                if (posY - i < 0 || _pieces[posX, posY - i] == null || _pieces[posX, posY - i].Owner != CurrentPlayer) ;

                else
                {
                    _score++;
                }
                
                // U to D
                if (posY + i > _boardSize - 1 || _pieces[posX, posY + i] == null ||
                    _pieces[posX, posY + i].Owner != CurrentPlayer) ;

                else
                {
                    _score++;
                }

            }
            if (CheckWin())
            {
                return;
            }

            _score = 1;
            //Diagonal TL -> BR 
            for (int i = 1; i <= _slotsToWin -1 ; i++)
            {
                // TL
                if (posY - i < 0 || posX -i < 0|| _pieces[posX - i, posY - i] == null ||
                    _pieces[posX - i, posY - i].Owner != CurrentPlayer) ;

                else
                {
                    _score++;
                }
                
                // BR
                if (posY + i > _boardSize - 1 || posX + i > _boardSize - 1 || _pieces[posX + i, posY + i] == null ||
                    _pieces[posX + i, posY + i].Owner != CurrentPlayer) ;

                else
                {
                    _score++;
                }

            }
            if (CheckWin())
            {
                return;
            }
            _score = 1;
            //Diagonal TR -> BL 
            for (int i = 1; i <= _slotsToWin -1 ; i++)
            {
                // TR
                if (posY - i < 0  || posX + i > _boardSize - 1|| _pieces[posX + i, posY - i] == null ||
                    _pieces[posX + i, posY - i].Owner != CurrentPlayer) ;
                
                else if (_pieces[posX + i, posY - i].Owner == CurrentPlayer)
                {
                    _score++;
                }

                else ;



                    // BL
                if (posY + i > _boardSize - 1 || posX - i < 0  || _pieces[posX - i, posY + i] == null ||
                    _pieces[posX - i, posY + i].Owner != CurrentPlayer) ;
                
                else if (_pieces[posX - i, posY + i].Owner == CurrentPlayer)
                {
                    _score++;
                }

                else ;
            }
            if (CheckWin())
            {
                return;
            }






            // if (_pieces[_boardSize-1, posY])
            // {
            //     
            // }
            // else
            // {
            //     for (int i = 0; i < _slotsToWin -1; i++)
            //     {
            //         if (_pieces[posX +i,posY] != null &&
            //             _pieces[posX +i, posY].Owner == CurrentPlayer )
            //         {
            //             for (int j= 0; j < _slotsToWin; j++)
            //             {
            //                 Debug.Log($"found fwen at {posX +1} , {posY.ToString()}");
            //                 _score++;
            //                 Debug.Log(_score);
            //             }
            //
            //         }
            //     }
            //     


        }

        bool CheckWin()
        {
            if (_score == _slotsToWin)
            {
                hasWon = true;
                Debug.Log($"{CurrentPlayer} Wins ! ");
                // StartCoroutine(MarkWinningTiles());
                // StartCoroutine(FadeTile());
                StopAllCoroutines();
                return true;

            }

            return false;
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
        

        // IEnumerator CheckWin(Tile tile)
        // {
        //     _score ++ ;
        //     Debug.Log($"Current Player is {CurrentPlayer} and {_winPiece.name} current score is {_score}. I am at {tile.gridPosition.x} , {tile.gridPosition.y}. ");
        //     yield return null;
        // }
        
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
            //_score = 1;
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