using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject[] Balls;
        public Vector3[] BallsPoseIni;
        public static PlayerController PlayerInstance;

        private Vector3 _inputStartPose, _inputEndPose, _inputPositionCurrent;
        private float _throwForce;
        private Rigidbody _currentBallRb;
        private Vector3 _directionSum;
        private RaycastHit _raycastHit;

        public Inputs InputCurrent;
        public float ForceMulti;
        public int CurrentBallIndex;
        public bool BallThrown;

        //parabola factors
        public Vector3 PStart, PEnd;
        public float PHeight;
        public int ParaNormalize = 1;

        public bool IsScored = false;

        public enum Inputs
        {
            Default,
            IsInputBegan,
            IsInputEnded,
            IsInputLast

        }

        // Start is called before the first frame update
        void Start()
        {
            PlayerInstance = this;
            foreach (var ball in Balls)
            {
                ball.GetComponent<Rigidbody>().isKinematic = true;
                ball.SetActive(true);
                //ball.GetComponent<Rigidbody>().useGravity = false;
            }
            BallsPoseIni = new Vector3[Balls.Length];
            for (int i = 0; i < Balls.Length; i++)
            {
                BallsPoseIni[i] = Balls[i].transform.position;
            }
            CurrentBallIndex = 0;
            _currentBallRb = Balls[CurrentBallIndex].GetComponent<Rigidbody>();
            BallThrown = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsScored)
            {
                if (CurrentBallIndex - 1 < Balls.Length)
                    GameController.Instance.GameCycle(Balls[CurrentBallIndex - 1].tag);
                IsScored = false;
            }
            if (!(CurrentBallIndex < Balls.Length))
            {

                GameController.Instance.GameOver();
                return;
            }
            
            if (!BallThrown)
            {
#if UNITY_EDITOR

                if (Input.GetMouseButtonDown(0) )
                    InputCurrent = Inputs.IsInputBegan;
                else if (Input.GetMouseButtonUp(0))
                    InputCurrent = Inputs.IsInputEnded;
                else if (Input.GetMouseButton(0))
                    InputCurrent = Inputs.IsInputLast;

                _inputPositionCurrent = Input.mousePosition;

#else

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                InputCurrent = Inputs.IsInputBegan;
            else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
                InputCurrent = Inputs.IsInputEnded;
            else if (Input.touchCount == 1)
                InputCurrent = Inputs.IsInputLast;

			_inputPositionCurrent = Input.GetTouch (0).position;

#endif
                switch (InputCurrent)
                {
                    case Inputs.IsInputBegan:
                        _inputStartPose = _inputPositionCurrent;
                        break;

                    case Inputs.IsInputEnded:
                        InputCurrent = Inputs.Default;
                        _inputEndPose = _inputPositionCurrent;
                        if (CurrentBallIndex < Balls.Length)
                            _currentBallRb = Balls[CurrentBallIndex].GetComponent<Rigidbody>();
                        _directionSum = new Vector3(_inputStartPose.x - _inputEndPose.x, _inputStartPose.y - _inputEndPose.y, _inputStartPose.y - _inputEndPose.y);
                        _directionSum = Camera.main.transform.TransformDirection(_directionSum);
                        BallThrown = true;
                        Throw(_currentBallRb);
                        
                        GetComponent<LineRenderer>().SetPosition(0, Vector3.zero);
                        GetComponent<LineRenderer>().SetPosition(1, Vector3.zero);
                        break;

                    case Inputs.IsInputLast:
                        _inputEndPose = _inputPositionCurrent;
                        Vector3[] linePoints = new Vector3[2];
                        PStart = Balls[CurrentBallIndex].transform.position;
                        Vector3 difference = (_inputStartPose - _inputEndPose)/ParaNormalize;
                        PEnd = new Vector3(Balls[CurrentBallIndex].transform.position.x + difference.x ,Balls[CurrentBallIndex].transform.position.y + difference.y, Balls[CurrentBallIndex].transform.position.z + difference.y);
                        linePoints[0] = PStart;
                        linePoints[1] = PEnd;
                        GetComponent<LineRenderer>().SetPositions(linePoints);
                        break;

                    default:
                        break;
                }
            

            }
            
        }

        void Throw(Rigidbody rb)
        {
            rb.isKinematic = false;
            rb.velocity = _directionSum*ForceMulti*Time.deltaTime;
            StartCoroutine(Cycle());
            //GameController.Instance.GameCycle();
        }

        IEnumerator Cycle()
        {
            yield return new WaitForSeconds(5);

            foreach (var t in Balls)
            {
                t.transform.Translate(Vector3.up);
            }
            //Balls[currentBallIndex].transform.position = gameObject.transform.position;
            Balls[CurrentBallIndex].SetActive(false);
            _currentBallRb.isKinematic = true;
            CurrentBallIndex++;
            BallThrown = false;
            IsScored = true;

        }

        public void StartDelay()
        {
            Invoke("StartGame", 0.5f);
        }

        void StartGame()
        {
            
            BallThrown = false;
        }
    }
}
