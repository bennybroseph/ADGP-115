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
    }

    private bool Yes()
    {
        return true;
    }
    // Update is called once per frame
    private void Update()
    {

    }
}
