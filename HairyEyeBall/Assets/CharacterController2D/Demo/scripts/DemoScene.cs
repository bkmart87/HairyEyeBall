using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DemoScene : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;
	//private HashSet<KeyCode> _usedKeys;

	KeyCode[] _keys = {KeyCode.Tab,
		/*KeyCode.Space,
		KeyCode.UpArrow,
		KeyCode.DownArrow,
		KeyCode.RightArrow,
		KeyCode.LeftArrow,
		KeyCode.Alpha0,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9,
		KeyCode.Quote,
		KeyCode.Comma,
		KeyCode.Minus,
		KeyCode.Period,
		KeyCode.Slash,
		KeyCode.Semicolon,
		KeyCode.Equals,
		KeyCode.LeftBracket,
		KeyCode.Backslash,
		KeyCode.RightBracket,
		KeyCode.BackQuote,*/
		KeyCode.A,
		KeyCode.B,
		KeyCode.C,
		KeyCode.D,
		KeyCode.E,
		KeyCode.F,
		KeyCode.G,
		KeyCode.H,
		KeyCode.I,
		KeyCode.J,
		KeyCode.K,
		KeyCode.L,
		KeyCode.M,
		KeyCode.N,
		KeyCode.O,
		KeyCode.P,
		KeyCode.Q,
		KeyCode.R,
		KeyCode.S,
		KeyCode.T,
		KeyCode.U,
		KeyCode.V,
		KeyCode.W,
		KeyCode.X,
		KeyCode.Y,
		KeyCode.Z};

		KeyCode leftMotion;
		KeyCode rightMotion;
		KeyCode jumpMotion;

	void RandomnizeKeys ()
	{
		int chosenIndex = Random.Range (0, _keys.Length);
		leftMotion = _keys [chosenIndex];
		chosenIndex = Random.Range (0, _keys.Length);
		rightMotion = _keys[chosenIndex];

		while (leftMotion == rightMotion) {
			rightMotion = _keys[Random.Range (0, _keys.Length)];
		}

		chosenIndex = Random.Range (0, _keys.Length);
		jumpMotion = _keys[chosenIndex];

		while (leftMotion == jumpMotion || rightMotion == jumpMotion) {
			jumpMotion = _keys[Random.Range (0, _keys.Length)];
				}
		print (leftMotion);
		print (rightMotion);
		print (jumpMotion);
	}

	void Awake()
	{
		//_usedKeys = new HashSet<KeyCode>();
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
	}
	IEnumerator RandomnizeOnInterval (float time){
				while (true) {
						RandomnizeKeys ();
						yield return new WaitForSeconds (time);
						
				}
		}

	void Start()
	{
		StartCoroutine (RandomnizeOnInterval(60F));
		}

	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent( Collider2D col )
	{
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

		if( _controller.isGrounded )
			_velocity.y = 0;

		if( Input.GetKey( rightMotion ) )
		{
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Run" ) );
		}
		else if( Input.GetKey( leftMotion ) )
		{
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Run" ) );
		}
		else
		{
			normalizedHorizontalSpeed = 0;

			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Idle" ) );
		}


		// we can only jump whilst grounded
		if( _controller.isGrounded && Input.GetKeyDown( jumpMotion ) )
		{
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
			_animator.Play( Animator.StringToHash( "Jump" ) );
		}


		// apply horizontal speed smoothing it
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		_controller.move( _velocity * Time.deltaTime );
	}

}
