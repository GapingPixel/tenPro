using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallStates
{
    Service,
    Main,
    Out
};

public class Ball : MonoBehaviour
{
    public float initialVelocity = 0.9f;

    private Rigidbody2D _ballrb;
    public Sprite BigBall;
    public Sprite small_ball;
    public SpriteRenderer spriteRender;
    public GameObject ShadowObject;
    public BallStates BallState = BallStates.Main;

    float SwingVelocity = 6f;
    [HideInInspector] public float _z = 0f; // 0-4.9 Low Height / 2.5-10Medium Height / 10++ High Height - Net Height 5 
    private readonly float _maxzSpeed = 1.0f;
    private float _gravity = 0.03f;
    private float _zSpeed = 0.0f;
    private float _realY = 0.0f;
    private float _speed = 1.0f;
    private bool _firstFrame = true;
    private bool _ballOnNet = false;
    [HideInInspector] public int _bounceCount = 0;
    private bool _upWall = false;

    public GameObject Player;
    private PlayerController _playerScript;
    [HideInInspector] public Vector2 ServiceVector;
    [HideInInspector] public float ServiceMultiplayer;
    [HideInInspector] public bool FastService;

    [HideInInspector] public float Vspd = 90f;

    // Start is called before the first frame update
    void Awake()
    {
        //spriteRender = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        spriteRender.drawMode = SpriteDrawMode.Sliced;
    }

    void Start()
    {
        _ballrb = GetComponent<Rigidbody2D>();
        _playerScript = Player.GetComponent<PlayerController>();
        //Instantiate(BallzGameobject, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        _zSpeed = _maxzSpeed;
    }

    public void Launch()
    {
        switch (FastService)
        {
            case true:
                _ballrb.velocity = ServiceVector * ( initialVelocity + ServiceMultiplayer);
                _zSpeed = _maxzSpeed*.75f;
                _z = 0.1f;
                break;

            case false:
                _ballrb.velocity = ServiceVector *( (initialVelocity*.75f) + ServiceMultiplayer);
                _zSpeed = _maxzSpeed*.74f;
                _z = 0.1f;
                break;
        }
        /*
        print("Left Paddle left: " + (Vector2) (Quaternion.Euler(0, 0, 301 + 5) * (Vector2.right * 24f)));
        print("Left Paddle right: " + (Vector2) (Quaternion.Euler(0, 0, 301 - 5) * (Vector2.right * 24f)));
        
        print("Right Paddle left: " + (Vector2) (Quaternion.Euler(0, 0, 239 + 5) * (Vector2.right * 24f)));
        print("Right Paddle right: " + (Vector2) (Quaternion.Euler(0, 0, 239 - 5) * (Vector2.right * 24f)));*/
        //print((Vector2) (Quaternion.Euler(0, 0, 301 + 5) * (Vector2.right * 24f)));
        /*print(new Vector2(12f, 20f) * SwingVelocity);
        
        print("Current Angle " + Mathf.Atan2(20,12) * Mathf.Rad2Deg);
        print((Vector2)(Quaternion.Euler(0,0,59) * (Vector2.right*SwingVelocity*24)));*/
    }

    void Service()
    {
        _ballrb.velocity = new Vector2(0, Vspd);
        spriteRender.sprite = small_ball;
    }

    private void onEnable()
    {
        Vspd = 90f;
    }

