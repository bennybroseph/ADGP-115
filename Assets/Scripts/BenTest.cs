using UnityEngine;

using Library;

using Event = Define.Event;
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

        Publisher.self.Subscribe(Event.Test, TestEvent);
        Publisher.self.Broadcast(Event.Test);
        Publisher.self.DelayedBroadcast(Event.Test);
    }

    private void TestEvent(Event a_Message, params object[] a_Params)
    {
        UnityDebug.Log("Event Fired");
    }
    
    // Update is called once per frame
    private void Update()
    {
        
    }
}
