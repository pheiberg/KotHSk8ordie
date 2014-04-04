using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using System;

public class player_script : MonoBehaviour {
	GamePadState state;
	GamePadState prevState;
	Vector2 _speed = new Vector2(3,2);
	public bool isFacingRight = true;
	public int PlayerId;
	public int Deaths = 0;
	GameObject[] spawnPoints;
	GUIStyle textStyle;
	public AudioClip[] sounds;

	bool isPlaying = false;

	public int maxJumpCount = 2;
	int currentJumpCount = 0;

	Vector2 startPosition;


	bool RenderText{get{return isPlaying && renderer.enabled;}}

	// Use this for initialization
	void Start () {

		SetTextStyle ();
		startPosition = transform.position;
		spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
	}

	void SetTextStyle()
	{
		textStyle = new GUIStyle ();
		textStyle.normal.textColor = Color.black;
		textStyle.fontSize = 20;
		textStyle.fontStyle = FontStyle.Bold;

		switch(PlayerId){
			case 1:
				textStyle.alignment = TextAnchor.UpperRight;
				break;
			case 2:
				textStyle.alignment = TextAnchor.LowerLeft;
				break;
			case 3:
				textStyle.alignment = TextAnchor.LowerRight;
				break;
			default:
				textStyle.alignment = TextAnchor.UpperLeft;
				break;
		}
	}

	// Update is called once per frame
	void Update () {

		prevState = state;
		state = GamePad.GetState((PlayerIndex)(PlayerId));

		if(prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed) {
			isPlaying = !isPlaying;
			Restart();
		}

		renderer.enabled = isPlaying;

		if (!isPlaying)
			return;

		var movement = new Vector2 (_speed.x * state.ThumbSticks.Left.X, 0);
		
		movement *= Time.deltaTime;
		transform.Translate (movement);

		if(currentJumpCount < maxJumpCount && (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed)) {
			GameObject.FindGameObjectWithTag ("AudioPlayerJump").gameObject.audio.Play();
			rigidbody2D.AddForce(new Vector2(0, 200));
			currentJumpCount++;
		}

		if(prevState.Buttons.Back == ButtonState.Released && state.Buttons.Back == ButtonState.Pressed) {
			PlayerDied();
		}



		rigidbody2D.AddTorque ((prevState.Triggers.Left - prevState.Triggers.Right) * .7f);

		if (movement.x > 0 && !isFacingRight) {
			Flip (1);
		}
		else if (movement.x < 0 && isFacingRight) {
			Flip(-1);
		}

		if (transform.position.y < -2f) {
			PlayerDied();
		}
	}

	void Restart()
	{
		Deaths = 0;
		ResetPlayer ();
	}

	void OnCollisionEnter2D(Collision2D other){
		bool collisionWithOtherPlayer = other.gameObject.name == "Player" && isPlaying;// && other.gameObject.GetComponent<player_script>().isPlaying;
		GamePad.SetVibration ((PlayerIndex)PlayerId, collisionWithOtherPlayer ? 1 : 0, 0);

		if (collisionWithOtherPlayer) {
			GameObject.FindGameObjectWithTag ("AudioPlayerHit").gameObject.audio.Play();	
		}

		if (other.gameObject.name == "Ground") {
			currentJumpCount = 0;
		}
	}

	void PlayerDied(){
		GameObject.FindGameObjectWithTag ("AudioPlayerDeath").gameObject.audio.Play();
		Deaths++;
		ResetPlayer();
	}

	void ResetPlayer(){
		int spawnId = (int)Math.Floor(UnityEngine.Random.value * spawnPoints.Length);
		transform.position = spawnPoints [spawnId].transform.position; //startPosition;
		rigidbody2D.velocity = new Vector2 (0, 0);
		transform.rotation = new Quaternion ();
		GamePad.SetVibration ((PlayerIndex)PlayerId, 0, 0);
	}

	void Flip(float direction)
	{
		transform.localScale = new Vector2(direction, 1);
		isFacingRight = !isFacingRight;
	}

	void OnGUI()
	{
		if(RenderText)
			GUI.Label (new Rect(10f, 10f, Screen.width - 20f, Screen.height - 20f), "Player" + (PlayerId + 1) + " deaths: " + Deaths, textStyle);
		//RenderPlayerInfo ();
	}

	void RenderPlayerInfo()
	{
		string text = "Player info\n";
		text += string.Format("IsConnected {0} Packet #{1}\n", state.IsConnected, state.PacketNumber);
		text += string.Format("\tTriggers {0} {1}\n", state.Triggers.Left, state.Triggers.Right);
		text += string.Format("\tD-Pad {0} {1} {2} {3}\n", state.DPad.Up, state.DPad.Right, state.DPad.Down, state.DPad.Left);
		text += string.Format("\tButtons Start {0} Back {1}\n", state.Buttons.Start, state.Buttons.Back);
		text += string.Format("\tButtons LeftStick {0} RightStick {1} LeftShoulder {2} RightShoulder {3}\n", state.Buttons.LeftStick, state.Buttons.RightStick, state.Buttons.LeftShoulder, state.Buttons.RightShoulder);
		text += string.Format("\tButtons A {0} B {1} X {2} Y {3}\n", state.Buttons.A, state.Buttons.B, state.Buttons.X, state.Buttons.Y);
		text += string.Format("\tSticks Left {0} {1} Right {2} {3}\n", state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y, state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
		GUI.Label(new Rect(0, 150.0f * PlayerId, Screen.width, Screen.height), text);
	}
}
