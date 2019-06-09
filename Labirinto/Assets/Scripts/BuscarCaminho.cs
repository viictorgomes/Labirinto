using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuscarCaminho : MonoBehaviour
{
    Grid grid;
    private GameObject inicioPos;
    private GameObject finalPos;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        inicioPos = GameObject.FindWithTag("Rato");
        finalPos = GameObject.FindWithTag("Queijo");

        if(inicioPos != null && finalPos != null)
            A_Estrela(inicioPos.transform.position, finalPos.transform.position);
    }

    void A_Estrela(Vector3 startPos, Vector3 finalPos)
    {
        bool diagonal = false;

        Node nodeInicial = grid.ObterNodeByPos(startPos);//obter o node mais próximo da cordenada startPos no mundo
        Node nodeAlvo = grid.ObterNodeByPos(finalPos);//obter o node mais próximo da cordenada finalPos no mundo

        List<Node> ListaAberta = new List<Node>();
        HashSet<Node> ListaFechada = new HashSet<Node>();

        ListaAberta.Add(nodeInicial);//adicionar o node inicial na lista aberta para poder começar

        while (ListaAberta.Count > 0)
        {
            Node nodeAtual = ListaAberta[0];
            for (int i = 1; i < ListaAberta.Count; i++)//loopar a lista aberta começando pelo 2 objeto
            {
                if (ListaAberta[i].custo_F < nodeAtual.custo_F || ListaAberta[i].custo_F == nodeAtual.custo_F && ListaAberta[i].custo_H < nodeAtual.custo_H)//If the f cost of that object is less than or equal to the f cost of the current node
                {
                    nodeAtual = ListaAberta[i];
                }
            }
            ListaAberta.Remove(nodeAtual);
            ListaFechada.Add(nodeAtual);

            if (nodeAtual == nodeAlvo)
            {
                ObterCaminhoFinal(nodeInicial, nodeAlvo);
            }

            foreach (Node nodeVizinho in grid.ObterNodesVizinhos(nodeAtual, diagonal))
            {
                if (!nodeVizinho.bAcessivel || ListaFechada.Contains(nodeVizinho))//se o vizinho for uma parede ou se já foi visitado
                {
                    continue; //prox 
                }
                int custoDeMovimento = nodeAtual.custo_G + ObterDistancia(nodeAtual, nodeVizinho);

                if (custoDeMovimento < nodeVizinho.custo_G || !ListaAberta.Contains(nodeVizinho))
                {
                    nodeVizinho.custo_G = custoDeMovimento;
                    nodeVizinho.custo_H = ObterDistancia(nodeVizinho, nodeAlvo);
                    nodeVizinho.parent = nodeAtual;

                    if (!ListaAberta.Contains(nodeVizinho))
                    {
                        ListaAberta.Add(nodeVizinho);
                    }
                }
            }

        }
    }

    void ObterCaminhoFinal(Node nodeInicial, Node nodeFinal)
    {
        List<Node> caminho = new List<Node>();
        Node nodeAtual = nodeFinal;

        while (nodeAtual != nodeInicial)
        {
            caminho.Add(nodeAtual);
            nodeAtual = nodeAtual.parent;
        }

        caminho.Reverse(); //Corrigir a ordem

        grid.caminho = caminho;
    }

    int ObterDistancia(Node nodeA, Node nodeB)
    {
        int ix = Mathf.Abs(nodeA.gridX - nodeB.gridX);//x1-x2
        int iy = Mathf.Abs(nodeA.gridY - nodeB.gridY);//y1-y2

        return ix + iy;
    }
}
