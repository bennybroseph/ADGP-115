using BennyBroseph;
using UnityEngine;
using BennyBroseph.Contextual;
using UnityDebug = UnityEngine.Debug;

public class BenTest : MonoBehaviour
{
    public enum TestStates { Init, Idle, State1 }

    // Use this for initialization
    private void Start()
    {
        var testStateMachine = new FiniteStateMachine<TestStates>();

        testStateMachine.AddTransition(TestStates.Init, TestStates.Idle);

        testStateMachine.Transition(TestStates.Idle);

        InputManager.self.AddOnKeyDown(OnKeyDown);
        InputManager.self.AddOnKeyUp(OnKeyUp);

        InputManager.self.AddOnMouseDown(OnMouseDown);
        InputManager.self.AddOnMouseMove(OnMouseMove);
    }

    private void OnKeyDown(Keys a_Key)
    {
        UnityDebug.Log("Pressed: " + a_Key);
    }
    private void OnKeyUp(Keys a_Key)
    {
        UnityDebug.Log("Released: " + a_Key);
    }

    private void OnMouseDown(int a_Button, Vector2 a_Position)
    {
        UnityDebug.Log(a_Button + " " + a_Position.ToString());
    }
    private void OnMouseMove(int a_Button, Vector2 a_Position)
    {
        UnityDebug.Log(a_Button + " " + a_Position.ToString());
    }
    // Update is called once per frame
    private void Update()
    {
        
    }
}
