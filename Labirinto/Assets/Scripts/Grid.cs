using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask inacessivelMask;

    private Vector2 tamanhoGridMundo;
    public Node[,] gridArray;

    public GameObject mazeLoader;
    public List<Node> caminho;

    private GameObject rato;
    private GameObject queijo;

    public bool mostrarGrid = false;

    public float tamanhoNode;
    float diametroNode;

    int tamanhoGridX;
    int tamanhoGridY;

    void Start()
    {
        diametroNode = tamanhoNode * 2;
        CriarGrid();
    }

    public void LimparGrid()
    {
        if (gridArray != null) gridArray = null;
    }

    void CalcularGrid()
    {
        tamanhoGridX = Mathf.RoundToInt(tamanhoGridMundo.x / diametroNode);
        tamanhoGridY = Mathf.RoundToInt(tamanhoGridMundo.y / diametroNode);

        var maze = mazeLoader.GetComponent<LabirintoManager>();
        tamanhoGridMundo.x = 5.5f * maze.Linhas;
        tamanhoGridMundo.y = 5.5f * maze.Colunas;
    }

    public void CriarGrid()
    {
        LimparGrid();
        CalcularGrid();

        gridArray = new Node[tamanhoGridX, tamanhoGridY];//define o tamanho do grid
        Vector3 posInicial = transform.position - Vector3.right * tamanhoGridMundo.x / 2 - Vector3.forward * tamanhoGridMundo.y / 2; //obter o ponto inicial do grid (canto inferior esquerdo)

        var maze = mazeLoader.GetComponent<LabirintoManager>();

        Vector3 fixPos = new Vector3(0,0,0);

        for (int x = 0; x < tamanhoGridX; x++)
        {
            for (int y = 0; y < tamanhoGridY; y++)
            {
                Vector3 worldPoint = posInicial + Vector3.right * (x * diametroNode + tamanhoNode) + Vector3.forward * (y * diametroNode + tamanhoNode);//obter a posição no mundo(v3) do canto esquerdo inferior
                
                bool parede = true;
                fixPos = worldPoint;

                //realizar o teste de colisão pra determinar se é uma parede ou não.
                if (Physics.CheckSphere(worldPoint, tamanhoNode, inacessivelMask))
                {
                    parede = false;
                }

                gridArray[x, y] = new Node(parede, worldPoint, x, y); //cria um novo node no array
            }
        }
    }

    public List<Node> ObterNodesVizinhos(Node node, bool diagonal)
    {
        List<Node> ListaVizinhos = new List<Node>();

        //possibilitar a visão de vizinhos na diagonal
        if (diagonal)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < tamanhoGridX && checkY >= 0 && checkY < tamanhoGridY)
                    {
                        ListaVizinhos.Add(gridArray[checkX, checkY]);
                    }
                }
            }
        }

        else
        {
            int checkX;//usado pra verificar se a posição está dentro do alcance da matriz
            int checkY;

            //verificar o lado direito do node atual.
            checkX = node.gridX + 1;
            checkY = node.gridY;
            if (checkX >= 0 && checkX < tamanhoGridX)//se a posição X está dentro do alcance da matriz
            {
                if (checkY >= 0 && checkY < tamanhoGridY)//se a posição Y está dentro do alcance da matriz
                {
                    ListaVizinhos.Add(gridArray[checkX, checkY]);//adiciona o node na lista de vizinhos
                }
            }

            //verificar o lado esquerdo do node atual.
            checkX = node.gridX - 1;
            checkY = node.gridY;
            if (checkX >= 0 && checkX < tamanhoGridX)
            {
                if (checkY >= 0 && checkY < tamanhoGridY)
                {
                    ListaVizinhos.Add(gridArray[checkX, checkY]);
                }
            }

            //verificar o topo do node atual.
            checkX = node.gridX;
            checkY = node.gridY + 1;
            if (checkX >= 0 && checkX < tamanhoGridX)
            {
                if (checkY >= 0 && checkY < tamanhoGridY)
                {
                    ListaVizinhos.Add(gridArray[checkX, checkY]);
                }
            }

            //verificar em baixo do node atual.
            checkX = node.gridX;
            checkY = node.gridY - 1;
            if (checkX >= 0 && checkX < tamanhoGridX)
            {
                if (checkY >= 0 && checkY < tamanhoGridY)
                {
                    ListaVizinhos.Add(gridArray[checkX, checkY]);
                }
            }
        }

        return ListaVizinhos; //retorna a lista de vizinhos
    }
    
    public Node ObterNodeByPos(Vector3 worldPosition)
    {
        //medir porcentagem do grid de acordo com a posição do v3
        float percentX = (worldPosition.x + tamanhoGridMundo.x / 2) / tamanhoGridMundo.x;
        float percentY = (worldPosition.z + tamanhoGridMundo.y / 2) / tamanhoGridMundo.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //calcular o index do grid de acordo com a porcentagem de distancia
        int x = Mathf.RoundToInt((tamanhoGridX - 1) * percentX);
        int y = Mathf.RoundToInt((tamanhoGridY - 1) * percentY);
        
        return gridArray[x, y];
    }


    //OnDrawGizmos é uma função nativa da Unity
    //gizmos são utilizados pra fazer "visual debugging", debugar obtendo retorno visual renderizando linhas ou formas geometricas com tamanhos e posições que quiser
    void OnDrawGizmos()
    {
        var maze = mazeLoader.GetComponent<LabirintoManager>();
        rato = GameObject.FindWithTag("Rato");
        queijo = GameObject.FindWithTag("Queijo");

        if (mostrarGrid) {
            //desenhar wireframe do grid
            Gizmos.DrawWireCube(transform.position, new Vector3(tamanhoGridMundo.x, 1, tamanhoGridMundo.y));

            if(rato != null) { 
                if (gridArray != null)
                {
                    Node playerNode = ObterNodeByPos(rato.transform.position);
                    Node queijoNode = ObterNodeByPos(queijo.transform.position);

                    foreach (Node n in gridArray)
                    {
                        //diferenciar cor dos nodes em conflito com paredes
                        Gizmos.color = (n.bAcessivel) ? Color.white : Color.red;

                        //colorir os nodes do grid que fazem parte do caminho final
                        if (caminho != null) {

                            if (caminho.Contains(n)) {
                                Gizmos.color = Color.black;
                            }
                        }

                        //colorir node mais próximo do gameObj do rato
                        if (playerNode == n)
                        {
                            Gizmos.color = Color.green;
                        }

                        //colorir node mais próximo do gameObj do queijo
                        if (queijoNode == n)
                        {
                            Gizmos.color = Color.yellow;
                        }

                        //desenhar o grid
                        Gizmos.DrawCube(n.vPos, Vector3.one * (diametroNode - .1f));
                    }
                }
            }
        }
    }

    

}