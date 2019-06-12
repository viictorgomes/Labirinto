using UnityEngine;
using System.Collections;

public class HuntAndKill : Labirinto
{

    //ponto de inicialização
	private int linhaAtual = 0;
	private int colunaAtual = 0;

	private bool scanCompleto = false;

	public HuntAndKill(Celula[,] mazeCells) : base(mazeCells)
    {

	}

	public override void CriarLabirinto ()
    {
		ExecutarHuntAndKill();
	}

    /*
        Algoritmo Hunt and Kill:

        1- Determinar um ponto de inicialização

        2- Realizar uma caminhada aleatória, esculpindo passagens para vizinhos não visitados, até que a célula atual não tenha vizinhos não visitados.

        3- Entrar no modo “hunt”(caça), em que scaneamos o grid buscando por uma célula não visitada que seja adjacente a uma célula visitada, 
          se for encontrado, cava/destroi(kill) uma passagem entre os dois e determina a célula anteriormente não visitada ser o novo ponto de inicialização.

        4- Repetir passos 2 e 3 até que o modo hunt scaneie todo o grid e não encontre nenhuma célula não visitada.
    */

    private void ExecutarHuntAndKill()
    {
		matrizCelulas [linhaAtual, colunaAtual].visitada = true;

		while (! scanCompleto)
        {
			Kill(); // Passo 2: começa a caminhada aleatória

			Hunt(); // Passo 3: entra no modo hunt e começa a buscar células não visitadas que sejam adjacentes a células visitadas
                    //          se não encontrar nenhuma, seta scanCompleto como true.
        }
    }

	private void Kill()
    {
		while (RotaDisponivel (linhaAtual, colunaAtual))
        {
			int direcao = GeradorSeed.GerarNovaSeed ();

			if (direcao == Direcao.Norte && CelulaDisponivel (linhaAtual - 1, colunaAtual))
            {
				// norte
				DestruirParedeCasoExista (matrizCelulas [linhaAtual, colunaAtual].paredeNorte);
				DestruirParedeCasoExista (matrizCelulas [linhaAtual - 1, colunaAtual].paredeSul);
				linhaAtual--;
			}
            else if (direcao == Direcao.Sul && CelulaDisponivel (linhaAtual + 1, colunaAtual))
            {
				// sul
				DestruirParedeCasoExista (matrizCelulas [linhaAtual, colunaAtual].paredeSul);
				DestruirParedeCasoExista (matrizCelulas [linhaAtual + 1, colunaAtual].paredeNorte);
				linhaAtual++;
			}
            else if (direcao == Direcao.Leste && CelulaDisponivel (linhaAtual, colunaAtual + 1))
            {
                // leste
                DestruirParedeCasoExista(matrizCelulas [linhaAtual, colunaAtual].paredeLeste);
				DestruirParedeCasoExista (matrizCelulas [linhaAtual, colunaAtual + 1].paredeOeste);
				colunaAtual++;
			}
            else if (direcao == Direcao.Oeste && CelulaDisponivel (linhaAtual, colunaAtual - 1))
            {
				// oeste
				DestruirParedeCasoExista (matrizCelulas [linhaAtual, colunaAtual].paredeOeste);
				DestruirParedeCasoExista (matrizCelulas [linhaAtual, colunaAtual - 1].paredeLeste);
				colunaAtual--;
			}

			matrizCelulas [linhaAtual, colunaAtual].visitada = true;
		}
	}

	private void Hunt()
    {
		scanCompleto = true; // primeiro setamos como true, e depois tentamos provar que seja false em baixo

		for (int l = 0; l < linhas; l++)
        {
			for (int c = 0; c < colunas; c++)
            {
				if (!matrizCelulas [l, c].visitada && CelulaContemCelulaAdjacente(l,c))
                {
					scanCompleto = false; // continuar o scan
					linhaAtual = l;
					colunaAtual = c;

					DestruirParedeAdjacente (linhaAtual, colunaAtual);
					matrizCelulas [linhaAtual, colunaAtual].visitada = true;

					return; // sair
				}
			}
		}
	}


