using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform[] positions;
    [SerializeField] float speed;
    private int actualPos=0;
    public void ThrowDice()
    {

        int rndnumber = Random.Range(1, 7);
        Debug.Log("el numero es: "+ rndnumber);
        StartCoroutine(Move(rndnumber));
    }

    IEnumerator Move(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            if (actualPos + 1 >= positions.Length)
                actualPos=-1;

            actualPos++;

            Vector3 destination = positions[actualPos].position;

            while (Vector3.Distance(transform.position, destination) > 0.0000001f)
            {
                transform.position = Vector3.MoveTowards
                    (transform.position, destination, speed * Time.deltaTime);

                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
