using UnityEngine;
using System.Collections;

public class LabirintoManager : MonoBehaviour
{

	public int Linhas, Colunas;

	public GameObject Parede;
    public GameObject Piso;
    private float escalaGameObj = 5.5f;

    public GameObject queijoMesh; // modelo 3d (prefab) do queijo
    public GameObject ratoMesh;   // modelo 3d (prefab) do rato

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

    //atualizar distancia do labirinto de acordo com o tamanho dele, para que sempre fique visivel no fundo do menu inicial
    public float CalcularAlturaCamera()
    {
        //calcular altura da camera
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
        
        InicializarLabirinto(); //Inicia a base do labirinto, em que todas as células estão fechadas por paredes em todas direções

        Labirinto huntAndKill = new HuntAndKill(matrizCelulas);
        huntAndKill.CriarLabirinto(); //Executa o HuntAndKill para dar forma ao labirinto, quebrando paredes e formando o labirinto perfeito(todas células acessíveis)
        
        var camPos = matrizCelulas[(Linhas/2), (Colunas/2)].piso.transform.position;

        camera.transform.position = new Vector3(0, CalcularAlturaCamera(), 0);

        PosicionarLabirinto();
    }

    //assim que o labirinto é gerado, ele é posicionado a partir do centro do primeiro piso
    //aqui executamos uma correção para que todos labirintos sempre fiquem alinhados com o grid posteriormente
    public void PosicionarLabirinto()
    {
        var tamanhoLabirintoX = escalaGameObj * Linhas - (escalaGameObj);
        var tamanhoLabirintoY = escalaGameObj * Colunas - (escalaGameObj);

        Vector3 fixedPos = new Vector3(-(tamanhoLabirintoX / 2), 0, -(tamanhoLabirintoY / 2));
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

        for (int l = 0; l < Linhas; l++)
        {
            for (int c = 0; c < Colunas; c++)
            {
				matrizCelulas [l, c] = new Celula();


                //adicionar os PISOS em todas as posições da matriz
                matrizCelulas[l, c].piso = Instantiate(Piso, new Vector3 (l*escalaGameObj, -0.25f, c*escalaGameObj), Quaternion.identity) as GameObject;
				matrizCelulas[l, c].piso.name = "Piso " + l + "," + c;
				matrizCelulas[l, c].piso.transform.Rotate(Vector3.right, 90f);
                matrizCelulas[l, c].piso.transform.SetParent(Labirinto);
                matrizCelulas[l, c].piso.tag = "Piso";
                matrizCelulas[l, c].piso.GetComponent<Renderer>().material.color = new Color(0.9f, 0.9f, 0.9f);

                //Preencher todo o labirinto inicial
                //fazendo as paredes externas e preenchendo todas as células
                //com paredes fechando toda célula por completo

                if (c == 0)
                {
					matrizCelulas[l, c].paredeOeste = Instantiate(Parede, new Vector3 (l*escalaGameObj, 0.8f, (c*escalaGameObj) - (escalaGameObj/2f)), Quaternion.identity) as GameObject;
					matrizCelulas[l, c].paredeOeste.name = "Parede (Oeste) " + l + "," + c;
                    matrizCelulas[l, c].paredeOeste.tag = "Parede";
                    matrizCelulas[l, c].paredeOeste.layer = LayerMask.NameToLayer("Parede");
                    matrizCelulas[l, c].paredeOeste.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(0.05f, 0.05f, 0.05f));
                    matrizCelulas[l, c].paredeOeste.GetComponent<Renderer>().material.SetTexture("wall", default);
                    matrizCelulas[l, c].paredeOeste.transform.SetParent(Labirinto);
                }
                
				matrizCelulas[l, c].paredeLeste = Instantiate(Parede, new Vector3 (l*escalaGameObj, 0.8f, (c*escalaGameObj) + (escalaGameObj/2f)), Quaternion.identity) as GameObject;
				matrizCelulas[l, c].paredeLeste.name = "Parede (Leste) " + l + "," + c;
                matrizCelulas[l, c].paredeLeste.tag = "Parede";
                matrizCelulas[l, c].paredeLeste.layer = LayerMask.NameToLayer("Parede");
                matrizCelulas[l, c].paredeLeste.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(0.05f, 0.05f, 0.05f));
                matrizCelulas[l, c].paredeLeste.GetComponent<Renderer>().material.SetTexture("wall", default);
                matrizCelulas[l, c].paredeLeste.transform.SetParent(Labirinto);

                if (l == 0)
                {
					matrizCelulas[l, c].paredeNorte = Instantiate(Parede, new Vector3 ((l*escalaGameObj) - (escalaGameObj/2f), 0.8f, c*escalaGameObj), Quaternion.identity) as GameObject;
					matrizCelulas[l, c].paredeNorte.name = "Parede (Norte) " + l + "," + c;
					matrizCelulas[l, c].paredeNorte.transform.Rotate (Vector3.up * 90f);
                    matrizCelulas[l, c].paredeNorte.tag = "Parede";
                    matrizCelulas[l, c].paredeNorte.layer = LayerMask.NameToLayer("Parede");
                    matrizCelulas[l, c].paredeNorte.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(0.05f, 0.05f, 0.05f));
                    matrizCelulas[l, c].paredeNorte.GetComponent<Renderer>().material.SetTexture("wall", default);
                    matrizCelulas[l, c].paredeNorte.transform.SetParent(Labirinto);
                }
                
				matrizCelulas[l, c].paredeSul = Instantiate(Parede, new Vector3 ((l*escalaGameObj) + (escalaGameObj/2f), 0.8f, c*escalaGameObj), Quaternion.identity) as GameObject;
				matrizCelulas[l, c].paredeSul.name = "Parede (Sul) " + l + "," + c;
                matrizCelulas[l, c].paredeSul.transform.Rotate (Vector3.up * 90f);
                matrizCelulas[l, c].paredeSul.tag = "Parede";
                matrizCelulas[l, c].paredeSul.layer = LayerMask.NameToLayer("Parede");
                matrizCelulas[l, c].paredeSul.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(0.05f, 0.05f, 0.05f));
                matrizCelulas[l, c].paredeSul.GetComponent<Renderer>().material.SetTexture("wall", default);
                matrizCelulas[l, c].paredeSul.transform.SetParent(Labirinto);
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
        queijoPickup.name = $"Queijo {queijoPos.piso.name.Substring(5, 3)}"; //fazer com o nome do gameObj do queijo seja "Queijo" + a posição do chão em que ele se encontra
    }

    //gerar uma posição aleatória do labirinto para spawnar o queijo
    private Celula GerarQueijoPos()
    {
        var ratoPos = matrizCelulas[0, 0].piso.transform.position;

        while (true)
        {
            int p1 = Random.Range(0, Linhas);//range começando do 0 pra frente para que não gere o queijo na posição inicial(0,0) que é onde o rato nasce
            int p2 = Random.Range(0, Colunas);
            var queijoPos = matrizCelulas[p1, p2].piso.transform.position;
            
            if(ratoPos != queijoPos) return matrizCelulas[p1, p2];
        }
    }

}
