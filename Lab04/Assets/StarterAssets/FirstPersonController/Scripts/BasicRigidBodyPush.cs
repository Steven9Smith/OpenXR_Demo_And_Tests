using UnityEngine;

public class BasicRigidBodyPush : MonoBehaviour
{
	public LayerMask pushLayers;
	public bool canPush;
	[Range(0.5f, 5000f)] public float strength = 1.1f;

	[SerializeField] private Animator anim;

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (canPush) PushRigidBodies(hit);
	}

	float currentTime = 0;
	[SerializeField] private float timeout = 0.5f;
	void Update()
	{
		currentTime += Time.deltaTime;
		if(currentTime >= timeout)
            if (anim != null) anim.SetBool("isPushing", false);

    }

	private void PushRigidBodies(ControllerColliderHit hit)
	{
		// https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

		// make sure we hit a non kinematic rigidbody
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic)
			return;
		// make sure we only push desired layer(s)
		var bodyLayerMask = 1 << body.gameObject.layer;
		if ((bodyLayerMask & pushLayers.value) == 0)
			return;
        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;
        

        // Calculate push direction from move direction, horizontal motion only
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

		// Apply the push and take strength into account
		body.AddForce(pushDir * strength, ForceMode.Impulse);
		currentTime = 0;
        //change animation to push animation
        if (anim != null) anim.SetBool("isPushing", true);
    }
}