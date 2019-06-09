using UnityEngine;
using System.Collections;

public class GeradorSeed
{
	public static int posAtual = 0;
	public const string seed = "123424123342421432233144441212334432121223344";

	public static int GerarNovaSeed()
    {
		string novaSeed = seed.Substring(posAtual++ % seed.Length, 1);

        //return int.Parse (novaSeed); //gerar aleatórios com a seed key, para que possa ser gerado o mesmo labirinto mais de uma vez quando a seed for a mesma
        return Random.Range(0, 5); //gerar sempre aleatório
	}
}
