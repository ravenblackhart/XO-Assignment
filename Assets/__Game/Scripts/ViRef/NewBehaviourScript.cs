using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private int toWin = 0; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<int> test = new List<int>();

        test.Add(1);
        test.Add(0);
        test.Add(1);
        test.Add(1);
        test.Add(1);
        test.Add(0);
        test.Add(1);



        foreach (int i in test)
        {
            if (i == 1 && toWin < 3)
            {
                StartCoroutine(CheckWin()); 
               

            }
            else if (toWin >= 3)
            {
                Debug.Log("You Win");
                StopCoroutine(CheckWin());
            }
            else
            {
                toWin = 0;
            }
        }
        Debug.Log("The result here is" + toWin.ToString());

        IEnumerator CheckWin()
        {
            toWin += 1;
            
            yield return null;
        }


    }
}
