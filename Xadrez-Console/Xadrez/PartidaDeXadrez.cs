﻿using System.Collections.Generic;
using tabuleiro;

namespace Xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro tab { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;
        public bool xeque { get; private set; }

        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.branca;
            terminada = false;
            xeque = false;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public Peca executaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQteMovimentos();
            Peca pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);
            if (pecaCapturada != null)
            {
                capturadas.Add(pecaCapturada);
            }
            //#JogadaEspecial roque pequeno
            if(p is Rei && destino.coluna == origem.coluna + 2)
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T, destinoT);
            }
            //#JogadaEspecial roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQteMovimentos();
                tab.colocarPeca(T, destinoT);
            }
            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQteMovimentos();
            if (pecaCapturada != null)
            {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p, origem);

            //#JogadaEspecial roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2)
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQteMovimentos();
                tab.colocarPeca(T, origemT);
            }

            //#JogadaEspecial roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQteMovimentos();
                tab.colocarPeca(T, origemT);
            }
        }
        public void realizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = executaMovimento(origem, destino);

            if (estaEmXeque(jogadorAtual))
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em Xeque!");
            }

            if (estaEmXeque(adversaria(jogadorAtual)))
            {
                xeque = true;
            }
            if (testeXequeMate(adversaria(jogadorAtual)))
            {
                terminada = true;
            }
            else
            {
                xeque = false;
            }
            turno++;
            mudaJogador();
        }

        public void validarPosicaoDeOrigem(Posicao pos)
        {
            if (tab.peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            if (jogadorAtual != tab.peca(pos).cor)
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            }
            if (!tab.peca(pos).existeMovimentosPossiveis())
            {
                throw new TabuleiroException("Não há movimentos possíveis para a peça de origem escolhida!");
            }
        }

        public void validarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if (!tab.peca(origem).movimentoPossivel(destino))
            {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }
        private void mudaJogador()
        {
            if (jogadorAtual == Cor.branca)
            {
                jogadorAtual = Cor.preto;
            }
            else
            {
                jogadorAtual = Cor.branca;
            }
        }

        public HashSet<Peca> pecasCaputuradas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in capturadas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }

            return aux;
        }

        public HashSet<Peca> pecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in pecas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(pecasCaputuradas(cor));
            return aux;
        }

        private Cor adversaria(Cor cor)
        {
            if(cor == Cor.branca)
            {
                return Cor.preto;
            }
            else
            {
                return Cor.branca;
            }
        }

        private Peca rei(Cor cor)
        {
            foreach(Peca x in pecasEmJogo(cor))
            {
                if(x is Rei)
                {
                    return x;
                }
            }
            return null;
        }

        public bool estaEmXeque(Cor cor)
        {
            Peca R = rei(cor);

            if(R == null)
            {
                throw new TabuleiroException("Não tem rei da cor" + cor + " no tabuleiro!");
            }
            foreach(Peca x in pecasEmJogo(adversaria(cor)))
            {
                bool[,] mat = x.movimentosPossiveis();
                if (mat[R.posicao.linha, R.posicao.coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public  bool testeXequeMate(Cor cor)
        {
            if (!estaEmXeque(cor))
            {
                return false;
            }

            foreach(Peca x in pecasEmJogo(cor))
            {
                bool[,] mat = x.movimentosPossiveis();
                for(int i =0; i < tab.linhas; i++)
                {
                    for(int j = 0; j < tab.colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = executaMovimento(origem, destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        public void colocarNovaPeca(char coluna, int linha, Peca peca)
        {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas()
        {
            colocarNovaPeca('a', 1, new Torre(tab, Cor.branca));
            colocarNovaPeca('b', 1, new Cavalo(tab, Cor.branca));
            colocarNovaPeca('c', 1, new Bispo(tab, Cor.branca));
            colocarNovaPeca('d', 1, new Dama(tab, Cor.branca));
            colocarNovaPeca('e', 1, new Rei(tab, Cor.branca,this));
            colocarNovaPeca('f', 1, new Bispo(tab, Cor.branca));
            colocarNovaPeca('g', 1, new Cavalo(tab, Cor.branca));
            colocarNovaPeca('h', 1, new Torre(tab, Cor.branca));
            colocarNovaPeca('a', 2, new Peao(tab, Cor.branca, this));
            colocarNovaPeca('b', 2, new Peao(tab, Cor.branca, this));
            colocarNovaPeca('c', 2, new Peao(tab, Cor.branca, this));
            colocarNovaPeca('d', 2, new Peao(tab, Cor.branca, this));
            colocarNovaPeca('e', 2, new Peao(tab, Cor.branca, this));
            colocarNovaPeca('f', 2, new Peao(tab, Cor.branca, this));
            colocarNovaPeca('g', 2, new Peao(tab, Cor.branca, this));
            colocarNovaPeca('h', 2, new Peao(tab, Cor.branca, this));

            colocarNovaPeca('a', 8, new Torre(tab, Cor.preto));
            colocarNovaPeca('b', 8, new Cavalo(tab, Cor.preto));
            colocarNovaPeca('c', 8, new Bispo(tab, Cor.preto));
            colocarNovaPeca('d', 8, new Dama(tab, Cor.preto));
            colocarNovaPeca('e', 8, new Rei(tab, Cor.preto,this));
            colocarNovaPeca('f', 8, new Bispo(tab, Cor.preto));
            colocarNovaPeca('g', 8, new Cavalo(tab, Cor.preto));
            colocarNovaPeca('h', 8, new Torre(tab, Cor.preto));
            colocarNovaPeca('a', 7, new Peao(tab, Cor.preto, this));
            colocarNovaPeca('b', 7, new Peao(tab, Cor.preto, this));
            colocarNovaPeca('c', 7, new Peao(tab, Cor.preto, this));
            colocarNovaPeca('d', 7, new Peao(tab, Cor.preto, this));
            colocarNovaPeca('e', 7, new Peao(tab, Cor.preto, this));
            colocarNovaPeca('f', 7, new Peao(tab, Cor.preto, this));
            colocarNovaPeca('g', 7, new Peao(tab, Cor.preto, this));
            colocarNovaPeca('h', 7, new Peao(tab, Cor.preto, this));
        }
    }
}
