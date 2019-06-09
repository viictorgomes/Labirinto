using UnityEngine;
using System.Collections;

public class LabirintoManager : MonoBehaviour
{

	public int Linhas, Colunas;
	public GameObject Parede;
    public GameObject Piso;
    private float Tamanho = 5.5f;

    public GameObject queijoMesh; // modelo 3d (prefab) do queijo
    public GameObject ratoMesh;// modelo 3d (prefab) do rato

    public GameObject camera;

    private Celula[,] matrizCelulas;

    private GameObject queijoPickup; //queijo
    private GameObject ratoPlayer;   //rato

    public Transform Labirinto;

    public void SetLinhas(int linhas)
    {
        Linhas = linhas;
    }

    public void SetColunas(int colunas)
    {
        Colunas = colunas;
    }
   
    void Start()
    {
        GerarLabirinto();
    }

    public float CalcularAlturaCamera()
    {
        //Calcular altura da camera
        var altura = 35;
        var alturaPorBloco = 7;
        if (Linhas > Colunas)
        {
            altura = altura + (alturaPorBloco * Linhas);
        }
        else
        {
            altura = altura + (alturaPorBloco * Colunas);
        }

        return altura;
    }

    public void GerarLabirinto()
    {
        if(matrizCelulas != null) DestruirLabirinto();
        
        InicializarLabirinto();

        Labirinto huntAndKill = new HuntAndKill(matrizCelulas);
        huntAndKill.CriarLabirinto();
        
        var camPos = matrizCelulas[(Linhas/2), (Colunas/2)].piso.transform.position;

        camera.transform.position = new Vector3(0, CalcularAlturaCamera(), 0);

        PosicionarLabirinto();
    }

    public void PosicionarLabirinto()
    {
        var labSizeX = 5.5f * Linhas - (5.5f);
        var labSizeY = 5.5f * Colunas - (5.5f);

        Vector3 fixedPos = new Vector3(-(labSizeX / 2), 0, -(labSizeY / 2));
        Labirinto.transform.localPosition = fixedPos;
    }


    public void DestruirLabirinto()
    {
        DestruirRatoQueijo();

        if (matrizCelulas != null)
        {
            foreach (var item in matrizCelulas)
            {
                GameObject.Destroy(item.piso);
                GameObject.Destroy(item.paredeLeste);
                GameObject.Destroy(item.paredeNorte);
                GameObject.Destroy(item.paredeSul);
                GameObject.Destroy(item.paredeOeste);
            }
        }

        //resetar pos inicial
        Labirinto.transform.position = new Vector3(0, 0, 0);
    }

    public void DestruirRatoQueijo()
    {
        if (ratoPlayer != null) GameObject.Destroy(ratoPlayer);
        if (queijoPickup != null) GameObject.Destroy(queijoPickup);
    }

