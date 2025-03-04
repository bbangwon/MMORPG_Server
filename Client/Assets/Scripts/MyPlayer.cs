using System.Collections;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager network;
    private void Start()
    {
        StartCoroutine(CoSendPacket());
        network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            var movePacket = new C_Move
            {
                posX = Random.Range(-50, 50),
                posY = 0f,
                posZ = Random.Range(-50, 50),
            };
            
            network.Send(movePacket.Write());
        }
    }
}
