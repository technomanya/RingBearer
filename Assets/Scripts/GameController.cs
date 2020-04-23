using UnityEngine;
using  UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        public GameObject EndPanel;
        public Text ScoreText;
        public Image[] Stars;

        public int Score;

        private ScoreInRow _inRow = ScoreInRow.ScoreDefault;

        public enum ScoreInRow
        {
            ScoreDefault,
            ScoreTwo,
            ScoreThree
        }
        // Start is called before the first frame update
        void Start()
        {
            foreach (var VARIABLE in Stars)
            {
                VARIABLE.color = Color.white;
            }
            Score = 0;
            _inRow = ScoreInRow.ScoreDefault;
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void GameCycle(string playerTag)
        {
            if (playerTag == "ScoredPlayer")
            {
                Debug.Log("Scored");
                AddScore();
            }
            else
            {
                foreach (var VARIABLE in Stars)
                {
                    VARIABLE.color = Color.white;
                }
                _inRow = ScoreInRow.ScoreDefault;
            }
        }

        public void Restart()
        {

            GameObject[] tempBalls = PlayerController.PlayerInstance.Balls;
            Vector3[] tempBallPose = PlayerController.PlayerInstance.BallsPoseIni;
            for (int i = 0; i < PlayerController.PlayerInstance.Balls.Length; i++)
            {
                PlayerController.PlayerInstance.Balls[i].transform.position =
                    PlayerController.PlayerInstance.BallsPoseIni[i];
                PlayerController.PlayerInstance.Balls[i].transform.rotation = Quaternion.identity;
                PlayerController.PlayerInstance.Balls[i].SetActive(true);
                PlayerController.PlayerInstance.Balls[i].tag = "Player";
            }
            PlayerController.PlayerInstance.StartDelay();
        }

        public void AddScore()
        {
            
            switch (_inRow)
            {
                case ScoreInRow.ScoreDefault:
                    Score += 1;
                    Stars[0].color = Color.green;
                    _inRow = ScoreInRow.ScoreTwo;
                    break;
                case ScoreInRow.ScoreTwo:
                    Score += 2;
                    Stars[1].color = Color.green;
                    _inRow = ScoreInRow.ScoreThree;
                    break;
                case ScoreInRow.ScoreThree:
                    Score += 3;
                    Stars[2].color = Color.green;
                    break;
                default:
                    break;
            }
        }

        public void StarsControl(bool b)
        { 
            foreach (var VARIABLE in Stars)
            {
                VARIABLE.gameObject.SetActive(b);
                VARIABLE.color = Color.white;
            }
        }

        public void GameOver()
        {
            PlayerController.PlayerInstance.BallThrown = true;
            PlayerController.PlayerInstance.CurrentBallIndex = 0;
            EndPanel.SetActive(true);
            ScoreText.text = "Score: "+Score.ToString();
            StarsControl(false);
        }
    }
}