    private void ResetShootBall()
    {
        if (transform.position.x < 45.0f || transform.position.x > 210.0f || ShadowObject.transform.position.y > 240.0f)
        {
            float x = Random.Range(50, 205);
            float y = Random.Range(144, 179);
            _z = 4;
            transform.position = new Vector2(x, y);
            float check = Random.Range(0f, 1f);
            _bounceCount = 0;
            if (y > 160) //Baseline
            {
                if (check <= 0.25f)
                {
                    //Slice
                    _ballrb.velocity = new Vector2(-12f, -20f) * SwingVelocity * .8f;
                    _zSpeed = _maxzSpeed * .8f;
                }
                else if (check <= 0.5f)
                {
                    //Flat
                    _ballrb.velocity = new Vector2(-12f, -20f) * SwingVelocity;
                    _zSpeed = _maxzSpeed;
                }
                else if (check <= 0.75f)
                {
                    //Top Spin
                    _ballrb.velocity = new Vector2(12f, -20f) * SwingVelocity * .9f;
                    _zSpeed = _maxzSpeed * 1.5f;
                }
                else if (check <= 1f)
                {
                    //Lob
                    _ballrb.velocity = new Vector2(12f, -20f) * SwingVelocity * .5f;
                    _zSpeed = _maxzSpeed * 2;
                }
            }
            else //Net
            {
                if (check <= 0.25f)
                {
                    //Light Volley
                    _ballrb.velocity = new Vector2(-12f, -20f) * SwingVelocity * 0.8f;
                    _zSpeed = _maxzSpeed;
                }
                else if (check <= 0.5f)
                {
                    //Strong Volley
                    _ballrb.velocity = new Vector2(12f, -20f) * SwingVelocity * 1.1f;
                    _zSpeed = _maxzSpeed * .7f;
                }
                else if (check <= 0.75f)
                {
                    //Top Spin
                    _ballrb.velocity = new Vector2(-12f, -20f) * SwingVelocity * .9f;
                    _zSpeed = _maxzSpeed * 1.5f;
                }
                else if (check <= 1f)
                {
                    //Lob
                    _ballrb.velocity = new Vector2(12f, -20f) * SwingVelocity * .5f;
                    _zSpeed = _maxzSpeed * 2;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        switch (BallState)
        {
            case BallStates.Main:
                ResetShootBall();
                if (_firstFrame)
                {
                    _firstFrame = false;
                    _realY = transform.position.y;
                }

                /*
                _z -= _gravity;
                _ballrb.velocity -= 0.007f * _ballrb.velocity;
                */
                if (_z <= 0)
                {
                    switch (_bounceCount)
                    {
                        case 0:
                            _z = 0.01f;
                            _zSpeed = _maxzSpeed;
                            _bounceCount++;
                            break;

                        case 1:
                            _z = 0.01f;
                            _zSpeed = _maxzSpeed / 3;
                            _bounceCount++;
                            break;

                        case 2:
                            _z = .01f;
                            _zSpeed = _maxzSpeed / 4.5f;
                            _bounceCount++;
                            break;

                        case 3:
                            _z = 0f;
                            _zSpeed = 0;
                            //_ballrb.velocity = new Vector2(0.0f,0.0f); 
                            break;
                    }
                }
                else
                {
                    _zSpeed -= _gravity;
                    _z += _zSpeed;
                }

                if (!_firstFrame)
                {
                    _realY = transform.position.y - _zSpeed;
                }

                //_YwithZ = _realY+_z;
                //print("Z: " + _z);
                //print("Z SPEED " + _zSpeed);
                //print("real Y " + _realY);
                if (_speed > 0)
                {
                    _speed -= 0.00002f;
                }
                else
                {
                    _speed = 0;
                }

                _ballrb.velocity = _ballrb.velocity * _speed;
                transform.position = new Vector2(transform.position.x, transform.position.y + _zSpeed);
                ShadowObject.transform.position = new Vector2(transform.position.x, transform.position.y - _z);

                //Net
                if (transform.position.y <= 136 && transform.position.y >= 129 && _z < 8 && !_ballOnNet)
                {
                    _ballrb.velocity = new Vector2(0, 0);
                    _ballOnNet = true;
                }

                //Update Sprites
                if (_z >= 8)
                {
                    spriteRender.sprite = BigBall;
                    spriteRender.size = new Vector2(.8f+_z/100, .8f+_z/100);
                }
                else
                {
                    spriteRender.sprite = small_ball;
                    spriteRender.size = new Vector2(1, 1);
                }

                break;

            case BallStates.Service:
                Vspd -= 1.5f;
                _ballrb.velocity = new Vector2(0, Vspd);
                spriteRender.sprite = small_ball;
                ShadowObject.SetActive(false);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _speed = 1;
        if (other.gameObject.CompareTag("Wall"))
        {
            _bounceCount = 0;
            /*float x1z = 3.0f;
            float y1z = 6f;
            float x2 = 6.0f;
            float y2 = 10f;*/
            float check = Random.Range(0f, 1f);
            //print("TUTURU");
            if (transform.position.y > 141f && !_upWall) //Up
            {
                _upWall = true;
                if (check <= 1f) //High Z
                {
                    //_ballrb.AddForce(new Vector2(-(x1z * SwingVelocity), -(y1z * SwingVelocity)), ForceMode2D.Impulse);
                    //_ballrb.velocity = new Vector2(-(3f * SwingVelocity), (6f * SwingVelocity) );*/
                    _ballrb.velocity = new Vector2(-(3f * SwingVelocity), -36);
                    _zSpeed = _maxzSpeed * 3.0f;
                }
                else //Normal Z
                {
                    _ballrb.velocity = new Vector2(-(12f * SwingVelocity), -(20f * SwingVelocity));
                    _zSpeed = _maxzSpeed * .8f;
                }
            }
            else if (transform.position.y < 141f && _upWall) //Down
            {
                _upWall = false;
                if (check <= .5f) //High Z
                {
                    _ballrb.velocity = new Vector2((3f * SwingVelocity), (6f * SwingVelocity));
                    _zSpeed = _maxzSpeed * 2.0f;
                }
                else //Normal Z
                {
                    //_ballrb.AddForce(new Vector2((x2 * SwingVelocity), (y2 * SwingVelocity)), ForceMode2D.Impulse);
                    _ballrb.velocity = new Vector2((12f * SwingVelocity), (20f * SwingVelocity));
                    _zSpeed = _maxzSpeed * .8f;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _speed = 1;

        if (collision.gameObject.CompareTag("PaddleRight"))
        {
            _bounceCount = 0;
            if (ShadowObject.transform.position.y > 141f) //Player 2
            {
                switch (_playerScript.CurrentZone)
                {
                    case Zone.BaseLine:
                        switch (_playerScript.SwingButton)
                        {
                            case Swings.A: //Slice
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .8f;
                                _playerScript.Player2RightSwingVector((SwingVelocity * 0.8f));
                                _zSpeed = _maxzSpeed * .8f;
                                break;

                            case Swings.B: //Flat
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity;
                                _ballrb.velocity = _playerScript.Player2RightSwingVector(SwingVelocity);
                                _zSpeed = _maxzSpeed;
                                break;

                            case Swings.X: //Top Spin
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .9f;
                                _ballrb.velocity = _playerScript.Player2RightSwingVector((SwingVelocity * 0.9f));
                                _zSpeed = _maxzSpeed * 1.5f;
                                break;

                            case Swings.Y: //Lob
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .5f;
                                _ballrb.velocity = _playerScript.Player2RightSwingVector((SwingVelocity * 0.5f));
                                _zSpeed = _maxzSpeed * 2;
                                break;
                        }

                        break;

                    case Zone.Net:
                        switch (_playerScript.SwingButton)
                        {
                            case Swings.A: //Light Volley
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * 0.8f;
                                _ballrb.velocity = _playerScript.Player2RightSwingVector((SwingVelocity * 0.8f));
                                _zSpeed = (_maxzSpeed)*.6f;
                                break;

                            case Swings.B: //Strong Volley
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * 1.1f;
                                _ballrb.velocity = _playerScript.Player2RightSwingVector((SwingVelocity * 1.1f));
                                _zSpeed = (_maxzSpeed * .7f)*.6f;
                                break;

                            case Swings.X: //Top Spin
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .9f;
                                _ballrb.velocity = _playerScript.Player2RightSwingVector((SwingVelocity * 0.9f));
                                _zSpeed = (_maxzSpeed * 1.5f)*.6f;
                                break;

                            case Swings.Y: //Lob
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .5f;
                                _ballrb.velocity = _playerScript.Player2RightSwingVector((SwingVelocity * 0.5f));
                                _zSpeed = (_maxzSpeed * 2)*.6f;
                                break;
                        }
                        break;
                }
            }
            else //Player 1
            {
                switch (_playerScript.CurrentZone)
                {
                    case Zone.BaseLine:
                        switch (_playerScript.SwingButton)
                        {
                            case Swings.A: //Slice
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .8f;
                                _playerScript.RightSwingVector((SwingVelocity * 0.8f));
                                _zSpeed = _maxzSpeed * .8f;
                                break;

                            case Swings.B: //Flat
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity;
                                _ballrb.velocity = _playerScript.RightSwingVector(SwingVelocity);
                                _zSpeed = _maxzSpeed;
                                break;

                            case Swings.X: //Top Spin
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .9f;
                                _ballrb.velocity = _playerScript.RightSwingVector((SwingVelocity * 0.9f));
                                _zSpeed = _maxzSpeed * 1.5f;
                                break;

                            case Swings.Y: //Lob
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .5f;
                                _ballrb.velocity = _playerScript.RightSwingVector((SwingVelocity * 0.5f));
                                _zSpeed = _maxzSpeed * 2;
                                break;
                        }

                        break;

                    case Zone.Net:
                        switch (_playerScript.SwingButton)
                        {
                            case Swings.A: //Light Volley
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * 0.8f;
                                _ballrb.velocity = _playerScript.RightSwingVector((SwingVelocity * 0.8f));
                                _zSpeed = (_maxzSpeed)*.6f;
                                break;

                            case Swings.B: //Strong Volley
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * 1.1f;
                                _ballrb.velocity = _playerScript.RightSwingVector((SwingVelocity * 1.1f));
                                _zSpeed = (_maxzSpeed * .7f)*.6f;
                                break;

                            case Swings.X: //Top Spin
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .9f;
                                _ballrb.velocity = _playerScript.RightSwingVector((SwingVelocity * 0.9f));
                                _zSpeed = (_maxzSpeed * 1.5f)*.6f;
                                break;

                            case Swings.Y: //Lob
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .5f;
                                _ballrb.velocity = _playerScript.RightSwingVector((SwingVelocity * 0.5f));
                                _zSpeed = (_maxzSpeed * 2)*.6f;
                                break;
                        }
                        break;
                }

                _upWall = false;
            }
        }
        else if (collision.gameObject.CompareTag("PaddleLeft"))
        {
            _bounceCount = 0;
            if (ShadowObject.transform.position.y > 141f) //Player 2
            {
                switch (_playerScript.CurrentZone)
                {
                    case Zone.BaseLine:
                        switch (_playerScript.SwingButton)
                        {
                            case Swings.A: //Slice
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .8f;
                                _playerScript.Player2LeftSwingVector((SwingVelocity * 0.8f));
                                _zSpeed = _maxzSpeed * .8f;
                                break;

                            case Swings.B: //Flat
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity;
                                _ballrb.velocity = _playerScript.Player2LeftSwingVector(SwingVelocity);
                                _zSpeed = _maxzSpeed;
                                break;

                            case Swings.X: //Top Spin
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .9f;
                                _ballrb.velocity = _playerScript.Player2LeftSwingVector((SwingVelocity * 0.9f));
                                _zSpeed = _maxzSpeed * 1.5f;
                                break;

                            case Swings.Y: //Lob
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .5f;
                                _ballrb.velocity = _playerScript.Player2LeftSwingVector((SwingVelocity * 0.5f));
                                _zSpeed = _maxzSpeed * 2;
                                break;
                        }

                        break;

                    case Zone.Net:
                        switch (_playerScript.SwingButton)
                        {
                            case Swings.A: //Light Volley
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * 0.8f;
                                _ballrb.velocity = _playerScript.Player2LeftSwingVector((SwingVelocity * 0.8f));
                                _zSpeed = (_maxzSpeed)*.6f;
                                break;

                            case Swings.B: //Strong Volley
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * 1.1f;
                                _ballrb.velocity = _playerScript.Player2LeftSwingVector((SwingVelocity * 1.1f));
                                _zSpeed = (_maxzSpeed * .7f)*.6f;
                                break;

                            case Swings.X: //Top Spin
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .9f;
                                _ballrb.velocity = _playerScript.Player2LeftSwingVector((SwingVelocity * 0.9f));
                                _zSpeed = (_maxzSpeed * 1.5f)*.6f;
                                break;

                            case Swings.Y: //Lob
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .5f;
                                _ballrb.velocity = _playerScript.Player2LeftSwingVector((SwingVelocity * 0.5f));
                                _zSpeed = (_maxzSpeed * 2)*.6f;
                                break;
                        }
                        break;
                }
            }
            else //Player 1
            {
                switch (_playerScript.CurrentZone)
                {
                    case Zone.BaseLine:
                        switch (_playerScript.SwingButton)
                        {
                            case Swings.A: //Slice
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .8f;
                                _playerScript.LeftSwingVector((SwingVelocity * 0.8f));
                                _zSpeed = _maxzSpeed * .8f;
                                break;

                            case Swings.B: //Flat
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity;
                                _ballrb.velocity = _playerScript.LeftSwingVector(SwingVelocity);
                                _zSpeed = _maxzSpeed;
                                break;

                            case Swings.X: //Top Spin
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .9f;
                                _ballrb.velocity = _playerScript.LeftSwingVector((SwingVelocity * 0.9f));
                                _zSpeed = _maxzSpeed * 1.5f;
                                break;

                            case Swings.Y: //Lob
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .5f;
                                _ballrb.velocity = _playerScript.LeftSwingVector((SwingVelocity * 0.5f));
                                _zSpeed = _maxzSpeed * 2;
                                break;
                        }

                        break;

                    case Zone.Net:
                        switch (_playerScript.SwingButton)
                        {
                            case Swings.A: //Light Volley
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * 0.8f;
                                _ballrb.velocity = _playerScript.LeftSwingVector((SwingVelocity * 0.8f));
                                _zSpeed = (_maxzSpeed)*.6f;
                                break;

                            case Swings.B: //Strong Volley
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * 1.1f;
                                _ballrb.velocity = _playerScript.LeftSwingVector((SwingVelocity * 1.1f));
                                _zSpeed = (_maxzSpeed * .7f)*.6f;
                                break;

                            case Swings.X: //Top Spin
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .9f;
                                _ballrb.velocity = _playerScript.LeftSwingVector((SwingVelocity * 0.9f));
                                _zSpeed = (_maxzSpeed * 1.5f)*.6f;
                                break;

                            case Swings.Y: //Lob
                                //_ballrb.velocity = new Vector2(-12f, 20f) * SwingVelocity * .5f;
                                _ballrb.velocity = _playerScript.LeftSwingVector((SwingVelocity * 0.5f));
                                _zSpeed = (_maxzSpeed * 2)*.6f;
                                break;
                        }
                        break;
                }

                _upWall = false;
            }
        }
        else if (collision.gameObject.CompareTag("PaddleSmash"))
        {
            _bounceCount = 0;
            _ballrb.velocity = new Vector2(0f, 15f) * SwingVelocity;
            _zSpeed = -(_maxzSpeed * .25f);
        }

        
    }
}