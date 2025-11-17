using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleWASDMovement : MonoBehaviour
{
	[Header("Movement Settings")]
	public float moveSpeed = 5f;

	private CharacterController characterController;
	private Vector2 inputVector;
	private Vector3 movement;

	void Start()
	{
		characterController = GetComponent<CharacterController>();
	}

	void Update()
	{
		MoveCharacter();
	}

	public void OnMove(InputValue value)
	{
		inputVector = value.Get<Vector2>();
	}

	private void MoveCharacter()
	{
		movement = new Vector3(inputVector.x, 0, inputVector.y);
		movement *= moveSpeed;
		movement.y = Physics.gravity.y;
		characterController.Move(movement * Time.deltaTime);
	}
}