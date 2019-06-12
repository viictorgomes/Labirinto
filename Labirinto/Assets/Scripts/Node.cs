using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool bAcessivel; // false caso este node esteja na mesma posição de uma parede
    public Vector3 vPos;    // posição dentro do mundo 3d

    public int gridX;
    public int gridY;

    public Node parent;

    public int custo_G;
    public int custo_H;

    public int custo_F
    {
        get {return custo_G + custo_H;}
    }

    public Node(bool _Acessivel, Vector3 _Pos, int _gridX, int _gridY){
        bAcessivel = _Acessivel;
        vPos = _Pos;
        gridX = _gridX;
        gridY = _gridY;

    }

}
