using UnityEngine;
using System.Collections;
using com.gamehouse.slingo.events;
using com.gamehouse.slingo.states;
using com.gamehouse.slingo.engine.game;

public class ReelRocketController : MonoBehaviour {

	public ReelItem parentReel;
	public GameObject starburst;
	private bool IsMoving = false;
	public float RocketStartSpeed;
	private float RocketSpeed;
	public float RocketAccel = 0.1f;
	public float MovementDelay = 1.0f;
	private bool hitBlocker = false;


	private void OnTriggerEnter (Collider other) 
	{
		Cell cell = null;

		if(other.transform.name.Contains("CellItem"))
		{
			cell = other.gameObject.GetComponent<CellItem>()._cell;
			UIManager.instance.matrixController.ApplyDaubResultNoPowerups(cell);
		}
		if(other.transform.name.Contains("Blockers"))
		{
			cell = other.transform.parent.parent.gameObject.GetComponent<CellItem>()._cell;
			UIManager.instance.matrixController.ApplyDaubResultNoPowerups(cell);
			IsMoving = false;
			hitBlocker = true;
			starburst.SetActive(true);
            EventDispatcher.TriggerEvent(new BlockerDestroyedByRocketEvent());
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(IsMoving)
		{
			transform.position += new Vector3(0.0f, RocketSpeed * Time.deltaTime, 0.0f);
			RocketSpeed += RocketAccel * Time.deltaTime;
		}
		else
		{
			RocketSpeed = RocketStartSpeed;
		}
		if(hitBlocker)
		{
			StartCoroutine(DestroyRocket());
		}
	}

	IEnumerator DestroyRocket()
	{
		AudioManager.instance.Stop("GSKFireworkLaunches");
		if(hitBlocker)
		{
			AudioManager.instance.Play("GSKFireworkExplo");
			hitBlocker = false;
		}
		yield return new WaitForSeconds(5.0f);
		IsMoving = false;
		parentReel.StopRocketAnim();
		starburst.SetActive(false);
		parentReel.IsUseable = false;
		EventDispatcher.TriggerEvent(new ChangeGameStateEvent(GameState.POST_DAUB_STATE)); 
	}

	public void startMovement()
	{
		IsMoving = true;
		StartCoroutine(WaitForOffscreen());
	}

	IEnumerator WaitForOffscreen()
	{
		yield return new WaitForSeconds(3.0f);
		StartCoroutine(DestroyRocket());
	}
}