using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{ // Start is called before the first frame update

		[SerializeField] GameObject MainMenuUI;

		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool menu;
		public bool interact;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnMenu(InputValue value)
		{
			MenuInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		public void InteractInput(bool interacting)
		{
			interact = interacting;
		}
        public void MenuInput(bool newMenuState)
        {
			if (menu)
			{
				Resume();
			}
			else
			{

                if (MainMenuUI != null) MainMenuUI.SetActive(true);
                else Debug.LogError("No Main Menu!");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                menu = true;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
		public void Resume()
        {
            if (MainMenuUI != null) MainMenuUI.SetActive(false);
            else Debug.LogError("No Main Menu!");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            menu = false;
        }
		public void Restart()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			Resume();
		}
		public void Quit()
		{
			Application.Quit();
		}
	}
   
}