using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttonstest : MonoBehaviour
{
    public Button masochism;
    public Button thirsty;
    public Button hungry;
    public Button masochism2;
    public Button thirsty2;
    public Button hungry2;
    public PlayerInput PlayerInput;

    void Start()
    {
        Button masochist = masochism.GetComponent<Button>();
		masochist.onClick.AddListener(TaskOnClickHP);

        Button thirst = thirsty.GetComponent<Button>();
		thirst.onClick.AddListener(TaskOnClickWA);

        Button hunger = hungry.GetComponent<Button>();
		hunger.onClick.AddListener(TaskOnClickHU);

        Button masochist2 = masochism2.GetComponent<Button>();
		masochist2.onClick.AddListener(TaskOnClickHP2);

        Button thirst2 = thirsty2.GetComponent<Button>();
		thirst2.onClick.AddListener(TaskOnClickWA2);

        Button hunger2 = hungry2.GetComponent<Button>();
		hunger2.onClick.AddListener(TaskOnClickHU2);
    }

	void TaskOnClickHP()
    {
		Debug.Log ("You have clicked the button!");
        PlayerInput.health = PlayerInput.health - 10;
	}
    void TaskOnClickWA()
    {
		Debug.Log ("You have clicked the button!");
        PlayerInput.thirst = PlayerInput.thirst - 10;
	}
    void TaskOnClickHU()
    {
		Debug.Log ("You have clicked the button!");
        PlayerInput.hunger = PlayerInput.hunger - 10;
	}
	void TaskOnClickHP2()
    {
		Debug.Log ("You have clicked the button!");
        PlayerInput.health = PlayerInput.health + 10;
	}
    void TaskOnClickWA2()
    {
		Debug.Log ("You have clicked the button!");
        PlayerInput.thirst = PlayerInput.thirst + 10;
	}
    void TaskOnClickHU2()
    {
		Debug.Log ("You have clicked the button!");
        PlayerInput.hunger = PlayerInput.hunger + 10;
	}
}