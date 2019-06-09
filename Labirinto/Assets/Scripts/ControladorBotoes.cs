using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorBotoes : MonoBehaviour
{
    public GameObject mazeLoader;
    public GameObject gridLoader;

    LabirintoManager maze;
    Grid grid;

    public InputField tamanhoX;
    public InputField tamanhoY;

    public Text resultMsg;
    public Button btnNovoLab;
    public Button btnConfig;

    public GameObject configMenu;
    private GameObject labirinto;

    private GameObject ratoMesh;
    public Transform mainCamPos;
    private Transform camPosInicial;

    private GameObject objetosMenu;

    private bool bPlaying;
    public void SetGameStatus(bool status) {bPlaying = status;}

    public Text playBtnText;

    void Start()
    {
        camPosInicial = mainCamPos;
        objetosMenu = GameObject.Find("MenuObjects");
    }

    void Awake()
    {
        maze = mazeLoader.GetComponent<LabirintoManager>();
        grid = gridLoader.GetComponent<Grid>();
    }

    public void Play() {
        camPosInicial = mainCamPos;
        if (bPlaying == false)
        {
            grid.CriarGrid();

            maze.SpawnaRato();
            maze.SpawnaQueijo();

            playBtnText.text = "Stop";
            bPlaying = true;

            btnNovoLab.interactable = false;
            btnConfig.interactable = false;

            objetosMenu.SetActive(false);
        }
        else if(bPlaying == true)
        {
            maze.DestruirRatoQueijo();

            playBtnText.text = "Play";
            mainCamPos.transform.position = new Vector3(0, maze.CalcularAlturaCamera(), 0);
            bPlaying = false;

            btnNovoLab.interactable = true;
            btnConfig.interactable = true;

            objetosMenu.SetActive(true);
        }
    }

    public void GerarMaze()
    {
        maze.GerarLabirinto();
        mainCamPos.transform.position = new Vector3(0, mainCamPos.transform.position.y, 0);
    }

    public void Configurar()
    {
        configMenu.SetActive(!configMenu.active);

        tamanhoX.placeholder.GetComponent<Text>().text = maze.Linhas.ToString();
        tamanhoY.placeholder.GetComponent<Text>().text = maze.Colunas.ToString();
    }

    public void OK()
    {

        int novoX = 0;
        int novoY = 0;

        if ((tamanhoX.text.Length != 0) || (tamanhoY.text.Length != 0))
        {
            novoX = int.Parse(tamanhoX.text);
            novoY = int.Parse(tamanhoY.text);
        }
        else
        {
            novoX = maze.Linhas;
            novoY = maze.Colunas;

            StartCoroutine(DrawResultMsg("É necessário preencher os valores de X e Y antes de confirmar", Color.red, 2));
            return;
        }

        maze.SetLinhas(novoX);
        maze.SetColunas(novoY);

        configMenu.SetActive(!configMenu.active);

        GerarMaze();

        grid.CriarGrid();

        camPosInicial = mainCamPos;
    }

    public void MostrarGrid()
    {
        grid.CriarGrid();
        grid.mostrarGrid = !grid.mostrarGrid;
    }

    public void Sair()
    {
       
    }

    public IEnumerator DrawResultMsg(string texto, Color c, float delay)
    {
        resultMsg.color = c;
        resultMsg.text = texto;
        resultMsg.enabled = true;
        yield return new WaitForSeconds(delay);
        resultMsg.enabled = false;
    }
}
