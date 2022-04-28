using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.UIElements;

public enum States
{
    Service, 
    Main,
    Swing,
    Score
};

public enum Swings
{
    A, 
    B,
    X,
    Y
};

public enum Zone
{
    BaseLine, 
    Net
};

public class PlayerController: MonoBehaviour
{
    public States PlayerState;
    [HideInInspector] public Swings SwingButton;
    [HideInInspector] public Zone CurrentZone;
    public Rigidbody2D rb;
    private float moveSpeed = 90f;
   
    Vector2 _moveDirection = Vector2.zero;
    
    public SpriteRenderer SpriteRenderer;
    //Animations for Main State
    public Sprite[] idle;
    public Sprite[] move_right;
    public Sprite[] move_up;
    public Sprite[] move_down;
    public Sprite[] swing_right;
    public Sprite[] swing_left;
    public Sprite[] swing_smash;
    //Animations for Service State
    public Sprite[] service_bounce_ball;
    public Sprite[] service_move;
    public Sprite[] service_hit_ball;
    public Sprite[] service_swing;

    int _frameCount = 0;
    int _count = 0;
    readonly int image_speed = 12;
    int image_index = 0;
    [HideInInspector] public bool Smash = false;

    public int player_number;

    public static int service_stage = 1;
    int service_stage_3_count = 125;
    int bounce_ball_anims_count = 0;
    
    public GameObject Ball;
    private Ball _ballScript;
    public GameObject PaddleRight;
    public GameObject PaddleLeft;
    public GameObject PaddleSmash;
    
    float _lastMoveY = 0f;
    float _lastMoveX = 0f;

    //Inputs 
    float moveX;
    float moveY;
    //bool swing = Input.GetKeyDown("x");
    bool buttonA;
    bool buttonB;
    bool buttonX;
    bool buttonY;
    
    void Start()
    {
        SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _ballScript = Ball.GetComponent<Ball>();
        //PlayerState = states.service;
        
        switch (player_number)
        {
            case 1:
                PlayerState = States.Service;
                image_index = 0;
                SpriteRenderer.sprite = service_bounce_ball[0];
                _frameCount = service_bounce_ball.Length;
                break;

            case 2:
                PlayerState = States.Main;
                SpriteRenderer.sprite = idle[0];
                _frameCount = idle.Length;
                //SpriteRenderer.flipX = true;
                break;
        }
        
        
    }
    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        //bool swing = Input.GetKeyDown("x");
        buttonA = Input.GetKeyDown("p");
        buttonB = Input.GetKeyDown("l");
        buttonX = Input.GetKeyDown("o");
        buttonY = Input.GetKeyDown("k");
        
        /*
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        bool swing = Input.GetKeyDown(KeyCode.Joystick1Button2);
        bool buttonA = Input.GetKeyDown(KeyCode.Joystick1Button1);
        bool buttonB = Input.GetKeyDown(KeyCode.Joystick1Button0);
        bool buttonX= Input.GetKeyDown(KeyCode.Joystick1Button3);
        bool buttonY = Input.GetKeyDown(KeyCode.Joystick1Button2);
        */
        if (player_number == 2)
        {
            /*moveX = Input.GetAxis("Horizontal2");
            moveY = Input.GetAxis("Vertical2");*/
            moveX = Input.GetAxisRaw("Horizontal2");
            moveY = Input.GetAxisRaw("Vertical2");
            buttonA = Input.GetKeyDown(KeyCode.Keypad6);
            buttonB = Input.GetKeyDown(KeyCode.Keypad2);
            buttonX = Input.GetKeyDown(KeyCode.Keypad5);
            buttonY = Input.GetKeyDown(KeyCode.Keypad1);
            /*bool buttonA = Input.GetKeyDown("u");
            bool buttonB = Input.GetKeyDown("h");
            bool buttonX= Input.GetKeyDown("y");
            bool buttonY = Input.GetKeyDown("g");*/
            //buttonA = Input.GetKeyDown("right ctrl");
        }
        
