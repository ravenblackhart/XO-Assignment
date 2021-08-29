using UnityEngine;

namespace FG {
	public class GamePiece : MonoBehaviour {
		public Player Owner { get; set; }
		public int pieceValue = 0;
	}
}
