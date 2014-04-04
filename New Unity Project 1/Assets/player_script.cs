﻿using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class player_script : MonoBehaviour {
	bool playerIndexSet = false;
	PlayerIndex playerIndex;
	GamePadState state;
	GamePadState prevState;
	public Vector2 _speed = new Vector2(2,2);
	public bool isFacingRight = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Find a PlayerIndex, for a single player game
		// Will find the first controller that is connected ans use it
		if (!playerIndexSet || !prevState.IsConnected)
		{
			for (int i = 0; i < 4; ++i)
			{
				PlayerIndex testPlayerIndex = (PlayerIndex)i;
				GamePadState testState = GamePad.GetState(testPlayerIndex);
				if (testState.IsConnected)
				{
					Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
					playerIndex = testPlayerIndex;
					playerIndexSet = true;
				}
			}
		}

		prevState = state;
		state = GamePad.GetState(playerIndex);
		
		var xxbox = state.ThumbSticks.Left.X;
		float x = xxbox;// Input.GetAxis ("Horizontal");
		
		var movement = new Vector2 (_speed.x * x, 0);
		
		movement *= Time.deltaTime;
		transform.Translate (movement);

		if(prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed || Input.GetButtonDown ("Jump")) {
			rigidbody2D.AddForce(new Vector2(0, 200));
		}

		if (movement.x > 0 && !isFacingRight) {
			Flip (1);
		}
		else if (movement.x < 0 && isFacingRight) {
			Flip(-1);
		}
	}

	void Flip(float direction)
	{
		transform.localScale = new Vector2(direction, 1);
		isFacingRight = !isFacingRight;
	}

	void OnGUI()
	{
		string text = "Use left stick to turn the cube, hold A to change color\n";
		text += string.Format("IsConnected {0} Packet #{1}\n", state.IsConnected, state.PacketNumber);
		text += string.Format("\tTriggers {0} {1}\n", state.Triggers.Left, state.Triggers.Right);
		text += string.Format("\tD-Pad {0} {1} {2} {3}\n", state.DPad.Up, state.DPad.Right, state.DPad.Down, state.DPad.Left);
		text += string.Format("\tButtons Start {0} Back {1}\n", state.Buttons.Start, state.Buttons.Back);
		text += string.Format("\tButtons LeftStick {0} RightStick {1} LeftShoulder {2} RightShoulder {3}\n", state.Buttons.LeftStick, state.Buttons.RightStick, state.Buttons.LeftShoulder, state.Buttons.RightShoulder);
		text += string.Format("\tButtons A {0} B {1} X {2} Y {3}\n", state.Buttons.A, state.Buttons.B, state.Buttons.X, state.Buttons.Y);
		text += string.Format("\tSticks Left {0} {1} Right {2} {3}\n", state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y, state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
		GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text);
	}
}