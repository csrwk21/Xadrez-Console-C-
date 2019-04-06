using System;
using tabuleiro;
using Xadrez;

namespace Xadrez_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Tabuleiro tab = new Tabuleiro(8, 8);
                tab.colocarPeca(new Torre(tab, Cor.preto), new Posicao(0, 0));
                tab.colocarPeca(new Torre(tab, Cor.preto), new Posicao(1, 3));
                tab.colocarPeca(new Rei(tab, Cor.preto), new Posicao(0, 2));

                tab.colocarPeca(new Torre(tab, Cor.branca), new Posicao(3, 5));

                Tela.imprimirTabuleiro(tab);


                Console.ReadLine();
            }
            catch(TabuleiroException e)
            {
                Console.WriteLine(e.Message);
            }

            
        }
    }
}
