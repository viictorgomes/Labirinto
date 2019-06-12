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
        bool diagonal = false; //permitir ou não os movimentos diagonais durante a busca

        Node nodeInicial = grid.ObterNodeByPos(startPos);//obter o node mais próximo da cordenada startPos no mundo
        Node nodeAlvo = grid.ObterNodeByPos(finalPos);//obter o node mais próximo da cordenada finalPos no mundo

        List<Node> ListaAberta = new List<Node>(); // Lista de Nodes que precisam ser testados
        HashSet<Node> ListaFechada = new HashSet<Node>(); //Lista de Nodes que já foram testados

        ListaAberta.Add(nodeInicial);//adicionar o node inicial na lista aberta para poder começar

        while (ListaAberta.Count > 0) //enquanto ainda houver nodes para serem testados
        {
            Node nodeAtual = ListaAberta[0];//setar nodeAtual como o primeiro elemento da ListaAberta

            for (int i = 1; i < ListaAberta.Count; i++)//loopar a lista aberta começando pelo 2 objeto(index1) porque o nodeAtual começa pelo index0, na linha de cima 
            {
                if (ListaAberta[i].custo_F < nodeAtual.custo_F || ListaAberta[i].custo_F == nodeAtual.custo_F && ListaAberta[i].custo_H < nodeAtual.custo_H)//se o custo F deste elemento da ListaAberta for menor ou igual ao custo F do node atual
                {
                    nodeAtual = ListaAberta[i]; //define este elemento como o node atual.
                }
            }

            //remove o atual da lista de nodes a serem testados, e adiciona na lista de nodes já testados
            ListaAberta.Remove(nodeAtual);
            ListaFechada.Add(nodeAtual);

            //se o node atual for o node alvo/destino, encontramos o caminho
            if (nodeAtual == nodeAlvo)
            {
                ObterCaminhoFinal(nodeInicial, nodeAlvo);
            }

            //visitar os nodes vizinhos
            foreach (Node nodeVizinho in grid.ObterNodesVizinhos(nodeAtual, diagonal))
            {
                if (!nodeVizinho.bAcessivel || ListaFechada.Contains(nodeVizinho))//se o vizinho for uma parede ou se o vizinho for um node que já foi testado
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
        
        //preencher o caminho final
        while (nodeAtual != nodeInicial)
        {
            caminho.Add(nodeAtual);
            nodeAtual = nodeAtual.parent;
        }

        caminho.Reverse(); //corrigir a ordem

        grid.caminho = caminho;
    }

    int ObterDistancia(Node nodeA, Node nodeB)
    {
        int x = Mathf.Abs(nodeA.gridX - nodeB.gridX);//x1-x2
        int y = Mathf.Abs(nodeA.gridY - nodeB.gridY);//y1-y2

        return x + y;
    }
}
