# Parâmetros e Estrutura do Pedido

No modelo atual, os dados que antes eram passados como vários parâmetros foram organizados em objetos, principalmente na classe `Pedido`, tornando o código mais organizado e fácil de manter.

---

## Classe Pedido

Representa o pedido completo, centralizando todas as informações necessárias para processamento.

- **Id**  
  Identificador único do pedido  
  _Validação: deve ser maior que 0_

- **Cliente (objeto Cliente)**  
  Contém os dados do cliente

- **Itens (List<ItemPedido>)**  
  Lista de produtos do pedido  
  _Deve conter pelo menos 1 item_

- **Cupom**  
  Código para desconto ou benefício (ex: frete grátis)

- **FormaPagamento (enum)**  
  Define como o pedido será pago:  
  `CARTAO`, `BOLETO`, `PIX`, `DINHEIRO`

- **EnderecoEntrega**  
  Endereço onde o pedido será entregue  
  _Obrigatório_

- **PesoTotal**  
  Usado para cálculo do frete

- **EntregaExpressa**  
  Indica se há taxa adicional de entrega rápida

- **Pais**  
  Define regras de frete:  
  `BR` → nacional  
  Outros → internacional

- **Parcelas**  
  Quantidade de parcelas (usado para cálculo de juros no cartão)

---

## Classe Cliente

Representa o cliente do pedido.

- **Nome**  
  Nome completo do cliente  
  _Obrigatório_

- **Email**  
  Usado para comunicação (não obrigatório no processamento)

- **Tipo (TipoCliente)**  
  Define o nível do cliente:  
  `VIP`, `PREMIUM`, `NORMAL`, `NOVO`

- **Bloqueado**  
  Indica se o cliente pode realizar pedidos  
  _Se true, o pedido é inválido_

---

## Classe ItemPedido

Representa cada item dentro do pedido.

- **Nome**  
  Nome do produto

- **Categoria (CategoriaItem)**  
  Define regras adicionais de preço:  
  `NORMAL`, `ALIMENTO`, `IMPORTADO`

- **Quantidade**  
  Deve ser maior que 0

- **PrecoUnitario**  
  Deve ser maior ou igual a 0

---

## Validação do Pedido

A validação é responsabilidade da classe `ValidadorPedido`.

Regras principais:
- Pedido deve ter ID válido
- Cliente deve ter nome
- Cliente não pode estar bloqueado
- Pedido deve conter itens
- Itens devem ter quantidade e preço válidos
- Endereço de entrega é obrigatório

---

## Cálculos do Pedido

Realizados pela classe `CalculadoraPedido`.

### Subtotal
- Soma dos itens (preço × quantidade)
- Taxas adicionais:
  - `ALIMENTO` → +2
  - `IMPORTADO` → +5

### Desconto
Baseado no tipo do cliente:
- VIP → 15%
- PREMIUM → 10%
- NORMAL → 2%
- NOVO → sem desconto

### Cupom
Pode gerar:
- Desconto adicional (`DESC10`, `DESC20`)
- Frete grátis (`FRETEGRATIS`)
- Desconto fixo para VIP (`VIP50`)

### Frete
Calculado com base em:
- País (nacional ou internacional)
- Peso do pedido
- Entrega expressa (taxa extra)

### Juros
Aplicado apenas para pagamento com cartão:
- 2% → até 6 parcelas
- 5% → acima de 6 parcelas

---

## Processamento do Pedido

A classe `PedidoService` é responsável por orquestrar todo o fluxo:

1. Valida o pedido (`ValidadorPedido`)
2. Calcula subtotal
3. Calcula frete
4. Aplica desconto
5. Aplica cupom
6. Calcula juros
7. Calcula total final

---

## Observação

Essa estrutura melhora a organização do sistema, separando responsabilidades em diferentes classes:
- `Pedido` → dados
- `ValidadorPedido` → regras
- `CalculadoraPedido` → cálculos
- `PedidoService` → controle do fluxo