	private void InicializarLabirinto()
    {

		matrizCelulas = new Celula[Linhas,Colunas];

        for (int r = 0; r < Linhas; r++)
        {
            for (int c = 0; c < Colunas; c++)
            {
				matrizCelulas [r, c] = new Celula();

                matrizCelulas[r, c].piso = Instantiate(Piso, new Vector3 (r*Tamanho, -0.25f, c*Tamanho), Quaternion.identity) as GameObject;
				matrizCelulas[r, c].piso.name = "Floor " + r + "," + c;
				matrizCelulas[r, c].piso.transform.Rotate(Vector3.right, 90f);
                matrizCelulas[r, c].piso.transform.SetParent(Labirinto);
                matrizCelulas[r, c].piso.tag = "Floor";
                matrizCelulas[r, c].piso.GetComponent<Renderer>().material.color = new Color(0.9f, 0.9f, 0.9f);


                if (c == 0)
                {
					matrizCelulas[r, c].paredeOeste = Instantiate(Parede, new Vector3 (r*Tamanho, 0.8f, (c*Tamanho) - (Tamanho/2f)), Quaternion.identity) as GameObject;
					matrizCelulas[r, c].paredeOeste.name = "West Wall " + r + "," + c;
                    matrizCelulas[r, c].paredeOeste.tag = "Wall";
                    matrizCelulas[r, c].paredeOeste.layer = LayerMask.NameToLayer("Wall");
                    matrizCelulas[r, c].paredeOeste.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(0.05f, 0.05f, 0.05f));
                    matrizCelulas[r, c].paredeOeste.GetComponent<Renderer>().material.SetTexture("wall", default);
                    matrizCelulas[r, c].paredeOeste.transform.SetParent(Labirinto);
                }
                
				matrizCelulas[r, c].paredeLeste = Instantiate(Parede, new Vector3 (r*Tamanho, 0.8f, (c*Tamanho) + (Tamanho/2f)), Quaternion.identity) as GameObject;
				matrizCelulas[r, c].paredeLeste.name = "East Wall " + r + "," + c;
                matrizCelulas[r, c].paredeLeste.tag = "Wall";
                matrizCelulas[r, c].paredeLeste.layer = LayerMask.NameToLayer("Wall");
                matrizCelulas[r, c].paredeLeste.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(0.05f, 0.05f, 0.05f));
                matrizCelulas[r, c].paredeLeste.GetComponent<Renderer>().material.SetTexture("wall", default);
                matrizCelulas[r, c].paredeLeste.transform.SetParent(Labirinto);

                if (r == 0)
                {
					matrizCelulas[r, c].paredeNorte = Instantiate(Parede, new Vector3 ((r*Tamanho) - (Tamanho/2f), 0.8f, c*Tamanho), Quaternion.identity) as GameObject;
					matrizCelulas[r, c].paredeNorte.name = "North Wall " + r + "," + c;
					matrizCelulas[r, c].paredeNorte.transform.Rotate (Vector3.up * 90f);
                    matrizCelulas[r, c].paredeNorte.tag = "Wall";
                    matrizCelulas[r, c].paredeNorte.layer = LayerMask.NameToLayer("Wall");
                    matrizCelulas[r, c].paredeNorte.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(0.05f, 0.05f, 0.05f));
                    matrizCelulas[r, c].paredeNorte.GetComponent<Renderer>().material.SetTexture("wall", default);
                    matrizCelulas[r, c].paredeNorte.transform.SetParent(Labirinto);
                }
                
				matrizCelulas[r, c].paredeSul = Instantiate(Parede, new Vector3 ((r*Tamanho) + (Tamanho/2f), 0.8f, c*Tamanho), Quaternion.identity) as GameObject;
				matrizCelulas[r, c].paredeSul.name = "South Wall " + r + "," + c;
                matrizCelulas[r, c].paredeSul.transform.Rotate (Vector3.up * 90f);
                matrizCelulas[r, c].paredeSul.tag = "Wall";
                matrizCelulas[r, c].paredeSul.layer = LayerMask.NameToLayer("Wall");
                matrizCelulas[r, c].paredeSul.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(0.05f, 0.05f, 0.05f));
                matrizCelulas[r, c].paredeSul.GetComponent<Renderer>().material.SetTexture("wall", default);
                matrizCelulas[r, c].paredeSul.transform.SetParent(Labirinto);
            }
        }
    }

    public void SpawnaRato()
    {
        if (ratoPlayer != null) return;

        var ratopos = matrizCelulas[0, 0].piso.transform.position;

        ratoPlayer = Instantiate(ratoMesh, new Vector3(ratopos.x, 0.25f, ratopos.z), Quaternion.identity) as GameObject;
        ratoPlayer.name = "Rato";
    }

    public void SpawnaQueijo()
    {
        if (queijoPickup != null) return;

        var queijoPos = GerarQueijoPos();
        var queijoTransformPos = queijoPos.piso.transform.position;

        queijoPickup = Instantiate(queijoMesh, new Vector3(queijoTransformPos.x, 0.25f, queijoTransformPos.z), Quaternion.identity) as GameObject;
        queijoPickup.name = $"Queijo {queijoPos.piso.name.Substring(6, 3)}";
    }

    private Celula GerarQueijoPos()
    {
        var ratoPos = matrizCelulas[0, 0].piso.transform.position;

        while (true)
        {
            int p1 = Random.Range(0, Linhas);
            int p2 = Random.Range(0, Colunas);
            var queijoPos = matrizCelulas[p1, p2].piso.transform.position;
            
            if(ratoPos != queijoPos) return matrizCelulas[p1, p2];
        }
        
    }

}
