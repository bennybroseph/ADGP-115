using UnityEngine;
using System.Collections;
using Library;


public class GameManager : MonoSingleton<GameManager>
{

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        Publisher.self.Update();
    }
}
