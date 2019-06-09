using UnityEngine;
using System.Collections;

public abstract class Labirinto
{

	protected Celula[,] matrizCelulas;
	protected int linhas, colunas;

	protected Labirinto(Celula[,] _matrizCelulas) : base()
    {
		this.matrizCelulas = _matrizCelulas;
		linhas = _matrizCelulas.GetLength(0);
		colunas = _matrizCelulas.GetLength(1);
	}

	public abstract void CriarLabirinto();
}