	private bool RotaDisponivel(int linha, int coluna)
    {
		int rotasDisponiveis = 0;

		if (linha > 0 && !matrizCelulas[linha-1,coluna].visitada)
        {
			rotasDisponiveis++;
		}

		if (linha < linhas - 1 && !matrizCelulas [linha + 1, coluna].visitada)
        {
			rotasDisponiveis++;
		}

		if (coluna > 0 && !matrizCelulas[linha,coluna-1].visitada)
        {
			rotasDisponiveis++;
		}

		if (coluna < colunas-1 && !matrizCelulas[linha,coluna+1].visitada)
        {
			rotasDisponiveis++;
		}

		return rotasDisponiveis > 0;
	}

	private bool CelulaDisponivel(int linha, int coluna)
    {
		if (linha >= 0 && linha < linhas && coluna >= 0 && coluna < colunas && !matrizCelulas [linha, coluna].visitada)
        {
			return true;
		}
        else
        {
			return false;
		}
	}

	private void DestruirParedeCasoExista(GameObject parede)
    {
		if (parede != null)
        {
			GameObject.Destroy (parede);
		}
	}

	private bool CelulaContemCelulaAdjacente(int linha, int coluna)
    {
		int celulasVisitadas = 0;

        // olhar 1 linha para cima(norte) se estivermos na linha 1 ou maior
        if (linha > 0 && matrizCelulas [linha - 1, coluna].visitada)
        {
			celulasVisitadas++;
		}

        // olhar uma linha abaixo(sul) se formos a penultima linha (ou menor)
        if (linha < (linhas-2) && matrizCelulas [linha + 1, coluna].visitada)
        {
			celulasVisitadas++;
		}

        // olhar uma linha a esquerda(oeste) se estivermos na coluna 1 ou maior
        if (coluna > 0 && matrizCelulas [linha, coluna - 1].visitada)
        {
			celulasVisitadas++;
		}

        // olhar uma linha à direita(leste) se formos a penultima coluna (ou menor)
        if (coluna < (colunas-2) && matrizCelulas [linha, coluna + 1].visitada)
        {
			celulasVisitadas++;
		}

        // retorna true se houver alguma célula visitada adjacente a esta
        return celulasVisitadas > 0;
	}

	private void DestruirParedeAdjacente(int linha, int coluna)
    {
		bool paredeDestruida = false;

		while (!paredeDestruida)
        {
			int direcao = GeradorSeed.GerarNovaSeed();

			if (direcao == Direcao.Norte && linha > 0 && matrizCelulas [linha - 1, coluna].visitada)
            {
				DestruirParedeCasoExista (matrizCelulas [linha, coluna].paredeNorte);
				DestruirParedeCasoExista (matrizCelulas [linha - 1, coluna].paredeSul);
				paredeDestruida = true;
			}
            else if (direcao == Direcao.Sul && linha < (linhas-2) && matrizCelulas [linha + 1, coluna].visitada)
            {
				DestruirParedeCasoExista (matrizCelulas [linha, coluna].paredeSul);
				DestruirParedeCasoExista (matrizCelulas [linha + 1, coluna].paredeNorte);
				paredeDestruida = true;
			}
            else if (direcao == Direcao.Leste && coluna > 0 && matrizCelulas [linha, coluna-1].visitada)
            {
				DestruirParedeCasoExista (matrizCelulas [linha, coluna].paredeOeste);
				DestruirParedeCasoExista (matrizCelulas [linha, coluna-1].paredeLeste);
				paredeDestruida = true;
			}
            else if (direcao == Direcao.Oeste && coluna < (colunas-2) && matrizCelulas [linha, coluna+1].visitada)
            {
				DestruirParedeCasoExista (matrizCelulas [linha, coluna].paredeLeste);
				DestruirParedeCasoExista (matrizCelulas [linha, coluna+1].paredeOeste);
				paredeDestruida = true;
			}
		}

	}

}

public class Direcao
{
    public const int Norte = 1;
    public const int Sul = 2;
    public const int Leste = 3;
    public const int Oeste = 4;
}

