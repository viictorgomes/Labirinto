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
    const float velocidade = 3.3f;

    private Animator ratoAnimator;
    
    void Start()
    {
        nodesCaminho = new List<Node>();

        gridLoader = GameObject.FindWithTag("GridLoader");

        var grid = gridLoader.GetComponent<Grid>();
        nodesCaminho.AddRange(grid.caminho.ToArray());

        //remover ultimo node do caminho para evitar que o rato entre dentro do queijo
        nodesCaminho.RemoveAt(nodesCaminho.Count -1);

        rato = GameObject.FindWithTag("Rato");
        queijoPos = GameObject.FindWithTag("Queijo").transform.position;

        posAtual = rato.transform.position; //setar a primeira posição inicial

        tempoEstimado = Vector3.Distance(posAtual, nodesCaminho[0].vPos) / velocidade;
        tempoPassado = 0;

        var rot = Quaternion.LookRotation(nodesCaminho[0].vPos - rato.transform.position);
        rato.transform.rotation = rot;

        ratoAnimator = rato.gameObject.GetComponent<Animator>();

        ratoAnimator.SetBool("correr", true);
    }

    /*
    Lógica de movimento:
                
        Existe uma lista de nodes com posições

        O rato movimenta de node por node, e não direto ao destino

        Assim que começamos, o rato se encontra no primeiro node, então removemos o primeiro e usamos o próximo como destino

        O tempo e o lerp são utilizados para fazer movimentos "suaves/lisos" de um node ao outro,
        como se estivesse andando de um até outro e não teleportando direto entre posições 

        Sempre há um node inicial(primeiroNode) e um final(newPos), sendo cada passo do rato, e sempre que ele chega no final(proximo passo), 
        deletamos o inicial e setamos um novo caminho(novo inicial e final), de forma que ele semrpe siga os passos dos nodes corretamente e não atravesse paredes
    */
    void Update()
    {
        if (nodesCaminho != null) 
        {
            if (nodesCaminho.Count == 0) //enquanto ainda houver nodes na lista de nodes caminho
                return;

            var primeiroNode = nodesCaminho[0];

            //mover o ratao
            tempoPassado += Time.deltaTime;
            float mediaTempo = Mathf.InverseLerp(0f, tempoEstimado, tempoPassado); //média de tempo, é usada para interpolar o tempo de movimento de um ponto A a B

            var newPos = Vector3.Lerp(posAtual, primeiroNode.vPos, mediaTempo); // Utilização do Lerp para gerar posições entre A a B dando um tempo para realização do mesmo

            rato.transform.position = newPos; //realizar o movimento do rato a partir da atualização de posições geradas pelo Lerp

            //mover a camera de acordo com a posição do rato, sem mexer na altura
            var cam = GameObject.FindWithTag("RatoCam");
            cam.transform.position = new Vector3(newPos.x, cam.transform.position.y, newPos.z);
            
            //descer a camera ao dar play
            if (cam.transform.position.y > 30f)
            {
                var newCam = new Vector3(cam.transform.position.x, cam.transform.position.y -1f, cam.transform.position.z);
                var newCamPos = Vector3.Lerp(cam.transform.position, newCam, mediaTempo);
                cam.transform.position = newCamPos;
            }
            
            //remover o primeiro node a cada node que ele andar
            if (mediaTempo >= 1f)
            {
                posAtual = primeiroNode.vPos; //atualizar a posAtual
                
                nodesCaminho.RemoveAt(0);

                Quaternion rot = Quaternion.identity; // rotação

                if (nodesCaminho.Count == 0) //chegou no final do caminho
                {
                    tempoEstimado = 0;
                    ratoAnimator.SetBool("correr", false);
                    //setar rot como a direção do queijo
                    rot = Quaternion.LookRotation(queijoPos - rato.transform.position); ;
                }
                else
                {
                    //setar rot como a direção do proximo node
                    rot = Quaternion.LookRotation(nodesCaminho[0].vPos - rato.transform.position);
                    tempoEstimado = Vector3.Distance(posAtual, nodesCaminho[0].vPos) / velocidade;
                }

                //atualizar rotação do rato para que esteja olhando para direção de rot
                rato.transform.rotation = rot;

                tempoPassado = 0;
                return;
            }
            
        }
    }
}