        switch (PlayerState)
        {
            case States.Main:
                
                if (moveX < 0) {
                    if (_lastMoveX >= 0)
                    {
                        image_index = 0;
                    }
                    SpriteRenderer.sprite = move_right[image_index];
                    _frameCount = move_right.Length;
                    SpriteRenderer.flipX = true;
                }  
                else if (moveX > 0)
                {
                    if (_lastMoveX <= 0)
                    {
                        image_index = 0;
                    }
                    SpriteRenderer.sprite = move_right[image_index];
                    _frameCount = move_right.Length;
                    SpriteRenderer.flipX = false;
                }

                if (moveY < 0)
                {
                    if (_lastMoveY >= 0)
                    {
                        image_index = 0;
                    }
                    SpriteRenderer.sprite = move_down[image_index];
                    _frameCount = move_down.Length;
                    /*if (player_number == 1)
                    {
                        SpriteRenderer.flipX = false;
                    }
                    else
                    {
                        SpriteRenderer.flipX = true;
                    }*/
                    
                }
                else if (moveY > 0)
                {
                    if (_lastMoveY <= 0)
                    {
                        image_index = 0;
                    }
                    SpriteRenderer.sprite = move_up[image_index];
                    _frameCount = move_up.Length;
                    /*if (player_number == 1)
                    {
                        SpriteRenderer.flipX = true;
                    }
                    else
                    {
                        SpriteRenderer.flipX = false;
                    }*/
                }
                
                if (moveY == 0 && moveX == 0)
                {
                    
                    SpriteRenderer.sprite = idle[image_index];
                    _frameCount = idle.Length;
                   
                }
            
                if((_lastMoveY == 0 && _lastMoveX == 0) && ( (moveX == 0 && moveY !=0) || (moveX != 0 && moveY == 0) ) )
                {
                    image_index = 0;
                }

                if (buttonA || buttonB || buttonX || buttonY)
                {
                    /*
                    if (_ballScript._z > 20)
                    {
                        print("Ball Z: " + _ballScript._z);
                    }
                    if (_ballScript._bounceCount == 0)
                    {
                        print("Bounce Count: " + _ballScript._bounceCount);
                    }
                    if (CurrentZone == Zone.Net)
                    {
                        print("Zone: " + CurrentZone);
                    }
                    if (Mathf.Abs(_ballScript.transform.position.x - transform.position.x) <= 8f)
                    {
                        print("X difference: " + Mathf.Abs(_ballScript.transform.position.x - transform.position.x));
                    }
                    if (Mathf.Abs(_ballScript.transform.position.y - transform.position.y) <= 30f)
                    {
                        print("Y difference: " + Mathf.Abs(_ballScript.transform.position.x - transform.position.x));
                    }
                    */
                    if (_ballScript._z > 20 && _ballScript._bounceCount == 0 && CurrentZone == Zone.Net && ( Mathf.Abs(_ballScript.transform.position.x - transform.position.x) <= 8f ) && ( Mathf.Abs(_ballScript.transform.position.y - transform.position.y) <= 35f  ) )
                    {
                        Smash = true;
                        //print("YEAH");
                    }
                    //print("Smash: " + Smash);
                    PlayerState = States.Swing;
                    _count = 0;
                    image_index = 0;

                    if (!Smash)
                    {
                        if (buttonA)
                        {
                            SwingButton = Swings.A;
                        }
                        else if (buttonB)
                        {
                            SwingButton = Swings.B;
                        }
                        else if (buttonX)
                        {
                            SwingButton = Swings.X;
                        }
                        else if (buttonY)
                        {
                            SwingButton = Swings.Y;
                        }
                        
                        if (SpriteRenderer.flipX == false)
                        {
                            Instantiate(PaddleRight,
                                new Vector2(transform.position.x - 4, transform.position.y + 2),
                                Quaternion.identity);
                        }
                        else
                        {
                            Instantiate(PaddleLeft, new Vector2(transform.position.x+4, transform.position.y + 2),
                                Quaternion.identity);
                        }
                    }
                    else
                    {
                        Instantiate(PaddleSmash, new Vector2(transform.position.x, transform.position.y),
                            Quaternion.identity);
                    }
                }
                _moveDirection = new Vector2(moveX, moveY).normalized;
                break;

            case States.Swing:
                switch (Smash)
                {
                    case false:
                        SpriteRenderer.sprite = swing_right[image_index];
                        _frameCount = swing_right.Length;
                        break;
                        
                    case true:
                        SpriteRenderer.sprite = swing_smash[image_index];
                        _frameCount = swing_smash.Length;
                        break;
                }
                break;

            case States.Service:
                switch (service_stage) {
                    case 1:
                        
                        SpriteRenderer.sprite = service_bounce_ball[image_index];
                        _frameCount = service_bounce_ball.Length;
                        
                        break;

                    case 2: 
                        SpriteRenderer.sprite = service_move[image_index];
                        _frameCount = service_move.Length;
                        _moveDirection = new Vector2(moveX, 0).normalized;
                        if (buttonA || buttonB)
                        {
                            service_stage = 3;
                            image_index = 0;
                            Ball.SetActive(true);
                            _ballScript.BallState = BallStates.Service;
                            _ballScript.Vspd = 90f;
                            Ball.transform.position = new Vector2(transform.position.x+7,transform.position.y+7);
                            //Ball._shadowCount = false;
                        }
                        break;

                    case 3:
                        SpriteRenderer.sprite = service_hit_ball[image_index];
                        _frameCount = service_hit_ball.Length;

                        if (buttonA || buttonB)
                        {
                            
                            if (service_stage_3_count >= 30)
                            {
                                service_stage = 1;
                                image_index = 0;
                                service_stage_3_count = 0;
                                Ball.SetActive(false);
                            } else if (service_stage_3_count < 30 && service_stage_3_count > 5)
                            {

                                if (buttonA)
                                {
                                    _ballScript.FastService = false;
                                }
                                else if (buttonB)
                                {
                                    _ballScript.FastService = true;
                                }
                                
                                ServiceLaunch();
                                _ballScript.ServiceMultiplayer = service_stage_3_count/7f;
                                /*print(service_stage_3_count);
                                print(_ballScript.ServiceMultiplayer);*/
                                service_stage_3_count = 0;
                                service_stage = 4;
                                _ballScript.BallState = BallStates.Main;
                                _ballScript.ShadowObject.SetActive(true);
                                _ballScript.Launch();
     
                            }
                            else
                            {
                                service_stage = 1;
                                image_index = 0;
                                service_stage_3_count = 0; 
                                Ball.SetActive(false);
                            }

                        }
                        break;

                    case 4:
                        SpriteRenderer.sprite = service_swing[image_index];
                        _frameCount = service_swing.Length;
                        
                        break;
                }
                break;
        }
        _lastMoveX = moveX;
        _lastMoveY = moveY;
    }

     private void FixedUpdate()
    {
        switch (PlayerState)
        {
            case States.Main:
            case States.Swing:
                
                _count++;
                if (_count >= image_speed)
                {
                    _count = 0;
                    image_index++;
                    if (image_index >= _frameCount)
                    {
                        image_index = 0;
                        if (PlayerState == States.Swing)
                        {
                            PlayerState = States.Main;
                            if (Smash)
                            {
                                Smash = false;
                            }
                        }
                    }
                }
                
                if (PlayerState == States.Main)
                {
                    rb.velocity = new Vector2(_moveDirection.x * moveSpeed, _moveDirection.y * moveSpeed);
                }
                else
                {
                    rb.velocity = new Vector2(_moveDirection.x, _moveDirection.y);
                }

                
                switch (player_number)
                {
                    case 1:
                        transform.position = new Vector2(Mathf.Clamp(transform.position.x, 61f, 197f), Mathf.Clamp(transform.position.y, 70f, 129f));
                        break;

                    case 2:
                        transform.position = new Vector2(Mathf.Clamp(transform.position.x, 61f, 197f), Mathf.Clamp(transform.position.y, 150f, 192f));
                        break;
                }
                break;

            case States.Service:
                
                switch (service_stage) {
                    case 1:
                        _count++;
                        if (_count >= image_speed)
                        {
                            _count = 0;
                            image_index++;
                            if (image_index >= _frameCount)
                            {
                                image_index = 0;
                                bounce_ball_anims_count++;
                                if (bounce_ball_anims_count >= 3)
                                {
                                    service_stage = 2;
                                    bounce_ball_anims_count = 0;
                                }

                            }
                        }
                        break;

                    case 2:
                        rb.velocity = new Vector2(_moveDirection.x * (moveSpeed*2), 0);
                    break;
                    
                    case 3:
                        rb.velocity = new Vector2(0, 0);

                        service_stage_3_count--;
                        if (service_stage_3_count <= 0) 
                        {
                            service_stage_3_count = 125;//1.5 secs
                            service_stage = 1;
                            Ball.SetActive(false);
                        }
                        break;

                    case 4:
                        _count++;
                        if (_count >= image_speed)
                        {
                            _count = 0;
                            image_index++;
                            if (image_index >= _frameCount)
                            {
                                image_index = 0;
                                PlayerState = States.Main;

                            }
                        }
                        break;
                }
                transform.position = new Vector2(Mathf.Clamp(transform.position.x, 80f, 120f), transform.position.y);
                break;
        }
    }
    
    void ServiceLaunch()
    {
        float _speed = 30;
        if (moveX < 0 && moveY == 0) {//Left
            _ballScript.ServiceVector = (Vector2)(Quaternion.Euler(0,0,67) * (Vector2.right*_speed)); 
        }  else if (moveX < 0 && moveY > 0) {//Left & Up
            _ballScript.ServiceVector = (Vector2)(Quaternion.Euler(0,0,62) * (Vector2.right*_speed));
        } else if (moveX == 0 && moveY > 0) {//Up
            _ballScript.ServiceVector = (Vector2)(Quaternion.Euler(0,0,57) * (Vector2.right*_speed));
        } else if (moveX == 0 && moveY < 0) {//Down
            _ballScript.ServiceVector = (Vector2)(Quaternion.Euler(0,0,52) * (Vector2.right*_speed*0.8f));
        } else if (moveX > 0 && moveY == 0) {//Right
            _ballScript.ServiceVector = (Vector2)(Quaternion.Euler(0,0,42) * (Vector2.right*_speed));
        } else if (moveX > 0 && moveY > 0) {//Right & Up
            _ballScript.ServiceVector = (Vector2)(Quaternion.Euler(0,0,47) * (Vector2.right*_speed));
                                    
        }
        else
        {
            _ballScript.ServiceVector = (Vector2)(Quaternion.Euler(0,0,52) * (Vector2.right*35));
            //_ballScript.ServiceVector = new Vector2(12f, 20f);
        }
        //print("Current Angle " + Mathf.Atan2(_ballScript.ServiceVector.y,_ballScript.ServiceVector.x) * Mathf.Rad2Deg);
    }
    
    public Vector2 RightSwingVector(float speed)
    {
        float baseAngle = 121;
        Vector2 ballVector2;
        if (moveX < 0 && moveY == 0) {//Left
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5*3) * (Vector2.right*speed*24)); 
        }  else if (moveX < 0 && moveY > 0) {//Left & Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5*2) * (Vector2.right*speed*24));
        } else if (moveX == 0 && moveY > 0) {//Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5) * (Vector2.right*speed*24));
        } else if (moveX == 0 && moveY < 0) {//Down
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle+5) * (Vector2.right*speed*24*0.8f));
        } else if (moveX > 0 && moveY == 0) {//Right
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle+5*2) * (Vector2.right*speed*24));
        } else if (moveX > 0 && moveY > 0) {//Right & Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle+5*3) * (Vector2.right*speed*24));
                                    
        }
        else
        {
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle) * (Vector2.right*speed*24));
        }

        return ballVector2;
    }
    
    public Vector2 LeftSwingVector(float speed)
    {
        float baseAngle = 59;
        Vector2 ballVector2;
        if (moveX < 0 && moveY == 0) {//Left
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle+5*3) * (Vector2.right*speed*24)); 
        }  else if (moveX < 0 && moveY > 0) {//Left & Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle+5*2) * (Vector2.right*speed*24));
        } else if (moveX == 0 && moveY > 0) {//Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle+5) * (Vector2.right*speed*24));
        } else if (moveX == 0 && moveY < 0) {//Down
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5) * (Vector2.right*speed*24*0.8f));
        } else if (moveX > 0 && moveY == 0) {//Right
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5*2) * (Vector2.right*speed*24));
        } else if (moveX > 0 && moveY > 0) {//Right & Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5*3) * (Vector2.right*speed*24));
                                    
        }
        else
        {
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle) * (Vector2.right*speed*24));
        }

        return ballVector2;
    }
    
    public Vector2 Player2LeftSwingVector(float speed)
    {
        float baseAngle = 301;
        Vector2 ballVector2;
        if (moveX < 0 && moveY == 0) {//Left
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle+5*3) * (Vector2.right*speed*24)); 
        }  else if (moveX < 0 && moveY < 0) {//Left & Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle+5*2) * (Vector2.right*speed*24));
        } else if (moveX == 0 && moveY > 0) {//Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle) * (Vector2.right*speed*24*0.8f));
        } else if (moveX == 0 && moveY < 0) {//Down
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5) * (Vector2.right*speed*24));
        } else if (moveX > 0 && moveY == 0) {//Right
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5*2) * (Vector2.right*speed*24));
        } else if (moveX > 0 && moveY < 0) {//Right & Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5*3) * (Vector2.right*speed*24));
                                    
        }
        else
        {
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle) * (Vector2.right*speed*24));
        }

        return ballVector2;
    }

    public Vector2 Player2RightSwingVector(float speed)
    {
        float baseAngle = 239;
        Vector2 ballVector2;
        if (moveX < 0 && moveY == 0) {//Left
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5*3) * (Vector2.right*speed*24)); 
        }  else if (moveX < 0 && moveY < 0) {//Left & Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5*2) * (Vector2.right*speed*24));
        } else if (moveX == 0 && moveY > 0) {//Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle-5) * (Vector2.right*speed*24*0.8f));
        } else if (moveX == 0 && moveY < 0) {//Down
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle) * (Vector2.right*speed*24));
        } else if (moveX > 0 && moveY == 0) {//Right
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle+5*2) * (Vector2.right*speed*24));
        } else if (moveX > 0 && moveY < 0) {//Right & Up
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle+5*3) * (Vector2.right*speed*24));
                                    
        }
        else
        {
            ballVector2 = (Vector2)(Quaternion.Euler(0,0,baseAngle) * (Vector2.right*speed*24));
        }

        return ballVector2;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ZoneNet"))
        {
            CurrentZone = Zone.Net;
        } else if (other.gameObject.CompareTag("ZoneBaseline"))
        {
            CurrentZone = Zone.BaseLine;
        } 
    }


}
