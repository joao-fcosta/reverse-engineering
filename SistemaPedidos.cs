using System;
using System.Collections.Generic;
using System.Linq;

public enum TipoCliente { VIP, PREMIUM, NORMAL, NOVO }
public enum FormaPagamento { CARTAO, BOLETO, PIX, DINHEIRO }
public enum CategoriaItem { NORMAL, ALIMENTO, IMPORTADO }

public class Cliente
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public TipoCliente Tipo { get; set; }
    public bool Bloqueado { get; set; }
}

public class ItemPedido
{
    public string Nome { get; set; }
    public CategoriaItem Categoria { get; set; }
    public int Quantidade { get; set; }
    public double PrecoUnitario { get; set; }
}

public class Pedido
{
    public int Id { get; set; }
    public Cliente Cliente { get; set; }
    public List<ItemPedido> Itens { get; set; }
    public string Cupom { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public string EnderecoEntrega { get; set; }
    public double PesoTotal { get; set; }
    public bool EntregaExpressa { get; set; }
    public string Pais { get; set; }
    public int Parcelas { get; set; }
}

public class ValidadorPedido
{
    public List<string> Validar(Pedido pedido)
    {
        var erros = new List<string>();

        if (pedido.Id <= 0)
            erros.Add("Pedido inválido");

        if (string.IsNullOrEmpty(pedido.Cliente?.Nome))
            erros.Add("Nome do cliente não informado");

        if (pedido.Cliente?.Bloqueado == true)
            erros.Add("Cliente bloqueado");

        if (pedido.Itens == null || pedido.Itens.Count == 0)
            erros.Add("Pedido sem itens");

        foreach (var item in pedido.Itens ?? new List<ItemPedido>())
        {
            if (item.Quantidade <= 0)
                erros.Add($"Quantidade inválida: {item.Nome}");

            if (item.PrecoUnitario < 0)
                erros.Add($"Preço inválido: {item.Nome}");
        }

        if (string.IsNullOrEmpty(pedido.EnderecoEntrega))
            erros.Add("Endereço não informado");

        return erros;
    }
}

public class CalculadoraPedido
{
    public double CalcularSubtotal(List<ItemPedido> itens)
    {
        double subtotal = 0;

        foreach (var item in itens)
        {
            subtotal += item.PrecoUnitario * item.Quantidade;

            if (item.Categoria == CategoriaItem.ALIMENTO)
                subtotal += 2;

            if (item.Categoria == CategoriaItem.IMPORTADO)
                subtotal += 5;
        }

        return subtotal;
    }

    public double CalcularDesconto(double subtotal, TipoCliente tipo)
    {
        return tipo switch
        {
            TipoCliente.VIP => subtotal * 0.15,
            TipoCliente.PREMIUM => subtotal * 0.10,
            TipoCliente.NORMAL => subtotal * 0.02,
            _ => 0
        };
    }

    public double AplicarCupom(string cupom, double subtotal, TipoCliente tipo, ref double frete)
    {
        double descontoExtra = 0;

        if (string.IsNullOrEmpty(cupom)) return 0;

        switch (cupom)
        {
            case "DESC10":
                descontoExtra = subtotal * 0.10;
                break;
            case "DESC20":
                descontoExtra = subtotal * 0.20;
                break;
            case "FRETEGRATIS":
                frete = 0;
                break;
            case "VIP50" when tipo == TipoCliente.VIP:
                descontoExtra = 50;
                break;
        }

        return descontoExtra;
    }

    public double CalcularFrete(string pais, double peso, bool expressa)
    {
        double frete;

        if (pais == "BR")
        {
            frete = peso <= 1 ? 10 :
                    peso <= 5 ? 25 :
                    peso <= 10 ? 40 : 70;

            if (expressa) frete += 30;
        }
        else
        {
            frete = peso <= 1 ? 50 :
                    peso <= 5 ? 80 : 120;

            if (expressa) frete += 70;
        }

        return frete;
    }

    public double CalcularJuros(FormaPagamento forma, int parcelas, double subtotal)
    {
        if (forma == FormaPagamento.CARTAO)
        {
            if (parcelas > 1 && parcelas <= 6)
                return subtotal * 0.02;

            if (parcelas > 6)
                return subtotal * 0.05;
        }

        return 0;
    }
}

public class PedidoService
{
    private readonly ValidadorPedido _validador = new();
    private readonly CalculadoraPedido _calc = new();

    public string Processar(Pedido pedido)
    {
        var erros = _validador.Validar(pedido);
        if (erros.Any())
            return string.Join("\n", erros);

        double subtotal = _calc.CalcularSubtotal(pedido.Itens);
        double frete = _calc.CalcularFrete(pedido.Pais, pedido.PesoTotal, pedido.EntregaExpressa);
        double desconto = _calc.CalcularDesconto(subtotal, pedido.Cliente.Tipo);

        desconto += _calc.AplicarCupom(pedido.Cupom, subtotal, pedido.Cliente.Tipo, ref frete);

        double juros = _calc.CalcularJuros(pedido.FormaPagamento, pedido.Parcelas, subtotal);

        double total = Math.Max(0, subtotal - desconto + frete + juros);

        return $"TOTAL_FINAL={total}";
    }
}
