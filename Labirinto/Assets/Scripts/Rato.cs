using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rato : MonoBehaviour
{
    private GameObject gridLoader;
    public GameObject rato;

    private Vector3 queijoPos;

    private List<Node> nodesCaminho;

    private Vector3 posAtual;
    private Vector3 posDestino;

    //duração do lerp (segundos)
    private float tempoEstimado;
    private float tempoPassado;

    //velocidade do rato
    const float speed = 3.3f;

    private Animator ratoAnimator;

    // Start is called before the first frame update
    void Start()
    {
        nodesCaminho = new List<Node>();

        gridLoader = GameObject.FindWithTag("GridLoader");

        var grid = gridLoader.GetComponent<Grid>();
        nodesCaminho.AddRange(grid.caminho.ToArray());

        //remover ultimo node para evitar que o rato entre dentro do queijo
        nodesCaminho.RemoveAt(nodesCaminho.Count -1);

        rato = GameObject.FindWithTag("Rato");
        queijoPos = GameObject.FindWithTag("Queijo").transform.position;
        posAtual = rato.transform.position;

        tempoEstimado = Vector3.Distance(posAtual, nodesCaminho[0].vPos) / speed;
        tempoPassado = 0;

        var rot = Quaternion.LookRotation(nodesCaminho[0].vPos - rato.transform.position);
        rato.transform.rotation = rot;

        ratoAnimator = rato.gameObject.GetComponent<Animator>();

        ratoAnimator.SetBool("correr", true);
    }

 
    void Update()
    {
        if (nodesCaminho != null)
        {
            if (nodesCaminho.Count == 0)
                return;

            var firstNode = nodesCaminho[0];

            //mover o ratao
            tempoPassado += Time.deltaTime;
            float num = Mathf.InverseLerp(0f, tempoEstimado, tempoPassado);
            var newPos = Vector3.Lerp(posAtual, firstNode.vPos, num);
            rato.transform.position = newPos;

            //mover a camera de acordo com a posição do rato, sem mexer na altura
            var cam = GameObject.FindWithTag("RatoCam");
            cam.transform.position = new Vector3(newPos.x, cam.transform.position.y, newPos.z);


            //descer a camera ao dar play
            if (cam.transform.position.y > 30f)
            {
                var newCam = new Vector3(cam.transform.position.x, cam.transform.position.y -1f, cam.transform.position.z);
                var newCamPos = Vector3.Lerp(cam.transform.position, newCam, num);
                cam.transform.position = newCamPos;
            }
            
            //remover o primeiro node a cada node que ele andar
            if (num >= 1f)
            {
                posAtual = firstNode.vPos;
                
                nodesCaminho.RemoveAt(0);

                Quaternion rot = Quaternion.identity;

                if (nodesCaminho.Count == 0)
                {
                    tempoEstimado = 0;
                    ratoAnimator.SetBool("correr", false);
                    rot = Quaternion.LookRotation(queijoPos - rato.transform.position); ;
                }
                else
                {
                    rot = Quaternion.LookRotation(nodesCaminho[0].vPos - rato.transform.position);
                    tempoEstimado = Vector3.Distance(posAtual, nodesCaminho[0].vPos) / speed;
                }

                rato.transform.rotation = rot;

                tempoPassado = 0;
                return;
            }
            
        }
    }
}
